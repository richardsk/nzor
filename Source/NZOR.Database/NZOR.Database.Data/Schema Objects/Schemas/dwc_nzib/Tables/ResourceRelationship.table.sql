CREATE TABLE dwc_nzib.[ResourceRelationship]
(
	TaxonID nvarchar(1000) NOT NULL, 
	RelatedResourceID nvarchar(1000) NULL,
	RelationshipOfResource nvarchar(1000) not null,
	RelationshipAccordingTo nvarchar(1000),
	RelationshipEstablishedDate nvarchar(1000)
)
