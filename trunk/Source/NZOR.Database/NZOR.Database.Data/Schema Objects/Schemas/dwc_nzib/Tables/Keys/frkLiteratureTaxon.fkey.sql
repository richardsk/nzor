ALTER TABLE dwc_nzib.[Literature]
    ADD CONSTRAINT [frkLiteratureTaxon] FOREIGN KEY ([TaxonID]) REFERENCES dwc_nzib.[Taxon] ([TaxonID]) ON DELETE NO ACTION ON UPDATE NO ACTION;

