using System;
using System.Windows.Forms;

namespace GeneratorApplication
{
	using SmallTuba.DataGenerator;

	public partial class Form1 : Form
	{
		private int numberOfMunicipalities;
		private int numberOfPollingVenues;
		private int numberOfVoters; 

		private string fileDestination;

		public Form1()
		{
			numberOfMunicipalities = 5;
			numberOfPollingVenues = 5;
			numberOfVoters = 100;

			fileDestination = "C:/ThoughShallNotUseXml.xml";

			InitializeComponent();

			label8.Hide();
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			saveFileDialog1.Filter = "Xml File (*.xml)|*.xml";
		}

		private void groupBox1_Enter(object sender, EventArgs e)
		{

		}

		private void label1_Click(object sender, EventArgs e)
		{

		}

		private void label2_Click(object sender, EventArgs e)
		{

		}

		private void label3_Click(object sender, EventArgs e)
		{

		}

		private void textBox3_TextChanged(object sender, EventArgs e)
		{
			var text = textBox3.Text;
			int number = 0;
			if (Int32.TryParse(text, out number) || text == "") {
				this.numberOfVoters = number;
				this.CalculateApprox();

			} else {
				MessageBox.Show("The number of voters must be .. A number!");
				textBox3.Text = 42+"";
			}
		}

		private void button1_Click(object sender, EventArgs e)
		{
			if (
				this.numberOfMunicipalities > 0 &&
				this.numberOfPollingVenues > 0 &&
				this.numberOfVoters > 0 &&
				this.fileDestination != "" &&
				this.fileDestination.EndsWith(".xml")
			)
			{
				label8.Show();

				var dg = new DataGenerator();
				dg.NumberOfMunicipalities = this.numberOfMunicipalities;
				dg.NumberOfPollingVenues = this.numberOfPollingVenues;
				dg.NumberOfVoters = this.numberOfVoters;
				dg.FileDestination = this.fileDestination;

				dg.Generate();

				label8.Hide();
			} else {
				MessageBox.Show("Numbers must be integers and positive. File destination must be valid and end with .xml!");
			}
		}

		private void label5_Click(object sender, EventArgs e)
		{

		}

		private void button2_Click(object sender, EventArgs e)
		{
			DialogResult result = saveFileDialog1.ShowDialog();

			if (result == DialogResult.OK) {
				textBox4.Text = saveFileDialog1.FileName;
			}
		}

		private void textBox4_TextChanged(object sender, EventArgs e)
		{
			this.fileDestination = textBox4.Text;
		}

		private void textBox1_TextChanged(object sender, EventArgs e) {
			var text = textBox1.Text;
			int number = 0;
			if (Int32.TryParse(text, out number) || text == "") {
				this.numberOfMunicipalities = number;
				this.CalculateApprox();

			} else {
				MessageBox.Show("The number of municipalities must be .. A number!");
				textBox1.Text = 5+"";
			}

		}

		private void label6_Click(object sender, EventArgs e)
		{

		}

		private void CalculateApprox() {
			int approx = numberOfMunicipalities * numberOfPollingVenues * numberOfVoters;

			label6.Text = string.Format("{0:0,0}", approx) + " voters";
		}

		private void textBox2_TextChanged(object sender, EventArgs e)
		{
			var text = textBox2.Text;
			int number = 0;
			if (Int32.TryParse(text, out number) || text == "") {
				this.numberOfPollingVenues = number;
				this.CalculateApprox();

			} else {
				MessageBox.Show("The number of polling venues must be .. A number!");
				textBox2.Text = 5+"";
			}
		}

		private void label7_Click(object sender, EventArgs e)
		{

		}

		private void label8_Click(object sender, EventArgs e)
		{

		}
	}
}
