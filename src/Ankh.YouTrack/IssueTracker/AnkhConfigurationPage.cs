using System;
using System.Windows.Forms;
using Ankh.ExtensionPoints.IssueTracker;
using Ankh.YouTrack.IssueTracker.Forms;

namespace Ankh.YouTrack.IssueTracker
{
    /// <summary>
    /// Configuration page 
    /// </summary>
    internal class AnkhConfigurationPage : IssueRepositoryConfigurationPage, IWin32Window
    {
        private ConfigurationPage _control;

        /// <summary>
        /// Gets or Sets the config settings
        /// </summary>
        public override IssueRepositorySettings Settings
        {
            get
            {
                return _control != null ? _control.UiToSettings() : base.Settings;
            }
            set
            {
                if (value != null && value.ConnectorName == AppConstants.CONNECTOR_NAME)
                {
                    ((ConfigurationPage)Control).SettingsToUi(value);
                }
            }
        }

        private UserControl Control
        {
            get
            {
                if (_control == null)
                {
                    _control = new ConfigurationPage();
                    _control.OnPageChanged += delegate(object sender, ConfigPageEventArgs args) { ConfigurationPageChanged(args); }; 
                }
                return _control;
            }
        }
        
        public IntPtr Handle
        {
            get { return Control.Handle; }
        }
    }
}
