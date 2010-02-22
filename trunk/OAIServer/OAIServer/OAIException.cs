using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OAIServer
{
    public enum OAIError
    {
        idDoesNotExist,
        badResumptionToken,
        badArgument,
        Unknown
    }

    public class OAIException : Exception
    {
        OAIError _oaiError = OAIError.Unknown;

        public OAIException(OAIError errCode)
        {
            _oaiError = errCode;
        }

        public override string ToString()
        {
            return "<error code=\"" + _oaiError.ToString() + "\"/>";
        }
    }
}
