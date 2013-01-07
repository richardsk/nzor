ALTER TABLE dwc_nzfungi.[Literature]
    ADD CONSTRAINT [frkLiteratureTaxon] FOREIGN KEY ([TaxonID]) REFERENCES dwc_nzfungi.[Taxon] ([TaxonID]) ON DELETE NO ACTION ON UPDATE NO ACTION;

