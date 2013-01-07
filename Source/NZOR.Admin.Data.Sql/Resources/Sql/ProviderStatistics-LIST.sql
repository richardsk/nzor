select distinct p.ProviderID, 
	d.DataSourceID,
	(select COUNT(nameid) from provider.Name where DataSourceID = d.DataSourceID) as ProviderNameCount,
	(select COUNT(conceptid) from provider.Concept where DataSourceID = d.DataSourceID) as ProviderConceptCount,
	(select COUNT(ReferenceID) from provider.Reference where DataSourceID = d.DataSourceID) as ProviderReferenceCount,
	(select COUNT(nameid) from provider.Name where DataSourceID = d.DataSourceID and ConsensusNameID is not null) as IntegratedNameCount,
	(select COUNT(conceptid) from provider.Concept where DataSourceID = d.DataSourceID and ConsensusConceptID is not null) as IntegratedConceptCount,
	(select COUNT(ReferenceID) from provider.Reference where DataSourceID = d.DataSourceID and ConsensusReferenceID is not null) as IntegratedReferenceCount,
	(select MAX(isnull(providermodifieddate, providercreateddate)) from provider.Name where DataSourceID = d.DataSourceID) as LastNameUpdatedDate,
	(select MAX(isnull(providermodifieddate, providercreateddate)) from provider.Concept where DataSourceID = d.DataSourceID) as LastConceptUpdatedDate,
	(select MAX(isnull(providermodifieddate, providercreateddate)) from provider.Reference where DataSourceID = d.DataSourceID) as LastReferenceUpdatedDate,
	dse.LastHarvestDate
from [admin].Provider p
inner join [admin].DataSource d on d.ProviderID = p.ProviderID 
left join  [admin].DataSourceEndpoint dse on dse.DataSourceId = d.DataSourceId