using System.Diagnostics;

namespace Ankh.YouTrack.Services.Models
{
	[DebuggerDisplay("{" + nameof(ShortName) + "}")]
    public class Project
    {
        public string ShortName { get; set; }
        
        public override string ToString()
        {
            return this.ShortName;
        }
    }
}