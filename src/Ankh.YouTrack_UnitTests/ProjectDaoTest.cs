using System.Net;
using System.Threading.Tasks;
using Ankh.YouTrack.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ankh.YouTrack_UnitTests
{
	[TestClass]
	public class ProjectDaoTest
	{
        private YouTrackConnect _target;

		[TestInitialize]
		public void Initialize()
		{
            _target = new YouTrackConnect(Constants.RepositoryUri);
		}

	    [TestMethod, ExpectedException(typeof(WebException))]
	    public async Task LoginTest()
	    {
	        var actual = await _target.LoginAsync("user", "pass");
	        Assert.IsNotNull(actual);
	    }

        [TestMethod]
		public async Task GetProjectsTest()
		{
			var actual = await _target.GetProjectsAsync();
			Assert.IsNotNull(actual);
			Assert.AreNotEqual(actual.Count, 0);
		}

	    [TestMethod]
	    public async Task GetIssuesTest()
	    {
	        var actual = await _target.GetIssuesAsync("", "");
	        Assert.IsNotNull(actual);
	        Assert.AreNotEqual(actual.Count, 0);
	    }

	    [TestMethod, ExpectedException(typeof(WebException))]
	    public async Task GetIssuesTestInvalidProject()
	    {
	        var actual = await _target.GetIssuesAsync("9287267jhsgs52g", "");
	        Assert.IsNotNull(actual);
	    }

	    [TestMethod]
	    public async Task GetIssuesTestInvalidSearch()
	    {
	        var actual = await _target.GetIssuesAsync("", "foobar9287267jhsgs52g");
	        Assert.IsNotNull(actual);
	        Assert.AreEqual(actual.Count, 0);
	    }
    }
}
