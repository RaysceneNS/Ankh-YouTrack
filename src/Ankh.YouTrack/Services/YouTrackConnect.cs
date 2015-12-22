using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Xml.Linq;
using Ankh.YouTrack.Services.Models;

namespace Ankh.YouTrack.Services
{
	internal class YouTrackConnect
    {
        private CredentialDialog _credDlg;
		private readonly Uri _uri;
		private readonly CookieContainer _cookieContainer = new CookieContainer();
		private bool _isLoggedIn;

	    public YouTrackConnect(Uri uri)
	    {
	        _uri = uri;
	    }
        
        public NetworkCredential GetUserCredential()
        {
            var cred = CredentialDialog.RetrieveCredentialFromApplicationInstanceCache(_uri.ToString());
            if (cred == null)
            {
                if (_credDlg == null)
                {
                    _credDlg = new CredentialDialog
                    {
                        Target = _uri.ToString(),
                        UseApplicationInstanceCredentialCache = true,
                        ShowSaveCheckBox = true,
                        ShowUIForSavedCredentials = true,
                        WindowTitle = "Issue Tracker",
                        MainInstruction = "Provide password for " + _uri
                    };
                }
                if (_credDlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    cred = _credDlg.Credentials;
                }
            }
            return cred;
        }

        public void ConfirmUserCredential(bool confirm)
        {
            if (_credDlg != null)
            {
                _credDlg.ConfirmCredentials(confirm);
            }
        }
        
	    /// <summary>
		/// Invoke the login method on the web server
		/// </summary>
		/// <param name="user">The user.</param>
		/// <param name="pwd">The password.</param>
		/// <returns>true if the login was successful</returns>
		public bool Login(string user, string pwd)
		{
			var dict = new Dictionary<string, string> {{"login", user}, {"password", pwd}};
		    var loginUri = new Uri(_uri.OriginalString.Replace(_uri.PathAndQuery, "/") + "rest/user/login");
			_isLoggedIn = Post(loginUri, dict);
			return _isLoggedIn;
		}

		/// <summary>
		/// Posts a web request to the uri.
		/// </summary>
		/// <param name="uri">The URI.</param>
		/// <param name="parameters">The parameters.</param>
		/// <returns></returns>
		private bool Post(Uri uri, ICollection<KeyValuePair<string, string>> parameters)
		{
			var formattedPostRequest = CreateFormattedPostRequest(parameters);
			byte[] bytes = Encoding.UTF8.GetBytes(formattedPostRequest);

			var req = (HttpWebRequest)WebRequest.Create(uri);
			req.ProtocolVersion = HttpVersion.Version11;
			req.Method = "POST";
			req.ContentLength = bytes.Length;
			req.ContentType = "application/x-www-form-urlencoded";
			req.CookieContainer = _cookieContainer;

			using (var rs = req.GetRequestStream())
			{
				rs.Write(bytes, 0, bytes.Length);
			}

			using (var response = (HttpWebResponse)req.GetResponse())
			{
			    if (response.StatusCode == HttpStatusCode.OK)
                    return true;
			    
                Trace.WriteLine(String.Format("POST failed. Received HTTP {0}", response.StatusCode));
			    return false;
			}
		}

		/// <summary>
		/// formats the key value pairs into WWW post format.
		/// </summary>
		/// <param name="values">The values.</param>
		/// <returns></returns>
		private static string CreateFormattedPostRequest(ICollection<KeyValuePair<string, string>> values)
		{
			var paramterBuilder = new StringBuilder();
			var counter = 0;
			foreach (var value in values)
			{
				paramterBuilder.AppendFormat("{0}={1}", value.Key, HttpUtility.UrlEncode(value.Value));

				if (counter != values.Count - 1)
					paramterBuilder.Append("&");
				counter++;
			}
			return paramterBuilder.ToString();
		}

		/// <summary>
		/// Makes the request.
		/// </summary>
		/// <param name="uri">The URI.</param>
		/// <returns></returns>
		private HttpWebRequest MakeRequest(Uri uri)
		{
			var req = (HttpWebRequest)WebRequest.Create(uri);
			req.ProtocolVersion = HttpVersion.Version11;
			req.Method = "GET";
			req.CookieContainer = _cookieContainer;
			return req;
		}

		/// <summary>
		/// Requests the doc.
		/// </summary>
		/// <param name="uri">The URI.</param>
		/// <returns></returns>
		private XDocument RequestDoc(Uri uri)
		{
			var req = MakeRequest(uri);
			var output = new StringBuilder();
			using (var resp = (HttpWebResponse)req.GetResponse())
			{
				Stream stream = resp.GetResponseStream();
				Encoding encode = Encoding.UTF8;

				if (stream == null)
					throw new NullReferenceException("Response stream is null.");

				// Pipes the stream to a higher level stream reader with the required encoding format. 
				using (var readStream = new StreamReader(stream, encode))
				{
					const int BUFFER_SIZE = 1024;
					var read = new char[BUFFER_SIZE];
					int count = readStream.Read(read, 0, BUFFER_SIZE);
					while (count > 0)
					{
						output.Append(new String(read, 0, count));
						count = readStream.Read(read, 0, BUFFER_SIZE);
					}
				}
			}
			return XDocument.Parse(output.ToString());
		}


		/// <summary>
		/// Gets the projects.
		/// </summary>
		/// <returns></returns>
		public IList<Project> GetProjects()
		{
			var projects = from c in RequestDoc(new Uri(_uri.OriginalString + "rest/project/all")).Descendants()
						   where c.Name.LocalName.Equals("project",StringComparison.OrdinalIgnoreCase)
						   select new Project
						   {
							   ShortName = c.Attribute("shortName").Value,
							   Name = c.Attribute("name").Value
						   };
			var list = projects.ToList();
			list.Insert(0, new Project{Name = "Everything", ShortName = ""});
			return list;
		}

		/// <summary>
		/// Get a list of all available states.
		/// </summary>
		/// <returns></returns>
		public IList<IssueState> GetStates()
		{
			var states = from c in RequestDoc(new Uri(_uri.OriginalString + "rest/project/states")).Descendants()
						 where c.Name.LocalName.Equals("state",StringComparison.OrdinalIgnoreCase)
						 select new IssueState
						   {
							   Resolved = bool.Parse(c.Attribute("resolved").Value),
							   Name = c.Attribute("name").Value
						   };
			return states.ToList();
		}

		/// <summary>
		/// Get a list of all available priorities.
		/// </summary>
		/// <returns></returns>
		public IList<IssuePriority> GetPriorities()
		{
            var priorities = from c in RequestDoc(new Uri(_uri.OriginalString + "rest/project/priorities")).Descendants()
							 where c.Name.LocalName.Equals("priority", StringComparison.OrdinalIgnoreCase)
							 select new IssuePriority
						   {
							   Priority = c.Attribute("priority").Value,
							   Name = c.Attribute("name").Value
						   };
			return priorities.ToList();
		}

		/// <summary>
		/// Get a list of all available issue types.
		/// </summary>
		/// <returns></returns>
		public IList<IssueType> GetIssueTypes()
		{
            var types = from c in RequestDoc(new Uri(_uri.OriginalString + "rest/project/types")).Descendants()
						where c.Name.LocalName.Equals("type", StringComparison.OrdinalIgnoreCase)
						select new IssueType
						   {
							   Name = c.Attribute("name").Value
						   };
			return types.ToList();
		}

		/// <summary>
		/// Executes the command.
		/// </summary>
		/// <param name="issueId">The issue id.</param>
		/// <param name="command">The command.</param>
		/// <param name="comment">The comment.</param>
		/// <param name="groupName">Name of the group.</param>
		/// <returns></returns>
		public void ExecuteCommand(string issueId, string command, string comment, string groupName)
		{
			if (string.IsNullOrEmpty(command))
				throw new Exception("Command is required.");

			var parameters = new Dictionary<string, string> { { "command", command } };

			if (!string.IsNullOrEmpty(comment))
				parameters.Add("comment", comment);

			if (!string.IsNullOrEmpty(groupName))
				parameters.Add("group", groupName);

            var uri = new Uri(_uri.OriginalString + "rest/issue/" + issueId + "/execute");
			Post(uri, parameters);
		}

		/// <summary>
		/// Gets the issues.
		/// </summary>
		/// <param name="projectId">The project id.</param>
		/// <param name="searchTerm">The search string to execute.</param>
		/// <param name="maxRecords">The max records.</param>
		/// <returns></returns>
		public IEnumerable<Issue> GetIssues(string projectId, string searchTerm, int maxRecords)
		{
			string queryString = string.IsNullOrEmpty(projectId) ? string.Format("rest/issue?max={0}", maxRecords) : string.Format("rest/issue/byproject/{0}?max={1}", projectId, maxRecords);

			if (!string.IsNullOrEmpty(searchTerm))
			{
				queryString += "&filter=" + HttpUtility.UrlEncode(searchTerm);
			}

            var xd = RequestDoc(new Uri(_uri.OriginalString + queryString));

			var issues = from c in xd.Descendants()
						 where c.Name.LocalName.Equals("issue", StringComparison.OrdinalIgnoreCase)
						 select new Issue
						 {
							 Id = c.Attribute("id").Value,
							 Created = new DateTime(1970, 1, 1).AddSeconds(long.Parse(c.GetStringValue("created")) / 1000.0),
							 Updated = new DateTime(1970, 1, 1).AddSeconds(long.Parse(c.GetStringValue("updated")) / 1000.0),
							 Summary = c.GetStringValue("summary"),
							 ReporterName = c.GetStringValue("reporterName"),
							 UpdaterName = c.GetStringValue("updaterName"),
							 AssigneeName = c.GetStringValue("assignee"),
							 Priority = c.GetStringValue("priority"),
							 State = c.GetStringValue("state"),
							 Type = c.GetStringValue("type"),
							 Project = c.GetStringValue("projectShortName")
						 };
			return issues;
		}
    }
}
