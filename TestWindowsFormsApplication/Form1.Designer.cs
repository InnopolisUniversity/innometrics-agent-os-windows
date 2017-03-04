using System.Drawing;
using System.Windows.Forms;

namespace TestWindowsFormsApplication
{
    partial class Form1
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.btnStop = new System.Windows.Forms.Button();
            this.btnStart = new System.Windows.Forms.Button();
            this.listBoxFilter = new System.Windows.Forms.ListBox();
            this.trayIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.txbAddFilter = new System.Windows.Forms.TextBox();
            this.btnAddFilter = new System.Windows.Forms.Button();
            this.groupBoxFiltering = new System.Windows.Forms.GroupBox();
            this.btnTransmit = new System.Windows.Forms.Button();
            this.groupBoxCollectionSettings = new System.Windows.Forms.GroupBox();
            this.chbEnableStateScanning = new System.Windows.Forms.CheckBox();
            this.chbEnableLeftClickTracking = new System.Windows.Forms.CheckBox();
            this.chbEnableForegroundWindowChangeTracking = new System.Windows.Forms.CheckBox();
            this.groupBoxFiltering.SuspendLayout();
            this.groupBoxCollectionSettings.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnStop
            // 
            this.btnStop.Enabled = false;
            this.btnStop.Location = new System.Drawing.Point(180, 12);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(138, 27);
            this.btnStop.TabIndex = 1;
            this.btnStop.Text = "Stop Tracking";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(12, 12);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(138, 27);
            this.btnStart.TabIndex = 2;
            this.btnStart.Text = "Start Tracking";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // listBoxFilter
            // 
            this.listBoxFilter.FormattingEnabled = true;
            this.listBoxFilter.Location = new System.Drawing.Point(6, 48);
            this.listBoxFilter.Name = "listBoxFilter";
            this.listBoxFilter.Size = new System.Drawing.Size(294, 290);
            this.listBoxFilter.TabIndex = 3;
            this.listBoxFilter.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listBoxFilter_MouseDoubleClick);
            // 
            // trayIcon
            // 
            this.trayIcon.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.trayIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("trayIcon.Icon")));
            this.trayIcon.Text = "Metrics";
            this.trayIcon.DoubleClick += new System.EventHandler(this.trayIcon_DoubleClick);
            // 
            // txbAddFilter
            // 
            this.txbAddFilter.Location = new System.Drawing.Point(6, 19);
            this.txbAddFilter.Name = "txbAddFilter";
            this.txbAddFilter.Size = new System.Drawing.Size(227, 20);
            this.txbAddFilter.TabIndex = 4;
            // 
            // btnAddFilter
            // 
            this.btnAddFilter.Location = new System.Drawing.Point(240, 15);
            this.btnAddFilter.Name = "btnAddFilter";
            this.btnAddFilter.Size = new System.Drawing.Size(60, 27);
            this.btnAddFilter.TabIndex = 5;
            this.btnAddFilter.Text = "Add";
            this.btnAddFilter.UseVisualStyleBackColor = true;
            this.btnAddFilter.Click += new System.EventHandler(this.btnAddFilter_Click);
            // 
            // groupBoxFiltering
            // 
            this.groupBoxFiltering.Controls.Add(this.txbAddFilter);
            this.groupBoxFiltering.Controls.Add(this.listBoxFilter);
            this.groupBoxFiltering.Controls.Add(this.btnAddFilter);
            this.groupBoxFiltering.Location = new System.Drawing.Point(12, 138);
            this.groupBoxFiltering.Name = "groupBoxFiltering";
            this.groupBoxFiltering.Size = new System.Drawing.Size(306, 346);
            this.groupBoxFiltering.TabIndex = 6;
            this.groupBoxFiltering.TabStop = false;
            this.groupBoxFiltering.Text = "Filtering";
            // 
            // btnTransmit
            // 
            this.btnTransmit.Location = new System.Drawing.Point(12, 490);
            this.btnTransmit.Name = "btnTransmit";
            this.btnTransmit.Size = new System.Drawing.Size(306, 38);
            this.btnTransmit.TabIndex = 7;
            this.btnTransmit.Text = "Transmit to server";
            this.btnTransmit.UseVisualStyleBackColor = true;
            this.btnTransmit.Click += new System.EventHandler(this.btnTransmit_Click);
            // 
            // groupBoxCollectionSettings
            // 
            this.groupBoxCollectionSettings.Controls.Add(this.chbEnableStateScanning);
            this.groupBoxCollectionSettings.Controls.Add(this.chbEnableLeftClickTracking);
            this.groupBoxCollectionSettings.Controls.Add(this.chbEnableForegroundWindowChangeTracking);
            this.groupBoxCollectionSettings.Location = new System.Drawing.Point(12, 45);
            this.groupBoxCollectionSettings.Name = "groupBoxCollectionSettings";
            this.groupBoxCollectionSettings.Size = new System.Drawing.Size(306, 87);
            this.groupBoxCollectionSettings.TabIndex = 8;
            this.groupBoxCollectionSettings.TabStop = false;
            this.groupBoxCollectionSettings.Text = "Settings";
            // 
            // chbEnableStateScanning
            // 
            this.chbEnableStateScanning.AutoSize = true;
            this.chbEnableStateScanning.Checked = true;
            this.chbEnableStateScanning.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chbEnableStateScanning.Location = new System.Drawing.Point(6, 65);
            this.chbEnableStateScanning.Name = "chbEnableStateScanning";
            this.chbEnableStateScanning.Size = new System.Drawing.Size(99, 17);
            this.chbEnableStateScanning.TabIndex = 2;
            this.chbEnableStateScanning.Text = "State Scanning";
            this.chbEnableStateScanning.UseVisualStyleBackColor = true;
            // 
            // chbEnableLeftClickTracking
            // 
            this.chbEnableLeftClickTracking.AutoSize = true;
            this.chbEnableLeftClickTracking.Checked = true;
            this.chbEnableLeftClickTracking.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chbEnableLeftClickTracking.Location = new System.Drawing.Point(6, 42);
            this.chbEnableLeftClickTracking.Name = "chbEnableLeftClickTracking";
            this.chbEnableLeftClickTracking.Size = new System.Drawing.Size(150, 17);
            this.chbEnableLeftClickTracking.TabIndex = 1;
            this.chbEnableLeftClickTracking.Text = "Mouse Left Click Tracking";
            this.chbEnableLeftClickTracking.UseVisualStyleBackColor = true;
            // 
            // chbEnableForegroundWindowChangeTracking
            // 
            this.chbEnableForegroundWindowChangeTracking.AutoSize = true;
            this.chbEnableForegroundWindowChangeTracking.Checked = true;
            this.chbEnableForegroundWindowChangeTracking.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chbEnableForegroundWindowChangeTracking.Location = new System.Drawing.Point(6, 19);
            this.chbEnableForegroundWindowChangeTracking.Name = "chbEnableForegroundWindowChangeTracking";
            this.chbEnableForegroundWindowChangeTracking.Size = new System.Drawing.Size(207, 17);
            this.chbEnableForegroundWindowChangeTracking.TabIndex = 0;
            this.chbEnableForegroundWindowChangeTracking.Text = "Foreground Window Change Tracking";
            this.chbEnableForegroundWindowChangeTracking.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(327, 541);
            this.Controls.Add(this.groupBoxCollectionSettings);
            this.Controls.Add(this.btnTransmit);
            this.Controls.Add(this.groupBoxFiltering);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.btnStop);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "Metrics";
            this.Resize += new System.EventHandler(this.Form1_Resize);
            this.groupBoxFiltering.ResumeLayout(false);
            this.groupBoxFiltering.PerformLayout();
            this.groupBoxCollectionSettings.ResumeLayout(false);
            this.groupBoxCollectionSettings.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.ListBox listBoxFilter;
        private NotifyIcon trayIcon;
        private TextBox txbAddFilter;
        private Button btnAddFilter;
        private GroupBox groupBoxFiltering;
        private Button btnTransmit;
        private GroupBox groupBoxCollectionSettings;
        private CheckBox chbEnableStateScanning;
        private CheckBox chbEnableLeftClickTracking;
        private CheckBox chbEnableForegroundWindowChangeTracking;
    }
}

