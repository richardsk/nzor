IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'sprSelect_NamesWithConcept')
	BEGIN
		DROP  Procedure  sprSelect_NamesWithConcept
	END

GO

CREATE Procedure sprSelect_NamesWithConcept
	@conceptType uniqueidentifier,
	@nameToId uniqueidentifier
AS

	select * 
	from vwConsensusConcepts
	where RelationshipTypeID = @conceptType
		and NameToID = @nameToId

GO


GRANT EXEC ON sprSelect_NamesWithConcept TO PUBLIC

GO


