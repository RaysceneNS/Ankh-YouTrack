namespace Ankh.YouTrack.IssueTracker.Forms
{
	partial class ConfigurationPage
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.Label urlLabel;
            this.textBoxRepositoryUri = new System.Windows.Forms.TextBox();
            this.buttonChooseProject = new System.Windows.Forms.Button();
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxProjectID = new System.Windows.Forms.TextBox();
            this.buttonTest = new System.Windows.Forms.Button();
            urlLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            this.SuspendLayout();
            // 
            // urlLabel
            // 
            urlLabel.Location = new System.Drawing.Point(4, 11);
            urlLabel.Name = "urlLabel";
            urlLabel.Size = new System.Drawing.Size(95, 20);
            urlLabel.TabIndex = 0;
            urlLabel.Text = "YouTrack URL";
            urlLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // textBoxRepositoryUri
            // 
            this.textBoxRepositoryUri.Location = new System.Drawing.Point(105, 12);
            this.textBoxRepositoryUri.Name = "textBoxRepositoryUri";
            this.textBoxRepositoryUri.Size = new System.Drawing.Size(206, 20);
            this.textBoxRepositoryUri.TabIndex = 1;
            this.textBoxRepositoryUri.Validating += new System.ComponentModel.CancelEventHandler(this.TextBoxRepositoryUri_Validating);
            // 
            // buttonChooseProject
            // 
            this.buttonChooseProject.Location = new System.Drawing.Point(317, 46);
            this.buttonChooseProject.Name = "buttonChooseProject";
            this.buttonChooseProject.Size = new System.Drawing.Size(79, 23);
            this.buttonChooseProject.TabIndex = 6;
            this.buttonChooseProject.Text = "Choose...";
            this.buttonChooseProject.UseVisualStyleBackColor = true;
            this.buttonChooseProject.Click += new System.EventHandler(this.ButtonChooseProject_Click);
            // 
            // errorProvider
            // 
            this.errorProvider.ContainerControl = this;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(45, 51);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(54, 13);
            this.label1.TabIndex = 11;
            this.label1.Text = "Project ID";
            // 
            // textBoxProjectID
            // 
            this.textBoxProjectID.Location = new System.Drawing.Point(105, 46);
            this.textBoxProjectID.Name = "textBoxProjectID";
            this.textBoxProjectID.ReadOnly = true;
            this.textBoxProjectID.Size = new System.Drawing.Size(206, 20);
            this.textBoxProjectID.TabIndex = 13;
            // 
            // buttonTest
            // 
            this.buttonTest.Location = new System.Drawing.Point(317, 11);
            this.buttonTest.Name = "buttonTest";
            this.buttonTest.Size = new System.Drawing.Size(79, 23);
            this.buttonTest.TabIndex = 14;
            this.buttonTest.Text = "Test";
            this.buttonTest.UseVisualStyleBackColor = true;
            this.buttonTest.Click += new System.EventHandler(this.ButtonTest_Click);
            // 
            // ConfigurationPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoValidate = System.Windows.Forms.AutoValidate.EnableAllowFocusChange;
            this.Controls.Add(this.buttonTest);
            this.Controls.Add(this.textBoxProjectID);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonChooseProject);
            this.Controls.Add(this.textBoxRepositoryUri);
            this.Controls.Add(urlLabel);
            this.Name = "ConfigurationPage";
            this.Size = new System.Drawing.Size(413, 108);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

        private System.Windows.Forms.TextBox textBoxRepositoryUri;
		private System.Windows.Forms.Button buttonChooseProject;
        private System.Windows.Forms.ErrorProvider errorProvider;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxProjectID;
        private System.Windows.Forms.Button buttonTest;
    }
}
