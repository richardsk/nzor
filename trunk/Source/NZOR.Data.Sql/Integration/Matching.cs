using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

using NZOR.Data.Entities.Common;

namespace NZOR.Data.Sql
{
    public class Matching
    {
        /// <summary>
        /// Get exact name matches on partial name
        /// </summary>
        /// <param name="cnnStr"></param>
        /// <param name="parsedName"></param>
        /// <returns></returns>
        public static DataTable GetPartialNameMatches(String cnnStr, String namePart)
        {
            DataTable dt = null;

            using (SqlConnection cnn = new SqlConnection(cnnStr))
            {
                cnn.Open();

                string sql = @"select n.NameID, np.Value as NamePart, n.GoverningCode 
                            from consensus.Name n
                            inner join consensus.NameProperty np on np.NameID = n.NameID and np.NamePropertyTypeID = '00806321-C8BD-4518-9539-1286DA02CA7D'
                            where lower(np.Value) = '" + namePart.Replace("'","''") + "'";

                using (SqlCommand cmd = cnn.CreateCommand())
                {
                    cmd.CommandText = sql;

                    DataSet ds = new DataSet();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(ds);

                    if (ds != null && ds.Tables.Count > 0)
                    {
                        dt = ds.Tables[0];
                    }
                }
            }

            return dt;
        }

        public static DataTable GetStackedNameMatches(String cnnStr, NameParseResult parsedName, bool wildcardMatch, bool useLevenshtein)
        {
            DataTable dt = null;
                        
            using (SqlConnection cnn = new SqlConnection(cnnStr))
            {
                cnn.Open();

                string sql = "select p1.seednameid as NameID, n.FullName";
                string fromSql = @" from consensus.StackedName p1 
                                inner join consensus.Name n on n.NameID = p1.SeedNameID ";

                string whereSql = " where ";

                if (parsedName.GetAuthors() != null && parsedName.GetAuthors() != "")
                {
                    sql += ", np.Value as Authors";
                    fromSql += " left join consensus.NameProperty np on np.NameID = n.NameID and np.NamePropertyTypeID = '006D86A8-08A5-4C1A-BC08-C07B0225E01B' ";
                    if (wildcardMatch)
                    {
                        whereSql += " np.Value like '%" + parsedName.GetAuthors().Replace("(", "").Replace(")", "") + "%' and ";
                    }
                    else
                    {
                        whereSql += " difference(np.Value, '" + parsedName.GetAuthors().Replace("(", "").Replace(")", "") + "') = 4 and ";
                    }
                }

                if (parsedName.Year != null && parsedName.Year != "")
                {
                    fromSql += " left join consensus.NameProperty ynp on ynp.NameID = n.NameID and ynp.NamePropertyTypeID = 'C9E19D58-BD92-4F3C-96E8-DE7FA6DCD5F7' ";
                    if (wildcardMatch)
                    {
                        whereSql += " (ynp.Value is null or ynp.Value like '%" + parsedName.Year + "%') and ";
                    }
                    else
                    {
                        whereSql += " (ynp.Value is null or ynp.Value = '" + parsedName.Year + "') and ";
                    }
                }

                int lastPos = 1;
                int pos = 1;
                foreach (NamePart np in parsedName.NameParts)
                {
                    if (np.Text != "sp." && np.Text != "spp.")
                    {
                        if (pos != 1)
                        {
                            fromSql += "inner join consensus.StackedName p" + pos.ToString() + " on p1.SeedNameID = p" + pos.ToString() + ".SeedNameID ";
                            whereSql += " and ";
                        }

                        if (pos == parsedName.NameParts.Count)
                        {
                            //use soundex/levenshtein for the canonical
                            if (useLevenshtein)
                            {
                                if (np.Rank.Length > 0) whereSql += "p" + pos.ToString() + ".RankName = '" + np.Rank + "' and ";
                                whereSql += "dbo.fnLevenshteinDistance(p" + pos.ToString() + ".CanonicalName, '" + np.Text +
                                     "') <= 1 and p" + pos.ToString() + ".Depth = " + (parsedName.NameParts.Count - pos).ToString();
                            }
                            else
                            {
                                if (np.Rank.Length > 0) whereSql += "p" + pos.ToString() + ".RankName = '" + np.Rank + "' and ";
                                whereSql += "replace(lower(p" + pos.ToString() + ".CanonicalName),'-','')";
                                if (wildcardMatch)
                                {
                                    whereSql += " like '%" + np.Text.ToLower() + "%'";
                                }
                                else
                                {
                                    whereSql += " = '" + np.Text.ToLower() + "'";
                                }
                                if (pos > 1) whereSql += " and p" + pos.ToString() + ".Depth < p" + lastPos.ToString() + ".Depth"; //= " + (parsedName.NameParts.Count - pos).ToString();
                            }
                        }
                        else
                        {
                            if (np.Rank.Length > 0) whereSql += "p" + pos.ToString() + ".RankName = '" + np.Rank + "' and ";
                            whereSql += "replace(p" + pos.ToString() + ".CanonicalName,'-','')";
                            if (wildcardMatch)
                            {
                                whereSql += " like '%" + np.Text.ToLower() + "%'";
                            }
                            else
                            {
                                whereSql += " = '" + np.Text.ToLower() + "'";
                            }
                            if (pos > 1) whereSql += " and p" + pos.ToString() + ".Depth < p" + lastPos.ToString() + ".Depth"; //= " + (parsedName.NameParts.Count - pos).ToString();
                        }

                        lastPos = pos;
                    }

                    pos += 1;
                }

                if (!whereSql.EndsWith("and ")) whereSql += " and ";
                whereSql += "p" + lastPos.ToString() + ".depth = 0";

                sql += fromSql + whereSql;

                using (SqlCommand cmd = cnn.CreateCommand())
                {
                    cmd.CommandText = sql;

                    DataSet ds = new DataSet();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(ds);

                    if (ds != null && ds.Tables.Count > 0)
                    {
                        dt = ds.Tables[0];
                        //check authors havent screwed up
                        if (dt.Rows.Count > 1 && dt.Columns.Contains("Authors"))
                        {
                            bool hasExactMatch = false;
                            foreach (DataRow row in dt.Rows)
                            {
                                if (row["Authors"].ToString().ToLower() == parsedName.GetAuthors().ToLower())
                                {
                                    hasExactMatch = true;
                                    break;
                                }
                            }

                            //remove non-exact matches
                            if (hasExactMatch)
                            {
                                for (int i = dt.Rows.Count - 1; i >= 0; i--)
                                {
                                    if (dt.Rows[i]["Authors"].ToString().ToLower() != parsedName.GetAuthors().ToLower()) dt.Rows[i].Delete();
                                }
                                dt.AcceptChanges();
                            }
                        }

                    }
                }
            }

            return dt;
        }
    }
}
