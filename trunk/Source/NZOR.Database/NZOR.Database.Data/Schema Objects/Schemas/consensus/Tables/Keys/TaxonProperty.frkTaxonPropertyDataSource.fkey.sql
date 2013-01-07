ALTER TABLE provider.TaxonProperty
	ADD CONSTRAINT [frkTaxonPropertyDataSource] 
	FOREIGN KEY (DataSourceID)
	REFERENCES [admin].DataSource (DataSourceID)	

