ALTER TABLE dwc_hosted.[Distribution]
    ADD CONSTRAINT [frkDistributionTaxon] FOREIGN KEY ([TaxonID]) REFERENCES dwc_hosted.[Taxon] ([TaxonID]) ON DELETE NO ACTION ON UPDATE NO ACTION;

