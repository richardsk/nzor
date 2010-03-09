using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace OAIServer
{
    public class GenValue
    {
        public Object Value = null;
        public String FixedAttrValue = "";
    }

    public class ValueGenResult
    {
        public List<GenValue> Values = new List<GenValue>();
        public bool MoreData = false;

        public void AddValue(Object val, String attrVals)
        {
            GenValue gv = new GenValue();
            gv.Value = val;
            gv.FixedAttrValue = attrVals;
            Values.Add(gv);
        }
    }

    public class SQLValueGen
    {
        private MetadataFormat _mdFormat = null;
        private DataSet _data = null;
        private String _recordId = "";
        private RepositoryConfig _rep = null;
        private MetadataFormatMapping _mapping = null;

        public SQLValueGen(RepositoryConfig rep, MetadataFormat mf, MetadataFormatMapping mapping, DataSet data, String recordId)
        {
            _rep = rep;
            _mdFormat = mf;
            _mapping = mapping;
            _data = data;
            _recordId = recordId;
        }

        public ValueGenResult GetValue(int recordIndex, String path)
        {
            ValueGenResult vgr = new ValueGenResult();

            List<SchemaMapping> sm = _mapping.GetMappings(path);
            if (sm != null)
            {
                foreach (SchemaMapping s in sm)
                {
                    Object val = null;
                    vgr.MoreData |= GetFieldValue(s.Set, s.Field, recordIndex, ref val);
                    
                    vgr.AddValue(val, GetFixedAttrValue(s.Set, s.Field));                    
                }
            }

            return vgr;
        }

        private String GetFixedAttrValue(String set, String field)
        {
            String val = "";
            DataConnection dc = _rep.GetDataConnection(set);
            if (dc != null)
            {
                FieldMapping fm = dc.GetMapping(field);
                if (fm != null)
                {
                    val = fm.Fixedattributes;
                }
            }

            return val;
        }

        protected bool GetFieldValue(String set, String dbField, int recordIndex, ref Object value)
        {
            bool more = false;

            if (_data == null || _data.Tables.Count == 0) return false;

            if (_data.Tables[set] == null || _data.Tables[set].Rows.Count == 0) return false;

            DatabaseMapping fm = (DatabaseMapping)_rep.GetDataConnection(set).GetMapping(dbField);
            if (fm == null) return false;

            DatabaseMapping idField = (DatabaseMapping)_rep.GetDataConnection(set).GetMapping(FieldMapping.IDENTIFIER);
            DataColumn col = _data.Tables[set].Columns[fm.ColumnOrAlias];
            if (col != null)
            {
                if (_recordId != null && _recordId.Length > 0)
                {
                    //DataRow[] recs = _data.Tables[set].Select(idField.ColumnOrAlias + "='" + _recordId + "'");

                    //if (recs != null && recs.Length > 0)
                    //{
                    //    value = recs[0][col].ToString();
                    //}

                    foreach (DataRow r in _data.Tables[set].Rows)
                    {
                        if (r[idField.ColumnOrAlias].ToString().ToLower() == _recordId.ToLower())
                        {
                            value = r[col].ToString();
                            break;
                        }
                    }
                }
                else
                {
                    if (recordIndex < _data.Tables[set].Rows.Count)
                    {
                        value = _data.Tables[set].Rows[recordIndex][col].ToString();
                        more = (_data.Tables[set].Rows.Count > (recordIndex+1));
                    }
                    else
                    {
                        more = false;
                    }

                }
            }

            return more;
        }
    }
}
