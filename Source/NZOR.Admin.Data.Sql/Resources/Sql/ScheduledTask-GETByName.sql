select ScheduledTaskId,
	RelatedId,
	Name,
	FrequencyDays,
	PreferredStartTimeGMT,
	LastRun,
	LastRunOutcome,
	[Status]
from [admin].ScheduledTask
where Name = @name