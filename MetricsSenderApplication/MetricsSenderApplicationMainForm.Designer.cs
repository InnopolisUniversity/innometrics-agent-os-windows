using System;
using System.Drawing;
using System.Windows.Forms;

namespace MetricsSenderApplication
{
    partial class MetricsSenderApplicationMainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MetricsSenderApplicationMainForm));
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.listBoxFilteringTitle = new System.Windows.Forms.ListBox();
            this.groupBoxFilteringTitle = new System.Windows.Forms.GroupBox();
            this.buttonAddFilterTitle = new System.Windows.Forms.Button();
            this.textBoxFilteringTitle = new System.Windows.Forms.TextBox();
            this.groupBoxFilteringDate = new System.Windows.Forms.GroupBox();
            this.labelFrom = new System.Windows.Forms.Label();
            this.labelUntil = new System.Windows.Forms.Label();
            this.dateTimePickerUntil = new System.Windows.Forms.DateTimePicker();
            this.dateTimePickerFrom = new System.Windows.Forms.DateTimePicker();
            this.buttonRefresh = new System.Windows.Forms.Button();
            this.buttonTransmit = new System.Windows.Forms.Button();
            this.buttonSettings = new System.Windows.Forms.Button();
            this.buttonDeleteProcessed = new System.Windows.Forms.Button();
            this.AgentName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Application = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Title = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.From = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Until = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Duration = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ExePath = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Ip = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Mac = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.User = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Url = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.groupBoxFilteringTitle.SuspendLayout();
            this.groupBoxFilteringDate.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridView
            // 
            this.dataGridView.AllowUserToAddRows = false;
            this.dataGridView.AllowUserToDeleteRows = false;
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.AgentName,
            this.Application,
            this.Title,
            this.From,
            this.Until,
            this.Duration,
            this.ExePath,
            this.Ip,
            this.Mac,
            this.User,
            this.Url});
            this.dataGridView.Location = new System.Drawing.Point(259, 12);
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.ReadOnly = true;
            this.dataGridView.Size = new System.Drawing.Size(746, 392);
            this.dataGridView.TabIndex = 2;
            // 
            // listBoxFilteringTitle
            // 
            this.listBoxFilteringTitle.FormattingEnabled = true;
            this.listBoxFilteringTitle.Location = new System.Drawing.Point(6, 58);
            this.listBoxFilteringTitle.Name = "listBoxFilteringTitle";
            this.listBoxFilteringTitle.Size = new System.Drawing.Size(208, 186);
            this.listBoxFilteringTitle.TabIndex = 1;
            this.listBoxFilteringTitle.DoubleClick += new System.EventHandler(this.listBoxFilteringTitle_DoubleClick);
            // 
            // groupBoxFilteringTitle
            // 
            this.groupBoxFilteringTitle.Controls.Add(this.buttonAddFilterTitle);
            this.groupBoxFilteringTitle.Controls.Add(this.textBoxFilteringTitle);
            this.groupBoxFilteringTitle.Controls.Add(this.listBoxFilteringTitle);
            this.groupBoxFilteringTitle.Location = new System.Drawing.Point(12, 12);
            this.groupBoxFilteringTitle.Name = "groupBoxFilteringTitle";
            this.groupBoxFilteringTitle.Size = new System.Drawing.Size(220, 251);
            this.groupBoxFilteringTitle.TabIndex = 0;
            this.groupBoxFilteringTitle.TabStop = false;
            this.groupBoxFilteringTitle.Text = "Window Title Filter";
            // 
            // buttonAddFilterTitle
            // 
            this.buttonAddFilterTitle.BackColor = System.Drawing.SystemColors.Control;
            this.buttonAddFilterTitle.Location = new System.Drawing.Point(163, 17);
            this.buttonAddFilterTitle.Name = "buttonAddFilterTitle";
            this.buttonAddFilterTitle.Size = new System.Drawing.Size(51, 30);
            this.buttonAddFilterTitle.TabIndex = 3;
            this.buttonAddFilterTitle.Text = "Add";
            this.buttonAddFilterTitle.UseVisualStyleBackColor = false;
            this.buttonAddFilterTitle.Click += new System.EventHandler(this.buttonAddFilterTitle_Click);
            // 
            // textBoxFilteringTitle
            // 
            this.textBoxFilteringTitle.Location = new System.Drawing.Point(6, 23);
            this.textBoxFilteringTitle.Name = "textBoxFilteringTitle";
            this.textBoxFilteringTitle.Size = new System.Drawing.Size(151, 20);
            this.textBoxFilteringTitle.TabIndex = 2;
            // 
            // groupBoxFilteringDate
            // 
            this.groupBoxFilteringDate.Controls.Add(this.labelFrom);
            this.groupBoxFilteringDate.Controls.Add(this.labelUntil);
            this.groupBoxFilteringDate.Controls.Add(this.dateTimePickerUntil);
            this.groupBoxFilteringDate.Controls.Add(this.dateTimePickerFrom);
            this.groupBoxFilteringDate.Location = new System.Drawing.Point(12, 269);
            this.groupBoxFilteringDate.Name = "groupBoxFilteringDate";
            this.groupBoxFilteringDate.Size = new System.Drawing.Size(220, 75);
            this.groupBoxFilteringDate.TabIndex = 3;
            this.groupBoxFilteringDate.TabStop = false;
            this.groupBoxFilteringDate.Text = "Date Filter";
            // 
            // labelFrom
            // 
            this.labelFrom.AutoSize = true;
            this.labelFrom.Location = new System.Drawing.Point(6, 25);
            this.labelFrom.Name = "labelFrom";
            this.labelFrom.Size = new System.Drawing.Size(30, 13);
            this.labelFrom.TabIndex = 3;
            this.labelFrom.Text = "From";
            // 
            // labelUntil
            // 
            this.labelUntil.AutoSize = true;
            this.labelUntil.Location = new System.Drawing.Point(6, 51);
            this.labelUntil.Name = "labelUntil";
            this.labelUntil.Size = new System.Drawing.Size(28, 13);
            this.labelUntil.TabIndex = 2;
            this.labelUntil.Text = "Until";
            // 
            // dateTimePickerUntil
            // 
            this.dateTimePickerUntil.CustomFormat = "dd/MM/yyyy HH:mm:ss";
            this.dateTimePickerUntil.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePickerUntil.Location = new System.Drawing.Point(51, 45);
            this.dateTimePickerUntil.Name = "dateTimePickerUntil";
            this.dateTimePickerUntil.Size = new System.Drawing.Size(163, 20);
            this.dateTimePickerUntil.TabIndex = 1;
            this.dateTimePickerUntil.ValueChanged += new System.EventHandler(this.ValidateTimeOrderOnValueChanged);
            // 
            // dateTimePickerFrom
            // 
            this.dateTimePickerFrom.CustomFormat = "dd/MM/yyyy HH:mm:ss";
            this.dateTimePickerFrom.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePickerFrom.Location = new System.Drawing.Point(51, 19);
            this.dateTimePickerFrom.Name = "dateTimePickerFrom";
            this.dateTimePickerFrom.Size = new System.Drawing.Size(163, 20);
            this.dateTimePickerFrom.TabIndex = 0;
            this.dateTimePickerFrom.ValueChanged += new System.EventHandler(this.ValidateTimeOrderOnValueChanged);
            // 
            // buttonRefresh
            // 
            this.buttonRefresh.Location = new System.Drawing.Point(12, 350);
            this.buttonRefresh.Name = "buttonRefresh";
            this.buttonRefresh.Size = new System.Drawing.Size(102, 23);
            this.buttonRefresh.TabIndex = 4;
            this.buttonRefresh.Text = "Show Activities";
            this.buttonRefresh.UseVisualStyleBackColor = true;
            this.buttonRefresh.Click += new System.EventHandler(this.buttonRefresh_Click);
            // 
            // buttonTransmit
            // 
            this.buttonTransmit.Location = new System.Drawing.Point(130, 350);
            this.buttonTransmit.Name = "buttonTransmit";
            this.buttonTransmit.Size = new System.Drawing.Size(102, 23);
            this.buttonTransmit.TabIndex = 5;
            this.buttonTransmit.Text = "Transmit Activities";
            this.buttonTransmit.UseVisualStyleBackColor = true;
            this.buttonTransmit.Click += new System.EventHandler(this.buttonTransmit_Click);
            // 
            // buttonSettings
            // 
            this.buttonSettings.Location = new System.Drawing.Point(130, 381);
            this.buttonSettings.Name = "buttonSettings";
            this.buttonSettings.Size = new System.Drawing.Size(102, 23);
            this.buttonSettings.TabIndex = 6;
            this.buttonSettings.Text = "Settings";
            this.buttonSettings.UseVisualStyleBackColor = true;
            this.buttonSettings.Click += new System.EventHandler(this.buttonSettings_Click);
            // 
            // buttonDeleteProcessed
            // 
            this.buttonDeleteProcessed.Location = new System.Drawing.Point(12, 381);
            this.buttonDeleteProcessed.Name = "buttonDeleteProcessed";
            this.buttonDeleteProcessed.Size = new System.Drawing.Size(102, 23);
            this.buttonDeleteProcessed.TabIndex = 7;
            this.buttonDeleteProcessed.Text = "Clear Transmitted";
            this.buttonDeleteProcessed.UseVisualStyleBackColor = true;
            this.buttonDeleteProcessed.Click += new System.EventHandler(this.buttonDeleteProcessed_Click);
            // 
            // AgentName
            // 
            this.AgentName.HeaderText = "Agent Name";
            this.AgentName.Name = "AgentName";
            this.AgentName.ReadOnly = true;
            // 
            // Application
            // 
            this.Application.HeaderText = "Application";
            this.Application.Name = "Application";
            this.Application.ReadOnly = true;
            // 
            // Title
            // 
            this.Title.HeaderText = "Title";
            this.Title.Name = "Title";
            this.Title.ReadOnly = true;
            // 
            // From
            // 
            this.From.HeaderText = "From";
            this.From.Name = "From";
            this.From.ReadOnly = true;
            // 
            // Until
            // 
            this.Until.HeaderText = "Until";
            this.Until.Name = "Until";
            this.Until.ReadOnly = true;
            // 
            // Duration
            // 
            this.Duration.HeaderText = "Duration";
            this.Duration.Name = "Duration";
            this.Duration.ReadOnly = true;
            // 
            // ExePath
            // 
            this.ExePath.HeaderText = "Executable Path";
            this.ExePath.Name = "ExePath";
            this.ExePath.ReadOnly = true;
            // 
            // Ip
            // 
            this.Ip.HeaderText = "IP";
            this.Ip.Name = "Ip";
            this.Ip.ReadOnly = true;
            // 
            // Mac
            // 
            this.Mac.HeaderText = "MAC";
            this.Mac.Name = "Mac";
            this.Mac.ReadOnly = true;
            // 
            // User
            // 
            this.User.HeaderText = "User";
            this.User.Name = "User";
            this.User.ReadOnly = true;
            // 
            // Url
            // 
            this.Url.HeaderText = "URL";
            this.Url.Name = "Url";
            this.Url.ReadOnly = true;
            // 
            // MetricsSenderApplicationMainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1017, 416);
            this.Controls.Add(this.buttonDeleteProcessed);
            this.Controls.Add(this.buttonSettings);
            this.Controls.Add(this.buttonTransmit);
            this.Controls.Add(this.buttonRefresh);
            this.Controls.Add(this.groupBoxFilteringDate);
            this.Controls.Add(this.groupBoxFilteringTitle);
            this.Controls.Add(this.dataGridView);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "MetricsSenderApplicationMainForm";
            this.Text = "Metrics Manager";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.groupBoxFilteringTitle.ResumeLayout(false);
            this.groupBoxFilteringTitle.PerformLayout();
            this.groupBoxFilteringDate.ResumeLayout(false);
            this.groupBoxFilteringDate.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView;
        private System.Windows.Forms.ListBox listBoxFilteringTitle;
        private System.Windows.Forms.GroupBox groupBoxFilteringTitle;
        private System.Windows.Forms.Button buttonAddFilterTitle;
        private System.Windows.Forms.TextBox textBoxFilteringTitle;
        private System.Windows.Forms.GroupBox groupBoxFilteringDate;
        private System.Windows.Forms.DateTimePicker dateTimePickerUntil;
        private System.Windows.Forms.DateTimePicker dateTimePickerFrom;
        private Button buttonRefresh;
        private Button buttonTransmit;
        private Button buttonSettings;
        private Label labelFrom;
        private Label labelUntil;
        private Button buttonDeleteProcessed;
        private DataGridViewTextBoxColumn AgentName;
        private DataGridViewTextBoxColumn Application;
        private DataGridViewTextBoxColumn Title;
        private DataGridViewTextBoxColumn From;
        private DataGridViewTextBoxColumn Until;
        private DataGridViewTextBoxColumn Duration;
        private DataGridViewTextBoxColumn ExePath;
        private DataGridViewTextBoxColumn Ip;
        private DataGridViewTextBoxColumn Mac;
        private DataGridViewTextBoxColumn User;
        private DataGridViewTextBoxColumn Url;
    }
}

