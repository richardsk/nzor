INSERT INTO
	provider.Concept
	(
	ConceptID, 
	
	NameID, 
	AccordingToReferenceID, 
	DataSourceID,

	ConsensusConceptID, 
	IntegrationBatchId,
	LinkStatus, 
	MatchScore, 
	MatchPath,
	ConceptSourceType,

	ProviderNameID, 
	ProviderReferenceID, 
	ProviderRecordID, 
	ProviderCreatedDate, 
	ProviderModifiedDate, 

	Orthography, 
	TaxonRank,
	HigherClassification,

	AddedDate,
	ModifiedDate
	)
VALUES
	(
	@ConceptID, 
	
	@NameID, 
	@AccordingToReferenceID, 
	@DataSourceID, 

	@consensusConceptId, 
	@integrationBatchId,
	@linkStatus, 
	@matchScore, 
	@matchPath,
	@ConceptSourceType,

	@ProviderNameID, 
	@ProviderReferenceID, 
	@ProviderRecordID, 
	@ProviderCreatedDate, 
	@ProviderModifiedDate, 

	@Orthography, 
	@TaxonRank,
	@HigherClassification,

	@AddedDate,
	@ModifiedDate
	);	