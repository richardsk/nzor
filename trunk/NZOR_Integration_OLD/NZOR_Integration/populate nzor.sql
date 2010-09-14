
--refs
delete prov.reference

insert prov.reference
select newid(), '4F2ABFEC-23A7-48F8-B5CD-C144192DAA8E', null, 'B8E1EF06-1F7D-43CE-BF30-71735E600A96', r.referenceguid, r.referenceupdateddate, getdate()
from proserver01.nuku.dbo.tblreference r

--concepts
delete prov.concept

insert prov.concept
select newid(), null, pn.nameid, r.referenceid, null, 'B8E1EF06-1F7D-43CE-BF30-71735E600A96', null, nameupdateddate, getdate()
from proserver01.nuku.dbo.tblname n
inner join prov.name pn on pn.providerrecordid = n.nameguid
left join prov.reference r on r.providerrecordid = n.namereferencefk

delete prov.conceptrelationship

insert prov.conceptrelationship
select newid(), c.conceptid, pc.conceptid, '6A11B466-1907-446F-9229-D604579AA155', null
from prov.name n
inner join prov.concept c on c.nameid = n.nameid
inner join proserver01.nuku.dbo.tblname pron on pron.nameguid = n.providerrecordid
inner join prov.name pn on pn.providerrecordid = pron.nameparentfk
inner join prov.concept pc on pc.nameid = pn.nameid


--backbone

delete dbo.taxonrank

insert taxonrank
select newid(), taxonrankname, '', taxonranksort, taxonrankincludeinfullname, null, null, 'migration', getdate(), null, null
from proserver01.nuku.dbo.tbltaxonrank

update tr
set knownabbreviations = r.rankknownabbreviations, matchrulesetid = r.rankmatchrulesetfk, 
	ancestorrankid = (select taxonrankid from taxonrank where name = ar.rankname)
from taxonrank tr
inner join compositae.dbo.tblrank r on r.rankname = tr.name
left join compositae.dbo.tblrank ar on ar.rankpk = r.ancestorrankfk


delete cons.name

insert cons.name
select newid(), n.namefull,  'A5233111-61A0-4AE6-9C2B-5E8E71F1473A', n.nameorthographyvariant, 'ICBN', 'migration', getdate(), null, null
from proserver01.nuku.dbo.tblname n 
where n.nameguid in ('2F5CA98D-EE05-461D-901A-419A88071133',
	'19F62517-BD63-42E3-97DA-E043A1644B1B',
	'8AE0A964-A505-4D3B-8E20-E916F9B83E74',
	'D3FC3540-2423-4A30-93D8-1A1102434F72',
	'C8694427-CC2D-4530-A0B7-9DE513E7122F',
	'39BECB7F-180A-4415-8684-5EAA441795F0',
	'628AA321-ABC1-44D2-ACC1-799705FFF073',
	'7B757DBE-E2DB-4217-90CA-3400614D860B',
	'3EA012A9-9AAB-44E8-8B0F-B02D28B45B43')

--BACK BONE PROV NAMES ...
delete prov.name
where providerrecordid  in ('2F5CA98D-EE05-461D-901A-419A88071133', 
	'19F62517-BD63-42E3-97DA-E043A1644B1B',
	'8AE0A964-A505-4D3B-8E20-E916F9B83E74',
	'D3FC3540-2423-4A30-93D8-1A1102434F72',
	'C8694427-CC2D-4530-A0B7-9DE513E7122F',
	'39BECB7F-180A-4415-8684-5EAA441795F0')
	
insert prov.name
select newid(), 
	(select nameid from cons.name where fullname = n.namefull collate SQL_Latin1_General_CP1_CI_AI),
	n.namefull, 
	'A5233111-61A0-4AE6-9C2B-5E8E71F1473A', 
	n.nameorthographyvariant, 
	'ICBN', 
	'B8E1EF06-1F7D-43CE-BF30-71735E600A96',
	n.nameguid,
	n.nameupdateddate,
	getdate()
from proserver01.nuku.dbo.tblname n 
where n.nameguid in ('2F5CA98D-EE05-461D-901A-419A88071133', 
	'19F62517-BD63-42E3-97DA-E043A1644B1B',
	'8AE0A964-A505-4D3B-8E20-E916F9B83E74',
	'D3FC3540-2423-4A30-93D8-1A1102434F72',
	'C8694427-CC2D-4530-A0B7-9DE513E7122F',
	'39BECB7F-180A-4415-8684-5EAA441795F0')
	
update prov.name
set consensusnameid = cn.nameid
from prov.name
inner join cons.name cn on cn.fullname = prov.name.fullname

delete c from prov.concept c 
where not exists(select * from prov.name where nameid = c.nameid)

delete cr from prov.conceptrelationship cr 
where not exists(select * from prov.concept where conceptid = fromconceptid)
	or not exists(select * from prov.concept where conceptid = toconceptid)

insert prov.concept
select newid(), null, pn.nameid, r.referenceid, null, 'B8E1EF06-1F7D-43CE-BF30-71735E600A96', null, nameupdateddate, getdate()
from prov.name pn
inner join proserver01.nuku.dbo.tblname n on n.nameguid = pn.providerrecordid
left join prov.reference r on r.providerrecordid = n.namereferencefk
where n.nameguid in ('2F5CA98D-EE05-461D-901A-419A88071133', 
	'19F62517-BD63-42E3-97DA-E043A1644B1B',
	'8AE0A964-A505-4D3B-8E20-E916F9B83E74',
	'D3FC3540-2423-4A30-93D8-1A1102434F72',
	'C8694427-CC2D-4530-A0B7-9DE513E7122F',
	'39BECB7F-180A-4415-8684-5EAA441795F0')
	
insert prov.conceptrelationship
select newid(), c.conceptid, pc.conceptid, '6A11B466-1907-446F-9229-D604579AA155', null
from prov.name n
inner join prov.concept c on c.nameid = n.nameid
inner join proserver01.nuku.dbo.tblname pron on pron.nameguid = n.providerrecordid
inner join prov.name pn on pn.providerrecordid = pron.nameparentfk
inner join prov.concept pc on pc.nameid = pn.nameid
where pron.nameguid in ('2F5CA98D-EE05-461D-901A-419A88071133', 
	'19F62517-BD63-42E3-97DA-E043A1644B1B',
	'8AE0A964-A505-4D3B-8E20-E916F9B83E74',
	'D3FC3540-2423-4A30-93D8-1A1102434F72',
	'C8694427-CC2D-4530-A0B7-9DE513E7122F',
	'39BECB7F-180A-4415-8684-5EAA441795F0')
	
--END BACKBONE PROV NAMES


delete cons.reference

insert cons.reference
select r.providerrecordid, '4F2ABFEC-23A7-48F8-B5CD-C144192DAA8E', 'migration', getdate(), null, null
from cons.name n 
inner join prov.name pn on pn.consensusnameid = n.nameid
inner join prov.concept c on c.nameid = pn.nameid
inner join prov.reference r on r.referenceid = accordingtoreferenceid 

update r
set consensusreferenceid = providerrecordid
from prov.reference r
inner join cons.reference cr on cr.referenceid = r.providerrecordid

--parent concepts
delete cons.concept

insert cons.concept
select newid(), pn.consensusnameid, r.consensusreferenceid, null, 'migration', getdate(), null, null
from proserver01.nuku.dbo.tblname n
inner join prov.name pn on pn.providerrecordid = n.nameguid
left join prov.reference r on r.providerrecordid = n.namereferencefk
where consensusnameid is not null

update pc
set consensusconceptid = c.conceptid
from prov.concept pc
inner join prov.name pn on pn.nameid = pc.nameid
inner join cons.concept c on c.nameid = pn.consensusnameid

delete cons.conceptrelationship

insert cons.conceptrelationship
select newid(), c.consensusconceptid, c.consensusconcepttoid, '6A11B466-1907-446F-9229-D604579AA155', null, 'migration', getdate(), null, null
from cons.name n
inner join vwProviderConcepts c on c.consensusnameid = n.nameid
where consensusnameid is not null

--cons rank

delete cons.nameproperty where nameclasspropertyid = 'A1D57520-3D64-4F7D-97C8-69B449AFA280'

insert cons.nameproperty 
select newid(), cn.nameid, 'A1D57520-3D64-4F7D-97C8-69B449AFA280', tr.taxonrankname, null, null, 'migration', getdate(), null, null
from proserver01.nuku.dbo.tblname n
inner join proserver01.nuku.dbo.tbltaxonrank tr on tr.taxonrankpk = n.nametaxonrankfk
inner join prov.name pn on pn.providerrecordid = n.nameguid
inner join cons.name cn on pn.consensusnameid = cn.nameid

update n 
set n.taxonrankid = tr.taxonrankid
from cons.name n
inner join cons.nameproperty np on np.nameid = n.nameid and nameclasspropertyid = 'A1D57520-3D64-4F7D-97C8-69B449AFA280'
inner join taxonrank tr on tr.name = np.value

--cons year

delete cons.nameproperty where nameclasspropertyid = 'C9E19D58-BD92-4F3C-96E8-DE7FA6DCD5F7'

insert cons.nameproperty
select newid(), cn.nameid, 'C9E19D58-BD92-4F3C-96E8-DE7FA6DCD5F7', n.nameyearofpublication, null, null, 'migration', getdate(), null, null
from proserver01.nuku.dbo.tblname n
inner join prov.name pn on pn.providerrecordid = n.nameguid
inner join cons.name cn on pn.consensusnameid = cn.nameid



--prov name properties

--rank
delete prov.nameproperty where nameclasspropertyid = 'A1D57520-3D64-4F7D-97C8-69B449AFA280'

insert prov.nameproperty
select newid(), pn.nameid, 'A1D57520-3D64-4F7D-97C8-69B449AFA280', tr.taxonrankname, null, null, null
from proserver01.nuku.dbo.tblname n
inner join proserver01.nuku.dbo.tbltaxonrank tr on tr.taxonrankpk = n.nametaxonrankfk
inner join prov.name pn on pn.providerrecordid = n.nameguid

update pn 
set pn.taxonrankid = tr.taxonrankid
from prov.name pn
inner join prov.nameproperty np on np.nameid = pn.nameid and nameclasspropertyid = 'A1D57520-3D64-4F7D-97C8-69B449AFA280'
inner join taxonrank tr on tr.name = np.value


--canonical

delete prov.nameproperty where nameclasspropertyid = '1F64E93C-7EE8-40D7-8681-52B56060D750'

insert prov.nameproperty
select newid(), pn.nameid, '1F64E93C-7EE8-40D7-8681-52B56060D750', n.namecanonical, null, null, null
from proserver01.nuku.dbo.tblname n
inner join prov.name pn on pn.providerrecordid = n.nameguid

--year 
delete prov.nameproperty where nameclasspropertyid = 'C9E19D58-BD92-4F3C-96E8-DE7FA6DCD5F7'

insert prov.nameproperty
select newid(), pn.nameid, 'C9E19D58-BD92-4F3C-96E8-DE7FA6DCD5F7', n.nameyearofpublication, null, null, null
from proserver01.nuku.dbo.tblname n
inner join prov.name pn on pn.providerrecordid = n.nameguid


--authors
delete prov.nameproperty where nameclasspropertyid = '006D86A8-08A5-4C1A-BC08-C07B0225E01B'

insert prov.nameproperty
select newid(), pn.nameid, '006D86A8-08A5-4C1A-BC08-C07B0225E01B', n.nameauthors, null, null, null
from proserver01.nuku.dbo.tblname n
inner join prov.name pn on pn.providerrecordid = n.nameguid



--select * from vwproviderconcepts where consensusnameid is not null
--select * from vwconsensusconcepts