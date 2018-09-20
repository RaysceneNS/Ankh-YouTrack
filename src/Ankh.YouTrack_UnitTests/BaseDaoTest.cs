using Ankh.YouTrack.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ankh.YouTrack_UnitTests
{
	[TestClass]
	public class BaseDaoTest
	{
		[TestMethod]
		public void GetUserCredentialTest()
		{
            var service = new YouTrackConnect(Constants.RepositoryUri);
            var cred = service.GetUserCredential();
	        Assert.IsNotNull(cred);
		}
	}
}
