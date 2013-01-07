ALTER TABLE dwc_nzflora.[Literature]
    ADD CONSTRAINT [frkLiteratureTaxon] FOREIGN KEY ([TaxonID]) REFERENCES dwc_nzflora.[Taxon] ([TaxonID]) ON DELETE NO ACTION ON UPDATE NO ACTION;

