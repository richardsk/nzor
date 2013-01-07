using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NZOR.Publish.Model.Common;
using NZOR.Publish.Model.Concepts;
using NZOR.Publish.Model.Names;
using NZOR.Publish.Model.Providers;
using NZOR.Publish.Model.References;
using NZOR.Publish.Publisher.Helpers;

namespace NZOR.Publish.Publisher.Builders
{
    class NamesBuilder
    {
        private readonly string _connectionString;

        SortedList<Guid, Provider> _providers;
        SortedList<Guid, Name> _names;
        SortedList<Guid, Concept> _concepts;
        SortedList<Guid, ReferenceLink> _referenceLinks;

        public NamesBuilder(string connectionString)
        {
            _connectionString = connectionString;

            _providers = new SortedList<Guid, Provider>();
            var providersBuilder = new ProvidersBuilder(connectionString);
            providersBuilder.Build();
            providersBuilder.Providers.ForEach(o => _providers.Add(o.ProviderId, o));

            _names = new SortedList<Guid, Name>(150000);
            _concepts = new SortedList<Guid, Concept>(400000);
            _referenceLinks = new SortedList<Guid, ReferenceLink>(20000);
        }

        public void Build()
        {
            Stopwatch stopwatch = new Stopwatch();

            LoadBaseReferenceDetails();
            LoadBaseNameDetails();

            UpdateBiostatuses();

            LoadBaseConceptDetails();

            LoadConceptRelationshipDetails();
            LoadConceptApplicationDetails();
            stopwatch.Restart();
            DetermineRelationships();
            stopwatch.Stop();
            long time = stopwatch.ElapsedMilliseconds;

            stopwatch.Restart();
            UpdateNameLinks();
            UpdateNameProperties();
            UpdateNameAnnotations();
            stopwatch.Stop();

            time = stopwatch.ElapsedMilliseconds;
            UpdateClassificationHierarchy();
            UpdateProviderNameLinks();
        }

        public List<Name> Names
        {
            get { return _names.Values.ToList(); }
        }

        public void DetermineRelationships()
        {
            Parallel.ForEach(_names.Values,
                name =>
                {
                    var parentRelationship = name.Concepts.SelectMany(o => o.FromRelationships).FirstOrDefault(o => o.IsActive && o.Type.Equals("is child of", StringComparison.OrdinalIgnoreCase));
                    if (parentRelationship != null) { name.ParentName = parentRelationship.ToConcept.Name; }

                    var acceptedRelationship = name.Concepts.SelectMany(o => o.FromRelationships).FirstOrDefault(o => o.IsActive && o.Type.Equals("is synonym of", StringComparison.OrdinalIgnoreCase));
                    if (acceptedRelationship != null)
                    {
                        name.AcceptedName = acceptedRelationship.ToConcept.Name;

                        if (name.NameId == name.AcceptedName.NameId)
                        {
                            name.Status = "Current";
                        }
                        else
                        {
                            name.Status = "Synonym";
                        }
                    }
                    else
                    {
                        name.Status = "Unknown";
                    }
                });
        }

        /// <summary>
        /// Biostatus records are grouped by TaxonPropertyId.
        /// </summary>
        private void UpdateBiostatuses()
        {
            string sql = @"

SELECT DISTINCT
	ISNULL(TaxonProperty.NameId, Concept.NameId) as NameId,

    TaxonProperty.TaxonPropertyId,
	TaxonPropertyType.Name AS Type,

	TaxonPropertyValue.Value
FROM
	consensus.TaxonProperty
	INNER JOIN consensus.TaxonPropertyValue		
		ON TaxonProperty.TaxonPropertyId = TaxonPropertyValue.TaxonPropertyId
	INNER JOIN dbo.TaxonPropertyType
		ON TaxonPropertyValue.TaxonPropertyTypeId = TaxonPropertyType.TaxonPropertyTypeId
	INNER JOIN dbo.TaxonPropertyClass		
		ON TaxonProperty.TaxonPropertyClassId = TaxonPropertyClass.TaxonPropertyClassId
	LEFT JOIN consensus.Concept on Concept.ConceptID = TaxonProperty.ConceptID
WHERE
	(TaxonProperty.NameId IS NOT NULL or Concept.NameID IS NOT NULL)
	AND TaxonPropertyClass.Name = N'Biostatus'
ORDER BY
	NameId,
    TaxonProperty.TaxonPropertyId

";

            using (var drd = DataAccess.ExecuteReader(_connectionString, sql))
            {
                Guid currentNameId = Guid.Empty;
                Guid currentTaxonPropertyId = Guid.Empty;
                Name name = null;
                Biostatus biostatus = null;

                while (drd.Read())
                {
                    if (drd.GetGuid("NameId") != currentNameId)
                    {
                        currentNameId = drd.GetGuid("NameId");
                        name = _names[currentNameId];

                        currentTaxonPropertyId = drd.GetGuid("TaxonPropertyId");

                        biostatus = new Biostatus();
                        name.Biostatuses.Add(biostatus);
                    }

                    if (drd.GetGuid("TaxonPropertyId") != currentTaxonPropertyId)
                    {
                        currentTaxonPropertyId = drd.GetGuid("TaxonPropertyId");

                        biostatus = new Biostatus();
                        name.Biostatuses.Add(biostatus);
                    }

                    if (name != null && biostatus != null)
                    {
                        switch (drd.GetString("Type"))
                        {
                            case "Biome":
                                biostatus.Biome.Add(drd.GetString("Value"));
                                break;
                            case "Environmental Context":
                                biostatus.EnvironmentalContext.Add(drd.GetString("Value"));
                                break;
                            case "Origin":
                                biostatus.Origin.Add(drd.GetString("Value"));
                                break;
                            case "Occurrence":
                                biostatus.Occurrence.Add(drd.GetString("Value"));
                                break;
                            case "GeoRegion":
                                biostatus.GeoRegion.Add(drd.GetString("Value"));
                                break;
                            case "GeoSchema":
                                biostatus.GeoSchema.Add(drd.GetString("Value"));
                                break;

                            default:
                                throw new Exception("Invalid taxon property type: " + drd.GetString("Type"));
                        }
                    }
                }
            }
        }

        private void UpdateClassificationHierarchy()
        {
            string sql = @"

SELECT
	SeedNameId,
	NameId
FROM
	consensus.StackedName
ORDER BY
	SeedNameId,
	Depth DESC
";

            using (var drd = DataAccess.ExecuteReader(_connectionString, sql))
            {
                Guid seedNameId = Guid.Empty;
                Guid currentNameId = Guid.Empty;
                Name name = null;

                while (drd.Read())
                {
                    seedNameId = drd.GetGuid("SeedNameId");

                    if (seedNameId != currentNameId)
                    {
                        currentNameId = seedNameId;
                        name = _names[currentNameId];
                    }

                    if (name != null)
                    {
                        name.ClassificationHierarchy.Add(GetNameLink(drd.GetGuid("NameId")));
                    }
                }
            }
        }

        private void UpdateProviderNameLinks()
        {
            string sql = @"

SELECT
	Name.ConsensusNameId AS ConsensusNameId,
	Name.NameId AS ProviderNameId,

	Name.FullName AS FullName,
    Name.ProviderRecordId,

	Provider.Code AS ProviderCode
FROM
	provider.Name
	INNER JOIN [admin].DataSource
		ON Name.DataSourceId = DataSource.DataSourceId
	INNER JOIN [admin].Provider
		ON DataSource.ProviderId = Provider.ProviderId
WHERE
	Name.ConsensusNameId IS NOT NULL

";

            using (var drd = DataAccess.ExecuteReader(_connectionString, sql))
            {
                while (drd.Read())
                {
                    if (_names.ContainsKey(drd.GetGuid("ConsensusNameId")))
                    {
                        Name name = _names[drd.GetGuid("ConsensusNameId")];

                        name.ProviderNames.Add(new ProviderNameLink
                        {
                            NameId = drd.GetGuid("ProviderNameId"),
                            FullName = drd.GetString("FullName"),
                            ProviderCode = drd.GetString("ProviderCode"),
                            ProviderRecordId = drd.GetString("ProviderRecordId")
                        });
                    }
                }
            }
        }

        private void LoadBaseNameDetails()
        {
            string sql = @"
SELECT
	Name.NameId,

	NameClass.Name AS Class,
	TaxonRank.DisplayName AS Rank,
	TaxonRank.SortOrder AS RankSortOrder,
    Name.IsRecombination,
	Name.GoverningCode,

	(
	SELECT
		TOP 1 Value
	FROM
		consensus.NameProperty 
		INNER JOIN dbo.NamePropertyType
			ON NameProperty.NamePropertyTypeId = NamePropertyType.NamePropertyTypeId
	WHERE
		NameProperty.NameId = Name.NameId
		AND NamePropertyType.NameClassId = Name.NameClassId 
		AND NamePropertyType.Name = N'NameText_FullName'
	) AS FullName,
	(
	SELECT
		TOP 1 Value
	FROM
		consensus.NameProperty 
		INNER JOIN dbo.NamePropertyType
			ON NameProperty.NamePropertyTypeId = NamePropertyType.NamePropertyTypeId
	WHERE
		NameProperty.NameId = Name.NameId
		AND NamePropertyType.NameClassId = Name.NameClassId 
		AND NamePropertyType.Name = N'NameText_FullNameFormatted'
	) AS FormattedFullName,
	(
	SELECT
		TOP 1 Value
	FROM
		consensus.NameProperty 
		INNER JOIN dbo.NamePropertyType
			ON NameProperty.NamePropertyTypeId = NamePropertyType.NamePropertyTypeId
	WHERE
		NameProperty.NameId = Name.NameId
		AND NamePropertyType.NameClassId = Name.NameClassId 
		AND NamePropertyType.Name = N'NameText_PartialName'
	) AS PartialName,
	(
	SELECT
		TOP 1 Value
	FROM
		consensus.NameProperty 
		INNER JOIN dbo.NamePropertyType
			ON NameProperty.NamePropertyTypeId = NamePropertyType.NamePropertyTypeId
	WHERE
		NameProperty.NameId = Name.NameId
		AND NamePropertyType.NameClassId = Name.NameClassId 
		AND NamePropertyType.Name = N'NameText_PartialNameFormatted'
	) AS FormattedPartialName,

    Name.AddedDate,
    COALESCE(Name.ModifiedDate, Name.AddedDate) AS ModifiedDate
FROM
	consensus.Name
	INNER JOIN dbo.TaxonRank
		ON Name.TaxonRankId = TaxonRank.TaxonRankId
	INNER JOIN dbo.NameClass
		ON Name.NameClassId = NameClass.NameClassId
";

            using (SqlDataReader drd = DataAccess.ExecuteReader(_connectionString, sql))
            {
                while (drd.Read())
                {
                    Name name = new Name();

                    name.NameId = drd.GetGuid("NameId");

                    name.Class = drd.GetString("Class");
                    name.Rank = drd.GetString("Rank");
                    name.RankSortOrder = drd.GetInt32("RankSortOrder");
                    name.GoverningCode = drd.GetString("GoverningCode").ToUpper();
                    if (!drd.IsDBNull("IsRecombination")) name.IsRecombination = drd.GetBoolean("IsRecombination");

                    name.FullName = CleanString(drd.GetString("FullName"));
                    name.FormattedFullName = CleanString(drd.GetString("FormattedFullName"));
                    name.PartialName = CleanString(drd.GetString("PartialName"));
                    name.FormattedPartialName = CleanString(drd.GetString("FormattedPartialName"));

                    name.AddedDate = drd.GetDateTime("AddedDate");
                    name.ModifiedDate = drd.GetDateTime("ModifiedDate");

                    if (name.Class.Equals("Vernacular Name", StringComparison.OrdinalIgnoreCase))
                    {
                        if (String.IsNullOrEmpty(name.FormattedFullName)) { name.FormattedFullName = name.FullName; }
                        if (String.IsNullOrEmpty(name.PartialName)) { name.PartialName = name.FullName; }
                        if (String.IsNullOrEmpty(name.FormattedPartialName)) { name.FormattedPartialName = name.PartialName; }
                    }

                    _names.Add(name.NameId, name);
                }
            }
        }

        private void UpdateNameAnnotations()
        {
            string sql = @"
    SELECT
        AnnotationId,
        NameId,
		AnnotationType,
        AnnotationText
	FROM
		consensus.Annotation
        ";

            using (SqlDataReader drd = DataAccess.ExecuteReader(_connectionString, sql))
            {
                while (drd.Read())
                {
                    Name name = _names[drd.GetGuid("NameId")];

                    if (name != null)
                    {
                        Annotation ann = new Annotation();
                        ann.AnnotationId = drd.GetGuid("AnnotationId");
                        ann.AnnotationType = drd.GetString("AnnotationType");
                        ann.AnnotationText = drd.GetString("AnnotationText");

                        name.Annotations.Add(ann);
                    }
                }
            }
        }

        private void UpdateNameProperties()
        {
            string sql = @"
SELECT
	Name.NameId,

	(
	SELECT
		TOP 1 Value
	FROM
		consensus.NameProperty 
		INNER JOIN dbo.NamePropertyType
			ON NameProperty.NamePropertyTypeId = NamePropertyType.NamePropertyTypeId
	WHERE
		NameProperty.NameId = Name.NameId
		AND NamePropertyType.NameClassId = Name.NameClassId 
		AND NamePropertyType.Name = N'PublishedIn'
	) AS PublishedIn,

	(
	SELECT
		TOP 1 Value
	FROM
		consensus.NameProperty 
		INNER JOIN dbo.NamePropertyType
			ON NameProperty.NamePropertyTypeId = NamePropertyType.NamePropertyTypeId
	WHERE
		NameProperty.NameId = Name.NameId
		AND NamePropertyType.NameClassId = Name.NameClassId 
		AND NamePropertyType.Name = N'Authors'
	) AS Authors,
	(
	SELECT
		TOP 1 Value
	FROM
		consensus.NameProperty 
		INNER JOIN dbo.NamePropertyType
			ON NameProperty.NamePropertyTypeId = NamePropertyType.NamePropertyTypeId
	WHERE
		NameProperty.NameId = Name.NameId
		AND NamePropertyType.NameClassId = Name.NameClassId 
		AND NamePropertyType.Name = N'BasionymAuthors'
	) AS BasionymAuthors,
	(
	SELECT
		TOP 1 Value
	FROM
		consensus.NameProperty 
		INNER JOIN dbo.NamePropertyType
			ON NameProperty.NamePropertyTypeId = NamePropertyType.NamePropertyTypeId
	WHERE
		NameProperty.NameId = Name.NameId
		AND NamePropertyType.NameClassId = Name.NameClassId 
		AND NamePropertyType.Name = N'CombinationAuthors'
	) AS CombinationAuthors,
	(
	SELECT
		TOP 1 Value
	FROM
		consensus.NameProperty 
		INNER JOIN dbo.NamePropertyType
			ON NameProperty.NamePropertyTypeId = NamePropertyType.NamePropertyTypeId
	WHERE
		NameProperty.NameId = Name.NameId
		AND NamePropertyType.NameClassId = Name.NameClassId 
		AND NamePropertyType.Name = N'Country'
	) AS Country,
	(
	SELECT
		TOP 1 Value
	FROM
		consensus.NameProperty 
		INNER JOIN dbo.NamePropertyType
			ON NameProperty.NamePropertyTypeId = NamePropertyType.NamePropertyTypeId
	WHERE
		NameProperty.NameId = Name.NameId
		AND NamePropertyType.NameClassId = Name.NameClassId 
		AND NamePropertyType.Name = N'CultivarNameGroup'
	) AS CultivarNameGroup,
	(
	SELECT
		TOP 1 Value
	FROM
		consensus.NameProperty 
		INNER JOIN dbo.NamePropertyType
			ON NameProperty.NamePropertyTypeId = NamePropertyType.NamePropertyTypeId
	WHERE
		NameProperty.NameId = Name.NameId
		AND NamePropertyType.NameClassId = Name.NameClassId 
		AND NamePropertyType.Name = N'InfragenericEpithet'
	) AS InfragenericEpithet,
	(
	SELECT
		TOP 1 Value
	FROM
		consensus.NameProperty 
		INNER JOIN dbo.NamePropertyType
			ON NameProperty.NamePropertyTypeId = NamePropertyType.NamePropertyTypeId
	WHERE
		NameProperty.NameId = Name.NameId
		AND NamePropertyType.NameClassId = Name.NameClassId 
		AND NamePropertyType.Name = N'InfraspecificEpithet'
	) AS InfraspecificEpithet,
	(
	SELECT
		TOP 1 Value
	FROM
		consensus.NameProperty 
		INNER JOIN dbo.NamePropertyType
			ON NameProperty.NamePropertyTypeId = NamePropertyType.NamePropertyTypeId
	WHERE
		NameProperty.NameId = Name.NameId
		AND NamePropertyType.NameClassId = Name.NameClassId 
		AND NamePropertyType.Name = N'Language'
	) AS Language,
	(
	SELECT
		TOP 1 Value
	FROM
		consensus.NameProperty 
		INNER JOIN dbo.NamePropertyType
			ON NameProperty.NamePropertyTypeId = NamePropertyType.NamePropertyTypeId
	WHERE
		NameProperty.NameId = Name.NameId
		AND NamePropertyType.NameClassId = Name.NameClassId 
		AND NamePropertyType.Name = N'MicroReference'
	) AS MicroReference,
	(
	SELECT
		ISNULL(Value + ', ','') 'text()' 
	FROM
		consensus.NameProperty 
		INNER JOIN dbo.NamePropertyType
			ON NameProperty.NamePropertyTypeId = NamePropertyType.NamePropertyTypeId
	WHERE
		NameProperty.NameId = Name.NameID 
        AND NamePropertyType.NameClassId = Name.NameClassId 
		AND NamePropertyType.Name = N'NomenclaturalStatus'
	FOR XML PATH('')
	) AS NomenclaturalStatus,
	(
    SELECT
		TOP 1 Value
	FROM
		consensus.NameProperty 
		INNER JOIN dbo.NamePropertyType
			ON NameProperty.NamePropertyTypeId = NamePropertyType.NamePropertyTypeId
	WHERE
		NameProperty.NameId = Name.NameId
		AND NamePropertyType.NameClassId = Name.NameClassId 
		AND NamePropertyType.Name = N'Orthography'
	) AS Orthography,
	(
	SELECT
		TOP 1 Value
	FROM
		consensus.NameProperty 
		INNER JOIN dbo.NamePropertyType
			ON NameProperty.NamePropertyTypeId = NamePropertyType.NamePropertyTypeId
	WHERE
		NameProperty.NameId = Name.NameId
		AND NamePropertyType.NameClassId = Name.NameClassId 
		AND NamePropertyType.Name = N'ProtologueOrthography'
	) AS ProtologueOrthography,
	(
	SELECT
		TOP 1 Value
	FROM
		consensus.NameProperty 
		INNER JOIN dbo.NamePropertyType
			ON NameProperty.NamePropertyTypeId = NamePropertyType.NamePropertyTypeId
	WHERE
		NameProperty.NameId = Name.NameId
		AND NamePropertyType.NameClassId = Name.NameClassId 
		AND NamePropertyType.Name = N'QualityCode'
	) AS QualityCode,
	(
	SELECT
		TOP 1 Value
	FROM
		consensus.NameProperty 
		INNER JOIN dbo.NamePropertyType
			ON NameProperty.NamePropertyTypeId = NamePropertyType.NamePropertyTypeId
	WHERE
		NameProperty.NameId = Name.NameId
		AND NamePropertyType.NameClassId = Name.NameClassId 
		AND NamePropertyType.Name = N'SpecificEpithet'
	) AS SpecificEpithet,
	(
	SELECT
		TOP 1 Value
	FROM
		consensus.NameProperty 
		INNER JOIN dbo.NamePropertyType
			ON NameProperty.NamePropertyTypeId = NamePropertyType.NamePropertyTypeId
	WHERE
		NameProperty.NameId = Name.NameId
		AND NamePropertyType.NameClassId = Name.NameClassId 
		AND NamePropertyType.Name = N'Uninomial'
	) AS Uninomial,
	(
	SELECT
		TOP 1 Value
	FROM
		consensus.NameProperty 
		INNER JOIN dbo.NamePropertyType
			ON NameProperty.NamePropertyTypeId = NamePropertyType.NamePropertyTypeId
	WHERE
		NameProperty.NameId = Name.NameId
		AND NamePropertyType.NameClassId = Name.NameClassId 
		AND NamePropertyType.Name = N'Year'
	) AS Year,
	(
	SELECT
		TOP 1 Value
	FROM
		consensus.NameProperty 
		INNER JOIN dbo.NamePropertyType
			ON NameProperty.NamePropertyTypeId = NamePropertyType.NamePropertyTypeId
	WHERE
		NameProperty.NameId = Name.NameId
		AND NamePropertyType.NameClassId = Name.NameClassId 
		AND NamePropertyType.Name = N'YearOnPublication'
	) AS YearOnPublication
FROM
	consensus.Name

";

            using (SqlDataReader drd = DataAccess.ExecuteReader(_connectionString, sql))
            {
                while (drd.Read())
                {
                    Name name = _names[drd.GetGuid("NameId")];

                    if (name != null)
                    {
                        if (!String.IsNullOrWhiteSpace(drd.GetString("PublishedIn")))
                        {
                            name.PublishedIn = new ReferenceLink { Citation = drd.GetString("PublishedIn") };
                        }

                        name.Authors = drd.GetString("Authors");
                        name.BasionymAuthors = drd.GetString("BasionymAuthors");
                        name.CombinationAuthors = drd.GetString("CombinationAuthors");
                        name.Country = drd.GetString("Country");
                        name.CultivarNameGroup = drd.GetString("CultivarNameGroup");
                        name.InfragenericEpithet = drd.GetString("InfragenericEpithet");
                        name.InfraspecificEpithet = drd.GetString("InfraspecificEpithet");
                        name.Language = drd.GetString("Language");
                        name.MicroReference = drd.GetString("MicroReference");
                        name.NomenclaturalStatus = drd.GetString("NomenclaturalStatus");                        
                        if (name.NomenclaturalStatus.EndsWith(", ")) name.NomenclaturalStatus = name.NomenclaturalStatus.Substring(0, name.NomenclaturalStatus.Length - 2);
                        name.Orthography = drd.GetString("Orthography");
                        name.ProtologueOrthography = drd.GetString("ProtologueOrthography");
                        name.QualityCode = drd.GetString("QualityCode");
                        name.SpecificEpithet = drd.GetString("SpecificEpithet");
                        name.Uninomial = drd.GetString("Uninomial");
                        name.Year = drd.GetString("Year");
                        name.YearOnPublication = drd.GetString("YearOnPublication");
                    }
                }
            }
        }

        private void UpdateNameLinks()
        {
            string sql = @"

SELECT 
	Name.NameId,

	(
	SELECT
		TOP 1 RelatedId
	FROM
		consensus.NameProperty 
		INNER JOIN dbo.NamePropertyType
			ON NameProperty.NamePropertyTypeId = NamePropertyType.NamePropertyTypeId
	WHERE
		NameProperty.NameId = Name.NameId
		AND NamePropertyType.NameClassId = Name.NameClassId 
		AND NamePropertyType.Name = N'Basionym'
	) AS BasionymNameId,
	(
	SELECT
		TOP 1 RelatedId
	FROM
		consensus.NameProperty 
		INNER JOIN dbo.NamePropertyType
			ON NameProperty.NamePropertyTypeId = NamePropertyType.NamePropertyTypeId
	WHERE
		NameProperty.NameId = Name.NameId
		AND NamePropertyType.NameClassId = Name.NameClassId 
		AND NamePropertyType.Name = N'BlockedName'
	) AS BlockedNameId,
	(
	SELECT
		TOP 1 RelatedId
	FROM
		consensus.NameProperty 
		INNER JOIN dbo.NamePropertyType
			ON NameProperty.NamePropertyTypeId = NamePropertyType.NamePropertyTypeId
	WHERE
		NameProperty.NameId = Name.NameId
		AND NamePropertyType.NameClassId = Name.NameClassId 
		AND NamePropertyType.Name = N'LaterHomonymOf'
	) AS LaterHomonymOfNameId,
	(
	SELECT
		TOP 1 RelatedId
	FROM
		consensus.NameProperty 
		INNER JOIN dbo.NamePropertyType
			ON NameProperty.NamePropertyTypeId = NamePropertyType.NamePropertyTypeId
	WHERE
		NameProperty.NameId = Name.NameId
		AND NamePropertyType.NameClassId = Name.NameClassId 
		AND NamePropertyType.Name = N'RecombinedName'
	) AS RecombinedNameId,
	(
	SELECT
		TOP 1 RelatedId
	FROM
		consensus.NameProperty 
		INNER JOIN dbo.NamePropertyType
			ON NameProperty.NamePropertyTypeId = NamePropertyType.NamePropertyTypeId
	WHERE
		NameProperty.NameId = Name.NameId
		AND NamePropertyType.NameClassId = Name.NameClassId 
		AND NamePropertyType.Name = N'TypeName'
	) AS TypeNameId
FROM
	consensus.Name

";

            using (SqlDataReader drd = DataAccess.ExecuteReader(_connectionString, sql))
            {
                while (drd.Read())
                {
                    Name name = _names[drd.GetGuid("NameId")];

                    if (name != null)
                    {
                        if (!drd.IsDBNull("BasionymNameId")) { name.BasionymName = GetNameLink(drd.GetGuid("BasionymNameId")); }
                        if (!drd.IsDBNull("BlockedNameId")) { name.BlockedName = GetNameLink(drd.GetGuid("BlockedNameId")); }
                        if (!drd.IsDBNull("LaterHomonymOfNameId")) { name.LaterHomonymOfName = GetNameLink(drd.GetGuid("LaterHomonymOfNameId")); }
                        if (!drd.IsDBNull("RecombinedNameId")) { name.RecombinedName = GetNameLink(drd.GetGuid("RecombinedNameId")); }
                        if (!drd.IsDBNull("TypeNameId")) { name.TypeName = GetNameLink(drd.GetGuid("TypeNameId")); }
                    }
                }
            }
        }

        private void LoadBaseConceptDetails()
        {
            string sql = @"

SELECT
	ConceptId,

	NameId,
	AccordingToReferenceId,

	Orthography,
	TaxonRank AS Rank,
	HigherClassification,

	COALESCE(AddedDate, GETDATE()) AS AddedDate,
	COALESCE(ModifiedDate, AddedDate, GETDATE()) AS ModifiedDate
FROM
	consensus.Concept
ORDER BY
    ConceptId

";

            using (var drd = DataAccess.ExecuteReader(_connectionString, sql))
            {
                while (drd.Read())
                {
                    var concept = new Concept();

                    concept.ConceptId = drd.GetGuid("ConceptId");

                    if (!drd.IsDBNull("NameId"))
                    {
                        var nameId = drd.GetGuid("NameId");

                        concept.Name = GetNameLink(nameId);

                        if (_names.ContainsKey(nameId)) { _names[nameId].Concepts.Add(concept); }
                    }

                    if (!drd.IsDBNull("AccordingToReferenceId")) { concept.Publication = GetReferenceLink(drd.GetGuid("AccordingToReferenceId")); }

                    concept.Orthography = drd.GetString("Orthography");
                    concept.Rank = drd.GetString("Rank");
                    concept.HigherClassification = drd.GetString("HigherClassification");

                    concept.AddedDate = drd.GetDateTime("AddedDate");
                    concept.ModifiedDate = drd.GetDateTime("ModifiedDate");

                    _concepts.Add(concept.ConceptId, concept);
                }
            }
        }

        private void LoadConceptRelationshipDetails()
        {
            string sql = @"
SELECT DISTINCT
	FromConcept.ConsensusConceptId AS FromConceptId,
	ToConcept.ConsensusConceptId AS ToConceptId,

	ConceptRelationshipType.Relationship AS Type,

	Provider.ProviderId
FROM
	provider.Concept FromConcept
	INNER JOIN provider.ConceptRelationship
		ON FromConcept.ConceptId = ConceptRelationship.FromConceptId
	INNER JOIN provider.Concept ToConcept
		ON ToConcept.ConceptId = ConceptRelationship.ToConceptId
	INNER JOIN dbo.ConceptRelationshipType
		ON ConceptRelationship.ConceptRelationshipTypeId = ConceptRelationshipType.ConceptRelationshipTypeId
	INNER JOIN [admin].DataSource
		ON FromConcept.DataSourceId = DataSource.DataSourceId
	INNER JOIN [admin].Provider
		ON DataSource.ProviderId = Provider.ProviderId
WHERE
	ConceptRelationship.InUse = 1
	AND FromConcept.ConsensusConceptId IS NOT NULL
	AND ToConcept.ConsensusConceptId IS NOT NULL

";
            var relationshipProviders = new SortedList<string, List<ProviderLink>>();

            using (var drd = DataAccess.ExecuteReader(_connectionString, sql))
            {
                while (drd.Read())
                {
                    string key = drd.GetGuid("FromConceptId").ToString() + drd.GetGuid("ToConceptId").ToString() + drd.GetString("Type");

                    if (relationshipProviders.ContainsKey(key))
                    {
                        relationshipProviders[key].Add(GetProviderLink(drd.GetGuid("ProviderId")));
                    }
                    else
                    {
                        relationshipProviders.Add(key, new List<ProviderLink> { GetProviderLink(drd.GetGuid("ProviderId")) });
                    }
                    //relationshipProviders.Add(new RelationshipProvider
                    //    {
                    //        FromConceptId = drd.GetGuid("FromConceptId"),
                    //        ToConceptId = drd.GetGuid("ToConceptId"),
                    //        Type = drd.GetString("Type"),
                    //        ProviderId = drd.GetGuid("ProviderId")
                    //    });
                }
            }

            sql = @"

SELECT
	FromConceptId,
	ToConceptId,
	
	ConceptRelationshipType.Relationship AS Type,
	IsActive,
	
	AddedDate,
	COALESCE(ModifiedDate, AddedDate) AS ModifiedDate
FROM
	consensus.ConceptRelationship
	INNER JOIN dbo.ConceptRelationshipType
		ON ConceptRelationship.ConceptRelationshipTypeId = ConceptRelationshipType.ConceptRelationshipTypeId

";

            using (var drd = DataAccess.ExecuteReader(_connectionString, sql))
            {
                while (drd.Read())
                {
                    var relationship = new Relationship();
                    string key = String.Empty;

                    if (!drd.IsDBNull("FromConceptId"))
                    {
                        Guid conceptId = drd.GetGuid("FromConceptId");
                        key = conceptId.ToString();

                        relationship.FromConcept = GetConceptLink(conceptId);

                        if (_concepts.ContainsKey(conceptId)) { _concepts[conceptId].FromRelationships.Add(relationship); }
                    }
                    if (!drd.IsDBNull("ToConceptId"))
                    {
                        Guid conceptId = drd.GetGuid("ToConceptId");
                        key += conceptId.ToString();

                        relationship.ToConcept = GetConceptLink(conceptId);

                        if (_concepts.ContainsKey(conceptId)) { _concepts[conceptId].ToRelationships.Add(relationship); }
                    }

                    relationship.Type = drd.GetString("Type");
                    key += relationship.Type;
                    relationship.IsActive = drd.GetBoolean("IsActive");

                    if (relationshipProviders.ContainsKey(key))
                    {
                        relationship.InUseByProviders = relationshipProviders[key];
                    }

                    //foreach (var relationshipProvider in relationshipProviders.Where(o => o.FromConceptId == relationship.FromConcept.ConceptId && o.ToConceptId == relationship.ToConcept.ConceptId && o.Type == relationship.Type))
                    //{
                    //    relationship.InUseByProviders.Add(new ProviderLink { ProviderId = relationshipProvider.ProviderId });
                    //}
                }
            }
        }

        private void LoadConceptApplicationDetails()
        {
            string sql = @"

SELECT
	FromConceptId,
	ToConceptId,
	
	ConceptApplicationType.Name AS Type,
	
	Gender,
	PartOfTaxon,
	LifeStage,
	
	AddedDate,
	COALESCE(ModifiedDate, AddedDate) AS ModifiedDate
FROM
	consensus.ConceptApplication
	INNER JOIN dbo.ConceptApplicationType
		ON ConceptApplication.ConceptApplicationTypeId = ConceptApplicationType.ConceptApplicationTypeId

";

            using (var drd = DataAccess.ExecuteReader(_connectionString, sql))
            {
                while (drd.Read())
                {
                    var application = new Application();

                    if (!drd.IsDBNull("FromConceptId"))
                    {
                        Guid conceptId = drd.GetGuid("FromConceptId");

                        application.FromConcept = GetConceptLink(conceptId);

                        if (_concepts.ContainsKey(conceptId)) { _concepts[conceptId].FromApplications.Add(application); }
                    }
                    if (!drd.IsDBNull("ToConceptId"))
                    {
                        Guid conceptId = drd.GetGuid("ToConceptId");

                        application.ToConcept = GetConceptLink(conceptId);

                        if (_concepts.ContainsKey(conceptId)) { _concepts[conceptId].ToApplications.Add(application); }
                    }

                    application.Type = drd.GetString("Type");
                    application.Gender = drd.GetString("Gender");
                    application.PartOfTaxon = drd.GetString("PartOfTaxon");
                    application.LifeStage = drd.GetString("LifeStage");
                }
            }
        }

        private void LoadBaseReferenceDetails()
        {
            string sql = @"

SELECT
	Reference.ReferenceId,

	ReferenceType.Name AS Type,
	(
	SELECT
		TOP 1 Value
	FROM
		consensus.ReferenceProperty 
		INNER JOIN dbo.ReferencePropertyType
			ON ReferenceProperty.ReferencePropertyTypeId = ReferencePropertyType.ReferencePropertyTypeId
	WHERE
		ReferenceProperty.ReferenceId = Reference.ReferenceId
		AND ReferencePropertyType.Name = N'Citation'
	) AS Citation
FROM
	consensus.Reference
	INNER JOIN dbo.ReferenceType
		ON Reference.ReferenceTypeId = ReferenceType.ReferenceTypeId

";

            using (var drd = DataAccess.ExecuteReader(_connectionString, sql))
            {
                while (drd.Read())
                {
                    var referenceLink = new ReferenceLink();

                    referenceLink.ReferenceId = drd.GetGuid("ReferenceId");

                    referenceLink.Type = drd.GetString("Type");
                    referenceLink.Citation = drd.GetString("Citation");

                    _referenceLinks.Add(referenceLink.ReferenceId, referenceLink);
                }
            }
        }

        /// <summary>
        /// Replace any special characters in the input string with standard characters.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private string CleanString(string input)
        {
            string value = Regex.Replace(input, @"\u00A0", " ");

            return value;
        }

        private NameLink GetNameLink(Guid nameId)
        {
            if (_names.ContainsKey(nameId))
            {
                Name name = _names[nameId];

                return new NameLink
                {
                    NameId = nameId,
                    FullName = name.FullName,
                    PartialName = name.PartialName,
                    Rank = name.Rank
                };
            }
            else
            {
                return null;
            }
        }

        private ConceptLink GetConceptLink(Guid conceptId)
        {
            if (_concepts.ContainsKey(conceptId))
            {
                Concept concept = _concepts[conceptId];

                return new ConceptLink
                {
                    ConceptId = conceptId,
                    Name = concept.Name,
                    Publication = concept.Publication
                };
            }
            else
            {
                return null;
            }
        }

        private ProviderLink GetProviderLink(Guid providerId)
        {
            if (_providers.ContainsKey(providerId))
            {
                var provider = _providers[providerId];

                return new ProviderLink
                {
                    ProviderId = providerId,
                    Code = provider.Code,
                    Name = provider.Name
                };
            }
            else
            {
                return null;
            }
        }

        private ReferenceLink GetReferenceLink(Guid referenceId)
        {
            if (_referenceLinks.ContainsKey(referenceId))
            {
                return _referenceLinks[referenceId];
            }
            else
            {
                return null;
            }
        }
    }
}
