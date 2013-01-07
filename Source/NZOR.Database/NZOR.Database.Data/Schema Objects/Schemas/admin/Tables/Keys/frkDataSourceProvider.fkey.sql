ALTER TABLE [admin].[DataSource]
    ADD CONSTRAINT frkDataSourceProvider FOREIGN KEY ([ProviderID]) REFERENCES [admin].[Provider] ([ProviderID]) ON DELETE NO ACTION ON UPDATE NO ACTION;

