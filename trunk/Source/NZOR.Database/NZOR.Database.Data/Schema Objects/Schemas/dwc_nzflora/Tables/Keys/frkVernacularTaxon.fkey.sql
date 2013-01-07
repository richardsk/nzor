ALTER TABLE dwc_nzflora.[Vernacular]
    ADD CONSTRAINT [frkVernacularTaxon] FOREIGN KEY ([TaxonID]) REFERENCES dwc_nzflora.[Taxon] ([TaxonID]) ON DELETE NO ACTION ON UPDATE NO ACTION;

