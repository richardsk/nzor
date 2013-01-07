ALTER TABLE dwc_nzac.[Distribution]
    ADD CONSTRAINT [frkDistributionTaxon] FOREIGN KEY ([TaxonID]) REFERENCES dwc_nzac.[Taxon] ([TaxonID]) ON DELETE NO ACTION ON UPDATE NO ACTION;

