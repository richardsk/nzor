using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NZOR.Publish.Model.Administration
{

    public class SettingResponse
    {
        public Guid SettingId { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
