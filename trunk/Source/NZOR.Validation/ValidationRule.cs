using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace NZOR.Validation
{
    public enum EValidationObjectType
    {
        NotSpecified,
        DataTable,
        DataRow,
        Object
    }

    public enum EValidationComparisonType
    {
        NotSpecified,
        UniqueConstraint,
        NotNull,
        NotNullOrEmpty,
        Maximum,
        Minimum
    }

    public class UniqueField
    {
        public String Field;
        public object Value;

        public UniqueField(string field, object value)
        {
            this.Field = field;
            this.Value = value;
        }
    }

    public class UniqueConstraint
    {
        public List<UniqueField> UniqueFields = new List<UniqueField>();

        public UniqueConstraint(List<UniqueField> fields)
        {
            UniqueFields.AddRange(fields);
        }
    }

    public class ValidationRule
    {
        public EValidationComparisonType ComparisonType = EValidationComparisonType.NotSpecified;
        public string Column = "";
        public string Property = "";
        public int Maximum = -1;
        public int Minimum = -1;
        public UniqueConstraint Constraint = null;

        /// <summary>
        /// Validate the data that is passed in
        /// </summary>
        /// <param name="data">The datatable, datarow or object that contains the value to be tested.</param>
        /// <returns></returns>
        public ValidationResultLookup Validate(EValidationObjectType objType, object data)
        {
            ValidationResultLookup res = new ValidationResultLookup();
            object value = null;
            string field = "";

            if (objType == EValidationObjectType.DataTable)
            {
                DataTable dt = (DataTable)data;
                foreach (DataRow dr in dt.Rows)
                {
                    ValidationResultLookup vrl = Validate(EValidationObjectType.DataRow, dr);
                    res.AddRange(vrl);
                }
            }
            else
            {
                ValidationResult vr = new ValidationResult();

                //unique constrain or row validation ?
                if (ComparisonType == EValidationComparisonType.UniqueConstraint)
                {
                    //must be a datarow rule
                    if (objType == EValidationObjectType.DataRow)
                    {
                        DataRow r = (DataRow)data;
                        string sel = "";
                        foreach (UniqueField uf in Constraint.UniqueFields)
                        {
                            if (sel.Length > 0) sel += " and ";
                            sel += uf.Field + "='" + uf.Value.ToString() + "'";
                        }

                        DataRow[] rows = r.Table.Select(sel);
                        if (rows.Length > 1)                        
                        {
                            vr.Result = EValidationResult.Fail;
                            vr.Message = "Unique constraint failed: " + sel;
                        }
                        res.Add(vr);
                    }
                }
                else
                {
                    if (objType == EValidationObjectType.DataRow)
                    {
                        DataRow r = (DataRow)data;
                        value = r[Column];
                        field = Column;
                    }
                    else if (objType == EValidationObjectType.Object)
                    {
                        value = data.GetType().GetProperty(Property).GetValue(data, null);
                        field = Property;
                    }
                    else
                    {
                        throw new Exception("Unknown validation type");
                    }
                    if (ComparisonType == EValidationComparisonType.NotNull)
                    {
                        if (value == null || value == DBNull.Value)
                        {
                            vr.Result = EValidationResult.Fail;
                            vr.Message = "Required field " + field + " is null";
                        }
                    }
                    else if (ComparisonType == EValidationComparisonType.NotNullOrEmpty)
                    {
                        if (value == null || value == DBNull.Value || value.ToString() == string.Empty)
                        {
                            vr.Result = EValidationResult.Fail;
                            vr.Message = "Required field " + field + " is null or empty";
                        }
                    }
                    else if (ComparisonType == EValidationComparisonType.Maximum || ComparisonType == EValidationComparisonType.Minimum)
                    {
                        if (value != null && value != DBNull.Value)
                        {
                            double numVal = 0;
                            if (double.TryParse(value.ToString(), out numVal))
                            {
                                if ((ComparisonType == EValidationComparisonType.Maximum && numVal > Maximum) ||
                                    (ComparisonType == EValidationComparisonType.Minimum && numVal < Minimum))
                                {
                                    vr.Result = EValidationResult.Fail;
                                    vr.Message = "The value in the field " + field;
                                    if (ComparisonType == EValidationComparisonType.Maximum) vr.Message += " exceeds the maximum value for this field type";
                                    if (ComparisonType == EValidationComparisonType.Minimum) vr.Message += " is less than the minimum value for this field type";
                                }
                            }
                        }
                    }

                    res.Add(vr);
                }
            }

            return res;
        }
    }

    public class ValidationRuleLookup : List<ValidationRule>
    {
    }
}
