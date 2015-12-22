using System;
using Ankh.YouTrack;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VSSDK.Tools.VsIdeTesting;
using Microsoft.VisualStudio.Shell.Interop;

namespace Ankh.YouTrack_IntegrationTests
{
	/// <summary>
	/// Integration test for package validation
	/// </summary>
	[TestClass]
	public class PackageTest
	{
		private delegate void ThreadInvoker();

		[TestMethod]
		[HostType("VS IDE")]
		public void PackageLoadTest()
		{
			UIThreadInvoker.Invoke((ThreadInvoker)delegate
			{
				//Get the Shell Service
				var shellService = VsIdeTestHostContext.ServiceProvider.GetService(typeof(SVsShell)) as IVsShell;
				Assert.IsNotNull(shellService);

				//Validate package load
				IVsPackage package;
				var packageGuid = new Guid(AppConstants.PACKAGE_GUID);
				Assert.IsTrue(0 == shellService.LoadPackage(ref packageGuid, out package));
				Assert.IsNotNull(package, "Package failed to load");
			});
		}
	}
}
