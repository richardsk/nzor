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

        public override string ToString()
        {
            if (Value != null) return Value.ToString();
            return "";
        }
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

    public class XmlValueGen
    {
        private MetadataFormat _mdFormat = null;
        private DataSet _data = null;
        private String _recordId = "";
        private RepositoryConfig _rep = null;
        private MetadataFormatMapping _mapping = null;

        public XmlValueGen(RepositoryConfig rep, MetadataFormat mf, MetadataFormatMapping mapping, DataSet data, String recordId)
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

            MetadataFormatSet set = _mapping.GetMappedSet(path);
            if (set == null) return vgr;

            List<SchemaMapping> sms = _mapping.GetMappings(path);
            if (sms != null)
            {
                foreach (SchemaMapping sm in sms)
                {
                    Object val = null;
                    vgr.MoreData |= GetFieldValue(set.Name, sm.Field, recordIndex, ref val);
                    
                    vgr.AddValue(val, GetFixedAttrValue(set.Name, sm.Field));                    
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

        protected bool GetFieldValue(String set, String field, int recordIndex, ref Object value)
        {
            bool more = false;

            if (_data == null || _data.Tables.Count == 0) return false;

            DataConnection dc = _rep.GetDataConnection(set);
            if (dc == null) return false;

            FieldMapping fm = dc.GetMapping(field);
            if (fm == null) return false;

            if (fm.GetType() == typeof(FixedValueMapping))
            {
                FixedValueMapping fxm = (FixedValueMapping)fm;
                value = fxm.GetValue(dc);
                return false;
            }
            
            if (_data.Tables[set] == null || _data.Tables[set].Rows.Count == 0) return false;

            DatabaseMapping dm = (DatabaseMapping)fm;
            if (dm == null) return false;

            DatabaseMapping idField = (DatabaseMapping)dc.GetMapping(FieldMapping.IDENTIFIER);
            DataColumn col = _data.Tables[set].Columns[dm.ColumnOrAlias];
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
                            if (col.DataType == typeof(DateTime))
                            {
                                value = "";
                                if (r[col] != DBNull.Value)
                                {
                                    DateTime dt = (DateTime)r[col];
                                    value = dt.ToString("s");
                                }
                            }
                            else
                            {
                                value = r[col].ToString();
                            }
                            break;
                        }
                    }
                }
                else
                {
                    if (recordIndex < _data.Tables[set].Rows.Count)
                    {
                        if (col.DataType == typeof(DateTime))
                        {
                            value = "";
                            if (_data.Tables[set].Rows[recordIndex][col] != DBNull.Value)
                            {
                                DateTime dt = (DateTime)_data.Tables[set].Rows[recordIndex][col];
                                value = dt.ToString("s");
                            }
                        }
                        else
                        {
                            value = _data.Tables[set].Rows[recordIndex][col].ToString();
                        }
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
