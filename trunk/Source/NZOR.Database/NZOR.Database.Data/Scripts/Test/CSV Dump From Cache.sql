use PlantName_Cache
go

set concat_null_yields_null off
go
select 	top 12000
	n.NameId,
	n.createddate as NameCreatedDate,
	n.modifieddate as NameModifiedDate,
	'"' + n.NameFull + '"' as FullName,
	n.[Rank],
	'"' + n.CanonicalName + '"' as CanonicalName,
	'"' + n.Authorship + '"' as Authorship,
	'"' + n.BasionymAuthors + '"' as BasionymAuthors,
	'"' + n.CombiningAuthors + '"' as CombiningAuthors,
	n.PublishedInId,
	'"' + n.PublishedIn + '"' as PublishedIn,
	'"' + n.[Year] + '"' as [Year],
	'"' + n.MicroReference + '"' as MicroReference,
	n.TypeNameId,
	'"' + n.ProtologueOrthography + '"' as Orthography,
	n.BasionymId,
	n.LaterHomonymOfId,
	'"' + n.BlockedName + '"' as BlockedName,
	'"' + n.RecombinedName + '"' as RecombinedName,
	'"' + n.NomenclaturalStatus + '"' as NomenclaturalStatus,
	n.NomenclaturalCode,
		
	p.PublicationID,
	p.[Type],
	p.CreatedDate as PublicationCreatedDate,
	p.ModifiedDate as PublicationModifiedDate,
	'"' + p.Citation + '"' as Citation,
	'"' + p.AuthorsSimple + '"' as AuthorsSimple,
	'"' + p.DateOfPublication + '"' as DateOfPublication,
	'"' + p.DateOnPublication + '"' as DateOnPublication,
	'"' + p.EditorsSimple + '"' as Editors,
	'"' + p.Volume + '"' as Volume,
	'"' + p.Issue + '"' as Issue,
	'"' + p.Edition + '"' as Edition,
	'"' + p.PageStart + '"' as PageStart, 
	'"' + p.PageEnd + '"' as PageEnd, 
	'"' + p.PageTotal + '"' as PageTotal,
	'"' + p.PublisherName + '"' as PublisherName,
	'"' + p.PublisherCity + '"' as PublisherCity,
	
	case when tc.taxonconceptid is null then c.createdDate else (select createddate from taxonconcept where taxonconceptid = tc.taxonconceptid) end as ConceptCreatedDate,
	case when tc.taxonconceptid is null then c.ModifiedDate else (select modifieddate from taxonconcept where taxonconceptid = tc.taxonconceptid) end as ConceptModifiedDate,
	case when tc.taxonconceptid is null then c.AccordingToId else (select accordingtoid from taxonconcept where taxonconceptid = tc.taxonconceptid) end as AccordingToId,
	case when tc.taxonconceptid is null then c.AcceptedNameId else (select nameid from taxonconcept where taxonconceptid = tc.acceptedconceptid) end as AcceptedNameId,
	case when tc.taxonconceptid is null then c.ParentNameId else (select nameid from taxonconcept where taxonconceptid = tc.parentconceptid) end as ParentNameId
	
	from Name n
	left join Publication p on p.publicationid = n.publishedinid
	left join namebasedconcept c on c.nameid = n.nameid
	left join taxonconcept tc on tc.nameid = n.nameid	
	inner join Compositae.dbo.tblRank r on r.rankname = n.rank
	order by r.ranksort
	