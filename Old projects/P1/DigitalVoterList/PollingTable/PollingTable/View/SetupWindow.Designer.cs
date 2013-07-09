namespace PollingTable.PollingTable.View
{
    using System.Drawing;
    using System.Windows.Forms;

    partial class SetupWindow
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
            this.passwordBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tableBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.connectBtn = new System.Windows.Forms.Button();
            this.ipTextBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // passwordBox
            // 
            this.passwordBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.passwordBox.Location = new System.Drawing.Point(109, 58);
            this.passwordBox.Name = "passwordBox";
            this.passwordBox.PasswordChar = '*';
            this.passwordBox.Size = new System.Drawing.Size(112, 20);
            this.passwordBox.TabIndex = 2;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 35);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(75, 13);
            this.label3.TabIndex = 12;
            this.label3.Text = "Table number:";
            // 
            // tableBox
            // 
            this.tableBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tableBox.Location = new System.Drawing.Point(109, 32);
            this.tableBox.Name = "tableBox";
            this.tableBox.Size = new System.Drawing.Size(112, 20);
            this.tableBox.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 61);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(87, 13);
            this.label2.TabIndex = 10;
            this.label2.Text = "Admin password:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(81, 13);
            this.label1.TabIndex = 9;
            this.label1.Text = "Target address:";
            // 
            // connectBtn
            // 
            this.connectBtn.Location = new System.Drawing.Point(145, 84);
            this.connectBtn.Name = "connectBtn";
            this.connectBtn.Size = new System.Drawing.Size(75, 23);
            this.connectBtn.TabIndex = 3;
            this.connectBtn.Text = "Connect";
            this.connectBtn.UseVisualStyleBackColor = true;
            this.Closed += (o, Ea) => this.Form1_Closed();

            // 
            // ipTextBox
            // 
            this.ipTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ipTextBox.Location = new System.Drawing.Point(109, 6);
            this.ipTextBox.Name = "ipTextBox";
            this.ipTextBox.Size = new System.Drawing.Size(112, 20);
            this.ipTextBox.TabIndex = 0;
            // 
            // SetupWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(228, 112);
            this.Controls.Add(this.passwordBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.tableBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.connectBtn);
            this.Controls.Add(this.ipTextBox);
            this.Name = "SetupWindow";
            this.Text = "SetupWindow";
            this.ResumeLayout(false);
            this.PerformLayout();
            Size size = new Size(260, 160);
            this.MaximumSize = size;
            this.MinimumSize = size;
            this.Closed += (o, Ea) => this.Form1_Closed();

        }

        #endregion

        private System.Windows.Forms.TextBox passwordBox;

        public string TableBox
        {
            get
            {
                return this.tableBox.Text;
            }
            set
            {
                this.tableBox.Text = value;
            }
        }

        public string IpTextBox
        {
            get
            {
                return this.ipTextBox.Text;
            }
            set
            {
                this.ipTextBox.Text = value;
            }
        }

        public string Password
        {
            get
            {
                return this.passwordBox.Text;
            }
            set
            {
                passwordBox.Text = value;
            }
        }

        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tableBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button connectBtn;
        private System.Windows.Forms.TextBox ipTextBox;

        public Button ConnectBtn
        {
            get
            {
                return connectBtn;
            }
        }



    }
}