--update provider records
update provider.Concept set ConsensusConceptID = null, LinkStatus = 'Unmatched', MatchPath = null, MatchScore = null, IntegrationBatchID = null
where ConsensusConceptID = @conceptId

if (@replacementId is null)
begin
	select @replacementId = ConceptId from consensus.Concept where NameID = @nameId and
		isnull(AccordingToReferenceID, '00000000-0000-0000-0000-000000000000') = isnull(@accordingToReferenceID, '00000000-0000-0000-0000-000000000000')
end

--insert deprecated records
INSERT INTO [dbo].[Deprecated]([DeprecatedId], [Table], [OldId], [NewId], [DeprecationDate])
VALUES (newid(), 'consensus.Concept', @conceptId, @replacementId, getdate())

--delete concept data

delete consensus.ConceptRelationship 
where FromConceptId = @conceptId

delete consensus.ConceptRelationship 
where ToConceptId = @conceptId

delete consensus.ConceptApplication 
where FromConceptID = @conceptId

delete consensus.ConceptApplication 
where ToConceptID = @conceptId

delete consensus.Concept
where ConceptId = @conceptId

