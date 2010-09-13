using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;

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

            XmlDocument doc = new XmlDocument();
            doc.Load("C:\\Development\\NZOR\\Dev\\NZOR\\Integration\\Configuration\\IntegConfig.xml");

            DataSet pn = NZOR.Data.ProviderName.GetNameMatchData(new Guid("88441283-026F-4EB2-9925-00556C4D2ABE"));

            List<NZOR.Matching.INameMatcher> routines = NZOR.Integration.Integrator.LoadConfig(doc, 1);
            NZOR.Data.MatchResult res = NZOR.Integration.Integrator.DoMatch(pn, routines);
            
        }

        private void integButton_Click(object sender, EventArgs e)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load("C:\\Development\\NZOR\\Dev\\NZOR\\Integration\\Configuration\\IntegConfig.xml");

            DataSet pn = NZOR.Data.ProviderName.GetNameMatchData(new Guid("88441283-026F-4EB2-9925-00556C4D2ABE"));

            NZOR.Integration.IntegrationProcessor proc = new NZOR.Integration.IntegrationProcessor();
            proc.RunIntegration(doc, 1);
        }
    }
}
