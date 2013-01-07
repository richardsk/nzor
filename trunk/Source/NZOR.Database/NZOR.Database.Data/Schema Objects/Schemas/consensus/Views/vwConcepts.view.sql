CREATE VIEW 
	[consensus].[vwConcepts]

AS 

SELECT     
	consensus.Name.NameID, 
	consensus.Name.FullName, 
	consensus.Name.NameClassID, 
	consensus.Name.GoverningCode, 
	consensus.Name.AddedDate, 
	consensus.Concept.ConceptID, 
	consensus.Concept.AccordingToReferenceID, 
	accCitation.Value AS AccordingToReference,
	consensus.Concept.Orthography, 
	consensus.ConceptRelationship.ConceptRelationshipID, 
	consensus.ConceptRelationship.FromConceptID, 
	consensus.ConceptRelationship.ToConceptID, 
	consensus.ConceptRelationship.Sequence, 
	ConceptTo.ConceptID AS ConceptToID, 
	ConceptTo.NameID AS NameToID, 
	ConceptTo.AccordingToReferenceID AS ReferenceToID, 
	NameTo.FullName AS NameToFull, 
	consensus.Name.TaxonRankID, 
	NameTo.TaxonRankID AS TaxonRankToID, 
	dbo.ConceptRelationshipType.Relationship, 
	dbo.ConceptRelationshipType.ConceptRelationshipTypeID,
	ConceptRelationship.IsActive
FROM         
	consensus.Concept 
	INNER JOIN consensus.ConceptRelationship 
		ON consensus.Concept.ConceptID = consensus.ConceptRelationship.FromConceptID 
	INNER JOIN consensus.Name 
		ON consensus.Concept.NameID = consensus.Name.NameID 
	INNER JOIN consensus.Concept AS ConceptTo 
		ON consensus.ConceptRelationship.ToConceptID = ConceptTo.ConceptID 
	INNER JOIN consensus.Name AS NameTo 
		ON ConceptTo.NameID = NameTo.NameID 
	INNER JOIN dbo.ConceptRelationshipType 
		ON consensus.ConceptRelationship.ConceptRelationshipTypeID = dbo.ConceptRelationshipType.ConceptRelationshipTypeID
	LEFT OUTER JOIN consensus.ReferenceProperty accCitation 
		ON accCitation.ReferenceID = consensus.Concept.AccordingToReferenceID 
		AND accCitation.ReferencePropertyTypeID = '7F835876-B459-4023-90E4-6C22646FBE07' --citation