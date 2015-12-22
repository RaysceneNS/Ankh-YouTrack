using System.Diagnostics;

namespace Ankh.YouTrack.Services.Models
{
	[DebuggerDisplay("{Name}")]
    public class Project
    {
        public string ShortName { get; set; }

        public string Name { get; set; }

        public override string ToString()
        {
            return this.Name;
        }
    }
}