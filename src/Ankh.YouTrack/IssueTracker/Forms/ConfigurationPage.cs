using System;
using System.Windows.Forms;
using Ankh.ExtensionPoints.IssueTracker;
using Ankh.YouTrack.Services;

namespace Ankh.YouTrack.IssueTracker.Forms
{
    /// <summary>
    /// UI for Issue repository configuration
    /// </summary>
    public partial class ConfigurationPage : UserControl
    {
        public event EventHandler<ConfigPageEventArgs> OnPageChanged;

        private readonly YouTrackConnect _connect;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationPage"/> class.
        /// </summary>
        public ConfigurationPage()
        {
            InitializeComponent();
            _connect = new YouTrackConnect();
        }

        internal IssueRepositorySettings UiToSettings()
        {
            var uri = new Uri(textBoxRepositoryUri.Text);
            string repositoryId = textBoxProjectID.Text;
            return new AnkhRepository(uri, repositoryId);
        }

        internal void SettingsToUi(IssueRepositorySettings settings)
        {
            textBoxRepositoryUri.Text = settings.RepositoryUri == null ? string.Empty : settings.RepositoryUri.ToString();
            textBoxProjectID.Text = settings.RepositoryId;

            this.ValidateChildren();
        }

        /// <summary>
        /// Handles the Click event of the button control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void ButtonChooseProject_Click(object sender, EventArgs e)
        {
            var dlg = new SelectProject(_connect)
            {
                ProjectId = textBoxProjectID.Text
            };

            if (dlg.ShowDialog(this) == DialogResult.OK)
            {
                textBoxProjectID.Text = dlg.ProjectId;
            }
        }

        /// <summary>
        /// Perform validation against the supplied uri
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBoxRepositoryUri_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            errorProvider.SetError(textBoxRepositoryUri, "");
            if (string.IsNullOrWhiteSpace(textBoxRepositoryUri.Text))
            {
                errorProvider.SetError(textBoxRepositoryUri, "Uri is required.");
                e.Cancel = true;
            }
            else
            {
                if (!Uri.TryCreate(textBoxRepositoryUri.Text.Trim(), UriKind.Absolute, out var uri))
                {
                    errorProvider.SetError(textBoxRepositoryUri, "Uri is invalid.");
                    e.Cancel = true;
                }
                else
                {
                    _connect.Uri = uri;
                }
            }

            buttonTest.Enabled = !e.Cancel;
            buttonChooseProject.Enabled = false;
        }

        private async void ButtonTest_Click(object sender, EventArgs e)
        {
            var cred = _connect.GetUserCredential();
            if (cred != null)
            {
                var ok = await _connect.LoginAsync(cred.UserName, cred.Password);
                _connect.ConfirmUserCredential(ok);
                buttonChooseProject.Enabled = ok;
                OnPageChanged?.Invoke(this, new ConfigPageEventArgs { IsComplete = ok });
            }
        }
    }
}
