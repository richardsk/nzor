ALTER TABLE [admin].Statistic
	ADD CONSTRAINT [frkStatisticStatisticType] 
	FOREIGN KEY (StatisticTypeId)
	REFERENCES [admin].StatisticType (StatisticTypeId)	

