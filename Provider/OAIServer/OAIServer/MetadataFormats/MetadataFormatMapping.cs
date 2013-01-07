﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace OAIServer
{
    public class MetadataFormatMapping
    {
        public List<MetadataFormatSet> Sets = new List<MetadataFormatSet>();
        public List<SchemaMapping> SchemaMappings = new List<SchemaMapping>();
        public List<SchemaMapping> GlobalMappings = new List<SchemaMapping>();

        public void Load(XmlNode node)
        {
            XmlNodeList l = node.SelectNodes("Sets/Set");
            foreach (XmlNode n in l)
            {
                MetadataFormatSet ms = new MetadataFormatSet();
                ms.Load(n);
                Sets.Add(ms);
            }

            l = node.SelectNodes("SchemaMappings/SchemaMapping");
            foreach (XmlNode n in l)
            {
                SchemaMapping sm = new SchemaMapping();
                sm.Load(n);

                if (sm.Set == "")
                {
                    GlobalMappings.Add(sm);
                }
                else
                {
                    SchemaMappings.Add(sm);
                }
            }

        }

        public List<SchemaMapping> GetMappings(String xpath)
        {
            List<SchemaMapping> sm = new List<SchemaMapping>();

            foreach (SchemaMapping s in SchemaMappings)
            {
                if (s.XPath == xpath)
                {
                    sm.Add(s);
                }
            }

            return sm;
        }

        public List<SchemaMapping> GetGlobalMappings(string xpath)
        {
            List<SchemaMapping> sm = new List<SchemaMapping>();

            foreach (SchemaMapping s in GlobalMappings)
            {
                if (s.XPath == xpath)
                {
                    sm.Add(s);
                }
            }

            return sm;
        }


        public MetadataFormatSet GetMappedSet(String xpath)
        {
            //return set that has the xpath within its indexing element
            MetadataFormatSet mfs = null;

            foreach (MetadataFormatSet m in Sets)
            {
                if (xpath.StartsWith(m.IndexingElement))
                {
                    mfs = m;
                    break;
                }
            }

            return mfs;
        }
    }
}
