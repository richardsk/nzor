update [admin].Feedback
set NameId = @nameId,
	[Message] = @message,
	[Status] = @status,
	Resolution = @resolution,
	Sender = @sender,
	SenderEmail = @senderEmail,
	SentTo = @sentTo,
	AddedDate = @addedDate,
	ModifiedDate = @modifiedDate
where FeedbackId = @feedbackId
