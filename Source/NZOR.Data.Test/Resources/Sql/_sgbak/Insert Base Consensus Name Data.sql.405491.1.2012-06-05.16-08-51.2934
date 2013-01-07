INSERT INTO consensus.Name
           ([NameID]
           ,[TaxonRankID]
           ,[NameClassID]
           ,[FullName]
           ,[GoverningCode]
           ,[AddedDate]
           ,[ModifiedDate])
VALUES ('7C087DE1-FD0C-4997-8874-06D61D7CB244', '057D6434-A12A-460D-B705-4510603FAE4F', 'A5233111-61A0-4AE6-9C2B-5E8E71F1473A', 'ROOT', NULL, getdate(), NULL)

INSERT INTO consensus.NameProperty
           ([NamePropertyID]
           ,[NameID]
           ,[NamePropertyTypeID]
           ,[RelatedID]
           ,[Sequence]
           ,[Value])
VALUES (newid(), '7C087DE1-FD0C-4997-8874-06D61D7CB244', '1F64E93C-7EE8-40D7-8681-52B56060D750', null, null, 'ROOT')
           
insert into consensus.concept
select '87F73532-0EA2-40BB-960A-AC03C60F26F5', '7C087DE1-FD0C-4997-8874-06D61D7CB244', NULL, null, null, null, GETDATE(), null
