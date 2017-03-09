﻿namespace MetricsCollectorApplication
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
            this.buttonStart = new System.Windows.Forms.Button();
            this.buttonStop = new System.Windows.Forms.Button();
            this.groupBoxSettings = new System.Windows.Forms.GroupBox();
            this.checkBoxForegroundWindowChangeTracking = new System.Windows.Forms.CheckBox();
            this.checkBoxMouseLeftClickTracking = new System.Windows.Forms.CheckBox();
            this.checkBoxStateScanning = new System.Windows.Forms.CheckBox();
            this.trayIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.groupBoxSettings.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonStart
            // 
            this.buttonStart.Location = new System.Drawing.Point(12, 12);
            this.buttonStart.Name = "buttonStart";
            this.buttonStart.Size = new System.Drawing.Size(118, 23);
            this.buttonStart.TabIndex = 0;
            this.buttonStart.Text = "Start Tracking";
            this.buttonStart.UseVisualStyleBackColor = true;
            this.buttonStart.Click += new System.EventHandler(this.buttonStart_Click);
            // 
            // buttonStop
            // 
            this.buttonStop.Location = new System.Drawing.Point(154, 12);
            this.buttonStop.Name = "buttonStop";
            this.buttonStop.Size = new System.Drawing.Size(118, 23);
            this.buttonStop.TabIndex = 1;
            this.buttonStop.Text = "Stop Tracking";
            this.buttonStop.UseVisualStyleBackColor = true;
            this.buttonStop.Click += new System.EventHandler(this.buttonStop_Click);
            // 
            // groupBoxSettings
            // 
            this.groupBoxSettings.Controls.Add(this.checkBoxStateScanning);
            this.groupBoxSettings.Controls.Add(this.checkBoxMouseLeftClickTracking);
            this.groupBoxSettings.Controls.Add(this.checkBoxForegroundWindowChangeTracking);
            this.groupBoxSettings.Location = new System.Drawing.Point(12, 41);
            this.groupBoxSettings.Name = "groupBoxSettings";
            this.groupBoxSettings.Size = new System.Drawing.Size(260, 92);
            this.groupBoxSettings.TabIndex = 2;
            this.groupBoxSettings.TabStop = false;
            this.groupBoxSettings.Text = "Settings";
            // 
            // checkBoxForegroundWindowChangeTracking
            // 
            this.checkBoxForegroundWindowChangeTracking.AutoSize = true;
            this.checkBoxForegroundWindowChangeTracking.Checked = true;
            this.checkBoxForegroundWindowChangeTracking.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxForegroundWindowChangeTracking.Location = new System.Drawing.Point(6, 19);
            this.checkBoxForegroundWindowChangeTracking.Name = "checkBoxForegroundWindowChangeTracking";
            this.checkBoxForegroundWindowChangeTracking.Size = new System.Drawing.Size(207, 17);
            this.checkBoxForegroundWindowChangeTracking.TabIndex = 0;
            this.checkBoxForegroundWindowChangeTracking.Text = "Foreground Window Change Tracking";
            this.checkBoxForegroundWindowChangeTracking.UseVisualStyleBackColor = true;
            // 
            // checkBoxMouseLeftClickTracking
            // 
            this.checkBoxMouseLeftClickTracking.AutoSize = true;
            this.checkBoxMouseLeftClickTracking.Checked = true;
            this.checkBoxMouseLeftClickTracking.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxMouseLeftClickTracking.Location = new System.Drawing.Point(6, 42);
            this.checkBoxMouseLeftClickTracking.Name = "checkBoxMouseLeftClickTracking";
            this.checkBoxMouseLeftClickTracking.Size = new System.Drawing.Size(150, 17);
            this.checkBoxMouseLeftClickTracking.TabIndex = 1;
            this.checkBoxMouseLeftClickTracking.Text = "Mouse Left Click Tracking";
            this.checkBoxMouseLeftClickTracking.UseVisualStyleBackColor = true;
            // 
            // checkBoxStateScanning
            // 
            this.checkBoxStateScanning.AutoSize = true;
            this.checkBoxStateScanning.Checked = true;
            this.checkBoxStateScanning.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxStateScanning.Location = new System.Drawing.Point(6, 65);
            this.checkBoxStateScanning.Name = "checkBoxStateScanning";
            this.checkBoxStateScanning.Size = new System.Drawing.Size(99, 17);
            this.checkBoxStateScanning.TabIndex = 2;
            this.checkBoxStateScanning.Text = "State Scanning";
            this.checkBoxStateScanning.UseVisualStyleBackColor = true;
            // 
            // trayIcon
            // 
            this.trayIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("trayIcon.Icon")));
            this.trayIcon.Text = "trayIcon";
            this.trayIcon.DoubleClick += new System.EventHandler(this.trayIcon_DoubleClick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 141);
            this.Controls.Add(this.groupBoxSettings);
            this.Controls.Add(this.buttonStop);
            this.Controls.Add(this.buttonStart);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "Metrics";
            this.Resize += new System.EventHandler(this.Form1_Resize);
            this.groupBoxSettings.ResumeLayout(false);
            this.groupBoxSettings.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonStart;
        private System.Windows.Forms.Button buttonStop;
        private System.Windows.Forms.GroupBox groupBoxSettings;
        private System.Windows.Forms.CheckBox checkBoxStateScanning;
        private System.Windows.Forms.CheckBox checkBoxMouseLeftClickTracking;
        private System.Windows.Forms.CheckBox checkBoxForegroundWindowChangeTracking;
        private System.Windows.Forms.NotifyIcon trayIcon;
    }
}
