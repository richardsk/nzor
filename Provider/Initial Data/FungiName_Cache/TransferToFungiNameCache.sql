IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TransferToFungiNameCache]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[TransferToFungiNameCache]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [dbo].[TransferToFungiNameCache]
as

exec [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.dbo.FungiCache0
exec dbo.FungiCache10
exec dbo.FungiCache15
exec dbo.FungiCache20
exec dbo.FungiCache30
exec dbo.FungiCache40
exec dbo.FungiCache45
exec [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.dbo.FungiCache5
exec [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.dbo.FungiCache50
exec dbo.FungiCache6


go

grant execute on dbo.[TransferToFungiNameCache] to dbi_user

go
