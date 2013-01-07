namespace NZORConsumer
{
    partial class SetupCtrl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.urlText = new System.Windows.Forms.TextBox();
            this.harvestIntervalText = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.uncertainCheck = new System.Windows.Forms.CheckBox();
            this.presentCheck = new System.Windows.Forms.CheckBox();
            this.startingTaxaList = new System.Windows.Forms.ListBox();
            this.removeTaxonLink = new System.Windows.Forms.LinkLabel();
            this.label5 = new System.Windows.Forms.Label();
            this.acceptedNameCheck = new System.Windows.Forms.CheckBox();
            this.addTaxonLink = new System.Windows.Forms.LinkLabel();
            this.label4 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.lastHarvestText = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.harvestStatusText = new System.Windows.Forms.TextBox();
            this.startStopLink = new System.Windows.Forms.LinkLabel();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.label3 = new System.Windows.Forms.Label();
            this.apiKeyText = new System.Windows.Forms.TextBox();
            this.apiKeyLink = new System.Windows.Forms.LinkLabel();
            this.clearCacheLink = new System.Windows.Forms.LinkLabel();
            this.nameCountText = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.errLogLink = new System.Windows.Forms.LinkLabel();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(5, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "NZOR service URL";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(5, 58);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(112, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Harvest interval (days)";
            // 
            // linkLabel1
            // 
            this.linkLabel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Location = new System.Drawing.Point(515, 29);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(67, 13);
            this.linkLabel1.TabIndex = 2;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "Ping Service";
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // urlText
            // 
            this.urlText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.urlText.Location = new System.Drawing.Point(123, 29);
            this.urlText.Name = "urlText";
            this.urlText.Size = new System.Drawing.Size(386, 20);
            this.urlText.TabIndex = 1;
            this.urlText.Leave += new System.EventHandler(this.urlText_Leave);
            // 
            // harvestIntervalText
            // 
            this.harvestIntervalText.Location = new System.Drawing.Point(123, 55);
            this.harvestIntervalText.Name = "harvestIntervalText";
            this.harvestIntervalText.Size = new System.Drawing.Size(100, 20);
            this.harvestIntervalText.TabIndex = 4;
            this.harvestIntervalText.Leave += new System.EventHandler(this.harvestIntervalText_Leave);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.uncertainCheck);
            this.groupBox1.Controls.Add(this.presentCheck);
            this.groupBox1.Controls.Add(this.startingTaxaList);
            this.groupBox1.Controls.Add(this.removeTaxonLink);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.acceptedNameCheck);
            this.groupBox1.Controls.Add(this.addTaxonLink);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Location = new System.Drawing.Point(5, 97);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(583, 191);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Scope of interest";
            // 
            // uncertainCheck
            // 
            this.uncertainCheck.AutoSize = true;
            this.uncertainCheck.Location = new System.Drawing.Point(120, 156);
            this.uncertainCheck.Name = "uncertainCheck";
            this.uncertainCheck.Size = new System.Drawing.Size(72, 17);
            this.uncertainCheck.TabIndex = 15;
            this.uncertainCheck.Text = "Uncertain";
            this.uncertainCheck.UseVisualStyleBackColor = true;
            this.uncertainCheck.CheckedChanged += new System.EventHandler(this.uncertainCheck_CheckedChanged);
            // 
            // presentCheck
            // 
            this.presentCheck.AutoSize = true;
            this.presentCheck.Location = new System.Drawing.Point(120, 133);
            this.presentCheck.Name = "presentCheck";
            this.presentCheck.Size = new System.Drawing.Size(62, 17);
            this.presentCheck.TabIndex = 13;
            this.presentCheck.Text = "Present";
            this.presentCheck.UseVisualStyleBackColor = true;
            this.presentCheck.CheckedChanged += new System.EventHandler(this.presentCheck_CheckedChanged);
            // 
            // startingTaxaList
            // 
            this.startingTaxaList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.startingTaxaList.FormattingEnabled = true;
            this.startingTaxaList.Location = new System.Drawing.Point(118, 32);
            this.startingTaxaList.Name = "startingTaxaList";
            this.startingTaxaList.Size = new System.Drawing.Size(343, 69);
            this.startingTaxaList.TabIndex = 12;
            // 
            // removeTaxonLink
            // 
            this.removeTaxonLink.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.removeTaxonLink.AutoSize = true;
            this.removeTaxonLink.Location = new System.Drawing.Point(468, 51);
            this.removeTaxonLink.Name = "removeTaxonLink";
            this.removeTaxonLink.Size = new System.Drawing.Size(47, 13);
            this.removeTaxonLink.TabIndex = 10;
            this.removeTaxonLink.TabStop = true;
            this.removeTaxonLink.Text = "Remove";
            this.removeTaxonLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.removeTaxonLink_LinkClicked);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 134);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(100, 13);
            this.label5.TabIndex = 6;
            this.label5.Text = "Restrict to biostatus";
            // 
            // acceptedNameCheck
            // 
            this.acceptedNameCheck.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.acceptedNameCheck.Location = new System.Drawing.Point(6, 104);
            this.acceptedNameCheck.Name = "acceptedNameCheck";
            this.acceptedNameCheck.Size = new System.Drawing.Size(129, 24);
            this.acceptedNameCheck.TabIndex = 5;
            this.acceptedNameCheck.Text = "Accepted names only";
            this.acceptedNameCheck.UseVisualStyleBackColor = true;
            this.acceptedNameCheck.CheckedChanged += new System.EventHandler(this.acceptedNameCheck_CheckedChanged);
            // 
            // addTaxonLink
            // 
            this.addTaxonLink.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.addTaxonLink.AutoSize = true;
            this.addTaxonLink.Location = new System.Drawing.Point(467, 35);
            this.addTaxonLink.Name = "addTaxonLink";
            this.addTaxonLink.Size = new System.Drawing.Size(26, 13);
            this.addTaxonLink.TabIndex = 4;
            this.addTaxonLink.TabStop = true;
            this.addTaxonLink.Text = "Add";
            this.addTaxonLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.addTaxonLink_LinkClicked);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 32);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(62, 13);
            this.label4.TabIndex = 2;
            this.label4.Text = "Taxa below";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(229, 58);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(65, 13);
            this.label6.TabIndex = 6;
            this.label6.Text = "Last harvest";
            // 
            // lastHarvestText
            // 
            this.lastHarvestText.Location = new System.Drawing.Point(301, 58);
            this.lastHarvestText.Name = "lastHarvestText";
            this.lastHarvestText.ReadOnly = true;
            this.lastHarvestText.Size = new System.Drawing.Size(146, 20);
            this.lastHarvestText.TabIndex = 7;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(8, 297);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(75, 13);
            this.label7.TabIndex = 8;
            this.label7.Text = "Harvest status";
            // 
            // harvestStatusText
            // 
            this.harvestStatusText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.harvestStatusText.Location = new System.Drawing.Point(123, 294);
            this.harvestStatusText.Multiline = true;
            this.harvestStatusText.Name = "harvestStatusText";
            this.harvestStatusText.ReadOnly = true;
            this.harvestStatusText.Size = new System.Drawing.Size(363, 71);
            this.harvestStatusText.TabIndex = 9;
            // 
            // startStopLink
            // 
            this.startStopLink.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.startStopLink.AutoSize = true;
            this.startStopLink.Location = new System.Drawing.Point(492, 297);
            this.startStopLink.Name = "startStopLink";
            this.startStopLink.Size = new System.Drawing.Size(76, 13);
            this.startStopLink.TabIndex = 10;
            this.startStopLink.TabStop = true;
            this.startStopLink.Text = "Start harvester";
            this.startStopLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.startStopLink_LinkClicked);
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(5, 6);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(43, 13);
            this.label3.TabIndex = 12;
            this.label3.Text = "Api Key";
            // 
            // apiKeyText
            // 
            this.apiKeyText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.apiKeyText.Location = new System.Drawing.Point(123, 3);
            this.apiKeyText.Name = "apiKeyText";
            this.apiKeyText.Size = new System.Drawing.Size(386, 20);
            this.apiKeyText.TabIndex = 13;
            this.apiKeyText.Leave += new System.EventHandler(this.apiKeyText_Leave);
            // 
            // apiKeyLink
            // 
            this.apiKeyLink.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.apiKeyLink.AutoSize = true;
            this.apiKeyLink.Location = new System.Drawing.Point(513, 6);
            this.apiKeyLink.Name = "apiKeyLink";
            this.apiKeyLink.Size = new System.Drawing.Size(65, 13);
            this.apiKeyLink.TabIndex = 14;
            this.apiKeyLink.TabStop = true;
            this.apiKeyLink.Text = "Get API Key";
            this.apiKeyLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.apiKeyLink_LinkClicked);
            // 
            // clearCacheLink
            // 
            this.clearCacheLink.AutoSize = true;
            this.clearCacheLink.Location = new System.Drawing.Point(298, 374);
            this.clearCacheLink.Name = "clearCacheLink";
            this.clearCacheLink.Size = new System.Drawing.Size(89, 13);
            this.clearCacheLink.TabIndex = 17;
            this.clearCacheLink.TabStop = true;
            this.clearCacheLink.Text = "Clear local cache";
            this.clearCacheLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.clearCacheLink_LinkClicked);
            // 
            // nameCountText
            // 
            this.nameCountText.Location = new System.Drawing.Point(123, 371);
            this.nameCountText.Name = "nameCountText";
            this.nameCountText.ReadOnly = true;
            this.nameCountText.Size = new System.Drawing.Size(169, 20);
            this.nameCountText.TabIndex = 16;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(8, 374);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(65, 13);
            this.label8.TabIndex = 15;
            this.label8.Text = "Name count";
            // 
            // errLogLink
            // 
            this.errLogLink.AutoSize = true;
            this.errLogLink.Location = new System.Drawing.Point(423, 374);
            this.errLogLink.Name = "errLogLink";
            this.errLogLink.Size = new System.Drawing.Size(76, 13);
            this.errLogLink.TabIndex = 19;
            this.errLogLink.TabStop = true;
            this.errLogLink.Text = "View Error Log";
            this.errLogLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.errLogLink_LinkClicked);
            // 
            // SetupCtrl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.errLogLink);
            this.Controls.Add(this.clearCacheLink);
            this.Controls.Add(this.nameCountText);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.apiKeyLink);
            this.Controls.Add(this.apiKeyText);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.startStopLink);
            this.Controls.Add(this.harvestStatusText);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.lastHarvestText);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.harvestIntervalText);
            this.Controls.Add(this.urlText);
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "SetupCtrl";
            this.Size = new System.Drawing.Size(646, 423);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.TextBox urlText;
        private System.Windows.Forms.TextBox harvestIntervalText;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckBox acceptedNameCheck;
        private System.Windows.Forms.LinkLabel addTaxonLink;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox lastHarvestText;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox harvestStatusText;
        private System.Windows.Forms.LinkLabel startStopLink;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private System.Windows.Forms.LinkLabel removeTaxonLink;
        private System.Windows.Forms.ListBox startingTaxaList;
        private System.Windows.Forms.LinkLabel apiKeyLink;
        private System.Windows.Forms.TextBox apiKeyText;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.LinkLabel clearCacheLink;
        private System.Windows.Forms.TextBox nameCountText;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.CheckBox uncertainCheck;
        private System.Windows.Forms.CheckBox presentCheck;
        private System.Windows.Forms.LinkLabel errLogLink;
    }
}
