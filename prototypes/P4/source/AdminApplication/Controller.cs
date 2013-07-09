namespace AdminApplication
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Windows.Forms;
    using System.Xml.Schema;
    using SmallTuba.Entities;
    using SmallTuba.IO;

    /// <author>Kåre Sylow Pedersen (ksyl@itu.dk)</author>
    /// <version>2011-12-12</version>
    /// <summary>
    /// The controller class for the gui, responsible for the actions made by the user.
    /// </summary>
    public class Controller
    {
        private List<PollingVenue> pollingVenues;
        private MainWindow form;
        private ExportWindow export;
        public event PollingVenuesChanged Changed;

        public delegate void PollingVenuesChanged();

        public Controller()
        {
            form = new MainWindow();
            export = new ExportWindow();
            this.InitializeEventSubscribers();
        }

        /// <summary>
        /// Adds event subscribers to the gui elements
        /// </summary>
        private void InitializeEventSubscribers()
        {
            form.ImportData.Click += (o, e) => FileOpenDialogImport();
            form.ExportData.Click += (o, e) => OpenExportWindow();
            export.ExportData.Click += (o, e) => ExportData();
            export.Cancel.Click += (o, e) => export.Close();
            Changed += this.UpdateTable;
        }

        /// <summary>
        /// Loads the polling venues and invoke the change event on the table overview
        /// </summary>
        /// <param name="path"></param>
        private void SetPollingVenues(string path)
        {
            FileLoader fl = new FileLoader();
            pollingVenues = fl.GetPollingVenues(path, this.ErrorLoadFileDialog);
            if (pollingVenues != null)
            {
                this.UpdateTable();
                Changed.Invoke();
            }     
        }

        /// <summary>
        /// Updates the visible table overview with the loaded polling venues
        /// </summary>
        private void UpdateTable()
        {
            var addresses = from n in pollingVenues select n.PollingVenueAddress;
            BindingSource bs = new BindingSource();
            bs.DataSource = addresses;
            this.form.TableView.DataSource = bs;
        }

        /// <summary>
        /// Opens the import file dialog box
        /// </summary>
        private void FileOpenDialogImport()
        {
            this.form.OpenFileDialog.Filter = "Xml Files (*.xml)|*.xml";
            if (this.form.OpenFileDialog.ShowDialog() == DialogResult.OK)
            {
                string path = form.OpenFileDialog.FileName;
                if(Path.GetExtension(path).Equals(".xml")){
                    this.SetPollingVenues(path);
                }else{
                    MessageBox.Show("The selected file type is not supported, please select a .xml file", "File error", MessageBoxButtons.OK);
                }
                
            }
        }

        /// <summary>
        /// Return the polling venue the user has selected in the table overview
        /// Returns null if no polling venue is selected.
        /// </summary>
        /// <returns>Polling Venue</returns>
        private PollingVenue GetSelectedPollingVenue()
        {
            if (this.form.TableView.SelectedRows.Count > 0)
            {
                return pollingVenues[form.TableView.SelectedRows[0].Index];
            }

            return null;

        }

        /// <summary>
        /// Checks that a polling venue is selected
        /// </summary>
        /// <returns>bool</returns>
        private bool PollingVenueSelected()
        {
            if (this.GetSelectedPollingVenue() == null)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Shows the message box if the import file is not correct
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">event args</param>
        private void ErrorLoadFileDialog(Object sender, ValidationEventArgs e)
        {
            MessageBox.Show(e.Message, "XML Parsing Error", MessageBoxButtons.OK);
        }

        /// <summary>
        /// The text from the gui text field ElectionName
        /// </summary>
        /// <returns>Name of the election</returns>
        private string ElectionName()
        {
            return form.ElectionName.Text;
        }

        /// <summary>
        /// The date from the date picker in the gui
        /// </summary>
        /// <returns>Date of the election</returns>
        private string ElectionDate()
        {
            return form.ElectionDate.Text;
        }

        /// <summary>
        /// Returns the selected folder path for export files
        /// </summary>
        /// <returns>path</returns>
        private string SelectedExportFolderPath()
        {
            return export.FolderBrowserDialog.SelectedPath;
        }

        /// <summary>
        /// Starts the gui
        /// </summary>
        public void Run()
        {
           Application.Run(form);
        }

        /// <summary>
        /// Opens the export form
        /// </summary>
        private void OpenExportWindow()
        {
            if (this.PollingVenueSelected())
            {
                export.ShowDialog();
            }else
            {
                MessageBox.Show("No polling venue is selected", "Notification", MessageBoxButtons.OK);
            }
            
        }

        /// <summary>
        /// Exports the data for each polling venue dependent on which checkboxes in
        /// the export form that are checked.
        /// </summary>
        private void ExportData()
        {
            //If no boxes are checked
            if(!this.ExportElementsSelected())
            {
                MessageBox.Show("No export elements are selected", "Notification", MessageBoxButtons.OK);
                return;
            }

            if (export.FolderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                FileSaver fs = new FileSaver(this.SelectedExportFolderPath(), this.GetSelectedPollingVenue().PollingVenueAddress.Name);
                if (export.PollingCards.Checked)
                {
                    fs.SavePollingCards(this.GetSelectedPollingVenue(), this.ElectionName(), this.ElectionDate());
                }
                if (export.VoterLists.Checked)
                {
                    fs.SaveVoterList(this.GetSelectedPollingVenue().Persons, this.ElectionName(), this.ElectionDate());
                }
                if (export.Voters.Checked)
                {
                    fs.SaveVoters(this.GetSelectedPollingVenue());
                }
            }

            export.Close(); //close the form
        }

        /// <summary>
        /// Checks that at least one check box is checked in the export form
        /// </summary>
        /// <returns>bool</returns>
        private bool ExportElementsSelected()
        {
            return export.PollingCards.Checked || export.VoterLists.Checked || export.Voters.Checked;
        }
    }
        
}
