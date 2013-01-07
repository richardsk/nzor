
select distinct pc1.ConsensusNameID as NameID, 
	pc1.ConceptRelationshipTypeID, 
	pc1.DataSourceID as DataSource1, 
	(select ConsensusReferenceID from provider.Reference where ReferenceID = pc1.AccordingToReferenceID) as AccordingToID1,
	pc1.ConsensusNameToId as NameTo1,	 
	pc1.Sequence as Sequence1, 
	pc2.DataSourceID as DataSource2, 
	(select ConsensusReferenceID from provider.Reference where ReferenceID = pc2.AccordingToReferenceID) as AccordingToID2,
	pc2.ConsensusNameToId as NameTo2, 
	pc2.Sequence as Sequence2, 
	pc1.SortOrder
from provider.vwConcepts pc1
inner join provider.vwConcepts pc2 on pc2.ConsensusNameID = pc1.ConsensusNameID and pc2.ConceptRelationshipTypeID = pc1.ConceptRelationshipTypeID	
	and pc1.ConceptRelationshipID <> pc2.ConceptRelationshipID
inner join consensus.vwConcepts cc on cc.ConceptID = pc1.ConsensusConceptID
inner join consensus.vwConcepts cc2 on cc2.ConceptID = pc2.ConsensusConceptID
where pc1.InUse = 1 and pc2.InUse = 1 and cc.IsActive = 1 and cc2.IsActive = 1
order by pc1.SortOrder
