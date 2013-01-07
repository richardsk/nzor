using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OAIServer
{
    public class Filter
    {
        private String _sql = "";

        public String SQL
        {
            get
            {
                return _sql;
            }
            set
            {
                _sql = value;
            }
        }

    }
}
