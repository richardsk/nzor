ALTER TABLE dwc_nzflora.[Distribution]
    ADD CONSTRAINT [frkDistributionTaxon] FOREIGN KEY ([TaxonID]) REFERENCES dwc_nzflora.[Taxon] ([TaxonID]) ON DELETE NO ACTION ON UPDATE NO ACTION;

