namespace Central.Central.Views
{
    partial class VoterCardGeneratorWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(VoterCardGeneratorWindow));
            this.btnAbort = new System.Windows.Forms.Button();
            this.btnGenerate = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.pbrGroups = new System.Windows.Forms.ProgressBar();
            this.label2 = new System.Windows.Forms.Label();
            this.pbrVoters = new System.Windows.Forms.ProgressBar();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.chbLimit = new System.Windows.Forms.CheckBox();
            this.chbProperty = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cbxProperty = new System.Windows.Forms.ComboBox();
            this.txbLimit = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.txbDestination = new System.Windows.Forms.TextBox();
            this.destinationFolderBrowser = new System.Windows.Forms.FolderBrowserDialog();
            this.lblStatus = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.txbCPR = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txbPollingStation = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txbMunicipality = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnAbort
            // 
            this.btnAbort.Location = new System.Drawing.Point(345, 265);
            this.btnAbort.Name = "btnAbort";
            this.btnAbort.Size = new System.Drawing.Size(80, 23);
            this.btnAbort.TabIndex = 19;
            this.btnAbort.Text = "Abort";
            this.btnAbort.UseVisualStyleBackColor = true;
            this.btnAbort.Visible = false;
            // 
            // btnGenerate
            // 
            this.btnGenerate.Location = new System.Drawing.Point(345, 265);
            this.btnGenerate.Name = "btnGenerate";
            this.btnGenerate.Size = new System.Drawing.Size(81, 23);
            this.btnGenerate.TabIndex = 15;
            this.btnGenerate.Text = "Generate";
            this.btnGenerate.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label3.Location = new System.Drawing.Point(12, 213);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(83, 13);
            this.label3.TabIndex = 14;
            this.label3.Text = "Overall progress";
            // 
            // pbrGroups
            // 
            this.pbrGroups.Location = new System.Drawing.Point(13, 229);
            this.pbrGroups.Name = "pbrGroups";
            this.pbrGroups.Size = new System.Drawing.Size(412, 23);
            this.pbrGroups.TabIndex = 13;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label2.Location = new System.Drawing.Point(12, 163);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(79, 13);
            this.label2.TabIndex = 12;
            this.label2.Text = "Group progress";
            // 
            // pbrVoters
            // 
            this.pbrVoters.Location = new System.Drawing.Point(12, 179);
            this.pbrVoters.Name = "pbrVoters";
            this.pbrVoters.Size = new System.Drawing.Size(413, 23);
            this.pbrVoters.TabIndex = 11;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.chbLimit);
            this.groupBox1.Controls.Add(this.chbProperty);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.cbxProperty);
            this.groupBox1.Controls.Add(this.txbLimit);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.groupBox1.Location = new System.Drawing.Point(180, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(245, 76);
            this.groupBox1.TabIndex = 10;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Group By";
            // 
            // chbLimit
            // 
            this.chbLimit.AutoSize = true;
            this.chbLimit.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chbLimit.ForeColor = System.Drawing.SystemColors.ControlText;
            this.chbLimit.Location = new System.Drawing.Point(8, 47);
            this.chbLimit.Name = "chbLimit";
            this.chbLimit.Size = new System.Drawing.Size(47, 17);
            this.chbLimit.TabIndex = 6;
            this.chbLimit.Text = "Limit";
            this.chbLimit.UseVisualStyleBackColor = true;
            // 
            // chbProperty
            // 
            this.chbProperty.AutoSize = true;
            this.chbProperty.Checked = true;
            this.chbProperty.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chbProperty.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chbProperty.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.chbProperty.Location = new System.Drawing.Point(8, 21);
            this.chbProperty.Name = "chbProperty";
            this.chbProperty.Size = new System.Drawing.Size(65, 17);
            this.chbProperty.TabIndex = 5;
            this.chbProperty.Text = "Property";
            this.chbProperty.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label1.Location = new System.Drawing.Point(161, 47);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(51, 12);
            this.label1.TabIndex = 4;
            this.label1.Text = "voters / file";
            // 
            // cbxProperty
            // 
            this.cbxProperty.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbxProperty.FormattingEnabled = true;
            this.cbxProperty.Items.AddRange(new object[] {
            "Municipality",
            "Polling Station",
            "Alphabet",
            "City"});
            this.cbxProperty.Location = new System.Drawing.Point(94, 19);
            this.cbxProperty.Name = "cbxProperty";
            this.cbxProperty.Size = new System.Drawing.Size(142, 21);
            this.cbxProperty.TabIndex = 3;
            this.cbxProperty.Text = "Municipality";
            // 
            // txbLimit
            // 
            this.txbLimit.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txbLimit.Location = new System.Drawing.Point(94, 45);
            this.txbLimit.Name = "txbLimit";
            this.txbLimit.Size = new System.Drawing.Size(65, 20);
            this.txbLimit.TabIndex = 2;
            this.txbLimit.Text = "1000";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnBrowse);
            this.groupBox2.Controls.Add(this.txbDestination);
            this.groupBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.groupBox2.Location = new System.Drawing.Point(180, 94);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(245, 56);
            this.groupBox2.TabIndex = 17;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Destination Folder";
            // 
            // btnBrowse
            // 
            this.btnBrowse.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBrowse.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnBrowse.Location = new System.Drawing.Point(179, 20);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(56, 23);
            this.btnBrowse.TabIndex = 1;
            this.btnBrowse.Text = "Browse";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.Browse);
            // 
            // txbDestination
            // 
            this.txbDestination.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txbDestination.Location = new System.Drawing.Point(8, 22);
            this.txbDestination.Name = "txbDestination";
            this.txbDestination.Size = new System.Drawing.Size(166, 20);
            this.txbDestination.TabIndex = 0;
            this.txbDestination.Text = "C:\\VoterCards";
            // 
            // destinationFolderBrowser
            // 
            this.destinationFolderBrowser.SelectedPath = "C:\\";
            // 
            // lblCurrentGroup
            // 
            this.lblStatus.Location = new System.Drawing.Point(100, 163);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(325, 13);
            this.lblStatus.TabIndex = 18;
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.txbCPR);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.txbPollingStation);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.txbMunicipality);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox3.ForeColor = System.Drawing.SystemColors.ControlText;
            this.groupBox3.Location = new System.Drawing.Point(12, 12);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(162, 138);
            this.groupBox3.TabIndex = 20;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Voters Selected";
            // 
            // txbCPR
            // 
            this.txbCPR.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txbCPR.Location = new System.Drawing.Point(6, 110);
            this.txbCPR.Name = "txbCPR";
            this.txbCPR.ReadOnly = true;
            this.txbCPR.Size = new System.Drawing.Size(150, 20);
            this.txbCPR.TabIndex = 5;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.label6.Location = new System.Drawing.Point(6, 97);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(58, 12);
            this.label6.TabIndex = 4;
            this.label6.Text = "CPR number";
            // 
            // txbPollingStation
            // 
            this.txbPollingStation.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txbPollingStation.Location = new System.Drawing.Point(6, 71);
            this.txbPollingStation.Name = "txbPollingStation";
            this.txbPollingStation.ReadOnly = true;
            this.txbPollingStation.Size = new System.Drawing.Size(150, 20);
            this.txbPollingStation.TabIndex = 3;
            this.txbPollingStation.Text = "All";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.label5.Location = new System.Drawing.Point(6, 58);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(63, 12);
            this.label5.TabIndex = 2;
            this.label5.Text = "Polling Station";
            // 
            // txbMunicipality
            // 
            this.txbMunicipality.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txbMunicipality.Location = new System.Drawing.Point(6, 32);
            this.txbMunicipality.Name = "txbMunicipality";
            this.txbMunicipality.ReadOnly = true;
            this.txbMunicipality.Size = new System.Drawing.Size(150, 20);
            this.txbMunicipality.TabIndex = 1;
            this.txbMunicipality.Text = "All";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.label4.Location = new System.Drawing.Point(6, 19);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(55, 12);
            this.label4.TabIndex = 0;
            this.label4.Text = "Municipality";
            // 
            // VoterCardGeneratorWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(437, 299);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.btnAbort);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.btnGenerate);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.pbrGroups);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.pbrVoters);
            this.Controls.Add(this.groupBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(453, 337);
            this.MinimumSize = new System.Drawing.Size(453, 337);
            this.Name = "VoterCardGeneratorWindow";
            this.Text = "Voter Card Generator";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnAbort;
        private System.Windows.Forms.Button btnGenerate;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ProgressBar pbrGroups;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ProgressBar pbrVoters;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cbxProperty;
        private System.Windows.Forms.TextBox txbLimit;
        private System.Windows.Forms.CheckBox chbLimit;
        private System.Windows.Forms.CheckBox chbProperty;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.TextBox txbDestination;
        private System.Windows.Forms.FolderBrowserDialog destinationFolderBrowser;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txbCPR;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txbPollingStation;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txbMunicipality;
    }
}