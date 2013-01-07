using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace NZOR.Validation
{
    public class ValidationProcessor
    {
        public static bool RecordSuccessfulValidations = false;

        public static ValidationResultLookup ValidateDataTable(DataTable dt, ValidationRuleLookup rules)
        {
            ValidationResultLookup res = new ValidationResultLookup();

            foreach (ValidationRule r in rules)
            {
                ValidationResultLookup vr = r.Validate(EValidationObjectType.DataTable, dt);
                res.AddRange(vr.FailedResults());
            }

            return res;
        }

        public static ValidationResultLookup ValidateDataRow(DataRow row, ValidationRuleLookup rules)
        {
            ValidationResultLookup res = new ValidationResultLookup();

            foreach (ValidationRule r in rules)
            {
                ValidationResultLookup vr = r.Validate(EValidationObjectType.DataRow, row);
                res.AddRange(vr.FailedResults());
            }

            return res;
        }
    }
}
