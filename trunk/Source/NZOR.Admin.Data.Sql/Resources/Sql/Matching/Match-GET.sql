if (@includeData = 0)
begin
	SELECT 
		MatchId,

		SubmitterEmail,
		ReceivedDate,
		Status,
		IsServiceMediated,
		ApiKey,
		Error,
		DoExternalLookup
	FROM 
		matching.Match
	WHERE 
		MatchId = @matchId
end
else
begin
	SELECT 
		MatchId,

		SubmitterEmail,
		ReceivedDate,
		Status,
		InputData,
		ResultData,
		ExternalLookupResults,
		IsServiceMediated,
		ApiKey,
		Error,
		DoExternalLookup
	FROM 
		matching.Match
	WHERE 
		MatchId = @matchId
end
