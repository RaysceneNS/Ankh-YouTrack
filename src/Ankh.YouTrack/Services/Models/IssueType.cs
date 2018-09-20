using System.Diagnostics;

namespace Ankh.YouTrack.Services.Models
{
	[DebuggerDisplay("{" + nameof(Name) + "}")]
	public class IssueType
	{
		public string Name { get; set; }

		public override string ToString()
		{
			return this.Name;
		}
	}
}
