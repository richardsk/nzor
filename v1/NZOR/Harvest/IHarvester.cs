using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace NZOR.Harvest
{
    public interface IHarvester
    {        
        XElement ListIds();
        XElement GetRecord(String id);
        XElement GetRecords(DateTime fromDate);
        XElement GetRecordSet(DateTime fromDate);
        XElement GetDeletedRecords(DateTime fromDate);
        Boolean Ping();
    }
}
