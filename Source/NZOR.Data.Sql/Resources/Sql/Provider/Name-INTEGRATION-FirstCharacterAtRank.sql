set concat_null_yields_null off

declare @concepts table(nameId uniqueidentifier, parentNames nvarchar(max), prefNames nvarchar(max))

insert @concepts

select pn.NameId, C.par, C.pref
from provider.Name pn
inner join dbo.TaxonRank tr on tr.TaxonRankID = pn.TaxonRankID
CROSS APPLY 
( 
    select (SELECT distinct '[' + CONVERT(VARCHAR(38), pcto.NameID) + ':' + convert(varchar(38), pnto.ConsensusNameID) + ':' + replace(REPLACE(pnto.FullName, '[', ''), ']', '') + '],' AS [text()] 
    FROM provider.Concept pc 
    inner join provider.ConceptRelationship pcr on pcr.FromConceptID = pc.ConceptID and pcr.ConceptRelationshipTypeID = '6A11B466-1907-446F-9229-D604579AA155' 
    inner join provider.Concept pcto on pcto.ConceptID = pcr.ToConceptID 
    inner join provider.Name pnto on pnto.NameID = pcto.NameID
    where pc.NameID = pn.NameID 
    FOR XML PATH('')),
    (SELECT distinct '[' + CONVERT(VARCHAR(38), pcto.NameID) + ':' + convert(varchar(38), pnto.ConsensusNameID) + ':' + replace(REPLACE(pnto.FullName, '[', ''), ']', '') + '],' AS [text()] 
    FROM provider.Concept pc 
        inner join provider.ConceptRelationship pcr on pcr.FromConceptID = pc.ConceptID and pcr.ConceptRelationshipTypeID = '0CA79AB3-E213-4F51-88B9-4CE01F735A1D' 
    inner join provider.Concept pcto on pcto.ConceptID = pcr.ToConceptID 
    inner join provider.Name pnto on pnto.NameID = pcto.NameID
    where pc.NameID = pn.NameID 
    FOR XML PATH(''))
) C (par, pref)
where pn.FullName like @character + '%' and tr.TaxonRankID = @rankId

select distinct pn.NameID,
    pn.ConsensusNameID,
    pn.LinkStatus,
    pn.MatchScore,
    pn.MatchPath,
    pn.FullName,
    pn.NameClassID,
    nc.Name as NameClass,
	nc.HasClassification,
    tr.TaxonRankID,
    tr.Name as TaxonRank,
    tr.SortOrder as TaxonRankSort,
    tr.MatchRuleSetID,
    ap.Value as Authors,
    pn.GoverningCode,	
	pn.IsRecombination,
    pn.DataSourceID,
    cp.Value as Canonical, --canonical
    yp.Value as YearOfPublication, --year on pub
    bp.RelatedID as BasionymID, --basionym
    bp.Value as Basionym, 
    bap.Value as BasionymAuthors, --basionym authors
    cap.Value as CombinationAuthors, --comb authors
    mrp.Value as MicroReference, --micro ref
    pip.Value as PublishedIn, --published in
    pc.parentNames as ParentNames,
    pc.prefNames as PreferredNames,
    pn.ProviderRecordId
from provider.Name pn
inner join dbo.TaxonRank tr on tr.TaxonRankID = pn.TaxonRankID
inner join dbo.NameClass nc on nc.NameClassID = pn.NameClassID
left join provider.NameProperty cp on cp.NameID = pn.NameID and cp.NamePropertyTypeID = '1F64E93C-7EE8-40D7-8681-52B56060D750'
left join provider.NameProperty yp on yp.NameID = pn.NameID and yp.NamePropertyTypeID = 'C9E19D58-BD92-4F3C-96E8-DE7FA6DCD5F7'
left join provider.NameProperty bp on bp.NameID = pn.NameID and bp.NamePropertyTypeID = 'F496FBCC-8DA6-4CA1-9884-11BD9B5DF63B'
left join provider.NameProperty ap on ap.NameID = pn.NameID and ap.NamePropertyTypeID = '006D86A8-08A5-4C1A-BC08-C07B0225E01B'
left join provider.NameProperty bap on bap.NameID = pn.NameID and bap.NamePropertyTypeID = '6272B3D0-C91B-4FD4-A714-662B10FA6E68'
left join provider.NameProperty cap on cap.NameID = pn.NameID and cap.NamePropertyTypeID = '6196CDC4-BACB-4172-8186-14BA494621A7'
left join provider.NameProperty mrp on mrp.NameID = pn.NameID and mrp.NamePropertyTypeID = '4A344D40-7448-49D6-956B-4392B33A749F'
left join provider.NameProperty pip on pip.NameID = pn.NameID and pip.NamePropertyTypeID = 'DEDC63F0-FB2A-420B-9932-786B4347DA45'
inner join @concepts pc on pc.nameId = pn.NameID
where (pn.DataSourceID = @dataSourceId or @dataSourceId is null) and pn.FullName like @character + '%' and tr.TaxonRankID = @rankId
order by nc.Name, tr.SortOrder; --order by name class to make sure scientific names come first :-)
