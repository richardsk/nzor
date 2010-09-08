 use nzor_data_test

--test Provider

INSERT [dbo].[Provider] ([ProviderID], [Name]) VALUES (N'7B5CC893-C710-4119-ADE5-B00A997CEEAA', N'Test Provider')


 --1. simple test Family
 
INSERT INTO [cons].[Name] ([NameID] ,[FullName] ,[TaxonRankID] ,[NameClassID] ,[OriginalOrthography] ,[GoverningCode] ,[AddedDate] ,[UpdatedDate])
     VALUES '46E9A632-2B47-45A7-9146-03F4BB25B12F', 'Testaceae Smith', 'A7820DCD-0266-4300-82F5-F10F8C5D6315', 'A5233111-61A0-4AE6-9C2B-5E8E71F1473A', null, 'ICBN', getdate(), null
     
INSERT INTO [cons].[NameProperty] ([NamePropertyID] ,[NameID] ,[NameClassPropertyID] ,[Value] ,[RelatedID] ,[Sequence] ,[AddedDate] ,[UpdatedDate])
     VALUES newid(), '46E9A632-2B47-45A7-9146-03F4BB25B12F', 'A1D57520-3D64-4F7D-97C8-69B449AFA280', 'family', null, getdate(), null

INSERT INTO [cons].[NameProperty] ([NamePropertyID] ,[NameID] ,[NameClassPropertyID] ,[Value] ,[RelatedID] ,[Sequence] ,[AddedDate] ,[UpdatedDate])
     VALUES newid(), '46E9A632-2B47-45A7-9146-03F4BB25B12F', '1F64E93C-7EE8-40D7-8681-52B56060D750', 'Testaceae', null, getdate(), null
     
INSERT INTO [cons].[NameProperty] ([NamePropertyID] ,[NameID] ,[NameClassPropertyID] ,[Value] ,[RelatedID] ,[Sequence] ,[AddedDate] ,[UpdatedDate])
     VALUES newid(), '46E9A632-2B47-45A7-9146-03F4BB25B12F', '006D86A8-08A5-4C1A-BC08-C07B0225E01B', 'Smith', null, getdate(), null


     
	--provider values - prov record ID = 8061AD92-E3B1-417A-BD85-CDAA834E3A7E

INSERT INTO prov.[Name] ([NameID] ,[FullName] ,[TaxonRankID] ,[NameClassID] ,[OriginalOrthography] ,[GoverningCode] , ProviderID, ProviderRecordID, [AddedDate] ,[UpdatedDate])
     VALUES 'C6A58A2E-315E-4EDD-91C0-8663A8584C69', 'Testaceae Smith', 'A7820DCD-0266-4300-82F5-F10F8C5D6315', 'A5233111-61A0-4AE6-9C2B-5E8E71F1473A', null, 'ICBN', '7B5CC893-C710-4119-ADE5-B00A997CEEAA', '8061AD92-E3B1-417A-BD85-CDAA834E3A7E', getdate(), null
     
INSERT INTO prov.[NameProperty] ([NamePropertyID] ,[NameID] ,[NameClassPropertyID] ,[Value] ,[RelatedID] ,[Sequence] ,[AddedDate] ,[UpdatedDate])
     VALUES newid(), 'C6A58A2E-315E-4EDD-91C0-8663A8584C69', 'A1D57520-3D64-4F7D-97C8-69B449AFA280', 'family', null, getdate(), null

INSERT INTO prov.[NameProperty] ([NamePropertyID] ,[NameID] ,[NameClassPropertyID] ,[Value] ,[RelatedID] ,[Sequence] ,[AddedDate] ,[UpdatedDate])
     VALUES newid(), 'C6A58A2E-315E-4EDD-91C0-8663A8584C69', '1F64E93C-7EE8-40D7-8681-52B56060D750', 'Testaceae', null, getdate(), null
     
INSERT INTO prov.[NameProperty] ([NamePropertyID] ,[NameID] ,[NameClassPropertyID] ,[Value] ,[RelatedID] ,[Sequence] ,[AddedDate] ,[UpdatedDate])
     VALUES newid(), 'C6A58A2E-315E-4EDD-91C0-8663A8584C69', '006D86A8-08A5-4C1A-BC08-C07B0225E01B', 'Smith', null, getdate(), null
     


