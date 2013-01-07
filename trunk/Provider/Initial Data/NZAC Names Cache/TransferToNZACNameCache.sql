IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TransferToNZACNameCache]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[TransferToNZACNameCache]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [dbo].[TransferToNZACNameCache]
as

exec [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.dbo.NZACCache0
exec dbo.NZACCache10
exec dbo.NZACCache15
exec dbo.NZACCache20
exec dbo.NZACCache30
exec dbo.NZACCache40
exec dbo.NZACCache45
exec [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.dbo.NZACCache5
exec [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.dbo.NZACCache50
exec dbo.NZACCache6


go

grant execute on dbo.[TransferToNZACNameCache] to dbi_user

go
