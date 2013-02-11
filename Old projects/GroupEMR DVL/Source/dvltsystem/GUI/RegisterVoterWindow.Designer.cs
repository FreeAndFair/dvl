namespace DVLTerminal.GUI
{
    partial class RegisterVoterWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RegisterVoterWindow));
            this.submitVoteButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.voteUpdatedTimer = new System.Windows.Forms.Timer(this.components);
            this.textBoxVotingNum = new System.Windows.Forms.TextBox();
            this.textBoxCPRNum = new System.Windows.Forms.TextBox();
            this.buttonEndElection = new System.Windows.Forms.Button();
            this.getVotersCount = new System.Windows.Forms.Timer(this.components);
            this.labelCountVoted = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.backgroundPicture = new System.Windows.Forms.PictureBox();
            this.questionmark1 = new System.Windows.Forms.PictureBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.voteProgressBar = new System.Windows.Forms.ProgressBar();
            this.connectedComputersLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.backgroundPicture)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.questionmark1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // submitVoteButton
            // 
            this.submitVoteButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.submitVoteButton.Location = new System.Drawing.Point(160, 266);
            this.submitVoteButton.Name = "submitVoteButton";
            this.submitVoteButton.Size = new System.Drawing.Size(310, 32);
            this.submitVoteButton.TabIndex = 3;
            this.submitVoteButton.Text = "Submit";
            this.submitVoteButton.UseVisualStyleBackColor = true;
            this.submitVoteButton.Click += new System.EventHandler(this.SubmitVoteButtonClick);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(160, 101);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(113, 20);
            this.label1.TabIndex = 8;
            this.label1.Text = "Voting number";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(160, 179);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(195, 20);
            this.label3.TabIndex = 13;
            this.label3.Text = "Four last digits in CPR no.:";
            // 
            // voteUpdatedTimer
            // 
            this.voteUpdatedTimer.Interval = 20;
            this.voteUpdatedTimer.Tick += new System.EventHandler(this.voteUpdatedTimerTick);
            // 
            // textBoxVotingNum
            // 
            this.textBoxVotingNum.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxVotingNum.Location = new System.Drawing.Point(160, 124);
            this.textBoxVotingNum.MaxLength = 10;
            this.textBoxVotingNum.Name = "textBoxVotingNum";
            this.textBoxVotingNum.Size = new System.Drawing.Size(279, 26);
            this.textBoxVotingNum.TabIndex = 1;
            this.textBoxVotingNum.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBoxVotingNum_KeyDown);
            // 
            // textBoxCPRNum
            // 
            this.textBoxCPRNum.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxCPRNum.Location = new System.Drawing.Point(160, 202);
            this.textBoxCPRNum.MaxLength = 4;
            this.textBoxCPRNum.Name = "textBoxCPRNum";
            this.textBoxCPRNum.Size = new System.Drawing.Size(279, 26);
            this.textBoxCPRNum.TabIndex = 2;
            this.textBoxCPRNum.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBoxCPRNum_KeyDown);
            // 
            // buttonEndElection
            // 
            this.buttonEndElection.Location = new System.Drawing.Point(535, 429);
            this.buttonEndElection.Name = "buttonEndElection";
            this.buttonEndElection.Size = new System.Drawing.Size(87, 31);
            this.buttonEndElection.TabIndex = 17;
            this.buttonEndElection.TabStop = false;
            this.buttonEndElection.Text = "End election";
            this.buttonEndElection.UseVisualStyleBackColor = true;
            this.buttonEndElection.Click += new System.EventHandler(this.buttonEndElection_Click);
            // 
            // getVotersCount
            // 
            this.getVotersCount.Interval = 1000;
            this.getVotersCount.Tick += new System.EventHandler(this.getVotersCount_Tick);
            // 
            // labelCountVoted
            // 
            this.labelCountVoted.AutoSize = true;
            this.labelCountVoted.Location = new System.Drawing.Point(161, 450);
            this.labelCountVoted.Name = "labelCountVoted";
            this.labelCountVoted.Size = new System.Drawing.Size(156, 13);
            this.labelCountVoted.TabIndex = 18;
            this.labelCountVoted.Text = "Currently n/a people has voted.";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(157, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(197, 33);
            this.label2.TabIndex = 20;
            this.label2.Text = "Register voter";
            // 
            // backgroundPicture
            // 
            this.backgroundPicture.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.backgroundPicture.Image = global::DVLTerminal.Properties.Resources.blue;
            this.backgroundPicture.Location = new System.Drawing.Point(0, 0);
            this.backgroundPicture.Name = "backgroundPicture";
            this.backgroundPicture.Size = new System.Drawing.Size(154, 475);
            this.backgroundPicture.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.backgroundPicture.TabIndex = 19;
            this.backgroundPicture.TabStop = false;
            // 
            // questionmark1
            // 
            this.questionmark1.Image = global::DVLTerminal.Properties.Resources.question;
            this.questionmark1.Location = new System.Drawing.Point(445, 125);
            this.questionmark1.Name = "questionmark1";
            this.questionmark1.Size = new System.Drawing.Size(25, 25);
            this.questionmark1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.questionmark1.TabIndex = 21;
            this.questionmark1.TabStop = false;
            this.toolTip1.SetToolTip(this.questionmark1, "Scan the barcode on the voting card,\r\nor enter manually if barcode is damaged.");
            // 
            // toolTip1
            // 
            this.toolTip1.IsBalloon = true;
            this.toolTip1.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.toolTip1.ToolTipTitle = "Help";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::DVLTerminal.Properties.Resources.question;
            this.pictureBox1.Location = new System.Drawing.Point(445, 203);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(25, 25);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 21;
            this.pictureBox1.TabStop = false;
            this.toolTip1.SetToolTip(this.pictureBox1, "Ask the voter for the last 4 digits\r\nin their CPR number and enter them here.");
            // 
            // voteProgressBar
            // 
            this.voteProgressBar.Location = new System.Drawing.Point(160, 432);
            this.voteProgressBar.Name = "voteProgressBar";
            this.voteProgressBar.Size = new System.Drawing.Size(310, 15);
            this.voteProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.voteProgressBar.TabIndex = 22;
            // 
            // connectedComputersLabel
            // 
            this.connectedComputersLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.connectedComputersLabel.Location = new System.Drawing.Point(330, 450);
            this.connectedComputersLabel.Name = "connectedComputersLabel";
            this.connectedComputersLabel.Size = new System.Drawing.Size(140, 23);
            this.connectedComputersLabel.TabIndex = 23;
            this.connectedComputersLabel.Text = "Connected to: n/a PCs";
            this.connectedComputersLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // RegisterVoterWindow
            // 
            this.AcceptButton = this.submitVoteButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(634, 472);
            this.Controls.Add(this.connectedComputersLabel);
            this.Controls.Add(this.voteProgressBar);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.questionmark1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.labelCountVoted);
            this.Controls.Add(this.buttonEndElection);
            this.Controls.Add(this.textBoxCPRNum);
            this.Controls.Add(this.textBoxVotingNum);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.submitVoteButton);
            this.Controls.Add(this.backgroundPicture);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(640, 500);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(640, 500);
            this.Name = "RegisterVoterWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "DVL System";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.RegisterVoterWindow_FormClosing);
            this.Load += new System.EventHandler(this.RegisterVoterWindow_Load);
            ((System.ComponentModel.ISupportInitialize)(this.backgroundPicture)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.questionmark1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button submitVoteButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Timer voteUpdatedTimer;
        private System.Windows.Forms.TextBox textBoxVotingNum;
        private System.Windows.Forms.TextBox textBoxCPRNum;
        private System.Windows.Forms.Button buttonEndElection;
        private System.Windows.Forms.Timer getVotersCount;
        private System.Windows.Forms.Label labelCountVoted;
        private System.Windows.Forms.PictureBox backgroundPicture;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.PictureBox questionmark1;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.ProgressBar voteProgressBar;
        private System.Windows.Forms.Label connectedComputersLabel;
    }
}

