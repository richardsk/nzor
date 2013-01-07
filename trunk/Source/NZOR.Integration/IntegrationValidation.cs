using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NZOR.Integration.Mapping;
using NZOR.Validation;

namespace NZOR.Integration
{
    public class IntegrationValidation
    {
        public ValidationResultLookup ValidateProviderData(NZOR.Data.DataSets.DsIntegrationName.ProviderNameRow pnRow)
        {
            ValidationResultLookup results = null;

            ValidationRuleLookup rules = new ValidationRuleLookup();
            foreach (NZORIntegrationField f in NZORIntegrationField.NZORFields(false, pnRow.NameClassID, pnRow.HasClassification).Values)
            {
                //for parent name checking, dont fail if the parent is not defined by the provider (= match on name and rank only)
                if (f.Field == NZORIntegrationFieldLookup.ParentNamesField && (pnRow.IsParentNamesNull() || pnRow.ParentNames == String.Empty) && pnRow.TaxonRankSort <= NZOR.Data.LookUps.Common.TaxonRankLookUp.SortOrderGenus)
                {
                    continue;
                }

                if (f.RequiredForMatching)
                {
                    ValidationRule vr = new ValidationRule();
                    vr.Column = f.Field;
                    vr.ComparisonType = EValidationComparisonType.NotNullOrEmpty;
                    rules.Add(vr);
                }
            }

            results = ValidationProcessor.ValidateDataRow(pnRow, rules);

            return results;
        }

    }
}
