using System;
using System.Windows.Forms;
using Ankh.ExtensionPoints.IssueTracker;
using Ankh.YouTrack.IssueTracker.Forms;

namespace Ankh.YouTrack.IssueTracker
{
    /// <summary>
    /// Configuration page 
    /// </summary>
    class AnkhConfigurationPage : IssueRepositoryConfigurationPage, IWin32Window
    {
        private ConfigurationPage _control;

        /// <summary>
        /// Gets or Sets the config settings
        /// </summary>
        public override IssueRepositorySettings Settings
        {
            get
            {
                if (_control != null)
                {
                    return _control.Settings;
                }

                return base.Settings;
            }
            set
            {
                if (value != null
                    && value.ConnectorName == AppConstants.CONNECTOR_NAME)
                {
                    // populate UI with new settings
                    ((ConfigurationPage)Control).Settings = value;
                }
            }
        }

        private UserControl Control
        {
            get
            {
                if (_control == null)
                {
                    var control = new ConfigurationPage();
                    control.OnPageEvent += control_OnPageEvent;
                    _control = control;
                }
                return _control;
            }
        }

        void control_OnPageEvent(object sender, ConfigPageEventArgs e)
        {
            // raise page changed event to notify AnkhSVN
            ConfigurationPageChanged(e);
        }

        #region IWin32Window Members

        public IntPtr Handle
        {
            get { return Control.Handle; }
        }

        #endregion
    }
}
