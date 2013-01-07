using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using NZOR.Data.DataSets;
using NZOR.Data.Entities.Integration;

namespace NZOR.Matching
{
    /// <summary>
    /// A class to hold all data to be used for matching (when DB disconnection is desired)
    /// </summary>
    public class MatchData
    {
        private static Mutex _providerDataMutex = new Mutex();
        private static Mutex _consensusDataMutex = new Mutex();
        
        private bool _requireProviderMutex = false;
        private bool _requireConsensusMutex = false;
        
        public DataForIntegration AllData { get; set; }
        public DsIntegrationName NameIntegrationSet { get; set; }
        public DsIntegrationReference ReferencesIntegrationSet { get; set; }
        
        public MatchData()
        {
        }

        public MatchData(bool requireProviderMutex, bool requireConsensusMutex, DataForIntegration dataForIntegration, DsIntegrationName nameIntegrationSet, DsIntegrationReference refsIntegrationSet)
        {
            AllData = dataForIntegration;
            NameIntegrationSet = nameIntegrationSet;
            ReferencesIntegrationSet = refsIntegrationSet;
            _requireProviderMutex = requireProviderMutex;
            _requireConsensusMutex = requireConsensusMutex;            
        }

        public void GetProviderDataLock()
        {
            if (_requireProviderMutex) _providerDataMutex.WaitOne();
        }

        public void ReleaseProviderDataLock()
        {
            if (_requireProviderMutex) _providerDataMutex.ReleaseMutex();
        }

        public void GetConsensusDataLock()
        {
            if (_requireConsensusMutex) _consensusDataMutex.WaitOne();
        }

        public void ReleaseConsensusDataLock()
        {
            if (_requireConsensusMutex) _consensusDataMutex.ReleaseMutex();
        }

        public void GetAllDataLock()
        {
            GetProviderDataLock();
            GetConsensusDataLock();
        }

        public void ReleaseAllDataLock()
        {
            ReleaseConsensusDataLock();
            ReleaseProviderDataLock();
        }

    }
}
