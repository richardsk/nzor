using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace NZOR.Harvest
{
    public static class Utility
    {
        public static T GetAttributeValue<T>(XElement element, String attributeName, T defaultValue)
        {
            if (element == null)
            {
                return defaultValue;
            }
            else
            {
                XAttribute target = element.Attribute(attributeName);

                if (target == null)
                {
                    return defaultValue;
                }
                else
                {
                    Type type = typeof(T);

                    if (type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(Nullable<>)))
                    {
                        return (T)Convert.ChangeType(target.Value, Nullable.GetUnderlyingType(type));
                    }
                    else
                    {
                        return (T)Convert.ChangeType(target.Value, type);
                    }
                }
            }
        }

        public static T GetElementValue<T>(XElement element, XNamespace xmlnamespace, String elementName, T defaultValue)
        {
            if (element == null)
            {
                return defaultValue;
            }
            else
            {
                if (String.IsNullOrEmpty(elementName))
                {
                    return (T)Convert.ChangeType(element.Value.Trim(), typeof(T));
                }
                else
                {
                    XElement target = element.Element((xmlnamespace ?? String.Empty) + elementName);

                    if (target == null)
                    {
                        return defaultValue;
                    }
                    else
                    {
                        return (T)Convert.ChangeType(target.Value.Trim(), typeof(T));
                    }
                }
            }
        }
    }
}
