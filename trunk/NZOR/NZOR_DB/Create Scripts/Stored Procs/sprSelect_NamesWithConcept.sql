IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'sprSelect_NamesWithConcept')
	BEGIN
		DROP  Procedure  sprSelect_NamesWithConcept
	END

GO

CREATE Procedure sprSelect_NamesWithConcept
	@conceptType nvarchar(200),
	@nameToId uniqueidentifier
AS

	/*select * 
	from vwConsensusConcepts
	where RelationshipTypeID = @conceptType
		and NameToID = @nameToId*/
				
	declare @ids table(id uniqueidentifier)
		
    insert @ids 
    select distinct n.NameID 
    from cons.Name n 
    inner join vwConsensusConcepts cc on cc.NameID = n.NameID 
    where Relationship =@conceptType and NameToID = @nameToID
    
    select n.* 
    from cons.Name n 
    inner join @ids i on i.id = n.NameID
    
    select np.*, ncp.PropertyName 
    from cons.NameProperty np 
    inner join @ids i on i.id = np.NameID 
    inner join dbo.NameClassProperty ncp on ncp.NameClassPropertyID = np.NameClassPropertyID
                    
	select c.* 
	from vwConsensusConcepts c 
	inner join @ids i on i.id = c.NameID

GO


GRANT EXEC ON sprSelect_NamesWithConcept TO PUBLIC

GO


