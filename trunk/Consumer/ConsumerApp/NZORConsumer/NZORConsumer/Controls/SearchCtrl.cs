using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace NZORConsumer
{
    public partial class SearchCtrl : UserControl
    {
        public event ProcessingMessage Message;

        public string SelectedNameId = "";
        public string SelectedFullName = "";

        private string _serviceUrl = "";
        private Model.ConsumerEntities _model = null;

        public SearchCtrl()
        {
            InitializeComponent();

            resultsGrid.AutoGenerateColumns = false;
        }

        public void Initialise(Model.ConsumerEntities ce)
        {
            _model = ce;
            _serviceUrl = _model.Harvests.First().ServiceUrl;
            messageLabel.Text = "";            
        }

        private void searchButton_Click(object sender, EventArgs e)
        {
            if (searchText.Text.Length > 0)
            {
                errorProvider1.Clear();
                resultsGrid.ClearSelection();
                messageLabel.Text = "";

                Cursor.Current = Cursors.WaitCursor;

                int total = 0;

                if (searchLocalCheck.Checked)
                {
                    string errMsg = "";
                    resultsGrid.DataSource = Data.ConsumerData.SearchNames(_model, searchText.Text, 200, out total, out errMsg);
                    if (errMsg != "") messageLabel.Text = errMsg;
                }
                else
                {
                    NZORServiceMessage processMessage = null;
                    resultsGrid.DataSource = ConsumerClient.Search(searchText.Text, _serviceUrl, 200, out total, out processMessage).Names;
                    
                    if (Message != null) Message(processMessage, false);
                }

                int cnt = Math.Min(200, total);
                if (messageLabel.Text == "") messageLabel.Text = "Showing " + cnt.ToString() + " of " + total.ToString() + " results";

                Cursor.Current = Cursors.Default;
            }
            else
            {
                errorProvider1.SetIconAlignment(searchText, ErrorIconAlignment.TopLeft);
                errorProvider1.SetError(searchText, "No search text specified");
            }
        }

        private void searchText_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                searchButton_Click(this, new EventArgs());
            }
        }

        private void resultsGrid_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                Model.Name tn = (Model.Name)(resultsGrid.SelectedRows[0].DataBoundItem);
                SelectedNameId = tn.NZORId;
                SelectedFullName = tn.FullName;
            }
            catch (Exception )
            {
            }
        }


    }
}
