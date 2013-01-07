CREATE TABLE [dbo].[Transformation]
(
	TransformationID uniqueidentifier not null,
	Name nvarchar(250) not null,
	Description nvarchar(100),
	Category nvarchar(100),
	XSLT xml not null
)
