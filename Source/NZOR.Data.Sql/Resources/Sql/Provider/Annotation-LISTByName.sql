SELECT [AnnotationID]
      ,a.[NameID]
      ,a.[ConceptID]
      ,a.[ReferenceID]
      ,a.[DataSourceID]
      ,[ConsensusAnnotationID]
      ,[AnnotationType]
      ,[AnnotationText]
      ,a.[ProviderNameID]
      ,a.[ProviderConceptID]
      ,a.[ProviderReferenceID]
      ,a.[ProviderRecordID]
      ,a.[ProviderCreatedDate]
      ,a.[ProviderModifiedDate]
      ,a.[AddedDate]
      ,a.[ModifiedDate]
from provider.Annotation a
WHERE 
	a.NameID = @NameID
ORDER BY 
	ProviderRecordID
