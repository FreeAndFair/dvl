using System.Windows.Forms;

namespace PollingTable.PollingTable.Log
{
    using System;
    using System.ComponentModel;

    using DBComm.DBComm.DO;

    public partial class LogWindow : Form
    {
        public LogWindow()
        {
            InitializeComponent();

            this.AddResetButtonClicked(this.ResetFields);
        }

        public ActivityEnum? Activity
        {
            get
            {
                ActivityEnum e;

                if (ActivityEnum.TryParse(this.activityBox.Text, out e))
                {
                    return e;
                }

                return null;
            }
        }

        public string Cpr
        {
            get
            {
                return this.cprBox.Text;
            }
        }

        public string Table
        {
            get
            {
                return this.tableBox.Text;
            }
        }

        public DateTime From
        {
            get
            {
                return this.timeFrom.Value;
            }
        }

        public DateTime To
        {
            get
            {
                return this.timeTo.Value;
            }
        }

        public string VotersText
        {
            set
            {
                this.voterLabel.Text = value;
            }
        }

        public void SetDataSource(BindingList<LogDO> voters)
        {
            this.dataGridView1.DataSource = voters;
        }

        public delegate void ButtonClicked();

        public void AddRefreshButtonClicked(ButtonClicked h)
        {
            this.refreshButton.Click += (o, eA) => h();
        }

        public void AddResetButtonClicked(ButtonClicked h)
        {
            this.resetButton.Click += (o, eA) => h();
        }

        public void ResetFields()
        {
            this.cprBox.Text = string.Empty;
            this.tableBox.Text = string.Empty;
            this.activityBox.Text = string.Empty;
            this.timeFrom.Value = DateTime.Now;
            this.timeTo.Value = DateTime.Now;
        }

        private void LogWindow_Load(object sender, EventArgs e)
        {

        }
    }
}
