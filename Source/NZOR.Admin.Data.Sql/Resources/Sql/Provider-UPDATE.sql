update [admin].Provider
set Name = @name,
	Code = @code,
	Url = @url,
	ContactEmail = @contactEmail,
	Disclaimer = @disclaimer,
	Attribution = @attribution,
	Licensing = @licensing,
	PublicStatement = @publicStatement,
	AddedDate = @addedDate,
	AddedBy = @addedBy,
	ModifiedDate = @modifiedDate,
	ModifiedBy = @modifiedBy
where ProviderId = @providerId