CREATE NONCLUSTERED INDEX idxConceptConsensusConceptID
ON [provider].[Concept] ([ConsensusConceptID])
INCLUDE ([ProviderCreatedDate],[ProviderModifiedDate])