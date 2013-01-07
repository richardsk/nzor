ALTER TABLE dwc_nzac.[Literature]
    ADD CONSTRAINT [frkLiteratureTaxon] FOREIGN KEY ([TaxonID]) REFERENCES dwc_nzac.[Taxon] ([TaxonID]) ON DELETE NO ACTION ON UPDATE NO ACTION;

