namespace NZORConsumer
{
    partial class NameSearchForm
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
            this.searchCtrl1 = new NZORConsumer.SearchCtrl();
            this.SelectButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // searchCtrl1
            // 
            this.searchCtrl1.Location = new System.Drawing.Point(12, 12);
            this.searchCtrl1.Name = "searchCtrl1";
            this.searchCtrl1.Size = new System.Drawing.Size(779, 446);
            this.searchCtrl1.TabIndex = 0;
            // 
            // SelectButton
            // 
            this.SelectButton.Location = new System.Drawing.Point(716, 464);
            this.SelectButton.Name = "SelectButton";
            this.SelectButton.Size = new System.Drawing.Size(75, 23);
            this.SelectButton.TabIndex = 1;
            this.SelectButton.Text = "Select";
            this.SelectButton.UseVisualStyleBackColor = true;
            this.SelectButton.Click += new System.EventHandler(this.SelectButton_Click);
            // 
            // NameSearchForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(803, 499);
            this.Controls.Add(this.SelectButton);
            this.Controls.Add(this.searchCtrl1);
            this.Name = "NameSearchForm";
            this.Text = "NameSearchForm";
            this.Load += new System.EventHandler(this.NameSearchForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private SearchCtrl searchCtrl1;
        private System.Windows.Forms.Button SelectButton;
    }
}