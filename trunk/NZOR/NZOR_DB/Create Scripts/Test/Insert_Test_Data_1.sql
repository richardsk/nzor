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
from provider.ConceptRelationship cr
inner join provider.Concept c on c.ConceptID = cr.FromConceptID
inner join provider.Name n on n.NameID = c.NameID
inner join dbo.TaxonRank tr on tr.TaxonRankID = n.TaxonRankID
where tr.SortOrder > 1600 or n.SubDataSetID = '38708533-E064-45A4-AB00-CDD76075C2B6'

delete c
from provider.Concept c 
inner join provider.Name n on n.NameID = c.NameID
inner join dbo.TaxonRank tr on tr.TaxonRankID = n.TaxonRankID
where tr.SortOrder > 1600 or n.SubDataSetID = '38708533-E064-45A4-AB00-CDD76075C2B6'

delete np
from provider.NameProperty np 
inner join provider.Name n on n.NameID = np.NameID
inner join dbo.TaxonRank tr on tr.TaxonRankID = n.TaxonRankID
where tr.SortOrder > 1600 or n.SubDataSetID = '38708533-E064-45A4-AB00-CDD76075C2B6'

delete n
from provider.Name n
inner join dbo.TaxonRank tr on tr.TaxonRankID = n.TaxonRankID
where SortOrder > 1600 or n.SubDataSetID = '38708533-E064-45A4-AB00-CDD76075C2B6'

delete dbo.SubDataSet where ProviderID = '7B5CC893-C710-4119-ADE5-B00A997CEEAA'
delete dbo.Provider where [Name] = 'Test Provider'


go
 
--insert concept for asterales? plantae?
if (not exists(select conceptid from cons.Name pn
	inner join vwConsensusConcepts cc on cc.NameID = pn.NameID
	where pn.FullName like 'Asterales%' and conceptrelationshiptypeid = '6A11B466-1907-446F-9229-D604579AA155'))
begin

	INSERT INTO [cons].[Concept] ([ConceptID] ,[NameID] ,[AccordingToReferenceID] ,[Orthography] ,[AddedDate] ,[UpdatedDate])
		 VALUES ('D984B42A-2B60-444D-AD72-430C9811A5F5', '63707033-44C0-4EE0-AF00-21B594DB8E55', null, null, getdate(), null) --asterales
		 
	INSERT INTO [cons].[Concept] ([ConceptID] ,[NameID] ,[AccordingToReferenceID] ,[Orthography] ,[AddedDate] ,[UpdatedDate])
		 VALUES ('52E8DE21-5AA7-4D66-A92A-7E8BAF477986', 'AD3ADA71-589B-4160-831F-D289CBFE974D', null, null, getdate(), null) --plantae

	INSERT INTO [cons].[ConceptRelationship] ([ConceptRelationshipID] ,[FromConceptID] ,[ToConceptID] ,[ConceptRelationshipTypeID] ,[Sequence] ,[AddedDate] ,[UpdatedDate])
		 VALUES (newid(), 'D984B42A-2B60-444D-AD72-430C9811A5F5', '52E8DE21-5AA7-4D66-A92A-7E8BAF477986', '6A11B466-1907-446F-9229-D604579AA155', null, getdate(), null)		

	--plantae to root
	INSERT INTO [cons].[ConceptRelationship] ([ConceptRelationshipID] ,[FromConceptID] ,[ToConceptID] ,[ConceptRelationshipTypeID] ,[Sequence] ,[AddedDate] ,[UpdatedDate])
		 VALUES (newid(), '52E8DE21-5AA7-4D66-A92A-7E8BAF477986', 'C666C820-0894-430B-AA12-F909663CB0F4', '6A11B466-1907-446F-9229-D604579AA155', null, getdate(), null)		
		 
end


--test Provider
INSERT [dbo].[Provider] ([ProviderID], [Name], Code) VALUES (N'7B5CC893-C710-4119-ADE5-B00A997CEEAA', N'Test Provider', 'TEST')
insert dbo.SubDataSet values('38708533-E064-45A4-AB00-CDD76075C2B6', '7B5CC893-C710-4119-ADE5-B00A997CEEAA', 'TEST_DS')

 -------------   1. simple test Family   ---------------------
 
INSERT INTO [cons].[Name] ([NameID] ,[FullName] ,[TaxonRankID] ,[NameClassID] ,[GoverningCode] ,[AddedDate] ,[UpdatedDate])
     VALUES ( '46E9A632-2B47-45A7-9146-03F4BB25B12F', 'Testaceae Smith', 'A7820DCD-0266-4300-82F5-F10F8C5D6315', 'A5233111-61A0-4AE6-9C2B-5E8E71F1473A', 'ICBN', getdate(), null )
     
INSERT INTO [cons].[NameProperty] ([NamePropertyID] ,[NameID] ,[NameClassPropertyID] ,[Value] ,[RelatedID] ,[Sequence] ,[AddedDate] ,[UpdatedDate])
     VALUES (newid(), '46E9A632-2B47-45A7-9146-03F4BB25B12F', 'A1D57520-3D64-4F7D-97C8-69B449AFA280', 'family', null, null, getdate(), null)

INSERT INTO [cons].[NameProperty] ([NamePropertyID] ,[NameID] ,[NameClassPropertyID] ,[Value] ,[RelatedID] ,[Sequence] ,[AddedDate] ,[UpdatedDate])
     VALUES (newid(), '46E9A632-2B47-45A7-9146-03F4BB25B12F', '1F64E93C-7EE8-40D7-8681-52B56060D750', 'Testaceae', null, null, getdate(), null)
     
INSERT INTO [cons].[NameProperty] ([NamePropertyID] ,[NameID] ,[NameClassPropertyID] ,[Value] ,[RelatedID] ,[Sequence] ,[AddedDate] ,[UpdatedDate])
     VALUES (newid(), '46E9A632-2B47-45A7-9146-03F4BB25B12F', '006D86A8-08A5-4C1A-BC08-C07B0225E01B', 'Smith', null, null, getdate(), null)

INSERT INTO [cons].[Concept] ([ConceptID] ,[NameID] ,[AccordingToReferenceID] ,[Orthography] ,[AddedDate] ,[UpdatedDate])
     VALUES ('4FCFFFDF-9C7D-4023-B586-A54776479695', '46E9A632-2B47-45A7-9146-03F4BB25B12F', null, null, getdate(), null)
     
--join to Asterales
declare @cid uniqueidentifier
select @cid = c.ConceptID
from cons.Name n
inner join cons.Concept c on c.NameID = n.NameID
where FullName like 'asterales%'

INSERT INTO [cons].[ConceptRelationship] ([ConceptRelationshipID] ,[FromConceptID] ,[ToConceptID] ,[ConceptRelationshipTypeID] ,[Sequence] ,[AddedDate] ,[UpdatedDate])
     VALUES (newid(), '4FCFFFDF-9C7D-4023-B586-A54776479695', @cid, '6A11B466-1907-446F-9229-D604579AA155', null, getdate(), null)

go
	--flat name
	delete cons.FlatName where seednameid = '46E9A632-2B47-45A7-9146-03F4BB25B12F'
    INSERT cons.FlatName EXEC sprSelect_FlatNameToRoot '46E9A632-2B47-45A7-9146-03F4BB25B12F'


go
     
	--provider values - 

	--insert provider name to match 'Asterales', prov record id = D3B4BC87-2D0F-4F55-829B-6400DA599021
INSERT INTO provider.[Name] ([NameID] ,[LinkStatus], [FullName] ,[TaxonRankID] ,[NameClassID] ,[GoverningCode] , SubDataSetID, ProviderRecordID, [AddedDate] ,[ProviderModifiedDate])
     VALUES ('E6AB7DCC-45CD-43B1-A353-DC62BE296847', 'Unmatched', 'Asterales', '2B1966D4-720B-4F58-9C01-1280E1BB0DAB', 'A5233111-61A0-4AE6-9C2B-5E8E71F1473A', 'ICBN', '38708533-E064-45A4-AB00-CDD76075C2B6', 'D3B4BC87-2D0F-4F55-829B-6400DA599021', getdate(), getdate())
     
INSERT INTO provider.[NameProperty] ([NamePropertyID] ,[NameID] ,[NameClassPropertyID] ,[Value] ,[RelatedID] ,[Sequence])
     VALUES (newid(), 'E6AB7DCC-45CD-43B1-A353-DC62BE296847', 'A1D57520-3D64-4F7D-97C8-69B449AFA280', 'order', null, null)

INSERT INTO provider.[NameProperty] ([NamePropertyID] ,[NameID] ,[NameClassPropertyID] ,[Value] ,[RelatedID] ,[Sequence])
     VALUES (newid(), 'E6AB7DCC-45CD-43B1-A353-DC62BE296847', '1F64E93C-7EE8-40D7-8681-52B56060D750', 'Asterales', null, null)
  
go        
     
     --insert provider family, prov record ID = 8061AD92-E3B1-417A-BD85-CDAA834E3A7E
INSERT INTO provider.[Name] ([NameID], LinkStatus ,[FullName] ,[TaxonRankID] ,[NameClassID] ,[GoverningCode] , SubDataSetID, ProviderRecordID, [AddedDate] ,[ProviderModifiedDate])
     VALUES ('C6A58A2E-315E-4EDD-91C0-8663A8584C69','Unmatched',  'Testaceae Smith', 'A7820DCD-0266-4300-82F5-F10F8C5D6315', 'A5233111-61A0-4AE6-9C2B-5E8E71F1473A', 'ICBN', '38708533-E064-45A4-AB00-CDD76075C2B6', '8061AD92-E3B1-417A-BD85-CDAA834E3A7E', getdate(), getdate())
     
INSERT INTO provider.[NameProperty] ([NamePropertyID] ,[NameID] ,[NameClassPropertyID] ,[Value] ,[RelatedID] ,[Sequence])
     VALUES (newid(), 'C6A58A2E-315E-4EDD-91C0-8663A8584C69', 'A1D57520-3D64-4F7D-97C8-69B449AFA280', 'family', null, null)

INSERT INTO provider.[NameProperty] ([NamePropertyID] ,[NameID] ,[NameClassPropertyID] ,[Value] ,[RelatedID] ,[Sequence])
     VALUES (newid(), 'C6A58A2E-315E-4EDD-91C0-8663A8584C69', '1F64E93C-7EE8-40D7-8681-52B56060D750', 'Testaceae', null, null)
     
INSERT INTO provider.[NameProperty] ([NamePropertyID] ,[NameID] ,[NameClassPropertyID] ,[Value] ,[RelatedID] ,[Sequence])
     VALUES (newid(), 'C6A58A2E-315E-4EDD-91C0-8663A8584C69', '006D86A8-08A5-4C1A-BC08-C07B0225E01B', 'Smith', null, null)

go
     
	--child of Asterales
INSERT INTO provider.[Concept] ([ConceptID], LinkStatus,[NameID] ,[AccordingToReferenceID] ,[Orthography] ,[AddedDate] ,[ProviderModifiedDate], SubDataSetID, ProviderRecordID)
     VALUES ('D1CB0D2E-6EB5-44BD-832E-26CCB381A3C2', 'Unmatched', 'C6A58A2E-315E-4EDD-91C0-8663A8584C69', null, null, getdate(), getdate(), '38708533-E064-45A4-AB00-CDD76075C2B6', newid()) --family concept
INSERT INTO provider.[Concept] ([ConceptID], LinkStatus ,[NameID] ,[AccordingToReferenceID] ,[Orthography] ,[AddedDate] ,[ProviderModifiedDate], SubDataSetID, ProviderRecordID)
     VALUES ('96FFB85B-7CB0-4783-9170-BEEE0DD02C3A', 'Unmatched', 'E6AB7DCC-45CD-43B1-A353-DC62BE296847', null, null, getdate(), getdate(), '38708533-E064-45A4-AB00-CDD76075C2B6', newid()) --order concept
     
INSERT INTO provider.[ConceptRelationship] ([ConceptRelationshipID] ,[FromConceptID] ,[ToConceptID] ,[ConceptRelationshipTypeID] ,[Sequence], InUse)
     VALUES (newid(), 'D1CB0D2E-6EB5-44BD-832E-26CCB381A3C2', '96FFB85B-7CB0-4783-9170-BEEE0DD02C3A', '6A11B466-1907-446F-9229-D604579AA155', null, 1)
	


-------------   2. simple test Genus    ---------------------
 
INSERT INTO [cons].[Name] ([NameID] ,[FullName] ,[TaxonRankID] ,[NameClassID] ,[GoverningCode] ,[AddedDate] ,[UpdatedDate])
     VALUES ( '87C799BC-DA5F-404C-921C-5BBF12A9A52C', 'Testgenus Sm.', '20552EB6-1BF0-4073-A021-A6C7A89B7F14', 'A5233111-61A0-4AE6-9C2B-5E8E71F1473A', 'ICBN', getdate(), null )
     
INSERT INTO [cons].[NameProperty] ([NamePropertyID] ,[NameID] ,[NameClassPropertyID] ,[Value] ,[RelatedID] ,[Sequence] ,[AddedDate] ,[UpdatedDate])
     VALUES (newid(), '87C799BC-DA5F-404C-921C-5BBF12A9A52C', 'A1D57520-3D64-4F7D-97C8-69B449AFA280', 'genus', null, null, getdate(), null)

INSERT INTO [cons].[NameProperty] ([NamePropertyID] ,[NameID] ,[NameClassPropertyID] ,[Value] ,[RelatedID] ,[Sequence] ,[AddedDate] ,[UpdatedDate])
     VALUES (newid(), '87C799BC-DA5F-404C-921C-5BBF12A9A52C', '1F64E93C-7EE8-40D7-8681-52B56060D750', 'Testgenus', null, null, getdate(), null)
     
INSERT INTO [cons].[NameProperty] ([NamePropertyID] ,[NameID] ,[NameClassPropertyID] ,[Value] ,[RelatedID] ,[Sequence] ,[AddedDate] ,[UpdatedDate])
     VALUES (newid(), '87C799BC-DA5F-404C-921C-5BBF12A9A52C', '006D86A8-08A5-4C1A-BC08-C07B0225E01B', 'Sm.', null, null, getdate(), null)

INSERT INTO [cons].[Concept] ([ConceptID] ,[NameID] ,[AccordingToReferenceID] ,[Orthography] ,[AddedDate] ,[UpdatedDate])
     VALUES ('2632F79D-0A26-4E2E-AD74-5C01E6F9539C', '87C799BC-DA5F-404C-921C-5BBF12A9A52C', null, null, getdate(), null)
     
--join to Testaceae
INSERT INTO [cons].[ConceptRelationship] ([ConceptRelationshipID] ,[FromConceptID] ,[ToConceptID] ,[ConceptRelationshipTypeID] ,[Sequence] ,[AddedDate] ,[UpdatedDate])
     VALUES (newid(), '2632F79D-0A26-4E2E-AD74-5C01E6F9539C', '4FCFFFDF-9C7D-4023-B586-A54776479695', '6A11B466-1907-446F-9229-D604579AA155', null, getdate(), null)

go
	--flat name
	delete cons.FlatName where seednameid = '87C799BC-DA5F-404C-921C-5BBF12A9A52C'
    INSERT cons.FlatName EXEC sprSelect_FlatNameToRoot '87C799BC-DA5F-404C-921C-5BBF12A9A52C'

go
     
	--provider values - 

	--insert provider name to match 'Testgenus', prov record id = 6816FBAC-80C2-4533-8986-5235A15B051C
INSERT INTO provider.[Name] ([NameID], LinkStatus ,[FullName] ,[TaxonRankID] ,[NameClassID] ,[GoverningCode] , SubDataSetID, ProviderRecordID, [AddedDate] ,[ProviderModifiedDate])
     VALUES ('3CF39BEE-E713-4063-9CA5-5EB05D6CE8F1', 'Unmatched', 'Testgenus', '20552EB6-1BF0-4073-A021-A6C7A89B7F14', 'A5233111-61A0-4AE6-9C2B-5E8E71F1473A', 'ICBN', '38708533-E064-45A4-AB00-CDD76075C2B6', '6816FBAC-80C2-4533-8986-5235A15B051C', getdate(), getdate())
     
INSERT INTO provider.[NameProperty] ([NamePropertyID] ,[NameID] ,[NameClassPropertyID] ,[Value] ,[RelatedID] ,[Sequence])
     VALUES (newid(), '3CF39BEE-E713-4063-9CA5-5EB05D6CE8F1', 'A1D57520-3D64-4F7D-97C8-69B449AFA280', 'genus', null, null)

INSERT INTO provider.[NameProperty] ([NamePropertyID] ,[NameID] ,[NameClassPropertyID] ,[Value] ,[RelatedID] ,[Sequence])
     VALUES (newid(), '3CF39BEE-E713-4063-9CA5-5EB05D6CE8F1', '1F64E93C-7EE8-40D7-8681-52B56060D750', 'Testgenus', null, null)
  
INSERT INTO provider.[NameProperty] ([NamePropertyID] ,[NameID] ,[NameClassPropertyID] ,[Value] ,[RelatedID] ,[Sequence])
     VALUES (newid(), '3CF39BEE-E713-4063-9CA5-5EB05D6CE8F1', '006D86A8-08A5-4C1A-BC08-C07B0225E01B', 'Brown ex Sm.', null, null)

go
     
	--child of Testaceae
INSERT INTO provider.[Concept] ([ConceptID], LinkStatus ,[NameID] ,[AccordingToReferenceID] ,[Orthography] ,[AddedDate] ,[ProviderModifiedDate], SubDataSetID, ProviderRecordID)
     VALUES ('7D82F575-5C79-413A-B8AF-98D014370F39', 'Unmatched', '3CF39BEE-E713-4063-9CA5-5EB05D6CE8F1', null, null, getdate(), getdate(), '38708533-E064-45A4-AB00-CDD76075C2B6', newid()) --genus concept
     
INSERT INTO provider.[ConceptRelationship] ([ConceptRelationshipID] ,[FromConceptID] ,[ToConceptID] ,[ConceptRelationshipTypeID] ,[Sequence], InUse)
     VALUES (newid(), '7D82F575-5C79-413A-B8AF-98D014370F39', 'D1CB0D2E-6EB5-44BD-832E-26CCB381A3C2', '6A11B466-1907-446F-9229-D604579AA155', null, 1)
     
go


-------------   2. simple test Species    ---------------------
 
INSERT INTO [cons].[Name] ([NameID] ,[FullName] ,[TaxonRankID] ,[NameClassID] ,[GoverningCode] ,[AddedDate] ,[UpdatedDate])
     VALUES ( '79B46D62-978E-4DFD-A634-559D399E006B', 'Testgenus testsp Smith', 'C21BB221-5291-4540-94D1-55A12D1BD0AD', 'A5233111-61A0-4AE6-9C2B-5E8E71F1473A', 'ICBN', getdate(), null )
     
INSERT INTO [cons].[NameProperty] ([NamePropertyID] ,[NameID] ,[NameClassPropertyID] ,[Value] ,[RelatedID] ,[Sequence] ,[AddedDate] ,[UpdatedDate])
     VALUES (newid(), '79B46D62-978E-4DFD-A634-559D399E006B', 'A1D57520-3D64-4F7D-97C8-69B449AFA280', 'species', null, null, getdate(), null)

INSERT INTO [cons].[NameProperty] ([NamePropertyID] ,[NameID] ,[NameClassPropertyID] ,[Value] ,[RelatedID] ,[Sequence] ,[AddedDate] ,[UpdatedDate])
     VALUES (newid(), '79B46D62-978E-4DFD-A634-559D399E006B', '1F64E93C-7EE8-40D7-8681-52B56060D750', 'testsp', null, null, getdate(), null)
     
INSERT INTO [cons].[NameProperty] ([NamePropertyID] ,[NameID] ,[NameClassPropertyID] ,[Value] ,[RelatedID] ,[Sequence] ,[AddedDate] ,[UpdatedDate])
     VALUES (newid(), '79B46D62-978E-4DFD-A634-559D399E006B', '006D86A8-08A5-4C1A-BC08-C07B0225E01B', 'Smith', null, null, getdate(), null)

INSERT INTO [cons].[Concept] ([ConceptID] ,[NameID] ,[AccordingToReferenceID] ,[Orthography] ,[AddedDate] ,[UpdatedDate])
     VALUES ('8B40DCC7-4C75-43D2-8297-59548ABE31B9', '79B46D62-978E-4DFD-A634-559D399E006B', null, null, getdate(), null)
     
--join to Testgenus
INSERT INTO [cons].[ConceptRelationship] ([ConceptRelationshipID] ,[FromConceptID] ,[ToConceptID] ,[ConceptRelationshipTypeID] ,[Sequence] ,[AddedDate] ,[UpdatedDate])
     VALUES (newid(), '8B40DCC7-4C75-43D2-8297-59548ABE31B9', '2632F79D-0A26-4E2E-AD74-5C01E6F9539C', '6A11B466-1907-446F-9229-D604579AA155', null, getdate(), null)

go
	--flat name
	delete cons.FlatName where seednameid = '79B46D62-978E-4DFD-A634-559D399E006B'
    INSERT cons.FlatName EXEC sprSelect_FlatNameToRoot '79B46D62-978E-4DFD-A634-559D399E006B'

go
     
	--provider values - 

	--insert provider name to match 'Testgenus testsp', prov record id = D3056447-38A7-43B5-B487-FF5F1ED90434
INSERT INTO provider.[Name] ([NameID], LinkStatus ,[FullName] ,[TaxonRankID] ,[NameClassID] ,[GoverningCode] , SubDataSetID, ProviderRecordID, [AddedDate] ,[ProviderModifiedDate])
     VALUES ('10A906E5-0CAB-4524-9BFC-FCD728D19060', 'Unmatched', 'Testgenus testsp Smith', 'C21BB221-5291-4540-94D1-55A12D1BD0AD', 'A5233111-61A0-4AE6-9C2B-5E8E71F1473A', 'ICBN', '38708533-E064-45A4-AB00-CDD76075C2B6', 'D3056447-38A7-43B5-B487-FF5F1ED90434', getdate(), getdate())
     
INSERT INTO provider.[NameProperty] ([NamePropertyID] ,[NameID] ,[NameClassPropertyID] ,[Value] ,[RelatedID] ,[Sequence])
     VALUES (newid(), '10A906E5-0CAB-4524-9BFC-FCD728D19060', 'A1D57520-3D64-4F7D-97C8-69B449AFA280', 'species', null, null)

INSERT INTO provider.[NameProperty] ([NamePropertyID] ,[NameID] ,[NameClassPropertyID] ,[Value] ,[RelatedID] ,[Sequence])
     VALUES (newid(), '10A906E5-0CAB-4524-9BFC-FCD728D19060', '1F64E93C-7EE8-40D7-8681-52B56060D750', 'testsp', null, null)
  
INSERT INTO provider.[NameProperty] ([NamePropertyID] ,[NameID] ,[NameClassPropertyID] ,[Value] ,[RelatedID] ,[Sequence])
     VALUES (newid(), '10A906E5-0CAB-4524-9BFC-FCD728D19060', '006D86A8-08A5-4C1A-BC08-C07B0225E01B', 'Smith', null, null)

go
     
	--child of Testgenus
INSERT INTO provider.[Concept] ([ConceptID], LinkStatus ,[NameID] ,[AccordingToReferenceID] ,[Orthography] ,[AddedDate] ,[ProviderModifiedDate], SubDataSetID, ProviderRecordID)
     VALUES ('3C608D14-757D-4E70-A895-7363C29F45B4', 'Unmatched', '10A906E5-0CAB-4524-9BFC-FCD728D19060', null, null, getdate(), getdate(), '38708533-E064-45A4-AB00-CDD76075C2B6', newid()) --species concept
     
INSERT INTO provider.[ConceptRelationship] ([ConceptRelationshipID] ,[FromConceptID] ,[ToConceptID] ,[ConceptRelationshipTypeID] ,[Sequence], InUse)
     VALUES (newid(), '3C608D14-757D-4E70-A895-7363C29F45B4', '7D82F575-5C79-413A-B8AF-98D014370F39', '6A11B466-1907-446F-9229-D604579AA155', null, 1)
     
go


	

