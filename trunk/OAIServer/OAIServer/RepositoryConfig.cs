using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace OAIServer
{
    public class RepositoryConfig
    {
        public String Name = "";
        public String AdminEmail = "";
        public String ProviderId = "";
        public String ProviderName = "";
        public String OrganisationUrl = "";
        public String MetadataDate = "";
        public String Disclaimer = "";
        public String Attribution = "";
        public String Licensing = "";
        public String DataSubsetId = "";

        public List<MetadataFormat> MetadataFormats = new List<MetadataFormat>();
        public List<String> Sets = new List<String>();
        public List<DataConnection> DataConnections = new List<DataConnection>();
        

        public void Load(String fileName)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(fileName);

            XmlNode n = doc.SelectSingleNode("//Services/Service");
            this.Name = n.Attributes["name"].InnerText;

            n = doc.SelectSingleNode("//Services/Service/AdminEmail");
            this.AdminEmail = n.InnerText;

            n = doc.SelectSingleNode("//Services/Service/ProviderId");
            if (n != null) this.ProviderId = n.InnerText;

            n = doc.SelectSingleNode("//Services/Service/ProviderName");
            if (n != null) this.ProviderName = n.InnerText;

            n = doc.SelectSingleNode("//Services/Service/OrganisationUrl");
            if (n != null) this.OrganisationUrl = n.InnerText;

            n = doc.SelectSingleNode("//Services/Service/MetadataDate");
            if (n != null) this.MetadataDate = n.InnerText;

            n = doc.SelectSingleNode("//Services/Service/Disclaimer");
            if (n != null) this.Disclaimer = n.InnerText;

            n = doc.SelectSingleNode("//Services/Service/Attribution");
            if (n != null) this.Attribution = n.InnerText;

            n = doc.SelectSingleNode("//Services/Service/Licensing");
            if (n != null) this.Licensing = n.InnerText;

            n = doc.SelectSingleNode("//Services/Service/DataSubsetId");
            if (n != null) this.DataSubsetId = n.InnerText;

            MetadataFormats.Clear();
            XmlNodeList ml = doc.SelectNodes("//Services/Service/MetadataFormats/MetadataFormat");

            foreach (XmlNode mn in ml)
            {
                MetadataFormat mf = new MetadataFormat();
                mf.Load(mn);

                MetadataFormats.Add(mf);
            }

            Sets.Clear();
            XmlNodeList nl = doc.SelectNodes("//Services/Service/Sets/Set");

            foreach (XmlNode sn in nl)
            {
                Sets.Add(sn.Attributes["name"].InnerText);
            }

            XmlNodeList dcl = doc.SelectNodes("//Services/Service/DataConnections/DataConnection");
            foreach (XmlNode dcn in dcl)
            {
                DataConnection dc = new DataConnection();
                dc.Load(dcn);
                dc.Repository = this;
                DataConnections.Add(dc);
            }
    
        }

        public MappedTable GetMappedTableByPath(String path)
        {
            MappedTable tbl = null;

            foreach (DataConnection dc in DataConnections)
            {
                MappedTable mt = dc.GetMappedTableByPath(path);
                if (mt != null)
                {
                    tbl = mt;
                    break;
                }
            }

            return tbl;
        }

        public DataConnection GetDataConnection(String set)
        {
            DataConnection dc = null;

            foreach (DataConnection d in DataConnections)
            {
                if (d.Set == set)
                {
                    dc = d;
                    break;
                }
            }

            return dc;
        }

        public MetadataFormat GetMetadataFormat(String prefix)
        {
            MetadataFormat mf = null;

            foreach (MetadataFormat m in MetadataFormats)
            {
                if (m.Prefix == prefix)
                {
                    mf = m;
                    break;
                }
            }

            return mf;
        }
    }
}
