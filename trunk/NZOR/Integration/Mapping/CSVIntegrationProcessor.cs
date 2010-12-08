using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace NZOR.Integration.Mapping
{
    public class CSVIntegrationProcessor
    {
        public Data.DsIntegrationName GetDataForIntegration(String csvFileName, IntegrationMapping mapping)
        {
            Data.DsIntegrationName data = new Data.DsIntegrationName();

            StreamReader rdr = File.OpenText(csvFileName);

            while (rdr.ReadLine()

            return data;
        }
    }
}
