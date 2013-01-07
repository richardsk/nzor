using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NZOR.Data.Entities.Provider;
using NZOR.Admin.Data.Entities;
using System.Xml;
using NZOR.Admin.Data.Sql.Repositories;
using NZOR.Admin.Data.Repositories;
using NZOR.Data.Sql;
using System.Data;
using NZOR.Data.LookUps.Common;
using NZOR.Data.Entities.Common;
using NZOR.Matching;

namespace NZOR.ExternalLookups
{
    internal class CoLExternalLookupService : AbstractExternalLookupProvider
    {
        public CoLExternalLookupService(ExternalLookupService svc, IProviderRepository provRepository, TaxonRankLookUp rankLookup) : base(svc, provRepository, rankLookup)
        {
        }

        public override string GetNameSearchUrl(string fullName)
        {
            string nameText = fullName;
            NameParseResult npr = NameParser.ParseName(fullName, RankLookup);
            if (npr.Outcome == NameParseOutcome.Succeeded)
            {
                nameText = npr.GetFullName(RankLookup, false, null);
            }
            if (LookupService.SpaceCharacterSubstitute != null) nameText = nameText.Replace(" ", LookupService.SpaceCharacterSubstitute);
            return LookupService.NameLookupEndpoint + nameText;
        }

        public override string GetNameResolutionUrl(string id)
        {         
            return LookupService.IDLookupEndpoint + id;
        }

        public override List<ExternalNameResult> ParseMatchingNames(string nameSearchResult, bool topLevelOnly)
        {
            List<ExternalNameResult> names = new List<ExternalNameResult>();

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(nameSearchResult);
            
            XmlNodeList nameNodes = doc.SelectNodes("//results/result");

            DataSource ds = ProviderRepository.GetDataSourceByCode("CoL");

            foreach (XmlNode nn in nameNodes)
            {
                List<ExternalNameResult> enr = new List<ExternalNameResult>();
                ParseName(nn, ds, topLevelOnly, enr);
                
                names.AddRange(enr);
            }

            return names;
        }

        private List<ExternalNameResult> ParseName(XmlNode nn, DataSource ds, bool topLevelOnly, List<ExternalNameResult> results)
        {
            ExternalNameResult enr = new ExternalNameResult();
            results.Add(enr);

            string cnnStr = System.Configuration.ConfigurationManager.ConnectionStrings["NZOR"].ConnectionString;

            Name provName = new Name();
            enr.Name = provName;
            provName.NameId = Guid.NewGuid();
            provName.AddedDate = DateTime.Now;
            provName.DataSourceId = ds.DataSourceId;

            XmlNode idNode = nn.SelectSingleNode("id");
            if (idNode == null) return results;

            provName.ProviderRecordId = idNode.InnerText;

            XmlNode authorsNode = nn.SelectSingleNode("author");
            XmlNode infraRankNode = nn.SelectSingleNode("infraspecies_marker");
            XmlNode infraSpNode = nn.SelectSingleNode("infraspecies");
            
            XmlNode rankNode = nn.SelectSingleNode("rank");
            int rankSortOrder = -1;
            if (rankNode != null)
            {
                TaxonRank rank = RankLookup.GetTaxonRank(rankNode.InnerText, null);
                if (rank != null) 
                {
                    provName.TaxonRankId = rank.TaxonRankId;
                    rankSortOrder = rank.SortOrder ?? -1;
                }
            }

            provName.FullName = nn.SelectSingleNode("name").InnerText;
            if (infraRankNode != null && infraRankNode.InnerText.Length > 0) provName.FullName += " " + infraRankNode.InnerText;
            if (infraSpNode != null && infraSpNode.InnerText.Length > 0) provName.FullName += " " + infraSpNode.InnerText;
            if (authorsNode != null && authorsNode.InnerText.Length > 0) provName.FullName += " " + authorsNode.InnerText;

            XmlNode kingdomNode = nn.SelectSingleNode("classification/taxon[rank='Kingdom']");
            if (kingdomNode == null && rankSortOrder == TaxonRankLookUp.SortOrderKingdom) kingdomNode = nn;
            if (kingdomNode != null)
            {
                string kingdom = kingdomNode.SelectSingleNode("name").InnerText;
                DataTable kdt = NZOR.Data.Sql.Matching.GetPartialNameMatches(cnnStr, kingdom);
                if (kdt != null && kdt.Rows.Count > 0)
                {
                    provName.GoverningCode = kdt.Rows[0]["GoverningCode"].ToString();
                }
            }
            if (provName.GoverningCode == null || provName.GoverningCode == "") provName.GoverningCode = "NCGN";

            provName.NameClassId = new Guid("A5233111-61A0-4AE6-9C2B-5E8E71F1473A");

            XmlNode sdNode = nn.SelectSingleNode("record_scrutiny_date/scrutiny");
            DateTime sdt = DateTime.MinValue;
            if (sdNode != null && DateTime.TryParse(sdNode.InnerText, out sdt)) provName.ProviderModifiedDate = sdt;
            
            XmlNode genusNode = nn.SelectSingleNode("genus");
            XmlNode speciesNode = nn.SelectSingleNode("species");

            //canonical
            if (genusNode != null || speciesNode != null || infraSpNode != null || provName.FullName != null)
            {
                NameProperty cnp = new NameProperty();
                cnp.NamePropertyTypeId = new Guid("1F64E93C-7EE8-40D7-8681-52B56060D750");
                cnp.Value = ((infraSpNode != null && infraSpNode.InnerText.Length > 0) ? infraSpNode.InnerText : (speciesNode != null ? speciesNode.InnerText : (genusNode != null ? genusNode.InnerText : provName.FullName)));
                provName.NameProperties.Add(cnp);
            }

            //authors
            if (authorsNode != null)
            {
                NameProperty anp = new NameProperty();
                anp.NamePropertyTypeId = new Guid("006D86A8-08A5-4C1A-BC08-C07B0225E01B");
                anp.Value = authorsNode.InnerText;
                provName.NameProperties.Add(anp);
            }


            XmlNode urlNode = nn.SelectSingleNode("url");
            if (urlNode != null) enr.WebUrl = urlNode.InnerText;

            if (!topLevelOnly)
            {
                //concepts/parents
                XmlNode statusNode = nn.SelectSingleNode("name_status");
                if (statusNode != null && statusNode.InnerText != "")
                {
                    Concept pc = new Concept();
                    ExternalNameResult existingResult = results.SingleOrDefault(e => e.Concepts.SingleOrDefault(c => c.ProviderNameId == provName.ProviderRecordId) != null);
                    if (existingResult != null)
                    {
                        pc = existingResult.Concepts.Single(c => c.ProviderNameId == provName.ProviderRecordId);
                    }
                    else
                    {
                        pc.ConceptId = Guid.NewGuid();
                        pc.AddedDate = DateTime.Now;
                        pc.DataSourceId = ds.DataSourceId;
                        pc.ProviderModifiedDate = provName.ProviderModifiedDate;
                        pc.ProviderNameId = provName.ProviderRecordId;
                        pc.ProviderRecordId = "CoL_NameConcept_" + provName.ProviderRecordId;
                        pc.Type = Concept.ConceptType.TaxonNameUse;
                        pc.NameId = provName.NameId;
                        enr.Concepts.Add(pc);
                    }

                    Concept toPc = null;
                    if (statusNode.InnerText.Contains("accepted"))
                    {
                        toPc = pc;
                    }
                    else
                    {
                        //get other name
                        XmlNode snNode = nn.SelectSingleNode("sn_id");
                        if (snNode != null && snNode.InnerText != "0")
                        {
                            string result = DoNameLookup(GetNameResolutionUrl(snNode.InnerText));
                            XmlDocument doc = new XmlDocument();
                            doc.LoadXml(result);

                            XmlNode nameNode = doc.SelectSingleNode("//results/result");
                            string toId = nameNode.SelectSingleNode("id").InnerText;

                            List<ExternalNameResult> snEnr = ParseName(nameNode, ds, true, results);                            
                            if (snEnr.Count > 0)
                            {
                                ExternalNameResult sen = snEnr.FirstOrDefault(p => p.Name.ProviderRecordId == toId);
                                existingResult = results.SingleOrDefault(e => e.Concepts.SingleOrDefault(c => c.ProviderNameId== sen.Name.ProviderRecordId) != null);
                                //Concept existingConcept = enr.Concepts.SingleOrDefault(c => c.ProviderNameId == snEnr[0].Name.ProviderRecordId);

                                if (existingResult != null)
                                {
                                    toPc = existingResult.Concepts.Single(c => c.ProviderNameId == sen.Name.ProviderRecordId);
                                }
                                else
                                {
                                    results.Add(sen);

                                    toPc = new Concept();
                                    toPc.AddedDate = DateTime.Now;
                                    toPc.ConceptId = Guid.NewGuid();
                                    toPc.DataSourceId = ds.DataSourceId;
                                    toPc.NameId = sen.Name.NameId;
                                    toPc.ProviderModifiedDate = provName.ProviderModifiedDate;
                                    toPc.ProviderRecordId = "CoL_NameConcept_" + sen.Name.ProviderRecordId;
                                    toPc.Type = Concept.ConceptType.TaxonNameUse;
                                    sen.Concepts.Add(toPc);
                                }
                            }
                        }
                    }

                    if (toPc != null)
                    {
                        ConceptRelationship pcr = new ConceptRelationship();
                        pcr.FromConceptId = pc.ConceptId;
                        pcr.ToConceptId = toPc.ConceptId;
                        pcr.InUse = true;
                        pcr.ConceptRelationshipId = Guid.NewGuid();
                        pcr.ConceptRelationshipTypeId = new Guid("0CA79AB3-E213-4F51-88B9-4CE01F735A1D");

                        pc.ConceptRelationships.Add(pcr);
                    }
                }


                XmlNode parentNode = nn.SelectSingleNode("(classification/taxon)[last()]");
                if (parentNode != null)
                {
                    Concept pc = new Concept();
                    ExternalNameResult existingResult = results.SingleOrDefault(e => e.Concepts.SingleOrDefault(c => c.ProviderNameId == provName.ProviderRecordId) != null);
                    if (existingResult != null)
                    {
                        pc = existingResult.Concepts.Single(c => c.ProviderNameId == provName.ProviderRecordId);
                    }
                    else
                    {
                        pc.ConceptId = Guid.NewGuid();
                        pc.AddedDate = DateTime.Now;
                        pc.DataSourceId = ds.DataSourceId;
                        pc.ProviderModifiedDate = provName.ProviderModifiedDate;
                        pc.ProviderNameId = provName.ProviderRecordId;
                        pc.ProviderRecordId = "CoL_NameConcept_" + provName.ProviderRecordId;
                        pc.Type = Concept.ConceptType.TaxonNameUse;
                        pc.NameId = provName.NameId;
                        enr.Concepts.Add(pc);
                    }

                    Concept toPc = null;
                    //get other name
                    XmlNode pnNode = parentNode.SelectSingleNode("id");
                    if (pnNode != null)
                    {
                        string result = DoNameLookup(GetNameResolutionUrl(pnNode.InnerText));
                        XmlDocument doc = new XmlDocument();
                        doc.LoadXml(result);

                        XmlNode nameNode = doc.SelectSingleNode("//results/result");
                        string toId = nameNode.SelectSingleNode("id").InnerText;

                        List<ExternalNameResult> pnEnr = ParseName(nameNode, ds, topLevelOnly, results);
                        if (pnEnr.Count > 0)
                        {
                            ExternalNameResult pen = pnEnr.FirstOrDefault(p => p.Name.ProviderRecordId == toId);
                            existingResult = results.SingleOrDefault(e => e.Concepts.SingleOrDefault(c => c.ProviderNameId == pen.Name.ProviderRecordId) != null);
                            //Concept existingConcept = enr.Concepts.SingleOrDefault(c => c.ProviderNameId == pnEnr[0].Name.ProviderRecordId);

                            if (existingResult != null)
                            {
                                toPc = existingResult.Concepts.Single(c => c.ProviderNameId == pen.Name.ProviderRecordId);
                            }
                            else
                            {
                                results.Add(pen);

                                toPc = new Concept();
                                toPc.AddedDate = DateTime.Now;
                                toPc.ConceptId = Guid.NewGuid();
                                toPc.DataSourceId = ds.DataSourceId;
                                toPc.NameId = pen.Name.NameId;
                                toPc.ProviderModifiedDate = provName.ProviderModifiedDate;
                                toPc.ProviderRecordId = "CoL_NameConcept_" + pen.Name.ProviderRecordId;
                                toPc.Type = Concept.ConceptType.TaxonNameUse;
                                pen.Concepts.Add(toPc);
                            }
                        }
                    }

                    if (toPc != null)
                    {
                        ConceptRelationship pcr = new ConceptRelationship();
                        pcr.FromConceptId = pc.ConceptId;
                        pcr.ToConceptId = toPc.ConceptId;
                        pcr.InUse = true;
                        pcr.ConceptRelationshipId = Guid.NewGuid();
                        pcr.ConceptRelationshipTypeId = new Guid("6A11B466-1907-446F-9229-D604579AA155");

                        pc.ConceptRelationships.Add(pcr);
                    }
                }
            }

            enr.DataSourceCode = "CoL";
            enr.LookupService = LookupService;
            enr.DataUrl = LookupService.IDLookupEndpoint + provName.ProviderRecordId;
            
            return results;
        }
    }
}
