ALTER TABLE dwc_nzfungi.[Distribution]
    ADD CONSTRAINT [frkDistributionTaxon] FOREIGN KEY ([TaxonID]) REFERENCES dwc_nzfungi.[Taxon] ([TaxonID]) ON DELETE NO ACTION ON UPDATE NO ACTION;

