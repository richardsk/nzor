IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[NZACCache5]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[NZACCache5]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [dbo].[NZACCache5]
as

delete nzac_name.Metadata

insert into nzac_name.Metadata(ProviderId, ProviderCode, Name, DataSourceId, OrganisationURL,
	Attribution, Disclaimer, Licensing)
select '7BC9CB00-23BC-40A4-8A7B-7B5CB4E5B573', 'NZAC', 'New Zealand Arthropod Names Database', 'NZAC', '',
	null, null, null
		
go

grant execute on dbo.[NZACCache5] to dbi_user

go
