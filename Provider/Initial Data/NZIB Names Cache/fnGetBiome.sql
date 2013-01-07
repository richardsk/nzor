
IF EXISTS (SELECT * FROM sysobjects WHERE type = 'FN' AND name = 'fnGetBiome')
	BEGIN
		DROP  Function  dbo.fnGetBiome
	END
GO

CREATE Function dbo.fnGetBiome
(
	@biostatusComment nvarchar(1000)
)
returns nvarchar(1000)
AS
begin
	declare @biome nvarchar(1000)	
	declare @pos int, @endPos int
	
	set @pos = CHARINDEX('Biome=', @biostatuscomment) + 5
	set @endPos = CHARINDEX(';', @biostatusComment, @pos)
	set @biome = ''
	
	while (@pos > 5 and @endPos <> 0)
	begin		
		set @biome = @biome + SUBSTRING(@biostatusComment, @pos + 1, @endPos - (@pos+1))
		set @biome = @biome + ','
		
		set @pos = CHARINDEX('=', @biostatuscomment, @endPos)
		set @endPos = CHARINDEX(';', @biostatusComment, @pos)
	end
	
	if (LEN(@biome) > 0)
	begin
		set @biome = SUBSTRING(@biome, 0, len(@biome))
	end
	
	return @biome
end
		
go

grant execute on dbo.fnGetBiome to dbi_user

go
