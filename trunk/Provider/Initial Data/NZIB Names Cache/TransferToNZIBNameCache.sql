IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TransferToNZIBNameCache]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[TransferToNZIBNameCache]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [dbo].[TransferToNZIBNameCache]
as

exec [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.dbo.NZIBCache0
exec dbo.NZIBcache10
exec dbo.NZIBcache15
exec dbo.NZIBcache20
exec dbo.NZIBcache30
exec dbo.NZIBcache40
exec dbo.NZIBcache45
exec [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.dbo.NZIBcache5
exec [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.dbo.NZIBcache50
exec dbo.NZIBcache6


go

grant execute on dbo.[TransferToNZIBNameCache] to dbi_user

go
