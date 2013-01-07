ALTER TABLE dwc_hosted.[Vernacular]
    ADD CONSTRAINT [frkVernacularTaxon] FOREIGN KEY ([TaxonID]) REFERENCES dwc_hosted.[Taxon] ([TaxonID]) ON DELETE NO ACTION ON UPDATE NO ACTION;

