using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NZOR.Integration.Mapping
{
    public class IntegrationMapping
    {
        public Boolean HasColumnHeaders = true;
        public List<ColumnMapping> ColumnMappings = new List<ColumnMapping>();

        public void AddMapping(ColumnMapping mapping)
        {
            ColumnMappings.Add(mapping);
        }

        public ColumnMapping GetMapping(String destinationColumn)
        {
            ColumnMapping cm = null;

            foreach (ColumnMapping mapping in ColumnMappings)
            {
                if (mapping.DestinationColumn == destinationColumn)
                {
                    cm = mapping;
                    break;
                }
            }

            return cm;
        }
    }
}
