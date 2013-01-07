IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TransferToNameCacheTest]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[TransferToNameCacheTest]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [dbo].[TransferToNameCacheTest]
as

exec [devserver02\sql2005].name_cache_test.dbo.Testcache0
exec dbo.Testcache10
exec dbo.Testcache15
exec dbo.Testcache20
exec dbo.Testcache30
exec dbo.Testcache40
exec dbo.Testcache45
exec dbo.Testcache5
exec dbo.Testcache50
exec dbo.Testcache6

