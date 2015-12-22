using System.Linq;
using Ankh.YouTrack.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ankh.YouTrack_UnitTests
{
	/// <summary>
	///This is a test class for ProjectDaoTest and is intended
	///to contain all ProjectDaoTest Unit Tests
	///</summary>
	[TestClass]
	public class ProjectDaoTest
	{
        private YouTrackConnect _target;

		/// <summary>
		/// Initializes this instance.
		/// </summary>
		[TestInitialize]
		public void Initialize()
		{
            _target = new YouTrackConnect(Constants.RepositoryUri);
		}

		/// <summary>
		///A test for GetProjects
		///</summary>
		[TestMethod]
		public void GetProjectsTest()
		{
			var actual = _target.GetProjects();
			Assert.IsNotNull(actual);
			Assert.AreNotEqual(actual.Count, 0);
		}

		/// <summary>
		///A test for GetStates
		///</summary>
		[TestMethod]
		public void GetStatesTest()
		{
			var actual = _target.GetStates();
			Assert.IsNotNull(actual);
			Assert.AreNotEqual(actual.Count, 0);
		}

		/// <summary>
		///A test for GetPriorities
		///</summary>
		[TestMethod]
		public void GetPrioritiesTest()
		{
			var actual = _target.GetPriorities();
			Assert.IsNotNull(actual);
			Assert.AreNotEqual(actual.Count, 0);
		}

		/// <summary>
		///A test for GetStates
		///</summary>
		[TestMethod]
		public void GetIssueTypesTest()
		{
			var actual = _target.GetIssueTypes();
			Assert.IsNotNull(actual);
			Assert.AreNotEqual(actual.Count, 0);
		}

		/// <summary>
		///A test for GetAllIssuesByProjectId
		///</summary>
		[TestMethod]
		public void GetIssuesTest()
		{
            var actual = _target.GetIssues("", "", 100);
			Assert.IsNotNull(actual);
			Assert.AreNotEqual(actual.Count(), 0);
		}
	}
}
