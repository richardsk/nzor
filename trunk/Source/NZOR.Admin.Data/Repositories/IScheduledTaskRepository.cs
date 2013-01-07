using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NZOR.Admin.Data.Entities;

namespace NZOR.Admin.Data.Repositories
{
    public interface IScheduledTaskRepository
    {
        List<ScheduledTask> ScheduledTasks { get; }
        ScheduledTask GetScheduledTask(Guid relatedId);
        ScheduledTask GetScheduledTask(String name);
        List<ScheduledTask> ListScheduledTasks();
        void Save();
    }
}
