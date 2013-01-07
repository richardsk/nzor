
select r.ReferenceID,
	ReferenceTypeID,
	rp.Value as Citation,
	props.val as Properties
from consensus.Reference r
left join consensus.ReferenceProperty rp on rp.ReferenceID = r.ReferenceID and rp.ReferencePropertyTypeID = '7F835876-B459-4023-90E4-6C22646FBE07' --citation
cross apply
(
    select (SELECT distinct '[' + CONVERT(VARCHAR(38), rp.ReferencePropertyTypeID) + ':' + rp.Value + '],' AS [text()] 
    FROM consensus.ReferenceProperty rp 
    where rp.ReferenceID = r.ReferenceID
    FOR XML PATH(''))
) props(val)
