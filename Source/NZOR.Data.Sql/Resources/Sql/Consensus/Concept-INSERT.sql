INSERT INTO
	consensus.Concept
	(
	ConceptID, 
	
	NameID, 
	AccordingToReferenceID, 
	
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
	
	@Orthography, 
	@TaxonRank,
	@HigherClassification,

	@AddedDate,
	@ModifiedDate
	);	