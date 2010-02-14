using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml;

namespace OAIServer
{
    public class OAIServer
    {
        public static List<RepositoryConfig> ConfigList = new List<RepositoryConfig>();

        public static void Load(String configDir)
        {
            ConfigList.Clear();

            String[] files = System.IO.Directory.GetFiles(configDir, "*.xml");
            foreach (String f in files)
            {
                RepositoryConfig rc = LoadConfig(f);
                if (rc != null) ConfigList.Add(rc);
            }
        }

        private static RepositoryConfig LoadConfig(String filename)
        {
            RepositoryConfig rc = null;
            
            try
            {
                rc = new RepositoryConfig();
                rc.Load(filename);
            }
            catch(Exception ex)
            {
                Log.LogError(ex);
            }

            return rc;
        }

        public static RepositoryConfig GetConfig(String repository)
        {
            RepositoryConfig rep = null;

            foreach (RepositoryConfig r in ConfigList)
            {
                if (r.Name == repository)
                {
                    rep = r;
                    break;
                }
            }

            return rep;
        }

        public static Object GetFixedFieldValue(String repository, string field)
        {
            Object val = null;

            RepositoryConfig rep = GetConfig(repository);
            val = rep.GetFieldValue(field);

            return val;
        }

        public static Object GetFieldValue(String recordId, String field)
        {
            Object val = null;

            return val;
        }

    }
}
