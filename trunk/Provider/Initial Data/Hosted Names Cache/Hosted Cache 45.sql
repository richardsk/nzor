IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[HostedCache45]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[HostedCache45]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [dbo].[HostedCache45]
as

	insert into [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.hosted_name.annotation
	select 'NameNote_' + cast(namenotecounterpk as nvarchar(10)),
		cast(namenotenamefk as nvarchar(38)),
		null,
		cast(publicationid as nvarchar(38)),
		notetypetext,
		namenotetext,
		isnull(namenoteaddeddate, getdate()),
		namenoteupdatedwhen
	from tblnamenotes nn
	inner join tblnamenotetype nt on nt.notetypecounterpk = nn.namenotetypefk
	inner join [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.hosted_name.name n on n.nameid = nn.namenotenamefk
	left join [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.hosted_name.publication p on p.publicationid = NameNoteReferenceFk	
	where NoteTypeText = 'taxonomic status' or NoteTypeText = 'NZIB Doubtful record' or NoteTypeText = 'NZIB Species Inquirende'
	
	insert into [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.hosted_name.annotation
	select 'NomenclaturalStatus_' + cast(ns.NomenclaturalStatusCounterPk as nvarchar(10)),
		cast(NomenclaturalStatusNameFk as nvarchar(38)),
		null,
		cast(publicationid as nvarchar(38)),
		NomenclaturalStatusTypeText,
		case when NomenclaturalStatusComment = '<Not Set>' then NomenclaturalStatusTypeText
		else isnull(NomenclaturalStatusComment, NomenclaturalStatusTypeText) end,
		isnull(NomenclaturalStatusAddedDate,getdate()),
		NomenclaturalStatusUpdatedDate
	from tblNomenclaturalStatus ns 
	inner join tblNomenclaturalStatusType nt on nt.NomenclaturalStatusTypeCounterPk = ns.NomenclaturalStatusStatusTypeFk
	inner join [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.hosted_name.name n on n.nameid = ns.NomenclaturalStatusNameFk
	left join [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.hosted_name.publication p on p.publicationid = NomenclaturalStatusReferenceFk	
	where NomenclaturalStatusComment is not null or NomenclaturalStatusTypeText is not null

	insert into [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.hosted_name.annotation
	select 'ProParte_' + cast(nameguid as varchar(38)),
		cast(nameguid as varchar(38)),
		null,
		null,
		'taxonomic status',
		'pro. parte',
		getdate(),
		null
	from tblname n
	inner join [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.hosted_name.name fn on n.nameguid = fn.nameid
	left join tblnamenotes nn on nn.namenotenamefk = n.nameguid and charindex('pro. parte', nn.namenotetext) <> 0
	where n.NameProParte = 1 and nn.NameNoteCounterPK is null
	
	insert into [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.hosted_name.annotation
	select 'NomDub_' + cast(nameguid as varchar(38)),
		cast(nameguid as varchar(38)),
		null,
		null,
		'taxonomic status',
		'nom. dub.',
		getdate(),
		null
	from tblname n
	inner join [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.hosted_name.name fn on n.nameguid = fn.nameid
	left join tblnamenotes nn on nn.namenotenamefk = n.nameguid and 
		(charindex('nom. dub.', nn.namenotetext) <> 0 or charindex('nomen dubium', nn.namenotetext) <> 0)
	where n.NameDubium = 1 and nn.NameNoteCounterPK is null
	
go

grant execute on dbo.[HostedCache45] to dbi_user

go
