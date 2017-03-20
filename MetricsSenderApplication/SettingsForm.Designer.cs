namespace MetricsSenderApplication
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
            this.buttonCheckUpdate = new System.Windows.Forms.Button();
            this.richTextBox = new System.Windows.Forms.RichTextBox();
            this.textBoxUpdateXmlUri = new System.Windows.Forms.TextBox();
            this.textBoxSendDataUri = new System.Windows.Forms.TextBox();
            this.textBoxAuthorizationUri = new System.Windows.Forms.TextBox();
            this.textBoxAssemblies = new System.Windows.Forms.TextBox();
            this.labelAuthorizationUri = new System.Windows.Forms.Label();
            this.labelSendDataUri = new System.Windows.Forms.Label();
            this.labelUpdateXmlUri = new System.Windows.Forms.Label();
            this.labelAssemblies = new System.Windows.Forms.Label();
            this.buttonSave = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // buttonCheckUpdate
            // 
            this.buttonCheckUpdate.Location = new System.Drawing.Point(202, 269);
            this.buttonCheckUpdate.Name = "buttonCheckUpdate";
            this.buttonCheckUpdate.Size = new System.Drawing.Size(106, 25);
            this.buttonCheckUpdate.TabIndex = 1;
            this.buttonCheckUpdate.Text = "CheckUpdate";
            this.buttonCheckUpdate.UseVisualStyleBackColor = true;
            this.buttonCheckUpdate.Click += new System.EventHandler(this.buttonCheckUpdate_Click);
            // 
            // richTextBox
            // 
            this.richTextBox.Location = new System.Drawing.Point(12, 12);
            this.richTextBox.Name = "richTextBox";
            this.richTextBox.ReadOnly = true;
            this.richTextBox.Size = new System.Drawing.Size(296, 129);
            this.richTextBox.TabIndex = 2;
            this.richTextBox.Text = "";
            // 
            // textBoxUpdateXmlUri
            // 
            this.textBoxUpdateXmlUri.Location = new System.Drawing.Point(111, 211);
            this.textBoxUpdateXmlUri.Name = "textBoxUpdateXmlUri";
            this.textBoxUpdateXmlUri.Size = new System.Drawing.Size(197, 20);
            this.textBoxUpdateXmlUri.TabIndex = 3;
            // 
            // textBoxSendDataUri
            // 
            this.textBoxSendDataUri.Location = new System.Drawing.Point(111, 185);
            this.textBoxSendDataUri.Name = "textBoxSendDataUri";
            this.textBoxSendDataUri.Size = new System.Drawing.Size(197, 20);
            this.textBoxSendDataUri.TabIndex = 4;
            // 
            // textBoxAuthorizationUri
            // 
            this.textBoxAuthorizationUri.Location = new System.Drawing.Point(111, 159);
            this.textBoxAuthorizationUri.Name = "textBoxAuthorizationUri";
            this.textBoxAuthorizationUri.Size = new System.Drawing.Size(197, 20);
            this.textBoxAuthorizationUri.TabIndex = 5;
            // 
            // textBoxAssemblies
            // 
            this.textBoxAssemblies.Location = new System.Drawing.Point(111, 237);
            this.textBoxAssemblies.Name = "textBoxAssemblies";
            this.textBoxAssemblies.Size = new System.Drawing.Size(197, 20);
            this.textBoxAssemblies.TabIndex = 6;
            // 
            // labelAuthorizationUri
            // 
            this.labelAuthorizationUri.AutoSize = true;
            this.labelAuthorizationUri.Location = new System.Drawing.Point(12, 162);
            this.labelAuthorizationUri.Name = "labelAuthorizationUri";
            this.labelAuthorizationUri.Size = new System.Drawing.Size(81, 13);
            this.labelAuthorizationUri.TabIndex = 7;
            this.labelAuthorizationUri.Text = "AuthorizationUri";
            // 
            // labelSendDataUri
            // 
            this.labelSendDataUri.AutoSize = true;
            this.labelSendDataUri.Location = new System.Drawing.Point(12, 188);
            this.labelSendDataUri.Name = "labelSendDataUri";
            this.labelSendDataUri.Size = new System.Drawing.Size(68, 13);
            this.labelSendDataUri.TabIndex = 8;
            this.labelSendDataUri.Text = "SendDataUri";
            // 
            // labelUpdateXmlUri
            // 
            this.labelUpdateXmlUri.AutoSize = true;
            this.labelUpdateXmlUri.Location = new System.Drawing.Point(12, 214);
            this.labelUpdateXmlUri.Name = "labelUpdateXmlUri";
            this.labelUpdateXmlUri.Size = new System.Drawing.Size(72, 13);
            this.labelUpdateXmlUri.TabIndex = 9;
            this.labelUpdateXmlUri.Text = "UpdateXmlUri";
            // 
            // labelAssemblies
            // 
            this.labelAssemblies.AutoSize = true;
            this.labelAssemblies.Location = new System.Drawing.Point(12, 240);
            this.labelAssemblies.Name = "labelAssemblies";
            this.labelAssemblies.Size = new System.Drawing.Size(59, 13);
            this.labelAssemblies.TabIndex = 10;
            this.labelAssemblies.Text = "Assemblies";
            // 
            // buttonSave
            // 
            this.buttonSave.Location = new System.Drawing.Point(15, 269);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(151, 25);
            this.buttonSave.TabIndex = 11;
            this.buttonSave.Text = "Save And Close Application";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(320, 306);
            this.Controls.Add(this.buttonSave);
            this.Controls.Add(this.labelAssemblies);
            this.Controls.Add(this.labelUpdateXmlUri);
            this.Controls.Add(this.labelSendDataUri);
            this.Controls.Add(this.labelAuthorizationUri);
            this.Controls.Add(this.textBoxAssemblies);
            this.Controls.Add(this.textBoxAuthorizationUri);
            this.Controls.Add(this.textBoxSendDataUri);
            this.Controls.Add(this.textBoxUpdateXmlUri);
            this.Controls.Add(this.richTextBox);
            this.Controls.Add(this.buttonCheckUpdate);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "SettingsForm";
            this.Text = "SettingsForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button buttonCheckUpdate;
        private System.Windows.Forms.RichTextBox richTextBox;
        private System.Windows.Forms.TextBox textBoxUpdateXmlUri;
        private System.Windows.Forms.TextBox textBoxSendDataUri;
        private System.Windows.Forms.TextBox textBoxAuthorizationUri;
        private System.Windows.Forms.TextBox textBoxAssemblies;
        private System.Windows.Forms.Label labelAuthorizationUri;
        private System.Windows.Forms.Label labelSendDataUri;
        private System.Windows.Forms.Label labelUpdateXmlUri;
        private System.Windows.Forms.Label labelAssemblies;
        private System.Windows.Forms.Button buttonSave;
    }
}