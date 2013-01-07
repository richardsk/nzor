ALTER TABLE [admin].[AttachmentPoint]
    ADD CONSTRAINT frkAttachmentPointProvider FOREIGN KEY ([ProviderID]) REFERENCES [admin].[Provider] ([ProviderID]) ON DELETE NO ACTION ON UPDATE NO ACTION;

