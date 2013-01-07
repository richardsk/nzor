delete tmpintegration

dbcc checkident ('tmpintegration', RESEED, 0)
go

insert tmpintegration
select 'E7ADB1D5-C072-4384-8656-327146646D80', null
--'88441283-026F-4EB2-9925-00556C4D2ABE', null
/*select pn.nameid, null
from prov.name pn
inner join prov.NameProperty pnp on pnp.nameid = pn.nameid and pnp.nameclasspropertyid = 'A1D57520-3D64-4F7D-97C8-69B449AFA280'
inner join dbo.namepropertylookup npl on npl.value = pnp.value
where consensusnameid is null
order by npl.sortorder */


declare @pos int
declare @id uniqueidentifier
declare @cnt int

select @pos = 1, @cnt = count(*) from tmpintegration

while (@pos <= @cnt)
begin
	
	select @id = recordid from tmpintegration where integorder = @pos
	
	print(@id)
	
	exec sprSelect_NameMatches @id
	
	set @pos = @pos + 1
		
end

