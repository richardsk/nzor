IF EXISTS (SELECT * FROM sysobjects WHERE type = 'V' AND name = 'vwProviderConcepts')
	BEGIN
		DROP  View vwProviderConcepts
	END
GO

CREATE View vwProviderConcepts AS

SELECT     prov.Name.NameID, prov.Name.ConsensusNameID, prov.Name.FullName, prov.Name.NameClassID, prov.Name.LinkStatus, 
                      prov.Name.OriginalOrthography, prov.Name.GoverningCode, prov.Name.ProviderID, prov.Name.ProviderRecordID, prov.Name.ProviderUpdatedDate, 
                      prov.Name.AddedDate, prov.Concept.ConceptID, prov.Concept.AccordingToReferenceID, prov.Concept.Orthography, 
                      prov.ConceptRelationship.ConceptRelationshipID, prov.ConceptRelationship.FromConceptID, prov.ConceptRelationship.ToConceptID, 
                      prov.ConceptRelationship.RelationshipTypeID, dbo.ConceptRelationshipType.Relationship, prov.ConceptRelationship.Sequence, 
                      ConceptTo.ConceptID AS ConceptToID, ConceptTo.NameID AS NameToID, ConceptTo.AccordingToReferenceID AS ReferenceToID, 
                      NameTo.FullName AS NameToFull, NameTo.LinkStatus AS NameToLinkStatus, NameTo.ProviderRecordID AS NameToProviderRecordID, 
                      prov.Concept.ConsensusConceptID, ConceptTo.ConsensusConceptID AS ConsensusConceptToId, prov.Name.TaxonRankID, 
                      NameTo.TaxonRankID AS TaxonRankToID, dbo.TaxonRank.Name AS RankName, dbo.TaxonRank.SortOrder, 
                      NameTo.ConsensusNameID AS ConsensusNameToID
FROM         prov.Concept INNER JOIN
                      prov.ConceptRelationship ON prov.Concept.ConceptID = prov.ConceptRelationship.FromConceptID INNER JOIN
                      prov.Name ON prov.Concept.NameID = prov.Name.NameID INNER JOIN
                      prov.Concept AS ConceptTo ON prov.ConceptRelationship.ToConceptID = ConceptTo.ConceptID INNER JOIN
                      prov.Name AS NameTo ON ConceptTo.NameID = NameTo.NameID INNER JOIN
                      dbo.ConceptRelationshipType ON 
                      prov.ConceptRelationship.RelationshipTypeID = dbo.ConceptRelationshipType.ConceptRelationshipTypeID INNER JOIN
                      dbo.TaxonRank ON prov.Name.TaxonRankID = dbo.TaxonRank.TaxonRankID

GO


GRANT SELECT ON vwProviderConcepts TO PUBLIC

GO

