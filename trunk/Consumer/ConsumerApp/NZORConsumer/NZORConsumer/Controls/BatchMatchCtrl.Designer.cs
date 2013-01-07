namespace NZORConsumer.Controls
{
    partial class BatchMatchCtrl
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
            this.pollButton = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.batchResultsGrid = new System.Windows.Forms.DataGridView();
            this.IdCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.FilenameCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DateCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.StatusCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.UrlCol = new System.Windows.Forms.DataGridViewLinkColumn();
            this.ResulsFileCol = new System.Windows.Forms.DataGridViewLinkColumn();
            this.submitButton = new System.Windows.Forms.Button();
            this.brokerCheck = new System.Windows.Forms.CheckBox();
            this.chooseFileButton = new System.Windows.Forms.Button();
            this.batchFileText = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.nameRequestsGrid = new System.Windows.Forms.DataGridView();
            this.label4 = new System.Windows.Forms.Label();
            this.FullNameCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DateReqCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.NameRequestIdCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.BatchMatchCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.NRStatusCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ProvNameResCol = new System.Windows.Forms.DataGridViewLinkColumn();
            ((System.ComponentModel.ISupportInitialize)(this.batchResultsGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nameRequestsGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // pollButton
            // 
            this.pollButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pollButton.Location = new System.Drawing.Point(759, 89);
            this.pollButton.Name = "pollButton";
            this.pollButton.Size = new System.Drawing.Size(93, 23);
            this.pollButton.TabIndex = 15;
            this.pollButton.Text = "Poll results";
            this.pollButton.UseVisualStyleBackColor = true;
            this.pollButton.Click += new System.EventHandler(this.pollButton_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 99);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(42, 13);
            this.label2.TabIndex = 14;
            this.label2.Text = "Results";
            // 
            // batchResultsGrid
            // 
            this.batchResultsGrid.AllowUserToAddRows = false;
            this.batchResultsGrid.AllowUserToDeleteRows = false;
            this.batchResultsGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.batchResultsGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.batchResultsGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.batchResultsGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.IdCol,
            this.FilenameCol,
            this.DateCol,
            this.StatusCol,
            this.UrlCol,
            this.ResulsFileCol});
            this.batchResultsGrid.Location = new System.Drawing.Point(10, 115);
            this.batchResultsGrid.Name = "batchResultsGrid";
            this.batchResultsGrid.Size = new System.Drawing.Size(842, 214);
            this.batchResultsGrid.TabIndex = 13;
            this.batchResultsGrid.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.batchResultsGrid_CellContentClick);
            // 
            // IdCol
            // 
            this.IdCol.DataPropertyName = "MatchId";
            this.IdCol.HeaderText = "Id";
            this.IdCol.Name = "IdCol";
            this.IdCol.ReadOnly = true;
            this.IdCol.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // FilenameCol
            // 
            this.FilenameCol.DataPropertyName = "Filename";
            this.FilenameCol.HeaderText = "Filename";
            this.FilenameCol.Name = "FilenameCol";
            this.FilenameCol.ReadOnly = true;
            // 
            // DateCol
            // 
            this.DateCol.DataPropertyName = "DateSubmitted";
            this.DateCol.HeaderText = "Date submitted";
            this.DateCol.Name = "DateCol";
            this.DateCol.ReadOnly = true;
            // 
            // StatusCol
            // 
            this.StatusCol.DataPropertyName = "Status";
            this.StatusCol.HeaderText = "Status";
            this.StatusCol.Name = "StatusCol";
            this.StatusCol.ReadOnly = true;
            // 
            // UrlCol
            // 
            this.UrlCol.DataPropertyName = "DownloadUrl";
            this.UrlCol.HeaderText = "Download Url";
            this.UrlCol.Name = "UrlCol";
            this.UrlCol.ReadOnly = true;
            // 
            // ResulsFileCol
            // 
            this.ResulsFileCol.DataPropertyName = "ResultsFile";
            this.ResulsFileCol.HeaderText = "Results file";
            this.ResulsFileCol.Name = "ResulsFileCol";
            this.ResulsFileCol.ReadOnly = true;
            // 
            // submitButton
            // 
            this.submitButton.Location = new System.Drawing.Point(84, 61);
            this.submitButton.Name = "submitButton";
            this.submitButton.Size = new System.Drawing.Size(75, 23);
            this.submitButton.TabIndex = 12;
            this.submitButton.Text = "Submit";
            this.submitButton.UseVisualStyleBackColor = true;
            this.submitButton.Click += new System.EventHandler(this.submitButton_Click);
            // 
            // brokerCheck
            // 
            this.brokerCheck.AutoSize = true;
            this.brokerCheck.Location = new System.Drawing.Point(84, 32);
            this.brokerCheck.Name = "brokerCheck";
            this.brokerCheck.Size = new System.Drawing.Size(217, 17);
            this.brokerCheck.TabIndex = 11;
            this.brokerCheck.Text = "Request names to be brokered if missing";
            this.brokerCheck.UseVisualStyleBackColor = true;
            // 
            // chooseFileButton
            // 
            this.chooseFileButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chooseFileButton.Location = new System.Drawing.Point(627, 2);
            this.chooseFileButton.Name = "chooseFileButton";
            this.chooseFileButton.Size = new System.Drawing.Size(26, 23);
            this.chooseFileButton.TabIndex = 10;
            this.chooseFileButton.Text = "...";
            this.chooseFileButton.UseVisualStyleBackColor = true;
            this.chooseFileButton.Click += new System.EventHandler(this.chooseFileButton_Click);
            // 
            // batchFileText
            // 
            this.batchFileText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.batchFileText.Location = new System.Drawing.Point(84, 3);
            this.batchFileText.Name = "batchFileText";
            this.batchFileText.Size = new System.Drawing.Size(540, 20);
            this.batchFileText.TabIndex = 9;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "Batch csv file";
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 336);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(83, 13);
            this.label3.TabIndex = 16;
            this.label3.Text = "Name Requests";
            // 
            // nameRequestsGrid
            // 
            this.nameRequestsGrid.AllowUserToAddRows = false;
            this.nameRequestsGrid.AllowUserToDeleteRows = false;
            this.nameRequestsGrid.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.nameRequestsGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.nameRequestsGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.nameRequestsGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.FullNameCol,
            this.DateReqCol,
            this.NameRequestIdCol,
            this.BatchMatchCol,
            this.NRStatusCol,
            this.ProvNameResCol});
            this.nameRequestsGrid.Location = new System.Drawing.Point(10, 352);
            this.nameRequestsGrid.Name = "nameRequestsGrid";
            this.nameRequestsGrid.Size = new System.Drawing.Size(842, 118);
            this.nameRequestsGrid.TabIndex = 17;
            this.nameRequestsGrid.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.nameRequestsGrid_CellContentClick);
            // 
            // label4
            // 
            this.label4.ForeColor = System.Drawing.Color.Red;
            this.label4.Location = new System.Drawing.Point(308, 30);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(345, 34);
            this.label4.TabIndex = 18;
            this.label4.Text = "Warning:  this setting will instruct NZOR to add missing names to the system.  On" +
    "ly check if you are sure you want this.";
            // 
            // FullNameCol
            // 
            this.FullNameCol.DataPropertyName = "FullName";
            this.FullNameCol.HeaderText = "Full name";
            this.FullNameCol.Name = "FullNameCol";
            this.FullNameCol.ReadOnly = true;
            // 
            // DateReqCol
            // 
            this.DateReqCol.DataPropertyName = "DateRequested";
            this.DateReqCol.HeaderText = "Date requested";
            this.DateReqCol.Name = "DateReqCol";
            this.DateReqCol.ReadOnly = true;
            // 
            // NameRequestIdCol
            // 
            this.NameRequestIdCol.DataPropertyName = "NameRequestId";
            this.NameRequestIdCol.HeaderText = "Column1";
            this.NameRequestIdCol.Name = "NameRequestIdCol";
            this.NameRequestIdCol.Visible = false;
            // 
            // BatchMatchCol
            // 
            this.BatchMatchCol.DataPropertyName = "BatchMatchId";
            this.BatchMatchCol.HeaderText = "Batch match Id";
            this.BatchMatchCol.Name = "BatchMatchCol";
            this.BatchMatchCol.ReadOnly = true;
            // 
            // NRStatusCol
            // 
            this.NRStatusCol.DataPropertyName = "Status";
            this.NRStatusCol.HeaderText = "Status";
            this.NRStatusCol.Name = "NRStatusCol";
            this.NRStatusCol.ReadOnly = true;
            // 
            // ProvNameResCol
            // 
            this.ProvNameResCol.DataPropertyName = "ProviderNameResults";
            this.ProvNameResCol.HeaderText = "Provider name results";
            this.ProvNameResCol.Name = "ProvNameResCol";
            this.ProvNameResCol.ReadOnly = true;
            // 
            // BatchMatchCtrl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label4);
            this.Controls.Add(this.nameRequestsGrid);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.pollButton);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.batchResultsGrid);
            this.Controls.Add(this.submitButton);
            this.Controls.Add(this.brokerCheck);
            this.Controls.Add(this.chooseFileButton);
            this.Controls.Add(this.batchFileText);
            this.Controls.Add(this.label1);
            this.Name = "BatchMatchCtrl";
            this.Size = new System.Drawing.Size(874, 484);
            ((System.ComponentModel.ISupportInitialize)(this.batchResultsGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nameRequestsGrid)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button pollButton;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DataGridView batchResultsGrid;
        private System.Windows.Forms.Button submitButton;
        private System.Windows.Forms.CheckBox brokerCheck;
        private System.Windows.Forms.Button chooseFileButton;
        private System.Windows.Forms.TextBox batchFileText;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DataGridView nameRequestsGrid;
        private System.Windows.Forms.DataGridViewTextBoxColumn IdCol;
        private System.Windows.Forms.DataGridViewTextBoxColumn FilenameCol;
        private System.Windows.Forms.DataGridViewTextBoxColumn DateCol;
        private System.Windows.Forms.DataGridViewTextBoxColumn StatusCol;
        private System.Windows.Forms.DataGridViewLinkColumn UrlCol;
        private System.Windows.Forms.DataGridViewLinkColumn ResulsFileCol;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.DataGridViewTextBoxColumn FullNameCol;
        private System.Windows.Forms.DataGridViewTextBoxColumn DateReqCol;
        private System.Windows.Forms.DataGridViewTextBoxColumn NameRequestIdCol;
        private System.Windows.Forms.DataGridViewTextBoxColumn BatchMatchCol;
        private System.Windows.Forms.DataGridViewTextBoxColumn NRStatusCol;
        private System.Windows.Forms.DataGridViewLinkColumn ProvNameResCol;
    }
}
