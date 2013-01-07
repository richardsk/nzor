IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TestCache45]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[TestCache45]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [dbo].[TestCache45]
as

	insert into [spidey\sql2005].name_cache.plant_name.annotation
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
	inner join [spidey\sql2005].name_cache.plant_name.name n on n.nameid = nn.namenotenamefk
	left join [spidey\sql2005].name_cache.plant_name.publication p on p.publicationid = NameNoteReferenceFk	
	where NoteTypeText = 'taxonomic status'
	
	insert into [spidey\sql2005].name_cache.plant_name.annotation
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
	inner join [spidey\sql2005].name_cache.plant_name.name n on n.nameid = ns.NomenclaturalStatusNameFk
	left join [spidey\sql2005].name_cache.plant_name.publication p on p.publicationid = NomenclaturalStatusReferenceFk	
	where NomenclaturalStatusComment is not null or NomenclaturalStatusTypeText is not null
	
go

grant execute on dbo.[TestCache45] to dbi_user

go
