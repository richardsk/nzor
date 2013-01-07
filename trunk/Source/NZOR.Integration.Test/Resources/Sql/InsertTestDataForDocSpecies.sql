delete consensus.StackedName where SeedNameID = '696120DF-D7B1-4345-8E7A-BAC684F93F1F' or SeedNameID = '6187E1E3-77EF-4FE1-BBAD-3F86A8595EA0'
delete consensus.ConceptRelationship where FromConceptID = '7DD8671C-5AB9-4E96-B1DE-843DF8B01590'
delete consensus.Concept where NameID = '696120DF-D7B1-4345-8E7A-BAC684F93F1F' or NameID = '6187E1E3-77EF-4FE1-BBAD-3F86A8595EA0'
delete consensus.NameProperty where NameID = '696120DF-D7B1-4345-8E7A-BAC684F93F1F' or NameID = '6187E1E3-77EF-4FE1-BBAD-3F86A8595EA0'
delete consensus.Name where NameID = '696120DF-D7B1-4345-8E7A-BAC684F93F1F' or NameID = '6187E1E3-77EF-4FE1-BBAD-3F86A8595EA0'


--genus
insert consensus.Name 
select '696120DF-D7B1-4345-8E7A-BAC684F93F1F', '20552EB6-1BF0-4073-A021-A6C7A89B7F14', 'A5233111-61A0-4AE6-9C2B-5E8E71F1473A', 'Cestrum', 'ICBN', GETDATE(), null

insert consensus.NameProperty 
select NEWID(), '696120DF-D7B1-4345-8E7A-BAC684F93F1F', 'A1D57520-3D64-4F7D-97C8-69B449AFA280', null, null, 'genus', GETDATE(), null

insert consensus.NameProperty 
select NEWID(), '696120DF-D7B1-4345-8E7A-BAC684F93F1F', '1F64E93C-7EE8-40D7-8681-52B56060D750', null, null, 'Cestrum', GETDATE(), null

insert consensus.Concept 
select 'AAE3FE40-3F23-442F-AFA4-749E0148C911', '696120DF-D7B1-4345-8E7A-BAC684F93F1F', null, null, null, null, GETDATE(), null


--species
insert consensus.Name 
select '6187E1E3-77EF-4FE1-BBAD-3F86A8595EA0', 'C21BB221-5291-4540-94D1-55A12D1BD0AD', 'A5233111-61A0-4AE6-9C2B-5E8E71F1473A', 'Cestrum aurantiacum Lindl.', 'ICBN', GETDATE(), null

insert consensus.NameProperty 
select NEWID(), '6187E1E3-77EF-4FE1-BBAD-3F86A8595EA0', 'A1D57520-3D64-4F7D-97C8-69B449AFA280', null, null, 'species', GETDATE(), null

insert consensus.NameProperty 
select NEWID(), '6187E1E3-77EF-4FE1-BBAD-3F86A8595EA0', '006D86A8-08A5-4C1A-BC08-C07B0225E01B', null, null, 'Lindl.', GETDATE(), null

insert consensus.NameProperty 
select NEWID(), '6187E1E3-77EF-4FE1-BBAD-3F86A8595EA0', '1F64E93C-7EE8-40D7-8681-52B56060D750', null, null, 'aurantiacum', GETDATE(), null

insert consensus.Concept 
select '7DD8671C-5AB9-4E96-B1DE-843DF8B01590', '6187E1E3-77EF-4FE1-BBAD-3F86A8595EA0', null, null, null, null, GETDATE(), null

insert consensus.ConceptRelationship 
select NEWID(), '7DD8671C-5AB9-4E96-B1DE-843DF8B01590', 'AAE3FE40-3F23-442F-AFA4-749E0148C911', '6A11B466-1907-446F-9229-D604579AA155', null, GETDATE(), null

go

	INSERT consensus.StackedName(SeedNameID, AccordingToID, NameID, TaxonRankID, CanonicalName, RankName, SortOrder, Depth)
	EXEC consensus.sprSelect_StackedNameToRoot '696120DF-D7B1-4345-8E7A-BAC684F93F1F'
	
	INSERT consensus.StackedName(SeedNameID, AccordingToID, NameID, TaxonRankID, CanonicalName, RankName, SortOrder, Depth)
	EXEC consensus.sprSelect_StackedNameToRoot '6187E1E3-77EF-4FE1-BBAD-3F86A8595EA0'
