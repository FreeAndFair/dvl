namespace PollingTable.PollingTable.View
{
    using System.Windows.Forms;

    using global::PollingTable.PollingTable.View.Root_elements;

    partial class UnRegVW
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
            this.warningMsg1 = new WarningMsg();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cityLabel = new System.Windows.Forms.Label();
            this.voterCityLabel = new System.Windows.Forms.Label();
            this.addressLabel = new System.Windows.Forms.Label();
            this.voterAddressLabel = new System.Windows.Forms.Label();
            this.nameLabel = new System.Windows.Forms.Label();
            this.voterNameLabel = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.cancelBtn1 = new CancelBtn();
            this.UnRegisterBtn = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // warningMsg1
            // 
            this.warningMsg1.Location = new System.Drawing.Point(12, 108);
            this.warningMsg1.Name = "warningMsg1";
            this.warningMsg1.Size = new System.Drawing.Size(300, 52);
            this.warningMsg1.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cityLabel);
            this.groupBox1.Controls.Add(this.voterCityLabel);
            this.groupBox1.Controls.Add(this.addressLabel);
            this.groupBox1.Controls.Add(this.voterAddressLabel);
            this.groupBox1.Controls.Add(this.nameLabel);
            this.groupBox1.Controls.Add(this.voterNameLabel);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(330, 90);
            this.groupBox1.TabIndex = 24;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Person information";
            // 
            // cityLabel
            // 
            this.cityLabel.AutoSize = true;
            this.cityLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cityLabel.Location = new System.Drawing.Point(6, 62);
            this.cityLabel.Name = "cityLabel";
            this.cityLabel.Size = new System.Drawing.Size(39, 20);
            this.cityLabel.TabIndex = 5;
            this.cityLabel.Text = "City:";
            // 
            // voterCityLabel
            // 
            this.voterCityLabel.AutoSize = true;
            this.voterCityLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.voterCityLabel.Location = new System.Drawing.Point(84, 62);
            this.voterCityLabel.Name = "voterCityLabel";
            this.voterCityLabel.Size = new System.Drawing.Size(0, 20);
            this.voterCityLabel.TabIndex = 6;
            // 
            // addressLabel
            // 
            this.addressLabel.AutoSize = true;
            this.addressLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.addressLabel.Location = new System.Drawing.Point(6, 42);
            this.addressLabel.Name = "addressLabel";
            this.addressLabel.Size = new System.Drawing.Size(72, 20);
            this.addressLabel.TabIndex = 3;
            this.addressLabel.Text = "Address:";
            // 
            // voterAddressLabel
            // 
            this.voterAddressLabel.AutoSize = true;
            this.voterAddressLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.voterAddressLabel.Location = new System.Drawing.Point(84, 42);
            this.voterAddressLabel.Name = "voterAddressLabel";
            this.voterAddressLabel.Size = new System.Drawing.Size(0, 20);
            this.voterAddressLabel.TabIndex = 4;
            // 
            // nameLabel
            // 
            this.nameLabel.AutoSize = true;
            this.nameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nameLabel.Location = new System.Drawing.Point(6, 22);
            this.nameLabel.Name = "nameLabel";
            this.nameLabel.Size = new System.Drawing.Size(55, 20);
            this.nameLabel.TabIndex = 1;
            this.nameLabel.Text = "Name:";
            // 
            // voterNameLabel
            // 
            this.voterNameLabel.AutoSize = true;
            this.voterNameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.voterNameLabel.Location = new System.Drawing.Point(84, 22);
            this.voterNameLabel.Name = "voterNameLabel";
            this.voterNameLabel.Size = new System.Drawing.Size(0, 20);
            this.voterNameLabel.TabIndex = 2;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(12, 163);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(170, 13);
            this.label9.TabIndex = 27;
            this.label9.Text = "Type in the administrator password";
            // 
            // textBox1
            // 
            this.textBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox1.Location = new System.Drawing.Point(15, 179);
            this.textBox1.Name = "textBox1";
            this.textBox1.PasswordChar = '*';
            this.textBox1.Size = new System.Drawing.Size(330, 45);
            this.textBox1.TabIndex = 26;
            // 
            // cancelBtn1
            // 
            this.cancelBtn1.Location = new System.Drawing.Point(183, 230);
            this.cancelBtn1.Name = "cancelBtn1";
            this.cancelBtn1.Size = new System.Drawing.Size(162, 62);
            this.cancelBtn1.TabIndex = 27;
            // 
            // UnRegisterBtn
            // 
            this.UnRegisterBtn.BackColor = System.Drawing.SystemColors.HotTrack;
            this.UnRegisterBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.UnRegisterBtn.Location = new System.Drawing.Point(15, 230);
            this.UnRegisterBtn.Name = "UnRegisterBtn";
            this.UnRegisterBtn.Size = new System.Drawing.Size(162, 62);
            this.UnRegisterBtn.TabIndex = 28;
            this.UnRegisterBtn.Text = "Unregister";
            this.UnRegisterBtn.UseVisualStyleBackColor = false;
            // 
            // UnRegVW
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(361, 307);

            

            this.Controls.Add(this.UnRegisterBtn);
            this.Controls.Add(this.cancelBtn1);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.warningMsg1);
            this.Name = "UnRegVW";
            this.Text = "UnRegVW";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private WarningMsg warningMsg1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label cityLabel;
        private System.Windows.Forms.Label voterCityLabel;
        private System.Windows.Forms.Label addressLabel;
        private System.Windows.Forms.Label voterAddressLabel;
        private System.Windows.Forms.Label nameLabel;
        private System.Windows.Forms.Label voterNameLabel;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox textBox1;
        private CancelBtn cancelBtn1;
        private System.Windows.Forms.Button UnRegisterBtn;

        public Button UnregisterButton
        {
            get
            {
                return UnRegisterBtn;
            }
        }

        public TextBox AdmPass
        {
            get
            {
                return textBox1;
            }
        }

    }
}