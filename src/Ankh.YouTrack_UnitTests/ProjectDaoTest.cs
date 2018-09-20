using System.Linq;
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

		[TestMethod]
		public async void GetProjectsTest()
		{
			var actual = await _target.GetProjectsAsync();
			Assert.IsNotNull(actual);
			Assert.AreNotEqual(actual.Count, 0);
		}

        [TestMethod]
		public async void GetIssuesTest()
		{
            var actual = await _target.GetIssuesAsync("", "");
			Assert.IsNotNull(actual);
			Assert.AreNotEqual(actual.Count(), 0);
		}
	}
}
