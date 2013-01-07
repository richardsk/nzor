using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NZOR.Admin.Data.Entities.Matching;

namespace NZOR.Admin.Data.Repositories.Matching
{
    public interface IMatchRepository
    {
        Match GetMatch(Guid id, bool includeData);

        List<Match> GetPendingMatches();

        void Save();
        void Save(Match match);
    }
}
