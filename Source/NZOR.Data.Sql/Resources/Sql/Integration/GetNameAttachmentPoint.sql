
select distinct ap.ConsensusNameID, ds.Code, apd.Ranking, Depth, ap.FullName
from provider.Name pn
inner join provider.StackedName sn on sn.SeedNameID = pn.NameID
inner join provider.Name acn on acn.NameID = sn.NameID
inner join [admin].DataSource ds on ds.DataSourceID = acn.DataSourceID
inner join [admin].AttachmentPointDataSource apd on apd.DataSourceID = ds.DataSourceID 
inner join [admin].AttachmentPoint ap on ap.AttachmentPointID = apd.AttachmentPointID and ap.ConsensusNameID = acn.ConsensusNameID
where pn.NameID = @nameId
order by depth, apd.Ranking
