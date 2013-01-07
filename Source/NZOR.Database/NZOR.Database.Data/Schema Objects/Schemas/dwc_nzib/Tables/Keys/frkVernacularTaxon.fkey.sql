ALTER TABLE dwc_nzib.[Vernacular]
    ADD CONSTRAINT [frkVernacularTaxon] FOREIGN KEY ([TaxonID]) REFERENCES dwc_nzib.[Taxon] ([TaxonID]) ON DELETE NO ACTION ON UPDATE NO ACTION;

