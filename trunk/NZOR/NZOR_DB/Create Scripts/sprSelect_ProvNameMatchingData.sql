IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'sprSelect_ProvNameMatchingData')
	BEGIN
		DROP  Procedure  sprSelect_ProvNameMatchingData
	END

GO

CREATE Procedure sprSelect_ProvNameMatchingData
	@provNameId uniqueidentifier
AS

	select * 
	from prov.Name pn
	inner join TaxonRank tr on tr.TaxonRankID = pn.TaxonRankID
	where NameID = @provNameId
	
	select * 
	from prov.NameProperty np
	inner join NameClassProperty ncp on ncp.NameClassPropertyID = np.NameClassPropertyID
	where NameID = @provNameId
	
	select * 
	from vwProviderConcepts
	where NameID = @provNameId
	

GO


GRANT EXEC ON sprSelect_ProvNameMatchingData TO PUBLIC

GO


