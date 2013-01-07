using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NZORConsumer.Model
{
    public class NameResultList : List<NameResult>
    {
        public List<Name> Names
        {
            get
            {
                List<Name> names = new List<Name>();
                foreach (NameResult n in this)
                {
                    names.Add(n.Name);
                }
                return names;
            }
        }
    }

    public class NameResult
    {
        public Name Name = null;
        public List<Provider> Providers = new List<Provider>();        
    }
}
