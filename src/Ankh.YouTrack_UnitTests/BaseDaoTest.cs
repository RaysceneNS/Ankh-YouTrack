using Ankh.YouTrack.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ankh.YouTrack_UnitTests
{
	[TestClass]
	public class BaseDaoTest
	{
	    /// <summary>
		///A test for GetProjects
		///</summary>
		[TestMethod]
		public void LoginTest()
		{
            YouTrackConnect service = new YouTrackConnect(Constants.RepositoryUri);

            var cred = service.GetUserCredential();
	        Assert.IsNotNull(cred);
		}
	}
}
