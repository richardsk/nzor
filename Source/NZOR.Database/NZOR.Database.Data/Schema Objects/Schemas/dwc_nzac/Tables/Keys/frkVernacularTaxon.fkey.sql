ALTER TABLE dwc_nzac.[Vernacular]
    ADD CONSTRAINT [frkVernacularTaxon] FOREIGN KEY ([TaxonID]) REFERENCES dwc_nzac.[Taxon] ([TaxonID]) ON DELETE NO ACTION ON UPDATE NO ACTION;

