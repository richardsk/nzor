ALTER TABLE [admin].[AttachmentPoint]
    ADD CONSTRAINT frkAttachmentPointDataSource FOREIGN KEY ([DataSourceID]) REFERENCES [admin].[DataSource] ([DataSourceID]) ON DELETE NO ACTION ON UPDATE NO ACTION;

