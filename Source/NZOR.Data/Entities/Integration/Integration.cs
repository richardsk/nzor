using System;
using System.Collections.Generic;

using NZOR.Data.DataSets;
using NZOR.Admin.Data.Datasets;

namespace NZOR.Data.Entities.Integration
{
    public enum IntegrationOutcome
    {
        Added,
        Mathced,
        Failed
    }

    public enum IntegrationState
    {
        None,
        Refreshing,
        References,
        Names,
        Concepts,
        Completed
    }

    public enum IntegrationDataGroup
    {
        FirstCharacterOfTaxonName
    }

    public enum IntegrationDatasetType
    {
        SingleNamesList,
        NamesByRank
    }

    public class StatusEventArgs : EventArgs
    {
        public string Status;

        public StatusEventArgs(string status)
        {
            Status = status;
        }
    }

    public class RecordForRefresh
    {
        public Guid Id = Guid.Empty;
        public NZOR.Admin.Data.Entities.NZORRecordType RecordType = Admin.Data.Entities.NZORRecordType.None;
        public String Error = null;
    }

    public class DeprecatedRecord
    {
        public string ProviderRecordId = string.Empty;
        public string ProviderCode = string.Empty;
        public string DataSourceCode = string.Empty;
        public NZOR.Admin.Data.Entities.NZORRecordType RecordType = Admin.Data.Entities.NZORRecordType.None;
    }

    public class CalculateNameParentData
    {
        public Guid ProviderNameID = Guid.Empty;
        public String ProviderNameCanonical = null;
        public String ProviderNameFullName = null;
        public List<Guid> ProviderNameParents = new List<Guid>();
        public Guid DataSourceID = Guid.Empty;
        public String ProviderRecordID = null;
        public Guid ProviderNameRankID = Guid.Empty;
        public String GoverningCode = null;
        public int ProviderNameRankSort = -1;
        public List<Admin.Data.Entities.AttachmentPoint> AttachmentPoints = null;
    }

    public class CalculateNameParentResult
    {
        public Guid ParentID = Guid.Empty;
        public String ParentFullName = null;
        public Guid ParentRankID = Guid.Empty;
        public string ErrorMessage = "";
    }

    public class IntegrationSaveResult
    {
        public int NamesInserted = 0;
        public int NamesUpdated = 0;
        public List<Guid> ProviderNamesIntegrated = new List<Guid>();
        public List<Guid> ProviderNamesWithErrors = new List<Guid>();

        public int ReferencesInserted = 0;
        public int ReferencesUpdated = 0;
        public List<Guid> ProviderReferencesIntegrated = new List<Guid>();
        public List<Guid> ProviderReferencesWithErrors = new List<Guid>();

        public List<String> Errors = new List<string>();

        public bool HaveProcessedReference(Guid provRefId)
        {
            if (ProviderReferencesIntegrated.Contains(provRefId)) return true;
            if (ProviderReferencesWithErrors.Contains(provRefId)) return true;
            return false;
        }

        public bool HaveProcessedName(Guid provNameId)
        {
            if (ProviderNamesIntegrated.Contains(provNameId)) return true;
            if (ProviderNamesWithErrors.Contains(provNameId)) return true;
            return false;
        }
    }

    public class DataForIntegration
    {
        public DsIntegrationReference References { get; set; }
        public SerializableDictionary<Guid, List<DsIntegrationName>> NamesByRank { get; set; }
        public DsIntegrationName SingleNamesSet { get; set; }
        public DsAttachmentPoint AttachmentPoints { get; set; }
        public DsConsensusData ConsensusData { get; set; }
        public IntegrationDatasetType DatasetType { get; set; }

        public DataForIntegration()
        {
        }

        public DataForIntegration(IntegrationDatasetType datasetType)
        {
            DatasetType = datasetType;
            NamesByRank = new SerializableDictionary<Guid, List<DsIntegrationName>>();
        }

        public DsIntegrationName.ProviderNameRow GetProviderName(Guid nameId)
        {
            DsIntegrationName.ProviderNameRow pn = null;

            if (DatasetType == IntegrationDatasetType.NamesByRank)
            {
                foreach (List<DsIntegrationName> dsList in NamesByRank.Values)
                {
                    foreach (DsIntegrationName dsn in dsList)
                    {
                        pn = dsn.ProviderName.FindByNameID(nameId);
                        if (pn != null) break;
                    }
                    if (pn != null) break;
                }
            }
            else
            {
                pn = SingleNamesSet.ProviderName.FindByNameID(nameId);
            }

            return pn;
        }

        public int ProviderNameCount()
        {
            int cnt = 0;

            if (DatasetType == IntegrationDatasetType.SingleNamesList)
            {
                cnt = SingleNamesSet.ProviderName.Count;
            }
            else
            {
                foreach (List<DsIntegrationName> dsList in NamesByRank.Values)
                {
                    foreach (DsIntegrationName dsn in dsList)
                    {
                        cnt += dsn.ProviderName.Count;
                    }
                }
            }

            return cnt;
        }

        public List<DsIntegrationName.ProviderNameRow> GetProviderNames(Guid consensusNameId)
        {
            List<DsIntegrationName.ProviderNameRow> pnList = new List<DsIntegrationName.ProviderNameRow>();

            if (DatasetType == IntegrationDatasetType.NamesByRank)
            {
                foreach (List<DsIntegrationName> dsList in NamesByRank.Values)
                {
                    foreach (DsIntegrationName dsn in dsList)
                    {
                        pnList.AddRange((DsIntegrationName.ProviderNameRow[])dsn.ProviderName.Select("ConsensusNameID = '" + consensusNameId.ToString() + "'"));
                    }
                }
            }
            else
            {
                pnList.AddRange((DsIntegrationName.ProviderNameRow[])SingleNamesSet.ProviderName.Select("ConsensusNameID = '" + consensusNameId.ToString() + "'"));
            }

            return pnList;
        }

        public List<DsIntegrationName.ProviderNameRow> GetChildProviderNames(Guid parentNameId)
        {
            List<DsIntegrationName.ProviderNameRow> pnList = new List<DsIntegrationName.ProviderNameRow>();

            if (DatasetType == IntegrationDatasetType.NamesByRank)
            {
                foreach (List<DsIntegrationName> dsList in NamesByRank.Values)
                {
                    foreach (DsIntegrationName dsn in dsList)
                    {
                        pnList.AddRange((DsIntegrationName.ProviderNameRow[])dsn.ProviderName.Select("ParentNames like '%" + parentNameId.ToString() + "%'"));
                    }
                }
            }
            else
            {
                pnList.AddRange((DsIntegrationName.ProviderNameRow[])SingleNamesSet.ProviderName.Select("ParentNames like '%" + parentNameId.ToString() + "%'"));
            }

            return pnList;
        }
    }

    public class ConsensusValueResult
    {
        public bool HasMajority = false;
        public object Value = null;
        public int? Sequence = null;
        public Guid? RelatedId = null;
        public Guid? accordingToId = null;
    }
}
