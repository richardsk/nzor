using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace NZORConsumer
{
    public partial class ProviderNameResultsForm : Form
    {
        public ProviderNameResultsForm()
        {
            InitializeComponent();
        }

        public void InitialiseForm(string brokeredNamesXml, string requestName)
        {
            providerNamesCtrl1.Initialise(brokeredNamesXml, requestName);
        }
                
        private void closeButton_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.OK;
        }

    }
}
