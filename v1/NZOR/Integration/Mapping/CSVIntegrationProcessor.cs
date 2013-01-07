using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data.SqlClient;

using LumenWorks.Framework.IO.Csv;

namespace NZOR.Integration.Mapping
{
    public class CSVIntegrationProcessor
    {
        public Data.DsIntegrationName GetDataForIntegration(SqlConnection cnn, String csvFileName, IntegrationMapping mapping)
        {
            Data.DsIntegrationName data = new Data.DsIntegrationName();

            //get consensus names
            Data.Integration.GetConsensusDataForIntegration(cnn, ref data);

            //provider names from csv file
            using (CsvReader csv = new CsvReader(new StreamReader(csvFileName), true))
            {
                int fieldCount = csv.FieldCount;
                string[] headers = csv.GetFieldHeaders();

                if (fieldCount != mapping.ColumnMappings.Count) throw new Exception("CSV column header count does not match mapped field count");

                while (csv.ReadNextRecord())
                {                    
                    //MUST be for provider table??
                    Data.DsIntegrationName.ProviderNameRow newRow = data.ProviderName.NewProviderNameRow();

                    foreach (String hdr in headers)
                    {
                        ColumnMapping cm = mapping.GetMapping(hdr);
                        if (cm == null) throw new Exception("Column mapping not found for CSV column '" + hdr + "'");

                        newRow[cm.DestinationColumn] = csv[hdr];
                    }

                    data.ProviderName.AddProviderNameRow(newRow);
                }
            }

            return data;
        }
    }
}
