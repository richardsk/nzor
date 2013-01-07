using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NZOR.Integration.Mapping
{
    public class NZORIntegrationField
    {
        public string Field = "";
        public bool RequiredForMatching = false;
        public bool MustBeProvided = false;
        public string PreText = "";
        public string PostText = "";

        private static Dictionary<Guid, NZORIntegrationFieldLookup> _nzorFields = new Dictionary<Guid, NZORIntegrationFieldLookup>();
        private static NZORIntegrationFieldLookup _simpleFields = new NZORIntegrationFieldLookup(true, false);
        
        /// <summary>
        /// Constructs a integration field, specifying if the field must be given in the provider dataset, and if the field must be provided/parsed/calculated to enable matching
        /// </summary>
        /// <param name="field"></param>
        /// <param name="mustBeProvided">Must be a field supplied by the provider dataset - i.e. it cannot be parsed or calculated from other fields.</param>
        /// <param name="requiredForMatching">Must be provided or able to be parsed so that it is populated for integration/matching</param>
        public NZORIntegrationField(string field, bool mustBeProvided, bool requiredForMatching, string preText, string postText)
        {
            this.Field = field;
            this.RequiredForMatching = requiredForMatching;
            this.MustBeProvided = mustBeProvided;
            this.PreText = preText;
            this.PostText = postText;
        }

        public static NZORIntegrationField GetField(Guid nameClassId, string fieldName)
        {
            if (!_nzorFields.ContainsKey(nameClassId)) return null;

            NZORIntegrationFieldLookup nl = _nzorFields[nameClassId];
            return nl[fieldName];
        }

        public static NZORIntegrationFieldLookup NZORFields(bool simple, Guid nameClassId, bool hasClassification)
        {
            if (simple) return _simpleFields; //simple matching/integration/validation 

            if (!_nzorFields.ContainsKey(nameClassId)) _nzorFields.Add(nameClassId, new NZORIntegrationFieldLookup(false, hasClassification));
            return _nzorFields[nameClassId];
        }
    }

    public class NZORIntegrationFieldLookup : Dictionary<String, NZORIntegrationField>
    {
        public static string ProviderRecordIdField = "ProviderRecordId";
        public static string FullNameField = "FullName";
        public static string TaxonRankField = "TaxonRank";
        public static string AuthorsField = "Authors";
        public static string GoverningCodeField = "GoverningCode";
        public static string CanonicalField = "Canonical";
        public static string YearOfPublicationField = "YearOfPublication";
        public static string MicroReferenceField = "MicroReference";
        public static string PublishedInField = "PublishedIn";
        public static string ParentNamesField = "ParentNames";
        public static string PreferredNamesField = "PreferredNames";

        public NZORIntegrationFieldLookup(bool simple, bool hasClassification)
        {
            if (simple)
            {
                this.Add(FullNameField, new NZORIntegrationField(FullNameField, true, false, "", ""));
                this.Add(ProviderRecordIdField, new NZORIntegrationField(ProviderRecordIdField, false, true, "", ""));
                this.Add(TaxonRankField, new NZORIntegrationField(TaxonRankField, false, true, "", ""));
                this.Add(AuthorsField, new NZORIntegrationField(AuthorsField, false, false, "", ""));
                this.Add(GoverningCodeField, new NZORIntegrationField(GoverningCodeField, false, false, "", ""));
                    //Parent Names format is [parent id:parent consensus id:parent full name],[ ...] - just working with full name
                this.Add(ParentNamesField, new NZORIntegrationField(ParentNamesField, false, false, "[::", "]"));
                    //Accpeted Names format is [accepted name id:accepted name consensus id:accepted name full name],[ ...]
                this.Add(PreferredNamesField, new NZORIntegrationField(PreferredNamesField, false, false, "[::", "]"));
            }
            else
            {
                if (hasClassification)
                {
                    this.Add(ProviderRecordIdField, new NZORIntegrationField(ProviderRecordIdField, true, true, "", ""));
                    this.Add(FullNameField, new NZORIntegrationField(FullNameField, true, false, "", ""));
                    this.Add(TaxonRankField, new NZORIntegrationField(TaxonRankField, false, true, "", ""));
                    this.Add(GoverningCodeField, new NZORIntegrationField(GoverningCodeField, false, true, "", ""));
                    this.Add(CanonicalField, new NZORIntegrationField(CanonicalField, true, true, "", ""));
                    this.Add(AuthorsField, new NZORIntegrationField(AuthorsField, false, false, "", ""));
                    this.Add(YearOfPublicationField, new NZORIntegrationField(YearOfPublicationField, false, false, "", ""));
                    this.Add(MicroReferenceField, new NZORIntegrationField(MicroReferenceField, false, false, "", ""));
                    this.Add(PublishedInField, new NZORIntegrationField(PublishedInField, false, false, "", ""));
                        //Parent Names format is [parent id:parent consensus id:parent full name],[ ...] - just working with ids
                    this.Add(ParentNamesField, new NZORIntegrationField(ParentNamesField, false, true, "[", "::]"));
                    this.Add(PreferredNamesField, new NZORIntegrationField(PreferredNamesField, false, false, "[", "::]"));
                }
                else
                {
                    //less validation for names with no classification
                    this.Add(ProviderRecordIdField, new NZORIntegrationField(ProviderRecordIdField, true, true, "", ""));
                    this.Add(FullNameField, new NZORIntegrationField(FullNameField, true, true, "", ""));
                    this.Add(AuthorsField, new NZORIntegrationField(AuthorsField, false, false, "", ""));
                    this.Add(YearOfPublicationField, new NZORIntegrationField(YearOfPublicationField, false, false, "", ""));
                    this.Add(MicroReferenceField, new NZORIntegrationField(MicroReferenceField, false, false, "", ""));
                    this.Add(PublishedInField, new NZORIntegrationField(PublishedInField, false, false, "", ""));
                    this.Add(PreferredNamesField, new NZORIntegrationField(PreferredNamesField, false, false, "[::", "]"));
                }
            }
        }

    }
}
