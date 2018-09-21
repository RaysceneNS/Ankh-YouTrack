using System;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;
using Ankh.YouTrack.Services;
using Ankh.YouTrack.Services.Models;

namespace Ankh.YouTrack.IssueTracker.Forms
{
    internal partial class SelectProject : Form
    {
        private readonly YouTrackConnect _connect;

        public SelectProject(YouTrackConnect connect)
        {
            _connect = connect;
            InitializeComponent();
        }

        public string ProjectId { get; set; }

        private async void SelectProject_Load(object sender, EventArgs e)
        {
            await LoadProjectsAsync();
        }

        private async Task LoadProjectsAsync()
        {
            this.Cursor = Cursors.WaitCursor;
            try
            {
                var projects = await _connect.GetProjectsAsync();
                cboProjects.DataSource = projects;
                foreach (var project in projects)
                {
                    if (project.ShortName == ProjectId)
                    {
                        cboProjects.SelectedItem = project;
                        break;
                    }
                }
                buttonOK.Enabled = true;
            }
            catch (WebException)
            {
                buttonOK.Enabled = false;
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        private void ButtonOK_Click(object sender, EventArgs e)
        {
            var selectedItem = cboProjects.SelectedItem;
            this.ProjectId = ((Project) selectedItem)?.ShortName;
        }
    }
}
