IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[HostedCache5]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[HostedCache5]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [dbo].[HostedCache5]
as

delete Hosted_name.Metadata

insert into Hosted_name.Metadata(ProviderId, ProviderCode, Name, DataSourceId, OrganisationURL,
	Attribution, Disclaimer, Licensing)
select '22B420A6-5C18-4CBC-9560-E48B2B24EDFF', 'NZOR_Hosted', 'NZOR Hosted Names', 'Hosted', '',
	null, null, null
		
go

grant execute on dbo.[HostedCache5] to dbi_user

go
