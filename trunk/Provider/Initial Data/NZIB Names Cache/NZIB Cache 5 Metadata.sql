IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[NZIBCache5]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[NZIBCache5]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [dbo].[NZIBCache5]
as

delete nzib_name.Metadata

insert into nzib_name.Metadata(ProviderId, ProviderCode, Name, DataSourceId, OrganisationURL,
	Attribution, Disclaimer, Licensing)
select '7BC9CB00-23BC-40A4-8A7B-7B5CB4E5B573', 'NZIB', 'New Zealand Inventory of Biodiversity', 'NZIB', '',
	null, null, null
		
		
go

grant execute on dbo.[NZIBCache5] to dbi_user

go
