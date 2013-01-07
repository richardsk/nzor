using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NZOR.Data.Entities.Common;

namespace NZOR.Data.LookUps.Common
{
    public class NameClassLookUp
    {
        private List<NameClass> _nameClasses;

        public static readonly String ScientificName = @"Scientific Name";
        public static readonly String TradeName = @"Trade Name";
        public static readonly String VernacularName = @"Vernacular Name";

        public NameClassLookUp(List<NameClass> nameClasses)
        {
            _nameClasses = nameClasses;
        }

        public NameClass GetNameClass(String nameClassCode)
        {
            NameClass nameClass = null;

            if (_nameClasses != null)
            {
                nameClass = (from o in _nameClasses
                             where o.Code.Equals(nameClassCode, StringComparison.OrdinalIgnoreCase)
                             select o).SingleOrDefault();
            }

            return nameClass;
        }

        public NameClass GetNameClassById(Guid nameClassId)
        {
            NameClass nameClass = null;

            if (_nameClasses != null)
            {
                nameClass = (from o in _nameClasses
                             where o.NameClassId.Equals(nameClassId)
                             select o).SingleOrDefault();
            }

            return nameClass;
        }
    }
}
