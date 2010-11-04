using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Data.SqlClient;

using NZOR.Data;

namespace TestNZOR
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            SqlConnection cnn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["NZOR"].ConnectionString);
            cnn.Open();

            XmlDocument doc = new XmlDocument();
            doc.Load("C:\\Development\\NZOR\\Dev\\NZOR\\Integration\\Configuration\\IntegConfig.xml");

            DsIntegrationName.ProviderNameRow pn = NZOR.Data.ProviderName.GetNameMatchData(cnn, new Guid("88441283-026F-4EB2-9925-00556C4D2ABE"));

            NZOR.Integration.MatchProcessor.LoadConfig(doc);

            NZOR.Integration.ConfigSet cs = NZOR.Integration.MatchProcessor.GetMatchSet(1);
            NZOR.Data.MatchResult res = NZOR.Integration.MatchProcessor.DoMatch(pn, cs.Routines, true, cnn);

            cnn.Close();
            
        }

        private void integButton_Click(object sender, EventArgs e)
        {
            SqlConnection cnn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["NZOR"].ConnectionString);
            cnn.Open();

            XmlDocument doc = new XmlDocument();
            doc.Load("C:\\Development\\NZOR\\Dev\\NZOR\\Integration\\Configuration\\IntegConfig.xml");

            DsIntegrationName.ProviderNameRow pn = NZOR.Data.ProviderName.GetNameMatchData(cnn, new Guid("88441283-026F-4EB2-9925-00556C4D2ABE"));

            NZOR.Integration.IntegrationProcessor.RunIntegration(doc, 1);

            cnn.Close();
        }
    }
}
