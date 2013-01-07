ALTER TABLE [darwincore].[Literature]
    ADD CONSTRAINT [frkLiteratureTaxon] FOREIGN KEY ([TaxonID]) REFERENCES [darwincore].[Taxon] ([TaxonID]) ON DELETE NO ACTION ON UPDATE NO ACTION;

