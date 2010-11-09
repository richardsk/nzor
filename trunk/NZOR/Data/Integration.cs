using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;

namespace NZOR.Data
{
    public class IntegrationSaveResult
    {
        public int NumberInserted = 0;
        public int NumberUpdated = 0;
        public int ProviderNamesIntegrated = 0;
        public int ProviderNamesWithErrors = 0;
        public List<String> Errors = new List<string>();
    }

    public class Integration
    {
        public static int Progress = 0;

        public static IntegrationSaveResult SaveIntegrationData(SqlConnection cnn, DsIntegrationName data)
        {
            Progress = 0;
            int total = data.ProviderName.Count + data.ConsensusName.Count;
            int done = 0;

            IntegrationSaveResult isr = new IntegrationSaveResult();

            foreach (DsIntegrationName.ConsensusNameRow cnRow in data.ConsensusName)
            {
                if (cnRow.RowState == System.Data.DataRowState.Added)
                {
                    ConsensusName.AddConsensusName(cnn, cnRow);
                    isr.NumberInserted++;
                }
                else if (cnRow.RowState == System.Data.DataRowState.Modified)
                {
                    ConsensusName.UpdateConsensusName(cnn, cnRow);
                    isr.NumberUpdated++;
                }

                done++;
                Progress = (done * 100 / total);
            }

            foreach (DsIntegrationName.ProviderNameRow pnRow in data.ProviderName)
            {
                if (pnRow.RowState == System.Data.DataRowState.Modified)
                {
                    ProviderName.UpdateProviderName(cnn, pnRow);
                    if (pnRow.LinkStatus == LinkStatus.DataFail.ToString() || pnRow.LinkStatus == LinkStatus.Multiple.ToString() || pnRow.LinkStatus == LinkStatus.MultipleParent.ToString() ||
                        pnRow.LinkStatus == LinkStatus.ParentMissing.ToString() || pnRow.LinkStatus == LinkStatus.ParentNotIntegrated.ToString())
                    {
                        isr.ProviderNamesWithErrors++;
                    }
                    else
                    {
                        isr.ProviderNamesIntegrated++;
                    }
                }

                done++;
                Progress = (done * 100 / total);
            }


            //TODO - any more clean up?

            return isr;
        }

    }
}
