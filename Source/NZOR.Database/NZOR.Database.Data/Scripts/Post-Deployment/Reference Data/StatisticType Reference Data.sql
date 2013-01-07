SET NOCOUNT ON

DECLARE @statType TABLE
	(
	StatisticTypeID UNIQUEIDENTIFIER NOT NULL, 
	Name NVARCHAR(max) NOT NULL,
	DisplayName NVARCHAR(max) not null
	)

INSERT INTO
	@statType
values('A5BF46BB-9C4E-437C-95B2-40EAD0E1D223', 'NZORNameCount', 'Number of Taxon Names in NZOR'),
	('C48F7122-DB46-4BFE-A870-85D696D736D3', 'NZORConceptCount', 'Number of Taxon Concepts in NZOR'),
	('858DB276-0627-4AA2-A8B9-C954E17263FC', 'NZORReferenceCount', 'Number of References in NZOR'),
	('94AF33D1-88A3-4876-8FBC-17A9540CD508', 'NZORAcceptedNameCount', 'Number of accepted Taxon Names in NZOR'),
	('71E96C28-1A05-4356-BF84-133E6D707E91', 'NZORSynonymCount', 'Number of synonyms in NZOR'),
	('BC8E69F2-5B68-461E-8815-555DEB63C33E', 'VernacularNameCount', 'Number of Vernacular Names in NZOR'),
	('3852F48D-2A08-4140-BB20-CCE8B65B0740', 'PresentInNZCount', 'Number of taxa present in New Zealand'),
	('4803A985-0A56-41DF-8474-7150880A7290', 'PlantaeSpeciesCount', 'Number of plant species in NZOR'),
	('AB405C0A-E94F-48DE-BA8F-A0C9061029D8', 'PlantaeSpeciesPresentCount', 'Number of plant species present in New Zealand'),
	('67B64BE4-D21D-4826-97FE-30EC1299E375', 'PlantaeSpeciesIndigenousCount', 'Number of plant species indigenous in New Zealand'),
	('2E2FB456-37A0-4D48-963C-CDACEA4413F3', 'PlantaeSpeciesEndemicCount', 'Number of plant species endemic to New Zealand'),
	('87D6A943-4DF9-4489-9697-6EBE37CD0BB4', 'PlantaeSpeciesIntroducedCount', 'Number of plant species introduced to New Zealand'),
	('4BB099C2-00CA-4C01-94DD-61065D1D08AF', 'PlantaeSpeciesNoBiostatusCount', 'Number of plant species endemic to New Zealand'),
	('DEBE569B-3DA4-4176-9643-BF656E74EDC0', 'FungiSpeciesCount', 'Number of fungi species in NZOR'),
	('642EDD76-C5E1-4BF6-B7D2-8074D293A307', 'FungiSpeciesPresentCount', 'Number of fungi species present in New Zealand'),
	('0713E57A-2A26-4F60-9F13-B329156678CD', 'FungiSpeciesIndigenousCount', 'Number of fungi species indigenous in New Zealand'),
	('25B1D281-6AA1-4F91-B464-808D6699B976', 'FungiSpeciesEndemicCount', 'Number of fungi species endemic to New Zealand'),
	('0E35C269-00CA-489F-92C6-9D4BAD828077', 'FungiSpeciesIntroducedCount', 'Number of fungi species introduced to New Zealand'),
	('07052D36-E9DC-4BEA-B12B-F6F4A4EC5595', 'FungiSpeciesNoBiostatusCount', 'Number of fungi species endemic to New Zealand'),
	('D7FBD5AD-405E-4018-B546-BFB38D3073E8', 'AnimaliaSpeciesCount', 'Number of animal species in NZOR'),
	('6E750645-46E4-432D-8652-82F59A5A2EEB', 'AnimaliaSpeciesPresentCount', 'Number of animal species present in New Zealand'),
	('3259D5E9-5C1D-48AD-9FB1-2A43CF4A6D99', 'AnimaliaSpeciesIndigenousCount', 'Number of animal species indigenous in New Zealand'),
	('91472A13-4C5B-4C50-8C04-469B476F52E9', 'AnimaliaSpeciesEndemicCount', 'Number of animal species endemic to New Zealand'),
	('B9D847B5-32C0-4FC3-943B-7638BAA0C9D2', 'AnimaliaSpeciesIntroducedCount', 'Number of animal species introduced to New Zealand'),
	('21C286A3-32DA-47FC-8BE9-BD6C2A4218D2', 'AnimaliaSpeciesNoBiostatusCount', 'Number of animal species endemic to New Zealand'),
	('D393D3E9-8BF0-431B-A6C8-C7C57C69DA7B', 'BacteriaSpeciesCount', 'Number of bacteria species in NZOR'),
	('DD975967-88BB-4E01-B0E8-0D569BA60ECA', 'BacteriaSpeciesPresentCount', 'Number of bacteria species present in New Zealand'),
	('C82A9F4A-C4FB-479A-86C7-C2C47999477B', 'BacteriaSpeciesIndigenousCount', 'Number of bacteria species indigenous in New Zealand'),
	('A4CB8DEA-B9BE-49E4-A58F-9EEA240B9B48', 'BacteriaSpeciesEndemicCount', 'Number of bacteria species endemic to New Zealand'),
	('C0250EF0-A857-49F9-8A38-3398F12E19C3', 'BacteriaSpeciesIntroducedCount', 'Number of bacteria species introduced to New Zealand'),
	('9B43FCE7-6D29-4423-9398-C20F51B37B8E', 'BacteriaSpeciesNoBiostatusCount', 'Number of bacteria species endemic to New Zealand'),
	('3352AE66-B1FB-4F42-A7BD-9172A3EB00CE', 'ViroidSpeciesCount', 'Number of viroid species in NZOR'),
	('AEE30687-F4BA-41B0-A9F2-E243B4EEAADE', 'ViroidSpeciesPresentCount', 'Number of viroid species present in New Zealand'),
	('65CB1512-9D36-4F55-BA7E-CD5AB94B9E74', 'ViroidSpeciesIndigenousCount', 'Number of viroid species indigenous in New Zealand'),
	('92C041CC-F7A9-4B3D-800C-8906FFCFCC66', 'ViroidSpeciesEndemicCount', 'Number of viroid species endemic to New Zealand'),
	('6DA5E6AC-9E24-4D62-8A5B-97D6E4E2F23E', 'ViroidSpeciesIntroducedCount', 'Number of viroid species introduced to New Zealand'),
	('79FF342E-B39B-457E-9F63-6A1FC069B9ED', 'ViroidSpeciesNoBiostatusCount', 'Number of viroid species endemic to New Zealand'),
	('CE5F20A9-A89A-49C9-BCB2-1367D5C6D195', 'ChromistaSpeciesCount', 'Number of chromista species in NZOR'),
	('AFBE6444-1ADD-4DAD-A088-CCA4D2F7BEA9', 'ChromistaSpeciesPresentCount', 'Number of chromista species present in New Zealand'),
	('A2AC3ED8-FF24-410F-A201-DE35E093FA8D', 'ChromistaSpeciesIndigenousCount', 'Number of chromista species indigenous in New Zealand'),
	('E1D23838-3FFC-4F60-BB15-44E8DF4BAF71', 'ChromistaSpeciesEndemicCount', 'Number of chromista species endemic to New Zealand'),
	('B2107A99-7FCE-4CCD-8743-CC27554072FE', 'ChromistaSpeciesIntroducedCount', 'Number of chromista species introduced to New Zealand'),
	('F507DBAC-433F-4BB0-859F-BF5315E5E9FA', 'ChromistaSpeciesNoBiostatusCount', 'Number of chromista species endemic to New Zealand'),
	('4F6463C2-77C0-43CB-B334-F353DB80F5A2', 'VirusSpeciesCount', 'Number of virus species in NZOR'),
	('8519F871-D337-4C16-9268-D2B11571BA55', 'VirusSpeciesPresentCount', 'Number of virus species present in New Zealand'),
	('280D0E3B-8BDD-4228-B2B9-7E935CD35AD3', 'VirusSpeciesIndigenousCount', 'Number of virus species indigenous in New Zealand'),
	('6FCCF165-3988-426B-9098-6DF3E4957638', 'VirusSpeciesEndemicCount', 'Number of virus species endemic to New Zealand'),
	('76FA19F0-E745-4E58-9D33-3121F2E110C5', 'VirusSpeciesIntroducedCount', 'Number of virus species introduced to New Zealand'),
	('7E51F5A9-4ACE-4B45-8DD4-E3C4E423CBED', 'VirusSpeciesNoBiostatusCount', 'Number of virus species endemic to New Zealand'),
	('6DE6ADE6-25FA-45E8-8BB7-752F81616A81', 'MollicuteSpeciesCount', 'Number of mollicute species in NZOR'),
	('B4F5CC43-551F-4454-BC12-4BE38BC66214', 'MollicuteSpeciesPresentCount', 'Number of mollicute species present in New Zealand'),
	('05335B5B-2048-4D4D-B2FE-22AD73F2454E', 'MollicuteSpeciesIndigenousCount', 'Number of mollicute species indigenous in New Zealand'),
	('45EE73C4-BF88-44F7-9BA8-E1F3E3BE63D9', 'MollicuteSpeciesEndemicCount', 'Number of mollicute species endemic to New Zealand'),
	('ED596B9F-6043-4CD3-B869-3B42157867A6', 'MollicuteSpeciesIntroducedCount', 'Number of mollicute species introduced to New Zealand'),
	('51C4DBF2-9E1F-4573-AA57-203C4624B5BD', 'MollicuteSpeciesNoBiostatusCount', 'Number of mollicute species endemic to New Zealand'),
	('51D29DCA-E19B-4E50-97B0-3D42DF418C1C', 'ProtozoaSpeciesCount', 'Number of protozoa species in NZOR'),
	('F6F67674-B030-4F01-BB16-68C65589B754', 'ProtozoaSpeciesPresentCount', 'Number of protozoa species present in New Zealand'),
	('21B63DA3-7AA6-42B6-9BA9-E87A04CD4A2F', 'ProtozoaSpeciesIndigenousCount', 'Number of protozoa species indigenous in New Zealand'),
	('D68DBCD4-0BB7-487E-9313-2A7F8F424259', 'ProtozoaSpeciesEndemicCount', 'Number of protozoa species endemic to New Zealand'),
	('E05641F5-2BCD-4AE3-87B7-27739205571B', 'ProtozoaSpeciesIntroducedCount', 'Number of protozoa species introduced to New Zealand'),
	('20DBC238-C2C6-4886-8100-DDEC44678694', 'ProtozoaSpeciesNoBiostatusCount', 'Number of protozoa species endemic to New Zealand'),
	('E4DD33CF-8FE4-4994-AAB7-296E25AD602F', 'ProviderNameCountNZFUNGI', 'Number of taxon names provided by provider NZ Fungi'),
	('5346A7FC-E828-49CF-AFB5-EC568B743DEF', 'ProviderNameCountNZFLORA', 'Number of taxon names provided by provider NZ Flora'),
	('67B8CB2C-A5AC-4D0F-A1BF-B46A6188286B', 'ProviderNameCountNZAC', 'Number of taxon names provided by provider NZ Arthropod Collection'),
	('73CF5394-A686-4FEC-996A-10CEE4D4CF50', 'ProviderNameCountNZIB', 'Number of taxon names provided by provider NZ Inventory of Biodiversity'),
	('568785CB-F5E8-4E18-80FB-8F55738EEB9E', 'ProviderNameCountGlobal_Cache', 'Number of taxon names provided by the global cache system'),
	('37E4C602-F589-430C-AC3E-96202E9E9E0E', 'ProviderNameCountNZOR_Hosted', 'Number of taxon names provided by provider NZOR hosted datasets'),
	('3FDAC214-A7B6-41D0-954F-722409F3CF5D', 'BiomeNameCountMarine', 'Number of taxa explicitly labelled mariine'),
	('A9891763-7B4E-4259-8107-75528CAD52D8', 'BiomeNameCountBrackish', 'Number of taxa explicitly labelled brackish'),
	('87FECD14-FC70-4866-874D-FAE425A74D16', 'BiomeNameCountFreshwater', 'Number of taxa explicitly labelled freshwater'),
	('C932E10F-B1E5-47DA-8483-E38BD6429D60', 'BiomeNameCountTerrestrial', 'Number of taxa explicitly labelled terrestrial')

MERGE 
    [admin].StatisticType AS Target
USING 
    @statType AS Source 
ON 
    (Target.StatisticTypeID = Source.StatisticTypeID)
WHEN MATCHED 
    THEN UPDATE
        SET   
            Target.Name = Source.Name,
			Target.DisplayName = Source.DisplayName

WHEN NOT MATCHED BY TARGET 
    THEN INSERT      
        (StatisticTypeID, Name, DisplayName)
    VALUES      
		(StatisticTypeID, Name, DisplayName)

WHEN NOT MATCHED BY SOURCE 
    THEN DELETE
; 


go
