using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NZOR.Admin.Data.Entities
{
    public enum ProviderCode
    {
        NZFLORA,
        NZIB,
        NZFUNGI,
        NZAC,
        NZOR_Hosted,
        NZOR_Test,
        NZOR_Test_2
    }

    public class Provider : Entity
    {
        public Guid ProviderId { get; set; }

        public String Code { get; set; }
        public String Name { get; set; }
        public String Url { get; set; }
        public String ContactEmail { get; set; }
        public String Disclaimer { get; set; }
        public String Attribution { get; set; }
        public String Licensing { get; set; }
        public String PublicStatement { get; set; }
        public DateTime? AddedDate { get; set; }
        public String AddedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public String ModifiedBy { get; set; }

        public List<DataSource> DataSources { get; set; }
        public List<AttachmentPoint> AttachmentPoints { get; set; }

        public Provider()
        {
            ProviderId = Guid.Empty;

            Code = null;
            Name = null;
            Url = null;
            ContactEmail = null;
            Disclaimer = null;
            Attribution = null;
            Licensing = null;
            PublicStatement = null;
            AddedDate = null;
            AddedBy = null;
            ModifiedDate = null;
            ModifiedBy = null;

            DataSources = new List<DataSource>();
            AttachmentPoints = new List<AttachmentPoint>();
        }
    }
}
