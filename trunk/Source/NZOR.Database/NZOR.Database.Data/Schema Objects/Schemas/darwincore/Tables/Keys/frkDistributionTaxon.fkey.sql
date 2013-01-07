ALTER TABLE [darwincore].[Distribution]
    ADD CONSTRAINT [frkDistributionTaxon] FOREIGN KEY ([TaxonID]) REFERENCES [darwincore].[Taxon] ([TaxonID]) ON DELETE NO ACTION ON UPDATE NO ACTION;

