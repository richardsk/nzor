using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NZOR.Admin.Data.Entities;
using NZOR.Publish.Model.Search.Names;
using NZOR.Admin.Data.Entities.Matching;

namespace NZOR.Matching.Batch
{
    //public class NameToMatch
    //{
    //    public string ProviderNameId = "";
    //    public string ProviderFullName = "";
    //    public NamesSearchResponse MatchResult = null;
    //    public string Message = "";
    //}

    //public class FlattenedResult
    //{
    //    public string ProviderRecordId;
    //    public string ProviderNameFull;
    //    public string PreferredName;
    //    public string ScientificForVernacular;
    //    public string Classification;
    //    public string HighestLevelNameFullName;
    //    public string NameWithoutAuthor;
    //    public Guid? Id;
    //    public string DisplayText;
    //    public int MatchScore;
    //}

    //public class MatchData
    //{
    //    public Match BatchMatch = null;
    //    public List<NameToMatch> MatchNames = new List<NameToMatch>();
    //    public string ErrorMsg = "";

    //    public List<FlattenedResult> FlattenedResults
    //    {
    //        get
    //        {
    //            List<FlattenedResult> ret = new List<FlattenedResult>();

    //            foreach (NameToMatch ntm in MatchNames)
    //            {
    //                bool match = false;
    //                if (ntm.MatchResult != null)
    //                {
    //                    int ranking = 1;
    //                    foreach (var nr in ntm.MatchResult.Results)
    //                    {
    //                        if (nr != null && nr.Name.FullName != null)
    //                        {
    //                            FlattenedResult fr = new FlattenedResult();
    //                            fr.ProviderRecordId = ntm.ProviderNameId;
    //                            fr.ProviderNameFull = ntm.ProviderFullName;
    //                            fr.DisplayText = nr.Name.FullName;
    //                            fr.Id = nr.Name.NameId;
    //                            fr.MatchScore = ranking;
    //                            fr.NameWithoutAuthor = nr.Name.PartialName;
    //                            fr.PreferredName = nr.Name.AcceptedName.FullName;
    //                           // fr.ScientificForVernacular = nr.IsVernacularForNames;
    //                            //  fr.Classification = nr.HigherClassification;  
    //                            //   fr.HighestLevelNameFullName = nr.HighestLevelNameFullName;

    //                            ret.Add(fr);

    //                            ranking++;
    //                            match = true;
    //                        }
    //                    }
    //                }

    //                if (!match)
    //                {
    //                    FlattenedResult fr = new FlattenedResult();
    //                    fr.ProviderRecordId = ntm.ProviderNameId;
    //                    fr.ProviderNameFull = ntm.ProviderFullName;
    //                    fr.DisplayText = "No match found";
    //                    fr.Id = Guid.Empty;
    //                    fr.MatchScore = 0;


    //                    string cnnStr = System.Configuration.ConfigurationManager.ConnectionStrings["NZOR"].ConnectionString;
    //                    NZOR.Data.Entities.Common.NameParseResult npr = NZOR.Matching.NameParser.ParseName(fr.ProviderNameFull);
    //                    NZOR.Data.Sql.Repositories.Common.LookUpRepository lr = new NZOR.Data.Sql.Repositories.Common.LookUpRepository(cnnStr);
    //                    NZOR.Data.LookUps.Common.TaxonRankLookUp trl = new NZOR.Data.LookUps.Common.TaxonRankLookUp(lr.GetTaxonRanks());
    //                    try
    //                    {
    //                        fr.NameWithoutAuthor = npr.GetFullName(trl, false, null);
    //                    }
    //                    catch (Exception e)
    //                    {
    //                        fr.DisplayText = "Processing Error";
    //                        //TODO Elmah.ErrorLog.GetDefault(  HttpContext.Current ).Log( new Elmah.Error(e,  HttpContext.Current) );
    //                    }

    //                    if (fr.NameWithoutAuthor == null)
    //                    {
    //                        fr.NameWithoutAuthor = string.Empty;
    //                    }

    //                    ret.Add(fr);
    //                }
    //            }


    //            return ret;
    //        }
    //    }
    //}

}
