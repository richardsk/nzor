using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

using NZOR.Integration.Mapping;
using NZOR.Data.DataSets;
using NZOR.Admin.Data.Process;
using System.Configuration;
using NZOR.Publish.Publisher;
using System.IO;

namespace NZOR.Server
{
    public class Process
    {
        public enum ProcessType
        {
            Unknown,
            Harvest,
            Match,
            Integrate,
            WebTransfer
        }

        public delegate void StatusEventHandler(string status, string runOutcome);
        public static event StatusEventHandler StatusEvent;

        public static int MaxRecords = -1;

        private static Integration.SQLIntegrationProcessor _processor = new Integration.SQLIntegrationProcessor();
        private static Harvest.Harvester _harvester = new Harvest.Harvester();

        private static ProcessType _processType = ProcessType.Unknown;

        //public static int Progress
        //{
        //    get
        //    {
        //        if (_processType == ProcessType.Integrate && _processor.Progress >= 100) return NZOR.Data.Sql.Integration.Progress;
        //        if (_processType == ProcessType.Integrate || _processType == ProcessType.Match) return _processor.Progress;
        //        return _harvester.Progress;
        //    }
        //}

        public static bool Finished
        {
            get
            {
                if (_processType == ProcessType.Harvest) return (_harvester.Progress >= 100);
                
                //if (_processType == ProcessType.Match) return (_processor.Progress >= 100);
                
                if (_processType == ProcessType.Integrate) return (NZOR.Data.Sql.Integration.Progress >= 100);

                return true;
            }
        }

        public static string Status
        {
            get
            {
                 
                //if (_processType == ProcessType.Integrate && _processor.Progress >= 100) return "Saving data... " + NZOR.Data.Sql.Integration.Progress.ToString() + "% complete.";
                //if ((_processType == ProcessType.Integrate || _processType == ProcessType.Match) && _processor.StatusText == "") return "Loading ...  0% complete.";
                if (_processType == ProcessType.Integrate || _processType == ProcessType.Match) return _processor.StatusText;                
                return _harvester.StatusText;
            }
        }

        /// <summary>
        /// Perfomrs both matching and integration.
        /// </summary>
        /// <param name="configFilePath"></param>
        /// <param name="dataFilePath"></param>
        public static ProcessResult RunIntegration(string configFilePath, string dataFilePath, string dataSourceCode, bool updateStackedNameData, bool doFullIntegration)
        {
            ProcessResult result = new ProcessResult();

            _processType = ProcessType.Integrate;

            try
            {
                if (StatusEvent != null) StatusEvent("Integrating", null);

                string cnnStr = System.Configuration.ConfigurationManager.ConnectionStrings["NZOR"].ConnectionString;

                Guid? dataSourceId = null;
                if (dataSourceCode != null && dataSourceCode != "")
                {
                    Admin.Data.Repositories.IProviderRepository pr = new Admin.Data.Sql.Repositories.ProviderRepository(cnnStr);
                    Admin.Data.Entities.DataSource ds = pr.GetDataSourceByCode(dataSourceCode);
                    if (ds != null)
                    {
                        dataSourceId = ds.DataSourceId;
                    }
                    else
                    {
                        throw new Exception("Unrecognised datasource code");
                    }
                }

                if (updateStackedNameData) NZOR.Data.Sql.Integration.UpdateProviderStackedNameData(cnnStr);
                
                XmlDocument config = new XmlDocument();
                config.Load(configFilePath);

                //Admin.Data.Repositories.IAdminRepository ar = new Admin.Data.Sql.Repositories.AdminRepository(adminCnnStr, cnnStr);
                //Admin.Data.Entities.NZORStatistics stats = ar.GetStatistics();

                if (doFullIntegration)
                {
                    _processor.RunInitialIntegration(config, false);
                }
                else
                {
                    _processor.RunUpdateIntegration(config);
                }



                //if (stats.NZORNameCount <= 1) //empty DB
                //{
                //    _processor.RunInitialIntegration(config);
                //}
                //else
                //{
                //    _processor.RunUpdateIntegration(config);
                //}


                /*NZOR.Data.Entities.Integration.DataForIntegration data = NZOR.Data.Sql.Integration.GetAllDataForIntegration(cnnStr, adminCnnStr, dataSourceId);

                //NZOR.Data.Entities.Integration.DataForIntegration data = NZOR.Data.Sql.Integration.GetGroupedDataForIntegration(Data.Entities.Integration.IntegrationDataGroup.FirstCharacterOfTaxonName, cnnStr, adminCnnStr, dataSourceId);
                
                NZOR.Data.Sql.Integration.SaveDataFile(data, dataFilePath);*/

                //DoMatching(configFilePath, dataFilePath);

                /*
                //save data from file to DB
                data = NZOR.Data.Sql.Integration.LoadDataFile(dataFilePath); //results

                NZOR.Data.Sql.Integration.SaveIntegrationData(cnnStr, adminCnnStr, data, true);
                */

                //update the stats
                Admin.Data.Repositories.IAdminRepository ar = new Admin.Data.Sql.Repositories.AdminRepository(cnnStr);
                ar.UpdateStatistics();

                result.ErrorMessages.AddRange(NZOR.Integration.IntegratorThread.Log);

                result.Result = ProcessResultOutcome.Succeded;
            }
            catch (Exception ex)
            {
                result.Result = ProcessResultOutcome.Failed;
                result.ErrorMessages.Add(DateTime.Now.ToString("s") + " : " + ex.Message + " : " + ex.StackTrace);
            }

            return result;
             
        }

        /// <summary>
        /// Perfortms the mathing process only, loading data from and putting the results into the dataFilePath file.
        /// </summary>
        /// <param name="configFilePath"></param>
        /// <param name="dataFilePath"></param>
        public static ProcessResult RunMatching(string configFilePath, string csvFilePath, string outputFilePath)
        {
            ProcessResult result = new ProcessResult();

            _processType = ProcessType.Match;

            try
            {
                XmlDocument config = new XmlDocument();
                config.Load(configFilePath);
                

                ////TODO 

                string cnnStr = System.Configuration.ConfigurationManager.ConnectionStrings["NZOR"].ConnectionString;
                
                List<string> errList = new List<string>();
                
                IntegrationMapping im = new IntegrationMapping();
                im.HasColumnHeaders = true;
                im.NameClassID = new Guid("A5233111-61A0-4AE6-9C2B-5E8E71F1473A");

                im.AddMapping(new ColumnMapping(0, "NameId", ColumnMapping.ColumnMapType.Column, NZORIntegrationField.GetField(im.NameClassID, "NameId"), ""));
                im.AddMapping(new ColumnMapping(3, "FullName", ColumnMapping.ColumnMapType.Column, NZORIntegrationField.GetField(im.NameClassID, "FullName"), ""));
                im.AddMapping(new ColumnMapping(4, "TaxonRank", ColumnMapping.ColumnMapType.Column, NZORIntegrationField.GetField(im.NameClassID, "TaxonRank"), ""));
                im.AddMapping(new ColumnMapping(6, "Authors", ColumnMapping.ColumnMapType.Column, NZORIntegrationField.GetField(im.NameClassID, "Authors"), ""));
                im.AddMapping(new ColumnMapping(20, "GoverningCode", ColumnMapping.ColumnMapType.Column, NZORIntegrationField.GetField(im.NameClassID, "GoverningCode"), ""));
                im.AddMapping(new ColumnMapping(5, "Canonical", ColumnMapping.ColumnMapType.Column, NZORIntegrationField.GetField(im.NameClassID, "Canonical"), ""));
                im.AddMapping(new ColumnMapping(28, "YearOfPublication", ColumnMapping.ColumnMapType.Column, NZORIntegrationField.GetField(im.NameClassID, "YearOfPublication"), ""));
                im.AddMapping(new ColumnMapping(12, "MicroReference", ColumnMapping.ColumnMapType.Column, NZORIntegrationField.GetField(im.NameClassID, "MicroReference"), ""));
                im.AddMapping(new ColumnMapping(10, "PublishedIn", ColumnMapping.ColumnMapType.Column, NZORIntegrationField.GetField(im.NameClassID, "PublishedIn"), ""));
                im.AddMapping(new ColumnMapping(43, "ParentId", ColumnMapping.ColumnMapType.Column, NZORIntegrationField.GetField(im.NameClassID, "ParentID"), ""));
                im.AddMapping(new ColumnMapping(42, "PreferredNameId", ColumnMapping.ColumnMapType.Column, NZORIntegrationField.GetField(im.NameClassID, "PreferredNameID"), ""));


                Integration.CSVProcessor proc = new Integration.CSVProcessor();
                NZOR.Data.Entities.Integration.DataForIntegration dfi = proc.GetDataForIntegration(cnnStr, csvFilePath, im, ref errList);

                List<Matching.MatchResult> results = _processor.GetMatches(dfi);

                result.ErrorMessages.AddRange(NZOR.Integration.IntegratorThread.Log);

                result.Result = ProcessResultOutcome.Succeded;
            }
            catch (Exception ex)
            {
                result.Result = ProcessResultOutcome.Failed;
                result.ErrorMessages.Add(DateTime.Now.ToString("s") + " : " + ex.Message + " : " + ex.StackTrace);
            }

            return result;
        }
        
        static void processor_StatusUpdate(object sender, Data.Entities.Integration.StatusEventArgs e)
        {
            if (StatusEvent != null) StatusEvent(e.Status, null);
        }

        /// <summary>
        /// Performs a harvest from all Provider endpoints that require harvesting
        /// </summary>
        public static ProcessResult RunHarvest(bool processUpdates)
        {
            ProcessResult result = new ProcessResult();

            _processType = ProcessType.Harvest;

            try
            {
                if (StatusEvent != null) StatusEvent("Harvesting", null);
                
                string cnnStr = System.Configuration.ConfigurationManager.ConnectionStrings["NZOR"].ConnectionString;
                
                _harvester.RunHarvesting(cnnStr, processUpdates);

                while (_harvester.Progress < 100)
                {
                    System.Threading.Thread.Sleep(10000);
                    if (StatusEvent != null) StatusEvent("Harvesting : " + _harvester.StatusText, null);
                }

                if (_harvester.TotalHarvested > 0) result.UpdateRequired = true;

                result.ErrorMessages.AddRange(_harvester.Errors);
                result.Log.AddRange(_harvester.Log);
                if (_harvester.StatusText == "Error")
                {
                    result.Result = ProcessResultOutcome.Failed;
                }
                else
                {
                    result.Result = ProcessResultOutcome.Succeded;
                }
            }
            catch(Exception ex)
            {
                result.Result = ProcessResultOutcome.Failed;
                result.ErrorMessages.Add(DateTime.Now.ToString("s") + " : " + ex.Message + " : " + ex.StackTrace);
            }

            return result;
        }

        public static ProcessResult RunInitialHarvest(string providerCode)
        {
            ProcessResult result = new ProcessResult();

            _processType = ProcessType.Harvest;

            try
            {
                if (StatusEvent != null) StatusEvent("Running Initial Harvest", null);

                string cnnStr = System.Configuration.ConfigurationManager.ConnectionStrings["NZOR"].ConnectionString;
                string provCnnStr = System.Configuration.ConfigurationManager.ConnectionStrings["Name_Cache"].ConnectionString;

                NZOR.Admin.Data.Entities.ProviderCode provCode;
                if (providerCode == null)
                {
                    //harvest all providers
                    //clear DB first
                    //NZOR.Data.Sql.Integration.ClearConsensusData(cnnStr);
                    //NZOR.Data.Sql.Integration.ClearProviderData(cnnStr);

                    Admin.Data.Repositories.IProviderRepository pr = new Admin.Data.Sql.Repositories.ProviderRepository(cnnStr);
                    List<Admin.Data.Entities.Provider> provs = pr.GetProviders();

                    foreach (Admin.Data.Entities.Provider p in provs)
                    {
                        if (Enum.TryParse(p.Code, out provCode))
                        {
                            //harvest data sources that have an endpoint
                            bool doHarvest = false;
                            foreach (Admin.Data.Entities.DataSource ds in p.DataSources)
                            {
                                List<Admin.Data.Entities.DataSourceEndpoint> dseList = pr.GetDatasetEndpoints(ds.DataSourceId);
                                foreach (Admin.Data.Entities.DataSourceEndpoint dse in dseList)
                                {
                                    if (dse.Url != null && dse.Url != "")
                                    {
                                        doHarvest = true;
                                        break;
                                    }
                                }
                            }

                            if (doHarvest)
                            {
                                if (StatusEvent != null) StatusEvent("Harvesting " + provCode, null);

                                _harvester.RunInitialHarvest(provCode, provCnnStr, cnnStr);

                                while (_harvester.Progress < 100)
                                {
                                    System.Threading.Thread.Sleep(10000);
                                    if (StatusEvent != null) StatusEvent("Initial Harvest : " + _harvester.StatusText, null);
                                }

                                result.ErrorMessages.AddRange(_harvester.Errors);
                                _harvester.Errors.Clear();
                                result.Log.AddRange(_harvester.Log);
                                _harvester.Log.Clear();
                            }
                        }
                    }
                }
                else if (Enum.TryParse(providerCode, out provCode))
                {                    
                    _harvester.RunInitialHarvest(provCode, provCnnStr, cnnStr);

                    while (_harvester.Progress < 100)
                    {
                        System.Threading.Thread.Sleep(10000);
                        if (StatusEvent != null) StatusEvent("Initial Harvest : " + _harvester.StatusText, null);
                    }
                                        
                    result.ErrorMessages.AddRange(_harvester.Errors);
                    result.Log.AddRange(_harvester.Log);
                }

                if (StatusEvent != null) StatusEvent("Completed Successfully", null);
                result.Result = ProcessResultOutcome.Succeded;
            }
            catch (Exception ex)
            {
                result.Result = ProcessResultOutcome.Failed;

                string msg = ex.Message + " : " + ex.StackTrace;
                if (ex.InnerException != null) msg += " : From: " + ex.InnerException.Message + " : " + ex.InnerException.StackTrace;

                result.ErrorMessages.Add(msg);
            }

            return result;
        }

        public static ProcessResult RunWebTransfer()
        {
            ProcessResult result = new ProcessResult();

            _processType = ProcessType.WebTransfer;

            try
            {
                if (StatusEvent != null) StatusEvent("Running Web Transfer", null);

                string dataConnectionString = ConfigurationManager.ConnectionStrings["NZOR"].ConnectionString;
                string outputFullFolderName = ConfigurationManager.AppSettings["OutputFullFolderName"];

                IndexBuilder indexBuilder = new IndexBuilder(dataConnectionString, outputFullFolderName);
                Publisher publisher = new Publisher(indexBuilder, false);
                publisher.GenerateIndexes();

                string deployFolder = ConfigurationManager.AppSettings["WebIndexDeployFolder"];
                string backupFolder = ConfigurationManager.AppSettings["WebIndexBackupFolder"];

                DirectoryInfo fromDir = new DirectoryInfo(outputFullFolderName);
                DirectoryInfo toDir = new DirectoryInfo(deployFolder);
                DirectoryInfo backupDir = new DirectoryInfo(backupFolder);
                CopyWebFiles(fromDir, toDir, backupDir);

                if (StatusEvent != null) StatusEvent("Transfer Completed Successfully", null);
                result.Result = ProcessResultOutcome.Succeded;
            }
            catch (Exception ex)
            {
                result.Result = ProcessResultOutcome.Failed;

                string msg = ex.Message + " : " + ex.StackTrace;
                if (ex.InnerException != null) msg += " : From: " + ex.InnerException.Message + " : " + ex.InnerException.StackTrace;

                result.ErrorMessages.Add(msg);
            }

            return result;
        }

        public static void CopyWebFiles(DirectoryInfo source, DirectoryInfo target, DirectoryInfo backupDir)
        {
            if (source.FullName.ToLower() == target.FullName.ToLower())
            {
                return;
            }

            if (Directory.Exists(target.FullName) == false)
            {
                Directory.CreateDirectory(target.FullName);
            }
            else
            {
                foreach (FileInfo file in target.GetFiles())
                {
                    file.CopyTo(Path.Combine(backupDir.FullName, file.Name), true);
                    file.Delete();
                }
                if (!Directory.Exists(Path.Combine(backupDir.FullName, "Indexes"))) Directory.CreateDirectory(Path.Combine(backupDir.FullName, "Indexes"));
                if (!Directory.Exists(Path.Combine(backupDir.FullName, "Indexes\\Names"))) Directory.CreateDirectory(Path.Combine(backupDir.FullName, "Indexes\\Names"));

                foreach (DirectoryInfo subDirectory in target.GetDirectories("Indexes\\Names"))
                {
                    foreach (FileInfo file in subDirectory.GetFiles())
                    {
                        if (!file.IsReadOnly)
                        {
                            file.CopyTo(Path.Combine(backupDir.FullName, "Indexes\\Names", file.Name), true);
                            file.Delete();
                        }
                    }
                }
            }

            foreach (FileInfo fi in source.GetFiles())
            {
                fi.CopyTo(Path.Combine(target.ToString(), fi.Name), true);
            }

            foreach (DirectoryInfo indexesDir in source.GetDirectories("Indexes"))
            {
                DirectoryInfo indexesSubDir = target.CreateSubdirectory(indexesDir.Name);

                foreach (DirectoryInfo namesDir in indexesDir.GetDirectories("Names"))
                {
                    DirectoryInfo namesSubDir = indexesSubDir.CreateSubdirectory(namesDir.Name);
                    foreach (FileInfo fi in namesDir.GetFiles())
                    {
                        if (!fi.IsReadOnly) fi.CopyTo(Path.Combine(namesSubDir.ToString(), fi.Name), true);
                    }
                }
            }
        }

    }
}
