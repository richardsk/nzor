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