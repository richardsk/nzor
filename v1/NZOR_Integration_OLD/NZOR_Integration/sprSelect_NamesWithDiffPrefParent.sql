 IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'sprSelect_NamesWithDiffPrefParent')
	BEGIN
		DROP  Procedure  sprSelect_NamesWithDiffPrefParent
	END

GO

CREATE Procedure sprSelect_NamesWithDiffPrefParent
	@parentNameGuid uniqueidentifier
AS


	select distinct n.NameGUID, 
		N.NameFull, 
		n.NameParentFk as CurrentParentFk, 
		n.NameParent as CurrentParent, 
		pn.NameFull as PreferredName, 
		pn.NameParentFk as NewParentFk, 
		pn.NameParent as NewParent 
	from tblName n
	inner join tblName pn on pn.NameGUID = n.NamePreferredFk and pn.NameGUID <> n.NameGUID
	inner join vwProviderName pr on pr.PNNameFk = n.NameGUID
	left join vwProviderConceptRelationship pc on pc.ProviderPk = pr.ProviderPk and pc.PCName1Id = pr.PNNameId and pc.PCRRelationshipFk = 6
	where n.NameParentFk = @parentNameGuid and n.NameParentFk <> pn.NameParentFk and pc.PCPk is null
	order by n.NameFull

	
	
GO


GRANT EXEC ON sprSelect_NamesWithDiffPrefParent TO PUBLIC

GO


