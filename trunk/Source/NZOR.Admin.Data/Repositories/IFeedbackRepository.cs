using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NZOR.Admin.Data.Entities;

namespace NZOR.Admin.Data.Repositories
{
    public interface IFeedbackRepository
    {
        List<Feedback> Feedbacks { get; }

        List<Feedback> GetFeedbackWithStatus(FeedbackStatus status);

        void Save();
    }
}
