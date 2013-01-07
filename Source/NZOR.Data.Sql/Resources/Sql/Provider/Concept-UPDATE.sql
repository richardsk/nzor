UPDATE
	provider.Concept
SET
	NameID = @NameID, 
	AccordingToReferenceID = @AccordingToReferenceID, 
	DataSourceID = @DataSourceID,
	ConsensusConceptId = @consensusConceptId,
	IntegrationBatchId = @integrationBatchId,
	LinkStatus = @linkStatus,
	MatchScore = @matchScore,
	MatchPath = @matchPath,
	ProviderNameId = @providerNameId,
	ProviderReferenceId = @providerReferenceId,
	ProviderRecordId = @providerRecordId,
	ProviderCreatedDate = @providerCreatedDate,
	ProviderModifiedDate = @ProviderModifiedDate, 
	
	ConceptSourceType = @ConceptSourceType,

	Orthography = @Orthography, 
	TaxonRank = @TaxonRank,
	HigherClassification = @HigherClassification,

	ModifiedDate = @ModifiedDate
WHERE
	ConceptID = @ConceptID

