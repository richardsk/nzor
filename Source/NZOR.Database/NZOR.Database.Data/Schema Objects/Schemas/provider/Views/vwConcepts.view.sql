CREATE VIEW 
	[provider].[vwConcepts]
AS 

SELECT     
provider.Name.NameID, provider.Name.ConsensusNameID, provider.Name.FullName, provider.Name.NameClassID, provider.Name.LinkStatus, 
provider.Name.GoverningCode, provider.Name.DataSourceID, provider.Name.ProviderRecordID, provider.Name.ProviderModifiedDate, 
provider.Name.AddedDate, provider.Concept.ConceptID, provider.Concept.AccordingToReferenceID, provider.Concept.Orthography, 
provider.ConceptRelationship.ConceptRelationshipID, provider.ConceptRelationship.FromConceptID, provider.ConceptRelationship.ToConceptID, 
provider.ConceptRelationship.ConceptRelationshipTypeID, dbo.ConceptRelationshipType.Relationship, provider.ConceptRelationship.Sequence, 
ConceptTo.ConceptID AS ConceptToID, ConceptTo.NameID AS NameToID, ConceptTo.AccordingToReferenceID AS ReferenceToID, 
NameTo.FullName AS NameToFull, NameTo.LinkStatus AS NameToLinkStatus, NameTo.ProviderRecordID AS NameToProviderRecordID, 
provider.Concept.ConsensusConceptID, ConceptTo.ConsensusConceptID AS ConsensusConceptToId, provider.Name.TaxonRankID, 
NameTo.TaxonRankID AS TaxonRankToID, dbo.TaxonRank.Name AS RankName, dbo.TaxonRank.SortOrder, 
NameTo.ConsensusNameID AS ConsensusNameToID, provider.ConceptRelationship.InUse
FROM         
provider.Concept INNER JOIN
provider.ConceptRelationship ON provider.Concept.ConceptID = provider.ConceptRelationship.FromConceptID INNER JOIN
provider.Name ON provider.Concept.NameID = provider.Name.NameID INNER JOIN
provider.Concept AS ConceptTo ON provider.ConceptRelationship.ToConceptID = ConceptTo.ConceptID INNER JOIN
provider.Name AS NameTo ON ConceptTo.NameID = NameTo.NameID INNER JOIN
dbo.ConceptRelationshipType ON 
provider.ConceptRelationship.ConceptRelationshipTypeID = dbo.ConceptRelationshipType.ConceptRelationshipTypeID INNER JOIN
dbo.TaxonRank ON provider.Name.TaxonRankID = dbo.TaxonRank.TaxonRankID