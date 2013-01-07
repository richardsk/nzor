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
    public partial class NameSearchForm : Form
    {
        public string SelectedNameId = "";
        public string SelectedFullName = "";

        public NameSearchForm()
        {
            InitializeComponent();
        }

        private void NameSearchForm_Load(object sender, EventArgs e)
        {

        }

        public void Initialise(Model.ConsumerEntities ce)
        {
            searchCtrl1.Initialise(ce);

        }

        private void SelectButton_Click(object sender, EventArgs e)
        {
            SelectedNameId = searchCtrl1.SelectedNameId;
            SelectedFullName = searchCtrl1.SelectedFullName;
            DialogResult = System.Windows.Forms.DialogResult.OK;
        }
    }
}
