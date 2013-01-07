UPDATE [admin].[NameRequest]
   SET [FullName] = @FullName
	  ,[RequestDate] = @RequestDate
      ,[ApiKey] = @ApiKey
	  ,[BatchMatchId] = @BatchMatchId
      ,[RequesterEmail] = @RequesterEmail
	  ,[ExternalLookupServiceId] = @ExternalLookupServiceId
	  ,[ExternalLookupDataUrl] = @ExternalLookupDataUrl
      ,[Status] = @Status
      ,[AddedDate] = @AddedDate
      ,[AddedBy] = @AddedBy
      ,[ModifiedDate] = @ModifiedDate
      ,[ModifiedBy] = @ModifiedBy
 WHERE [NameRequestId] = @NameRequestId

