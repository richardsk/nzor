ALTER TABLE [admin].[HarvestLog]
    ADD CONSTRAINT frkHarvestLogProviderEndpoint FOREIGN KEY ([ProviderEndpointID]) REFERENCES [admin].[DataSourceEndpoint] ([DataSourceEndpointID]) ON DELETE NO ACTION ON UPDATE NO ACTION;

