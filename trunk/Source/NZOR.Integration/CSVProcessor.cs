using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data.SqlClient;
using System.Data;

using NZOR.Data.DataSets;
using NZOR.Data.Entities.Common;
using NZOR.Data.LookUps.Common;
using NZOR.Data.Sql.Repositories.Common;
using NZOR.Data.Entities.Integration;

namespace NZOR.Integration
{
    public class CSVProcessor
    {
        public long LoadingProgress = 0;

        public DataForIntegration GetDataForIntegration(String cnnStr, String csvFileName, Mapping.IntegrationMapping mapping, ref List<String> errors)
        {
            LoadingProgress = 0;

            DataForIntegration data = new DataForIntegration(IntegrationDatasetType.SingleNamesList);

            //get consensus names
            Data.Sql.Integration.GetConsensusNameDataForIntegration(cnnStr, ref data);

            DsIntegrationName dsn = new DsIntegrationName();
            ParseProviderData(cnnStr, csvFileName, mapping, ref dsn, ref errors);
            
            data.SingleNamesSet = dsn;

            //if (mapping.MappingType == IntegrationMappingType.Simple && mapping.GetMappingByDestination(Mapping.NZORIntegrationFieldLookup.ParentIdField) == null) numLines = numLines * 2;

            //long progPos = 0;

            LoadingProgress = 100;

            return data;
        }

        /// <summary>
        /// Simple parsing and of csv data.  Column mappings must include mappings to ProviderRecordId and FullName.
        /// </summary>
        /// <param name="cnnStr"></param>
        /// <param name="csvFileName"></param>
        /// <param name="mapping"></param>
        /// <param name="errors"></param>
        /// <returns></returns>
        public NameParseResultCollection ParseSimpleProviderData(String cnnStr, String csvFileName, Mapping.IntegrationMapping mapping)
        {
            NameParseResultCollection results = new NameParseResultCollection();

            LoadingProgress = 0;

            ///count lines
            long numLines = 0;
            StreamReader rdr = new StreamReader(csvFileName);
            while (rdr.ReadLine() != null) numLines++;
            rdr.Close();

            using (Microsoft.VisualBasic.FileIO.TextFieldParser parser = new Microsoft.VisualBasic.FileIO.TextFieldParser(csvFileName))
            {
                parser.HasFieldsEnclosedInQuotes = true;
                parser.TextFieldType = Microsoft.VisualBasic.FileIO.FieldType.Delimited;
                parser.SetDelimiters(",");

                List<String> colNames = new List<string>();
                if (mapping.HasColumnHeaders) colNames.AddRange(parser.ReadFields());

                LookUpRepository rep = new LookUpRepository(cnnStr);
                TaxonRankLookUp trl = new TaxonRankLookUp(rep.GetTaxonRanks());

                while (!parser.EndOfData)
                {
                    string[] vals = null;
                    string fullName = "";
                    string recordId = "";
                    try
                    {
                        vals = parser.ReadFields();
                        
                        Mapping.ColumnMapping fullNameMapping = mapping.GetMappingByDestination(Mapping.NZORIntegrationFieldLookup.FullNameField);
                        fullName = vals[fullNameMapping.SourceColumnIndex];
                        NameParseResult npr = Matching.NameParser.ParseName(fullName, trl);

                        Mapping.ColumnMapping idMapping = mapping.GetMappingByDestination(Mapping.NZORIntegrationFieldLookup.ProviderRecordIdField);
                        if (idMapping != null) 
                        {
                            npr.Id = vals[idMapping.SourceColumnIndex];
                            recordId = npr.Id;
                        }

                        results.Add(npr);
                    }
                    catch (Exception ex)
                    {
                        NameParseResult npr = new NameParseResult();
                        npr.Error = ex.Message;
                        npr.OriginalNameText = fullName;
                        npr.Id = recordId;
                        results.Add(npr);
                    }

                    LoadingProgress = (parser.LineNumber * 100 / numLines);
                    if (LoadingProgress >= 100) LoadingProgress = 99;
                }

                parser.Close();
            }

            LoadingProgress = 100;

            return results;
        }

        public void ParseProviderData(String cnnStr, String csvFileName, Mapping.IntegrationMapping mapping, ref DsIntegrationName nameData, ref List<String> errors)
        {
            LoadingProgress = 0;

            LookUpRepository rep = new LookUpRepository(cnnStr);
            List<TaxonRank> ranks = rep.GetTaxonRanks();
            TaxonRankLookUp trl = new TaxonRankLookUp(ranks);
            NameClassLookUp ncl = new NameClassLookUp(rep.GetNameClasses());
            NameClass nameCls = ncl.GetNameClassById(mapping.NameClassID);

            //count lines
            long numLines = 0;
            StreamReader rdr = new StreamReader(csvFileName);
            while (rdr.ReadLine() != null) numLines++;
            rdr.Close();

            using (Microsoft.VisualBasic.FileIO.TextFieldParser parser = new Microsoft.VisualBasic.FileIO.TextFieldParser(csvFileName))
            {
                parser.HasFieldsEnclosedInQuotes = true;
                parser.TextFieldType = Microsoft.VisualBasic.FileIO.FieldType.Delimited;
                parser.SetDelimiters(",");

                List<String> colNames = new List<string>();
                if (mapping.HasColumnHeaders) colNames.AddRange(parser.ReadFields());
                                
                while (!parser.EndOfData)
                {
                    DsIntegrationName.ProviderNameRow newRow = nameData.ProviderName.NewProviderNameRow();
                                        
                    string[] vals = null;

                    try
                    {
                        vals = parser.ReadFields();

                        foreach (Mapping.ColumnMapping cm in mapping.ColumnMappings)
                        {
                            int sourcePos = cm.SourceColumnIndex;
                            String val = "";

                            if (cm.MapType == Mapping.ColumnMapping.ColumnMapType.StaticText) val = cm.StaticTextValue;
                            else
                            {
                                if (vals[sourcePos].Length > 0) val = vals[sourcePos];
                            }

                            if (val.Length > 0)
                            {
                                newRow[cm.DestinationField.Field] = cm.DestinationField.PreText + val + cm.DestinationField.PostText;

                                if (cm.DestinationField.Field.Equals("ProviderRecordID", StringComparison.InvariantCultureIgnoreCase))
                                {
                                    //use provider record id for name id too
                                    Guid id = Guid.Empty;
                                    if (!Guid.TryParse(val, out id)) id = Guid.NewGuid();
                                    newRow.NameID = id;
                                }
                            }
                        }

                        //if the mapping type is simple, then we need to do some parsing, elaboration etc
                        if (mapping.MappingType == Mapping.IntegrationMappingType.Simple) ParseSimpleMappingFields(mapping, newRow, trl);

                        //standardise ranks etc
                        String err = StandardiseValues(ranks, nameCls, newRow);
                        if (err.Length > 0) errors.Add(err);

                        AddPNRow(nameData.ProviderName, newRow, false);
                    }
                    catch (Exception ex)
                    {
                        errors.Add(ex.Message);
                    }

                    LoadingProgress = (parser.LineNumber * 100 / numLines);
                    if (LoadingProgress >= 100) LoadingProgress = 99;
                }
                
                parser.Close();
            }

            //if (mapping.MappingType == IntegrationMappingType.Simple && mapping.GetMappingByDestination(Mapping.NZORIntegrationFieldLookup.ParentIdField) == null)
            //{
            //    progPos = numLines / 2;

            //    DsIntegrationName.ProviderNameDataTable newNames = new DsIntegrationName.ProviderNameDataTable();

            //    //foreach (DsIntegrationName.ProviderNameRow pnRow in data.ProviderName)
            //    //{
            //    //    try
            //    //    {
            //    //        BuildParentsForSimpleMapping(mapping, pnRow, newNames);                        
            //    //    }
            //    //    catch (Exception ex)
            //    //    {
            //    //        errors.Add(ex.Message);
            //    //    }

            //    //    LoadingProgress = (progPos * 100 / numLines);
            //    //    if (LoadingProgress >= 100) LoadingProgress = 99;
            //    //    progPos++;
            //    //}

            //    foreach (DsIntegrationName.ProviderNameRow pr in newNames)
            //    {
            //        String err = StandardiseValues(ranks, mapping.NameClassID, nameCls, pr);
            //        if (err.Length > 0) errors.Add(err);
            //        AddPNRow(data.ProviderName, pr, true);
            //    }
            //}

            LoadingProgress = 100;
        }

        private void AddPNRow(DsIntegrationName.ProviderNameDataTable data, DsIntegrationName.ProviderNameRow pnRow, bool doCopy)
        {
            //find correct sort position for row
            //only if valid row
            if (pnRow.IsTaxonRankSortNull()) return;

            int pos = 0;
            foreach (DsIntegrationName.ProviderNameRow pr in data)
            {
                if (pr.TaxonRankSort > pnRow.TaxonRankSort) break;
                pos++;
            }
            if (doCopy) data.ImportRow(pnRow);
            else data.Rows.InsertAt(pnRow, pos);
        }

        private void ParseSimpleMappingFields(Mapping.IntegrationMapping mapping, DsIntegrationName.ProviderNameRow pnRow, TaxonRankLookUp trl)
        {
            //simple mapping probably only has full name and parent.  maybe Id and rank

            NameParseResult parsedName = Matching.NameParser.ParseName(pnRow.FullName, trl);

            if (mapping.GetMappingByDestination(Mapping.NZORIntegrationFieldLookup.CanonicalField) == null || pnRow[Mapping.NZORIntegrationFieldLookup.CanonicalField].ToString() == string.Empty)
            {
                pnRow[Mapping.NZORIntegrationFieldLookup.CanonicalField] = parsedName.GetCanonical().Text;
            }

            if (mapping.GetMappingByDestination(Mapping.NZORIntegrationFieldLookup.TaxonRankField) == null || pnRow[Mapping.NZORIntegrationFieldLookup.TaxonRankField].ToString() == string.Empty)
            {
                pnRow[Mapping.NZORIntegrationFieldLookup.TaxonRankField] = parsedName.GetCanonical().Rank;
            }

            if (mapping.GetMappingByDestination(Mapping.NZORIntegrationFieldLookup.ProviderRecordIdField) == null || pnRow[Mapping.NZORIntegrationFieldLookup.ProviderRecordIdField].ToString() == string.Empty)
            {
                pnRow[Mapping.NZORIntegrationFieldLookup.ProviderRecordIdField] = Guid.NewGuid().ToString(); //random id
            }

            if (mapping.GetMappingByDestination(Mapping.NZORIntegrationFieldLookup.AuthorsField) == null || pnRow[Mapping.NZORIntegrationFieldLookup.AuthorsField].ToString() == string.Empty)
            {                
                pnRow[Mapping.NZORIntegrationFieldLookup.AuthorsField] = parsedName.GetAuthors();
            }

            if (mapping.GetMappingByDestination(Mapping.NZORIntegrationFieldLookup.ParentNamesField) == null || pnRow[Mapping.NZORIntegrationFieldLookup.ParentNamesField].ToString() == string.Empty)
            {
                NamePart par = parsedName.GetParent();
                if (par != null) pnRow[Mapping.NZORIntegrationFieldLookup.ParentNamesField] = "[::" + par.Text + "]"; //format is [parent id:parent cons id:parent full name]
            }
        }

        private void BuildParentsForSimpleMapping(Mapping.IntegrationMapping mapping, DsIntegrationName.ProviderNameRow pnRow, DsIntegrationName.ProviderNameDataTable newNames, TaxonRankLookUp trl)
        {
            NameParseResult parsedName = Matching.NameParser.ParseName(pnRow.FullName, trl);

            DsIntegrationName.ProviderNameRow newRow = null;

            //build parent hierarchy
            for (int i = 0; i < parsedName.NameParts.Count - 1; i++)
            {
                DataRow[] parRows = pnRow.Table.Select("Canonical = '" + parsedName.NameParts[i].Text.Replace("'","''") + "' and TaxonRank = '" + parsedName.NameParts[i].Rank + "'");
                DataRow[] newParRows = newNames.Select("Canonical = '" + parsedName.NameParts[i].Text.Replace("'","''") + "' and TaxonRank = '" + parsedName.NameParts[i].Rank + "'");
                if (parRows.Length > 1) throw new Exception("Cannot parse name " + pnRow.FullName + ".  Parent name is ambiguous.");

                if (parRows.Length == 0 && newParRows.Length == 0)
                {
                    //need to add parent
                    newRow = newNames.NewProviderNameRow();
                    newRow.NameID = Guid.NewGuid();
                    newRow.ProviderRecordID = newRow.NameID.ToString();
                    newRow.FullName = parsedName.NameParts[i].Text;
                    newRow.Canonical = parsedName.NameParts[i].Text;
                    newRow.TaxonRank = parsedName.NameParts[i].Rank;
                    newNames.AddProviderNameRow(newRow);
                                        
                    pnRow[Mapping.NZORIntegrationFieldLookup.ParentNamesField] = "[" + newRow.NameID + "::" + newRow.FullName + "]";
                }
                else
                {
                    //set parent id
                    if (parRows.Length > 0) pnRow[Mapping.NZORIntegrationFieldLookup.ParentNamesField] = "[" + parRows[0]["NameID"].ToString() + "::" + parRows[0]["FullName"].ToString() + "]";
                    else pnRow[Mapping.NZORIntegrationFieldLookup.ParentNamesField] = "[" + newParRows[0]["NameID"].ToString() + "::" + newParRows[0]["FullName"].ToString() + "]";
                }
            }
        }

        public String StandardiseValues(List<TaxonRank> ranks, NameClass cls, DsIntegrationName.ProviderNameRow row)
        {
            //set rank id, match rule set id, etc
            //return any error
            String err = "";

            try
            {
                row.NameClassID = cls.NameClassId;
                row.NameClass = cls.Name;
                row.HasClassification = cls.HasClassification;
                row.LinkStatus = NZOR.Data.LinkStatus.Unmatched.ToString();

                foreach (TaxonRank r in ranks)
                {
                    if (r.KnownAbbreviations.ToLower().Contains("@" + row.TaxonRank.ToLower() + "@"))
                    {
                        row.TaxonRankID = r.TaxonRankId;
                        row.TaxonRank = r.Name;
                        row.MatchRuleSetID = (r.MatchRuleSetId.HasValue ? r.MatchRuleSetId.Value : -1);
                        row.TaxonRankSort = r.SortOrder.Value;
                        break;
                    }
                }
                if (row.IsTaxonRankIDNull()) err = "Unknown Taxon Rank : " + row.TaxonRank + ", for name " + row.FullName + " : ID = " + row.NameID.ToString();

                //TODO others?
            }
            catch (Exception ex)
            {
                err = "Error processing name " + row.FullName + ", ID = " + row.NameID.ToString() + " : " + ex.Message;
            }

            return err;
        }
    }
}
