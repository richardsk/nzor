INSERT INTO
	[admin].Provider(ProviderId, Name, Code)
VALUES (N'd0b0b9fb-bda0-43a6-9bb2-07b6f3db2c26', N'NZOR Test', N'NZOR_Test'),
	(N'e4e30dc8-c708-445f-86c3-235e0028f129', N'New Zealand Fungi Names Database', N'NZFUNGI'),
	(N'e5afbf6d-d7f7-4f01-b107-70e3a8bfcd91', N'NIWA', N'NIWA'),
	(N'b8e1ef06-1f7d-43ce-bf30-71735e600a96', N'Plant Names Database', N'NZFLORA'),
	(N'91cbf580-e513-4ca4-89af-724253a393ac', N'New Zealand Inventory of Biodiversity', N'NZIB'),
	(N'14afd938-734e-40f5-9021-f252b7357bc8', N'Catalog Of Life', N'CoL2010')

INSERT INTO
	[admin].DataSource(DataSourceId, ProviderId, Name, Code, AddedDate, AddedBy)
values (N'175d49cd-0785-4008-bb56-04df3e46de13', N'e4e30dc8-c708-445f-86c3-235e0028f129', N'NZFUNGI', N'NZFUNGI', getdate(), 'system'),
	(N'2dd748d7-0cf4-4a74-8e01-3464f688603b', N'd0b0b9fb-bda0-43a6-9bb2-07b6f3db2c26', N'NZOR_Test', N'NZOR_Test', getdate(), 'system'),
	(N'c93f3e15-92da-4e93-9de0-416f937cc8e5', N'91cbf580-e513-4ca4-89af-724253a393ac', N'NZIB', N'NZIB', getdate(), 'system'),
	(N'f710b2d6-b492-4104-845b-49990afb1abb', N'd0b0b9fb-bda0-43a6-9bb2-07b6f3db2c26', N'NZOR_Test_2', N'NZOR_Test_2', getdate(), 'system'),
	(N'8b1e7565-bdd6-493e-8cd0-87bbee15fdf3', N'14afd938-734e-40f5-9021-f252b7357bc8', N'Col2010-2', N'Col2010-2', getdate(), 'system'),
	(N'f6235951-ca30-4449-87f3-9159beebfb24', N'b8e1ef06-1f7d-43ce-bf30-71735e600a96', N'NZFLORA', N'NZFLORA', getdate(), 'system'),
	(N'ee25c5b4-ac33-4627-b54b-ee821f131dc7', N'14afd938-734e-40f5-9021-f252b7357bc8', N'Col2010', N'Col2010', getdate(), 'system')

