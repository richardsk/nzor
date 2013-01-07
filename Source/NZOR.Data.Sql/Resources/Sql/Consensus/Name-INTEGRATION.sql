select cn.NameID,
	cn.FullName,
	cn.NameClassID,
	nc.Name as NameClass,
	tr.TaxonRankID,
	tr.Name as TaxonRank,
	tr.SortOrder as TaxonRankSort,
	ap.Value as Authors,
	cn.GoverningCode,	
	cn.IsRecombination,
	cp.Value as Canonical, --canonical
	yp.Value as YearOfPublication, --year of pub
	bp.RelatedID as BasionymID, --basionym
	bp.Value as Basionym, 
	bap.Value as BasionymAuthors, --basionym authors
	cap.Value as CombinationAuthors, --comb authors
	mrp.Value as MicroReference, --micro ref
	pip.Value as PublishedIn, --published in
	(select top 1 pc.NameToID) as ParentID, --parent name
	ids.list as ParentIDsToRoot,
	(select top 1 pc.NameToFull) as Parent,
	(select top 1 prc.NameToID) as PreferredNameID, --pref name
	(select top 1 prc.NameToFull) as PreferredName
from consensus.Name cn                
left join dbo.TaxonRank tr on tr.TaxonRankID = cn.TaxonRankID
left join dbo.NameClass nc on nc.NameClassID = cn.NameClassID
left join consensus.NameProperty cp on cp.NameID = cn.NameID and cp.NamePropertyTypeID = '1F64E93C-7EE8-40D7-8681-52B56060D750'
left join consensus.NameProperty yp on yp.NameID = cn.NameID and yp.NamePropertyTypeID = 'C9E19D58-BD92-4F3C-96E8-DE7FA6DCD5F7'
left join consensus.NameProperty bp on bp.NameID = cn.NameID and bp.NamePropertyTypeID = 'F496FBCC-8DA6-4CA1-9884-11BD9B5DF63B'
left join consensus.NameProperty ap on ap.NameID = cn.NameID and ap.NamePropertyTypeID = '006D86A8-08A5-4C1A-BC08-C07B0225E01B'
left join consensus.NameProperty bap on bap.NameID = cn.NameID and bap.NamePropertyTypeID = '6272B3D0-C91B-4FD4-A714-662B10FA6E68'
left join consensus.NameProperty cap on cap.NameID = cn.NameID and cap.NamePropertyTypeID = '6196CDC4-BACB-4172-8186-14BA494621A7'
left join consensus.NameProperty mrp on mrp.NameID = cn.NameID and mrp.NamePropertyTypeID = '4A344D40-7448-49D6-956B-4392B33A749F'
left join consensus.NameProperty pip on pip.NameID = cn.NameID and pip.NamePropertyTypeID = 'DEDC63F0-FB2A-420B-9932-786B4347DA45'
left join (select NameID, NameToID, NameToFull from consensus.vwConcepts where ConceptRelationshipTypeID = '6A11B466-1907-446F-9229-D604579AA155' and IsActive = 1) pc 
	on pc.NameID = cn.NameID
left join (select NameID, NameToID, NameToFull from consensus.vwConcepts where ConceptRelationshipTypeID = '0CA79AB3-E213-4F51-88B9-4CE01F735A1D' and IsActive = 1) prc
	on prc.NameID = cn.NameId
CROSS APPLY 
( 
	SELECT '[' + CONVERT(VARCHAR(38), fn.NameID) + ':' + convert(varchar(38), fn.TaxonRankID) + '],' AS [text()]  --parent names [Parent Guid:Rank Guid],[Parent Guid:Rank Guid] ...
	FROM consensus.StackedName fn
	WHERE fn.SeedNameID = cn.NameID 
	FOR XML PATH('') 
) ids (list);