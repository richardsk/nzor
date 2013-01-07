ALTER TABLE [admin].BrokeredName
	ADD CONSTRAINT [frkBrokeredNameNameRequest] 
	FOREIGN KEY (NameRequestId)
	REFERENCES [admin].NameRequest (NameRequestId)	

