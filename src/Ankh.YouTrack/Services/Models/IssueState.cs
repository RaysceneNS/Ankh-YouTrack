using System.Diagnostics;

namespace Ankh.YouTrack.Services.Models
{
	[DebuggerDisplay("{" + nameof(Name) + "}")]
	public class IssueState
	{
		public string Name { get; set; }

		public bool Resolved { get; set; }

		public override string ToString()
		{
			return this.Name;
		}
	}
}
