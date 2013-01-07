using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace OAIServer
{
    class Utility
    {
        private static int _colKey = 0;

        public static void ReplaceXmlField(ref string xml, string field, Object xeVal)
        {
            String val = "";
            if (xeVal != null) val = xeVal.ToString();
            xml = xml.Replace(field, val);
        }

        public static String NextColumnKey()
        {
            _colKey++;
            return "Col" + _colKey.ToString();
        }

    }
}
