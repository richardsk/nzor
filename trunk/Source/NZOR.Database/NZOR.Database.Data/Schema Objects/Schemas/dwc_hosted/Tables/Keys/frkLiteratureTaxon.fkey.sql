ALTER TABLE dwc_hosted.[Literature]
    ADD CONSTRAINT [frkLiteratureTaxon] FOREIGN KEY ([TaxonID]) REFERENCES dwc_hosted.[Taxon] ([TaxonID]) ON DELETE NO ACTION ON UPDATE NO ACTION;

