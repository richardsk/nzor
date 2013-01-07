use nzor_data
go

update pnp
	set relatedid = pr.ReferenceId
	from prov.Name pn
		inner join prov.NameProperty pnp on pn.NameId = pnp.NameId
			and pnp.NameClassPropertyId = 'DEDC63F0-FB2A-420B-9932-786B4347DA45'
		inner join prov.Reference pr on pnp.ProviderRelatedId = pr.ProviderRecordId
go


update cnp
	set RelatedId = pr.ConsensusReferenceID
	from  prov.Name pn
	inner join prov.NameProperty pnp on pn.NameId = pnp.NameId
		and pnp.NameClassPropertyId = 'DEDC63F0-FB2A-420B-9932-786B4347DA45'
	inner join prov.Reference pr on pnp.RelatedId = pr.ReferenceId
	inner join cons.NameProperty cnp on pn.ConsensusNameID = cnp.NameID
		and cnp.NameClassPropertyId = 'DEDC63F0-FB2A-420B-9932-786B4347DA45'
go


update pnp
	set relatedid = pn.NameId
	from prov.NameProperty pnp
	 inner join prov.Name pn on pnp.ProviderRelatedId = pn.ProviderRecordId
	where pnp.NameClassPropertyId = 'F496FBCC-8DA6-4CA1-9884-11BD9B5DF63B'
go

update cnp
set relatedid = pn2.ConsensusNameId
	from cons.NameProperty cnp
		inner join prov.Name pn on cnp.NameId = pn.ConsensusNameId
		inner join prov.NameProperty pnp on pn.NameId = pnp.NameId
			and pnp.NameClassPropertyId = 'F496FBCC-8DA6-4CA1-9884-11BD9B5DF63B'
		inner join prov.name pn2 on pnp.RelatedId = pn2.NameId
	where cnp.NameClassPropertyId = 'F496FBCC-8DA6-4CA1-9884-11BD9B5DF63B'
go