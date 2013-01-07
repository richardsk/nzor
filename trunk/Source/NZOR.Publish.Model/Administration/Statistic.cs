using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NZOR.Publish.Model.Administration
{
    public class Statistic
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public DateTime Date { get; set; }
        public int Value { get; set; }

        public Statistic()
        {
            Name = String.Empty;
            DisplayName = String.Empty;
        }
    }
}
