using System;
using System.Text;
using System.Windows.Forms;
using Ankh.ExtensionPoints.IssueTracker;
using Ankh.YouTrack.IssueTracker.Forms;

namespace Ankh.YouTrack.IssueTracker
{
    /// <summary>
    /// Represents an Issue Repository
    /// </summary>
    internal class AnkhRepository : IssueRepository, IWin32Window, IDisposable
    {
	    private readonly Uri _uri;
		private readonly string _repositoryId;
		private IssuesView _control;

		/// <summary>
		/// Creates the repository from the supplied settings.
		/// </summary>
		/// <param name="settings">The settings.</param>
		/// <returns></returns>
        public static IssueRepository Create(IssueRepositorySettings settings)
		{
		    return settings != null ? new AnkhRepository(settings.RepositoryUri, settings.RepositoryId) : null;
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="AnkhRepository"/> class.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="repositoryId">The repository id.</param>
        public AnkhRepository(Uri uri, string repositoryId)
            : base(AppConstants.CONNECTOR_NAME)
        {
            _uri = uri;
            _repositoryId = repositoryId;
        }

        /// <summary>
        /// Gets the repository connection URL. This is passed to 
        /// us from the vs-issuerepository-uri svn property
        /// </summary>
        public override Uri RepositoryUri
        {
            get { return _uri; }
        }

        /// <summary>
        /// Gets the repository id hosted on the RepositoryUri. This is passed to 
        /// us from the vs-issuerepository-id svn property
        /// </summary>
        /// <remarks>optional</remarks>
        public override string RepositoryId
        {
            get { return _repositoryId; }
        }

        /// <summary>
        /// Gets the repository label
        /// </summary>
        public override string Label
        {
            get { return RepositoryId ?? (RepositoryUri == null ? string.Empty : RepositoryUri.ToString()); }
        }

		/// <summary>
		/// Called just before the changes are committed.
		/// </summary>
		/// <param name="args"></param>
        public override void PreCommit(PreCommitArgs args)
        {
            // modify commit message here
            var sb = new StringBuilder(args.CommitMessage ?? string.Empty);

			if (_control != null)
			{
				var issues = _control.SelectedIssues;

				bool first = true;
				foreach (var issue in issues)
				{
				    if (first)
				    {
				        sb.Append(Environment.NewLine);
				        sb.Append("Issue: ");
				        sb.Append(issue);
				        first = false;
				    }
				    else
				    {
                        sb.Append(", ");
                        sb.Append(issue);
				    }
				}
			}

			args.CommitMessage = sb.ToString();
            args.Cancel = false; // true if "some" pre-commit check fails
        }
        
		/// <summary>
		/// Show issue details
		/// </summary>
		/// <param name="issueId">Issue identifier</param>
        public override void NavigateTo(string issueId)
        {
            // show issue details in the browser
		    if (!string.IsNullOrEmpty(issueId))
		    {
		        var issueUri = RepositoryUri + "/issue/" + issueId;
		        System.Diagnostics.Process.Start(issueUri);
		    }
        }

		/// <summary>
		/// Gets the handle to the window represented by the implementer.
		/// </summary>
		/// <returns>A handle to the window represented by the implementer.</returns>
        public IntPtr Handle
        {
            get { return Control.Handle; }
        }

        private IssuesView Control
        {
            get { return _control ?? (_control = new IssuesView(_uri, _repositoryId)); }
        }

        #region IDisposable Members

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
        public void Dispose()
        {
		    if (_control != null && !_control.IsDisposed && !_control.Disposing)
		    {
		        _control.Dispose();
		    }
        	_control = null;
        }

        #endregion
    }
}
