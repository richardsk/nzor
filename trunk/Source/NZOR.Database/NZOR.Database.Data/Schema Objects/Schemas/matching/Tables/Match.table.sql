CREATE TABLE 
	matching.Match
	(
	MatchId UNIQUEIDENTIFIER NOT NULL,
	 
	SubmitterEmail NVARCHAR(200) NOT NULL,
	ReceivedDate DATETIME NOT NULL,
	Status NVARCHAR(50) NOT NULL,
	InputData NVARCHAR(MAX) NOT NULL,
	ResultData NVARCHAR(MAX) NOT NULL,
	ExternalLookupResults NVARCHAR(MAX) NULL,
	IsServiceMediated bit null,
	ApiKey nvarchar(250) null,
	Error nvarchar(max) null,
	DoExternalLookup bit null
	)
