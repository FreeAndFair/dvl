namespace ClientApplication {
	using System;
	using System.Windows.Forms;
	using SmallTuba.Entities;
	using SmallTuba.Network.RPC;
	using SmallTuba.Utility;

	/// <author>Christian Olsson (chro@itu.dk)</author>
	/// <version>2011-12-12</version>
	/// <summary>
	/// The controller of the client application
	/// </summary>
	public class Controller {
		/// <summary>
		/// The welcome window
		/// </summary>
		private WelcomeForm welcomeForm;
		
		/// <summary>
		/// The main window
		/// </summary>
		private MainForm mainForm;
		
		/// <summary>
		/// The log window
		/// </summary>
		private LogForm logForm;
		
		/// <summary>
		/// The network client
		/// </summary>
		private VoterClient networkClient;
		
		/// <summary>
		/// The voter currently displayed in the main window
		/// </summary>
		private Person currentVoter;
		
		/// <summary>
		/// The model of this application
		/// </summary>
		private Model model;

		/// <summary>
		/// Creates a new controller
		/// </summary>
		public Controller() {
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			this.welcomeForm = new WelcomeForm();
			this.mainForm = new MainForm();
			this.networkClient = new VoterClient("Client: " + System.Net.Dns.GetHostName());
			this.model = new Model();
			this.currentVoter = null;
		}

		/// <summary>
		/// Starts the application
		/// </summary>
		public void Run() {
			this.SetListeners();
			this.SetDropDown();
			Application.Run(this.welcomeForm);
		}

		/// <summary>
		/// Sets the listeners for the main window and the welcome window
		/// </summary>
		private void SetListeners() {
			this.welcomeForm.RefreshButton.Click += (object sender, EventArgs e) => this.SetDropDown();
			this.welcomeForm.OKButton.Click += (object sender, EventArgs e) => this.GoToMainForm();
			this.mainForm.IdTextBox.KeyDown += this.IdTextBoxEnter;
			this.mainForm.CprTextBox.KeyDown += this.CprTextBoxEnter;
			this.mainForm.IdSearchButton.Click += (object sender, EventArgs e) => this.SearchId();
			this.mainForm.CprSearchButton.Click += (object sender, EventArgs e) => this.SearchCpr();
			this.mainForm.LogButton.Click += (object sender, EventArgs e) => this.CreateLog();
			this.mainForm.RegisterButton.Click += (object sender, EventArgs e) => this.Register();
			this.mainForm.UnregisterButton.Click += (object sender, EventArgs e) => this.Unregister();
			this.mainForm.ClearButton.Click += (object sender, EventArgs e) => this.ClearVoter();
			this.mainForm.FormClosed += (object sender, FormClosedEventArgs e) => Application.Exit();
		}

		/// <summary>
		/// Sets the dropdown menu with a response from the network client
		/// </summary>
		private void SetDropDown() {
			this.welcomeForm.dropdown.Items.Clear();
			string[] arr = this.networkClient.ValidTables();
			if (arr != null) {
				this.welcomeForm.dropdown.Items.AddRange(arr);
				this.welcomeForm.dropdown.SelectedIndex = 0;
				this.welcomeForm.OKButton.Enabled = true;
			}
			else {
				this.welcomeForm.dropdown.Items.Add("No connection");
				this.welcomeForm.OKButton.Enabled = false;
			}
		}

		/// <summary>
		/// Hides the welcome window and displays the main window
		/// </summary>
		private void GoToMainForm() {
			this.model.Name = this.welcomeForm.dropdown.SelectedItem.ToString();
			this.welcomeForm.Hide();
			this.networkClient.Name = this.model.Name;
			this.mainForm.ThisTable.Text = this.model.Name;
			this.ClearVoter();
			this.mainForm.Show();
		}

		/// <summary>
		/// Searches for a voter with the id from the textbox and sets the current voter in the main window
		/// </summary>
		private void SearchId() {
			try {
				int id = int.Parse(this.mainForm.IdTextBox.Text);
				Person person = this.networkClient.GetPersonFromId(id);
				this.SetVoter(person);
			}
			catch (Exception exception) {
				MessageBox.Show("Invalid input");
				Debug.WriteLine(exception.Message);
			}
		}

		/// <summary>
		/// Searches for a voter with the cpr from the textbox and sets the current voter in the main window
		/// </summary>
		private void SearchCpr() {
			try {
				int cpr = int.Parse(this.mainForm.CprTextBox.Text);
				Person person = this.networkClient.GetPersonFromCpr(cpr);
				this.SetVoter(person);
			}
			catch (Exception exception) {
				MessageBox.Show("Invalid input");
				Debug.WriteLine(exception.Message);
			}
		}

		/// <summary>
		/// Creates a new log window with the posts from this client
		/// </summary>
		private void CreateLog() {
			this.logForm = new LogForm();
			this.logForm.LogListBox.Items.AddRange(this.model.Log.ToArray());
			this.logForm.TableLable.Text = this.model.Name;
			this.logForm.ChooseButton.Click += (object sender, EventArgs e) => this.ChooseLine();
			this.logForm.CloseButton.Click += (object sender, EventArgs e) => this.CloseLog();
			this.logForm.LogListBox.MouseDoubleClick += (object sender, MouseEventArgs e) => this.ChooseLine();
			this.logForm.Show();
		}

		/// <summary>
		/// Tries to register the current voter
		/// </summary>
		private void Register() {
			if (this.networkClient.RegisterVoter(this.currentVoter)) {
				this.model.Log.Add(new ClientLog(this.currentVoter, "registered"));
				this.ClearVoter();
				MessageBox.Show("The voter was registered succesfully");
			}
			else {
				MessageBox.Show("The voter could not be registered!!!");
			}
		}

		/// <summary>
		/// Tries to unregister the current voter
		/// </summary>
		private void Unregister() {
			if (this.networkClient.UnregisterVoter(this.currentVoter)) {
				this.model.Log.Add(new ClientLog(this.currentVoter, "unregistered"));
				this.ClearVoter();
				MessageBox.Show("The voter was unregistered succesfully");
			}
			else {
				MessageBox.Show("The voter could not be unregistered!!!");
			}
		}

		/// <summary>
		/// Chooses the selected line from the log and sets this voter as the current
		/// </summary>
		private void ChooseLine() {
			if (this.logForm.LogListBox.SelectedItem != null) {
				ClientLog logState = (ClientLog) this.logForm.LogListBox.SelectedItem;
				Person person = this.networkClient.GetPersonFromId(logState.Voter.VoterId);
				this.SetVoter(person);
				this.CloseLog();
			}
		}

		/// <summary>
		/// closes the log window
		/// </summary>
		private void CloseLog() {
			this.logForm.Hide();
			this.logForm.Dispose();
		}

		/// <summary>
		/// Sets the current voter
		/// </summary>
		/// <param name="voter">The voter to be set</param>
		private void SetVoter(Person voter) {
			if (voter == null) {
				MessageBox.Show("No network connection");
			}
			else if (!voter.Exists) {
				MessageBox.Show("No voter found matching this criteria");
			}
			else {
				this.currentVoter = voter;
				this.mainForm.RegisterButton.Enabled = true;
				this.mainForm.UnregisterButton.Enabled = true;
				this.mainForm.ClearButton.Enabled = true;
				this.mainForm.ID.Text = voter.VoterId.ToString();
				this.mainForm.FirstName.Text = voter.FirstName;
				this.mainForm.LastName.Text = voter.LastName;
				this.mainForm.Cpr.Text = voter.Cpr.ToString();
				this.mainForm.Voted.Text = voter.Voted.ToString();
				if (voter.Voted) {
					this.mainForm.Table.Text = voter.VotedPollingTable;
					DateTime time = TimeConverter.ConvertFromUnixTimestamp(voter.VotedTime);
					this.mainForm.Time.Text = time.ToLocalTime().Hour.ToString() + ":" + time.ToLocalTime().Minute.ToString();
				}
				else {
					this.mainForm.Table.Text = string.Empty;
					this.mainForm.Time.Text = string.Empty;
				}
			}
		}

		/// <summary>
		/// Clear the current voter
		/// </summary>
		private void ClearVoter() {
			this.currentVoter = null;
			this.mainForm.IdTextBox.Text = string.Empty;
			this.mainForm.CprTextBox.Text = string.Empty;
			this.mainForm.RegisterButton.Enabled = false;
			this.mainForm.UnregisterButton.Enabled = false;
			this.mainForm.ClearButton.Enabled = false;
			this.mainForm.ID.Text = string.Empty;
			this.mainForm.FirstName.Text = string.Empty;
			this.mainForm.LastName.Text = string.Empty;
			this.mainForm.Cpr.Text = string.Empty;
			this.mainForm.Voted.Text = string.Empty;
			this.mainForm.Table.Text = string.Empty;
			this.mainForm.Time.Text = string.Empty;
		}

		/// <summary>
		/// Is called when a character is enteres in the id textbox
		/// </summary>
		/// <param name="sender">The sender</param>
		/// <param name="e">the event</param>
		private void IdTextBoxEnter(object sender, KeyEventArgs e) {
			if (e.KeyCode == Keys.Enter) {
				this.SearchId();
			}
		}

		/// <summary>
		/// Is called when a character is enteres in the cpr textbox
		/// </summary>
		/// <param name="sender">The sender</param>
		/// <param name="e">the event</param>
		private void CprTextBoxEnter(object sender, KeyEventArgs e) {
			if (e.KeyCode == Keys.Enter) {
				this.SearchCpr();
			}
		}
	}
}