
select r.ReferenceID,
	r.ReferenceTypeID,
	r.DataSourceID,
	r.ConsensusReferenceID,
	r.LinkStatus,
	r.MatchScore,
	rp.Value as Citation,
	r.ProviderRecordID,
	props.val as Properties
from provider.Reference r
left join provider.ReferenceProperty rp on rp.ReferenceID = r.ReferenceID and rp.ReferencePropertyTypeID = '7F835876-B459-4023-90E4-6C22646FBE07' --citation
cross apply
(
    select (SELECT distinct '[' + CONVERT(VARCHAR(38), rp.ReferencePropertyTypeID) + ':' + rp.Value + '],' AS [text()] 
    FROM provider.ReferenceProperty rp 
    where rp.ReferenceID = r.ReferenceID
    FOR XML PATH(''))
) props(val)
where r.DataSourceID = @dataSourceId or @dataSourceId is null

