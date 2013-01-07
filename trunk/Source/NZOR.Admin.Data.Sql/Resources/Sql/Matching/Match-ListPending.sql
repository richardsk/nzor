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
	Status = 'Pending' or Status = 'Error'
