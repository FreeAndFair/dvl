namespace ClientApplication
{
    partial class MainForm
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
            this.logButton = new System.Windows.Forms.Button();
            this.thisTableLabel = new System.Windows.Forms.Label();
            this.idTextBox = new System.Windows.Forms.TextBox();
            this.cprTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.searchIdButton = new System.Windows.Forms.Button();
            this.searchCprButton = new System.Windows.Forms.Button();
            this.registerButton = new System.Windows.Forms.Button();
            this.unregisterButton = new System.Windows.Forms.Button();
            this.clearButton = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.timeLabel = new System.Windows.Forms.Label();
            this.tableLabel = new System.Windows.Forms.Label();
            this.votedLabel = new System.Windows.Forms.Label();
            this.cprLabel = new System.Windows.Forms.Label();
            this.lastNameLabel = new System.Windows.Forms.Label();
            this.firstNameLabel = new System.Windows.Forms.Label();
            this.idLabel = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // logButton
            // 
            this.logButton.Location = new System.Drawing.Point(9, 48);
            this.logButton.Name = "logButton";
            this.logButton.Size = new System.Drawing.Size(75, 23);
            this.logButton.TabIndex = 1;
            this.logButton.Text = "Log";
            this.logButton.UseVisualStyleBackColor = true;
            // 
            // thisTableLabel
            // 
            this.thisTableLabel.AutoSize = true;
            this.thisTableLabel.Location = new System.Drawing.Point(6, 22);
            this.thisTableLabel.Name = "thisTableLabel";
            this.thisTableLabel.Size = new System.Drawing.Size(43, 13);
            this.thisTableLabel.TabIndex = 2;
            this.thisTableLabel.Text = "Table ?";
            // 
            // idTextBox
            // 
            this.idTextBox.Location = new System.Drawing.Point(74, 20);
            this.idTextBox.Name = "idTextBox";
            this.idTextBox.Size = new System.Drawing.Size(100, 20);
            this.idTextBox.TabIndex = 3;
            // 
            // cprTextBox
            // 
            this.cprTextBox.Location = new System.Drawing.Point(75, 52);
            this.cprTextBox.Name = "cprTextBox";
            this.cprTextBox.Size = new System.Drawing.Size(100, 20);
            this.cprTextBox.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(14, 25);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(18, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "ID";
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(15, 59);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(43, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Cpr. Nr.";
            // 
            // searchIdButton
            // 
            this.searchIdButton.Location = new System.Drawing.Point(191, 18);
            this.searchIdButton.Name = "searchIdButton";
            this.searchIdButton.Size = new System.Drawing.Size(51, 23);
            this.searchIdButton.TabIndex = 7;
            this.searchIdButton.Text = "Search";
            this.searchIdButton.UseVisualStyleBackColor = true;
            // 
            // searchCprButton
            // 
            this.searchCprButton.Location = new System.Drawing.Point(192, 52);
            this.searchCprButton.Name = "searchCprButton";
            this.searchCprButton.Size = new System.Drawing.Size(50, 23);
            this.searchCprButton.TabIndex = 8;
            this.searchCprButton.Text = "Search";
            this.searchCprButton.UseVisualStyleBackColor = true;
            // 
            // registerButton
            // 
            this.registerButton.Location = new System.Drawing.Point(133, 179);
            this.registerButton.Name = "registerButton";
            this.registerButton.Size = new System.Drawing.Size(75, 23);
            this.registerButton.TabIndex = 10;
            this.registerButton.Text = "Register";
            this.registerButton.UseVisualStyleBackColor = true;
            // 
            // unregisterButton
            // 
            this.unregisterButton.Location = new System.Drawing.Point(214, 179);
            this.unregisterButton.Name = "unregisterButton";
            this.unregisterButton.Size = new System.Drawing.Size(75, 23);
            this.unregisterButton.TabIndex = 11;
            this.unregisterButton.Text = "Unregister";
            this.unregisterButton.UseVisualStyleBackColor = true;
            // 
            // clearButton
            // 
            this.clearButton.Location = new System.Drawing.Point(295, 179);
            this.clearButton.Name = "clearButton";
            this.clearButton.Size = new System.Drawing.Size(75, 23);
            this.clearButton.TabIndex = 12;
            this.clearButton.Text = "Clear";
            this.clearButton.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.idTextBox);
            this.groupBox1.Controls.Add(this.cprTextBox);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.searchIdButton);
            this.groupBox1.Controls.Add(this.searchCprButton);
            this.groupBox1.Location = new System.Drawing.Point(15, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(255, 90);
            this.groupBox1.TabIndex = 13;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Find voter";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.thisTableLabel);
            this.groupBox2.Controls.Add(this.logButton);
            this.groupBox2.Location = new System.Drawing.Point(292, 15);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(106, 88);
            this.groupBox2.TabIndex = 14;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "This table";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.timeLabel);
            this.groupBox3.Controls.Add(this.tableLabel);
            this.groupBox3.Controls.Add(this.votedLabel);
            this.groupBox3.Controls.Add(this.cprLabel);
            this.groupBox3.Controls.Add(this.lastNameLabel);
            this.groupBox3.Controls.Add(this.firstNameLabel);
            this.groupBox3.Controls.Add(this.idLabel);
            this.groupBox3.Controls.Add(this.label10);
            this.groupBox3.Controls.Add(this.label9);
            this.groupBox3.Controls.Add(this.label8);
            this.groupBox3.Controls.Add(this.label7);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.registerButton);
            this.groupBox3.Controls.Add(this.unregisterButton);
            this.groupBox3.Controls.Add(this.clearButton);
            this.groupBox3.Location = new System.Drawing.Point(15, 109);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(383, 214);
            this.groupBox3.TabIndex = 15;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Voter";
            // 
            // timeLabel
            // 
            this.timeLabel.BackColor = System.Drawing.SystemColors.Window;
            this.timeLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.timeLabel.Location = new System.Drawing.Point(270, 135);
            this.timeLabel.Name = "timeLabel";
            this.timeLabel.Size = new System.Drawing.Size(100, 23);
            this.timeLabel.TabIndex = 33;
            this.timeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tableLabel
            // 
            this.tableLabel.BackColor = System.Drawing.SystemColors.Window;
            this.tableLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.tableLabel.Location = new System.Drawing.Point(139, 135);
            this.tableLabel.Name = "tableLabel";
            this.tableLabel.Size = new System.Drawing.Size(100, 23);
            this.tableLabel.TabIndex = 32;
            this.tableLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // votedLabel
            // 
            this.votedLabel.BackColor = System.Drawing.SystemColors.Window;
            this.votedLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.votedLabel.Location = new System.Drawing.Point(13, 135);
            this.votedLabel.Name = "votedLabel";
            this.votedLabel.Size = new System.Drawing.Size(100, 23);
            this.votedLabel.TabIndex = 31;
            this.votedLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cprLabel
            // 
            this.cprLabel.BackColor = System.Drawing.SystemColors.Window;
            this.cprLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.cprLabel.Location = new System.Drawing.Point(270, 84);
            this.cprLabel.Name = "cprLabel";
            this.cprLabel.Size = new System.Drawing.Size(100, 23);
            this.cprLabel.TabIndex = 30;
            this.cprLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lastNameLabel
            // 
            this.lastNameLabel.BackColor = System.Drawing.SystemColors.Window;
            this.lastNameLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lastNameLabel.Location = new System.Drawing.Point(139, 84);
            this.lastNameLabel.Name = "lastNameLabel";
            this.lastNameLabel.Size = new System.Drawing.Size(100, 23);
            this.lastNameLabel.TabIndex = 29;
            this.lastNameLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // firstNameLabel
            // 
            this.firstNameLabel.BackColor = System.Drawing.SystemColors.Window;
            this.firstNameLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.firstNameLabel.Location = new System.Drawing.Point(10, 84);
            this.firstNameLabel.Name = "firstNameLabel";
            this.firstNameLabel.Size = new System.Drawing.Size(100, 23);
            this.firstNameLabel.TabIndex = 28;
            this.firstNameLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // idLabel
            // 
            this.idLabel.BackColor = System.Drawing.SystemColors.Window;
            this.idLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.idLabel.Location = new System.Drawing.Point(10, 37);
            this.idLabel.Name = "idLabel";
            this.idLabel.Size = new System.Drawing.Size(100, 23);
            this.idLabel.TabIndex = 27;
            this.idLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(270, 122);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(30, 13);
            this.label10.TabIndex = 25;
            this.label10.Text = "Time";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(139, 122);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(34, 13);
            this.label9.TabIndex = 23;
            this.label9.Text = "Table";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(267, 71);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(23, 13);
            this.label8.TabIndex = 21;
            this.label8.Text = "Cpr";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(136, 71);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(58, 13);
            this.label7.TabIndex = 19;
            this.label7.Text = "Last Name";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(10, 122);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(35, 13);
            this.label6.TabIndex = 17;
            this.label6.Text = "Voted";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(7, 71);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(57, 13);
            this.label5.TabIndex = 15;
            this.label5.Text = "First Name";
            this.label5.Click += new System.EventHandler(this.label5_Click_1);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(7, 20);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(18, 13);
            this.label4.TabIndex = 13;
            this.label4.Text = "ID";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(412, 338);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "MainForm";
            this.Text = "Digital Voter Registration System";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button logButton;
        private System.Windows.Forms.Label thisTableLabel;
        private System.Windows.Forms.TextBox idTextBox;
        private System.Windows.Forms.TextBox cprTextBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button searchIdButton;
        private System.Windows.Forms.Button searchCprButton;
        private System.Windows.Forms.Button registerButton;
        private System.Windows.Forms.Button unregisterButton;
        private System.Windows.Forms.Button clearButton;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label idLabel;
        private System.Windows.Forms.Label timeLabel;
        private System.Windows.Forms.Label tableLabel;
        private System.Windows.Forms.Label votedLabel;
        private System.Windows.Forms.Label cprLabel;
        private System.Windows.Forms.Label lastNameLabel;
        private System.Windows.Forms.Label firstNameLabel;
    }
}