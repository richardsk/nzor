PRINT 'Starting ReferenceType Reference Data'

GO

SET NOCOUNT ON

DECLARE @ReferenceType TABLE
	(
	ReferenceTypeID UNIQUEIDENTIFIER NOT NULL, 

	Name NVARCHAR(50) NOT NULL,
	Description NVARCHAR(MAX) NOT NULL
	)

INSERT INTO
	@ReferenceType
VALUES 
	('810048ef-0449-4200-9a60-174e139d8398', N'Chapter', N'Chapter'),
	('fe30f9f1-5c42-4c8b-8256-189ad0d3825d', N'Unpublished', N'Unpublished work'),
	('3bb00cfa-5bfd-477a-b257-2a19e2a5b1e5', N'Book', N'Book'),
	('e1e65b51-5461-4d4d-9ba9-34a534ca5fe3', N'Serial', N'Serial (book/monograph) in series'),
	('20723e4e-44c0-438d-be89-6e3ae20566d0', N'Series', N'Series'),
	('f8725863-5387-403e-a612-82276d9d676a', N'In press', N'Work submitted for publication'),
	('678dd535-6778-4830-b7bf-9f3ee613f31a', N'Magazine', N'Magazine'),
	('70caefca-c71e-4f85-95d1-a08198a48b4b', N'Abstract', N'Abstract'),
	('9bf657d9-f6f2-4147-892a-ad1acbec5c24', N'Catalog', N'Catalog'),
	('a032d0aa-7b53-4d68-9801-ba7f0ce0d324', N'Article', N'Article'),
	('84e6427b-6cf4-4f93-a1c9-bcc182b6c6ad', N'Pamphlet', N'Pamphlet'),
	('dd0dd918-8209-4975-b02d-bcebd361da0d', N'Journal', N'Name of Journal'),
	('7b585a15-d654-40c2-8640-faf4fde7a384', N'Electronic Source', N'Electronic source'),
	('5470A77B-1252-4912-84AE-3F17F5F8F41C', N'Thesis', N'Thesis'),
	('875DBC0E-DE94-4838-A934-CF127E659A65', N'Report', N'Report'),
	('1EF6FCA4-019F-40B6-8F8F-194FF59B25F3', N'Generic', N'Generic'),
	('02A256AB-9139-4CEE-AE09-2309EAB8312B', N'Conference proceedings', N'Conference proceedings')

MERGE 
    dbo.ReferenceType AS Target
USING 
    @ReferenceType AS Source 
ON 
    (Target.ReferenceTypeID = Source.ReferenceTypeID)
WHEN MATCHED 
    THEN UPDATE
        SET   
            Target.Name = Source.Name,
            Target.Description = Source.Description

WHEN NOT MATCHED BY TARGET 
    THEN INSERT      
        (ReferenceTypeID, Name, Description)
    VALUES      
		(ReferenceTypeID, Name, Description)

WHEN NOT MATCHED BY SOURCE 
    THEN DELETE
; 

GO

PRINT 'Finished ReferenceType Reference Data'

GO