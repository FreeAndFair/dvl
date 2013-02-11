// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VoterCardGenerator.cs" company="DVL">
//   Jan Meier
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Central.Central.Views
{
    using System;
    using System.Windows.Forms;

    using global::Central.Central.Models;

    /// <summary>
    /// Window for representing and manipulating the Voter Box Managaer.
    /// Depicts the current state of the Voter Box Manager.
    /// </summary>
    public partial class VoterBoxManagerWindow : Form, ISubView
    {
        private VoterBoxManager model;

        /// <summary>
        /// Initializes a new instance of the <see cref="VoterBoxManagerWindow"/> class.
        /// </summary>
        /// <param name="model">The associated model. </param>
        public VoterBoxManagerWindow(VoterBoxManager model)
        {
            InitializeComponent();
            this.model = model;
        }

        /// <summary>
        /// May I have the 'address' input?
        /// </summary>
        public string Address
        {
            get
            {
                return adressTB.Text;
            }
        }

        /// <summary>
        /// May I have the 'port' input?
        /// </summary>
        public string Port
        {
            get
            {
                return portBox.Text;
            }
        }

        /// <summary>
        /// May I have the 'user' input?
        /// </summary>
        public string User
        {
            get
            {
                return userBox.Text;
            }
        }

        /// <summary>
        /// May I have the 'password' input?
        /// </summary>
        public string Password
        {
            get
            {
                return pwBox.Text;
            }
        }

        /// <summary> Notify me when the window is closing. </summary>
        /// <param name="handler">The handler to be called upon closing.</param>
        public void AddClosingHandler(Action<ISubModel> handler)
        {
            this.Disposed += (o, eA) => handler(model);
        }

        public ISubModel GetModel()
        {
            return model; // In an ideal world this would be a property, but interfaces can't contain properties.
        }

        public delegate void ButtonHandler();

        /// <summary>
        /// Notify me about validation requests.
        /// </summary>
        /// <param name="h">Handler to be called upon validation requests.</param>
        public void AddValidateHandler(ButtonHandler h)
        {
            this.validateButton.Click += (o, eA) => h();
        }

        /// <summary>
        /// Notify me about upload requests.
        /// </summary>
        /// <param name="h">Handler to be called upon upload requests.</param>
        public void AddUploadHandler(ButtonHandler h)
        {
            this.uploadButton.Click += (o, eA) => h();
        }

        /// <summary>
        /// Notify me about connect requests.
        /// </summary>
        /// <param name="h">Handler to be called upon connect requests.</param>
        public void AddConnectHandler(ButtonHandler h)
        {
            this.connectButton.Click += (o, eA) => h();
        }

        /// <summary>
        /// Update the progress bar.
        /// </summary>
        public void UpdateProgress()
        {
            this.progressBar1.PerformStep();
        }

        /// <summary>
        /// Update progress text.
        /// </summary>
        /// <param name="text"></param>
        public void UpdateProgressText(string text)
        {
            this.progressTF.Text += text + Environment.NewLine;
            this.progressTF.SelectionStart = this.progressTF.Text.Length;
            this.progressTF.ScrollToCaret();
        }
    }
}
