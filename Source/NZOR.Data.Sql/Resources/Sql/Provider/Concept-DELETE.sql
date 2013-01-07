
delete provider.ConceptRelationship 
where FromConceptId = @conceptId

delete provider.ConceptRelationship 
where ToConceptId = @conceptId

delete provider.ConceptApplication 
where FromConceptID = @conceptId

delete provider.ConceptApplication 
where ToConceptID = @conceptId

delete provider.Concept
where ConceptId = @conceptId
