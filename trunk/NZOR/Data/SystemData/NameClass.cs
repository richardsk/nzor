using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;

namespace NZOR.Data
{
    public class NameClassProperty
    {
        public static string YearOnPublication = "YearOnPublication";
        public static string Basionym = "Basionym";
        public static string CombinationAuthors = "CombinationAuthors";
        public static string MicroReference = "MicroReference";
        public static string Canonical = "Canonical";
        public static string Rank = "Rank";
        public static string PublishedIn = "PublishedIn";
        public static string Orthography = "Orthography";
        public static string Authors = "Authors";
        public static string Year = "Year";
        public static string Country = "Country";
        public static string Language = "Language";


        public Guid ID = Guid.Empty;
        public String Name = "";
        public String PropertyType = "";
        public int MinOccurrences = -1;
        public int MaxOccurrences = -1;
        public String GoverningCode = "";

        public NameClassProperty(Guid id, String name, String propType, int minOcc, int maxOcc, String govCode)
        {
            this.ID = id;
            this.Name = name;
            this.PropertyType = propType;
            this.MinOccurrences = minOcc;
            this.MaxOccurrences = maxOcc;
            this.GoverningCode = govCode;
        }
    }

    public class NameClass
    {
    #region "Static Mambers"
        public static Dictionary<Guid, NameClass> NameClasses = null;
       
        public static NameClassProperty GetPropertyOfClassType(SqlConnection cnn, Guid nameClassID, String propertyName)
        {
            if (NameClasses == null) Load(cnn);

            NameClassProperty ncp = null;

            foreach (NameClass nc in NameClasses.Values)
            {
                if (nc.ID == nameClassID)
                {
                    ncp = nc.GetProperty(propertyName);
                    break;
                }
            }

            return ncp;
        }

        public static void Load(SqlConnection cnn)
        {
            NameClasses = new Dictionary<Guid, NameClass>();

            using (SqlCommand cmd = cnn.CreateCommand())
            {
                cmd.CommandText = "select * from dbo.NameClass; select * from dbo.NameClassProperty";

                DataSet ds = new DataSet();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds);

                foreach (DataRow nr in ds.Tables[0].Rows)
                {
                    NameClass nc = new NameClass((Guid)nr["NameClassID"], nr["Name"].ToString(), nr["Description"].ToString());
                    NameClasses.Add(nc.ID, nc);
                }
                                
                foreach (DataRow ncpr in ds.Tables[1].Rows)
                {
                    NameClassProperty ncp = new NameClassProperty((Guid)ncpr["NameClassPropertyID"], ncpr["Name"].ToString(), ncpr["Type"].ToString(),
                        (int)(ncpr.IsNull("MinOccurrences") ? -1 : (int)ncpr["MinOccurrences"]),
                        (int)(ncpr.IsNull("MaxOccurrences") ? -1 : (int)ncpr["MaxOccurrences"]),
                        ncpr["GoverningCode"].ToString());

                    NameClasses[(Guid)ncpr["NameClassID"]].Properties.Add(ncp.ID, ncp);
                }
            }
        }
            
     #endregion

        public Guid ID = Guid.Empty;
        public String Name = "";
        public String Description = "";
        public Dictionary<Guid, NameClassProperty> Properties = new Dictionary<Guid,NameClassProperty>();

        public NameClass(Guid id, String name, String desc)
        {
            this.ID = id;
            this.Name = name;
            this.Description = desc;
        }

        public NameClassProperty GetProperty(String name)
        {
            NameClassProperty ncp = null;

            foreach (NameClassProperty n in Properties.Values)
            {
                if (n.Name == name)
                {
                    ncp = n;
                    break;
                }
            }

            return ncp;
        }
    }
}
