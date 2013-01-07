using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Data.SqlClient;
using NZOR.Data.DataSets;

namespace NZOR.Command.Test.UnitTests
{
    [TestFixture]
    public class CommandTest
    {
        static System.Timers.Timer _t = new System.Timers.Timer();

        [Test]
        public void TestCSVIntegrate()
        {
        }

        [Test]
        public void TestLog()
        {
            NZOR.Server.Log.LogEvent("test");
        }

        [Test]
        public void TestHarvest()
        {
            _t.Elapsed += new System.Timers.ElapsedEventHandler(Timer_Elapsed);
            _t.Interval = 8000;
            _t.Start();

            NZOR.Server.Process.RunHarvest(true);

            _t.Stop();
        }

        [Test]
        public void TestInitialHarvest()
        {
            _t.Elapsed += new System.Timers.ElapsedEventHandler(Timer_Elapsed);
            _t.Interval = 8000;
            _t.Start();

            NZOR.Server.Process.RunInitialHarvest(null);

            _t.Stop();
        }

        [Test]
        public void TestIntegrate()
        {
            string cnnStr = System.Configuration.ConfigurationManager.ConnectionStrings["NZOR"].ConnectionString;
         
            //update stackedname data first?
            //NZOR.Data.Sql.Integration.UpdateProviderStackedNameData(cnnStr);

            //DsIntegration data = NZOR.Data.Sql.Integration.GetAllDataForIntegration(cnnStr, adminCnnStr, null);
         
            string f = @"C:\Development\NZOR\Source\NZOR.Integration\data.dat";
            //NZOR.Data.Sql.Integration.SaveDataFile(data, f);

            _t.Elapsed += new System.Timers.ElapsedEventHandler(Timer_Elapsed);
            _t.Interval = 8000;
            _t.Start();

            try
            {
                NZOR.Server.Process.MaxRecords = 4000;

                NZOR.Server.Process.RunIntegration(@"C:\Development\NZOR\Source\NZOR.Integration\Configuration\IntegConfig.xml", f, "NZFUNGI", false, false);

                //save data from file to DB
                //data = NZOR.Data.Sql.Integration.LoadDataFile(f); //results

                //NZOR.Data.Sql.Integration.SaveIntegrationData(cnnStr, data);
            }
            catch (Exception ex)
            {
                string s = ex.Message;
            }

            _t.Stop();
        }

        //static void TestLog()
        //{
          //  NZOR.Utility.Log.MaxLogSize = 10;
         //   NZOR.Utility.Log.LogEvent("test event");
        //    NZOR.Utility.Log.LogEvent("test event 2");
        //}

        static void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            //Console.WriteLine("Saving data... " + NZOR.Data.Sql.Integration.Progress.ToString() + "% complete.");
            //if (NZOR.Data.Sql.Integration.Progress < 100) _t.Start();

            Console.WriteLine(NZOR.Server.Process.Status);
            if (!NZOR.Server.Process.Finished) _t.Start();
        }
    }
}
