select FeebackId,
	NameId,
	[Message],
	[Status],
	Resolution,
	Sender,
	SenderEmail,
	SentTo,
	AddedDate,
	ModifiedDate
where [Status] = @status
