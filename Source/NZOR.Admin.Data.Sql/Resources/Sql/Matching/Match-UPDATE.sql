UPDATE
	matching.Match
SET 
	SubmitterEmail = @SubmitterEmail,
    ReceivedDate = @ReceivedDate,
    Status = @Status,
    InputData = @InputData,
    ResultData = @ResultData,
	ExternalLookupResults = @ExternalLookupResults,
	IsServiceMediated = @IsServiceMediated,
	ApiKey = @ApiKey,
	Error = @Error,
	DoExternalLookup = @DoExternalLookup
WHERE 
	MatchId = @MatchId