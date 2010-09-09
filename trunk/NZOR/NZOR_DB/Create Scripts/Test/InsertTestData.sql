use nzor_data_test


--delete old data
delete cr 
from cons.ConceptRelationship cr
inner join cons.Concept c on c.ConceptID = cr.FromConceptID
inner join cons.Name n on n.NameID = c.NameID
inner join dbo.TaxonRank tr on tr.TaxonRankID = n.TaxonRankID
where tr.SortOrder > 1600

delete c
from cons.Concept c 
inner join cons.Name n on n.NameID = c.NameID
inner join dbo.TaxonRank tr on tr.TaxonRankID = n.TaxonRankID
where tr.SortOrder > 1600

delete np
from cons.NameProperty np 
inner join cons.Name n on n.NameID = np.NameID
inner join dbo.TaxonRank tr on tr.TaxonRankID = n.TaxonRankID
where tr.SortOrder > 1600

delete n
from cons.Name n
inner join dbo.TaxonRank tr on tr.TaxonRankID = n.TaxonRankID
where SortOrder > 1600


delete cr 
from prov.ConceptRelationship cr
inner join prov.Concept c on c.ConceptID = cr.FromConceptID
inner join prov.Name n on n.NameID = c.NameID
inner join dbo.TaxonRank tr on tr.TaxonRankID = n.TaxonRankID
where tr.SortOrder > 1600 or n.providerid = '7B5CC893-C710-4119-ADE5-B00A997CEEAA'

delete c
from prov.Concept c 
inner join prov.Name n on n.NameID = c.NameID
inner join dbo.TaxonRank tr on tr.TaxonRankID = n.TaxonRankID
where tr.SortOrder > 1600 or n.providerid = '7B5CC893-C710-4119-ADE5-B00A997CEEAA'

delete np
from prov.NameProperty np 
inner join prov.Name n on n.NameID = np.NameID
inner join dbo.TaxonRank tr on tr.TaxonRankID = n.TaxonRankID
where tr.SortOrder > 1600 or n.providerid = '7B5CC893-C710-4119-ADE5-B00A997CEEAA'

delete n
from prov.Name n
inner join dbo.TaxonRank tr on tr.TaxonRankID = n.TaxonRankID
where SortOrder > 1600 or n.providerid = '7B5CC893-C710-4119-ADE5-B00A997CEEAA'


delete dbo.Provider where [Name] = 'Test Provider'


go

--test Provider
INSERT [dbo].[Provider] ([ProviderID], [Name]) VALUES (N'7B5CC893-C710-4119-ADE5-B00A997CEEAA', N'Test Provider')


 -------------   1. simple test Family   ---------------------
 
INSERT INTO [cons].[Name] ([NameID] ,[FullName] ,[TaxonRankID] ,[NameClassID] ,[OriginalOrthography] ,[GoverningCode] ,[AddedDate] ,[UpdatedDate])
     VALUES ( '46E9A632-2B47-45A7-9146-03F4BB25B12F', 'Testaceae Smith', 'A7820DCD-0266-4300-82F5-F10F8C5D6315', 'A5233111-61A0-4AE6-9C2B-5E8E71F1473A', null, 'ICBN', getdate(), null )
     
INSERT INTO [cons].[NameProperty] ([NamePropertyID] ,[NameID] ,[NameClassPropertyID] ,[Value] ,[RelatedID] ,[Sequence] ,[AddedDate] ,[UpdatedDate])
     VALUES (newid(), '46E9A632-2B47-45A7-9146-03F4BB25B12F', 'A1D57520-3D64-4F7D-97C8-69B449AFA280', 'family', null, null, getdate(), null)

INSERT INTO [cons].[NameProperty] ([NamePropertyID] ,[NameID] ,[NameClassPropertyID] ,[Value] ,[RelatedID] ,[Sequence] ,[AddedDate] ,[UpdatedDate])
     VALUES (newid(), '46E9A632-2B47-45A7-9146-03F4BB25B12F', '1F64E93C-7EE8-40D7-8681-52B56060D750', 'Testaceae', null, null, getdate(), null)
     
INSERT INTO [cons].[NameProperty] ([NamePropertyID] ,[NameID] ,[NameClassPropertyID] ,[Value] ,[RelatedID] ,[Sequence] ,[AddedDate] ,[UpdatedDate])
     VALUES (newid(), '46E9A632-2B47-45A7-9146-03F4BB25B12F', '006D86A8-08A5-4C1A-BC08-C07B0225E01B', 'Smith', null, null, getdate(), null)

INSERT INTO [cons].[Concept] ([ConceptID] ,[NameID] ,[AccordingToReferenceID] ,[Orthography] ,[AddedDate] ,[UpdatedDate])
     VALUES ('4FCFFFDF-9C7D-4023-B586-A54776479695', '46E9A632-2B47-45A7-9146-03F4BB25B12F', null, null, getdate(), null)
     
--join to Asterales
INSERT INTO [cons].[ConceptRelationship] ([ConceptRelationshipID] ,[FromConceptID] ,[ToConceptID] ,[ConceptRelationshipTypeID] ,[Sequence] ,[AddedDate] ,[UpdatedDate])
     VALUES (newid(), '4FCFFFDF-9C7D-4023-B586-A54776479695', 'F8C457FB-F657-4D3C-AEEB-482708DE53EC', '6A11B466-1907-446F-9229-D604579AA155', null, getdate(), null)


go
     
	--provider values - 

	--insert provider name to match 'Asterales', prov record id = D3B4BC87-2D0F-4F55-829B-6400DA599021
INSERT INTO prov.[Name] ([NameID] ,[FullName] ,[TaxonRankID] ,[NameClassID] ,[OriginalOrthography] ,[GoverningCode] , ProviderID, ProviderRecordID, [AddedDate] ,[ProviderUpdatedDate])
     VALUES ('E6AB7DCC-45CD-43B1-A353-DC62BE296847', 'Asterales', '2B1966D4-720B-4F58-9C01-1280E1BB0DAB', 'A5233111-61A0-4AE6-9C2B-5E8E71F1473A', null, 'ICBN', '7B5CC893-C710-4119-ADE5-B00A997CEEAA', 'D3B4BC87-2D0F-4F55-829B-6400DA599021', getdate(), getdate())
     
INSERT INTO prov.[NameProperty] ([NamePropertyID] ,[NameID] ,[NameClassPropertyID] ,[Value] ,[RelatedID] ,[Sequence])
     VALUES (newid(), 'E6AB7DCC-45CD-43B1-A353-DC62BE296847', 'A1D57520-3D64-4F7D-97C8-69B449AFA280', 'order', null, null)

INSERT INTO prov.[NameProperty] ([NamePropertyID] ,[NameID] ,[NameClassPropertyID] ,[Value] ,[RelatedID] ,[Sequence])
     VALUES (newid(), 'E6AB7DCC-45CD-43B1-A353-DC62BE296847', '1F64E93C-7EE8-40D7-8681-52B56060D750', 'Asterales', null, null)
  
go        
     
     --insert provider family, prov record ID = 8061AD92-E3B1-417A-BD85-CDAA834E3A7E
INSERT INTO prov.[Name] ([NameID] ,[FullName] ,[TaxonRankID] ,[NameClassID] ,[OriginalOrthography] ,[GoverningCode] , ProviderID, ProviderRecordID, [AddedDate] ,[ProviderUpdatedDate])
     VALUES ('C6A58A2E-315E-4EDD-91C0-8663A8584C69', 'Testaceae Smith', 'A7820DCD-0266-4300-82F5-F10F8C5D6315', '7B5CC893-C710-4119-ADE5-B00A997CEEAA', null, 'ICBN', '7B5CC893-C710-4119-ADE5-B00A997CEEAA', '8061AD92-E3B1-417A-BD85-CDAA834E3A7E', getdate(), getdate())
     
INSERT INTO prov.[NameProperty] ([NamePropertyID] ,[NameID] ,[NameClassPropertyID] ,[Value] ,[RelatedID] ,[Sequence])
     VALUES (newid(), 'C6A58A2E-315E-4EDD-91C0-8663A8584C69', 'A1D57520-3D64-4F7D-97C8-69B449AFA280', 'family', null, null)

INSERT INTO prov.[NameProperty] ([NamePropertyID] ,[NameID] ,[NameClassPropertyID] ,[Value] ,[RelatedID] ,[Sequence])
     VALUES (newid(), 'C6A58A2E-315E-4EDD-91C0-8663A8584C69', '1F64E93C-7EE8-40D7-8681-52B56060D750', 'Testaceae', null, null)
     
INSERT INTO prov.[NameProperty] ([NamePropertyID] ,[NameID] ,[NameClassPropertyID] ,[Value] ,[RelatedID] ,[Sequence])
     VALUES (newid(), 'C6A58A2E-315E-4EDD-91C0-8663A8584C69', '006D86A8-08A5-4C1A-BC08-C07B0225E01B', 'Smith', null, null)

go
     
	--child of Asterales
INSERT INTO prov.[Concept] ([ConceptID] ,[NameID] ,[AccordingToReferenceID] ,[Orthography] ,[AddedDate] ,[ProviderUpdatedDate], ProviderID, ProviderRecordID)
     VALUES ('D1CB0D2E-6EB5-44BD-832E-26CCB381A3C2', 'C6A58A2E-315E-4EDD-91C0-8663A8584C69', null, null, getdate(), getdate(), '7B5CC893-C710-4119-ADE5-B00A997CEEAA', newid()) --family concept
INSERT INTO prov.[Concept] ([ConceptID] ,[NameID] ,[AccordingToReferenceID] ,[Orthography] ,[AddedDate] ,[ProviderUpdatedDate], ProviderID, ProviderRecordID)
     VALUES ('96FFB85B-7CB0-4783-9170-BEEE0DD02C3A', 'E6AB7DCC-45CD-43B1-A353-DC62BE296847', null, null, getdate(), getdate(), '7B5CC893-C710-4119-ADE5-B00A997CEEAA', newid()) --order concept
     
INSERT INTO prov.[ConceptRelationship] ([ConceptRelationshipID] ,[FromConceptID] ,[ToConceptID] ,[RelationshipTypeID] ,[Sequence])
     VALUES (newid(), 'D1CB0D2E-6EB5-44BD-832E-26CCB381A3C2', '96FFB85B-7CB0-4783-9170-BEEE0DD02C3A', '6A11B466-1907-446F-9229-D604579AA155', null)
	

