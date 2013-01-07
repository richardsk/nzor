namespace NZORConsumer
{
    partial class ProviderNameResultsForm
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
            this.providerNamesCtrl1 = new NZORConsumer.Controls.ProviderNamesCtrl();
            this.closeButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // providerNamesCtrl1
            // 
            this.providerNamesCtrl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.providerNamesCtrl1.Location = new System.Drawing.Point(12, 12);
            this.providerNamesCtrl1.Name = "providerNamesCtrl1";
            this.providerNamesCtrl1.Size = new System.Drawing.Size(747, 450);
            this.providerNamesCtrl1.TabIndex = 0;
            // 
            // closeButton
            // 
            this.closeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.closeButton.Location = new System.Drawing.Point(684, 468);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(75, 23);
            this.closeButton.TabIndex = 1;
            this.closeButton.Text = "Close";
            this.closeButton.UseVisualStyleBackColor = true;
            this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
            // 
            // ProviderNameResultsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(771, 503);
            this.Controls.Add(this.closeButton);
            this.Controls.Add(this.providerNamesCtrl1);
            this.Name = "ProviderNameResultsForm";
            this.Text = "Provider Name Results";
            this.ResumeLayout(false);

        }

        #endregion

        private Controls.ProviderNamesCtrl providerNamesCtrl1;
        private System.Windows.Forms.Button closeButton;
    }
}