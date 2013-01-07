﻿SELECT 
	ProviderID, 
	Name,
	Code,
	Url,
	ContactEmail,
	Disclaimer,
	Attribution,
	Licensing,
	PublicStatement,
	AddedDate,
	AddedBy,
	ModifiedDate,
	ModifiedBy
FROM 
	[admin].Provider
WHERE Code = @code