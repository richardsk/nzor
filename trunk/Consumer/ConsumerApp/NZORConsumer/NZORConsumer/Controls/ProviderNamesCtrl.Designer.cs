namespace NZORConsumer.Controls
{
    partial class ProviderNamesCtrl
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
            this.provNamesGrid = new System.Windows.Forms.DataGridView();
            this.titleLabel = new System.Windows.Forms.Label();
            this.NameCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SourceCol = new System.Windows.Forms.DataGridViewLinkColumn();
            this.ProviderNameSourceCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.StatusCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.provNamesGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // provNamesGrid
            // 
            this.provNamesGrid.AllowUserToAddRows = false;
            this.provNamesGrid.AllowUserToDeleteRows = false;
            this.provNamesGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.provNamesGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.provNamesGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.provNamesGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.NameCol,
            this.SourceCol,
            this.ProviderNameSourceCol,
            this.StatusCol});
            this.provNamesGrid.Location = new System.Drawing.Point(3, 22);
            this.provNamesGrid.Name = "provNamesGrid";
            this.provNamesGrid.Size = new System.Drawing.Size(595, 335);
            this.provNamesGrid.TabIndex = 0;
            this.provNamesGrid.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.provNamesGrid_CellContentClick);
            // 
            // titleLabel
            // 
            this.titleLabel.AutoSize = true;
            this.titleLabel.Location = new System.Drawing.Point(3, 6);
            this.titleLabel.Name = "titleLabel";
            this.titleLabel.Size = new System.Drawing.Size(215, 13);
            this.titleLabel.TabIndex = 1;
            this.titleLabel.Text = "Provider names resulting from name request ";
            // 
            // NameCol
            // 
            this.NameCol.DataPropertyName = "FullName";
            this.NameCol.HeaderText = "Full name";
            this.NameCol.Name = "NameCol";
            this.NameCol.ReadOnly = true;
            // 
            // SourceCol
            // 
            this.SourceCol.DataPropertyName = "DataSource";
            this.SourceCol.HeaderText = "Record source";
            this.SourceCol.Name = "SourceCol";
            this.SourceCol.ReadOnly = true;
            this.SourceCol.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // ProviderNameSourceCol
            // 
            this.ProviderNameSourceCol.DataPropertyName = "ProvNameSource";
            this.ProviderNameSourceCol.HeaderText = "Provider name source";
            this.ProviderNameSourceCol.Name = "ProviderNameSourceCol";
            this.ProviderNameSourceCol.ReadOnly = true;
            this.ProviderNameSourceCol.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // StatusCol
            // 
            this.StatusCol.DataPropertyName = "ProvNameStatus";
            this.StatusCol.HeaderText = "Provider name status";
            this.StatusCol.Name = "StatusCol";
            this.StatusCol.ReadOnly = true;
            // 
            // ProviderNamesCtrl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.titleLabel);
            this.Controls.Add(this.provNamesGrid);
            this.Name = "ProviderNamesCtrl";
            this.Size = new System.Drawing.Size(601, 360);
            ((System.ComponentModel.ISupportInitialize)(this.provNamesGrid)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView provNamesGrid;
        private System.Windows.Forms.Label titleLabel;
        private System.Windows.Forms.DataGridViewTextBoxColumn NameCol;
        private System.Windows.Forms.DataGridViewLinkColumn SourceCol;
        private System.Windows.Forms.DataGridViewTextBoxColumn ProviderNameSourceCol;
        private System.Windows.Forms.DataGridViewTextBoxColumn StatusCol;
    }
}
