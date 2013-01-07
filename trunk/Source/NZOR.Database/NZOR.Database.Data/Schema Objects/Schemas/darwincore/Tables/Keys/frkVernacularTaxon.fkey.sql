ALTER TABLE [darwincore].[Vernacular]
    ADD CONSTRAINT [frkVernacularTaxon] FOREIGN KEY ([TaxonID]) REFERENCES [darwincore].[Taxon] ([TaxonID]) ON DELETE NO ACTION ON UPDATE NO ACTION;

