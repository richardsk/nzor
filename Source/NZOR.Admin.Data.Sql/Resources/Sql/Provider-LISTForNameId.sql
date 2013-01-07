SELECT distinct
	p.ProviderID, 
	p.Name,
	p.Code,
	p.Url,
	p.ContactEmail,
	p.Disclaimer,
	p.Attribution,
	p.Licensing,
	p.PublicStatement,
	p.AddedDate,
	p.AddedBy,
	p.ModifiedDate,
	p.ModifiedBy
FROM 
	[admin].Provider p
	inner join [admin].DataSource ds on ds.ProviderId = p.ProviderId
	inner join provider.Name pn on pn.DataSourceId = ds.DataSourceId
where pn.ConsensusNameId = @consensusNameId
