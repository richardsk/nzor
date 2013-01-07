using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NZOR.Integration.Mapping
{
    public enum IntegrationMappingType
    {
        Simple,
        Structured
    }

    public class IntegrationMapping
    {
        public Boolean HasColumnHeaders = true;
        public List<ColumnMapping> ColumnMappings = new List<ColumnMapping>();
        public Guid NameClassID = Guid.Empty;
        public IntegrationMappingType MappingType = IntegrationMappingType.Structured;

        public void AddMapping(ColumnMapping mapping)
        {
            ColumnMappings.Add(mapping);
        }

        public ColumnMapping GetMapping(string sourceColumn)
        {
            ColumnMapping cm = null;

            foreach (ColumnMapping mapping in ColumnMappings)
            {
                if (mapping.SourceColumn == sourceColumn)
                {
                    cm = mapping;
                    break;
                }
            }

            return cm;
        }

        public ColumnMapping GetMappingByDestination(string destinationField)
        {
            ColumnMapping cm = null;

            foreach (ColumnMapping mapping in ColumnMappings)
            {
                if (mapping.DestinationField.Field == destinationField)
                {
                    cm = mapping;
                    break;
                }
            }

            return cm;
        }
    }
}
