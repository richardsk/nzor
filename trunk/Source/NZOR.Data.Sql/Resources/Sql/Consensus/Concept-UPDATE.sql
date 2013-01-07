UPDATE
	consensus.Concept
SET
	NameID = @NameID, 
	AccordingToReferenceID = @AccordingToReferenceID, 
	
	Orthography = @Orthography, 
	TaxonRank = @TaxonRank,
	HigherClassification = @HigherClassification,

	ModifiedDate = @ModifiedDate
WHERE
	ConceptID = @ConceptID

