UPDATE [admin].[BrokeredName]
   SET [NameRequestId] = @NameRequestId
      ,[ExternalLookupServiceId] = @ExternalLookupServiceId
      ,[ProviderRecordId] = @ProviderRecordId
      ,[NZORProviderNameId] = @NZORProviderNameId
	  ,[DataUrl] = @DataUrl
	  ,[WebUrl] = @WebUrl
      ,[AddedDate] = @AddedDate
      ,[AddedBy] = @AddedBy
      ,[ModifiedDate] = @ModifiedDate
      ,[ModifiedBy] = @ModifiedBy
 WHERE [BrokeredNameId] = @BrokeredNameId

