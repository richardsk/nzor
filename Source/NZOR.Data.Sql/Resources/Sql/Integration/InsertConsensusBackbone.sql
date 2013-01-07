--works with empty consensus tables - inserts backbone data, ie:
-- distinct reference records
-- distinct names based on canonical, authors
-- distinct taxon properties and concept applications

declare @runDate datetime
set @runDate = getdate()
 
--references

create table #refs(citation nvarchar(max), refType uniqueidentifier, id uniqueidentifier, isNew bit)

--insert existing references
insert #refs(id, citation, refType, isNew)
select r.ReferenceID, Value, r.ReferenceTypeID, 0
from consensus.ReferenceProperty rp
inner join consensus.Reference r on r.ReferenceID = rp.ReferenceID
where ReferencePropertyTypeID = '7F835876-B459-4023-90E4-6C22646FBE07'

insert #refs(citation, refType, isNew)
select distinct Value, r.ReferenceTypeID, 1
from provider.ReferenceProperty rp
inner join provider.Reference r on r.ReferenceID = rp.ReferenceID
left join #refs er on er.citation = Value collate Latin1_General_CI_AS and er.refType = r.ReferenceTypeID
where ReferencePropertyTypeID = '7F835876-B459-4023-90E4-6C22646FBE07'
	and r.ConsensusReferenceID is null and er.citation is null
	
update #refs
set id = NEWID()
where id is null

insert consensus.Reference
select id, refType, GETDATE(), null
from #refs
where isNew = 1

insert consensus.ReferenceProperty(ReferenceID, ReferencePropertyTypeID, Value)
select distinct id, rp.ReferencePropertyTypeID, rp.Value collate Latin1_General_CI_AS
from provider.ReferenceProperty rp
inner join provider.ReferenceProperty rp2 on rp2.ReferencePropertyTypeID = '7F835876-B459-4023-90E4-6C22646FBE07' 
	and rp2.ReferenceID = rp.ReferenceID
inner join provider.Reference pr on pr.ReferenceID = rp.ReferenceID
inner join #refs r on r.citation = rp2.Value collate Latin1_General_CI_AS and r.refType = pr.ReferenceTypeID
where r.isNew = 1


update pr 
set pr.ConsensusReferenceID = r.id,
	pr.LinkStatus = 'Matched',
	pr.MatchScore = 100,
	pr.ModifiedDate = GETDATE()
from provider.Reference pr
inner join provider.ReferenceProperty rp on pr.ReferenceID = rp.ReferenceID 
	and ReferencePropertyTypeID = '7F835876-B459-4023-90E4-6C22646FBE07'
inner join #refs r on r.citation = rp.Value collate Latin1_General_CI_AS and r.refType = pr.ReferenceTypeID
where pr.ConsensusReferenceID is null


--names
create table #names(class uniqueidentifier, rank uniqueidentifier, canonical nvarchar(500), authors nvarchar(500), year int, genus nvarchar(500), species nvarchar(500), nomcode nvarchar(10), sortorder int, id uniqueidentifier, isNew bit)

create index idx_canonical on #names(canonical)
create index idx_authors on #names(authors)
create index idx_genus on #names(genus)
create index idx_species on #names(species)

--existing names
insert #names
select n.nameclassid, n.TaxonRankID as Rank, cnp.Value as Canonical, 
	case when charindex(' ex ', anp.Value) <> 0 
		then replace(replace(replace(replace(replace(SUBSTRING(anp.value, CHARINDEX(' ex ',anp.Value) + 4, LEN(anp.value)), ' and ', ' & '), ' et ', ' & '), '.', ' '), ',',' '), '  ', ' ')
	else
		replace(replace(replace(replace(replace(anp.value, ' and ', ' & '), ' et ', ' & '), '.', ' '), ',',' '), '  ', ' ')
	end as Authors, 
	case when isnumeric(ynp.value) = 1 then cast(ynp.Value as int) else null end as Year, 
	gn.CanonicalName as Genus, sn.CanonicalName as Species,
	n.GoverningCode, tr.SortOrder, n.nameid, 0
from consensus.Name n
inner join consensus.NameProperty cnp on cnp.NameID = n.NameID and cnp.NamePropertyTypeID = '1F64E93C-7EE8-40D7-8681-52B56060D750'
inner join TaxonRank tr on tr.TaxonRankID = n.TaxonRankID
left join consensus.NameProperty anp on anp.NameID = n.NameID and anp.NamePropertyTypeID = '006D86A8-08A5-4C1A-BC08-C07B0225E01B'
left join consensus.NameProperty ynp on ynp.NameID = n.NameID and ynp.NamePropertyTypeID = 'C9E19D58-BD92-4F3C-96E8-DE7FA6DCD5F7'
left join consensus.StackedName gn on gn.SeedNameID = n.nameid and gn.Depth > 0
	and gn.TaxonRankID = '20552EB6-1BF0-4073-A021-A6C7A89B7F14'
left join consensus.StackedName sn on sn.SeedNameID = n.nameid and sn.Depth > 0
	and sn.TaxonRankID = 'C21BB221-5291-4540-94D1-55A12D1BD0AD'


--first names with all fields non-null - authors, year [, page?]

insert #names
select distinct n.NameClassID, n.TaxonRankID as Rank, cnp.Value as Canonical, 
	case when charindex(' ex ', anp.Value) <> 0 
		then replace(replace(replace(replace(replace(SUBSTRING(anp.value, CHARINDEX(' ex ',anp.Value) + 4, LEN(anp.value)), ' and ', ' & '), ' et ', ' & '), '.', ' '), ',',' '), '  ', ' ')
	else
		replace(replace(replace(replace(replace(anp.value, ' and ', ' & '), ' et ', ' & '), '.', ' '), ',',' '), '  ', ' ')
	end as Authors, 
	cast(max(ynp.Value) as int) as Year, 
	gn.CanonicalName as Genus, sn.CanonicalName as Species,
	n.GoverningCode, tr.SortOrder, null, 1
from provider.Name n
inner join provider.NameProperty cnp on cnp.NameID = n.NameID and cnp.NamePropertyTypeID = '1F64E93C-7EE8-40D7-8681-52B56060D750'
inner join TaxonRank tr on tr.TaxonRankID = n.TaxonRankID
inner join provider.NameProperty anp on anp.NameID = n.NameID and anp.NamePropertyTypeID = '006D86A8-08A5-4C1A-BC08-C07B0225E01B'
inner join provider.NameProperty ynp on ynp.NameID = n.NameID and ynp.NamePropertyTypeID = 'C9E19D58-BD92-4F3C-96E8-DE7FA6DCD5F7'
left join provider.StackedName gn on gn.SeedNameID = n.nameid and gn.Depth > 0
	and gn.TaxonRankID = '20552EB6-1BF0-4073-A021-A6C7A89B7F14'
left join provider.StackedName sn on sn.SeedNameID = n.nameid and sn.Depth > 0
	and sn.TaxonRankID = 'C21BB221-5291-4540-94D1-55A12D1BD0AD'
where consensusnameid is null and isnumeric(ynp.Value) = 1
	and (tr.sortorder <= 3000 or gn.canonicalname is not null)
	and (tr.sortorder <= 4200 or sn.canonicalname is not null)
	and not exists(select Canonical from #names where canonical = cnp.Value collate Latin1_General_CI_AS and 
			ISNUMERIC(year) = 1 and
			rank = tr.TaxonRankID 
			and authors = case when charindex(' ex ', anp.Value) <> 0 
				then replace(replace(replace(replace(replace(SUBSTRING(anp.value, CHARINDEX(' ex ',anp.Value) + 4, LEN(anp.value)), ' and ', ' & '), ' et ', ' & '), '.', ' '), ',',' '), '  ', ' ')
				else
					replace(replace(replace(replace(replace(anp.value, ' and ', ' & '), ' et ', ' & '), '.', ' '), ',',' '), '  ', ' ')
				end collate Latin1_General_CI_AS 
			and isnull(round(ynp.Value,-1),ISNULL(round([year],-1), 0)) = isnull(round([year],-1),0) 
			and ISNULL(genus, '') = isnull(gn.CanonicalName,'') collate Latin1_General_CI_AS
			and isnull(species,'') = ISNULL(sn.CanonicalName, '') collate Latin1_General_CI_AS and nomcode = n.GoverningCode collate Latin1_General_CI_AS)
group by n.TaxonRankID, cnp.Value, 
	case when charindex(' ex ', anp.Value) <> 0 
		then replace(replace(replace(replace(replace(SUBSTRING(anp.value, CHARINDEX(' ex ',anp.Value) + 4, LEN(anp.value)), ' and ', ' & '), ' et ', ' & '), '.', ' '), ',',' '), '  ', ' ')
	else
		replace(replace(replace(replace(replace(anp.value, ' and ', ' & '), ' et ', ' & '), '.', ' '), ',',' '), '  ', ' ')
	end, 
	ROUND(ynp.Value,-1), gn.CanonicalName, sn.CanonicalName, 
	n.GoverningCode, tr.SortOrder, n.NameClassID
order by tr.SortOrder, Genus, Species, Canonical

--names with authors and no year
insert #names
select distinct n.NameClassID, n.TaxonRankID as Rank, cnp.Value as Canonical, 	
	case when charindex(' ex ', anp.Value) <> 0 
		then replace(replace(replace(replace(replace(SUBSTRING(anp.value, CHARINDEX(' ex ',anp.Value) + 4, LEN(anp.value)), ' and ', ' & '), ' et ', ' & '), '.', ' '), ',',' '), '  ', ' ')
	else
		replace(replace(replace(replace(replace(anp.value, ' and ', ' & '), ' et ', ' & '), '.', ' '), ',',' '), '  ', ' ')
	end as Authors, 
	null, 	
	gn.CanonicalName as Genus, sn.CanonicalName as Species,
	n.GoverningCode, tr.SortOrder, null, 1
from provider.Name n
inner join provider.NameProperty cnp on cnp.NameID = n.NameID and cnp.NamePropertyTypeID = '1F64E93C-7EE8-40D7-8681-52B56060D750'
inner join TaxonRank tr on tr.TaxonRankID = n.TaxonRankID
inner join provider.NameProperty anp on anp.NameID = n.NameID --non-null authors
	and anp.NamePropertyTypeID = '006D86A8-08A5-4C1A-BC08-C07B0225E01B'
left join provider.NameProperty ynp on ynp.NameID = n.NameID and ynp.NamePropertyTypeID = 'C9E19D58-BD92-4F3C-96E8-DE7FA6DCD5F7'
left join provider.StackedName gn on gn.SeedNameID = n.nameid and gn.Depth > 0
	and gn.TaxonRankID = '20552EB6-1BF0-4073-A021-A6C7A89B7F14'
left join provider.StackedName sn on sn.SeedNameID = n.nameid and sn.Depth > 0
	and sn.TaxonRankID = 'C21BB221-5291-4540-94D1-55A12D1BD0AD'
where consensusnameid is null and (ynp.Value is null or ISNUMERIC(ynp.value) = 0)
	and (tr.sortorder <= 3000 or gn.canonicalname is not null)
	and (tr.sortorder <= 4200 or sn.canonicalname is not null)
	and not exists(select Canonical from #names where canonical = cnp.Value collate Latin1_General_CI_AS and 
			rank = tr.TaxonRankID 
			and authors = case when charindex(' ex ', anp.Value) <> 0 
				then replace(replace(replace(replace(replace(SUBSTRING(anp.value, CHARINDEX(' ex ',anp.Value) + 4, LEN(anp.value)), ' and ', ' & '), ' et ', ' & '), '.', ' '), ',',' '), '  ', ' ')
				else
					replace(replace(replace(replace(replace(anp.value, ' and ', ' & '), ' et ', ' & '), '.', ' '), ',',' '), '  ', ' ')
				end collate Latin1_General_CI_AS 
			and ISNULL(genus, '') = isnull(gn.CanonicalName,'') collate Latin1_General_CI_AS
			and isnull(species,'') = ISNULL(sn.CanonicalName, '') collate Latin1_General_CI_AS and nomcode = n.GoverningCode collate Latin1_General_CI_AS)
group by n.TaxonRankID, cnp.Value, 
	case when charindex(' ex ', anp.Value) <> 0 
		then replace(replace(replace(replace(replace(SUBSTRING(anp.value, CHARINDEX(' ex ',anp.Value) + 4, LEN(anp.value)), ' and ', ' & '), ' et ', ' & '), '.', ' '), ',',' '), '  ', ' ')
	else
		replace(replace(replace(replace(replace(anp.value, ' and ', ' & '), ' et ', ' & '), '.', ' '), ',',' '), '  ', ' ')
	end, 
	gn.CanonicalName, sn.CanonicalName, 
	n.GoverningCode, tr.SortOrder, n.NameClassID
order by tr.SortOrder, Genus, Species, Canonical


--next get names with null authors but have a year, and no names already match on the other fields
insert #names
select distinct n.NameClassID, n.TaxonRankID as Rank, 
	cnp.Value as Canonical, 
	null, --authors 
	cast(max(ynp.Value) as int) as Year,
	gn.CanonicalName as Genus, sn.CanonicalName as Species,
	n.GoverningCode, tr.SortOrder, null, 1
from provider.Name n
inner join provider.NameProperty cnp on cnp.NameID = n.NameID and cnp.NamePropertyTypeID = '1F64E93C-7EE8-40D7-8681-52B56060D750'
inner join TaxonRank tr on tr.TaxonRankID = n.TaxonRankID
left join provider.NameProperty anp on anp.NameID = n.NameID and anp.NamePropertyTypeID = '006D86A8-08A5-4C1A-BC08-C07B0225E01B'
left join provider.NameProperty ynp on ynp.NameID = n.NameID and ynp.NamePropertyTypeID = 'C9E19D58-BD92-4F3C-96E8-DE7FA6DCD5F7'
left join provider.StackedName gn on gn.SeedNameID = n.nameid and gn.Depth > 0
	and gn.TaxonRankID = '20552EB6-1BF0-4073-A021-A6C7A89B7F14'
left join provider.StackedName sn on sn.SeedNameID = n.nameid and sn.Depth > 0
	and sn.TaxonRankID = 'C21BB221-5291-4540-94D1-55A12D1BD0AD'
where consensusnameid is null and anp.Value is null
	and isnumeric(ynp.Value) = 1
	and (tr.sortorder <= 3000 or gn.canonicalname is not null)
	and (tr.sortorder <= 4200 or sn.canonicalname is not null)
	and not exists(select Canonical from #names where canonical = cnp.Value collate Latin1_General_CI_AS and 
			ISNUMERIC(year) = 1 
			and round(year,-1) = round(cast(ynp.Value as int),-1)
			and rank = tr.TaxonRankID 
			and ISNULL(genus, '') = isnull(gn.CanonicalName,'') collate Latin1_General_CI_AS
			and isnull(species,'') = ISNULL(sn.CanonicalName, '') collate Latin1_General_CI_AS and nomcode = n.GoverningCode collate Latin1_General_CI_AS)
group by n.TaxonRankID, cnp.Value, gn.CanonicalName, sn.CanonicalName, ROUND(ynp.Value,-1),
	n.GoverningCode, tr.SortOrder, n.NameClassID
order by tr.SortOrder, Genus, Species, Canonical

--next get names with null authors, null year, and no names already match on the other fields
insert #names
select distinct n.NameClassID, n.TaxonRankID as Rank, 
	cnp.Value as Canonical, 
	null, --authors 
	null, --year
	gn.CanonicalName as Genus, sn.CanonicalName as Species,
	n.GoverningCode, tr.SortOrder, null, 1
from provider.Name n
inner join provider.NameProperty cnp on cnp.NameID = n.NameID and cnp.NamePropertyTypeID = '1F64E93C-7EE8-40D7-8681-52B56060D750'
inner join TaxonRank tr on tr.TaxonRankID = n.TaxonRankID
left join provider.NameProperty anp on anp.NameID = n.NameID and anp.NamePropertyTypeID = '006D86A8-08A5-4C1A-BC08-C07B0225E01B'
left join provider.NameProperty ynp on ynp.NameID = n.NameID and ynp.NamePropertyTypeID = 'C9E19D58-BD92-4F3C-96E8-DE7FA6DCD5F7'
left join provider.StackedName gn on gn.SeedNameID = n.nameid and gn.Depth > 0
	and gn.TaxonRankID = '20552EB6-1BF0-4073-A021-A6C7A89B7F14'
left join provider.StackedName sn on sn.SeedNameID = n.nameid and sn.Depth > 0
	and sn.TaxonRankID = 'C21BB221-5291-4540-94D1-55A12D1BD0AD'
where consensusnameid is null and anp.Value is null
	and (isnumeric(ynp.Value) = 0 or ynp.Value is null)
	and (tr.sortorder <= 3000 or gn.canonicalname is not null)
	and (tr.sortorder <= 4200 or sn.canonicalname is not null)
	and not exists(select Canonical from #names where canonical = cnp.Value collate Latin1_General_CI_AS and 
			rank = tr.TaxonRankID 
			and ISNULL(genus, '') = isnull(gn.CanonicalName,'') collate Latin1_General_CI_AS
			and isnull(species,'') = ISNULL(sn.CanonicalName, '') collate Latin1_General_CI_AS and nomcode = n.GoverningCode collate Latin1_General_CI_AS)
group by n.TaxonRankID, cnp.Value, gn.CanonicalName, sn.CanonicalName, n.GoverningCode, tr.SortOrder, n.NameClassID
order by tr.SortOrder, Genus, Species, Canonical


update #names
set id = NEWID()
where id is null

insert consensus.Name
select id, rank, class, '', nomcode, null, GETDATE(), null
from #names
where isnew = 1

--with year
insert consensus.NameProperty(NameID, NamePropertyTypeID, Sequence, Value)
select distinct id, np.NamePropertyTypeID, np.Sequence, np.Value
from #names cn, provider.Name pn 
inner join provider.NameProperty np on np.NameID = pn.NameID
inner join TaxonRank tr on tr.TaxonRankID = pn.TaxonRankID
inner join provider.NameProperty cnp on cnp.NameID = pn.NameID and cnp.NamePropertyTypeID = '1F64E93C-7EE8-40D7-8681-52B56060D750'	
left join provider.NameProperty anp on anp.NameID = pn.NameID and anp.NamePropertyTypeID = '006D86A8-08A5-4C1A-BC08-C07B0225E01B'
inner join provider.NameProperty ynp on ynp.NameID = pn.NameID and ynp.NamePropertyTypeID = 'C9E19D58-BD92-4F3C-96E8-DE7FA6DCD5F7'
left join provider.StackedName gn on gn.SeedNameID = pn.nameid and gn.Depth > 0
	and gn.TaxonRankID = '20552EB6-1BF0-4073-A021-A6C7A89B7F14'
left join provider.StackedName sn on sn.SeedNameID = pn.nameid and sn.Depth > 0
	and sn.TaxonRankID = 'C21BB221-5291-4540-94D1-55A12D1BD0AD' 
where isnew = 1 and pn.TaxonRankID = cn.rank 
	and pn.GoverningCode = cn.nomcode collate Latin1_General_CI_AS	
	and ISNUMERIC(ynp.value) = 1
	and ISNUMERIC(cn.year) = 1
	and cnp.Value = cn.canonical collate Latin1_General_CI_AS
	and isnull(case when charindex(' ex ', anp.Value) <> 0 
			then replace(replace(replace(replace(replace(SUBSTRING(anp.value, CHARINDEX(' ex ',anp.Value) + 4, LEN(anp.value)), ' and ', ' & '), ' et ', ' & '), '.', ' '), ',',' '), '  ', ' ')
			else
				replace(replace(replace(replace(replace(anp.value, ' and ', ' & '), ' et ', ' & '), '.', ' '), ',',' '), '  ', ' ')
			end, ISNULL(cn.authors,'')) = isnull(cn.authors,'') collate Latin1_General_CI_AS
	and (case when ISNUMERIC(ynp.value) = 0 then 0 else isnull(round(ynp.Value,-1),ISNULL(round(cn.year,-1), 0)) end)
		= isnull(round(cn.year,-1),0) 
	and ISNULL(genus, '') = isnull(gn.CanonicalName,'') collate Latin1_General_CI_AS
	and isnull(species,'') = ISNULL(sn.CanonicalName, '') collate Latin1_General_CI_AS
			

--without year
insert consensus.NameProperty(NameID, NamePropertyTypeID, Sequence, Value)
select distinct id, np.NamePropertyTypeID, np.Sequence, np.Value
from #names cn, provider.Name pn 
inner join provider.NameProperty np on np.NameID = pn.NameID
inner join TaxonRank tr on tr.TaxonRankID = pn.TaxonRankID
inner join provider.NameProperty cnp on cnp.NameID = pn.NameID and cnp.NamePropertyTypeID = '1F64E93C-7EE8-40D7-8681-52B56060D750'	
left join provider.NameProperty anp on anp.NameID = pn.NameID and anp.NamePropertyTypeID = '006D86A8-08A5-4C1A-BC08-C07B0225E01B'
left join provider.NameProperty ynp on ynp.NameID = pn.NameID and ynp.NamePropertyTypeID = 'C9E19D58-BD92-4F3C-96E8-DE7FA6DCD5F7'
left join provider.StackedName gn on gn.SeedNameID = pn.nameid and gn.Depth > 0
	and gn.TaxonRankID = '20552EB6-1BF0-4073-A021-A6C7A89B7F14'
left join provider.StackedName sn on sn.SeedNameID = pn.nameid and sn.Depth > 0
	and sn.TaxonRankID = 'C21BB221-5291-4540-94D1-55A12D1BD0AD' 
where isnew = 1 and pn.TaxonRankID = cn.rank 
	and pn.GoverningCode = cn.nomcode collate Latin1_General_CI_AS	
	and (ynp.value is null or isnumeric(ynp.Value) = 0)
	and (cn.year is null or isnumeric(cn.year) = 0)
	and cnp.Value = cn.canonical collate Latin1_General_CI_AS
	and isnull(case when charindex(' ex ', anp.Value) <> 0 
			then replace(replace(replace(replace(replace(SUBSTRING(anp.value, CHARINDEX(' ex ',anp.Value) + 4, LEN(anp.value)), ' and ', ' & '), ' et ', ' & '), '.', ' '), ',',' '), '  ', ' ')
			else
				replace(replace(replace(replace(replace(anp.value, ' and ', ' & '), ' et ', ' & '), '.', ' '), ',',' '), '  ', ' ')
			end, ISNULL(cn.authors,'')) = isnull(cn.authors,'') collate Latin1_General_CI_AS
	and ISNULL(genus, '') = isnull(gn.CanonicalName,'') collate Latin1_General_CI_AS
	and isnull(species,'') = ISNULL(sn.CanonicalName, '') collate Latin1_General_CI_AS

	

--update prov name links
--with year
update pn
set pn.consensusnameid = cn.id, 
	pn.MatchScore = case when anp.Value is null then 90 else 100 end,
	pn.LinkStatus = 'Matched',
	pn.MatchPath = 'Distinct name matching'
from #names cn, provider.Name pn 
inner join TaxonRank tr on tr.TaxonRankID = pn.TaxonRankID
inner join provider.NameProperty cnp on cnp.NameID = pn.NameID and cnp.NamePropertyTypeID = '1F64E93C-7EE8-40D7-8681-52B56060D750'
left join provider.NameProperty anp on anp.NameID = pn.NameID and anp.NamePropertyTypeID = '006D86A8-08A5-4C1A-BC08-C07B0225E01B'
inner join provider.NameProperty ynp on ynp.NameID = pn.NameID and ynp.NamePropertyTypeID = 'C9E19D58-BD92-4F3C-96E8-DE7FA6DCD5F7'
left join provider.StackedName gn on gn.SeedNameID = pn.nameid and gn.Depth > 0
	and gn.TaxonRankID = '20552EB6-1BF0-4073-A021-A6C7A89B7F14'
left join provider.StackedName sn on sn.SeedNameID = pn.nameid and sn.Depth > 0
	and sn.TaxonRankID = 'C21BB221-5291-4540-94D1-55A12D1BD0AD' 
where pn.TaxonRankID = cn.rank 
	and isnumeric(ynp.Value) = 1
	and ISNUMERIC(cn.year) = 1
	and pn.GoverningCode = cn.nomcode collate Latin1_General_CI_AS
	and cnp.Value = cn.canonical collate Latin1_General_CI_AS
	and isnull(case when charindex(' ex ', anp.Value) <> 0 
			then replace(replace(replace(replace(replace(SUBSTRING(anp.value, CHARINDEX(' ex ',anp.Value) + 4, LEN(anp.value)), ' and ', ' & '), ' et ', ' & '), '.', ' '), ',',' '), '  ', ' ')
			else
				replace(replace(replace(replace(replace(anp.value, ' and ', ' & '), ' et ', ' & '), '.', ' '), ',',' '), '  ', ' ')
			end, ISNULL(cn.authors,'')) = isnull(cn.authors,'') collate Latin1_General_CI_AS
	and (case when ISNUMERIC(ynp.value) = 0 then 0 else isnull(round(ynp.Value,-1),ISNULL(round(cn.year,-1), 0)) end)
		= isnull(round(cn.year,-1),0) 
	and ISNULL(genus, '') = isnull(gn.CanonicalName,'') collate Latin1_General_CI_AS
	and isnull(species,'') = ISNULL(sn.CanonicalName, '') collate Latin1_General_CI_AS

--without year
update pn
set pn.consensusnameid = cn.id, 
	pn.MatchScore = case when anp.Value is null then 90 else 100 end,
	pn.LinkStatus = 'Matched',
	pn.MatchPath = 'Distinct name matching'
from #names cn, provider.Name pn 
inner join TaxonRank tr on tr.TaxonRankID = pn.TaxonRankID
inner join provider.NameProperty cnp on cnp.NameID = pn.NameID and cnp.NamePropertyTypeID = '1F64E93C-7EE8-40D7-8681-52B56060D750'
left join provider.NameProperty anp on anp.NameID = pn.NameID and anp.NamePropertyTypeID = '006D86A8-08A5-4C1A-BC08-C07B0225E01B'
left join provider.NameProperty ynp on ynp.NameID = pn.NameID and ynp.NamePropertyTypeID = 'C9E19D58-BD92-4F3C-96E8-DE7FA6DCD5F7'
left join provider.StackedName gn on gn.SeedNameID = pn.nameid and gn.Depth > 0
	and gn.TaxonRankID = '20552EB6-1BF0-4073-A021-A6C7A89B7F14'
left join provider.StackedName sn on sn.SeedNameID = pn.nameid and sn.Depth > 0
	and sn.TaxonRankID = 'C21BB221-5291-4540-94D1-55A12D1BD0AD' 
where pn.TaxonRankID = cn.rank 
	and (isnumeric(ynp.Value) = 0 or ynp.Value is null)
	and pn.GoverningCode = cn.nomcode collate Latin1_General_CI_AS
	and cnp.Value = cn.canonical collate Latin1_General_CI_AS
	and isnull(case when charindex(' ex ', anp.Value) <> 0 
			then replace(replace(replace(replace(replace(SUBSTRING(anp.value, CHARINDEX(' ex ',anp.Value) + 4, LEN(anp.value)), ' and ', ' & '), ' et ', ' & '), '.', ' '), ',',' '), '  ', ' ')
			else
				replace(replace(replace(replace(replace(anp.value, ' and ', ' & '), ' et ', ' & '), '.', ' '), ',',' '), '  ', ' ')
			end, ISNULL(cn.authors,'')) = isnull(cn.authors,'') collate Latin1_General_CI_AS
	and (isnumeric(ynp.value) = 0 or ynp.value is null)
	and ISNULL(genus, '') = isnull(gn.CanonicalName,'') collate Latin1_General_CI_AS
	and isnull(species,'') = ISNULL(sn.CanonicalName, '') collate Latin1_General_CI_AS


--name property related Ids
update cnp
set RelatedID = topn.ConsensusNameID
from provider.Name pn
inner join provider.NameProperty pnp on pnp.NameID = pn.NameID and pnp.RelatedID is not null
inner join provider.Name topn on pnp.RelatedID = topn.NameID
inner join consensus.Name cn on cn.NameID = pn.ConsensusNameID
inner join consensus.NameProperty cnp on cnp.NameID = cn.NameID and cnp.NamePropertyTypeID = pnp.NamePropertyTypeID
where topn.ConsensusNameID is not null


update n
set IsRecombination = 
	(select top 1 IsRecombination from provider.Name where IsRecombination is not null and ConsensusNameID = n.NameID)
from consensus.Name n


--concepts
insert consensus.Concept(NameID, AccordingToReferenceID)
select distinct cn.nameid, pr.ConsensusReferenceID
from consensus.Name cn
inner join provider.Name pn on pn.ConsensusNameID = cn.NameID
inner join provider.Concept pc on pc.NameID = pn.NameID
left join provider.Reference pr on pr.ReferenceID = pc.AccordingToReferenceID
where ConsensusConceptID is null

update pc
set pc.ConsensusConceptID = cc.ConceptID,
	pc.LinkStatus = 'Matched',
	pc.MatchScore = 100,
	pc.MatchPath = 'Distinct name matching'
from provider.Concept pc
inner join provider.Name pn on pn.NameID = pc.NameID
left join provider.Reference pr on pr.ReferenceID = pc.AccordingToReferenceID
inner join consensus.Concept cc on cc.NameID = pn.ConsensusNameID 
	and isnull(cast(cc.AccordingToReferenceID as varchar(38)),'') = ISNULL(cast(pr.ConsensusReferenceID as varchar(38)),'')
	

update cc
set cc.HigherClassification = pc.HigherClassification
from consensus.Concept cc
inner join provider.Concept pc on pc.ConsensusConceptID = cc.ConceptID
where pc.HigherClassification is not null

update cc
set cc.Orthography = pc.Orthography
from consensus.Concept cc
inner join provider.Concept pc on pc.ConsensusConceptID = cc.ConceptID
where pc.Orthography is not null

update cc
set cc.TaxonRank = pc.TaxonRank
from consensus.Concept cc
inner join provider.Concept pc on pc.ConsensusConceptID = cc.ConceptID
where pc.TaxonRank is not null


update c
set c.AddedDate = GETDATE()
from consensus.Concept c
inner join #names n on n.id = c.nameid
where n.isnew = 1


--concept relationships
delete consensus.conceptrelationship

insert consensus.ConceptRelationship(FromConceptID, ToConceptID, ConceptRelationshipTypeID, IsActive, Sequence)
select distinct cc.ConceptID, pcto.ConsensusConceptID, pcr.ConceptRelationshipTypeID, pcr.InUse, pcr.Sequence
from consensus.Concept cc 
inner join provider.Concept pc on pc.ConsensusConceptID = cc.ConceptID
inner join provider.ConceptRelationship pcr on pcr.FromConceptID = pc.ConceptID
inner join provider.Concept pcto on pcto.ConceptID = pcr.ToConceptID
inner join consensus.Concept cto on cto.ConceptID = pcto.ConsensusConceptID


update consensus.ConceptRelationship 
set AddedDate = GETDATE()
where addeddate is null


--taxon properties

delete consensus.taxonpropertyvalue
delete consensus.taxonproperty

create table #propVals(consTPID uniqueidentifier, taxonPropId uniqueidentifier, classId uniqueidentifier, consNameId uniqueidentifier, nameId uniqueidentifier, conceptId uniqueidentifier, refId uniqueidentifier, val nvarchar(4000), inuse bit, dt datetime)

create index idx_tp on #propVals(taxonPropId)
create index idx_nm on #propVals(nameId)
create index idx_cn on #propVals(conceptId)

insert #propVals
select pt.consensustaxonpropertyid,
		pt.TaxonPropertyID,
		pt.TaxonPropertyClassID,
		ISNULL(pn.consensusnameid, pcn.ConsensusNameID),
		pn.ConsensusNameID,
		pc.ConsensusConceptID,
		pr.ConsensusReferenceID,
		cast(data.val as nvarchar(max)),
		pt.InUse,
		isnull(isnull(isnull(isnull(isnull(pt.ProviderModifiedDate, pt.ProviderCreatedDate), pn.ProviderModifiedDate), pn.ProviderCreatedDate), pcn.ProviderModifiedDate), pcn.ProviderCreatedDate)
from provider.TaxonProperty pt
left join provider.Concept pc on pc.ConceptID = pt.ConceptID
left join provider.name pcn on pcn.nameid = pc.nameid
left join provider.Name pn on pn.NameID = pt.NameID
left join provider.Reference pr on pr.ReferenceID = pt.ReferenceID
cross apply (select tpt.Name as '@Type',
			ptpv.Value as 'text()'			
			from provider.TaxonProperty ptp 
			inner join provider.TaxonPropertyValue ptpv on ptpv.TaxonPropertyID = ptp.TaxonPropertyID
			inner join dbo.TaxonPropertyType tpt on tpt.TaxonPropertyTypeID = ptpv.TaxonPropertyTypeID
			where pt.TaxonPropertyID = ptp.TaxonPropertyID  
			for xml path('Value'), type) data(val)
where (pn.ConsensusNameID is not null or pc.ConsensusConceptID is not null)
	
--only keep most recent taxon property per name/class

declare @toKeep table(id uniqueidentifier);

with tp (rn, id) as (select ROW_NUMBER() over(partition by classId, consNameId, refId order by dt desc, len(val) desc), taxonPropId from #propVals )	
insert @toKeep 
select id from tp where rn = 1
						
delete pv
from #propVals pv
left join @toKeep tk on tk.id = pv.taxonPropId
where tk.id is null

update ptp
set consensustaxonpropertyid = null, LinkStatus = null, MatchScore = null
from provider.TaxonProperty ptp
left join @toKeep tk on tk.id = ptp.TaxonPropertyID
where tk.id is null

create table #taxonProps(consTPID uniqueidentifier, classID uniqueidentifier, referenceID uniqueidentifier, conceptID uniqueidentifier, nameID uniqueidentifier, inUse bit, vals nvarchar(4000), ids xml)

insert #taxonProps(classID, referenceID, conceptID, nameID, vals, inuse)	
select distinct classId, refId, conceptid, nameid, val, inuse
from #propVals

update #taxonProps set consTPID = NEWID()

update tp
set tp.ids = ids.val
from #taxonProps tp
cross apply (select taxonPropId as TaxonPropertyID 
	from #propVals pv 
	where pv.classId = tp.classID
		and pv.val = tp.vals
		and (pv.conceptId = tp.conceptID or pv.nameId = tp.nameID)
	for xml path('IDs'), type) ids(val)

insert consensus.TaxonProperty
select consTPID, classID, null, referenceID, conceptID, nameID, inUse, GETDATE(), null
from #taxonProps tp

update ptp
set ptp.ConsensusTaxonPropertyID = tp.consTPID,
	LinkStatus = 'Matched',
	MatchScore = 100
from #taxonProps tp 
cross apply tp.ids.nodes('/IDs/TaxonPropertyID') as id(val) 
inner join provider.TaxonProperty ptp on ptp.TaxonPropertyID = id.val.value('.', 'nvarchar(1000)')

insert consensus.TaxonPropertyValue(TaxonPropertyID, TaxonPropertyTypeID, Value)
select distinct ctp.TaxonPropertyID, ptpv.TaxonPropertyTypeID, ptpv.Value
from consensus.TaxonProperty ctp
inner join provider.TaxonProperty ptp on ptp.ConsensusTaxonPropertyID = ctp.TaxonPropertyID
inner join provider.TaxonPropertyValue ptpv on ptpv.TaxonPropertyID = ptp.TaxonPropertyID
inner join #taxonProps tp on tp.consTPID = ctp.TaxonPropertyID


drop table #propVals
drop table #taxonProps


--vernaculars and concept applications 
-- need to insert vernacular names first
declare @vernNames table(id uniqueidentifier, fullName nvarchar(500), language nvarchar(200), country nvarchar(200), isNew bit)

--existing vernaculars
insert @vernNames
select n.nameid, fullname, lnp.value, cnp.value, 0
from consensus.Name n
left join consensus.NameProperty lnp on lnp.NameID = n.NameID and lnp.NamePropertyTypeID = '44F50B86-5A11-4582-81C2-E03B7DF069EF'
left join consensus.NameProperty cnp on cnp.NameID = n.NameID and cnp.NamePropertyTypeID = 'C57F037C-1F1B-4E20-9557-A2D9161C1118'
where n.NameClassID = '05BCC19C-27E8-492C-8ADD-EC5F73325BC5'


--names with country and language
insert @vernNames(fullName, language, country, isNew)
select distinct pn.FullName, lnp.Value, cnp.Value, 1
from provider.Name pn
inner join provider.NameProperty lnp on lnp.NameID = pn.NameID and lnp.NamePropertyTypeID = '44F50B86-5A11-4582-81C2-E03B7DF069EF'
inner join provider.NameProperty cnp on cnp.NameID = pn.NameID and cnp.NamePropertyTypeID = 'C57F037C-1F1B-4E20-9557-A2D9161C1118'
inner join provider.Concept pc on pc.NameId = pn.NameId --must have a concept defined to point to a scientific name, otherwise we dont want to know
where pn.NameClassID = '05BCC19C-27E8-492C-8ADD-EC5F73325BC5'
	and not exists(select * from @vernNames where fullName like '%>' + pn.FullName + '<%' and country = cnp.value
					and language = lnp.Value)

--names with no country
insert @vernNames(fullName, language, country, isNew)
select distinct pn.FullName, lnp.Value, cnp.Value, 1
from provider.Name pn
inner join provider.NameProperty lnp on lnp.NameID = pn.NameID and lnp.NamePropertyTypeID = '44F50B86-5A11-4582-81C2-E03B7DF069EF'
left join provider.NameProperty cnp on cnp.NameID = pn.NameID and cnp.NamePropertyTypeID = 'C57F037C-1F1B-4E20-9557-A2D9161C1118'
inner join provider.Concept pc on pc.NameId = pn.NameId --must have a concept defined to point to a scientific name, otherwise we dont want to know
where pn.NameClassID = '05BCC19C-27E8-492C-8ADD-EC5F73325BC5'
	and cnp.Value is null
	and not exists(select * from @vernNames where fullName like '%>' + pn.FullName + '<%'
					and language = lnp.Value)

--names with no language
insert @vernNames(fullName, language, country, isNew)
select distinct pn.FullName, lnp.Value, cnp.Value, 1
from provider.Name pn
left join provider.NameProperty lnp on lnp.NameID = pn.NameID and lnp.NamePropertyTypeID = '44F50B86-5A11-4582-81C2-E03B7DF069EF'
inner join provider.NameProperty cnp on cnp.NameID = pn.NameID and cnp.NamePropertyTypeID = 'C57F037C-1F1B-4E20-9557-A2D9161C1118'
inner join provider.Concept pc on pc.NameId = pn.NameId --must have a concept defined to point to a scientific name, otherwise we dont want to know
where pn.NameClassID = '05BCC19C-27E8-492C-8ADD-EC5F73325BC5'
	and lnp.Value is null
	and not exists(select * from @vernNames where fullName like '%>' + pn.FullName + '<%'
					and country = cnp.Value)

--names with no language or country
insert @vernNames(fullName, language, country, isNew)
select distinct pn.FullName, lnp.Value, cnp.Value, 1
from provider.Name pn
left join provider.NameProperty lnp on lnp.NameID = pn.NameID and lnp.NamePropertyTypeID = '44F50B86-5A11-4582-81C2-E03B7DF069EF'
left join provider.NameProperty cnp on cnp.NameID = pn.NameID and cnp.NamePropertyTypeID = 'C57F037C-1F1B-4E20-9557-A2D9161C1118'
inner join provider.Concept pc on pc.NameId = pn.NameId --must have a concept defined to point to a scientific name, otherwise we dont want to know
where pn.NameClassID = '05BCC19C-27E8-492C-8ADD-EC5F73325BC5'
	and lnp.Value is null
	and cnp.Value is null
	and not exists(select * from @vernNames where fullName like '%>' + pn.FullName + '<%')

update @vernNames 
set id = NEWID()
where isNew = 1

insert consensus.Name 
select id, '057D6434-A12A-460D-B705-4510603FAE4F', '05BCC19C-27E8-492C-8ADD-EC5F73325BC5', fullName, null, null, GETDATE(), null
from @vernNames n
where isNew = 1

update pn
set ConsensusNameID = v.id,
	LinkStatus = 'Matched',
	MatchScore = 100,
	MatchPath = 'Distinct name matching'
from provider.Name pn
inner join @vernNames v on v.FullName like '%>' + pn.fullName + '<%'
left join provider.NameProperty lnp on lnp.NameID = pn.NameID and lnp.NamePropertyTypeID = '44F50B86-5A11-4582-81C2-E03B7DF069EF'
left join provider.NameProperty cnp on cnp.NameID = pn.NameID and cnp.NamePropertyTypeID = 'C57F037C-1F1B-4E20-9557-A2D9161C1118'
where (lnp.Value is null or lnp.value = ISNULL(v.language,''))
	and (cnp.value is null or cnp.Value = ISNULL(v.country,''))
	and isNew = 1

insert consensus.NameProperty(NameID, NamePropertyTypeID, Sequence, Value)
select distinct n.NameID, pnp.NamePropertyTypeID, pnp.Sequence, pnp.Value
from @vernNames v
inner join consensus.Name n on n.NameID = v.id
inner join provider.Name pn on pn.ConsensusNameID = n.NameID
inner join provider.NameProperty pnp on pnp.NameID = pn.NameID 
where isNew = 1

insert consensus.Concept(NameID, AccordingToReferenceID)
select distinct pn.ConsensusNameID, pr.ConsensusReferenceID
from provider.Name pn
inner join @vernNames vn on vn.id = pn.consensusnameid
inner join provider.Concept pc on pc.NameID = pn.NameID
left join provider.Reference pr on pr.ReferenceID = pc.AccordingToReferenceID
where pn.NameClassID = '05BCC19C-27E8-492C-8ADD-EC5F73325BC5'
	and pn.ConsensusNameID is not null
	and isNew = 1

update pc
set pc.ConsensusConceptID = cc.ConceptID,
	pc.LinkStatus = 'Matched',
	pc.MatchScore = 100,
	pc.MatchPath = 'Distinct name matching'
from provider.Concept pc
inner join provider.Name pn on pn.NameID = pc.NameID
left join provider.Reference pr on pr.ReferenceID = pc.AccordingToReferenceID
inner join consensus.Concept cc on cc.NameID = pn.ConsensusNameID 
	and isnull(cast(cc.AccordingToReferenceID as varchar(38)),'') = ISNULL(cast(pr.ConsensusReferenceID as varchar(38)),'')
where pn.NameClassID = '05BCC19C-27E8-492C-8ADD-EC5F73325BC5'
	and consensusconceptid is null
	
	

--trade names and concepts
-- need to insert trade names first
declare @tradeNames table(id uniqueidentifier, fullName nvarchar(500), language nvarchar(200), country nvarchar(200), isNew bit)

--existing trade names
insert @tradeNames
select n.nameid, fullname, lnp.value, cnp.value, 0
from consensus.Name n
left join consensus.NameProperty lnp on lnp.NameID = n.NameID and lnp.NamePropertyTypeID = '2089E79F-4CD4-4CD7-AA0D-A3AD0CDFD331'
left join consensus.NameProperty cnp on cnp.NameID = n.NameID and cnp.NamePropertyTypeID = '5F3CD05D-3BCC-4D2B-BE71-BFAC7DF9FD17'
where n.NameClassID = '3D3A13B8-C673-459C-B98D-8A5B08E3CA44'

--names with country and language
insert @tradeNames(fullName, language, country, isNew)
select distinct pn.FullName, lnp.Value, cnp.Value, 1
from provider.Name pn
inner join provider.NameProperty lnp on lnp.NameID = pn.NameID and lnp.NamePropertyTypeID = '2089E79F-4CD4-4CD7-AA0D-A3AD0CDFD331'
inner join provider.NameProperty cnp on cnp.NameID = pn.NameID and cnp.NamePropertyTypeID = '5F3CD05D-3BCC-4D2B-BE71-BFAC7DF9FD17'
where pn.NameClassID = '3D3A13B8-C673-459C-B98D-8A5B08E3CA44'
	and not exists(select * from @tradeNames where fullName = pn.FullName and country = cnp.value
					and language = lnp.Value)

--names with no country
insert @tradeNames(fullName, language, country, isNew)
select distinct pn.FullName, lnp.Value, cnp.Value, 1
from provider.Name pn
inner join provider.NameProperty lnp on lnp.NameID = pn.NameID and lnp.NamePropertyTypeID = '2089E79F-4CD4-4CD7-AA0D-A3AD0CDFD331'
left join provider.NameProperty cnp on cnp.NameID = pn.NameID and cnp.NamePropertyTypeID = '5F3CD05D-3BCC-4D2B-BE71-BFAC7DF9FD17'
where pn.NameClassID = '3D3A13B8-C673-459C-B98D-8A5B08E3CA44'
	and cnp.Value is null
	and not exists(select * from @tradeNames where fullName = pn.FullName 
					and language = lnp.Value)

--names with no language
insert @tradeNames(fullName, language, country, isNew)
select distinct pn.FullName, lnp.Value, cnp.Value, 1
from provider.Name pn
left join provider.NameProperty lnp on lnp.NameID = pn.NameID and lnp.NamePropertyTypeID = '2089E79F-4CD4-4CD7-AA0D-A3AD0CDFD331'
inner join provider.NameProperty cnp on cnp.NameID = pn.NameID and cnp.NamePropertyTypeID = '5F3CD05D-3BCC-4D2B-BE71-BFAC7DF9FD17'
where pn.NameClassID = '3D3A13B8-C673-459C-B98D-8A5B08E3CA44'
	and lnp.Value is null
	and not exists(select * from @tradeNames where fullName = pn.FullName 
					and country = cnp.Value)

--names with no language or country
insert @tradeNames(fullName, language, country, isNew)
select distinct pn.FullName, lnp.Value, cnp.Value, 1
from provider.Name pn
left join provider.NameProperty lnp on lnp.NameID = pn.NameID and lnp.NamePropertyTypeID = '2089E79F-4CD4-4CD7-AA0D-A3AD0CDFD331'
left join provider.NameProperty cnp on cnp.NameID = pn.NameID and cnp.NamePropertyTypeID = '5F3CD05D-3BCC-4D2B-BE71-BFAC7DF9FD17'
where pn.NameClassID = '3D3A13B8-C673-459C-B98D-8A5B08E3CA44'
	and lnp.Value is null
	and cnp.Value is null
	and not exists(select * from @tradeNames where fullName = pn.FullName)

update @tradeNames 
set id = NEWID()
where isNew = 1

insert consensus.Name 
select id, '057D6434-A12A-460D-B705-4510603FAE4F', '3D3A13B8-C673-459C-B98D-8A5B08E3CA44', fullName, null, null, GETDATE(), null
from @tradeNames n
where isNew = 1

update pn
set ConsensusNameID = t.id,
	LinkStatus = 'Matched',
	MatchScore = 100,
	MatchPath = 'Distinct name matching'
from provider.Name pn
inner join @tradeNames t on t.fullName = pn.FullName
left join provider.NameProperty lnp on lnp.NameID = pn.NameID and lnp.NamePropertyTypeID = '2089E79F-4CD4-4CD7-AA0D-A3AD0CDFD331'
left join provider.NameProperty cnp on cnp.NameID = pn.NameID and cnp.NamePropertyTypeID = '5F3CD05D-3BCC-4D2B-BE71-BFAC7DF9FD17'
where ISNULL(lnp.value,'') = ISNULL(t.language,'')
	and ISNULL(cnp.value,'') = ISNULL(t.country,'')
	and isNew = 1

insert consensus.NameProperty(NameID, NamePropertyTypeID, Sequence, Value)
select distinct n.NameID, pnp.NamePropertyTypeID, pnp.Sequence, pnp.Value
from @tradeNames t
inner join consensus.Name n on n.NameID = t.id
inner join provider.Name pn on pn.ConsensusNameID = n.NameID
inner join provider.NameProperty pnp on pnp.NameID = pn.NameID 
	and isNew = 1

insert consensus.Concept(NameID, AccordingToReferenceID)
select distinct pn.ConsensusNameID, pr.ConsensusReferenceID
from provider.Name pn
inner join provider.Concept pc on pc.NameID = pn.NameID
inner join @tradeNames t on t.id = pn.consensusnameid
left join provider.Reference pr on pr.ReferenceID = pc.AccordingToReferenceID
where pn.NameClassID = '3D3A13B8-C673-459C-B98D-8A5B08E3CA44'
	and pn.ConsensusNameID is not null
	and t.isNew = 1

update pc
set pc.ConsensusConceptID = cc.ConceptID,
	pc.LinkStatus = 'Matched',
	pc.MatchScore = 100,
	pc.MatchPath = 'Distinct name matching'
from provider.Concept pc
inner join provider.Name pn on pn.NameID = pc.NameID
left join provider.Reference pr on pr.ReferenceID = pc.AccordingToReferenceID
inner join consensus.Concept cc on cc.NameID = pn.ConsensusNameID 
	and isnull(cast(cc.AccordingToReferenceID as varchar(38)),'') = ISNULL(cast(pr.ConsensusReferenceID as varchar(38)),'')
where pn.NameClassID = '3D3A13B8-C673-459C-B98D-8A5B08E3CA44'
	and consensusconceptid is null
	

delete consensus.ConceptApplication

insert consensus.ConceptApplication(FromConceptID, ToConceptID, ConceptApplicationTypeID, Gender, PartOfTaxon, LifeStage, GeoRegionID, GeographicSchemaID)
select distinct pc.ConsensusConceptID, pcto.ConsensusConceptID, pca.ConceptApplicationTypeID, pca.Gender, pca.PartOfTaxon, pca.LifeStage, 
	g.georegionid, gs.geographicschemaid
from provider.ConceptApplication pca
inner join provider.Concept pc on pc.ConceptID = pca.FromConceptID
inner join provider.Concept pcto on pcto.ConceptID = pca.ToConceptID
left join dbo.GeographicSchema gs on gs.name = pca.geographicschema
left join dbo.GeoRegion g on g.name = pca.georegion and g.geographicschemaid = gs.geographicschemaid
where pc.ConsensusConceptID is not null and pcto.ConsensusConceptID is not null
	and not exists(select conceptapplicationid from consensus.conceptapplication
		where fromconceptid = pc.consensusconceptid and
			toconceptid = pcto.consensusconceptid and
			conceptapplicationtypeid = pca.conceptapplicationtypeid and
			isnull(gender,'') = isnull(pca.gender,'') and
			isnull(partoftaxon,'') = isnull(pca.partoftaxon,'') and
			isnull(lifestage,'') = isnull(pca.lifestage,'') and
			isnull(georegionid, '00000000-0000-0000-0000-000000000000') = isnull(g.georegionid,'00000000-0000-0000-0000-000000000000') and
			isnull(geographicschemaid, '00000000-0000-0000-0000-000000000000') = isnull(gs.geographicschemaid, '00000000-0000-0000-0000-000000000000'))


update consensus.ConceptApplication set AddedDate = GETDATE() where addeddate is null  --both vernaculars and trade names


--unlink hanging provider concepts
update pc
set pc.ConsensusConceptID = null, LinkStatus = null, MatchPath = null, MatchScore = null
from provider.Concept pc
left join consensus.Concept cc on cc.ConceptID = pc.ConsensusConceptID
where pc.ConsensusConceptID is not null and cc.ConceptID is null


--update attpoints with new cons name ids
update ap
set ap.ConsensusNameID = pn.ConsensusNameID
from [admin].AttachmentPoint ap
inner join provider.Name pn on pn.ProviderRecordID = ap.ProviderRecordID collate Latin1_General_CI_AI
where pn.consensusnameid is not null

---------------------------------------------------------------------------
--fix mupltiple parent concepts issue

create table #attPointNames(dsId uniqueidentifier, ranking int, provRecId nvarchar(1000), nameId uniqueidentifier, consNameId uniqueidentifier, depth int, ord int);

WITH attPN (dsId, ranking, provRecId, nameId, consNameId, depth) AS 
( 
    SELECT apds.DataSourceID, apds.Ranking, n.ProviderRecordID, n.NameID, n.ConsensusNameID, 0
    FROM [admin].AttachmentPointDataSource apds 
		inner join [admin].AttachmentPoint ap on ap.AttachmentPointID = apds.AttachmentPointID
		inner join consensus.Name cn on cn.NameID = ap.ConsensusNameID
		inner join provider.Name N on n.ConsensusNameID = cn.NameID
    
    UNION ALL 
     
    SELECT s.dsId, s.ranking, n2.ProviderRecordID, n2.NameID, n2.ConsensusNameID, s.depth + 1
    FROM provider.Name n
		inner join attPN s on s.NameID = n.NameID
        inner join provider.Concept c on c.NameID = n.NameID 
        inner join provider.ConceptRelationship cr on cr.ToConceptID = c.ConceptID 
			and cr.ConceptRelationshipTypeID = '6A11B466-1907-446F-9229-D604579AA155' and cr.InUse = 1
        inner join provider.Concept c2 on c2.ConceptID = cr.fromConceptID
        inner join provider.Name n2 on n2.NameID = c2.NameID and n2.nameid <> n.nameid
    
) 

insert #attPointNames
SELECT distinct *, ROW_NUMBER() over(partition by consnameid order by ranking)
FROM attPN 


update apn 
set ranking = apds.Ranking
from #attPointNames apn
inner join [admin].AttachmentPoint ap on ap.providerrecordid = apn.provRecId collate Latin1_General_CI_AS and apn.dsId = ap.datasourceid
inner join [admin].AttachmentPointDataSource apds on ap.AttachmentPointID = apds.AttachmentPointID



create table #conceptConflicts(nameId uniqueidentifier, ccrId1 uniqueidentifier, ccrId2 uniqueidentifier)

insert #conceptConflicts
select distinct pn1.ConsensusNameID, ccr1.ConceptRelationshipID, ccr2.ConceptRelationshipID
from provider.Name pn1 
inner join dbo.TaxonRank tr on tr.TaxonRankID = pn1.TaxonRankID
inner join provider.Concept pc1 on pc1.NameID = pn1.NameID
inner join provider.ConceptRelationship pcr1 on pcr1.FromConceptID = pc1.ConceptID and pcr1.InUse = 1
	and pcr1.ConceptRelationshipTypeID = '6A11B466-1907-446F-9229-D604579AA155'
inner join provider.Name pn2 on pn2.ConsensusNameID = pn1.ConsensusNameID and pn2.NameID <> pn1.NameID
inner join provider.Concept pc2 on pc2.NameID = pn2.NameID
inner join provider.ConceptRelationship pcr2 on pcr2.FromConceptID = pc2.ConceptID and pcr2.InUse = 1 
	and pcr2.ConceptRelationshipTypeID = '6A11B466-1907-446F-9229-D604579AA155'	
	and pcr2.ToConceptID <> pcr1.ToConceptID
inner join consensus.Concept cc1 on cc1.ConceptID = pc1.ConsensusConceptID
inner join consensus.ConceptRelationship ccr1 on ccr1.FromConceptID = cc1.ConceptID and ccr1.IsActive = 1
	and ccr1.ConceptRelationshipTypeID = pcr1.ConceptRelationshipTypeID
inner join consensus.Concept cc2 on cc2.ConceptID = pc2.ConsensusConceptID and cc2.ConceptID <> cc1.ConceptID
inner join consensus.ConceptRelationship ccr2 on ccr2.FromConceptID = cc2.ConceptID and ccr2.IsActive = 1
	and ccr2.ConceptRelationshipTypeID = pcr1.ConceptRelationshipTypeID

update cr1
set cr1.IsActive = 0
from #conceptConflicts ccr
inner join consensus.ConceptRelationship cr1 on cr1.ConceptRelationshipID = ccrId1
inner join consensus.ConceptRelationship cr2 on cr2.ConceptRelationshipID = ccrId2
inner join consensus.Concept c1 on c1.ConceptID = cr1.FromConceptID
inner join consensus.Concept c2 on c2.ConceptID = cr2.FromConceptID
inner join provider.Concept pc1 on pc1.ConsensusConceptID = c1.ConceptID
inner join provider.Concept pc2 on pc2.ConsensusConceptID = c2.ConceptID
inner join #attPointNames apn1 on apn1.provRecId = pc1.ProviderNameID collate SQL_Latin1_General_CP1_CI_AS and apn1.ord <> 1
inner join #attPointNames apn2 on apn2.provRecId = pc2.ProviderNameID collate SQL_Latin1_General_CP1_CI_AS and apn2.ord = 1

	
drop table #attPointNames
drop table #conceptConflicts

--end multiple parent issues
-------------------------------------------------------------------------------


exec consensus.sprUpdate_StackedNameData


--annotations
delete consensus.Annotation

declare @ann table(nameId uniqueidentifier, conceptId uniqueidentifier, refId uniqueidentifier, annText nvarchar(max), annType nvarchar(250))

insert @ann
select distinct pn.[ConsensusNameID]
      ,pc.[ConsensusConceptID]
      ,pr.[ConsensusReferenceID]
      ,[AnnotationText]
      ,[AnnotationType]
from provider.Annotation a
left join provider.Name pn on pn.NameID = a.NameID
left join provider.Concept pc on pc.ConceptID = a.ConceptID
left join provider.Reference pr on pr.ReferenceID = a.ReferenceID

insert consensus.Annotation
select newid(),
	  nameId,
	  conceptId,
	  refId,
	  annType,
	  annText,
      getdate(),
	  null
from @ann

update pa
set pa.ConsensusAnnotationID = ca.annotationid
from provider.annotation pa
left join provider.Name pn on pn.NameID = pa.NameID
left join provider.Concept pc on pc.ConceptID = pa.ConceptID
left join provider.Reference pr on pr.ReferenceID = pa.ReferenceID
inner join consensus.annotation ca on isnull(ca.nameid, '00000000-0000-0000-0000-000000000000') = isnull(pn.consensusnameid, '00000000-0000-0000-0000-000000000000')
	and isnull(ca.conceptid, '00000000-0000-0000-0000-000000000000') = isnull(pc.consensusconceptid, '00000000-0000-0000-0000-000000000000')
	and isnull(ca.referenceid, '00000000-0000-0000-0000-000000000000') = isnull(pr.consensusreferenceid, '00000000-0000-0000-0000-000000000000')
	and ca.annotationtype = pa.annotationtype
	and ca.annotationtext = pa.annotationtext

--delete non-linked ones
delete a
from consensus.Annotation a
left join consensus.Name n on n.NameID = a.NameID
where n.NameID is null

--update full names
declare @ids table(nameid uniqueidentifier, row int identity)
insert @ids
select nameid 
from consensus.Name
where FullName = '' or nameclassid = '05BCC19C-27E8-492C-8ADD-EC5F73325BC5' or fullname = '<FullName><Name>ROOT</Name></FullName>' 
	or FullName not like '<%'

declare @cnt int, @pos int, @id uniqueidentifier

select @cnt = COUNT(*), @pos = 1
from @ids

while (@pos <= @cnt)
begin
	select @id = nameid from @ids where row = @pos
	
	update consensus.name
	set FullName = isnull(consensus.GetFullName(@id),'')
	where NameID = @id
	
	set @pos = @pos + 1
end


            
--scientific names full names
--insert consensus.NameProperty 
--select newid(), NameID, '86E7590B-EF34-4E19-970B-608703B858A5', null, null, replace(dbo.ApplyXSLT(FullName, 
--    (select convert(nvarchar(max), xslt) from dbo.transformation where name = 'NameText_FullName'), 0),'&amp;', '&')
--from consensus.Name
--where NameClassID = 'A5233111-61A0-4AE6-9C2B-5E8E71F1473A'
--	and addeddate > @runDate

--insert consensus.NameProperty 
--select newid(), NameID, '86B84828-E1C0-45BD-A5C0-7B272EDC97EF', null, null, replace(dbo.ApplyXSLT(FullName, 
--    (select convert(nvarchar(max), xslt) from dbo.transformation where name = 'NameText_FullNameFormatted'), 0),'&amp;', '&')
--from consensus.Name
--where NameClassID = 'A5233111-61A0-4AE6-9C2B-5E8E71F1473A'
--	and addeddate > @runDate

--insert consensus.NameProperty 
--select newid(), NameID, '00806321-C8BD-4518-9539-1286DA02CA7D', null, null, replace(dbo.ApplyXSLT(FullName, 
--    (select convert(nvarchar(max), xslt) from dbo.transformation where name = 'NameText_PartialName'), 0),'&amp;', '&')
--from consensus.Name
--where NameClassID = 'A5233111-61A0-4AE6-9C2B-5E8E71F1473A'
--	and addeddate > @runDate

--insert consensus.NameProperty 
--select newid(), NameID, 'F721F463-5F16-4333-9C7D-DDF848F2D1A9', null, null, replace(dbo.ApplyXSLT(FullName, 
--    (select convert(nvarchar(max), xslt) from dbo.transformation where name = 'NameText_PartialNameFormatted'), 0),'&amp;', '&')
--from consensus.Name
--where NameClassID = 'A5233111-61A0-4AE6-9C2B-5E8E71F1473A'
--	and addeddate > @runDate

----vernacular names full names
--insert consensus.NameProperty 
--select newid(), NameID, '88020F95-1282-4D9A-819A-0973F7F50284', null, null, replace(dbo.ApplyXSLT(FullName, 
--    (select convert(nvarchar(max), xslt) from dbo.transformation where name = 'NameText_FullName'), 0),'&amp;', '&')
--from consensus.Name
--where NameClassID = '05BCC19C-27E8-492C-8ADD-EC5F73325BC5'
--	and addeddate > @runDate

--insert consensus.NameProperty 
--select newid(), NameID, 'C4954CF2-6A07-469B-B470-2D56E60C6666', null, null, replace(dbo.ApplyXSLT(FullName, 
--    (select convert(nvarchar(max), xslt) from dbo.transformation where name = 'NameText_FullName'), 0),'&amp;', '&')
--from consensus.Name
--where NameClassID = '3D3A13B8-C673-459C-B98D-8A5B08E3CA44'
--	and addeddate > @runDate

	
drop table #names
drop table #refs


