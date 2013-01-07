using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;

namespace NZOR.Publish.Data.Helpers
{
    static class DataSourceHelper
    {
        public static List<T> DeserializeDataSource<T>(string dataSourceFileFullName)
        {
            List<T> items = new List<T>();

            string xml = System.IO.File.ReadAllText(dataSourceFileFullName);

            using (StreamReader reader = new StreamReader(dataSourceFileFullName))
            {
                var serializer = new XmlSerializer(typeof(List<T>));

                items = serializer.Deserialize(reader) as List<T>;
            }

            return items;
        }
    }
}
