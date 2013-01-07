
delete cr
from provider.ConceptRelationship cr
inner join provider.Concept c on c.ConceptID = cr.FromConceptID
where DataSourceID = @dataSourceId

delete ca
from provider.ConceptApplication ca
inner join provider.Concept c on c.ConceptID = ca.FromConceptID
where DataSourceID = @dataSourceId

delete provider.Concept 
where DataSourceID = @dataSourceId

delete np
from provider.NameProperty np
inner join provider.Name n on n.NameID = np.NameID
where DataSourceID = @dataSourceId

delete tpv 
from provider.TaxonPropertyValue tpv
inner join provider.TaxonProperty tp on tp.TaxonPropertyID = tpv.TaxonPropertyID
where DataSourceID = @dataSourceId

delete provider.TaxonProperty
where DataSourceID = @dataSourceId

delete provider.Name 
where DataSourceID = @dataSourceId

delete sn
from provider.StackedName sn
inner join provider.Name n on n.NameID = sn.NameID
where DataSourceID = @dataSourceId

delete rp
from provider.ReferenceProperty rp
inner join provider.Reference r on r.ReferenceID = rp.ReferenceID
where DataSourceID = @dataSourceId

delete provider.Reference
where DataSourceID = @dataSourceId

