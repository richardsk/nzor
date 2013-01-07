UPDATE
	consensus.Reference
SET
	ReferenceTypeID = @ReferenceTypeID, 	
	ModifiedDate = @ModifiedDate
WHERE
	ReferenceID = @ReferenceID;
