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
		public void GetProjectsTest()
		{
			var actual = _target.GetProjects();
			Assert.IsNotNull(actual);
			Assert.AreNotEqual(actual.Count, 0);
		}

		[TestMethod]
		public void GetStatesTest()
		{
			var actual = _target.GetStates();
			Assert.IsNotNull(actual);
			Assert.AreNotEqual(actual.Count, 0);
		}

		[TestMethod]
		public void GetPrioritiesTest()
		{
			var actual = _target.GetPriorities();
			Assert.IsNotNull(actual);
			Assert.AreNotEqual(actual.Count, 0);
		}

		[TestMethod]
		public void GetIssueTypesTest()
		{
			var actual = _target.GetIssueTypes();
			Assert.IsNotNull(actual);
			Assert.AreNotEqual(actual.Count, 0);
		}

		[TestMethod]
		public void GetIssuesTest()
		{
            var actual = _target.GetIssues("", "", 100);
			Assert.IsNotNull(actual);
			Assert.AreNotEqual(actual.Count(), 0);
		}
	}
}
