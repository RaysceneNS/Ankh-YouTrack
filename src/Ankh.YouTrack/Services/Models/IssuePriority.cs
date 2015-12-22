using System.Diagnostics;

namespace Ankh.YouTrack.Services.Models
{
	[DebuggerDisplay("{Name}, priority {Priority}")]
	public class IssuePriority
	{
		public string Name { get; set; }
		public string Priority { get; set; }

		public override string ToString()
		{
			return this.Name;
		}
	}
}
