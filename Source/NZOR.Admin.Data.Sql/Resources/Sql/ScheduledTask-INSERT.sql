insert [admin].ScheduledTask
select @scheduledTaskId, @name, @relatedId, @frequencyDays, @preferredStartTimeGMT, @lastRun, @lastRunOutcome, @status
