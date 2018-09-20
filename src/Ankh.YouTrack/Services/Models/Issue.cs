using System;
using System.Diagnostics;

namespace Ankh.YouTrack.Services.Models
{
	[DebuggerDisplay("{" + nameof(Id) + "}")]
    public class Issue
    {
        public string Id { get; set; }
		public string Summary { get; set; }
		public string ReporterName { get; set; }
		public string UpdaterName { get; set; }
        public string AssigneeName { get; set; }
        public string State { get; set; }
        public string Project { get; set; }
		public string Priority { get; set; }
		public string Type { get; set; }
        public DateTime Created { get; set; }
		public DateTime Updated { get; set; }
    }
}