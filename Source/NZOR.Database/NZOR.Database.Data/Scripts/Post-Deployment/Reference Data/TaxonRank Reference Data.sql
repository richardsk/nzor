PRINT 'Starting TaxonRank Reference Data'

GO

SET NOCOUNT ON

DECLARE @TaxonRank TABLE
	(
	TaxonRankID UNIQUEIDENTIFIER NOT NULL,

	AncestorRankID UNIQUEIDENTIFIER NULL,
	MatchRuleSetID INT NULL,

	Name NVARCHAR(100) NOT NULL,
	DisplayName nvarchar(100) not null,
	KnownAbbreviations NVARCHAR(200) NULL,
	SortOrder INT NULL,
	IncludeInFullName BIT NULL,
	ShowRank bit null,
	GoverningCode nvarchar(10) null,

	AddedBy NVARCHAR(100) NOT NULL,
	AddedDate DATETIME NOT NULL,
	ModifiedBy NVARCHAR(100) NULL,
	ModifiedDate DATETIME NULL
	)

INSERT INTO
	@TaxonRank
VALUES 
    ('97641e2d-500c-4578-b787-0719ce7d6aae', NULL, 1, N'subsection', N'subsection', N'@subsection@subsect.@', 3600, NULL, 1, 'ICBN', N'migration', '2009-11-05', NULL, NULL),
    ('61764DD6-A289-41BE-A590-2289B3160395', NULL, 1, N'subsection', N'subsection', N'@subsection@subsect.@', 1880, NULL, 0, 'ICZN', N'migration', '2009-11-05', NULL, NULL),
    ('dd891344-2665-41cb-83c3-08d1935d6ab5', NULL, 2, N'superphylum', N'superphylum', N'@superphylum@superphyl.@', 700, NULL, 0, null, N'migration', '2009-11-05', NULL, NULL),
    ('afaa738b-7ee2-4f78-a4e5-0c0aba21963e', NULL, 2, N'subclass', N'subclass', N'@subclass@subcl.@', 1300, NULL, 0, null, N'migration', '2009-11-05', NULL, NULL),
    ('2b1966d4-720b-4f58-9c01-1280e1bb0dab', NULL, 2, N'order', N'order', N'@order@ord.@', 1600, NULL, 0, null, N'migration', '2009-11-05', NULL, NULL),
    ('2afa15e1-8bac-46f7-8008-173ad214609c', NULL, 1, N'subseries', N'subseries', N'@subseries@subser.@', 4000, 1, 1, null, N'migration', '2009-11-05', NULL, NULL),
    ('ed1b5291-bdfe-4aea-a368-27531905a32f', NULL, 2, N'subkingdom', N'subkingdom', N'@subkingdom@subreg@', 600, NULL, 0, null, N'migration', '2009-11-05', NULL, NULL),
    ('95d480f8-c36a-4dd2-91df-30c86131b5d3', NULL, 4, N'graft hybrid', N'graft hybrid', N'@graft hybrid@', 5800, 1, 0, null, N'migration', '2009-11-05', NULL, NULL),
    ('f351a7bc-1bfe-4404-b2b3-3edf96d51388', NULL, 4, N'biovar', N'biovar', N'@biovar@biovar.@', 5400, 1, 1, null, N'migration', '2009-11-05', NULL, NULL),
    ('c65ce8b4-ef37-4dad-9214-40f26dc28f3d', NULL, 1, N'subgenus', N'subgenus', N'@subgenus@subgen.@subg.@', 3200, 1, 0, null, N'migration', '2009-11-05', NULL, NULL),
    ('057d6434-a12a-460d-b705-4510603fae4f', NULL, 3, N'none', N'none', N'@none@unranked@', 200, NULL, 0, null, N'migration', '2009-11-05', NULL, NULL),
    ('853aa64f-fcbe-4a5f-b705-4b3daa0244f2', NULL, 4, N'f.sp.', N'f.sp.', N'@f.sp.@', 5400, 1, 1, null, N'migration', '2009-11-05', NULL, NULL),
    ('228535fe-6f1d-4a4d-bfd5-53a79db1a8cc', NULL, 2, N'superclass', N'superclass', N'@superclass@supercl.@', 1100, NULL, 0, null, N'migration', '2009-11-05', NULL, NULL),
    ('c21bb221-5291-4540-94d1-55a12d1bd0ad', N'20552eb6-1bf0-4073-a021-a6c7a89b7f14', 1, N'species', N'species', N'@species@sp.@spec.@', 4200, 1, 0, null, N'migration', '2009-11-05', NULL, NULL),
    ('4683cf13-2092-49b8-ba5a-5bb187463b81', NULL, 2, N'Division', N'division', N'@Division@div.@', 800, NULL, 0, null, N'migration', '2009-11-05', NULL, NULL),
    ('677b3abb-aba1-4dda-bbd5-5e33f7130184', NULL, 2, N'supertribe', N'supertribe', N'@supertribe@supertrib.@', 2300, NULL, 0, null, N'migration', '2009-11-05', NULL, NULL),
    ('4ccaef11-b97b-4a90-a01a-60548fa9eb36', NULL, 2, N'subfamily', N'subfamily', N'@subfamily@subfam.@', 2200, NULL, 0, null, N'migration', '2009-11-05', NULL, NULL),
    ('766F55CB-2819-40AD-A77F-F5F7CEEFF5CC', NULL, 2, N'nominalInfraFamily', N'nominalInfraFamily', N'@nominalInfraFamily@nom. infra family@', 2500, NULL, 0, null, N'migration', '2009-11-05', NULL, NULL),
    ('f3185c6e-58c1-4b6a-8f42-60ec005a571b', NULL, 4, N'hybrid formula', N'hybrid formula', N'@hybrid formula@', 5800, 1, 0, null, N'migration', '2009-11-05', NULL, NULL),
    ('e25f8ec4-0a5f-42bc-b0dc-6774646360b0', NULL, 2, N'anamorph', N'anamorph', N'@anamorph@', 2800, NULL, 0, null, N'migration', '2009-11-05', NULL, NULL),
    ('724024ad-7fbe-4db2-a0ac-725c236e5555', NULL, 4, N'subsp', N'subspecies', N'@subsp.@subsp@subspecies@ssp.@ssp@', 4400, 1, 1, null, N'migration', '2009-11-05', NULL, NULL),
    ('522bc641-d1e3-4fe5-96d7-757cc744ed21', NULL, 4, N'subvar.', N'subvariety', N'@subvar.@subvar@subvariety@', 4800, 1, 1, null, N'migration', '2009-11-05', NULL, NULL),
    ('abbcc4fd-017f-49dd-9f62-83a7764a605d', NULL, 2, N'subtribe', N'subtribe', N'@subtribe@subtrib.@', 2600, NULL, 0, null, N'migration', '2009-11-05', NULL, NULL),
    ('48d9de4b-9093-4962-b535-8dba8cc89628', NULL, 4, N'f.', N'forma', N'@f.@forma@', 5000, 1, 1, null, N'migration', '2009-11-05', NULL, NULL),
    ('140cf1a4-9a5e-4bb3-8d5f-93bfceb0c042', NULL, 4, N'subforma', N'subforma', N'@subforma@subf.@subfo.@', 5200, 1, 1, null, N'migration', '2009-11-05', NULL, NULL),
    ('d8680813-141d-4031-9fee-943862f4e4b2', NULL, 4, N'var.', N'variety', N'@var.@var@variety@v.@', 4600, 1, 1, null, N'migration', '2009-11-05', NULL, NULL),
    ('e8be9953-b6f7-419a-9bfa-9e6f29e0d525', NULL, 2, N'subphylum', N'subphylum', N'@subphylum@subphyl.@', 1000, NULL, 0, null, N'migration', '2009-11-05', NULL, NULL),
    ('708f4b9f-4994-428f-a141-a0769fa92a6d', NULL, 1, N'section', N'section', N'@section@sect.@', 3400, null, 1, 'ICBN', N'migration', '2009-11-05', NULL, NULL),
	('C4432BD3-E400-4072-8A2E-6E9D0DB97B52', NULL, 1, N'section', N'section', N'@section@sect.@', 1875, null, 0, 'ICZN', N'migration', '2009-11-05', NULL, NULL),
    ('de860ea0-d36b-4663-a9e1-a45a64c11e93', NULL, 2, N'phylum', N'phylum', N'@phylum@phyl.@', 800, NULL, 0, null, N'migration', '2009-11-05', NULL, NULL),
    ('9e2b99b6-5a21-4b8c-8ad4-a67e9fd73494', NULL, 2, N'suborder', N'suborder', N'@suborder@subord.@', 1800, NULL, 0, null, N'migration', '2009-11-05', NULL, NULL),
    ('20552eb6-1bf0-4073-a021-a6c7a89b7f14', NULL, 2, N'genus', N'genus', N'@genus@gen.@Genus@', 3000, 1, 0, null, N'migration', '2009-11-05', NULL, NULL),
    ('b665c051-da60-4fb4-8380-b365d405db8e', NULL, 2, N'superorder', N'superorder', N'@superorder@superord.@', 1500, NULL, 0, null, N'migration', '2009-11-05', NULL, NULL),
    ('62bc0365-292c-4ef8-a953-ba10b3042229', NULL, 4, N'intergen hybrid', N'intergen hybrid', N'@intergen hybrid@', 5800, NULL, 0, null, N'migration', '2009-11-05', NULL, NULL),
    ('4090af7a-6502-415d-8a41-bf23561374bd', NULL, 2, N'infraorder', N'infraorder', N'@infraorder@infraord.@', 1850, NULL, 0, null, N'migration', '2009-11-05', NULL, NULL),
    ('ca36388f-62fa-4bba-87f2-c01e6f8a5912', NULL, 2, N'infraclass', N'infraclass', N'@infraclass@infracl.@', 1400, NULL, 0, null, N'migration', '2009-11-05', NULL, NULL),
    ('90D11BA9-FBE7-4D0C-9B5C-4703F151B7A9', NULL, 2, N'subinfraclass', N'subinfraclass', N'@subinfraclass@', 1450, NULL, 0, null, N'migration', '2009-11-05', NULL, NULL),
    ('8a07e8d2-9610-4a30-8f04-c39770686f2d', NULL, 2, N'class', N'class', N'@class@cl.@', 1200, NULL, 0, null, N'migration', '2009-11-05', NULL, NULL),
    ('b3e30ea8-2706-4387-a853-c7b0c71769f6', NULL, 2, N'tribe', N'tribe', N'@tribe@trib.@', 2400, NULL, 0, null, N'migration', '2009-11-05', NULL, NULL),
    ('486c20ef-1296-4f08-b9b8-ceeeef4fceae', NULL, 4, N'cv', N'cultivar', N'@cv@cv.@cultivar@', 5800, 1, 0, null, N'migration', '2009-11-05', NULL, NULL),
    ('49ee68c3-521c-4926-8b96-d3dc7842e028', NULL, 1, N'series', N'series', N'@series@ser.@', 3800, 1, 1, null, N'migration', '2009-11-05', NULL, NULL),
    ('6d8f8c1f-d204-409f-b3b2-d431fe1d4845', NULL, 4, N'phagovar', N'phagovar', N'@phagovar@', 5400, 1, 1, null, N'migration', '2009-11-05', NULL, NULL),
    ('e8efcd3a-3b49-4017-b693-d466b9d165fa', NULL, 4, N'intragen hybrid', N'intragen hybrid', N'@intragen hybrid@', 5800, 1, 0, null, N'migration', '2009-11-05', NULL, NULL),
    ('63abe687-02b6-4787-bbe4-d5289c304f66', NULL, 2, N'kingdom', N'kingdom', N'@kingdom@reg.@', 400, NULL, 0, null, N'migration', '2009-11-05', NULL, NULL),
    ('a7820dcd-0266-4300-82f5-f10f8c5d6315', NULL, 2, N'family', N'family', N'@family@fam.@', 2000, NULL, 0, null, N'migration', '2009-11-05', NULL, NULL),
    ('64097e92-224e-49af-b1f0-f39b1c797c1a', NULL, 4, N'serovar', N'serovar', N'@serovar@', 5400, 1, 1, null, N'migration', '2009-11-05', NULL, NULL),
    ('af32fd2c-6939-4443-80c3-f5dc31a993f7', NULL, 2, N'superfamily', N'superfamily', N'@superfamily@superfam.@', 1900, NULL, 0, null, N'migration', '2009-11-05', NULL, NULL),
    ('9df84d53-d138-4ed2-a551-fd4cfe2b462c', NULL, 4, N'pv.', N'pathovar', N'@pv.@pv@pathovar@', 5400, 1, 1, null, N'migration', '2009-11-05', NULL, NULL),
    ('8cafab16-ff2a-4c9d-a923-23202760cbbe', NULL, 4, N'δ', N'δ', N'@δ@', 5600, 1, 1, null, N'migration', '2009-11-05', NULL, NULL),
    ('0004b5c3-d08c-4a64-9bf1-15d317f7088d', NULL, 4, N'γ', N'γ', N'@γ@', 5600, 1, 1, null, N'migration', '2009-11-05', NULL, NULL),
    ('7d83f513-2cea-4e65-a58b-7cc4cc6c840d', NULL, 4, N'α', N'α', N'@α@[alpha]@', 5600, 1, 1, null, N'migration', '2009-11-05', NULL, NULL),
    ('d817f056-f29a-484a-a8d6-ce7dbbcc553b', NULL, 4, N'ß', N'ß', N'@ß@[beta]@[beta].@beta@β@', 5600, 1, 1, null, N'migration', '2009-11-05', NULL, NULL)

MERGE 
    dbo.TaxonRank AS Target
USING 
    @TaxonRank AS Source 
ON 
    (Target.TaxonRankID = Source.TaxonRankID)
WHEN MATCHED 
    THEN UPDATE
        SET   
            Target.AncestorRankID = Source.AncestorRankID,
            Target.MatchRuleSetID = Source.MatchRuleSetID,
    
            Target.Name = Source.Name,
			Target.DisplayName = Source.DisplayName,
            Target.KnownAbbreviations = Source.KnownAbbreviations,
            Target.SortOrder = Source.SortOrder,
            Target.IncludeInFullName = Source.IncludeInFullName,
			Target.ShowRank = Source.ShowRank,
			Target.GoverningCode = Source.GoverningCode,

            Target.AddedBy = Source.AddedBy,
            Target.AddedDate = Source.AddedDate,
            Target.ModifiedBy = Source.ModifiedBy,
            Target.ModifiedDate = Source.ModifiedDate

WHEN NOT MATCHED BY TARGET 
    THEN INSERT      
        (TaxonRankID, AncestorRankID, MatchRuleSetID, Name, DisplayName, KnownAbbreviations, SortOrder, IncludeInFullName, ShowRank, GoverningCode, AddedBy, AddedDate, ModifiedBy, ModifiedDate)
    VALUES      
        (TaxonRankID, AncestorRankID, MatchRuleSetID, Name, DisplayName, KnownAbbreviations, SortOrder, IncludeInFullName, ShowRank, GoverningCode, AddedBy, AddedDate, ModifiedBy, ModifiedDate)

WHEN NOT MATCHED BY SOURCE 
    THEN DELETE
; 

GO

PRINT 'Finished TaxonRank Reference Data'

GO