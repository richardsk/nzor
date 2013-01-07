namespace NZORConsumer
{
    partial class ConsumerForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConsumerForm));
            this.ObservationTab = new System.Windows.Forms.TabControl();
            this.setupPage = new System.Windows.Forms.TabPage();
            this.setupCtrl1 = new NZORConsumer.SetupCtrl();
            this.searchPage = new System.Windows.Forms.TabPage();
            this.searchCtrl1 = new NZORConsumer.SearchCtrl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.observationCtrl1 = new NZORConsumer.Controls.ObservationCtrl();
            this.batchMatchTab = new System.Windows.Forms.TabPage();
            this.processingInfoText = new System.Windows.Forms.WebBrowser();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.batchMatchCtrl1 = new NZORConsumer.Controls.BatchMatchCtrl();
            this.ObservationTab.SuspendLayout();
            this.setupPage.SuspendLayout();
            this.searchPage.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.batchMatchTab.SuspendLayout();
            this.SuspendLayout();
            // 
            // ObservationTab
            // 
            this.ObservationTab.Controls.Add(this.setupPage);
            this.ObservationTab.Controls.Add(this.searchPage);
            this.ObservationTab.Controls.Add(this.tabPage1);
            this.ObservationTab.Controls.Add(this.batchMatchTab);
            this.ObservationTab.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ObservationTab.Location = new System.Drawing.Point(0, 0);
            this.ObservationTab.Name = "ObservationTab";
            this.ObservationTab.SelectedIndex = 0;
            this.ObservationTab.Size = new System.Drawing.Size(814, 459);
            this.ObservationTab.TabIndex = 0;
            // 
            // setupPage
            // 
            this.setupPage.Controls.Add(this.setupCtrl1);
            this.setupPage.Location = new System.Drawing.Point(4, 22);
            this.setupPage.Name = "setupPage";
            this.setupPage.Padding = new System.Windows.Forms.Padding(3);
            this.setupPage.Size = new System.Drawing.Size(806, 433);
            this.setupPage.TabIndex = 0;
            this.setupPage.Text = "Setup";
            this.setupPage.UseVisualStyleBackColor = true;
            // 
            // setupCtrl1
            // 
            this.setupCtrl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.setupCtrl1.Location = new System.Drawing.Point(3, 3);
            this.setupCtrl1.Name = "setupCtrl1";
            this.setupCtrl1.Size = new System.Drawing.Size(800, 427);
            this.setupCtrl1.TabIndex = 0;
            // 
            // searchPage
            // 
            this.searchPage.Controls.Add(this.searchCtrl1);
            this.searchPage.Location = new System.Drawing.Point(4, 22);
            this.searchPage.Name = "searchPage";
            this.searchPage.Padding = new System.Windows.Forms.Padding(3);
            this.searchPage.Size = new System.Drawing.Size(806, 433);
            this.searchPage.TabIndex = 1;
            this.searchPage.Text = "Search";
            this.searchPage.UseVisualStyleBackColor = true;
            // 
            // searchCtrl1
            // 
            this.searchCtrl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.searchCtrl1.Location = new System.Drawing.Point(3, 3);
            this.searchCtrl1.Name = "searchCtrl1";
            this.searchCtrl1.Size = new System.Drawing.Size(800, 427);
            this.searchCtrl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.observationCtrl1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(806, 433);
            this.tabPage1.TabIndex = 2;
            this.tabPage1.Text = "Observations";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // observationCtrl1
            // 
            this.observationCtrl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.observationCtrl1.Location = new System.Drawing.Point(3, 3);
            this.observationCtrl1.Name = "observationCtrl1";
            this.observationCtrl1.Size = new System.Drawing.Size(800, 427);
            this.observationCtrl1.TabIndex = 0;
            // 
            // batchMatchTab
            // 
            this.batchMatchTab.Controls.Add(this.batchMatchCtrl1);
            this.batchMatchTab.Location = new System.Drawing.Point(4, 22);
            this.batchMatchTab.Name = "batchMatchTab";
            this.batchMatchTab.Padding = new System.Windows.Forms.Padding(3);
            this.batchMatchTab.Size = new System.Drawing.Size(806, 433);
            this.batchMatchTab.TabIndex = 3;
            this.batchMatchTab.Text = "Batch Matches";
            this.batchMatchTab.UseVisualStyleBackColor = true;
            // 
            // processingInfoText
            // 
            this.processingInfoText.AllowWebBrowserDrop = false;
            this.processingInfoText.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.processingInfoText.Location = new System.Drawing.Point(0, 459);
            this.processingInfoText.Name = "processingInfoText";
            this.processingInfoText.Size = new System.Drawing.Size(814, 141);
            this.processingInfoText.TabIndex = 1;
            this.processingInfoText.WebBrowserShortcutsEnabled = false;
            this.processingInfoText.Navigating += new System.Windows.Forms.WebBrowserNavigatingEventHandler(this.processingInfoText_Navigating);
            // 
            // splitter1
            // 
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.splitter1.Location = new System.Drawing.Point(0, 456);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(814, 3);
            this.splitter1.TabIndex = 2;
            this.splitter1.TabStop = false;
            // 
            // batchMatchCtrl1
            // 
            this.batchMatchCtrl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.batchMatchCtrl1.Location = new System.Drawing.Point(3, 3);
            this.batchMatchCtrl1.Name = "batchMatchCtrl1";
            this.batchMatchCtrl1.Size = new System.Drawing.Size(800, 427);
            this.batchMatchCtrl1.TabIndex = 0;
            // 
            // ConsumerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(814, 600);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.ObservationTab);
            this.Controls.Add(this.processingInfoText);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ConsumerForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "NZOR Consumer";
            this.Load += new System.EventHandler(this.ConsumerForm_Load);
            this.ObservationTab.ResumeLayout(false);
            this.setupPage.ResumeLayout(false);
            this.searchPage.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.batchMatchTab.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl ObservationTab;
        private System.Windows.Forms.TabPage setupPage;
        private System.Windows.Forms.TabPage searchPage;
        private SearchCtrl searchCtrl1;
        private SetupCtrl setupCtrl1;
        private System.Windows.Forms.WebBrowser processingInfoText;
        private System.Windows.Forms.TabPage tabPage1;
        private Controls.ObservationCtrl observationCtrl1;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.TabPage batchMatchTab;
        private Controls.BatchMatchCtrl batchMatchCtrl1;
    }
}

