using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NZOR.Data.Entities.Common;

namespace NZOR.Data.LookUps.Common
{
    /// <summary>
    /// Utility class for looking up name property type ids.
    /// </summary>
    public class NamePropertyTypeLookUp
    {
        List<NamePropertyType> _namePropertyTypes;

        public const String Rank = @"Rank";

        public const String Canonical = @"Canonical";
        public const String Uninomial = @"Uninomial";
        public const String Genus = @"Genus";
        public const String InfragenericEpithet = @"InfragenericEpithet";
        public const String SpecificEpithet = @"SpecificEpithet";
        public const String InfraspecificEpithet = @"InfraspecificEpithet";
        public const String CultivarNameGroup = @"CultivarNameGroup";

        public const String Authors = @"Authors";
        public const String Basionym = @"Basionym";
        public const String BasionymAuthors = @"BasionymAuthors";
        public const String BlockedName = @"BlockedName";
        public const String CombinationAuthors = @"CombinationAuthors";
        public const String Country = @"Country";
        public const String Language = @"Language";
        public const String LaterHomonymOf = @"LaterHomonymOf";
        public const String MicroReference = @"MicroReference";
        public const String NomenclaturalStatus = @"NomenclaturalStatus";
        public const String Orthography = @"Orthography";
        public const String PublishedIn = @"PublishedIn";
        public const String QualityCode = @"QualityCode";
        public const String RecombinedName = @"RecombinedName";
        public const String TypeName = @"TypeName";
        public const String Year = @"Year";
        public const String YearOnPublication = @"YearOnPublication";
        public const String YearOfPublication = @"YearOfPublication";

        public const String NameTextFullName = @"NameText_FullName";
        public const String NameTextFullNameFormatted = @"NameText_FullNameFormatted";
        public const String NameTextPartialName = @"NameText_PartialName";
        public const String NameTextPartialNameFormatted = @"NameText_PartialNameFormatted";

        public NamePropertyTypeLookUp(List<NamePropertyType> namePropertyTypes)
        {
            _namePropertyTypes = namePropertyTypes;
        }

        public NamePropertyType GetNamePropertyType(Guid nameClassId, String namePropertyTypeName)
        {
            NamePropertyType namePropertyType = _namePropertyTypes.SingleOrDefault(
                o => o.NameClassId == nameClassId && o.Name.Equals(namePropertyTypeName, StringComparison.OrdinalIgnoreCase));

            return namePropertyType;
        }

        public NamePropertyType GetNamePropertyType(Guid namePropertyTypeId)
        {
            NamePropertyType namePropertyType = _namePropertyTypes.SingleOrDefault(
                o => o.NamePropertyTypeId.Equals(namePropertyTypeId));

            return namePropertyType;
        }
    }
}
