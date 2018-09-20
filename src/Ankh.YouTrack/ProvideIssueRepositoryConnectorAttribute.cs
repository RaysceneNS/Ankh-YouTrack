using System;
using Microsoft.VisualStudio.Shell;

namespace Ankh.YouTrack
{
	/// <summary>
	/// This attribute registers the package as Issue Repository Connector.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
	[System.Runtime.InteropServices.Guid("072C0B48-7BCC-49ce-927C-1EC92279E8CC")]
	public sealed class ProvideIssueRepositoryConnectorAttribute : RegistrationAttribute
	{
        private const string RegKeyConnectors = "IssueRepositoryConnectors";
        private const string RegKeyName = "Name";
        private const string RegValueService = "Service";
        private const string RegValuePackage = "Package";
        
        private readonly Type _connectorService;
		private readonly string _regName;
		private readonly string _uiName;
		private readonly Type _uiNamePkg;

		/// <summary>
		/// Initializes a new instance of the <see cref="ProvideIssueRepositoryConnectorAttribute"/> class.
		/// </summary>
		/// <param name="connectorServiceType">Type of the connector service.</param>
		/// <param name="regName">Name of the reg.</param>
		/// <param name="uiNamePkg">The UI name PKG.</param>
		/// <param name="uiName">Name of the UI.</param>
		public ProvideIssueRepositoryConnectorAttribute(Type connectorServiceType, string regName, Type uiNamePkg, string uiName)
		{
			this._connectorService = connectorServiceType;
			this._regName = regName;
			this._uiNamePkg = uiNamePkg;
			this._uiName = uiName;
		}

        /// <summary>
        /// Gets Issue repository connector service's global identifier.
        /// </summary>
        public Guid IssueRepositoryConnectorService
        {
            get
            {
                return _connectorService.GUID;
            }
        }

        /// <summary>
        /// Gets the name of the issue repository connector (used in registry)
        /// </summary>
        public string RegName
        {
            get
            {
                return _regName;
            }
        }

        /// <summary>
        /// Gets the global identifier used to register the issue repository connector
        /// </summary>
        public Guid RegGuid
        {
            get
            {
                return IssueRepositoryConnectorService;
            }
        }

        /// <summary>
        /// Gets the package identifier the proffers the connector service.
        /// </summary>
        public Guid UINamePkg
        {
            get
            {
                return _uiNamePkg.GUID;
            }
        }

        /// <summary>
        /// Gets the string resource identifier that represents the UI name of the issue tracker repository connector.
        /// </summary>
        public string UIName
        {
            get
            {
                return _uiName;
            }
        }

		/// <summary>
		/// Registers this VSPackage with a given context, when called by an external registration tool such as regpkg.exe. 
		/// For more information, see Registering VSPackages.
		/// </summary>
		/// <param name="context">A registration context provided by an external registration tool. The context can be 
		/// used to create registry keys, log registration activity, and obtain information about the component being registered.
		/// </param>
		public override void Register(RegistrationContext context)
		{
			context.Log.WriteLine("Issue Repository Connector:\t\t{0}\n", this._regName);
            using (var connectorsKey = context.CreateKey(RegKeyConnectors))
			{
                using (var connectorKey = connectorsKey.CreateSubkey(RegGuid.ToString("B").ToUpperInvariant()))
				{
					connectorKey.SetValue("", this.RegName);
                    connectorKey.SetValue(RegValueService, IssueRepositoryConnectorService.ToString("B").ToUpperInvariant());

                    using (var connectorNameKey = connectorKey.CreateSubkey(RegKeyName))
					{
						connectorNameKey.SetValue("", UIName);
                        connectorNameKey.SetValue(RegValuePackage, UINamePkg.ToString("B").ToUpperInvariant());
						connectorNameKey.Close();
					}
					connectorKey.Close();
				}
				connectorsKey.Close();
			}
		}

		/// <summary>
		/// Removes registration information about a VSPackage when called by an external registration tool such 
		/// as regpkg.exe. For more information, see Registering VSPackages. Any class deriving from the 
		/// RegistrationAttribute class must implement this method.
		/// </summary>
		/// <param name="context">A registration context provided by an external registration tool. The context 
		/// can be used to remove registry keys, log registration activity, and obtain information about the 
		/// component being registered.
		/// </param>
		public override void Unregister(RegistrationContext context)
		{
			context.RemoveKey(RegKeyConnectors + @"\" + RegGuid.ToString("B"));
		}
	}
}
