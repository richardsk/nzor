ALTER TABLE dwc_nzfungi.[Vernacular]
    ADD CONSTRAINT [frkVernacularTaxon] FOREIGN KEY ([TaxonID]) REFERENCES dwc_nzfungi.[Taxon] ([TaxonID]) ON DELETE NO ACTION ON UPDATE NO ACTION;

