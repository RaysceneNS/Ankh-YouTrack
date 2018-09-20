using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Ankh.YouTrack.Services;
using Ankh.YouTrack.Services.Models;

namespace Ankh.YouTrack.IssueTracker.Forms
{
	public partial class IssuesView : UserControl
	{
        private Uri _uri;
        private string _repositoryId;
	    private readonly List<string> _selectedIssues;

        /// <summary>
        /// Initializes a new instance of the <see cref="IssuesView"/> class.
        /// </summary>
        public IssuesView()
		{
            _selectedIssues = new List<string>();
			InitializeComponent();
		}

        public void LoadData(Uri uri, string repositoryId)
        {
            _uri = uri;
            _repositoryId = repositoryId;
        }

        /// <summary>
        /// Handles the Load event of the IssuesListView control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void IssuesListViewLoad(object sender, EventArgs e)
        {
            dgvList.AutoGenerateColumns = false;

            //disable controls if no uri setup
            if (_uri == null)
            {
                this.errorProvider.SetError(this, "YouTrack Uri must be set in Configuration page.");
                this.Enabled = false;
            }
            else
            {
                this.errorProvider.SetError(this, "");
                this.Enabled = true;
            }
	    }
        
		/// <summary>
		/// Populates this instance.
		/// </summary>
		private async Task PerformSearchAsync()
		{
			Cursor = Cursors.WaitCursor;
		    dgvList.DataSource = null;
            errorProvider.SetError(dgvList, "");
            
		    var youTrackConnect = new YouTrackConnect(_uri);
		    try
		    {
		        var issues = await youTrackConnect.GetIssuesAsync(_repositoryId, textQuery.Text);
		        
                youTrackConnect.ConfirmUserCredential(true);
		        dgvList.DataSource = issues;
		        dgvList.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);


		        //assign prior selections
		        foreach (DataGridViewRow row in dgvList.Rows)
		        {
		            var issue = (Issue)row.DataBoundItem;
                    if (_selectedIssues.Contains(issue.Id))
                    {
                        row.Cells[0].Value = true;
                    }
		        }
                
                dgvList.Enabled = true;
		    }
		    catch (Exception ex)
		    {
		        errorProvider.SetError(dgvList,
		            $"Error updating issues list check connection settings {ex.Message}.");
		        dgvList.Enabled = false;
                youTrackConnect.ConfirmUserCredential(false);
		    }
		    finally
		    {
		        Cursor = Cursors.Default;
		    }
		}

		/// <summary>
		/// Handles the Click event of the search button.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private async void BtnSearchClick(object sender, EventArgs e)
		{
			await PerformSearchAsync();
		}

        internal IEnumerable<string> SelectedIssues
        {
            get { return _selectedIssues.AsEnumerable(); }
        }

		/// <summary>
		/// Handles the CurrentCellDirtyStateChanged event of the dgvList control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void DgvListCurrentCellDirtyStateChanged(object sender, EventArgs e)
		{
			if (dgvList.IsCurrentCellDirty)
				dgvList.CommitEdit(DataGridViewDataErrorContexts.Commit);
		}

        private void DgvList_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != 0) 
                return;

            var row = dgvList.Rows[e.RowIndex];
            var issue = (Issue) row.DataBoundItem;

            if(issue == null)
                return;

            var isChecked = (bool) row.Cells[0].EditedFormattedValue;
            var issueId = issue.Id;

            if (isChecked)
            {
                if (!_selectedIssues.Contains(issueId))
                {
                    _selectedIssues.Add(issueId);
                }
            }
            else
            {
                _selectedIssues.Remove(issueId);
            }
        }

        private void TextQuery_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnSearch.PerformClick();
                e.SuppressKeyPress = true;
                e.Handled = true;
            }
        }
    }
}
