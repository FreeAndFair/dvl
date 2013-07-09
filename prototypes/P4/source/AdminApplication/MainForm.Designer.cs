namespace AdminApplication
{
    using System.Windows.Forms;

    partial class MainWindow
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

        public Button ImportData
        {
            get
            {
                return importData;
            }
        }

        public Button ExportData
        {
            get
            {
                return exportData;
            }
        }

        public DataGridView TableView
        {
            get
            {
                return tableView;
            }
        }

        public DateTimePicker ElectionDate
        {
            get
            {
                return electionDate;
            }
        }

        public TextBox ElectionName
        {
            get
            {
                return electionName;
            }
        }

        public OpenFileDialog OpenFileDialog
        {
            get
            {
                return openFileDialog;
            }
        }


        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.electionName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.electionDate = new System.Windows.Forms.DateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            this.importData = new System.Windows.Forms.Button();
            this.exportData = new System.Windows.Forms.Button();
            this.tableView = new System.Windows.Forms.DataGridView();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            ((System.ComponentModel.ISupportInitialize)(this.tableView)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // electionName
            // 
            this.electionName.Location = new System.Drawing.Point(106, 19);
            this.electionName.Name = "electionName";
            this.electionName.Size = new System.Drawing.Size(200, 20);
            this.electionName.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(18, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(76, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Election Name";
            this.label1.Click += new System.EventHandler(this.label1_Click_1);
            // 
            // electionDate
            // 
            this.electionDate.Location = new System.Drawing.Point(106, 62);
            this.electionDate.Name = "electionDate";
            this.electionDate.Size = new System.Drawing.Size(200, 20);
            this.electionDate.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(18, 69);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(71, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Election Date";
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // importData
            // 
            this.importData.Location = new System.Drawing.Point(372, 24);
            this.importData.Name = "importData";
            this.importData.Size = new System.Drawing.Size(130, 23);
            this.importData.TabIndex = 6;
            this.importData.Text = "Import Data";
            this.importData.UseVisualStyleBackColor = true;
            this.importData.Click += new System.EventHandler(this.button1_Click);
            // 
            // exportData
            // 
            this.exportData.Location = new System.Drawing.Point(372, 53);
            this.exportData.Name = "exportData";
            this.exportData.Size = new System.Drawing.Size(130, 23);
            this.exportData.TabIndex = 7;
            this.exportData.Text = "Export Data";
            this.exportData.UseVisualStyleBackColor = true;
            // 
            // tableView
            // 
            this.tableView.AllowUserToAddRows = false;
            this.tableView.AllowUserToDeleteRows = false;
            this.tableView.AllowUserToResizeRows = false;
            this.tableView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.tableView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.tableView.Cursor = System.Windows.Forms.Cursors.Default;
            this.tableView.Location = new System.Drawing.Point(21, 24);
            this.tableView.MultiSelect = false;
            this.tableView.Name = "tableView";
            this.tableView.ReadOnly = true;
            this.tableView.RowHeadersVisible = false;
            this.tableView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.tableView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.tableView.Size = new System.Drawing.Size(324, 200);
            this.tableView.TabIndex = 10;
            this.tableView.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentClick);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.electionName);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.electionDate);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(322, 100);
            this.groupBox1.TabIndex = 11;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Election Details";
            this.groupBox1.Enter += new System.EventHandler(this.groupBox1_Enter);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.importData);
            this.groupBox3.Controls.Add(this.exportData);
            this.groupBox3.Controls.Add(this.tableView);
            this.groupBox3.Location = new System.Drawing.Point(12, 135);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(520, 244);
            this.groupBox3.TabIndex = 13;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Polling Venues";
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(544, 391);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox1);
            this.Name = "MainWindow";
            this.Text = "Admin Application";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.tableView)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox electionName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DateTimePicker electionDate;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button importData;
        private System.Windows.Forms.Button exportData;
        private DataGridView tableView;
        private GroupBox groupBox1;
        private GroupBox groupBox3;
        private OpenFileDialog openFileDialog;
    }
}

