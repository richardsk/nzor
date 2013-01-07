PRINT 'Starting [admin].ExternalLookupService Data'

SET NOCOUNT ON

DECLARE @ExternalLookupService TABLE
	(
	ExternalLookupServiceId uniqueidentifier not null,
	Name nvarchar(250) not null,
	Description nvarchar(500) null,
	DataFormat nvarchar(150) not null,
	NameLookupEndpoint nvarchar(500) null,
	ConceptLookupEndpoint nvarchar(500) null,
	ReferenceLookupEndpoint nvarchar(500) null,
	IDLookupEndpoint nvarchar(500) null,
	SpaceCharacterSubstitute nvarchar(10) null,
	IconFilename nvarchar(100) null,
	LookupServiceClassName nvarchar(500) null
	)

INSERT INTO
	@ExternalLookupService
values ('FF6A719D-CA69-4AB4-95AD-3F8E6BC7E81A','Catalogue Of Life', 'Catalogue Of Life', 'CoL Web Service', 'http://www.catalogueoflife.org/col/webservice?response=full&name=',null, null,'http://www.catalogueoflife.org/col/webservice?response=full&id=', '+', 'CoL_logo.png', 'NZOR.ExternalLookups.CoLExternalLookupService'),
	   ('43A9CB5E-6E5C-4B3B-9ACE-18796E5E2CAF','Encyclopedia Of Life', 'Encyclopedia Of Life', 'EOL API', 'http://www.eol.org/api/search/1.0/',null, null,'http://www.eol.org/api/search/1.0/', '+', 'EoL_logo.png', null),
	   ('053D51B4-63AD-46E4-A109-A43BC2AA1F62','Encyclopedia Of Life Web', 'Encyclopedia Of Life Web Page', 'HTML', 'http://www.eol.org/search/',null, null, null, '+', 'EoL_logo.png', null),
	   ('D554D039-5634-471A-9547-628EE46ADC1E','Catalogue Of Life Web', 'Catalogue Of Life Web Page', 'HTML', 'http://www.catalogueoflife.org/col/search/all/key/',null, null, null, '+', 'CoL_logo.png', null)
	
MERGE 
    [admin].ExternalLookupService AS Target
USING 
    @ExternalLookupService AS Source 
ON 
    (Target.ExternalLookupServiceId = Source.ExternalLookupServiceId)
WHEN MATCHED 
    THEN UPDATE
        SET   
			target.name = source.name,
			target.description = source.description,
			target.dataformat = source.dataformat,
			target.namelookupendpoint = source.namelookupendpoint,
			target.conceptlookupendpoint = source.conceptlookupendpoint,
			target.referencelookupendpoint = source.referencelookupendpoint,
			target.idlookupendpoint = source.idlookupendpoint,
			target.spacecharactersubstitute = source.spacecharactersubstitute,
			target.iconfilename = source.iconfilename,
			target.LookupServiceClassName = source.LookupServiceClassName

WHEN NOT MATCHED BY TARGET 
    THEN INSERT      
        (ExternalLookupServiceId, Name, description, dataformat, namelookupendpoint, conceptlookupendpoint, referencelookupendpoint, idlookupendpoint, spacecharactersubstitute, iconfilename, LookupServiceClassName)
    VALUES      
        (ExternalLookupServiceId, Name, description, dataformat, namelookupendpoint, conceptlookupendpoint, referencelookupendpoint, idlookupendpoint, spacecharactersubstitute, iconfilename, LookupServiceClassName)

WHEN NOT MATCHED BY SOURCE 
    THEN DELETE
; 

GO

PRINT 'Finished [admin].ExternalLookupService Data'

go

