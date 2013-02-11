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
    public partial class LogForm : Form
    {
        public LogForm()
        {
            InitializeComponent();
        }

        public Label TableLable
        {
            get { return this.tableLabel; }
        }

        public ListBox LogListBox
        {
            get { return this.listBox; }
        }

        public Button ChooseButton
        {
            get { return this.chooseButton; }
        }

        public Button CloseButton
        {
            get { return this.closeButton; }
        }

        private void ChooseButton_Click(object sender, EventArgs e)
        {

        }

        private void LogForm_Load(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
