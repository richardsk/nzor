IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TransferToPlantNameCache]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[TransferToPlantNameCache]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [dbo].[TransferToPlantNameCache]
as

exec [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.dbo.pndcache0
exec dbo.pndcache10
exec dbo.pndcache15
exec dbo.pndcache20
exec dbo.pndcache30
exec dbo.pndcache40
exec dbo.pndcache45
exec [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.dbo.pndcache5
exec [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.dbo.pndcache50
exec dbo.pndcache6


go

grant execute on dbo.[TransferToPlantNameCache] to dbi_user

go
