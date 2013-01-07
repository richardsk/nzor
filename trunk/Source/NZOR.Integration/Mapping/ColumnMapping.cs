using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NZOR.Integration.Mapping
{
    public class ColumnMapping
    {
        public enum ColumnMapType
        {
            Column,
            StaticText
        }

        public String SourceColumn { get; set; }
        public int SourceColumnIndex { get; set; }
        public ColumnMapType MapType { get; set; }
        public NZORIntegrationField DestinationField { get; set; }
        public String StaticTextValue { get; set; }

        public ColumnMapping()
        {
        }
        
        public ColumnMapping(int sourceColIndex, String sourceCol, ColumnMapType mapType, NZORIntegrationField destinationField, String staticTextVal)
        {
            SourceColumn = sourceCol;
            SourceColumnIndex = sourceColIndex;
            MapType = mapType;
            DestinationField = destinationField;
            StaticTextValue = staticTextVal;
        }
    }
}
