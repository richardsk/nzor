select [ReferenceID]
      ,[ReferenceTypeID]
      ,[AddedDate]
      ,[ModifiedDate]
from consensus.Reference 
where referenceId = @referenceId
