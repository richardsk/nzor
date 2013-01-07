-- update name properties that are pointing to this name

update np
set Value = n.FullName
from consensus.NameProperty np
inner join consensus.Name n on n.NameID = np.RelatedID
where n.NameID = @nameId

