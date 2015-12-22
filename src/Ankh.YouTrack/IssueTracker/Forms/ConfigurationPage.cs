using System;
using System.Windows.Forms;
using Ankh.ExtensionPoints.IssueTracker;
using Ankh.YouTrack.Services;
using Ankh.YouTrack.Services.Models;

namespace Ankh.YouTrack.IssueTracker.Forms
{
    /// <summary>
    /// UI for Issue repository configuration
    /// </summary>
    public partial class ConfigurationPage : UserControl
    {
        private string _repositoryId;
        public event EventHandler<ConfigPageEventArgs> OnPageEvent;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationPage"/> class.
        /// </summary>
        public ConfigurationPage()
        {
            InitializeComponent();
        }

        internal IssueRepositorySettings Settings
        {
            get
            {
                return SaveSettings();
            }
            set
            {
                SelectSettings(value);
            }
        }

        /// <summary>
        /// Saves UI values to existing settings
        /// </summary>
        private IssueRepositorySettings SaveSettings()
        {
            var uri = new Uri(textRepositoryUri.Text);

            string repositoryId = null;
            var selectedItem = cboProjects.SelectedItem;
            if(selectedItem != null)
                repositoryId = ((Project)selectedItem).ShortName;
            return new AnkhRepository(uri, repositoryId, null);
        }

        /// <summary>
        /// Populates UI with existing settings
        /// </summary>
        private void SelectSettings(IssueRepositorySettings settings)
        {
            textRepositoryUri.Text = settings.RepositoryUri == null ? string.Empty : settings.RepositoryUri.ToString();
            _repositoryId = settings.RepositoryId;

            LoadProjects();
        }

        /// <summary>
        /// Handles the Click event of the button control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void buttonLoadProjects_Click(object sender, EventArgs e)
        {
            LoadProjects();
        }

        private void LoadProjects()
        {
            if (!this.ValidateChildren())
                return;

            ConfigPageEventArgs args = new ConfigPageEventArgs();
            this.Cursor = Cursors.WaitCursor;

            var repoUri =
                new Uri(textRepositoryUri.Text.EndsWith("/") ? textRepositoryUri.Text : textRepositoryUri.Text + "/");

            var youTrackConnect = new YouTrackConnect(repoUri);
            try
            {
                var cred = youTrackConnect.GetUserCredential();
                if (cred != null)
                {
                    if (!youTrackConnect.Login(cred.UserName, cred.Password))
                        throw new Exception("Username and/or Password is not valid.");
                    var projects = youTrackConnect.GetProjects();
                    cboProjects.DataSource = projects;
                    cboProjects.DisplayMember = "Name";
                    foreach (var project in projects)
                    {
                        if (project.ShortName == _repositoryId)
                        {
                            cboProjects.SelectedItem = project;
                            break;
                        }
                    }
                    args.IsComplete = true;
                    cboProjects.Enabled = true;
                    youTrackConnect.ConfirmUserCredential(true);
                }
            }
            catch (Exception ex)
            {
                youTrackConnect.ConfirmUserCredential(false);
                args.IsComplete = false;
                args.Exception = ex;
                cboProjects.Enabled = false;
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }

            if (OnPageEvent != null)
            {
                OnPageEvent(this, args);
            }
        }

        /// <summary>
        /// Perform validation agains the supplied uri
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textUrl_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            errorProvider.SetError(textRepositoryUri, "");
            if (string.IsNullOrWhiteSpace(textRepositoryUri.Text))
            {
                errorProvider.SetError(textRepositoryUri, "Uri is required.");
                e.Cancel = true;
            }
            else
            {
                Uri uri;
                bool validUri = Uri.TryCreate(textRepositoryUri.Text.Trim(), UriKind.Absolute, out uri);
                if (!validUri)
                {
                    errorProvider.SetError(textRepositoryUri, "Uri is invalid.");
                    e.Cancel = true;
                }
            }
        }
    }
}
