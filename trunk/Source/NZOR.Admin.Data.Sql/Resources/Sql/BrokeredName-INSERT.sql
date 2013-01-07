INSERT INTO [admin].[BrokeredName]
           ([BrokeredNameId]
           ,[NameRequestId]
           ,[ExternalLookupServiceId]
           ,[ProviderRecordId]
           ,[NZORProviderNameId]
		   ,[DataUrl]
		   ,[WebUrl]
           ,[AddedDate]
           ,[AddedBy]
           ,[ModifiedDate]
           ,[ModifiedBy])
     VALUES
           (@BrokeredNameId
           ,@NameRequestId
           ,@ExternalLookupServiceId
           ,@ProviderRecordId
           ,@NZORProviderNameId
		   ,@DataUrl
		   ,@WebUrl
           ,@AddedDate
           ,@AddedBy
           ,@ModifiedDate
           ,@ModifiedBy)

