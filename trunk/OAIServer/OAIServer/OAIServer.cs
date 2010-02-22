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

            if (field == FieldMapping.ADMIN_EMAIL)
            {
                val = rep.AdminEmail;
            }
            else if (field == FieldMapping.REPOSITORY_NAME)
            {
                val = repository;
            }
            else
            {
                foreach (DataConnection dc in rep.DataConnections)
                {
                    foreach (FieldMapping fm in dc.Mappings)
                    {
                        if (fm.GetType() == typeof(FixedValueMapping))
                        {
                            FixedValueMapping fvm = (FixedValueMapping)fm;
                            if (fvm.Field == field)
                            {
                                val = fvm.Value;
                                break;
                            }
                        }
                        else if (fm.GetType() == typeof(SQLMaxValueMapping) || fm.GetType() == typeof(SQLMinValueMapping))
                        {
                            DatabaseMapping dbm = (DatabaseMapping)fm;
                            if (dbm.Field == field)
                            {
                                val = dbm.GetValue(dc);
                                break;
                            }
                        }
                    }

                    if (val != null) break;
                }
            }

            return val;
        }

        public static Object GetFieldValue(String recordId, String field)
        {
            Object val = null;

            return val;
        }

    }
}
