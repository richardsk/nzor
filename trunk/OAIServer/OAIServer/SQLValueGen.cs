using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace OAIServer
{
    public class ValueGenResult
    {
        public Object Value = null;
        public bool MoreData = false;
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

            SchemaMapping sm = _mapping.GetMapping(path);
            if (sm != null)
            {
                Object val = null;
                vgr.MoreData = GetFieldValue(sm.Set, sm.Field, recordIndex, ref val);
                vgr.Value = val;
            }

            return vgr;
        }

        protected bool GetFieldValue(String set, String dbField, int recordIndex, ref Object value)
        {
            bool more = false;

            if (_data == null || _data.Tables.Count == 0) return false;

            if (_data.Tables[set] == null || _data.Tables[set].Rows.Count == 0) return false;

            DatabaseMapping fm = (DatabaseMapping)_rep.GetDataConnection(set).GetMapping(dbField);
            DatabaseMapping idField = (DatabaseMapping)_rep.GetDataConnection(set).GetMapping(FieldMapping.IDENTIFIER);
            DataColumn col = _data.Tables[set].Columns[fm.ColumnOrAlias];
            if (col != null)
            {
                if (_recordId != null && _recordId.Length > 0)
                {
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
