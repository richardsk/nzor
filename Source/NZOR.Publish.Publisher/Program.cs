using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using NZOR.Publish.Model.Names;
using NZOR.Publish.Publisher.Builders;
using System.IO;
using System.Xml.Serialization;
using NZOR.Publish.Model.Common;
using NZOR.Publish.Publisher.SearchIndexes;
using System.Diagnostics;
using System.Configuration;
using NZOR.Publish.Model.Administration;

namespace NZOR.Publish.Publisher
{
    class Program
    {
        static void Main(string[] args)
        {
            string dataConnectionString = ConfigurationManager.ConnectionStrings["NZOR"].ConnectionString;
            string outputFullFolderName = ConfigurationManager.AppSettings["OutputFullFolderName"];

            IndexBuilder indexBuilder = new IndexBuilder(dataConnectionString, outputFullFolderName);
            Publisher publisher = new Publisher(indexBuilder, true);

            publisher.GenerateIndexes();

            Console.ReadKey();
        }

    }
}
