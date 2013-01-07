select *
from consensus.StackedName
where SeedNameID = @nameId
order by Depth desc