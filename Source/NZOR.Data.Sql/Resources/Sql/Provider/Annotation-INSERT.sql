INSERT INTO [provider].[Annotation]
           ([AnnotationID]
           ,[NameID]
           ,[ConceptID]
           ,[ReferenceID]
           ,[DataSourceID]
           ,[ConsensusAnnotationID]
           ,[AnnotationType]
           ,[AnnotationText]
           ,[ProviderNameID]
           ,[ProviderConceptID]
           ,[ProviderReferenceID]
           ,[ProviderRecordID]
           ,[ProviderCreatedDate]
           ,[ProviderModifiedDate]
           ,[AddedDate]
           ,[ModifiedDate])
     VALUES
           (@AnnotationID
           ,@NameID
           ,@ConceptID
           ,@ReferenceID
           ,@DataSourceID
           ,@ConsensusAnnotationID
           ,@AnnotationType
           ,@AnnotationText
           ,@ProviderNameID
           ,@ProviderConceptID
           ,@ProviderReferenceID
           ,@ProviderRecordID
           ,@ProviderCreatedDate
           ,@ProviderModifiedDate
           ,@AddedDate
           ,@ModifiedDate)


