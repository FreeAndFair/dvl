using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ClientApplication
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        public Label ThisTable
        {
            get { return this.thisTableLabel; }
        }

        public Label ID
        {
            get { return this.idLabel; }
        }

        public Label FirstName
        {
            get { return this.firstNameLabel; }
        }

        public Label LastName
        {
            get { return this.lastNameLabel; }
        }

        public Label Cpr
        {
            get { return this.cprLabel; }
        }

        public Label Voted
        {
            get { return this.votedLabel; }
        }

        public Label Table
        {
            get { return this.tableLabel; }
        }

        public Label Time
        {
            get { return this.timeLabel; }
        }

        public TextBox IdTextBox
        {
            get { return this.idTextBox; }
        }

        public TextBox CprTextBox
        {
            get { return this.cprTextBox;  }
        }

        public Button IdSearchButton
        {
            get { return this.searchIdButton; }
        }

        public Button CprSearchButton
        {
            get { return this.searchCprButton; }
        }

        public Button LogButton
        {
            get { return this.logButton;
            }
        }

        public Button RegisterButton
        {
            get { return this.registerButton; }
        }

        public Button UnregisterButton
        {
            get { return this.unregisterButton; }
        }

        public Button ClearButton
        {
            get { return this.clearButton;  }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void label10_Click(object sender, EventArgs e)
        {

        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }

        private void label5_Click_1(object sender, EventArgs e)
        {

        }
    }
}
