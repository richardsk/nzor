ALTER TABLE dwc_nzib.[Distribution]
    ADD CONSTRAINT [frkDistributionTaxon] FOREIGN KEY ([TaxonID]) REFERENCES dwc_nzib.[Taxon] ([TaxonID]) ON DELETE NO ACTION ON UPDATE NO ACTION;

