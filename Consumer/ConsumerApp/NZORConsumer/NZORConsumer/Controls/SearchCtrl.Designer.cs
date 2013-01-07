namespace NZORConsumer
{
    partial class SearchCtrl
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
            this.searchText = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.searchButton = new System.Windows.Forms.Button();
            this.resultsGrid = new System.Windows.Forms.DataGridView();
            this.fullNameColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.rankColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.authorsCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.yearCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.govCodeCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.messageLabel = new System.Windows.Forms.Label();
            this.searchLocalCheck = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.resultsGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Search for";
            // 
            // searchText
            // 
            this.searchText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.searchText.Location = new System.Drawing.Point(67, 4);
            this.searchText.Name = "searchText";
            this.searchText.Size = new System.Drawing.Size(409, 20);
            this.searchText.TabIndex = 1;
            this.searchText.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.searchText_KeyPress);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(4, 51);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(42, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Results";
            // 
            // searchButton
            // 
            this.searchButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.searchButton.Location = new System.Drawing.Point(483, 3);
            this.searchButton.Name = "searchButton";
            this.searchButton.Size = new System.Drawing.Size(75, 23);
            this.searchButton.TabIndex = 3;
            this.searchButton.Text = "Search";
            this.searchButton.UseVisualStyleBackColor = true;
            this.searchButton.Click += new System.EventHandler(this.searchButton_Click);
            // 
            // resultsGrid
            // 
            this.resultsGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.resultsGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.resultsGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.resultsGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.fullNameColumn,
            this.rankColumn,
            this.authorsCol,
            this.yearCol,
            this.govCodeCol});
            this.resultsGrid.Location = new System.Drawing.Point(7, 67);
            this.resultsGrid.Name = "resultsGrid";
            this.resultsGrid.ReadOnly = true;
            this.resultsGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.resultsGrid.Size = new System.Drawing.Size(551, 249);
            this.resultsGrid.TabIndex = 4;
            this.resultsGrid.SelectionChanged += new System.EventHandler(this.resultsGrid_SelectionChanged);
            // 
            // fullNameColumn
            // 
            this.fullNameColumn.DataPropertyName = "FullName";
            this.fullNameColumn.FillWeight = 200F;
            this.fullNameColumn.HeaderText = "Full name";
            this.fullNameColumn.Name = "fullNameColumn";
            this.fullNameColumn.ReadOnly = true;
            // 
            // rankColumn
            // 
            this.rankColumn.DataPropertyName = "TaxonRank";
            this.rankColumn.HeaderText = "Rank";
            this.rankColumn.Name = "rankColumn";
            this.rankColumn.ReadOnly = true;
            // 
            // authorsCol
            // 
            this.authorsCol.DataPropertyName = "Authors";
            this.authorsCol.HeaderText = "Authors";
            this.authorsCol.Name = "authorsCol";
            this.authorsCol.ReadOnly = true;
            // 
            // yearCol
            // 
            this.yearCol.DataPropertyName = "YearOfPublication";
            this.yearCol.HeaderText = "Year";
            this.yearCol.Name = "yearCol";
            this.yearCol.ReadOnly = true;
            // 
            // govCodeCol
            // 
            this.govCodeCol.DataPropertyName = "GoverningCode";
            this.govCodeCol.HeaderText = "Governing code";
            this.govCodeCol.Name = "govCodeCol";
            this.govCodeCol.ReadOnly = true;
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // messageLabel
            // 
            this.messageLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.messageLabel.AutoSize = true;
            this.messageLabel.Location = new System.Drawing.Point(280, 51);
            this.messageLabel.Name = "messageLabel";
            this.messageLabel.Size = new System.Drawing.Size(26, 13);
            this.messageLabel.TabIndex = 5;
            this.messageLabel.Text = "msg";
            this.messageLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // searchLocalCheck
            // 
            this.searchLocalCheck.AutoSize = true;
            this.searchLocalCheck.Location = new System.Drawing.Point(67, 28);
            this.searchLocalCheck.Name = "searchLocalCheck";
            this.searchLocalCheck.Size = new System.Drawing.Size(118, 17);
            this.searchLocalCheck.TabIndex = 6;
            this.searchLocalCheck.Text = "Search local cache";
            this.searchLocalCheck.UseVisualStyleBackColor = true;
            // 
            // SearchCtrl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.searchLocalCheck);
            this.Controls.Add(this.messageLabel);
            this.Controls.Add(this.resultsGrid);
            this.Controls.Add(this.searchButton);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.searchText);
            this.Controls.Add(this.label1);
            this.Name = "SearchCtrl";
            this.Size = new System.Drawing.Size(570, 328);
            ((System.ComponentModel.ISupportInitialize)(this.resultsGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox searchText;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button searchButton;
        private System.Windows.Forms.DataGridView resultsGrid;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private System.Windows.Forms.Label messageLabel;
        private System.Windows.Forms.DataGridViewTextBoxColumn fullNameColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn rankColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn authorsCol;
        private System.Windows.Forms.DataGridViewTextBoxColumn yearCol;
        private System.Windows.Forms.DataGridViewTextBoxColumn govCodeCol;
        private System.Windows.Forms.CheckBox searchLocalCheck;
    }
}
