using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NZOR.Data.Entities.Common;

namespace NZOR.Data.LookUps.Common
{
    public class ReferencePropertyTypeLookUp
    {
        private List<ReferencePropertyType> _referencePropertyTypes;

        public static readonly String Title = @"Title";
        public static readonly String StartPage = @"Start Page";
        public static readonly String Editor = @"Editor";
        public static readonly String Author = @"Author";
        public static readonly String Keyword = @"Keyword";
        public static readonly String Citation = @"Citation";
        public static readonly String Volume = @"Volume";
        public static readonly String Edition = @"Edition";
        public static readonly String PlaceOfPublication = @"PlaceOfPublication";
        public static readonly String EndPage = @"End Page";
        public static readonly String Link = @"Link";
        public static readonly String Date = @"Date";
        public static readonly String Page = @"Page";
        public static readonly String Publisher = @"Publisher";
        public static readonly String ParentReferenceID = @"ParentReferenceID";
        public static readonly String Issue = @"Issue";
        public static readonly String Identifier = @"Identifer";

        public ReferencePropertyTypeLookUp(List<ReferencePropertyType> referencePropertyTypes)
        {
            _referencePropertyTypes = referencePropertyTypes;
        }

        public ReferencePropertyType GetReferencePropertyType(String type)
        {
            ReferencePropertyType referencePropertyType = null;

            if (_referencePropertyTypes != null)
            {
                referencePropertyType = (from o in _referencePropertyTypes
                                         where o.Name == type
                                         select o).SingleOrDefault();
            }

            return referencePropertyType;
        }
    }
}
