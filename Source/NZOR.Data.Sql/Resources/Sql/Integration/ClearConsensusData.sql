
delete cr
from consensus.ConceptRelationship cr
inner join consensus.Concept c on c.ConceptID = cr.FromConceptID 
inner join consensus.Name n on n.NameID = c.NameID
where FullName <> '<FullName><Name>ROOT</Name></FullName>'

delete ca
from consensus.ConceptApplication ca
inner join consensus.Concept c on c.ConceptID = ca.FromConceptID 
inner join consensus.Name n on n.NameID = c.NameID
where FullName <> '<FullName><Name>ROOT</Name></FullName>'

delete c
from consensus.Concept c 
inner join consensus.Name n on n.NameID = c.NameID
where FullName <> '<FullName><Name>ROOT</Name></FullName>'

delete np 
from consensus.NameProperty np
inner join consensus.Name n on n.NameID = np.NameID
where FullName <> '<FullName><Name>ROOT</Name></FullName>'

delete consensus.TaxonPropertyValue 
delete consensus.TaxonProperty

delete consensus.Name where FullName <> '<FullName><Name>ROOT</Name></FullName>'

update provider.Name set ConsensusNameID = null, LinkStatus = 'Unmatched', MatchPath = null, MatchScore = null, IntegrationBatchID = null

delete consensus.StackedName

update provider.Concept set ConsensusConceptID = null, LinkStatus = 'Unmatched', MatchScore = null


delete consensus.ReferenceProperty
delete consensus.Reference

update provider.Reference set ConsensusReferenceID = null, LinkStatus = 'Unmatched', MatchScore = null

update provider.TaxonProperty set ConsensusTaxonPropertyID = null, LinkStatus = 'Unmatched', MatchScore = null
