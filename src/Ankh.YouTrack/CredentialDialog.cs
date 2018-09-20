﻿// Copyright © Sven Groot (Ookii.org) 2009
// BSD license; see license.txt for details.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Permissions;
using System.Text;
using System.Windows.Forms;

namespace Ankh.YouTrack
{
    /// <summary>
    /// An enumeration that displays how the text in the <see cref="CredentialDialog.MainInstruction"/> and <see cref="CredentialDialog.Content"/>
    /// properties is displayed on a credential dialog in Windows XP.
    /// </summary>
    /// <remarks>
    /// <para>
    ///   Windows XP does not support the distinct visual style of the main instruction, so there is no visual difference between the
    ///   text of the <see cref="CredentialDialog.MainInstruction"/> and <see cref="CredentialDialog.Content"/> properties. Depending
    ///   on the scenario, you may wish to hide either the main instruction or the content text.
    /// </para>
    /// </remarks>
    public enum DownlevelTextMode
    {
        /// <summary>
        /// The text of the <see cref="CredentialDialog.MainInstruction"/> and <see cref="CredentialDialog.Content"/> properties is
        /// concatenated together, separated by an empty line.
        /// </summary>
        MainInstructionAndContent,
        /// <summary>
        /// Only the text of the <see cref="CredentialDialog.MainInstruction"/> property is shown.
        /// </summary>
        MainInstructionOnly,
        /// <summary>
        /// Only the text of the <see cref="CredentialDialog.Content"/> property is shown.
        /// </summary>
        ContentOnly
    }

    // ReSharper disable UnusedMember.Global
    // ReSharper disable InconsistentNaming
    static class NativeMethods
    {
        public static bool IsWindowsVistaOrLater
        {
            get
            {
                return Environment.OSVersion.Platform == PlatformID.Win32NT && Environment.OSVersion.Version >= new Version(6, 0, 6000);
            }
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr GetActiveWindow();

        #region Credentials

        internal const int CREDUI_MAX_USERNAME_LENGTH = 256 + 1 + 256;
        internal const int CREDUI_MAX_PASSWORD_LENGTH = 256;

        [Flags]
        public enum CREDUI_FLAGS
        {
            INCORRECT_PASSWORD = 0x1,
            DO_NOT_PERSIST = 0x2,
            REQUEST_ADMINISTRATOR = 0x4,
            EXCLUDE_CERTIFICATES = 0x8,
            REQUIRE_CERTIFICATE = 0x10,
            SHOW_SAVE_CHECK_BOX = 0x40,
            ALWAYS_SHOW_UI = 0x80,
            REQUIRE_SMARTCARD = 0x100,
            PASSWORD_ONLY_OK = 0x200,
            VALIDATE_USERNAME = 0x400,
            COMPLETE_USERNAME = 0x800,
            PERSIST = 0x1000,
            SERVER_CREDENTIAL = 0x4000,
            EXPECT_CONFIRMATION = 0x20000,
            GENERIC_CREDENTIALS = 0x40000,
            USERNAME_TARGET_CREDENTIALS = 0x80000,
            KEEP_USERNAME = 0x100000
        }

        [Flags]
        public enum CredUIWinFlags
        {
            Generic = 0x1,
            Checkbox = 0x2,
            AutoPackageOnly = 0x10,
            InCredOnly = 0x20,
            EnumerateAdmins = 0x100,
            EnumerateCurrentUser = 0x200,
            SecurePrompt = 0x1000,
            Pack32Wow = 0x10000000
        }

        internal enum CredUIReturnCodes
        {
            NO_ERROR = 0,
            ERROR_CANCELLED = 1223,
            ERROR_NO_SUCH_LOGON_SESSION = 1312,
            ERROR_NOT_FOUND = 1168,
            ERROR_INVALID_ACCOUNT_NAME = 1315,
            ERROR_INSUFFICIENT_BUFFER = 122,
            ERROR_INVALID_PARAMETER = 87,
            ERROR_INVALID_FLAGS = 1004
        }

        internal enum CredTypes
        {
            CRED_TYPE_GENERIC = 1,
            CRED_TYPE_DOMAIN_PASSWORD = 2,
            CRED_TYPE_DOMAIN_CERTIFICATE = 3,
            CRED_TYPE_DOMAIN_VISIBLE_PASSWORD = 4
        }

        internal enum CredPersist
        {
            Session = 1,
            LocalMachine = 2,
            Enterprise = 3
        }

        [SuppressMessage("Microsoft.Design", "CA1049:TypesThatOwnNativeResourcesShouldBeDisposable")]
        internal struct CREDUI_INFO
        {
            public int cbSize;
            public IntPtr hwndParent;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string pszMessageText;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string pszCaptionText;
            public IntPtr hbmBanner;
        }

        [DllImport("credui.dll", CharSet = CharSet.Unicode)]
        extern static internal CredUIReturnCodes CredUIPromptForCredentials(
            ref CREDUI_INFO pUiInfo,
            string targetName,
            IntPtr reserved,
            int dwAuthError,
            StringBuilder pszUserName,
            uint ulUserNameMaxChars,
            StringBuilder pszPassword,
            uint ulPaswordMaxChars,
            [MarshalAs(UnmanagedType.Bool), In(), Out()] ref bool pfSave,
            CREDUI_FLAGS dwFlags);

        [DllImport("credui.dll", CharSet = CharSet.Unicode)]
        public static extern CredUIReturnCodes CredUIPromptForWindowsCredentials(
            ref CREDUI_INFO pUiInfo,
            uint dwAuthError,
            ref uint pulAuthPackage,
            IntPtr pvInAuthBuffer,
            uint ulInAuthBufferSize,
            out IntPtr ppvOutAuthBuffer,
            out uint pulOutAuthBufferSize,
            [MarshalAs(UnmanagedType.Bool)]ref bool pfSave,
            CredUIWinFlags dwFlags);

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, EntryPoint = "CredReadW", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        extern static internal bool CredRead(string TargetName, CredTypes Type, int Flags, out IntPtr Credential);

        [DllImport("advapi32.dll"), ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        extern static internal void CredFree(IntPtr Buffer);

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, EntryPoint = "CredDeleteW", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        extern static internal bool CredDelete(string TargetName, CredTypes Type, int Flags);

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, EntryPoint = "CredWriteW", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        extern static internal bool CredWrite(ref CREDENTIAL Credential, int Flags);

        [DllImport("credui.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CredPackAuthenticationBuffer(uint dwFlags, string pszUserName, string pszPassword, IntPtr pPackedCredentials, ref uint pcbPackedCredentials);

        [DllImport("credui.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CredUnPackAuthenticationBuffer(uint dwFlags, IntPtr pAuthBuffer, uint cbAuthBuffer, StringBuilder pszUserName, ref uint pcchMaxUserName, StringBuilder pszDomainName, ref uint pcchMaxDomainName, StringBuilder pszPassword, ref uint pcchMaxPassword);

        // Disable the "Internal field is never assigned to" warning.
#pragma warning disable 649
        // This type does not own the IntPtr native resource; when CredRead is used, CredFree must be called on the
        // IntPtr that the struct was marshalled from to release all resources including the CredentialBlob IntPtr,
        // When allocating the struct manually for CredWrite you should also manually deallocate the CredentialBlob.
        [SuppressMessage("Microsoft.Design", "CA1049:TypesThatOwnNativeResourcesShouldBeDisposable")]
        public struct CREDENTIAL
        {
            public int Flags;
            public CredTypes Type;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string TargetName;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string Comment;
            public long LastWritten;
            public uint CredentialBlobSize;
            // Since the resource pointed to must be either released manually or by CredFree, SafeHandle is not appropriate here
            [SuppressMessage("Microsoft.Reliability", "CA2006:UseSafeHandleToEncapsulateNativeResources")]
            public IntPtr CredentialBlob;
            [MarshalAs(UnmanagedType.U4)]
            public CredPersist Persist;
            public int AttributeCount;
            public IntPtr Attributes;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string TargetAlias;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string UserName;
        }
#pragma warning restore 649

        #endregion
    }

    /// <summary>
    /// Represents a dialog box that allows the user to enter generic credentials.
    /// </summary>
    /// <remarks>
    /// <para>
    ///   This class is meant for generic credentials; it does not provide access to all the functionality
    ///   of the Windows CredUI API. Features such as Windows domain credentials or alternative security
    ///   providers (e.g. smartcards or biometric devices) are not supported.
    /// </para>
    /// <para>
    ///   The <see cref="CredentialDialog"/> class provides methods for storing and retrieving credentials,
    ///   and also manages automatic persistence of credentials by using the "Save password" checkbox on
    ///   the credentials dialog. To specify the target for which the credentials should be saved, set the
    ///   <see cref="Target"/> property.
    /// </para>
    /// <note>
    ///   This class requires Windows XP or later.
    /// </note>
    /// </remarks>
    /// <threadsafety instance="false" static="true" />
    [DefaultProperty("MainInstruction"), DefaultEvent("UserNameChanged"), Description("Allows access to credential UI for generic credentials.")]
    public class CredentialDialog : Component
    {
        private string _confirmTarget;
        private readonly NetworkCredential _credentials = new NetworkCredential();
        private bool _isSaveChecked;
        private string _target;

        private static readonly Dictionary<string, NetworkCredential> _applicationInstanceCredentialCache = new Dictionary<string, NetworkCredential>();
        private string _caption;
        private string _text;
        private string _windowTitle;

        /// <summary>
        /// Event raised when the <see cref="UserName"/> property changes.
        /// </summary>
        [Category("Property Changed"), Description("Event raised when the value of the UserName property changes.")]
        public event EventHandler UserNameChanged;
        /// <summary>
        /// Event raised when the <see cref="Password"/> property changes.
        /// </summary>
        [Category("Property Changed"), Description("Event raised when the value of the Password property changes.")]
        public event EventHandler PasswordChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="CredentialDialog"/> class.
        /// </summary>
        public CredentialDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CredentialDialog"/> class with the specified container.
        /// </summary>
        /// <param name="container">The <see cref="IContainer"/> to add the component to.</param>
        public CredentialDialog(IContainer container)
        {
            container?.Add(this);

            InitializeComponent();
        }

        /// <summary>
        /// Gets or sets whether to use the application instance credential cache.
        /// </summary>
        /// <value>
        /// <see langword="true" /> when credentials are saved in the application instance cache; <see langref="false" /> if they are not.
        /// The default value is <see langword="false" />.
        /// </value>
        /// <remarks>
        /// <para>
        ///   The application instance credential cache stores credentials in memory while an application is running. When the
        ///   application exits, this cache is not persisted.
        /// </para>
        /// <para>
        ///   When the <see cref="UseApplicationInstanceCredentialCache"/> property is set to <see langword="true"/>, credentials that
        ///   are confirmed with <see cref="ConfirmCredentials"/> when the user checked the "save password" option will be stored
        ///   in the application instance cache as well as the operating system credential store.
        /// </para>
        /// <para>
        ///   When <see cref="ShowDialog()"/> is called, and credentials for the specified <see cref="Target"/> are already present in
        ///   the application instance cache, the dialog will not be shown and the cached credentials are returned, even if
        ///   <see cref="ShowUIForSavedCredentials"/> is <see langword="true"/>.
        /// </para>
        /// <para>
        ///   The application instance credential cache allows you to prevent prompting the user again for the lifetime of the
        ///   application if the "save password" checkbox was checked, but when the application is restarted you can prompt again
        ///   (initializing the dialog with the saved credentials). To get this behaviour, the <see cref="ShowUIForSavedCredentials"/>
        ///   property must be set to <see langword="true"/>.
        /// </para>
        /// </remarks>
        [Category("Behavior"), Description("Indicates whether to use the application instance credential cache."), DefaultValue(false)]
        public bool UseApplicationInstanceCredentialCache { get; set; }

        /// <summary>
        /// Gets or sets whether the "save password" checkbox is checked.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if the "save password" is checked; otherwise, <see langword="false" />.
        /// The default value is <see langword="false" />.
        /// </value>
        /// <remarks>
        /// The value of this property is only valid if the dialog box is displayed with a save checkbox.
        /// Set this property before showing the dialog to determine the initial checked value of the save checkbox.
        /// </remarks>
        [Category("Appearance"), Description("Indicates whether the \"Save password\" checkbox is checked."), DefaultValue(false)]
        public bool IsSaveChecked
        {
            get { return _isSaveChecked; }
            set
            {
                _confirmTarget = null;
                _isSaveChecked = value;
            }
        }

        /// <summary>
        /// Gets the password the user entered in the dialog.
        /// </summary>
        /// <value>
        /// The password entered in the password field of the credentials dialog.
        /// </value>
        [Browsable(false)]
        public string Password
        {
            get { return _credentials.Password; }
            private set
            {
                _confirmTarget = null;
                _credentials.Password = value;
                OnPasswordChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets the user-specified user name and password in a <see cref="NetworkCredential"/> object.
        /// </summary>
        /// <value>
        /// A <see cref="NetworkCredential"/> instance containing the user name and password specified on the dialog.
        /// </value>
        [Browsable(false)]
        public NetworkCredential Credentials
        {
            get { return _credentials; }
        }

        /// <summary>
        /// Gets the user name the user entered in the dialog.
        /// </summary>
        /// <value>
        /// The user name entered in the user name field of the credentials dialog.
        /// The default value is an empty string ("").
        /// </value>
        [Browsable(false)]
        public string UserName
        {
            get { return _credentials.UserName ?? string.Empty; }
            private set
            {
                _confirmTarget = null;
                _credentials.UserName = value;
                OnUserNameChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets or sets the target for the credentials, typically a server name.
        /// </summary>
        /// <value>
        /// The target for the credentials. The default value is an empty string ("").
        /// </value>
        /// <remarks>
        /// Credentials are stored on a per user, not on a per application basis. To ensure that credentials stored by different 
        /// applications do not conflict, you should prefix the target with an application-specific identifer, e.g. 
        /// "Company_Application_target".
        /// </remarks>
        [Category("Behavior"), Description("The target for the credentials, typically the server name prefixed by an application-specific identifier."), DefaultValue("")]
        public string Target
        {
            get { return _target ?? string.Empty; }
            set
            {
                _target = value;
                _confirmTarget = null;
            }
        }

        /// <summary>
        /// Gets or sets the title of the credentials dialog.
        /// </summary>
        /// <value>
        /// The title of the credentials dialog. The default value is an empty string ("").
        /// </value>
        /// <remarks>
        /// <para>
        ///   This property is not used on Windows Vista and newer versions of windows; the window title will always be "Windows Security"
        ///   in that case.
        /// </para>
        /// </remarks>
        [Localizable(true), Category("Appearance"), Description("The title of the credentials dialog."), DefaultValue("")]
        public string WindowTitle
        {
            get { return _windowTitle ?? string.Empty; }
            set { _windowTitle = value; }
        }

        /// <summary>
        /// Gets or sets a brief message to display in the dialog box.
        /// </summary>
        /// <value>
        /// A brief message that will be displayed in the dialog box. The default value is an empty string ("").
        /// </value>
        /// <remarks>
        /// <para>
        ///   On Windows Vista and newer versions of Windows, this text is displayed using a different style to set it apart
        ///   from the other text. In the default style, this text is a slightly larger and colored blue. The style is identical
        ///   to the main instruction of a task dialog.
        /// </para>
        /// <para>
        ///   On Windows XP, this text is not distinguished from other text. It's display mode depends on the <see cref="DownlevelTextMode"/>
        ///   property.
        /// </para>
        /// </remarks>
        [Localizable(true), Category("Appearance"), Description("A brief message that will be displayed in the dialog box."), DefaultValue("")]
        public string MainInstruction
        {
            get { return _caption ?? string.Empty; }
            set { _caption = value; }
        }

        /// <summary>
        /// Gets or sets additional text to display in the dialog.
        /// </summary>
        /// <value>
        /// Additional text to display in the dialog. The default value is an empty string ("").
        /// </value>
        /// <remarks>
        /// <para>
        ///   On Windows Vista and newer versions of Windows, this text is placed below the <see cref="MainInstruction"/> text.
        /// </para>
        /// <para>
        ///   On Windows XP, how and if this text is displayed depends on the value of the <see cref="DownlevelTextMode"/>
        ///   property.
        /// </para>
        /// </remarks>
        [Localizable(true), Category("Appearance"), Description("Additional text to display in the dialog."), DefaultValue("")]
        public string Content
        {
            get { return _text ?? string.Empty; }
            set { _text = value; }
        }

        /// <summary>
        /// Gets or sets a value that indicates how the text of the <see cref="MainInstruction"/> and <see cref="Content"/> properties
        /// is displayed on Windows XP.
        /// </summary>
        /// <value>
        /// One of the values of the <see cref="YouTrack.DownlevelTextMode"/> enumeration. The default value is
        /// <see cref="YouTrack.DownlevelTextMode.MainInstructionAndContent"/>.
        /// </value>
        /// <remarks>
        /// <para>
        ///   Windows XP does not support the distinct visual style of the main instruction, so there is no visual difference between the
        ///   text of the <see cref="CredentialDialog.MainInstruction"/> and <see cref="CredentialDialog.Content"/> properties. Depending
        ///   on your requirements, you may wish to hide either the main instruction or the content text.
        /// </para>
        /// <para>
        ///   This property has no effect on Windows Vista and newer versions of Windows.
        /// </para>
        /// </remarks>
        [Localizable(true), Category("Appearance"), Description("Indicates how the text of the MainInstruction and Content properties is displayed on Windows XP."), DefaultValue(DownlevelTextMode.MainInstructionAndContent)]
        public DownlevelTextMode DownlevelTextMode { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether a check box is shown on the dialog that allows the user to choose whether to save
        /// the credentials or not.
        /// </summary>
        /// <value>
        /// <see langword="true" /> when the "save password" checkbox is shown on the credentials dialog; otherwise, <see langword="false"/>.
        /// The default value is <see langword="false" />.
        /// </value>
        /// <remarks>
        /// When this property is set to <see langword="true" />, you must call the <see cref="ConfirmCredentials"/> method to save the
        /// credentials. When this property is set to <see langword="false" />, the credentials will never be saved, and you should not call
        /// the <see cref="ConfirmCredentials"/> method.
        /// </remarks>
        [Category("Appearance"), Description("Indicates whether a check box is shown on the dialog that allows the user to choose whether to save the credentials or not."), DefaultValue(false)]
        public bool ShowSaveCheckBox { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether the dialog should be displayed even when saved credentials exist for the 
        /// specified target.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if the dialog is displayed even when saved credentials exist; otherwise, <see langword="false" />.
        /// The default value is <see langword="false" />.
        /// </value>
        /// <remarks>
        /// <para>
        ///   This property applies only when the <see cref="ShowSaveCheckBox"/> property is <see langword="true" />.
        /// </para>
        /// <para>
        ///   Note that even if this property is <see langword="true" />, if the proper credentials exist in the 
        ///   application instance credentials cache the dialog will not be displayed.
        /// </para>
        /// </remarks>
        [Category("Behavior"), Description("Indicates whether the dialog should be displayed even when saved credentials exist for the specified target."), DefaultValue(false)]
        public bool ShowUIForSavedCredentials { get; set; }

        /// <summary>
        /// Gets a value that indicates whether the current credentials were retrieved from a credential store.
        /// </summary>
        /// <value>
        /// <see langword="true"/> if the current credentials returned by the <see cref="UserName"/>, <see cref="Password"/>,
        /// and <see cref="Credentials"/> properties were retrieved from either the application instance credential cache
        /// or the operating system's credential store; otherwise, <see langword="false"/>.
        /// </value>
        /// <remarks>
        /// <para>
        ///   You can use this property to determine if the credentials dialog was shown after a call to <see cref="ShowDialog()"/>.
        ///   If the dialog was shown, this property will be <see langword="false"/>; if the credentials were retrieved from the
        ///   application instance cache or the credential store and the dialog was not shown it will be <see langword="true"/>.
        /// </para>
        /// <para>
        ///   If the <see cref="ShowUIForSavedCredentials"/> property is set to <see langword="true"/>, and the dialog is shown
        ///   but populated with stored credentials, this property will still return <see langword="false"/>.
        /// </para>
        /// </remarks>
        public bool IsStoredCredential { get; private set; }


        /// <summary>
        /// Shows the credentials dialog as a modal dialog.
        /// </summary>
        /// <returns><see cref="DialogResult.OK" /> if the user clicked OK; otherwise, <see cref="DialogResult.Cancel" />.</returns>
        /// <remarks>
        /// <para>
        ///   The credentials dialog will not be shown if one of the following conditions holds:
        /// </para>
        /// <list type="bullet">
        ///   <item>
        ///     <description>
        ///       <see cref="UseApplicationInstanceCredentialCache"/> is <see langword="true"/> and the application instance
        ///       credential cache contains credentials for the specified <see cref="Target"/>, even if <see cref="ShowUIForSavedCredentials"/>
        ///       is <see langword="true"/>.
        ///     </description>
        ///   </item>
        ///   <item>
        ///     <description>
        ///       <see cref="ShowSaveCheckBox"/> is <see langword="true"/>, <see cref="ShowUIForSavedCredentials"/> is <see langword="false"/>, and the operating system credential store
        ///       for the current user contains credentials for the specified <see cref="Target"/>.
        ///     </description>
        ///   </item>
        /// </list>
        /// <para>
        ///   In these cases, the <see cref="Credentials"/>, <see cref="UserName"/> and <see cref="Password"/> properties will
        ///   be set to the saved credentials and this function returns immediately, returning <see cref="DialogResult.OK"/>.
        /// </para>
        /// <para>
        ///   If the <see cref="ShowSaveCheckBox"/> property is <see langword="true"/>, you should call <see cref="ConfirmCredentials"/>
        ///   after validating if the provided credentials are correct.
        /// </para>
        /// </remarks>
        /// <exception cref="Win32Exception">An error occurred while showing the credentials dialog.</exception>
        /// <exception cref="InvalidOperationException"><see cref="Target"/> is an empty string ("").</exception>
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        public DialogResult ShowDialog()
        {
            return ShowDialog(null);
        }

        /// <summary>
        /// Shows the credentials dialog as a modal dialog with the specified owner.
        /// </summary>
        /// <param name="owner">The <see cref="IWin32Window"/> that owns the credentials dialog.</param>
        /// <returns><see cref="DialogResult.OK" /> if the user clicked OK; otherwise, <see cref="DialogResult.Cancel" />.</returns>
        /// <remarks>
        /// <para>
        ///   The credentials dialog will not be shown if one of the following conditions holds:
        /// </para>
        /// <list type="bullet">
        ///   <item>
        ///     <description>
        ///       <see cref="UseApplicationInstanceCredentialCache"/> is <see langword="true"/> and the application instance
        ///       credential cache contains credentials for the specified <see cref="Target"/>, even if <see cref="ShowUIForSavedCredentials"/>
        ///       is <see langword="true"/>.
        ///     </description>
        ///   </item>
        ///   <item>
        ///     <description>
        ///       <see cref="ShowSaveCheckBox"/> is <see langword="true"/>, <see cref="ShowUIForSavedCredentials"/> is <see langword="false"/>, and the operating system credential store
        ///       for the current user contains credentials for the specified <see cref="Target"/>.
        ///     </description>
        ///   </item>
        /// </list>
        /// <para>
        ///   In these cases, the <see cref="Credentials"/>, <see cref="UserName"/> and <see cref="Password"/> properties will
        ///   be set to the saved credentials and this function returns immediately, returning <see cref="DialogResult.OK"/>.
        /// </para>
        /// <para>
        ///   If the <see cref="ShowSaveCheckBox"/> property is <see langword="true"/>, you should call <see cref="ConfirmCredentials"/>
        ///   after validating if the provided credentials are correct.
        /// </para>
        /// </remarks>
        /// <exception cref="Win32Exception">An error occurred while showing the credentials dialog.</exception>
        /// <exception cref="InvalidOperationException"><see cref="Target"/> is an empty string ("").</exception>
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        public DialogResult ShowDialog(IWin32Window owner)
        {
            if (string.IsNullOrEmpty(_target))
                throw new InvalidOperationException("Empty credential target");

            UserName = "";
            Password = "";
            IsStoredCredential = false;

            if (RetrieveCredentialsFromApplicationInstanceCache())
            {
                IsStoredCredential = true;
                _confirmTarget = Target;
                return DialogResult.OK;
            }

            bool storedCredentials = false;
            if (ShowSaveCheckBox && RetrieveCredentials())
            {
                IsSaveChecked = true;
                if (!ShowUIForSavedCredentials)
                {
                    IsStoredCredential = true;
                    _confirmTarget = Target;
                    return DialogResult.OK;
                }
                storedCredentials = true;
            }

            IntPtr ownerHandle = owner == null ? NativeMethods.GetActiveWindow() : owner.Handle;
            bool result;
            if (NativeMethods.IsWindowsVistaOrLater)
                result = PromptForCredentialsCredUIWin(ownerHandle, storedCredentials);
            else
                result = PromptForCredentialsCredUI(ownerHandle, storedCredentials);
            return result ? DialogResult.OK : DialogResult.Cancel;
        }

        /// <summary>
        /// Confirms the validity of the credential provided by the user.
        /// </summary>
        /// <param name="confirm"><see langword="true" /> if the credentials that were specified on the dialog are valid; otherwise, <see langword="false" />.</param>
        /// <remarks>
        /// Call this function after calling <see cref="ShowDialog()" /> when <see cref="ShowSaveCheckBox"/> is <see langword="true" />.
        /// Only when this function is called with <paramref name="confirm"/> set to <see langword="true" /> will the credentials be
        /// saved in the credentials store and/or the application instance credential cache.
        /// </remarks>
        /// <exception cref="InvalidOperationException"><see cref="ShowDialog()"/> was not called, or the user did not click OK, or <see cref="ShowSaveCheckBox"/> was <see langword="false" />
        /// at the call, or the value of <see cref="Target"/> or <see cref="IsSaveChecked"/>
        /// was changed after the call.</exception>
        /// <exception cref="Win32Exception">There was an error saving the credentials.</exception>
        public void ConfirmCredentials(bool confirm)
        {
            if (_confirmTarget == null || _confirmTarget != Target)
                throw new InvalidOperationException("Credential prompt not called");

            _confirmTarget = null;

            if (IsSaveChecked && confirm)
            {
                if (UseApplicationInstanceCredentialCache)
                {
                    lock (_applicationInstanceCredentialCache)
                    {
                        _applicationInstanceCredentialCache[Target] = new NetworkCredential(UserName, Password);
                    }
                }

                StoreCredential(Target, Credentials);
            }
        }

        /// <summary>
        /// Stores the specified credentials in the operating system's credential store for the currently logged on user.
        /// </summary>
        /// <param name="target">The target name for the credentials.</param>
        /// <param name="credential">The credentials to store.</param>
        /// <exception cref="ArgumentNullException">
        /// <para>
        ///   <paramref name="target"/> is <see langword="null" />.
        /// </para>
        /// <para>
        ///   -or-
        /// </para>
        /// <para>
        ///   <paramref name="credential"/> is <see langword="null" />.
        /// </para>
        /// </exception>
        /// <exception cref="ArgumentException"><paramref name="target"/> is an empty string ("").</exception>
        /// <exception cref="Win32Exception">An error occurred storing the credentials.</exception>
        /// <remarks>
        /// <note>
        ///   The <see cref="NetworkCredential.Domain"/> property is ignored and will not be stored, even if it is
        ///   not <see langword="null" />.
        /// </note>
        /// <para>
        ///   If the credential manager already contains credentials for the specified <paramref name="target"/>, they
        ///   will be overwritten; this can even overwrite credentials that were stored by another application. Therefore 
        ///   it is strongly recommended that you prefix the target name to ensure uniqueness, e.g. using the
        ///   form "Company_ApplicationName_www.example.com".
        /// </para>
        /// </remarks>
        public static void StoreCredential(string target, NetworkCredential credential)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));
            if (target.Length == 0)
                throw new ArgumentException("Empty credential target", nameof(target));
            if (credential == null)
                throw new ArgumentNullException(nameof(credential));

            NativeMethods.CREDENTIAL c = new NativeMethods.CREDENTIAL
            {
                UserName = credential.UserName,
                TargetName = target,
                Persist = NativeMethods.CredPersist.Enterprise
            };
            byte[] encryptedPassword = EncryptPassword(credential.Password);
            c.CredentialBlob = Marshal.AllocHGlobal(encryptedPassword.Length);
            try
            {
                Marshal.Copy(encryptedPassword, 0, c.CredentialBlob, encryptedPassword.Length);
                c.CredentialBlobSize = (uint)encryptedPassword.Length;
                c.Type = NativeMethods.CredTypes.CRED_TYPE_GENERIC;
                if (!NativeMethods.CredWrite(ref c, 0))
                    throw new Win32Exception(Marshal.GetLastWin32Error());
            }
            finally
            {
                Marshal.FreeCoTaskMem(c.CredentialBlob);
            }
        }

        /// <summary>
        /// Retrieves credentials for the specified target from the operating system's credential store for the current user.
        /// </summary>
        /// <param name="target">The target name for the credentials.</param>
        /// <returns>The credentials if they were found; otherwise, <see langword="null" />.</returns>
        /// <remarks>
        /// <para>
        ///   If the requested credential was not originally stored using the <see cref="CredentialDialog"/> class (but e.g. by 
        ///   another application), the password may not be decoded correctly.
        /// </para>
        /// <para>
        ///   This function does not check the application instance credential cache for the credentials; for that you can use
        ///   the <see cref="RetrieveCredentialFromApplicationInstanceCache"/> function.
        /// </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException"><paramref name="target"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException"><paramref name="target"/> is an empty string ("").</exception>
        /// <exception cref="Win32Exception">An error occurred retrieving the credentials.</exception>
        public static NetworkCredential RetrieveCredential(string target)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));
            if (target.Length == 0)
                throw new ArgumentException("Empty credential target", nameof(target));

            NetworkCredential cred = RetrieveCredentialFromApplicationInstanceCache(target);
            if (cred != null)
                return cred;

            bool result = NativeMethods.CredRead(target, NativeMethods.CredTypes.CRED_TYPE_GENERIC, 0, out var credential);
            int error = Marshal.GetLastWin32Error();
            if (result)
            {
                try
                {
                    NativeMethods.CREDENTIAL c = (NativeMethods.CREDENTIAL)Marshal.PtrToStructure(credential, typeof(NativeMethods.CREDENTIAL));
                    byte[] encryptedPassword = new byte[c.CredentialBlobSize];
                    Marshal.Copy(c.CredentialBlob, encryptedPassword, 0, encryptedPassword.Length);
                    cred = new NetworkCredential(c.UserName, DecryptPassword(encryptedPassword));
                }
                finally
                {
                    NativeMethods.CredFree(credential);
                }
                return cred;
            }
            if (error == (int)NativeMethods.CredUIReturnCodes.ERROR_NOT_FOUND)
                return null;
            throw new Win32Exception(error);
        }

        /// <summary>
        /// Tries to get the credentials for the specified target from the application instance credential cache.
        /// </summary>
        /// <param name="target">The target for the credentials, typically a server name.</param>
        /// <returns>The credentials that were found in the application instance cache; otherwise, <see langword="null" />.</returns>
        /// <remarks>
        /// <para>
        ///   This function will only check the the application instance credential cache; the operating system's credential store
        ///   is not checked. To retrieve credentials from the operating system's store, use <see cref="RetrieveCredential"/>.
        /// </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException"><paramref name="target"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException"><paramref name="target"/> is an empty string ("").</exception>
        public static NetworkCredential RetrieveCredentialFromApplicationInstanceCache(string target)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));
            if (target.Length == 0)
                throw new ArgumentException("Empty credential target", nameof(target));

            lock (_applicationInstanceCredentialCache)
            {
                if (_applicationInstanceCredentialCache.TryGetValue(target, out var cred))
                {
                    return cred;
                }
            }
            return null;
        }

        /// <summary>
        /// Deletes the credentials for the specified target.
        /// </summary>
        /// <param name="target">The name of the target for which to delete the credentials.</param>
        /// <returns><see langword="true"/> if the credential was deleted from either the application instance cache or
        /// the operating system's store; <see langword="false"/> if no credentials for the specified target could be found
        /// in either store.</returns>
        /// <remarks>
        /// <para>
        ///   The credentials for the specified target will be removed from the application instance credential cache
        ///   and the operating system's credential store.
        /// </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException"><paramref name="target"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException"><paramref name="target"/> is an empty string ("").</exception>
        /// <exception cref="Win32Exception">An error occurred deleting the credentials from the operating system's credential store.</exception>
        public static bool DeleteCredential(string target)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));
            if (target.Length == 0)
                throw new ArgumentException("Empty credential target", nameof(target));

            bool found;
            lock (_applicationInstanceCredentialCache)
            {
                found = _applicationInstanceCredentialCache.Remove(target);
            }

            if (NativeMethods.CredDelete(target, NativeMethods.CredTypes.CRED_TYPE_GENERIC, 0))
            {
                found = true;
            }
            else
            {
                int error = Marshal.GetLastWin32Error();
                if (error != (int)NativeMethods.CredUIReturnCodes.ERROR_NOT_FOUND)
                    throw new Win32Exception(error);
            }
            return found;
        }

        /// <summary>
        /// Raises the <see cref="UserNameChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs"/> containing data for the event.</param>
        protected virtual void OnUserNameChanged(EventArgs e)
        {
            UserNameChanged?.Invoke(this, e);
        }

        /// <summary>
        /// Raises the <see cref="PasswordChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs"/> containing data for the event.</param>
        protected virtual void OnPasswordChanged(EventArgs e)
        {
            PasswordChanged?.Invoke(this, e);
        }

        private bool PromptForCredentialsCredUI(IntPtr owner, bool storedCredentials)
        {
            NativeMethods.CREDUI_INFO info = CreateCredUIInfo(owner, true);
            NativeMethods.CREDUI_FLAGS flags = NativeMethods.CREDUI_FLAGS.GENERIC_CREDENTIALS | NativeMethods.CREDUI_FLAGS.DO_NOT_PERSIST | NativeMethods.CREDUI_FLAGS.ALWAYS_SHOW_UI;
            if (ShowSaveCheckBox)
                flags |= NativeMethods.CREDUI_FLAGS.SHOW_SAVE_CHECK_BOX;

            var user = new StringBuilder(NativeMethods.CREDUI_MAX_USERNAME_LENGTH);
            user.Append(UserName);
            var pw = new StringBuilder(NativeMethods.CREDUI_MAX_PASSWORD_LENGTH);
            pw.Append(Password);

            NativeMethods.CredUIReturnCodes result = NativeMethods.CredUIPromptForCredentials(ref info, Target, IntPtr.Zero, 0, user, NativeMethods.CREDUI_MAX_USERNAME_LENGTH, pw, NativeMethods.CREDUI_MAX_PASSWORD_LENGTH, ref _isSaveChecked, flags);
            switch (result)
            {
                case NativeMethods.CredUIReturnCodes.NO_ERROR:
                    UserName = user.ToString();
                    Password = pw.ToString();
                    if (ShowSaveCheckBox)
                    {
                        _confirmTarget = Target;
                        // If the credential was stored previously but the user has now cleared the save checkbox,
                        // we want to delete the credential.
                        if (storedCredentials && !IsSaveChecked)
                            DeleteCredential(Target);
                    }
                    return true;
                case NativeMethods.CredUIReturnCodes.ERROR_CANCELLED:
                    return false;
                default:
                    throw new Win32Exception((int)result);
            }
        }

        private bool PromptForCredentialsCredUIWin(IntPtr owner, bool storedCredentials)
        {
            NativeMethods.CREDUI_INFO info = CreateCredUIInfo(owner, false);
            NativeMethods.CredUIWinFlags flags = NativeMethods.CredUIWinFlags.Generic;
            if (ShowSaveCheckBox)
                flags |= NativeMethods.CredUIWinFlags.Checkbox;

            IntPtr inBuffer = IntPtr.Zero;
            IntPtr outBuffer = IntPtr.Zero;
            try
            {
                uint inBufferSize = 0;
                if (UserName.Length > 0)
                {
                    NativeMethods.CredPackAuthenticationBuffer(0, UserName, Password, IntPtr.Zero, ref inBufferSize);
                    if (inBufferSize > 0)
                    {
                        inBuffer = Marshal.AllocCoTaskMem((int)inBufferSize);
                        if (!NativeMethods.CredPackAuthenticationBuffer(0, UserName, Password, inBuffer, ref inBufferSize))
                            throw new Win32Exception(Marshal.GetLastWin32Error());
                    }
                }

                uint package = 0;
                NativeMethods.CredUIReturnCodes result = NativeMethods.CredUIPromptForWindowsCredentials(ref info, 0, ref package, inBuffer, inBufferSize, out outBuffer, out var outBufferSize, ref _isSaveChecked, flags);
                switch (result)
                {
                    case NativeMethods.CredUIReturnCodes.NO_ERROR:
                        StringBuilder userName = new StringBuilder(NativeMethods.CREDUI_MAX_USERNAME_LENGTH);
                        StringBuilder password = new StringBuilder(NativeMethods.CREDUI_MAX_PASSWORD_LENGTH);
                        uint userNameSize = (uint)userName.Capacity;
                        uint passwordSize = (uint)password.Capacity;
                        uint domainSize = 0;
                        if (!NativeMethods.CredUnPackAuthenticationBuffer(0, outBuffer, outBufferSize, userName, ref userNameSize, null, ref domainSize, password, ref passwordSize))
                            throw new Win32Exception(Marshal.GetLastWin32Error());
                        UserName = userName.ToString();
                        Password = password.ToString();
                        if (ShowSaveCheckBox)
                        {
                            _confirmTarget = Target;
                            // If the credential was stored previously but the user has now cleared the save checkbox,
                            // we want to delete the credential.
                            if (storedCredentials && !IsSaveChecked)
                                DeleteCredential(Target);
                        }
                        return true;
                    case NativeMethods.CredUIReturnCodes.ERROR_CANCELLED:
                        return false;
                    default:
                        throw new Win32Exception((int)result);
                }
            }
            finally
            {
                if (inBuffer != IntPtr.Zero)
                    Marshal.FreeCoTaskMem(inBuffer);
                if (outBuffer != IntPtr.Zero)
                    Marshal.FreeCoTaskMem(outBuffer);
            }
        }

        private NativeMethods.CREDUI_INFO CreateCredUIInfo(IntPtr owner, bool downlevelText)
        {
            NativeMethods.CREDUI_INFO info = new NativeMethods.CREDUI_INFO();
            info.cbSize = Marshal.SizeOf(info);
            info.hwndParent = owner;
            if (downlevelText)
            {
                info.pszCaptionText = WindowTitle;
                switch (DownlevelTextMode)
                {
                    case DownlevelTextMode.MainInstructionAndContent:
                        if (MainInstruction.Length == 0)
                            info.pszMessageText = Content;
                        else if (Content.Length == 0)
                            info.pszMessageText = MainInstruction;
                        else
                            info.pszMessageText = MainInstruction + Environment.NewLine + Environment.NewLine + Content;
                        break;
                    case DownlevelTextMode.MainInstructionOnly:
                        info.pszMessageText = MainInstruction;
                        break;
                    case DownlevelTextMode.ContentOnly:
                        info.pszMessageText = Content;
                        break;
                }
            }
            else
            {
                // Vista and later don't use the window title.
                info.pszMessageText = Content;
                info.pszCaptionText = MainInstruction;
            }
            return info;
        }

        private bool RetrieveCredentials()
        {
            var credential = RetrieveCredential(Target);
            if (credential != null)
            {
                UserName = credential.UserName;
                Password = credential.Password;
                return true;
            }
            return false;
        }

        private static byte[] EncryptPassword(string password)
        {
            byte[] protectedData = ProtectedData.Protect(Encoding.UTF8.GetBytes(password), null, DataProtectionScope.CurrentUser);
            return protectedData;
        }

        private static string DecryptPassword(byte[] encrypted)
        {
            try
            {
                return Encoding.UTF8.GetString(ProtectedData.Unprotect(encrypted, null, DataProtectionScope.CurrentUser));
            }
            catch (CryptographicException)
            {
                return string.Empty;
            }
        }

        private bool RetrieveCredentialsFromApplicationInstanceCache()
        {
            if (UseApplicationInstanceCredentialCache)
            {
                var credential = RetrieveCredentialFromApplicationInstanceCache(Target);
                if (credential != null)
                {
                    UserName = credential.UserName;
                    Password = credential.Password;
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing"><see langword="true"/> if managed resources should be disposed; otherwise, <see langword="false"/>.</param>
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                {
                    components?.Dispose();
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
        }

        #endregion
    }
}
