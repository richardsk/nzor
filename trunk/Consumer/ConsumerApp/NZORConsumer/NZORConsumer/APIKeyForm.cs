using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace NZORConsumer
{
    public partial class APIKeyForm : Form
    {
        public APIKeyForm()
        {
            InitializeComponent();
        }
        
        private void apiLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(System.Configuration.ConfigurationManager.AppSettings["NZORAPIKeyUrl"]);
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            if (apiKeyText.Text.Length > 0)
            {
                Settings.APIKey = apiKeyText.Text;
                DialogResult = System.Windows.Forms.DialogResult.OK;
            }
        }
    }
}
