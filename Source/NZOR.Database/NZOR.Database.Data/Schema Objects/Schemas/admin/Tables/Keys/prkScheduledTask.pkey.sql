﻿ALTER TABLE [admin].[ScheduledTask]
    ADD CONSTRAINT [prkScheduleTask] PRIMARY KEY CLUSTERED ([ScheduledTaskID] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF);

