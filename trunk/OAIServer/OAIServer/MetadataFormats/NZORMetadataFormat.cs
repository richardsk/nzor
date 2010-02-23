using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml;
using System.Data;

namespace OAIServer
{
    class NZORMetadataFormat : MetadataFormat
    {
        //these need to match Set names in config and OAI requests
        public static String NZORMetadataObject = "Metadata";
        public static String NZORNameObject = "TaxonName";
        public static String NZORNamePropertyObject = "TaxonProperty";
        public static String NZORConceptObject = "TaxonConcept";
        public static String NZORPublicationObject = "Publication";

        public override String ProcessResults(System.Data.DataSet results, RepositoryConfig rep, String id)
        {
            return base.ProcessResults(results, rep, id);


            //Alterntive hard-coded method ...

            String xml = "";

            this._rep = rep;
            this._results = results;

            XmlDocument doc = new XmlDocument();
            XmlNode root = doc.AppendChild(doc.CreateElement("DataSet"));

            if (_results.Tables[NZORNameObject] != null && _results.Tables[NZORNameObject].Rows.Count > 0)
            {
                XmlNode namesNode = root.AppendChild(doc.CreateElement("TaxonNames"));

                foreach (DataRow nameRow in _results.Tables[NZORNameObject].Rows)
                {
                    XmlNode nameNode = namesNode.AppendChild(doc.CreateElement("TaxonName"));

                    //nameNode.AppendChild(doc.CreateElement("NameFull")).InnerText = GetFieldValue(NZORNameObject, "");
                }
            }


            return xml;
        }
    }
}
