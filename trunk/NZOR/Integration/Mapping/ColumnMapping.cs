using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NZOR.Integration.Mapping
{
    public class ColumnMapping
    {
        public int SourceColumnIndex = -1;
        public String DestinationTable = "";
        public String DestinationColumn = "";

        public ColumnMapping()
        {
        }

        public ColumnMapping(int sourceColIndex, String destTable, String destCol)
        {
            SourceColumnIndex = sourceColIndex;
            DestinationTable = destTable;
            DestinationColumn = destCol;
        }
    }
}
