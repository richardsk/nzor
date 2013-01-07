IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TransferToHostedNameCache]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[TransferToHostedNameCache]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [dbo].[TransferToHostedNameCache]
as

exec [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.dbo.HostedCache0
exec dbo.HostedCache10
exec dbo.HostedCache15
exec dbo.HostedCache20
exec dbo.HostedCache30
exec dbo.HostedCache40
exec dbo.HostedCache45
exec [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.dbo.HostedCache5
exec [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.dbo.HostedCache50
exec dbo.HostedCache6


go

grant execute on dbo.[TransferToHostedNameCache] to dbi_user

go
