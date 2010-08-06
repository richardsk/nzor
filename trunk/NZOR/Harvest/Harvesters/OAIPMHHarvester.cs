using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NZOR.Harvest
{
    class OAIPMHHarvester : NZOR.Harvest.IHarvester
    {
        #region IHarvester Members

        public System.Xml.Linq.XElement ListIds()
        {
            throw new NotImplementedException();
        }

        public System.Xml.Linq.XElement GetRecord(string id)
        {
            throw new NotImplementedException();
        }

        public System.Xml.Linq.XElement GetRecords(DateTime fromDate)
        {
            throw new NotImplementedException();
        }

        public System.Xml.Linq.XElement GetRecordSet(DateTime fromDate)
        {
            throw new NotImplementedException();
        }

        public System.Xml.Linq.XElement GetDeletedRecords(DateTime fromDate)
        {
            throw new NotImplementedException();
        }

        public bool Ping()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
