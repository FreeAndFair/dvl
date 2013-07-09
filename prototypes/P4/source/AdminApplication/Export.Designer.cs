namespace AdminApplication
{
    using System.Windows.Forms;

    partial class ExportWindow
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

        public CheckBox PollingCards
        {
            get
            {
                return pollingCards;
            }
        }

        public CheckBox VoterLists
        {
            get
            {
                return voterLists;
            }
        }

        public CheckBox Voters
        {
            get
            {
                return voters;
            }
        }

        public Button ExportData
        {
            get
            {
                return exportData;
            }
        }

        public FolderBrowserDialog FolderBrowserDialog
        {
            get
            {
                return folderBrowserDialog;
            }
        }

        public Button Cancel
        {
            get
            {
                return cancel;
            }
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.pollingCards = new System.Windows.Forms.CheckBox();
            this.voterLists = new System.Windows.Forms.CheckBox();
            this.voters = new System.Windows.Forms.CheckBox();
            this.exportData = new System.Windows.Forms.Button();
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.cancel = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pollingCards
            // 
            this.pollingCards.AutoSize = true;
            this.pollingCards.Checked = true;
            this.pollingCards.CheckState = System.Windows.Forms.CheckState.Checked;
            this.pollingCards.Location = new System.Drawing.Point(25, 19);
            this.pollingCards.Name = "pollingCards";
            this.pollingCards.Size = new System.Drawing.Size(87, 17);
            this.pollingCards.TabIndex = 0;
            this.pollingCards.Text = "Polling Cards";
            this.pollingCards.UseVisualStyleBackColor = true;
            // 
            // voterLists
            // 
            this.voterLists.AutoSize = true;
            this.voterLists.Checked = true;
            this.voterLists.CheckState = System.Windows.Forms.CheckState.Checked;
            this.voterLists.Location = new System.Drawing.Point(25, 42);
            this.voterLists.Name = "voterLists";
            this.voterLists.Size = new System.Drawing.Size(75, 17);
            this.voterLists.TabIndex = 1;
            this.voterLists.Text = "Voter Lists";
            this.voterLists.UseVisualStyleBackColor = true;
            // 
            // voters
            // 
            this.voters.AutoSize = true;
            this.voters.Checked = true;
            this.voters.CheckState = System.Windows.Forms.CheckState.Checked;
            this.voters.Location = new System.Drawing.Point(25, 65);
            this.voters.Name = "voters";
            this.voters.Size = new System.Drawing.Size(56, 17);
            this.voters.TabIndex = 2;
            this.voters.Text = "Voters";
            this.voters.UseVisualStyleBackColor = true;
            // 
            // exportData
            // 
            this.exportData.Location = new System.Drawing.Point(16, 120);
            this.exportData.Name = "exportData";
            this.exportData.Size = new System.Drawing.Size(75, 23);
            this.exportData.TabIndex = 3;
            this.exportData.Text = "Export Data";
            this.exportData.UseVisualStyleBackColor = true;
            // 
            // cancel
            // 
            this.cancel.Location = new System.Drawing.Point(97, 120);
            this.cancel.Name = "cancel";
            this.cancel.Size = new System.Drawing.Size(75, 23);
            this.cancel.TabIndex = 4;
            this.cancel.Text = "Cancel";
            this.cancel.UseVisualStyleBackColor = true;
            this.cancel.Click += new System.EventHandler(this.button1_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.pollingCards);
            this.groupBox1.Controls.Add(this.voterLists);
            this.groupBox1.Controls.Add(this.voters);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(160, 93);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Elements To Export";
            // 
            // ExportWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(198, 169);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.exportData);
            this.Controls.Add(this.cancel);
            this.Name = "ExportWindow";
            this.Text = "Export";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckBox pollingCards;
        private System.Windows.Forms.CheckBox voterLists;
        private System.Windows.Forms.CheckBox voters;
        private System.Windows.Forms.Button exportData;
        private FolderBrowserDialog folderBrowserDialog;
        private Button cancel;
        private GroupBox groupBox1;
    }
}