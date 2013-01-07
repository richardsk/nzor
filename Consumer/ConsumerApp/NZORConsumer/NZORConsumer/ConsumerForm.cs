using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Configuration;
using System.IO;

namespace NZORConsumer
{
    public partial class ConsumerForm : Form
    {
        private delegate void UpdateMessageDelegate(NZORServiceMessage message, bool append);
        private UpdateMessageDelegate updateDelegate = null;

        private Dictionary<Guid, string> _xmlResults = new Dictionary<Guid, string>();
        
        public ConsumerForm()
        {
            InitializeComponent();

            updateDelegate = new UpdateMessageDelegate(UpdateMessage);
        }

        private void ConsumerForm_Load(object sender, EventArgs e)
        {
            try
            {
                this.Text = "NZOR Consumer " + Application.ProductVersion;
                searchCtrl1.Message += new ProcessingMessage(childCtrl_Message);
                setupCtrl1.Message += new ProcessingMessage(childCtrl_Message);
                observationCtrl1.Message += new ProcessingMessage(childCtrl_Message);
                batchMatchCtrl1.Message += new ProcessingMessage(childCtrl_Message);

                string errMsg = "";
                NZORConsumer.Model.ConsumerEntities ce = new NZORConsumer.Model.ConsumerEntities();
                Data.ConsumerData.CreateDefaultHarvest(ce, out errMsg);

                //api key been set?
                if (Settings.APIKey == null)
                {
                    APIKeyForm akf = new APIKeyForm();
                    akf.ShowDialog();
                }

                setupCtrl1.Initialise();
                searchCtrl1.Initialise(ce);
                observationCtrl1.Initialise(ce);
                batchMatchCtrl1.Initialise(ce);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading data.  " + ex.Message);
            }
        }

        public void UpdateMessage(NZORServiceMessage message, bool append)
        {
            try
            {
                if (message != null)
                {
                    Guid id = Guid.NewGuid();
                    if (!append) _xmlResults.Clear();
                    _xmlResults.Add(id, message.XmlResult);

                    string html = "";

                    if (!append || processingInfoText.DocumentText.Length == 0)
                    {
                        html = "<html>";
                    }
                    else
                    {
                        html = processingInfoText.DocumentText.Substring(0, processingInfoText.DocumentText.Length - 7);
                    }

                    html += "<p>";
                    foreach (string url in message.Urls)
                    {
                        html += url + " <a href='" + url + "' target='_blank'>Show in browser</a><br/>";
                    }

                    if (message.XmlResult.Length > 0)
                    {
                        html += "<a href='" + id.ToString() + "'>XML result (length=" + message.XmlResult.Length.ToString() + ")</a>";
                    }

                    html += "</p>";

                    if (message.Error != "") html += "<p style='color:red'>" + message.Error + "</p>";

                    html += "</html>";

                    processingInfoText.DocumentText = html;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "System error");
            }
        }

        void childCtrl_Message(NZORServiceMessage message, bool append)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(updateDelegate, new object[] { message, append });
            }
            else
            {
                UpdateMessage(message, append);
            }
        }

        private void processingInfoText_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            string xml = "no result";
            try
            {
                xml = _xmlResults[new Guid(e.Url.PathAndQuery)];

                Cursor.Current = Cursors.WaitCursor;

                XmlResultForm xrf = new XmlResultForm();
                xrf.XmlText = xml;
                xrf.ShowDialog();

                e.Cancel = true;
            }
            catch (Exception )
            {
            }

            Cursor.Current = Cursors.Default;
        }


    }
}
