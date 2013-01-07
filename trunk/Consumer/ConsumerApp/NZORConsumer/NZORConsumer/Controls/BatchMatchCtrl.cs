using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Diagnostics;

namespace NZORConsumer.Controls
{
    public partial class BatchMatchCtrl : UserControl
    {
        public event ProcessingMessage Message;

        Model.ConsumerEntities _model = null;

        public BatchMatchCtrl()
        {
            InitializeComponent();
        }

        public void Initialise(Model.ConsumerEntities model)
        {
            _model = model;

            batchResultsGrid.DataSource = _model.BatchMatches.ToList();
            nameRequestsGrid.DataSource = _model.NameRequests.ToList();
        }

        private void submitButton_Click(object sender, EventArgs e)
        {
            if (batchFileText.Text.Length > 0)
            {
                Cursor.Current = Cursors.WaitCursor;

                try
                {
                    NZORConsumer.Model.ConsumerEntities ce = new NZORConsumer.Model.ConsumerEntities();
                    Model.Harvest h = ce.Harvests.FirstOrDefault();

                    NZORServiceMessage msg = null;
                    Model.BatchMatch bm = ConsumerClient.SubmitBatchMatch(h.ServiceUrl, batchFileText.Text, brokerCheck.Checked, out msg);

                    if (bm != null)
                    {
                        ce.BatchMatches.AddObject(bm);
                        ce.SaveChanges();

                        RefreshGrids();                    }

                    if (Message != null) Message(msg, false);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "System error");
                } 
                
                Cursor.Current = Cursors.Default;
            }

        }


        private void chooseFileButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.DefaultExt = "csv";
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                batchFileText.Text = ofd.FileName;
            }
        }


        private void pollButton_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            try
            {
                NZORConsumer.Model.ConsumerEntities ce = new NZORConsumer.Model.ConsumerEntities();
                Model.Harvest h = ce.Harvests.FirstOrDefault();

                NZORServiceMessage msg = null;

                foreach (Model.BatchMatch bm in _model.BatchMatches)
                {
                    if (bm.Status != "Completed")
                    {
                        Model.BatchMatch bmResult = ConsumerClient.GetBatchMatch(h.ServiceUrl, bm.MatchId, out msg);

                        bm.DownloadUrl = bmResult.DownloadUrl;
                        bm.Status = bmResult.Status;

                        if (bm.Status == "Completed")
                        {
                            WebRequest req = HttpWebRequest.Create(bmResult.DownloadUrl);
                            WebResponse resp = req.GetResponse();

                            StreamReader rdr = new StreamReader(resp.GetResponseStream());
                            string data = rdr.ReadToEnd();
                            rdr.Close();

                            string fname = "Batches\\" + bm.MatchId + ".csv";
                            bm.ResultsFile = fname;

                            File.WriteAllText(fname, data);
                        }
                    }
                }
                _model.SaveChanges();
                
                List<Model.NameRequest> requests = ConsumerClient.GetNameRequests(h.ServiceUrl, out msg);
                foreach (Model.NameRequest nr in _model.NameRequests) _model.NameRequests.DeleteObject(nr);
                _model.SaveChanges();

                foreach (Model.NameRequest nr in requests)
                {
                     _model.NameRequests.AddObject(nr);                    
                }
                _model.SaveChanges();

                RefreshGrids();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "System error");
            }

            Cursor.Current = Cursors.Default;
        }

        private void RefreshGrids()
        {
            batchResultsGrid.DataSource = _model.BatchMatches.ToList();
            nameRequestsGrid.DataSource = _model.NameRequests.ToList();
        }

        private void nameRequestsGrid_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (nameRequestsGrid.Columns[e.ColumnIndex].Name == ProvNameResCol.Name && nameRequestsGrid[e.ColumnIndex, e.RowIndex].Value.ToString() != "")
            {
                ProviderNameResultsForm fm = new ProviderNameResultsForm();

                string xml = nameRequestsGrid[ProvNameResCol.Name, e.RowIndex].Value.ToString();
                string name = nameRequestsGrid[FullNameCol.Name, e.RowIndex].Value.ToString();
                fm.InitialiseForm(xml, name);
                fm.ShowDialog();
            }
        }

        private void batchResultsGrid_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if ((batchResultsGrid.Columns[e.ColumnIndex].Name == UrlCol.Name || batchResultsGrid.Columns[e.ColumnIndex].Name == ResulsFileCol.Name)
                 && batchResultsGrid[e.ColumnIndex, e.RowIndex].Value.ToString() != "")
            {
                try
                {
                    Process.Start(batchResultsGrid[e.ColumnIndex, e.RowIndex].Value.ToString());
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

        }


    }
}
