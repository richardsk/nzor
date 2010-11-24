using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace TestNZOR
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new Form1());

            string cnnStr = System.Configuration.ConfigurationManager.ConnectionStrings["NZOR"].ConnectionString;
            SqlConnection cnn = new SqlConnection(cnnStr);

            cnn.Open();
            NZOR.Data.DsIntegrationName data = NZOR.Data.ProviderName.GetAllDataForIntegration(cnn);
            cnn.Close();

            string f = @"C:\Development\NZOR\Dev\NZOR\Integration\data.dat";
            NZOR.Data.Integration.SaveDataFile(data, f);
            
            NZORConsole.Program.Main(new string[]{"Integrate", @"C:\Development\NZOR\Dev\NZOR\Integration\Configuration\IntegConfig.xml", f});

            //save data from file to DB
            data = NZOR.Data.Integration.LoadDataFile(f); //results

            cnn.Open();
            NZOR.Data.Integration.SaveIntegrationData(cnn, data);
            cnn.Close();
        }
    }
}
