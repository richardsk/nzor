IF EXISTS (SELECT * FROM sysobjects WHERE type = 'V' AND name = 'vwConsensusConcepts')
	BEGIN
		DROP  View vwConsensusConcepts
	END
GO

CREATE View vwConsensusConcepts AS

SELECT     cons.Name.NameID, cons.Name.FullName, cons.Name.NameClassID, cons.Name.OriginalOrthography, cons.Name.GoverningCode, 
                      cons.Name.AddedDate, cons.Concept.ConceptID, cons.Concept.AccordingToReferenceID, cons.Concept.Orthography, 
                      cons.ConceptRelationship.ConceptRelationshipID, cons.ConceptRelationship.FromConceptID, cons.ConceptRelationship.ToConceptID, 
                      cons.ConceptRelationship.Sequence, ConceptTo.ConceptID AS ConceptToID, ConceptTo.NameID AS NameToID, 
                      ConceptTo.AccordingToReferenceID AS ReferenceToID, NameTo.FullName AS ConsensusNameToFull, cons.Name.TaxonRankID, 
                      NameTo.TaxonRankID AS TaxonRankToID, dbo.ConceptRelationshipType.Relationship, dbo.ConceptRelationshipType.ConceptRelationshipTypeID
FROM         cons.Concept INNER JOIN
                      cons.ConceptRelationship ON cons.Concept.ConceptID = cons.ConceptRelationship.FromConceptID INNER JOIN
                      cons.Name ON cons.Concept.NameID = cons.Name.NameID INNER JOIN
                      cons.Concept AS ConceptTo ON cons.ConceptRelationship.ToConceptID = ConceptTo.ConceptID INNER JOIN
                      cons.Name AS NameTo ON ConceptTo.NameID = NameTo.NameID INNER JOIN
                      dbo.ConceptRelationshipType ON cons.ConceptRelationship.ConceptRelationshipTypeID = dbo.ConceptRelationshipType.ConceptRelationshipTypeID

GO


GRANT SELECT ON vwConsensusConcepts TO PUBLIC

GO

