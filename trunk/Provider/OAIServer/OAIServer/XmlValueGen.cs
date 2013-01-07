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
        public bool HasDynamicData = false;

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

        public bool HasIndexingPoint(String path)
        {
            MappedTable mt = _rep.GetMappedTableByPath(path);
            return (mt != null);
        }

        public ValueGenResult GetValue(int recordIndex, String path)
        {
            ValueGenResult vgr = new ValueGenResult();

            MetadataFormatSet set = _mapping.GetMappedSet(path);
            
            List<SchemaMapping> sms = _mapping.GetMappings(path);
            if (sms != null && set != null && sms.Count > 0)
            {
                foreach (SchemaMapping sm in sms)
                {
                    Object val = null;
                    vgr.MoreData |= GetFieldValue(set.Name, sm.Field, recordIndex, ref val);

                    if (IsDynamicValueField(set.Name, sm.Field) && val != null && val.ToString().Length > 0) vgr.HasDynamicData = true;
                        
                    vgr.AddValue(val, GetFixedAttrValue(set.Name, sm.Field));
                }
            }
            else
            {
                List<SchemaMapping> gms = _mapping.GetGlobalMappings(path);

                if (gms == null || gms.Count == 0)
                {
                    vgr = null; //not found
                }
                else
                {
                    foreach (SchemaMapping sm in gms)
                    {
                        Object val = OAIServer.GetFixedFieldValue(_rep.Name, sm.Field);

                        if (val != null && val.ToString().Length > 0) vgr.HasDynamicData = true;
                        vgr.AddValue(val, "");
                    }
                }
            }

            return vgr;
        }

        public bool IsDynamicValueField(string setName, string field)
        {
            bool isDyn = false;

            DataConnection dc = _rep.GetDataConnection(setName);
            if (dc != null)
            {
                FieldMapping fm = dc.GetMapping(field);

                if (fm != null && fm.GetType() != typeof(FixedValueMapping))
                {
                    isDyn = true;
                }
            }
            return isDyn;
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

                    bool found = false;
                    int pos = 0;
                    foreach (DataRow r in _data.Tables[set].Rows)
                    {
                        if (r[idField.ColumnOrAlias].ToString().ToLower() == _recordId.ToLower())
                        {
                            if (!found)
                            {
                                if (pos == recordIndex)
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
                                        if (r[col].GetType() == typeof(bool)) value = value.ToString().ToLower(); //lower case bool values for xml
                                    }
                                    found = true;
                                }
                                pos++;
                            }
                            else
                            {
                                //any more records with same id and diff values for this field?
                                object diffVal = "";
                                if (col.DataType == typeof(DateTime))
                                {
                                    diffVal = "";
                                    if (r[col] != DBNull.Value)
                                    {
                                        DateTime dt = (DateTime)r[col];
                                        diffVal = dt.ToString("s");
                                    }
                                }
                                else
                                {
                                    diffVal = r[col].ToString();
                                }
                                if (diffVal != null && diffVal.ToString() != "" && diffVal.ToString() != value.ToString())
                                {
                                    more = true;
                                    break;
                                }
                            }
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
