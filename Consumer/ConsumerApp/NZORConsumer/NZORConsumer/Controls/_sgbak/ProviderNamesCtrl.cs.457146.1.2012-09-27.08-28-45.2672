using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Diagnostics;

namespace NZORConsumer.Controls
{
    public partial class ProviderNamesCtrl : UserControl
    {
        private class BrokeredName
        {
            public string FullName { get; set; }
            public string DataSource { get; set; }
            public string ProvNameSource { get; set; }
            public string ProvNameStatus { get; set; }
        }

        public ProviderNamesCtrl()
        {
            InitializeComponent();
        }

        public void Initialise(string brokeredNamesXml, string requestName)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(brokeredNamesXml);

            List<BrokeredName> bnList = new List<BrokeredName>();

            XmlNodeList bnNodes = doc.SelectNodes("//BrokeredName");
            foreach (XmlNode bnNode in bnNodes)
            {
                BrokeredName bn = new BrokeredName();
                bn.DataSource = bnNode.SelectSingleNode("DataSource").InnerText;
                bn.FullName = bnNode.SelectSingleNode("FullName").InnerText;
                bn.ProvNameSource = bnNode.SelectSingleNode("ProviderNameSource").InnerText;
                if (bnNode.SelectSingleNode("ProviderNameStatus") != null) bn.ProvNameStatus = bnNode.SelectSingleNode("ProviderNameStatus").InnerText;

                bnList.Add(bn);
            }

            provNamesGrid.DataSource = bnList;

            titleLabel.Text += requestName;
        }

        private void provNamesGrid_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (provNamesGrid.Columns[e.ColumnIndex].Name == SourceCol.Name)
            {
                try
                {
                    Process.Start(provNamesGrid[e.ColumnIndex, e.RowIndex].Value.ToString());
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

    }
}
