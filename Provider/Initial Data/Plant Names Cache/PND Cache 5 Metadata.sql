IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PNDCache5]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[PNDCache5]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [dbo].[PNDCache5]
as

delete plant_name.Metadata

insert into plant_name.Metadata(ProviderId, ProviderCode, Name, DataSourceId, OrganisationURL,
	Attribution, Disclaimer, Licensing)
select 'ED07387F-BD46-41CF-8A2F-AF95CE342CCB', 'NZFLORA', 'Plant Names Database', 'NZFLORA', 'http://nzflora.lanedcareresearch.co.nz',
	'Plant Names Database', 'These data are drawn from a dynamic data source that is continually being edited.  They may contain errors or omissions.',
		null
		
go

grant execute on dbo.[PNDCache5] to dbi_user

go
