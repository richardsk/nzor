using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace NZORConsumer
{
    public partial class SetupCtrl : UserControl
    {
        public event ProcessingMessage Message;

        private Harvest.HarvestProcessor _processor = new Harvest.HarvestProcessor(); 
        private System.Timers.Timer _timer = new System.Timers.Timer();


        public SetupCtrl()
        {
            InitializeComponent();
        }

        public void Initialise()
        {
            _processor.Message += new ProcessingMessage(_processor_Message);
            
            Model.ConsumerEntities ce = new Model.ConsumerEntities();
            DisplaySetup(ce);
        }

        private void DisplaySetup(Model.ConsumerEntities ce)
        {
            Model.Harvest harvest = ce.Harvests.First();

            apiKeyText.Text = Settings.APIKey;
            if (apiKeyText.Text.Length > 0) apiKeyLink.Visible = false;

            urlText.Text = harvest.ServiceUrl;
            harvestIntervalText.Text = harvest.IntervalDays.ToString();
            lastHarvestText.Text = harvest.LastHarvest.ToString();

            string errMsg = "";
            nameCountText.Text = Data.ConsumerData.NameCount(ce, out errMsg).ToString();

            DisplayStartingTaxa(ce);

            DisplayBiostatus(ce);

            UpdateHarvestStatus();
        }

        void _processor_Message(NZORServiceMessage message, bool append)
        {
            if (Message != null) Message(message, append);
        }

        void addBSForm_Message(NZORServiceMessage message, bool append)
        {
            if (Message != null) Message(message, append);
        }

        private void DisplayStartingTaxa(Model.ConsumerEntities ce)
        {
            startingTaxaList.Items.Clear();
            startingTaxaList.DisplayMember = "FullName";
            //just show all names as we only have one harvest setup
            foreach (Model.HarvestScopeName hsn in ce.HarvestScopeNames)
            {
                startingTaxaList.Items.Add(hsn);
            }
        }

        private void DisplayBiostatus(Model.ConsumerEntities ce)
        {
            Model.BiostatusOption bo = ce.BiostatusOptions.FirstOrDefault();
            presentCheck.Checked = false;
            //absentCheck.Checked = false;
            uncertainCheck.Checked = false;
            if (bo != null)
            {
                if (bo.Present != null && bo.Present.Value) presentCheck.Checked = true;
                //if (bo.Absent != null && bo.Absent.Value) absentCheck.Checked = true;
                if (bo.Uncertain != null && bo.Uncertain.Value) uncertainCheck.Checked = true;
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string errMsg = "";
            NZORServiceMessage procMessage = null;
            bool ok = ConsumerClient.Ping(urlText.Text, out errMsg, out procMessage);

            if (Message != null) Message(procMessage, false);

            if (ok)
            {
                MessageBox.Show("Successful ping");
            }
            else
            {
                MessageBox.Show("Failed to ping web service : " + errMsg);
            }
        }
        

        private void addTaxonLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            NameSearchForm nsf = new NameSearchForm();
            Model.ConsumerEntities ce = new Model.ConsumerEntities();
            nsf.Initialise(ce);
            if (nsf.ShowDialog() == DialogResult.OK)
            {
                Model.HarvestScope hs = ce.HarvestScopes.First();
                Model.HarvestScopeName hsn = new Model.HarvestScopeName();
                hsn.HarvestScopeId = hs.HarvestScopeId;
                hsn.NZORNameId = nsf.SelectedNameId;
                hsn.FullName = nsf.SelectedFullName;

                ce.HarvestScopeNames.AddObject(hsn);
                ce.SaveChanges();

                ce = new Model.ConsumerEntities();
                DisplaySetup(ce);
            }
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            try
            {
                int intDays = 0;
                if (harvestIntervalText.Text == "" || !int.TryParse(harvestIntervalText.Text, out intDays) || intDays < 1 || intDays > 100)
                {
                    errorProvider1.SetError(harvestIntervalText, "Interval is not valid.  It must be a whole number between 1 and 100.");
                }
                Model.ConsumerEntities ce = new Model.ConsumerEntities();
                Model.Harvest h = ce.Harvests.First();
                h.IntervalDays = intDays;
                //reset harvest date
                h.LastHarvest = null;

                ce.HarvestScopes.First().AcceptedNamesOnly = acceptedNameCheck.Checked;

                ce.SaveChanges();

                Settings.APIKey = apiKeyText.Text;

                ce = new Model.ConsumerEntities();
                DisplaySetup(ce);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving harvest data : " + ex.Message);
            }
        }

        private void startStopLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (_processor.Status == Harvest.HarvestProcessor.HarvestStatus.NotRunning)
            {
                _timer.Interval = 2000;
                _timer.AutoReset = false;
                _timer.Elapsed += new System.Timers.ElapsedEventHandler(_timer_Elapsed);

                _processor.Start(); //resumeCheck.Checked);
                startStopLink.Text = "Stop harvester";

                _timer.Start();
            }
            else
            {
                _processor.Stop();
                _timer.Stop();
                startStopLink.Text = "Start harvester";
            }


            Model.ConsumerEntities ce = new Model.ConsumerEntities();
            DisplaySetup(ce);
        }

        public void UpdateHarvestStatus()
        {
            string lastHarvest = "none";
            string nextHarvest = _processor.Status == Harvest.HarvestProcessor.HarvestStatus.Running ? "now" : "";
            Model.ConsumerEntities ce = new Model.ConsumerEntities();
            Model.Harvest h = ce.Harvests.First();
            if (h.LastHarvest != null) 
            {
                lastHarvest = h.LastHarvest.Value.ToString();
                nextHarvest = h.LastHarvest.Value.AddDays(h.IntervalDays.Value).ToString();
                lastHarvestText.Text = h.LastHarvest.ToString();
            }

            string errMsg = "";
            harvestStatusText.Text = _processor.Status.ToString() + ", " + _processor.Progress.ToString() + "% done.  Last harvest: " + lastHarvest + ". Next harvest: " + nextHarvest + ".";
            
            if (_processor.Status == Harvest.HarvestProcessor.HarvestStatus.Running)
            {
                harvestStatusText.Text += Environment.NewLine + _processor.StatusMessage;
                    //"Harvested " + _processor.TotalDone.ToString() + " of " + _processor.TotalToHarvest.ToString() + " names.";
            }

            int nCount = Data.ConsumerData.NameCount(ce, out errMsg);
            nameCountText.Text = nCount.ToString();

            if (_processor.Status == Harvest.HarvestProcessor.HarvestStatus.Running)
            {
                startStopLink.Text = "Stop harvester";
            }
            else
            {
                startStopLink.Text = "Start harvester";
            }
        }

        private void _timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            MethodInvoker mi = new MethodInvoker(UpdateHarvestStatus);
            this.Invoke(mi);
            _timer.Start();
        }

        private void apiKeyLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(System.Configuration.ConfigurationManager.AppSettings["NZORAPIKeyUrl"]);
        }

        private void removeTaxonLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (startingTaxaList.SelectedItem != null)
            {
                Model.ConsumerEntities ce = new Model.ConsumerEntities();
                Model.HarvestScopeName selName = ce.HarvestScopeNames.Where(hsn => hsn.HarvestScopeNameId == ((Model.HarvestScopeName)startingTaxaList.SelectedItem).HarvestScopeNameId).FirstOrDefault();
                ce.DeleteObject(selName);
                ce.SaveChanges();

                ce = new Model.ConsumerEntities();
                DisplaySetup(ce);
            }
        }

        private void clearCacheLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (_processor.Status == Harvest.HarvestProcessor.HarvestStatus.NotRunning || _processor.Progress == 100)
            {
                if (MessageBox.Show("Are you sure?", "Clear all NZOR data from local cache", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    Cursor.Current = Cursors.WaitCursor;
                    Model.ConsumerEntities ce = new Model.ConsumerEntities();
                    foreach (Model.Provider p in ce.Providers)
                    {
                        ce.DeleteObject(p);
                    }
                    foreach (Model.NameProvider np in ce.NameProviders)
                    {
                        ce.DeleteObject(np);
                    }
                    foreach (Model.Name n in ce.Names)
                    {
                        ce.DeleteObject(n);
                    }
                    Model.Harvest h = ce.Harvests.First();
                    h.LastHarvest = null;

                    ce.SaveChanges();

                    ce = new Model.ConsumerEntities();
                    DisplaySetup(ce);

                    Cursor.Current = Cursors.Default;
                }
            }
            else
            {
                System.Media.SystemSounds.Beep.Play();
            }
        }

        private void urlText_Leave(object sender, EventArgs e)
        {
            Model.ConsumerEntities ce = new Model.ConsumerEntities();
            Model.Harvest h = ce.Harvests.First();
            if (h.ServiceUrl != urlText.Text)
            {
                h.ServiceUrl = urlText.Text;
                ce.SaveChanges();
            }
        }

        private void apiKeyText_Leave(object sender, EventArgs e)
        {
            Settings.APIKey = apiKeyText.Text;
        }

        private void harvestIntervalText_Leave(object sender, EventArgs e)
        {
            Model.ConsumerEntities ce = new Model.ConsumerEntities();
            Model.Harvest h = ce.Harvests.First();
            int days = 2;
            int.TryParse(harvestIntervalText.Text, out days);            
            if (h.IntervalDays != days)
            {
                h.IntervalDays = days;
                ce.SaveChanges();
            }
        }

        private void acceptedNameCheck_CheckedChanged(object sender, EventArgs e)
        {
            Model.ConsumerEntities ce = new Model.ConsumerEntities();
            Model.HarvestScope h = ce.HarvestScopes.First();
            if (h.AcceptedNamesOnly != acceptedNameCheck.Checked)
            {
                h.AcceptedNamesOnly = acceptedNameCheck.Checked;
                ce.SaveChanges();
            }
        }

        private void presentCheck_CheckedChanged(object sender, EventArgs e)
        {
            SaveBiostatus();
        }

        private void absentCheck_CheckedChanged(object sender, EventArgs e)
        {
            SaveBiostatus();
        }

        private void uncertainCheck_CheckedChanged(object sender, EventArgs e)
        {
            SaveBiostatus();
        }

        private void SaveBiostatus()
        {
            Model.ConsumerEntities ce = new Model.ConsumerEntities();
            Model.BiostatusOption bo = ce.BiostatusOptions.FirstOrDefault();
            if (bo == null)
            {
                bo = new Model.BiostatusOption();
                ce.BiostatusOptions.AddObject(bo);
            }
            bo.Present = presentCheck.Checked;
            //bo.Absent = absentCheck.Checked;
            bo.Uncertain = uncertainCheck.Checked;
            ce.SaveChanges();
        }

        private void errLogLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string logFile = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "NZOR\\log.txt");
            if (System.IO.File.Exists(logFile))
            {
                Process.Start(logFile);
            }
            else
            {
                MessageBox.Show("No log file exists");
            }
        }

    }
}
