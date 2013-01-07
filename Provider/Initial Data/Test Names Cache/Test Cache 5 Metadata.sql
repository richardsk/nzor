IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TestCache5]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[TestCache5]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [dbo].[TestCache5]
as

delete [devserver02\sql2005].name_cache_test.test_name.Metadata
delete [devserver02\sql2005].name_cache_test.test_name_2.Metadata

insert into [devserver02\sql2005].name_cache_test.test_name.Metadata(ProviderId, Name, DataSourceId, OrganisationURL,
	Attribution, Disclaimer, Licensing)
select 'CD4DD7CE-48F1-404B-BF33-00A715957C35', 'NZOR Test', 'NZOR_Test', '',
	'', '', null
		
		
insert into [devserver02\sql2005].name_cache_test.test_name_2.Metadata(ProviderId, Name, DataSourceId, OrganisationURL,
	Attribution, Disclaimer, Licensing)
select '6B584613-DD83-40F9-8D51-1E891B22A273', 'NZOR Test 2', 'NZOR_Test_2', '',
	'', '', null
		
