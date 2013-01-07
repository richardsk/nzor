UPDATE [provider].[Annotation]
   SET [NameID] = @NameID
      ,[ConceptID] = @ConceptID
      ,[ReferenceID] = @ReferenceID
      ,[DataSourceID] = @DataSourceID
      ,[ConsensusAnnotationID] = @ConsensusAnnotationID
      ,[AnnotationType] = @AnnotationType
      ,[AnnotationText] = @AnnotationText
      ,[ProviderNameID] = @ProviderNameID
      ,[ProviderConceptID] = @ProviderConceptID
      ,[ProviderReferenceID] = @ProviderReferenceID
      ,[ProviderRecordID] = @ProviderRecordID
      ,[ProviderCreatedDate] = @ProviderCreatedDate
      ,[ProviderModifiedDate] = @ProviderModifiedDate
      ,[AddedDate] = @AddedDate
      ,[ModifiedDate] = @ModifiedDate
 WHERE [AnnotationID] = @AnnotationID


