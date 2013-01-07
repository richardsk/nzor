
select nameid, fullname, linkstatus, MatchPath
from provider.Name 
where LinkStatus NOT IN ('unmatched', 'integrating','matched','inserted') and datasourceid = @datasourceId

select c.conceptid, n.fullname + ' according to ' + isnull(rp.Value ,'[null reference]') as [Text], c.linkstatus
from provider.Concept c
inner join provider.Name n on n.nameid = c.nameid
left join provider.Reference r on r.ReferenceId = c.accordingtoreferenceid
left join provider.referenceproperty rp on rp.referenceid = r.referenceid and rp.referencepropertytypeid = '7F835876-B459-4023-90E4-6C22646FBE07'
where c.LinkStatus NOT IN ('unmatched', 'integrating','matched','inserted') and c.datasourceid = @datasourceId

select r.referenceid, rp.Value as [Text], r.linkstatus
from provider.Reference r 
left join provider.referenceproperty rp on rp.referenceid = r.referenceid and rp.referencepropertytypeid = '7F835876-B459-4023-90E4-6C22646FBE07'
where r.LinkStatus NOT IN ('unmatched', 'integrating','matched','inserted') and r.datasourceid = @datasourceId
