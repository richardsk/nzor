update consensus.NameProperty
set Value = @Value,
	Sequence = @Sequence,
	RelatedID = @relatedID
where NameID = @nameID and NamePropertyTypeID = @namePropertyTypeID
