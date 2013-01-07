select acn.NameID, pn.DataSourceID, pn.FullName, ds.Code, apd.Ranking, Depth
from consensus.Name cn
inner join consensus.StackedName sn on sn.SeedNameID = cn.NameID
inner join consensus.Name acn on acn.NameID = sn.NameID
inner join [admin].AttachmentPoint ap on ap.ConsensusNameID = acn.NameID
inner join [admin].AttachmentPointDataSource apd on apd.AttachmentPointID = ap.AttachmentPointID
inner join provider.Name pn on pn.DataSourceID = apd.DataSourceID and pn.ConsensusNameID = acn.NameID
inner join [admin].DataSource ds on ds.DataSourceID = apd.DataSourceID
where cn.NameID = @nameId
order by depth, apd.Ranking
