create database NZOR_Data_Test
go

USE [NZOR_Data_Test]


GO
/****** Object:  Schema [cons]    Script Date: 09/08/2010 15:20:15 ******/
CREATE SCHEMA [cons] AUTHORIZATION [dbo]
GO
/****** Object:  Schema [prov]    Script Date: 09/08/2010 15:20:15 ******/
CREATE SCHEMA [prov] AUTHORIZATION [dbo]
GO
/****** Object:  StoredProcedure [dbo].[sprCreateDataScript]    Script Date: 09/08/2010 15:20:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Aaron
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[sprCreateDataScript] 
	@Table nvarchar(50), 
	@Schema nvarchar(50) = 'dbo',
	@booIdInsert bit,
	@booFor2008 bit = 1
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

DECLARE @Result nvarchar(max)
	declare @Col nvarchar(100)
	declare @ColType nvarchar(50)
	declare @Append1 nvarchar(max)
	declare @append2 nvarchar(max)
	declare @Select nvarchar(max)

	SET CONCAT_NULL_YIELDS_NULL  OFF
	set @append1 = 'INSERT INTO ' + @schema + '.' + @Table + ' ('
	create table #TempDataScript(rowid [int] identity, [value] [nvarchar] (max))
	set @Select = 'insert #TempDataScript select'

	DECLARE curTableCols CURSOR FOR 
		select sc.name, st.name from sysobjects as so inner join syscolumns as sc
			on so.id = sc.id inner join systypes as st on sc.xtype = st.xtype
			where so.xtype = 'U'and not st.name = 'sysname' and so.name = @Table
			order by sc.colorder

	open curTableCols
	fetch next from curTableCols
		into @Col, @ColType
	while @@Fetch_Status = 0
	Begin
		set @Append1 = @Append1 + @col + ', '
		if @ColType = 'xml' or @ColType = 'sql_variant'
			Begin
				set @Select = @select + ' char(39)  + ' 
					+ 'cast(' + @col + ' as nvarchar(max))' + ' + char(39) ' + ' + ' + '''' + ',' + '''' + ' + '
			end
		else
			Begin
				set @Select = @select + ' char(39)  + ' 
					+ 'cast(replace(' + @col + ', char(39), char(39) + char(39)) as nvarchar(max))' + ' + char(39) ' + ' + ' + '''' + ',' + '''' + ' + '
			end
		fetch next from curTableCols
		into @Col, @ColType
	end
	set @Append1 = left(@Append1, len(@Append1) - 1) + ') '
	set @Select = left(@Select, len(@Select) - 7) + ' from ' + @Schema + '.' + @table
	
	close curTableCols
	deallocate curTableCols

	exec (@Select)

	declare @Count int
	declare @Pos int

	select @count = count(*) from #TempDataScript
	
	--print 'Delete from ' + @schema + '.' + @table

	--print 'go'

	if @booIdInsert = 1
	begin
		print 'set identity_insert ' + @schema + '.' + @table + ' on'
	end

	set @pos = 1
	if @booFor2008 = 1
	begin
		print @append1
		print 'values'	
	end
	while @pos <= @count
	begin
		select @append2 = value from #TempDataScript where rowid = @pos
		set @append2 = replace(@append2, char(39) + char(39), 'null')
		if @booFor2008 = 1
		begin
			print '(' + @append2 + '),'
		end
		else
		begin
			print @append1
			print 'values (' + @append2 + ')'
			print 'go'
		end
		set @pos = @pos + 1
	end
		

	if @booIdInsert = 1
	begin
		print 'set identity_insert ' + @schema + '.' + @table + ' off'
	end
	if @booFor2008 = 1
	begin
		print 'go'
	end
	drop table #TempDataScript
	
END
GO
/****** Object:  Table [dbo].[ReferenceType]    Script Date: 09/08/2010 15:20:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ReferenceType](
	[ReferenceTypeID] [uniqueidentifier] NOT NULL,
	[Type] [nvarchar](50) NULL,
	[Description] [nvarchar](max) NULL,
 CONSTRAINT [PK_ReferenceType] PRIMARY KEY CLUSTERED 
(
	[ReferenceTypeID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ReferenceFieldType]    Script Date: 09/08/2010 15:20:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ReferenceFieldType](
	[ReferenceFieldTypeID] [uniqueidentifier] NOT NULL,
	[Type] [nvarchar](50) NULL,
	[Description] [nvarchar](max) NULL,
 CONSTRAINT [PK_ReferenceFieldType] PRIMARY KEY CLUSTERED 
(
	[ReferenceFieldTypeID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Provider]    Script Date: 09/08/2010 15:20:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Provider](
	[ProviderID] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](500) NOT NULL,
 CONSTRAINT [PK_Provider] PRIMARY KEY CLUSTERED 
(
	[ProviderID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[NamePropertyLookup]    Script Date: 09/08/2010 15:20:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[NamePropertyLookup](
	[NamePropertyLookupID] [uniqueidentifier] NOT NULL,
	[NameClassPropertyID] [uniqueidentifier] NOT NULL,
	[ParentLoookupID] [uniqueidentifier] NULL,
	[Value] [nvarchar](1000) NULL,
	[AlternativeStrings] [xml] NULL,
	[SortOrder] [int] NULL,
 CONSTRAINT [PK_NamePropertyLookup] PRIMARY KEY CLUSTERED 
(
	[NamePropertyLookupID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Match]    Script Date: 09/08/2010 15:20:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Match](
	[MatchPk] [int] NOT NULL,
	[MatchRuleSet] [nvarchar](50) NULL,
	[MatchFirst] [bit] NULL,
	[MatchFunction] [nvarchar](1000) NULL,
	[MatchThreshold] [int] NULL,
	[MatchPassFk] [int] NULL,
	[MatchFailFk] [int] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[GeoRegionSchema]    Script Date: 09/08/2010 15:20:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[GeoRegionSchema](
	[GeoRegionSchemaID] [uniqueidentifier] NOT NULL,
	[SchemaName] [nvarchar](500) NOT NULL,
	[Description] [nvarchar](max) NULL,
 CONSTRAINT [PK_GeoRegionSchema] PRIMARY KEY CLUSTERED 
(
	[GeoRegionSchemaID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  UserDefinedFunction [dbo].[fnLevenshteinDistance]    Script Date: 09/08/2010 15:20:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create function [dbo].[fnLevenshteinDistance]( @s varchar(300), @t varchar(300) ) 
	--Returns the Levenshtein Distance between strings s1 and s2.
	--Original developer: Michael Gilleland   http://www.merriampark.com/ld.htm
	--Translated to TSQL by Joseph Gama
returns varchar(50)
as
BEGIN
	DECLARE @d varchar(6000), @LD int, @m int, @n int, @i
	int, @j int,
	@s_i char(1), @t_j char(1),@cost int
	--Step 1
	SET @n=LEN(@s)
	SET @m=LEN(@t)
	SET @d=replicate(CHAR(0),6000)
	If @n = 0
		BEGIN
		SET @LD = @m
		GOTO done
		END
	If @m = 0
		BEGIN
		SET @LD = @n
		GOTO done
		END
	--Step 2
	SET @i=0
	WHILE @i<=@n
		BEGIN
		SET @d=STUFF(@d,@i+1,1,CHAR(@i))--d(i, 0) = i
		SET @i=@i+1
		END

	SET @i=0
	WHILE @i<=@m
		BEGIN
		SET @d=STUFF(@d,@i*(@n+1)+1,1,CHAR(@i))--d(0, j) = j
		SET @i=@i+1
		END
	--goto done
	--Step 3
		SET @i=1
		WHILE @i<=@n
			BEGIN
			SET @s_i=(substring(@s,@i,1))
	--Step 4
		SET @j=1
		WHILE @j<=@m
			BEGIN
			SET @t_j=(substring(@t,@j,1))
			--Step 5
			If @s_i = @t_j
				SET @cost=0
			ELSE
				SET @cost=1
	--Step 6
			SET @d=STUFF(@d,@j*(@n+1)+@i+1,1,CHAR(dbo.MIN3(
			ASCII(substring(@d,@j*(@n+1)+@i-1+1,1))+1,
			ASCII(substring(@d,(@j-1)*(@n+1)+@i+1,1))+1,
			ASCII(substring(@d,(@j-1)*(@n+1)+@i-1+1,1))+@cost)
			))
			SET @j=@j+1
			END
		SET @i=@i+1
		END      
	--Step 7
	SET @LD = ASCII(substring(@d,@n*(@m+1)+@m+1,1))
	done:
	--RETURN @LD
	--I kept this code that can be used to display the matrix with all calculated values
	--From Query Analyser it provides a nice way to check the algorithm in action
	--
	RETURN @LD
	--declare @z varchar(255)
	--set @z=''
	--SET @i=0
	--WHILE @i<=@n
	--	BEGIN
	--	SET @j=0
	--	WHILE @j<=@m
	--		BEGIN
	--		set
	--@z=@z+CONVERT(char(3),ASCII(substring(@d,@i*(@m+1)+@j+1 ,1)))
	--		SET @j=@j+1 
	--		END
	--	SET @i=@i+1
	--	END
	--print dbo.wrap(@z,3*(@n+1))
END
GO
/****** Object:  Table [cons].[FlatName]    Script Date: 09/08/2010 15:20:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [cons].[FlatName](
	[FlatNameID] [uniqueidentifier] NOT NULL,
	[ParentNameID] [uniqueidentifier] NOT NULL,
	[NameID] [uniqueidentifier] NOT NULL,
	[Canonical] [nvarchar](250) NOT NULL,
	[TaxonRankID] [uniqueidentifier] NULL,
	[RankName] [nvarchar](250) NULL,
	[Depth] [int] NULL,
	[SeedNameID] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_FlatName] PRIMARY KEY CLUSTERED 
(
	[FlatNameID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ConceptRelationshipType]    Script Date: 09/08/2010 15:20:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ConceptRelationshipType](
	[ConceptRelationshipTypeID] [uniqueidentifier] NOT NULL,
	[Relationship] [nvarchar](50) NOT NULL,
	[MinOccurences] [int] NULL,
	[MaxOccurrences] [int] NULL,
 CONSTRAINT [PK_ConceptRelationshipType] PRIMARY KEY CLUSTERED 
(
	[ConceptRelationshipTypeID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[NameClass]    Script Date: 09/08/2010 15:20:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[NameClass](
	[NameClassID] [uniqueidentifier] NOT NULL,
	[Title] [nvarchar](50) NOT NULL,
	[Description] [nvarchar](max) NULL,
 CONSTRAINT [PK_NameClass] PRIMARY KEY CLUSTERED 
(
	[NameClassID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TaxonRank]    Script Date: 09/08/2010 15:20:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TaxonRank](
	[TaxonRankID] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[KnownAbbreviations] [nvarchar](200) NULL,
	[SortOrder] [int] NULL,
	[IncludeInFullName] [bit] NULL,
	[AncestorRankID] [uniqueidentifier] NULL,
	[MatchRuleSetID] [int] NULL,
	[AddedBy] [nvarchar](100) NOT NULL,
	[AddedDate] [datetime] NOT NULL,
	[ModifiedBy] [nvarchar](100) NULL,
	[ModifiedDate] [datetime] NULL,
 CONSTRAINT [PK_TaxonRank] PRIMARY KEY CLUSTERED 
(
	[TaxonRankID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TaxonPropertyLookup]    Script Date: 09/08/2010 15:20:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TaxonPropertyLookup](
	[TaxonPropertyLookupID] [uniqueidentifier] NOT NULL,
	[TaxonPropertyTypeId] [uniqueidentifier] NOT NULL,
	[ParentLookupId] [uniqueidentifier] NULL,
	[Value] [nvarchar](500) NULL,
	[AlternativeStrings] [xml] NULL,
 CONSTRAINT [PK_BiostatusLookup] PRIMARY KEY CLUSTERED 
(
	[TaxonPropertyLookupID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TaxonPropertyClass]    Script Date: 09/08/2010 15:20:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TaxonPropertyClass](
	[TaxonPropertyClassID] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](150) NULL,
	[Description] [nvarchar](max) NULL,
 CONSTRAINT [PK_TaxonPropertyClass] PRIMARY KEY CLUSTERED 
(
	[TaxonPropertyClassID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [cons].[TaxonPropertyValue]    Script Date: 09/08/2010 15:20:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [cons].[TaxonPropertyValue](
	[TaxonPropertyValueID] [uniqueidentifier] NOT NULL,
	[TaxonPropertyID] [uniqueidentifier] NOT NULL,
	[TaxonPropertyTypeID] [uniqueidentifier] NOT NULL,
	[Value] [nvarchar](50) NULL,
 CONSTRAINT [PK_BiostatusValue] PRIMARY KEY CLUSTERED 
(
	[TaxonPropertyValueID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TaxonPropertyType]    Script Date: 09/08/2010 15:20:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TaxonPropertyType](
	[TaxonPropertyTypeID] [uniqueidentifier] NOT NULL,
	[TaxonPropertyClassID] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Description] [nvarchar](max) NULL,
 CONSTRAINT [PK_BiostatusValueType] PRIMARY KEY CLUSTERED 
(
	[TaxonPropertyTypeID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  StoredProcedure [dbo].[sprSelect_ParentAtRank]    Script Date: 09/08/2010 15:20:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[sprSelect_ParentAtRank]
	@nameId uniqueidentifier,
	@taxonRankId uniqueidentifier
AS
	
	select NameID 
	from cons.FlatName
	where TaxonRankID = @taxonRankId and SeedNameId = @nameId
GO
/****** Object:  Table [prov].[Name]    Script Date: 09/08/2010 15:20:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [prov].[Name](
	[NameID] [uniqueidentifier] NOT NULL,
	[ConsensusNameID] [uniqueidentifier] NULL,
	[LinkStatus] [varchar](15) NULL,
	[MatchScore] [int] NULL,
	[FullName] [nvarchar](500) NULL,
	[TaxonRankID] [uniqueidentifier] NULL,
	[NameClassID] [uniqueidentifier] NOT NULL,
	[OriginalOrthography] [nvarchar](500) NULL,
	[GoverningCode] [varchar](5) NULL,
	[ProviderID] [uniqueidentifier] NULL,
	[ProviderRecordID] [nvarchar](1000) NULL,
	[ProviderUpdatedDate] [datetime] NULL,
	[AddedDate] [datetime] NULL,
 CONSTRAINT [PK_Name] PRIMARY KEY CLUSTERED 
(
	[NameID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [cons].[Name]    Script Date: 09/08/2010 15:20:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [cons].[Name](
	[NameID] [uniqueidentifier] NOT NULL,
	[FullName] [nvarchar](500) NULL,
	[TaxonRankID] [uniqueidentifier] NULL,
	[NameClassID] [uniqueidentifier] NOT NULL,
	[OriginalOrthography] [nvarchar](500) NULL,
	[GoverningCode] [varchar](5) NULL,
	[AddedDate] [datetime] NULL,
	[UpdatedDate] [datetime] NULL,
 CONSTRAINT [PK_Name_1] PRIMARY KEY CLUSTERED 
(
	[NameID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[GeoRegion]    Script Date: 09/08/2010 15:20:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[GeoRegion](
	[GeoRegionID] [uniqueidentifier] NOT NULL,
	[RegionName] [nvarchar](500) NOT NULL,
	[Language] [nvarchar](50) NULL,
	[GeoRegionSchemaID] [uniqueidentifier] NOT NULL,
	[CorrectRegionID] [uniqueidentifier] NULL,
	[ParentRegionID] [uniqueidentifier] NULL,
 CONSTRAINT [PK_GeoRegion] PRIMARY KEY CLUSTERED 
(
	[GeoRegionID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  UserDefinedFunction [dbo].[fnLevenshteinPercentage]    Script Date: 09/08/2010 15:20:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create Function [dbo].[fnLevenshteinPercentage]
(
	@strA nvarchar(1000),
	@strB nvarchar(1000)
)
returns int

AS

begin

	declare @max int, @lev int
	
	if (isnull(@strA,'') = '' or isnull(@strB,'') = '') return 0
	
	set @max = dbo.fnMax(len(@strA), len(@strB))
	set @lev = dbo.fnLevenshteinDistance(@strA, @strB)
	
	return 100 - (@lev * 100 / @max)
end
GO
/****** Object:  Table [prov].[Reference]    Script Date: 09/08/2010 15:20:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [prov].[Reference](
	[ReferenceID] [uniqueidentifier] NOT NULL,
	[ReferenceTypeID] [uniqueidentifier] NOT NULL,
	[ConsensusReferenceID] [uniqueidentifier] NULL,
	[LinkStatus] [varchar](15) NULL,
	[MatchScore] [int] NULL,
	[ProviderID] [uniqueidentifier] NULL,
	[ProviderRecordID] [nvarchar](1000) NULL,
	[ProviderUpdatedDate] [datetime] NULL,
	[AddedDate] [datetime] NULL,
 CONSTRAINT [PK_Reference_1] PRIMARY KEY CLUSTERED 
(
	[ReferenceID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [cons].[Reference]    Script Date: 09/08/2010 15:20:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [cons].[Reference](
	[ReferenceID] [uniqueidentifier] NOT NULL,
	[ReferenceTypeID] [uniqueidentifier] NOT NULL,
	[AddedDate] [datetime] NOT NULL,
	[UpdatedDate] [datetime] NULL,
 CONSTRAINT [PK_Reference] PRIMARY KEY CLUSTERED 
(
	[ReferenceID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[NameClassProperty]    Script Date: 09/08/2010 15:20:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[NameClassProperty](
	[NameClassPropertyID] [uniqueidentifier] NOT NULL,
	[NameClassID] [uniqueidentifier] NOT NULL,
	[PropertyName] [nvarchar](50) NULL,
	[PropertyType] [nchar](10) NULL,
	[MinOccurrences] [int] NULL,
	[MaxOccurrences] [int] NULL,
	[GoverningCode] [varchar](5) NULL,
 CONSTRAINT [PK_NameClassProperty] PRIMARY KEY CLUSTERED 
(
	[NameClassPropertyID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ReferenceFieldMap]    Script Date: 09/08/2010 15:20:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ReferenceFieldMap](
	[ReferenceFieldMapID] [uniqueidentifier] NOT NULL,
	[ReferenceTypeID] [uniqueidentifier] NOT NULL,
	[ReferenceFieldTypeID] [uniqueidentifier] NOT NULL,
	[MinimumOccurrence] [int] NULL,
	[MaximumOccurrence] [int] NULL,
	[Sequence] [int] NULL,
	[Label] [nvarchar](150) NULL,
 CONSTRAINT [PK_ReferenceFieldMap] PRIMARY KEY CLUSTERED 
(
	[ReferenceFieldMapID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [prov].[ReferenceField]    Script Date: 09/08/2010 15:20:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [prov].[ReferenceField](
	[ReferenceFieldID] [uniqueidentifier] NOT NULL,
	[ReferenceID] [uniqueidentifier] NOT NULL,
	[ReferenceFieldTypeID] [uniqueidentifier] NOT NULL,
	[Value] [nvarchar](max) NULL,
 CONSTRAINT [PK_ReferenceField] PRIMARY KEY CLUSTERED 
(
	[ReferenceFieldID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [cons].[ReferenceField]    Script Date: 09/08/2010 15:20:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [cons].[ReferenceField](
	[ReferenceFieldID] [uniqueidentifier] NOT NULL,
	[ReferenceID] [uniqueidentifier] NOT NULL,
	[ReferenceFieldTypeID] [uniqueidentifier] NOT NULL,
	[Value] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_ReferenceField_1] PRIMARY KEY CLUSTERED 
(
	[ReferenceFieldID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [prov].[NameProperty]    Script Date: 09/08/2010 15:20:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [prov].[NameProperty](
	[NamePropertyID] [uniqueidentifier] NOT NULL,
	[NameID] [uniqueidentifier] NOT NULL,
	[NameClassPropertyID] [uniqueidentifier] NOT NULL,
	[Value] [nvarchar](max) NULL,
	[RelatedID] [uniqueidentifier] NULL,
	[ProviderRelatedID] [nvarchar](1000) NULL,
	[Sequence] [int] NULL,
 CONSTRAINT [PK_NameProperty] PRIMARY KEY CLUSTERED 
(
	[NamePropertyID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [cons].[NameProperty]    Script Date: 09/08/2010 15:20:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [cons].[NameProperty](
	[NamePropertyID] [uniqueidentifier] NOT NULL,
	[NameID] [uniqueidentifier] NOT NULL,
	[NameClassPropertyID] [uniqueidentifier] NOT NULL,
	[Value] [nvarchar](max) NULL,
	[RelatedID] [uniqueidentifier] NULL,
	[Sequence] [int] NULL,
	[AddedDate] [datetime] NOT NULL,
	[UpdatedDate] [datetime] NULL,
 CONSTRAINT [PK_NameProperty_1] PRIMARY KEY CLUSTERED 
(
	[NamePropertyID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [prov].[Concept]    Script Date: 09/08/2010 15:20:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [prov].[Concept](
	[ConceptID] [uniqueidentifier] NOT NULL,
	[ConsensusConceptID] [uniqueidentifier] NULL,
	[LinkStatus] [varchar](15) NULL,
	[MatchScore] [int] NULL,
	[ProviderNameID] [nvarchar](1000) NULL,
	[NameID] [uniqueidentifier] NOT NULL,
	[ProviderReferenceID] [nvarchar](1000) NULL,
	[AccordingToReferenceID] [uniqueidentifier] NULL,
	[Orthography] [nvarchar](1000) NULL,
	[ProviderID] [uniqueidentifier] NULL,
	[ProviderRecordID] [nvarchar](1000) NULL,
	[ProviderUpdatedDate] [datetime] NULL,
	[AddedDate] [datetime] NULL,
 CONSTRAINT [PK_Concept] PRIMARY KEY CLUSTERED 
(
	[ConceptID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [cons].[Concept]    Script Date: 09/08/2010 15:20:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [cons].[Concept](
	[ConceptID] [uniqueidentifier] NOT NULL,
	[NameID] [uniqueidentifier] NOT NULL,
	[AccordingToReferenceID] [uniqueidentifier] NULL,
	[Orthography] [nvarchar](1000) NULL,
	[AddedDate] [datetime] NOT NULL,
	[UpdatedDate] [datetime] NULL,
 CONSTRAINT [PK_Concept_1] PRIMARY KEY CLUSTERED 
(
	[ConceptID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [cons].[TaxonProperty]    Script Date: 09/08/2010 15:20:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [cons].[TaxonProperty](
	[TaxonPropertyID] [uniqueidentifier] NOT NULL,
	[TaxonPropertyClassID] [uniqueidentifier] NOT NULL,
	[GeoRegionID] [uniqueidentifier] NULL,
	[ReferenceID] [uniqueidentifier] NULL,
	[ConceptID] [uniqueidentifier] NULL,
	[NameID] [uniqueidentifier] NULL,
	[InUse] [bit] NULL,
	[AddedDate] [datetime] NULL,
	[UpdatedDate] [datetime] NULL,
 CONSTRAINT [PK_TaxonProperty] PRIMARY KEY CLUSTERED 
(
	[TaxonPropertyID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [prov].[TaxonProperty]    Script Date: 09/08/2010 15:20:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [prov].[TaxonProperty](
	[TaxonPropertyID] [uniqueidentifier] NOT NULL,
	[TaxonPropertyClassID] [uniqueidentifier] NOT NULL,
	[ConsensusTaxonPropertyID] [uniqueidentifier] NULL,
	[LinkStatus] [varchar](15) NULL,
	[MatchScore] [int] NULL,
	[ProviderRecordID] [nvarchar](1000) NOT NULL,
	[GeoSchema] [nvarchar](500) NULL,
	[GeoRegion] [nvarchar](500) NULL,
	[GeoRegionID] [uniqueidentifier] NULL,
	[ProviderReferenceID] [nvarchar](1000) NULL,
	[ReferenceID] [uniqueidentifier] NULL,
	[ConceptID] [uniqueidentifier] NULL,
	[ProviderNameID] [nvarchar](1000) NULL,
	[NameID] [uniqueidentifier] NULL,
	[ProviderID] [uniqueidentifier] NOT NULL,
	[InUse] [bit] NULL,
	[ProviderUpdatedDate] [datetime] NULL,
	[AddedDate] [datetime] NULL,
 CONSTRAINT [PK_Biostatus] PRIMARY KEY CLUSTERED 
(
	[TaxonPropertyID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [prov].[ConceptRelationship]    Script Date: 09/08/2010 15:20:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [prov].[ConceptRelationship](
	[ConceptRelationshipID] [uniqueidentifier] NOT NULL,
	[FromConceptID] [uniqueidentifier] NULL,
	[ToConceptID] [uniqueidentifier] NULL,
	[RelationshipTypeID] [uniqueidentifier] NULL,
	[Sequence] [int] NULL,
	[InUse] [bit] NULL,
 CONSTRAINT [PK_ConceptRelationship] PRIMARY KEY CLUSTERED 
(
	[ConceptRelationshipID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [cons].[ConceptRelationship]    Script Date: 09/08/2010 15:20:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [cons].[ConceptRelationship](
	[ConceptRelationshipID] [uniqueidentifier] NOT NULL,
	[FromConceptID] [uniqueidentifier] NOT NULL,
	[ToConceptID] [uniqueidentifier] NOT NULL,
	[ConceptRelationshipTypeID] [uniqueidentifier] NOT NULL,
	[Sequence] [int] NULL,
	[AddedDate] [datetime] NOT NULL,
	[UpdatedDate] [datetime] NULL,
 CONSTRAINT [PK_ConceptRelationship_1] PRIMARY KEY CLUSTERED 
(
	[ConceptRelationshipID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  UserDefinedFunction [dbo].[GetParentNameList]    Script Date: 09/08/2010 15:20:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[GetParentNameList](@ChildNameID UNIQUEIDENTIFIER)
RETURNS @Result TABLE
(
    NameID UNIQUEIDENTIFIER NOT NULL,
    ParentNameID UNIQUEIDENTIFIER NOT NULL,
    AccordingToReferenceID UNIQUEIDENTIFIER NOT NULL,
    Level INT NOT NULL
)
/*
<Documentation>
	<Name>
		GetParentNameList
	</Name>

	<CodeItemType>
		User Defined Function
	</CodeItemType>

	<VersionDate>
		23-February-2010
	</VersionDate>

	<Description>
		Returns a list of the parent names for a given name.

        This is the 'flat names' structure.
	</Description>

	<Example>
		SELECT * FROM dbo.GetParentNameList('CEF15938-7C82-4A0C-B6C7-21DC7EFA8D26')

        SELECT
            *
        FROM
            dbo.GetParentNameList('CEF15938-7C82-4A0C-B6C7-21DC7EFA8D26') tbl
            INNER JOIN cons.Name Name
                ON tbl.NameID = Name.NameID

		SELECT * FROM dbo.GetParentNameList('00000000-0000-0000-0000-000000000024')

        SELECT
            Name.NameID,
            ParentList.ParentNameID,
            Name.FullName,
            ParentName.FullName
        FROM
            cons.Name Name 
            CROSS APPLY dbo.GetParentNameList(NameID) ParentList
            INNER JOIN cons.Name ParentName
                ON ParentList.ParentNameID = ParentName.NameID
        ORDER BY
            Name.NameID,
            ParentList.Level DESC
	</Example>
</Documentation>
*/

AS

BEGIN

    DECLARE @IsChildOfConceptRelationshipTypeID UNIQUEIDENTIFIER 
    SET @IsChildOfConceptRelationshipTypeID = '6A11B466-1907-446F-9229-D604579AA155'

    ;WITH CTE
        (
        NameID,
        ParentNameID,
        AccordingToReferenceID,
        Level
        )
    AS
        (
        -- Anchor member
        SELECT
            FromConcept.NameID,
            ToConcept.NameID,
            FromConcept.AccordingToReferenceID,
            0
        FROM
            cons.Concept ToConcept
            INNER JOIN cons.ConceptRelationship ConceptRelationship
                ON ToConcept.ConceptID = ConceptRelationship.ToConceptID
            INNER JOIN cons.Concept FromConcept
                ON ConceptRelationship.FromConceptID = FromConcept.ConceptID
        WHERE
            ConceptRelationship.ConceptRelationshipTypeID = @IsChildOfConceptRelationshipTypeID
            AND FromConcept.NameID = @ChildNameID

        UNION ALL

        -- Recursive member
        SELECT
            FromConcept.NameID,
            ToConcept.NameID,
            FromConcept.AccordingToReferenceID,
            Level + 1
        FROM
            cons.Concept ToConcept
            INNER JOIN cons.ConceptRelationship ConceptRelationship
                ON ToConcept.ConceptID = ConceptRelationship.ToConceptID
            INNER JOIN cons.Concept FromConcept
                ON ConceptRelationship.FromConceptID = FromConcept.ConceptID
            INNER JOIN CTE
                ON FromConcept.NameID = CTE.ParentNameID
        WHERE
            ConceptRelationship.ConceptRelationshipTypeID = @IsChildOfConceptRelationshipTypeID
        )

    -- Outer query
    INSERT INTO
        @Result
    SELECT
        @ChildNameID,
        ParentNameID,
        AccordingToReferenceID,
        Level
    FROM
        CTE
       
    RETURN
END
GO
/****** Object:  View [dbo].[vwProviderConcepts]    Script Date: 09/08/2010 15:20:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE View [dbo].[vwProviderConcepts] AS

SELECT     prov.Name.NameID, prov.Name.ConsensusNameID, prov.Name.FullName, prov.Name.NameClassID, prov.Name.LinkStatus, 
                      prov.Name.OriginalOrthography, prov.Name.GoverningCode, prov.Name.ProviderID, prov.Name.ProviderRecordID, prov.Name.ProviderUpdatedDate, 
                      prov.Name.AddedDate, prov.Concept.ConceptID, prov.Concept.AccordingToReferenceID, prov.Concept.Orthography, 
                      prov.ConceptRelationship.ConceptRelationshipID, prov.ConceptRelationship.FromConceptID, prov.ConceptRelationship.ToConceptID, 
                      prov.ConceptRelationship.RelationshipTypeID, dbo.ConceptRelationshipType.Relationship, prov.ConceptRelationship.Sequence, 
                      ConceptTo.ConceptID AS ConceptToID, ConceptTo.NameID AS NameToID, ConceptTo.AccordingToReferenceID AS ReferenceToID, 
                      NameTo.FullName AS NameToFull, NameTo.LinkStatus AS NameToLinkStatus, NameTo.ProviderRecordID AS NameToProviderRecordID, 
                      prov.Concept.ConsensusConceptID, ConceptTo.ConsensusConceptID AS ConsensusConceptToId, prov.Name.TaxonRankID, 
                      NameTo.TaxonRankID AS TaxonRankToID, dbo.TaxonRank.Name AS RankName, dbo.TaxonRank.SortOrder, 
                      NameTo.ConsensusNameID AS ConsensusNameToID
FROM         prov.Concept INNER JOIN
                      prov.ConceptRelationship ON prov.Concept.ConceptID = prov.ConceptRelationship.FromConceptID INNER JOIN
                      prov.Name ON prov.Concept.NameID = prov.Name.NameID INNER JOIN
                      prov.Concept AS ConceptTo ON prov.ConceptRelationship.ToConceptID = ConceptTo.ConceptID INNER JOIN
                      prov.Name AS NameTo ON ConceptTo.NameID = NameTo.NameID INNER JOIN
                      dbo.ConceptRelationshipType ON 
                      prov.ConceptRelationship.RelationshipTypeID = dbo.ConceptRelationshipType.ConceptRelationshipTypeID INNER JOIN
                      dbo.TaxonRank ON prov.Name.TaxonRankID = dbo.TaxonRank.TaxonRankID
GO
/****** Object:  View [dbo].[vwConsensusConcepts]    Script Date: 09/08/2010 15:20:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE View [dbo].[vwConsensusConcepts] AS

SELECT     cons.Name.NameID, cons.Name.FullName, cons.Name.NameClassID, cons.Name.OriginalOrthography, cons.Name.GoverningCode, 
                      cons.Name.AddedDate, cons.Concept.ConceptID, cons.Concept.AccordingToReferenceID, cons.Concept.Orthography, 
                      cons.ConceptRelationship.ConceptRelationshipID, cons.ConceptRelationship.FromConceptID, cons.ConceptRelationship.ToConceptID, 
                      cons.ConceptRelationship.Sequence, ConceptTo.ConceptID AS ConceptToID, ConceptTo.NameID AS NameToID, 
                      ConceptTo.AccordingToReferenceID AS ReferenceToID, NameTo.FullName AS ConsensusNameToFull, cons.Name.TaxonRankID, 
                      NameTo.TaxonRankID AS TaxonRankToID, dbo.ConceptRelationshipType.Relationship, dbo.ConceptRelationshipType.ConceptRelationshipTypeID
FROM         cons.Concept INNER JOIN
                      cons.ConceptRelationship ON cons.Concept.ConceptID = cons.ConceptRelationship.FromConceptID INNER JOIN
                      cons.Name ON cons.Concept.NameID = cons.Name.NameID INNER JOIN
                      cons.Concept AS ConceptTo ON cons.ConceptRelationship.ToConceptID = ConceptTo.ConceptID INNER JOIN
                      cons.Name AS NameTo ON ConceptTo.NameID = NameTo.NameID INNER JOIN
                      dbo.ConceptRelationshipType ON cons.ConceptRelationship.ConceptRelationshipTypeID = dbo.ConceptRelationshipType.ConceptRelationshipTypeID
GO
/****** Object:  Table [prov].[TaxonPropertyValue]    Script Date: 09/08/2010 15:20:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [prov].[TaxonPropertyValue](
	[TaxonPropertyValueID] [uniqueidentifier] NOT NULL,
	[TaxonPropertyID] [uniqueidentifier] NOT NULL,
	[TaxonPropertyTypeID] [uniqueidentifier] NOT NULL,
	[Value] [nvarchar](50) NULL,
 CONSTRAINT [PK_BiostatusValue] PRIMARY KEY CLUSTERED 
(
	[TaxonPropertyValueID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  StoredProcedure [dbo].[sprSelect_ProvNameMatchingData]    Script Date: 09/08/2010 15:20:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[sprSelect_ProvNameMatchingData]
	@provNameId uniqueidentifier
AS

	select * 
	from prov.Name pn
	inner join TaxonRank tr on tr.TaxonRankID = pn.TaxonRankID
	where NameID = @provNameId
	
	select * 
	from prov.NameProperty np
	inner join NameClassProperty ncp on ncp.NameClassPropertyID = np.NameClassPropertyID
	where NameID = @provNameId
	
	select * 
	from vwProviderConcepts
	where NameID = @provNameId
GO
/****** Object:  StoredProcedure [dbo].[sprSelect_FlatNameToRoot]    Script Date: 09/08/2010 15:20:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[sprSelect_FlatNameToRoot]
	@StartNameId uniqueidentifier
AS

	DECLARE @tableResults	TABLE(NameId uniqueidentifier, NameLevel int )
	DECLARE @ParentId		uniqueidentifier
	DECLARE @NameId		uniqueidentifier
	DECLARE @intCount		int
	DECLARE @GrandParent	uniqueidentifier
	
	DECLARE @intLevel		int
	
	
	SELECT @ParentId = NameToID, @NameId = n.NameId
	FROM  cons.Name n
	inner join vwConsensusConcepts cc on cc.nameid = n.nameid and ConceptRelationshipTypeID = '6A11B466-1907-446F-9229-D604579AA155' -- child
	WHERE n.NameId = @StartNameId
	
	SET @intLevel = 0
	
 	SELECT @intCount = COUNT(*) FROM @tableResults WHERE NameId = @ParentId
	
	WHILE @intCount < 1 
	AND (NOT @NameId IS NULL)
	BEGIN
		
		INSERT INTO @tableResults VALUES(@NameId, @intLevel)
		
		SELECT @GrandParent = NameToID 
		FROM  cons.Name n
		inner join vwConsensusConcepts cc on cc.nameid = n.nameid and ConceptRelationshipTypeID = '6A11B466-1907-446F-9229-D604579AA155' -- child
		WHERE n.NameId = @ParentId
		
		SET @NameId = @ParentId
		SET @ParentId = @GrandParent
		
		SET @intLevel = @intLevel + 1
		
		SELECT @intCount = COUNT(*) FROM @tableResults WHERE NameId = @NameId
	END
	
	
	
	SELECT newid() as FlatNameID,
		cc.NameToID AS ParentNameID,
		cast(n.NameId as varchar(38)) AS NameID, 
		(select Value from cons.NameProperty where NameID = n.NameID and NameClassPropertyID = '1F64E93C-7EE8-40D7-8681-52B56060D750') AS Canonical, 
		tr.TaxonRankID AS TaxonRankID, 
		tr.Name AS RankName, 
		res.NameLevel AS Depth,  
		cast(@StartNameId as varchar(38)) as SeedNameID
	FROM  @tableResults res
	JOIN cons.Name n ON n.NameId = res.NameId
	inner join vwConsensusConcepts cc on cc.nameid = n.nameid and ConceptRelationshipTypeID = '6A11B466-1907-446F-9229-D604579AA155' -- child
	LEFT JOIN dbo.TaxonRank tr ON n.TaxonRankID = tr.TaxonRankID	
	ORDER BY res.NameLevel DESC
GO
/****** Object:  StoredProcedure [dbo].[sprUpdate_FlatNameData]    Script Date: 09/08/2010 15:20:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[sprUpdate_FlatNameData]
AS

	-- clear table
	truncate table cons.FlatName

	DECLARE @NameId uniqueidentifier

	-- iterate through name table
	DECLARE names_cursor   CURSOR FORWARD_ONLY FOR
	SELECT  NameID FROM cons.Name
	ORDER BY NameID

	OPEN names_cursor
	-- Perform the first fetch.
	FETCH NEXT FROM names_cursor INTO @NameId

	-- Check @@FETCH_STATUS to see if there are any more rows to fetch.
	WHILE @@FETCH_STATUS = 0
		BEGIN

		INSERT cons.FlatName
		EXEC sprSelect_FlatNameToRoot @NameId

		-- This is executed as long as the previous fetch succeeds.
		FETCH NEXT FROM names_cursor INTO @NameId
		END

	CLOSE names_cursor
	DEALLOCATE names_cursor
GO
/****** Object:  Default [DF_FlatName_FlatNameID]    Script Date: 09/08/2010 15:20:18 ******/
ALTER TABLE [cons].[FlatName] ADD  CONSTRAINT [DF_FlatName_FlatNameID]  DEFAULT (newid()) FOR [FlatNameID]
GO
/****** Object:  Default [DF_NameClassProperty_NameClassPropertyID]    Script Date: 09/08/2010 15:20:18 ******/
ALTER TABLE [dbo].[NameClassProperty] ADD  CONSTRAINT [DF_NameClassProperty_NameClassPropertyID]  DEFAULT (newid()) FOR [NameClassPropertyID]
GO
/****** Object:  ForeignKey [FK_Concept_Name]    Script Date: 09/08/2010 15:20:18 ******/
ALTER TABLE [cons].[Concept]  WITH CHECK ADD  CONSTRAINT [FK_Concept_Name] FOREIGN KEY([NameID])
REFERENCES [cons].[Name] ([NameID])
GO
ALTER TABLE [cons].[Concept] CHECK CONSTRAINT [FK_Concept_Name]
GO
/****** Object:  ForeignKey [FK_ConceptRelationship_Concept]    Script Date: 09/08/2010 15:20:18 ******/
ALTER TABLE [cons].[ConceptRelationship]  WITH CHECK ADD  CONSTRAINT [FK_ConceptRelationship_Concept] FOREIGN KEY([FromConceptID])
REFERENCES [cons].[Concept] ([ConceptID])
GO
ALTER TABLE [cons].[ConceptRelationship] CHECK CONSTRAINT [FK_ConceptRelationship_Concept]
GO
/****** Object:  ForeignKey [FK_ConceptRelationship_Concept1]    Script Date: 09/08/2010 15:20:18 ******/
ALTER TABLE [cons].[ConceptRelationship]  WITH CHECK ADD  CONSTRAINT [FK_ConceptRelationship_Concept1] FOREIGN KEY([ToConceptID])
REFERENCES [cons].[Concept] ([ConceptID])
GO
ALTER TABLE [cons].[ConceptRelationship] CHECK CONSTRAINT [FK_ConceptRelationship_Concept1]
GO
/****** Object:  ForeignKey [FK_ConceptRelationship_ConceptRelationshipType]    Script Date: 09/08/2010 15:20:18 ******/
ALTER TABLE [cons].[ConceptRelationship]  WITH CHECK ADD  CONSTRAINT [FK_ConceptRelationship_ConceptRelationshipType] FOREIGN KEY([ConceptRelationshipTypeID])
REFERENCES [dbo].[ConceptRelationshipType] ([ConceptRelationshipTypeID])
GO
ALTER TABLE [cons].[ConceptRelationship] CHECK CONSTRAINT [FK_ConceptRelationship_ConceptRelationshipType]
GO
/****** Object:  ForeignKey [FK_Name_NameClass]    Script Date: 09/08/2010 15:20:18 ******/
ALTER TABLE [cons].[Name]  WITH CHECK ADD  CONSTRAINT [FK_Name_NameClass] FOREIGN KEY([NameClassID])
REFERENCES [dbo].[NameClass] ([NameClassID])
GO
ALTER TABLE [cons].[Name] CHECK CONSTRAINT [FK_Name_NameClass]
GO
/****** Object:  ForeignKey [FK_Name_TaxonRank]    Script Date: 09/08/2010 15:20:18 ******/
ALTER TABLE [cons].[Name]  WITH CHECK ADD  CONSTRAINT [FK_Name_TaxonRank] FOREIGN KEY([TaxonRankID])
REFERENCES [dbo].[TaxonRank] ([TaxonRankID])
GO
ALTER TABLE [cons].[Name] CHECK CONSTRAINT [FK_Name_TaxonRank]
GO
/****** Object:  ForeignKey [FK_NameProperty_Name]    Script Date: 09/08/2010 15:20:18 ******/
ALTER TABLE [cons].[NameProperty]  WITH CHECK ADD  CONSTRAINT [FK_NameProperty_Name] FOREIGN KEY([NameID])
REFERENCES [cons].[Name] ([NameID])
GO
ALTER TABLE [cons].[NameProperty] CHECK CONSTRAINT [FK_NameProperty_Name]
GO
/****** Object:  ForeignKey [FK_NameProperty_NameClassProperty]    Script Date: 09/08/2010 15:20:18 ******/
ALTER TABLE [cons].[NameProperty]  WITH CHECK ADD  CONSTRAINT [FK_NameProperty_NameClassProperty] FOREIGN KEY([NameClassPropertyID])
REFERENCES [dbo].[NameClassProperty] ([NameClassPropertyID])
GO
ALTER TABLE [cons].[NameProperty] CHECK CONSTRAINT [FK_NameProperty_NameClassProperty]
GO
/****** Object:  ForeignKey [FK_NameProperty_NameClassProperty1]    Script Date: 09/08/2010 15:20:18 ******/
ALTER TABLE [cons].[NameProperty]  WITH CHECK ADD  CONSTRAINT [FK_NameProperty_NameClassProperty1] FOREIGN KEY([NameClassPropertyID])
REFERENCES [dbo].[NameClassProperty] ([NameClassPropertyID])
GO
ALTER TABLE [cons].[NameProperty] CHECK CONSTRAINT [FK_NameProperty_NameClassProperty1]
GO
/****** Object:  ForeignKey [FK_Reference_ReferenceType]    Script Date: 09/08/2010 15:20:18 ******/
ALTER TABLE [cons].[Reference]  WITH CHECK ADD  CONSTRAINT [FK_Reference_ReferenceType] FOREIGN KEY([ReferenceTypeID])
REFERENCES [dbo].[ReferenceType] ([ReferenceTypeID])
GO
ALTER TABLE [cons].[Reference] CHECK CONSTRAINT [FK_Reference_ReferenceType]
GO
/****** Object:  ForeignKey [FK_ReferenceField_Reference]    Script Date: 09/08/2010 15:20:18 ******/
ALTER TABLE [cons].[ReferenceField]  WITH CHECK ADD  CONSTRAINT [FK_ReferenceField_Reference] FOREIGN KEY([ReferenceID])
REFERENCES [cons].[Reference] ([ReferenceID])
GO
ALTER TABLE [cons].[ReferenceField] CHECK CONSTRAINT [FK_ReferenceField_Reference]
GO
/****** Object:  ForeignKey [FK_ReferenceField_ReferenceFieldType]    Script Date: 09/08/2010 15:20:18 ******/
ALTER TABLE [cons].[ReferenceField]  WITH CHECK ADD  CONSTRAINT [FK_ReferenceField_ReferenceFieldType] FOREIGN KEY([ReferenceFieldTypeID])
REFERENCES [dbo].[ReferenceFieldType] ([ReferenceFieldTypeID])
GO
ALTER TABLE [cons].[ReferenceField] CHECK CONSTRAINT [FK_ReferenceField_ReferenceFieldType]
GO
/****** Object:  ForeignKey [FK_TaxonProperty_GeoRegion]    Script Date: 09/08/2010 15:20:18 ******/
ALTER TABLE [cons].[TaxonProperty]  WITH CHECK ADD  CONSTRAINT [FK_TaxonProperty_GeoRegion] FOREIGN KEY([GeoRegionID])
REFERENCES [dbo].[GeoRegion] ([GeoRegionID])
GO
ALTER TABLE [cons].[TaxonProperty] CHECK CONSTRAINT [FK_TaxonProperty_GeoRegion]
GO
/****** Object:  ForeignKey [FK_TaxonProperty_Name]    Script Date: 09/08/2010 15:20:18 ******/
ALTER TABLE [cons].[TaxonProperty]  WITH CHECK ADD  CONSTRAINT [FK_TaxonProperty_Name] FOREIGN KEY([NameID])
REFERENCES [cons].[Name] ([NameID])
GO
ALTER TABLE [cons].[TaxonProperty] CHECK CONSTRAINT [FK_TaxonProperty_Name]
GO
/****** Object:  ForeignKey [FK_TaxonProperty_Reference]    Script Date: 09/08/2010 15:20:18 ******/
ALTER TABLE [cons].[TaxonProperty]  WITH CHECK ADD  CONSTRAINT [FK_TaxonProperty_Reference] FOREIGN KEY([ReferenceID])
REFERENCES [cons].[Reference] ([ReferenceID])
GO
ALTER TABLE [cons].[TaxonProperty] CHECK CONSTRAINT [FK_TaxonProperty_Reference]
GO
/****** Object:  ForeignKey [FK_TaxonProperty_TaxonPropertyClass]    Script Date: 09/08/2010 15:20:18 ******/
ALTER TABLE [cons].[TaxonProperty]  WITH CHECK ADD  CONSTRAINT [FK_TaxonProperty_TaxonPropertyClass] FOREIGN KEY([TaxonPropertyClassID])
REFERENCES [dbo].[TaxonPropertyClass] ([TaxonPropertyClassID])
GO
ALTER TABLE [cons].[TaxonProperty] CHECK CONSTRAINT [FK_TaxonProperty_TaxonPropertyClass]
GO
/****** Object:  ForeignKey [FK_GeoRegion_GeoRegionSchema]    Script Date: 09/08/2010 15:20:18 ******/
ALTER TABLE [dbo].[GeoRegion]  WITH CHECK ADD  CONSTRAINT [FK_GeoRegion_GeoRegionSchema] FOREIGN KEY([GeoRegionSchemaID])
REFERENCES [dbo].[GeoRegionSchema] ([GeoRegionSchemaID])
GO
ALTER TABLE [dbo].[GeoRegion] CHECK CONSTRAINT [FK_GeoRegion_GeoRegionSchema]
GO
/****** Object:  ForeignKey [FK_NameClassProperty_NameClass]    Script Date: 09/08/2010 15:20:18 ******/
ALTER TABLE [dbo].[NameClassProperty]  WITH CHECK ADD  CONSTRAINT [FK_NameClassProperty_NameClass] FOREIGN KEY([NameClassID])
REFERENCES [dbo].[NameClass] ([NameClassID])
GO
ALTER TABLE [dbo].[NameClassProperty] CHECK CONSTRAINT [FK_NameClassProperty_NameClass]
GO
/****** Object:  ForeignKey [FK_ReferenceFieldMap_ReferenceFieldType]    Script Date: 09/08/2010 15:20:18 ******/
ALTER TABLE [dbo].[ReferenceFieldMap]  WITH CHECK ADD  CONSTRAINT [FK_ReferenceFieldMap_ReferenceFieldType] FOREIGN KEY([ReferenceFieldTypeID])
REFERENCES [dbo].[ReferenceFieldType] ([ReferenceFieldTypeID])
GO
ALTER TABLE [dbo].[ReferenceFieldMap] CHECK CONSTRAINT [FK_ReferenceFieldMap_ReferenceFieldType]
GO
/****** Object:  ForeignKey [FK_ReferenceFieldMap_ReferenceType]    Script Date: 09/08/2010 15:20:18 ******/
ALTER TABLE [dbo].[ReferenceFieldMap]  WITH CHECK ADD  CONSTRAINT [FK_ReferenceFieldMap_ReferenceType] FOREIGN KEY([ReferenceTypeID])
REFERENCES [dbo].[ReferenceType] ([ReferenceTypeID])
GO
ALTER TABLE [dbo].[ReferenceFieldMap] CHECK CONSTRAINT [FK_ReferenceFieldMap_ReferenceType]
GO
/****** Object:  ForeignKey [FK_TaxonPropertyType_TaxonPropertyClass]    Script Date: 09/08/2010 15:20:18 ******/
ALTER TABLE [dbo].[TaxonPropertyType]  WITH CHECK ADD  CONSTRAINT [FK_TaxonPropertyType_TaxonPropertyClass] FOREIGN KEY([TaxonPropertyClassID])
REFERENCES [dbo].[TaxonPropertyClass] ([TaxonPropertyClassID])
GO
ALTER TABLE [dbo].[TaxonPropertyType] CHECK CONSTRAINT [FK_TaxonPropertyType_TaxonPropertyClass]
GO
/****** Object:  ForeignKey [FK_Concept_Name]    Script Date: 09/08/2010 15:20:18 ******/
ALTER TABLE [prov].[Concept]  WITH CHECK ADD  CONSTRAINT [FK_Concept_Name] FOREIGN KEY([NameID])
REFERENCES [prov].[Name] ([NameID])
GO
ALTER TABLE [prov].[Concept] CHECK CONSTRAINT [FK_Concept_Name]
GO
/****** Object:  ForeignKey [FK_Concept_Provider]    Script Date: 09/08/2010 15:20:18 ******/
ALTER TABLE [prov].[Concept]  WITH CHECK ADD  CONSTRAINT [FK_Concept_Provider] FOREIGN KEY([ProviderID])
REFERENCES [dbo].[Provider] ([ProviderID])
GO
ALTER TABLE [prov].[Concept] CHECK CONSTRAINT [FK_Concept_Provider]
GO
/****** Object:  ForeignKey [FK_ConceptRelationship_Concept]    Script Date: 09/08/2010 15:20:18 ******/
ALTER TABLE [prov].[ConceptRelationship]  WITH CHECK ADD  CONSTRAINT [FK_ConceptRelationship_Concept] FOREIGN KEY([FromConceptID])
REFERENCES [prov].[Concept] ([ConceptID])
GO
ALTER TABLE [prov].[ConceptRelationship] CHECK CONSTRAINT [FK_ConceptRelationship_Concept]
GO
/****** Object:  ForeignKey [FK_ConceptRelationship_Concept1]    Script Date: 09/08/2010 15:20:18 ******/
ALTER TABLE [prov].[ConceptRelationship]  WITH CHECK ADD  CONSTRAINT [FK_ConceptRelationship_Concept1] FOREIGN KEY([ToConceptID])
REFERENCES [prov].[Concept] ([ConceptID])
GO
ALTER TABLE [prov].[ConceptRelationship] CHECK CONSTRAINT [FK_ConceptRelationship_Concept1]
GO
/****** Object:  ForeignKey [FK_ConceptRelationship_ConceptRelationshipType]    Script Date: 09/08/2010 15:20:18 ******/
ALTER TABLE [prov].[ConceptRelationship]  WITH CHECK ADD  CONSTRAINT [FK_ConceptRelationship_ConceptRelationshipType] FOREIGN KEY([RelationshipTypeID])
REFERENCES [dbo].[ConceptRelationshipType] ([ConceptRelationshipTypeID])
GO
ALTER TABLE [prov].[ConceptRelationship] CHECK CONSTRAINT [FK_ConceptRelationship_ConceptRelationshipType]
GO
/****** Object:  ForeignKey [FK_Name_Provider]    Script Date: 09/08/2010 15:20:18 ******/
ALTER TABLE [prov].[Name]  WITH CHECK ADD  CONSTRAINT [FK_Name_Provider] FOREIGN KEY([ProviderID])
REFERENCES [dbo].[Provider] ([ProviderID])
GO
ALTER TABLE [prov].[Name] CHECK CONSTRAINT [FK_Name_Provider]
GO
/****** Object:  ForeignKey [FK_NameProperty_Name]    Script Date: 09/08/2010 15:20:18 ******/
ALTER TABLE [prov].[NameProperty]  WITH NOCHECK ADD  CONSTRAINT [FK_NameProperty_Name] FOREIGN KEY([NameID])
REFERENCES [prov].[Name] ([NameID])
GO
ALTER TABLE [prov].[NameProperty] CHECK CONSTRAINT [FK_NameProperty_Name]
GO
/****** Object:  ForeignKey [FK_NameProperty_NameClassProperty]    Script Date: 09/08/2010 15:20:18 ******/
ALTER TABLE [prov].[NameProperty]  WITH CHECK ADD  CONSTRAINT [FK_NameProperty_NameClassProperty] FOREIGN KEY([NameClassPropertyID])
REFERENCES [dbo].[NameClassProperty] ([NameClassPropertyID])
GO
ALTER TABLE [prov].[NameProperty] CHECK CONSTRAINT [FK_NameProperty_NameClassProperty]
GO
/****** Object:  ForeignKey [FK_Reference_Provider]    Script Date: 09/08/2010 15:20:18 ******/
ALTER TABLE [prov].[Reference]  WITH CHECK ADD  CONSTRAINT [FK_Reference_Provider] FOREIGN KEY([ProviderID])
REFERENCES [dbo].[Provider] ([ProviderID])
GO
ALTER TABLE [prov].[Reference] CHECK CONSTRAINT [FK_Reference_Provider]
GO
/****** Object:  ForeignKey [FK_Reference_ReferenceType]    Script Date: 09/08/2010 15:20:18 ******/
ALTER TABLE [prov].[Reference]  WITH CHECK ADD  CONSTRAINT [FK_Reference_ReferenceType] FOREIGN KEY([ReferenceTypeID])
REFERENCES [dbo].[ReferenceType] ([ReferenceTypeID])
GO
ALTER TABLE [prov].[Reference] CHECK CONSTRAINT [FK_Reference_ReferenceType]
GO
/****** Object:  ForeignKey [FK_ReferenceField_Reference]    Script Date: 09/08/2010 15:20:18 ******/
ALTER TABLE [prov].[ReferenceField]  WITH CHECK ADD  CONSTRAINT [FK_ReferenceField_Reference] FOREIGN KEY([ReferenceID])
REFERENCES [prov].[Reference] ([ReferenceID])
GO
ALTER TABLE [prov].[ReferenceField] CHECK CONSTRAINT [FK_ReferenceField_Reference]
GO
/****** Object:  ForeignKey [FK_ReferenceField_ReferenceFieldType]    Script Date: 09/08/2010 15:20:18 ******/
ALTER TABLE [prov].[ReferenceField]  WITH CHECK ADD  CONSTRAINT [FK_ReferenceField_ReferenceFieldType] FOREIGN KEY([ReferenceFieldTypeID])
REFERENCES [dbo].[ReferenceFieldType] ([ReferenceFieldTypeID])
GO
ALTER TABLE [prov].[ReferenceField] CHECK CONSTRAINT [FK_ReferenceField_ReferenceFieldType]
GO
/****** Object:  ForeignKey [FK_Biostatus_Concept]    Script Date: 09/08/2010 15:20:18 ******/
ALTER TABLE [prov].[TaxonProperty]  WITH CHECK ADD  CONSTRAINT [FK_Biostatus_Concept] FOREIGN KEY([ConceptID])
REFERENCES [prov].[Concept] ([ConceptID])
GO
ALTER TABLE [prov].[TaxonProperty] CHECK CONSTRAINT [FK_Biostatus_Concept]
GO
/****** Object:  ForeignKey [FK_Biostatus_Name]    Script Date: 09/08/2010 15:20:18 ******/
ALTER TABLE [prov].[TaxonProperty]  WITH CHECK ADD  CONSTRAINT [FK_Biostatus_Name] FOREIGN KEY([NameID])
REFERENCES [prov].[Name] ([NameID])
GO
ALTER TABLE [prov].[TaxonProperty] CHECK CONSTRAINT [FK_Biostatus_Name]
GO
/****** Object:  ForeignKey [FK_Biostatus_Provider]    Script Date: 09/08/2010 15:20:18 ******/
ALTER TABLE [prov].[TaxonProperty]  WITH CHECK ADD  CONSTRAINT [FK_Biostatus_Provider] FOREIGN KEY([ProviderID])
REFERENCES [dbo].[Provider] ([ProviderID])
GO
ALTER TABLE [prov].[TaxonProperty] CHECK CONSTRAINT [FK_Biostatus_Provider]
GO
/****** Object:  ForeignKey [FK_TaxonProperty_TaxonPropertyClass]    Script Date: 09/08/2010 15:20:18 ******/
ALTER TABLE [prov].[TaxonProperty]  WITH CHECK ADD  CONSTRAINT [FK_TaxonProperty_TaxonPropertyClass] FOREIGN KEY([TaxonPropertyClassID])
REFERENCES [dbo].[TaxonPropertyClass] ([TaxonPropertyClassID])
GO
ALTER TABLE [prov].[TaxonProperty] CHECK CONSTRAINT [FK_TaxonProperty_TaxonPropertyClass]
GO
/****** Object:  ForeignKey [FK_TaxonPropertyValue_TaxonProperty]    Script Date: 09/08/2010 15:20:18 ******/
ALTER TABLE [prov].[TaxonPropertyValue]  WITH CHECK ADD  CONSTRAINT [FK_TaxonPropertyValue_TaxonProperty] FOREIGN KEY([TaxonPropertyID])
REFERENCES [prov].[TaxonProperty] ([TaxonPropertyID])
GO
ALTER TABLE [prov].[TaxonPropertyValue] CHECK CONSTRAINT [FK_TaxonPropertyValue_TaxonProperty]
GO
/****** Object:  ForeignKey [FK_TaxonPropertyValue_TaxonPropertyType]    Script Date: 09/08/2010 15:20:18 ******/
ALTER TABLE [prov].[TaxonPropertyValue]  WITH CHECK ADD  CONSTRAINT [FK_TaxonPropertyValue_TaxonPropertyType] FOREIGN KEY([TaxonPropertyTypeID])
REFERENCES [dbo].[TaxonPropertyType] ([TaxonPropertyTypeID])
GO
ALTER TABLE [prov].[TaxonPropertyValue] CHECK CONSTRAINT [FK_TaxonPropertyValue_TaxonPropertyType]
GO
