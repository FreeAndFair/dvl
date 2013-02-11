namespace PollingTable.PollingTable.View
{
    using System.Windows.Forms;

    using global::PollingTable.PollingTable.View.Root_elements;

    partial class WarningVW
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cityLabel = new System.Windows.Forms.Label();
            this.voterCityLabel = new System.Windows.Forms.Label();
            this.addressLabel = new System.Windows.Forms.Label();
            this.voterAddressLabel = new System.Windows.Forms.Label();
            this.nameLabel = new System.Windows.Forms.Label();
            this.voterNameLabel = new System.Windows.Forms.Label();
            this.cancelBtn1 = new CancelBtn();
            this.UnlockBtn = new System.Windows.Forms.Button();
            this.warningMsg1 = new WarningMsg();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
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
            // cancelBtn1
            // 
            this.cancelBtn1.Location = new System.Drawing.Point(180, 167);
            this.cancelBtn1.Name = "cancelBtn1";
            this.cancelBtn1.Size = new System.Drawing.Size(162, 62);
            this.cancelBtn1.TabIndex = 35;
            // 
            // UnlockBtn
            // 
            this.UnlockBtn.Image = Properties.Resources.lock_open;
            this.UnlockBtn.Location = new System.Drawing.Point(312, 108);
            this.UnlockBtn.Name = "UnlockBtn";
            this.UnlockBtn.Size = new System.Drawing.Size(30, 30);
            this.UnlockBtn.TabIndex = 36;
            this.UnlockBtn.UseVisualStyleBackColor = true;
            // 
            // warningMsg1
            // 
            this.warningMsg1.Location = new System.Drawing.Point(13, 109);
            this.warningMsg1.Name = "warningMsg1";
            this.warningMsg1.Size = new System.Drawing.Size(300, 52);
            this.warningMsg1.TabIndex = 37;
            // 
            // WarningVW
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(356, 242);
            this.Controls.Add(this.warningMsg1);
            this.Controls.Add(this.UnlockBtn);
            this.Controls.Add(this.cancelBtn1);
            this.Controls.Add(this.groupBox1);
            this.Name = "WarningVW";
            this.Text = "WarningVW";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label cityLabel;
        private System.Windows.Forms.Label voterCityLabel;
        private System.Windows.Forms.Label addressLabel;
        private System.Windows.Forms.Label voterAddressLabel;
        private System.Windows.Forms.Label nameLabel;
        private System.Windows.Forms.Label voterNameLabel;
        private CancelBtn cancelBtn1;
        private System.Windows.Forms.Button UnlockBtn;
        private WarningMsg warningMsg1;

        public Button UnlockButton
        {
            get
            {
                return UnlockBtn;
            }
        }

    }
}