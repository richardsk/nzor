IF EXISTS (SELECT * FROM sysobjects WHERE type = 'V' AND name = 'vwConsensusConcepts')
	BEGIN
		DROP  View vwConsensusConcepts
	END
GO

CREATE View vwConsensusConcepts AS

SELECT     consensus.Name.NameID, consensus.Name.FullName, consensus.Name.NameClassID, consensus.Name.GoverningCode, 
                      consensus.Name.AddedDate, consensus.Concept.ConceptID, consensus.Concept.AccordingToReferenceID, consensus.Concept.Orthography, 
                      consensus.ConceptRelationship.ConceptRelationshipID, consensus.ConceptRelationship.FromConceptID, consensus.ConceptRelationship.ToConceptID, 
                      consensus.ConceptRelationship.Sequence, ConceptTo.ConceptID AS ConceptToID, ConceptTo.NameID AS NameToID, 
                      ConceptTo.AccordingToReferenceID AS ReferenceToID, NameTo.FullName AS NameToFull, consensus.Name.TaxonRankID, 
                      NameTo.TaxonRankID AS TaxonRankToID, dbo.ConceptRelationshipType.Relationship, dbo.ConceptRelationshipType.ConceptRelationshipTypeID
FROM         consensus.Concept INNER JOIN
                      consensus.ConceptRelationship ON consensus.Concept.ConceptID = consensus.ConceptRelationship.FromConceptID INNER JOIN
                      consensus.Name ON consensus.Concept.NameID = consensus.Name.NameID INNER JOIN
                      consensus.Concept AS ConceptTo ON consensus.ConceptRelationship.ToConceptID = ConceptTo.ConceptID INNER JOIN
                      consensus.Name AS NameTo ON ConceptTo.NameID = NameTo.NameID INNER JOIN
                      dbo.ConceptRelationshipType ON consensus.ConceptRelationship.ConceptRelationshipTypeID = dbo.ConceptRelationshipType.ConceptRelationshipTypeID

GO


GRANT SELECT ON vwConsensusConcepts TO PUBLIC

GO

