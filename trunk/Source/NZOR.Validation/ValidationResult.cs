using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NZOR.Validation
{
    public enum EValidationResult
    {
        NotChecked,
        Pass,
        Fail
    }

    public class ValidationResult
    {
        public EValidationResult Result = EValidationResult.NotChecked;
        public String Message = "";
    }

    public class ValidationResultLookup : List<ValidationResult>
    {
        public bool AllClear()
        {
            bool allclear = true;
            foreach (ValidationResult vr in this)
            {
                if (vr.Result == EValidationResult.Fail) allclear = false;
            }
            return allclear;
        }

        public string ErrorMessages()
        {
            string msgs = "";
            foreach (ValidationResult vr in this)
            {
                if (vr.Result == EValidationResult.Fail && vr.Message.Length > 0)
                {
                    if (msgs.Length > 0) msgs += " : ";
                    msgs += vr.Message;
                }
            }
            return msgs;
        }

        public List<ValidationResult> FailedResults()
        {
            List<ValidationResult> res = new List<ValidationResult>();

            foreach (ValidationResult vr in this)
            {
                if (vr.Result == EValidationResult.Fail) res.Add(vr);
            }

            return res;
        }
    }
}
