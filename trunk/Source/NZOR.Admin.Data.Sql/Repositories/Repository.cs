using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data.Common;

namespace NZOR.Admin.Data.Sql.Repositories
{
    public abstract class Repository<TEntity>
    {
        protected String _connectionString;

        public Repository(String connectionString)
        {
            _connectionString = connectionString;
        }
    }

}
