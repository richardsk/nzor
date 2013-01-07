using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data.Common;

namespace NZOR.Data.Sql.Repositories
{
    public abstract class Repository<TEntity> where TEntity : class
    {
        protected string _connectionString;

        public Repository(string connectionString)
        {
            _connectionString = connectionString;
        }
    }
}
