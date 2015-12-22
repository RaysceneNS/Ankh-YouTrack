using System;
using System.Collections.Generic;
using System.Linq;
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
	        this.Enabled = _uri != null;
	    }
        
		/// <summary>
		/// Populates this instance.
		/// </summary>
		private void PerformSearch()
		{
			var currentCursor = Cursor;
			Cursor = Cursors.WaitCursor;

            errorProvider.SetError(dgvList,"");

			const int MAX_RECORDS = 100;

		    var youTrackConnect = new YouTrackConnect(_uri);
		    try
		    {
		        string projectId = _repositoryId;

		        string search = textQuery.Text;
		        var issues = youTrackConnect.GetIssues(projectId, search, MAX_RECORDS).ToList();
		        
                youTrackConnect.ConfirmUserCredential(true);
		        dgvList.DataSource = issues;
		        dgvList.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
		        dgvList.Enabled = true;
		    }
		    catch (Exception ex)
		    {
		        errorProvider.SetError(dgvList,
		            String.Format("Error updating issues list check connection settings {0}.", ex.Message));
		        dgvList.Enabled = false;
                youTrackConnect.ConfirmUserCredential(false);
		    }
		    finally
		    {
		        Cursor = currentCursor;
		    }
		}

		/// <summary>
		/// Handles the Click event of the search button.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void BtnSearchClick(object sender, EventArgs e)
		{
			PerformSearch();
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

        private void dgvList_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != 0) 
                return;

            var row = dgvList.Rows[e.RowIndex];
            var issue = ((Issue) row.DataBoundItem);

            if(issue == null)
                return;
            
            if ((bool)row.Cells[0].EditedFormattedValue)
                _selectedIssues.Add(issue.Id);
            else
                _selectedIssues.Remove(issue.Id);
        }
	}
}
