namespace Central.Central.Views
{

    partial class VoterBoxManagerWindow
    {
        public VoterBoxManagerWindow()
        {
            this.progressBar1.Step = 45;
        }

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(VoterBoxManagerWindow));
            this.label2 = new System.Windows.Forms.Label();
            this.connectButton = new System.Windows.Forms.Button();
            this.adressTB = new System.Windows.Forms.TextBox();
            this.addressLabel = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.progressTF = new System.Windows.Forms.TextBox();
            this.uploadButton = new System.Windows.Forms.Button();
            this.validateButton = new System.Windows.Forms.Button();
            this.userBox = new System.Windows.Forms.TextBox();
            this.userLabel = new System.Windows.Forms.Label();
            this.pwBox = new System.Windows.Forms.TextBox();
            this.pwLabel = new System.Windows.Forms.Label();
            this.portBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(11, 206);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(51, 13);
            this.label2.TabIndex = 15;
            this.label2.Text = "Progress:";
            // 
            // connectButton
            // 
            this.connectButton.Location = new System.Drawing.Point(225, 11);
            this.connectButton.Name = "connectButton";
            this.connectButton.Size = new System.Drawing.Size(75, 74);
            this.connectButton.TabIndex = 5;
            this.connectButton.Text = "Connect";
            this.connectButton.UseVisualStyleBackColor = true;
            // 
            // adressTB
            // 
            this.adressTB.Location = new System.Drawing.Point(98, 13);
            this.adressTB.Name = "adressTB";
            this.adressTB.Size = new System.Drawing.Size(75, 20);
            this.adressTB.TabIndex = 0;
            this.adressTB.Text = "192.168.20.11";
            // 
            // addressLabel
            // 
            this.addressLabel.AutoSize = true;
            this.addressLabel.Location = new System.Drawing.Point(11, 16);
            this.addressLabel.Name = "addressLabel";
            this.addressLabel.Size = new System.Drawing.Size(81, 13);
            this.addressLabel.TabIndex = 8;
            this.addressLabel.Text = "Target address:";
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(14, 221);
            this.progressBar1.Maximum = 4;
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(284, 26);
            this.progressBar1.Step = 1;
            this.progressBar1.TabIndex = 11;
            // 
            // progressTF
            // 
            this.progressTF.Enabled = false;
            this.progressTF.Location = new System.Drawing.Point(14, 91);
            this.progressTF.Multiline = true;
            this.progressTF.Name = "progressTF";
            this.progressTF.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.progressTF.Size = new System.Drawing.Size(286, 114);
            this.progressTF.TabIndex = 12;
            // 
            // uploadButton
            // 
            this.uploadButton.Location = new System.Drawing.Point(235, 250);
            this.uploadButton.Name = "uploadButton";
            this.uploadButton.Size = new System.Drawing.Size(63, 26);
            this.uploadButton.TabIndex = 13;
            this.uploadButton.Text = "Upload";
            this.uploadButton.UseVisualStyleBackColor = true;
            // 
            // validateButton
            // 
            this.validateButton.Location = new System.Drawing.Point(161, 250);
            this.validateButton.Name = "validateButton";
            this.validateButton.Size = new System.Drawing.Size(68, 26);
            this.validateButton.TabIndex = 14;
            this.validateButton.Text = "Validate";
            this.validateButton.UseVisualStyleBackColor = true;
            // 
            // userBox
            // 
            this.userBox.Location = new System.Drawing.Point(96, 39);
            this.userBox.Name = "userBox";
            this.userBox.Size = new System.Drawing.Size(119, 20);
            this.userBox.TabIndex = 2;
            // 
            // userLabel
            // 
            this.userLabel.AutoSize = true;
            this.userLabel.Location = new System.Drawing.Point(11, 42);
            this.userLabel.Name = "userLabel";
            this.userLabel.Size = new System.Drawing.Size(32, 13);
            this.userLabel.TabIndex = 17;
            this.userLabel.Text = "User:";
            // 
            // pwBox
            // 
            this.pwBox.ForeColor = System.Drawing.SystemColors.WindowText;
            this.pwBox.Location = new System.Drawing.Point(96, 65);
            this.pwBox.Name = "pwBox";
            this.pwBox.PasswordChar = '*';
            this.pwBox.Size = new System.Drawing.Size(119, 20);
            this.pwBox.TabIndex = 3;
            // 
            // pwLabel
            // 
            this.pwLabel.AutoSize = true;
            this.pwLabel.Location = new System.Drawing.Point(11, 68);
            this.pwLabel.Name = "pwLabel";
            this.pwLabel.Size = new System.Drawing.Size(56, 13);
            this.pwLabel.TabIndex = 19;
            this.pwLabel.Text = "Password:";
            // 
            // portBox
            // 
            this.portBox.Location = new System.Drawing.Point(178, 13);
            this.portBox.Name = "portBox";
            this.portBox.Size = new System.Drawing.Size(37, 20);
            this.portBox.TabIndex = 1;
            this.portBox.Text = "3306";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(171, 15);
            this.label1.Margin = new System.Windows.Forms.Padding(0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(10, 13);
            this.label1.TabIndex = 21;
            this.label1.Text = ":";
            // 
            // VoterBoxManagerWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(311, 281);
            this.Controls.Add(this.portBox);
            this.Controls.Add(this.adressTB);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pwLabel);
            this.Controls.Add(this.pwBox);
            this.Controls.Add(this.userLabel);
            this.Controls.Add(this.userBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.validateButton);
            this.Controls.Add(this.uploadButton);
            this.Controls.Add(this.progressTF);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.connectButton);
            this.Controls.Add(this.addressLabel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "VoterBoxManagerWindow";
            this.Text = "Voter Box Manager";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button connectButton;
        private System.Windows.Forms.TextBox adressTB;
        private System.Windows.Forms.Label addressLabel;

        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.TextBox progressTF;
        private System.Windows.Forms.Button uploadButton;
        private System.Windows.Forms.Button validateButton;
        private System.Windows.Forms.TextBox userBox;
        private System.Windows.Forms.Label userLabel;
        private System.Windows.Forms.TextBox pwBox;
        private System.Windows.Forms.Label pwLabel;
        private System.Windows.Forms.TextBox portBox;
        private System.Windows.Forms.Label label1;
    }
}