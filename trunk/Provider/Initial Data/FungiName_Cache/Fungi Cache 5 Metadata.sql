IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FungiCache5]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[FungiCache5]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [dbo].[FungiCache5]
as

delete fungi_name.Metadata

insert into fungi_name.Metadata(ProviderId, ProviderCode, Name, DataSourceId, OrganisationURL,
	Attribution, Disclaimer, Licensing)
select '0B82097D-57A2-467C-AB5E-EC2A9766D080', 'NZUNGI', 'Fungi Names Database', 'NZFUNGI', 'http://nzfungi.lanedcareresearch.co.nz',
	'Fungi Names Database', 'These data are drawn from a dynamic data source that is continually being edited.  They may contain errors or omissions.',
		null
		
go

grant execute on dbo.[FungiCache5] to dbi_user

go
