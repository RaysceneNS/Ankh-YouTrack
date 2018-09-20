using Ankh.YouTrack;
using Microsoft.VsSDK.UnitTestLibrary;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ankh.YouTrack_UnitTests
{
	[TestClass]
	public class PackageTest
	{
		[TestMethod]
		public void CreateInstance()
		{
			var package = new AnkhYouTrackPackage();
            Assert.IsNotNull(package);
		}

		[TestMethod]
		public void IsIVsPackage()
		{
			var package = new AnkhYouTrackPackage() as IVsPackage;
			Assert.IsNotNull(package, "The object does not implement IVsPackage");
		}

	    [TestMethod]
	    public void PackageSetSite()
	    {
	        IVsPackage package = new AnkhYouTrackPackage();
	        var serviceProvider = OleServiceProvider.CreateOleServiceProviderWithBasicServices();
	        Assert.AreEqual(0, package.SetSite(serviceProvider), "SetSite did not return S_OK");
	    }

	    [TestMethod]
	    public void PackageUnSetSite()
	    {
	        IVsPackage package = new AnkhYouTrackPackage();
	        Assert.AreEqual(0, package.SetSite(null), "SetSite(null) did not return S_OK");
	    }
	}
}
