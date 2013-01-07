ALTER TABLE [admin].[DataSourceEndpoint]
    ADD CONSTRAINT frkDataSourceEndpointDataSource FOREIGN KEY ([DataSourceID]) REFERENCES [admin].[DataSource] ([DataSourceID]) ON DELETE NO ACTION ON UPDATE NO ACTION;

