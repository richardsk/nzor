INSERT INTO [dbo].[Deprecated]
           ([DeprecatedId]
           ,[Table]
           ,[OldId]
           ,[NewId]
           ,[DeprecationDate])
     VALUES
           (newid(),
		   @table,
		   @oldId,
		   @newId,
		   @deprecationDate
		   )


