using System;
using System.Collections.Generic;
using System.Linq;
using NZOR.Admin.Data.Entities;
using NZOR.Matching.Batch.Helpers;
using NZOR.Publish.Data.Repositories;
using NZOR.Publish.Model.Search;
using NZOR.Publish.Model.Search.Names;
using NZOR.Data.LookUps.Common;
using NZOR.Data.Sql.Repositories.Common;
using NZOR.Admin.Data.Entities.Matching;

namespace NZOR.Matching.Batch.Matchers
{
    public class NameMatcher
    {
        private readonly string _searchIndexBaseFolderFullName;
        private NameRepository _nameRepository = null;

        List<ExternalLookupService> _externalHtmlLookupServices;
        TaxonRankLookUp _taxonRankLookup;

        private string _connectionString = "";

        public NameMatcher(string searchIndexBaseFolderFullName, string connectionString)
        {
            _searchIndexBaseFolderFullName = searchIndexBaseFolderFullName;
            _nameRepository = new NameRepository(_searchIndexBaseFolderFullName);
            
            if (!String.IsNullOrWhiteSpace(connectionString))
            {
                _connectionString = connectionString;

                LookUpRepository lookupRepository = new NZOR.Data.Sql.Repositories.Common.LookUpRepository(connectionString);
                _taxonRankLookup = new TaxonRankLookUp(lookupRepository.GetTaxonRanks());

                var externalLookupRepository = new Admin.Data.Sql.Repositories.ExternalLookupRepository(connectionString);
                _externalHtmlLookupServices = externalLookupRepository
                   .ListLookupServices()
                   .Where(o => o.DataFormat.Equals("HTML", StringComparison.OrdinalIgnoreCase))
                   .ToList();
            }
        }

        public List<NameMatchResult> Match(string inputData, int? numberOfRowsToProcess = null)
        {
            List<NameMatchResult> results = new List<NameMatchResult>();

            // Parse the input data.
            var inputDataParser = new InputDataParser();
            List<SubmittedName> submittedNames = inputDataParser.ParseInputData(inputData);

            if (numberOfRowsToProcess.HasValue)
            {
                submittedNames = submittedNames.Take(numberOfRowsToProcess.Value).ToList();
            }

            foreach (var submittedName in submittedNames)
            {
                NameMatchResult result = null;
                try
                {
                    NameMatchResult prevMatch = results.Where(m => m.SubmittedScientificName.ToLower() == submittedName.ScientificName.ToLower()).FirstOrDefault();
                    if (prevMatch != null)
                    {
                        result = new NameMatchResult();
                        result.NameMatches.AddRange(prevMatch.NameMatches);
                        result.SubmittedId = submittedName.Id;
                        result.SubmittedScientificName = submittedName.ScientificName;                        
                    }
                    else
                    {
                        result = MatchName(submittedName);
                    }
                }
                catch (Exception ex)
                {
                    result = new NameMatchResult();
                    result.Message = "Failed to match name (" + submittedName.Id + ":" + submittedName.ScientificName + ") : " + ex.Message + " : " + ex.StackTrace;                    
                }
                results.Add(result);
            }

            return results;
        }

        /// <summary>
        /// Applies the algorithms to determine the most likely matches for the submitted name.
        /// </summary>
        /// <param name="submittedName"></param>
        /// <returns></returns>
        private NameMatchResult MatchName(SubmittedName submittedName)
        {
            NameMatchResult result = new NameMatchResult();

            result.SubmittedId = submittedName.Id;
            result.SubmittedScientificName = submittedName.ScientificName;

            if (submittedName.ScientificName.Trim() == string.Empty) return result;

            string searchTerm = result.SubmittedScientificName.Replace("(", "").Replace(")", "").Replace("[","").Replace("]","");

            // Determine possible matches.
            List<NameSearchResult> matchResults = StackedNameMatch(searchTerm);

            if (matchResults == null || matchResults.Count == 0)
            {
                searchTerm += " ";
                searchTerm = searchTerm.Replace("ea ", "ae ");
                searchTerm = searchTerm.Replace("ea(", "ae(");
                searchTerm = searchTerm.Replace("ea-", "ae-");
                searchTerm = searchTerm.Replace("ii ", "i ");
                searchTerm = searchTerm.Replace("ii-", "i-");
                searchTerm = searchTerm.Replace(" sp. ", "");
                searchTerm = searchTerm.Replace(" spp. ", "");
                searchTerm = searchTerm.Replace("-", "");

                searchTerm = searchTerm.Trim();

                matchResults = StackedNameMatch(searchTerm);

                if (matchResults == null || matchResults.Count == 0)
                {
                    //fuzzier matching
                    matchResults = WildcardNameMatch(searchTerm, false);

                    if (matchResults == null || matchResults.Count == 0)
                    {
                        // try partial name
                        matchResults = WildcardNameMatch(searchTerm, true);
                    }
                }
            }

            // If there are results then populate the name matches for the submitted name
            if (matchResults != null && matchResults.Count > 0)
            {
                foreach (var nameResult in matchResults)
                {
                    var match = new NameMatch();
                    match.NzorId = nameResult.Name.NameId.ToString();
                    match.NzorFullName = nameResult.Name.FullName;
                    match.PartialName = nameResult.Name.PartialName;
                    match.Authors = nameResult.Name.Authors;
                    match.OriginalAuthors = nameResult.Name.BasionymAuthors;
                    match.Year = nameResult.Name.Year;
                    //todo match.OriginalYear = nameResult.Name.

                    string classif = "";
                    string classifRanks = "";
                    string classifIds = "";
                    for (int hi = nameResult.Name.ClassificationHierarchy.Count - 1; hi >= 0; hi--)
                    {
                        Publish.Model.Names.NameLink nl = nameResult.Name.ClassificationHierarchy[hi];
                        classif += nl.PartialName + "|";
                        classifRanks += nl.Rank + "|";
                        classifIds += nl.NameId + "|";
                    }
                    classif = classif.TrimEnd('|');
                    classifRanks = classifRanks.TrimEnd('|');
                    classifIds = classifIds.TrimEnd('|');

                    match.Classification = classif;
                    match.ClassificationRanks = classifRanks;
                    match.ClassificationIds = classifIds;

                    if (nameResult.Name.AcceptedName != null)
                    {
                        match.PreferredName = nameResult.Name.AcceptedName.FullName;
                        match.PreferredNameId = nameResult.Name.AcceptedName.NameId.ToString();
                    }

                    match.TaxonomicStatus = nameResult.Name.Status;
                    //todo - fix in DB
                    if (match.TaxonomicStatus == "Current") match.TaxonomicStatus = "Preferred";

                    match.NomenclaturalStatus = nameResult.Name.NomenclaturalStatus;
                    
                    if (nameResult.Name.Concepts != null)
                    {
                        foreach (Publish.Model.Concepts.Concept c in nameResult.Name.Concepts)
                        {
                            foreach (Publish.Model.Concepts.Relationship cr in c.ToRelationships)
                            {
                                if (c.Publication != null && c.Publication.Citation != null)
                                {
                                    if (cr.Type == ConceptRelationshipTypeLookUp.IsChildOf)
                                    {
                                        match.ParentAccordingTo += c.Publication.Citation + "|";
                                    }
                                    if (cr.Type == ConceptRelationshipTypeLookUp.IsSynonymOf)
                                    {
                                        match.PreferredAccordingTo += c.Publication.Citation + "|";
                                    }
                                }
                            }

                            if (c.ToApplications != null && c.ToApplications.Count > 0)
                            {
                                foreach (Publish.Model.Concepts.Application ca in c.ToApplications)
                                {
                                    match.VernacularNamesForScientific += ca.FromConcept.Name.FullName + "|";
                                    match.VernacularNamesForScientificIds += ca.FromConcept.Name.NameId.ToString() + "|";
                                }
                            }
                            if (c.FromApplications != null && c.FromApplications.Count > 0)
                            {
                                foreach (Publish.Model.Concepts.Application ca in c.FromApplications)
                                {
                                    match.ScientificNamesForVernacular += ca.ToConcept.Name.FullName + "|";
                                    match.ScientificNamesForVernacularIds += ca.ToConcept.Name.NameId.ToString() + "|";
                                }
                            }
                        }
                        match.ScientificNamesForVernacular = match.ScientificNamesForVernacular.TrimEnd('|');
                        match.ScientificNamesForVernacularIds = match.ScientificNamesForVernacularIds.TrimEnd('|');
                        match.VernacularNamesForScientific = match.VernacularNamesForScientific.TrimEnd('|');
                        match.VernacularNamesForScientificIds = match.VernacularNamesForScientificIds.TrimEnd('|');
                        match.ParentAccordingTo = match.ParentAccordingTo.TrimEnd('|');
                        match.PreferredAccordingTo = match.PreferredAccordingTo.TrimEnd('|');
                    }
                    
                    if (nameResult.Name.Biostatuses != null && nameResult.Name.Biostatuses.Count > 0)
                    {
                        Publish.Model.Common.Biostatus bs = nameResult.Name.Biostatuses[0];
                        match.Biostatus += string.Join(",", bs.Origin) + "|";                        
                        match.Biostatus += string.Join(",", bs.Occurrence) + "|";
                        match.Biostatus += string.Join(",", bs.EnvironmentalContext) + "|";
                        match.Biostatus += string.Join(",", bs.GeoRegion) + "|";
                        match.Biostatus += string.Join(",", bs.GeoSchema) + "|";
                        match.Biostatus += string.Join(",", bs.Biome);
                    }
                    
                    match.Score = nameResult.Score;

                    match.ExternalLookups = GetExternalLookups(nameResult.Name.FullName);

                    result.NameMatches.Add(match);
                }
            }

            return result;
        }

        private List<NameSearchResult> WildcardNameMatch(string nameText, bool doPartialNameSearch)
        {
            NamesSearchResponse response = null;
            
            string query = nameText;

            if (_nameRepository != null && !String.IsNullOrWhiteSpace(query))
            {
                // Try exact match first
                var request = new SearchRequest();

                request.Query = "\"" + query + "\"";
                response = _nameRepository.Search(request);

                if (response == null || !response.Results.Any())
                {
                    // Try fuzzier matching
                    request = new SearchRequest();

                    query = SearchHelper.BuildSearchString(query, true);
                    request.Query = query;

                    response = _nameRepository.Search(request);

                    if (!response.Results.Any() && doPartialNameSearch)
                    {
                        // Try with a partial name
                        request = new SearchRequest();

                        request.Query = "partialname: " + query;

                        response = _nameRepository.Search(request);
                        
                    }
                }
            }

            if (response == null) return null;
            return response.Results;
        }

        private List<NameSearchResult> StackedNameMatch(string nameText)
        {
            List<NameSearchResult> results = null;
            
            if (_taxonRankLookup != null)
            {
                // Standardise ranks
                NZOR.Data.Entities.Common.NameParseResult nameParseResult = NZOR.Matching.NameParser.ParseName(nameText, _taxonRankLookup);

                foreach (NZOR.Data.Entities.Common.NamePart namePart in nameParseResult.NameParts)
                {
                    NZOR.Data.Entities.Common.TaxonRank taxonRank = _taxonRankLookup.GetTaxonRank(namePart.Rank, null);
                    if (taxonRank != null)
                    {
                        namePart.Rank = taxonRank.Name;
                    }
                }

                System.Data.DataTable res = NZOR.Data.Sql.Matching.GetStackedNameMatches(_connectionString, nameParseResult, false, false);
                if (res == null || res.Rows.Count == 0)
                {
                    res = NZOR.Data.Sql.Matching.GetStackedNameMatches(_connectionString, nameParseResult, true, false);
                }

                //put results into a namesearch results structure
                if (res != null && res.Rows.Count > 0)
                {
                    results = new List<NameSearchResult>();          
                    foreach (System.Data.DataRow row in res.Rows)
                    {
                        NameSearchResult nsr = new NameSearchResult();
                        nsr.Name = _nameRepository.SingleOrDefault(new Guid(row["NameId"].ToString()));
                        nsr.Score = 1;
                        results.Add(nsr);
                    }
                }
            }

            return results;
        }

        private List<ExternalLookup> GetExternalLookups(string name)
        {
            List<ExternalLookup> externalLookups = new List<ExternalLookup>();

            if (_externalHtmlLookupServices != null && _taxonRankLookup != null)
            {
                NZOR.Data.Entities.Common.NameParseResult nameParseResult = NZOR.Matching.NameParser.ParseName(name, _taxonRankLookup);

                string nameWithoutAuthors = nameParseResult.GetFullName(_taxonRankLookup, false, null);

                foreach (var externalLookupService in _externalHtmlLookupServices)
                {
                    var externalLookup = new ExternalLookup();
                    string lookupName = nameWithoutAuthors.Replace(" ", externalLookupService.SpaceCharacterSubstitute);

                    externalLookup.OrganisationName = externalLookupService.Name;
                    externalLookup.Type = externalLookupService.DataFormat;
                    externalLookup.SearchUrl = externalLookupService.NameLookupEndpoint + lookupName;

                    externalLookups.Add(externalLookup);
                }
            }

            return externalLookups;
        }
    }
}
