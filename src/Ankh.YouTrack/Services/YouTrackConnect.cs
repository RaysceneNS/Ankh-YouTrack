using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;
using Ankh.YouTrack.Services.Models;

namespace Ankh.YouTrack.Services
{
	internal class YouTrackConnect
    {
        private CredentialDialog _credDlg;
		private Uri _uri;
		private readonly CookieContainer _cookieContainer = new CookieContainer();
		private bool _isLoggedIn;

        public YouTrackConnect()
        {
        }

        public YouTrackConnect(Uri uri)
        {
            _uri = uri;
        }

        public Uri Uri
        {
            get { return _uri; }
            set { _uri = value; }
        }

        public NetworkCredential GetUserCredential()
        {
            var credential = CredentialDialog.RetrieveCredentialFromApplicationInstanceCache(_uri.ToString());
            if (credential != null)
                return credential;

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
                credential = _credDlg.Credentials;
            }
            return credential;
        }

        public void ConfirmUserCredential(bool confirm)
        {
            _credDlg?.ConfirmCredentials(confirm);
        }
        
	    /// <summary>
		/// Invoke the login method on the web server
		/// </summary>
		/// <param name="user">The user.</param>
		/// <param name="pwd">The password.</param>
		/// <returns>true if the login was successful</returns>
		public async Task<bool> LoginAsync(string user, string pwd)
		{
			var dict = new Dictionary<string, string> {{"login", user}, {"password", pwd}};
		    var loginUri = new Uri(_uri.OriginalString.Replace(_uri.PathAndQuery, "/") + "rest/user/login");
			_isLoggedIn = await PostAsync(loginUri, dict);
			return _isLoggedIn;
		}

		/// <summary>
		/// Posts a web request to the uri.
		/// </summary>
		/// <param name="uri">The URI.</param>
		/// <param name="parameters">The parameters.</param>
		/// <returns></returns>
		private async Task<bool> PostAsync(Uri uri, ICollection<KeyValuePair<string, string>> parameters)
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

			using (var response = (HttpWebResponse)await req.GetResponseAsync())
			{
			    if (response.StatusCode == HttpStatusCode.OK)
                    return true;
			    
                Trace.WriteLine($"POST failed. Received HTTP {response.StatusCode}");
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
			var parameterBuilder = new StringBuilder();
			var counter = 0;
			foreach (var value in values)
			{
				parameterBuilder.AppendFormat("{0}={1}", value.Key, HttpUtility.UrlEncode(value.Value));

				if (counter != values.Count - 1)
					parameterBuilder.Append("&");
				counter++;
			}
			return parameterBuilder.ToString();
		}
        
		/// <summary>
		/// Requests the xml document from uri.
		/// </summary>
		/// <param name="uri">The URI.</param>
		/// <returns></returns>
		private async Task<XDocument> RequestDocumentAsync(Uri uri)
		{
		    var req = WebRequest.CreateHttp(uri);
		    req.ProtocolVersion = HttpVersion.Version11;
		    req.Method = "GET";
		    req.CookieContainer = _cookieContainer;

            var output = new StringBuilder();
			using (var resp = (HttpWebResponse) await req.GetResponseAsync())
			{
				var stream = resp.GetResponseStream();
				var encode = Encoding.UTF8;

				if (stream == null)
					throw new NullReferenceException("Response stream is null.");

				// Pipes the stream to a higher level stream reader with the required encoding format. 
				using (var readStream = new StreamReader(stream, encode))
				{
					const int BUFFER_SIZE = 10240;
					var read = new char[BUFFER_SIZE];
					int count = readStream.Read(read, 0, BUFFER_SIZE);
					while (count > 0)
					{
						output.Append(new string(read, 0, count));
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
		public async Task<IList<Project>> GetProjectsAsync()
		{
		    var doc = await RequestDocumentAsync(new Uri(_uri.OriginalString + "rest/project/all"));
            var projects = from c in doc.Descendants()
						   where c.Name.LocalName.Equals("project",StringComparison.OrdinalIgnoreCase)
						   select new Project
						   {
							   ShortName = c.Attribute("shortName")?.Value
						   };
			var list = projects.ToList();
		    list.Insert(0, new Project {ShortName = ""});
			return list;
		}

        /// <summary>
		/// Gets the issues.
		/// </summary>
		/// <param name="projectId">The project id.</param>
		/// <param name="searchTerm">The search string to execute.</param>
		/// <param name="maxRecords">The max records.</param>
		/// <returns></returns>
		public async Task<IList<Issue>> GetIssuesAsync(string projectId, string searchTerm, int maxRecords = 100)
		{
			string queryString = string.IsNullOrEmpty(projectId) ? $"rest/issue?max={maxRecords}" : $"rest/issue/byproject/{projectId}?max={maxRecords}";

			if (!string.IsNullOrEmpty(searchTerm))
			{
				queryString += "&filter=" + HttpUtility.UrlEncode(searchTerm)
				                   .Replace("%23", "#"); //unencode the hash character, the rest api wants these raw
			}

            var xd = await RequestDocumentAsync(new Uri(_uri.OriginalString + queryString));

			var issues = from c in xd.Descendants()
						 where c.Name.LocalName.Equals("issue", StringComparison.OrdinalIgnoreCase)
						 select new Issue
						 {
							 Id = c.Attribute("id")?.Value,
							 Created = c.GetDateTimeValue("created"),
							 Updated = c.GetDateTimeValue("updated"),
							 Summary = c.GetStringValue("summary"),
							 ReporterName = c.GetStringValue("reporterName"),
							 UpdaterName = c.GetStringValue("updaterName"),
							 AssigneeName = c.GetStringValue("assignee"),
							 Priority = c.GetStringValue("priority"),
							 State = c.GetStringValue("state"),
							 Type = c.GetStringValue("type"),
							 Project = c.GetStringValue("projectShortName")
						 };
			return issues.ToList();
		}
    }
}
