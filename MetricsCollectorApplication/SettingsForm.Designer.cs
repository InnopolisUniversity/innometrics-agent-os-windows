namespace MetricsCollectorApplication
{
    partial class SettingsForm
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
            this.textBoxDataSavingIntervalSec = new System.Windows.Forms.TextBox();
            this.textBoxStateScanIntervalSec = new System.Windows.Forms.TextBox();
            this.labelDataSavingIntervalSec = new System.Windows.Forms.Label();
            this.labelStateScanIntervalSec = new System.Windows.Forms.Label();
            this.buttonSave = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // textBoxDataSavingIntervalSec
            // 
            this.textBoxDataSavingIntervalSec.Location = new System.Drawing.Point(187, 12);
            this.textBoxDataSavingIntervalSec.Name = "textBoxDataSavingIntervalSec";
            this.textBoxDataSavingIntervalSec.Size = new System.Drawing.Size(79, 20);
            this.textBoxDataSavingIntervalSec.TabIndex = 0;
            // 
            // textBoxStateScanIntervalSec
            // 
            this.textBoxStateScanIntervalSec.Location = new System.Drawing.Point(187, 38);
            this.textBoxStateScanIntervalSec.Name = "textBoxStateScanIntervalSec";
            this.textBoxStateScanIntervalSec.Size = new System.Drawing.Size(79, 20);
            this.textBoxStateScanIntervalSec.TabIndex = 1;
            // 
            // labelDataSavingIntervalSec
            // 
            this.labelDataSavingIntervalSec.AutoSize = true;
            this.labelDataSavingIntervalSec.Location = new System.Drawing.Point(12, 15);
            this.labelDataSavingIntervalSec.Name = "labelDataSavingIntervalSec";
            this.labelDataSavingIntervalSec.Size = new System.Drawing.Size(153, 13);
            this.labelDataSavingIntervalSec.TabIndex = 2;
            this.labelDataSavingIntervalSec.Text = "Data Saving Interval (seconds)";
            // 
            // labelStateScanIntervalSec
            // 
            this.labelStateScanIntervalSec.AutoSize = true;
            this.labelStateScanIntervalSec.Location = new System.Drawing.Point(12, 41);
            this.labelStateScanIntervalSec.Name = "labelStateScanIntervalSec";
            this.labelStateScanIntervalSec.Size = new System.Drawing.Size(167, 13);
            this.labelStateScanIntervalSec.TabIndex = 3;
            this.labelStateScanIntervalSec.Text = "State Scanning Interval (seconds)";
            // 
            // buttonSave
            // 
            this.buttonSave.Location = new System.Drawing.Point(12, 75);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(150, 23);
            this.buttonSave.TabIndex = 4;
            this.buttonSave.Text = "Save And Close Application";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(280, 110);
            this.Controls.Add(this.buttonSave);
            this.Controls.Add(this.labelStateScanIntervalSec);
            this.Controls.Add(this.labelDataSavingIntervalSec);
            this.Controls.Add(this.textBoxStateScanIntervalSec);
            this.Controls.Add(this.textBoxDataSavingIntervalSec);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "SettingsForm";
            this.Text = "Settings";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxDataSavingIntervalSec;
        private System.Windows.Forms.TextBox textBoxStateScanIntervalSec;
        private System.Windows.Forms.Label labelDataSavingIntervalSec;
        private System.Windows.Forms.Label labelStateScanIntervalSec;
        private System.Windows.Forms.Button buttonSave;
    }
}