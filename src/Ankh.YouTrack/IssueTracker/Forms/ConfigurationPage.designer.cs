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
            this.textRepositoryUri = new System.Windows.Forms.TextBox();
            this.btnLoadProjects = new System.Windows.Forms.Button();
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.cboProjects = new System.Windows.Forms.ComboBox();
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
            // textRepositoryUri
            // 
            this.textRepositoryUri.Location = new System.Drawing.Point(105, 12);
            this.textRepositoryUri.Name = "textRepositoryUri";
            this.textRepositoryUri.Size = new System.Drawing.Size(261, 20);
            this.textRepositoryUri.TabIndex = 1;
            this.textRepositoryUri.Validating += new System.ComponentModel.CancelEventHandler(this.TextUrl_Validating);
            // 
            // btnLoadProjects
            // 
            this.btnLoadProjects.Location = new System.Drawing.Point(105, 36);
            this.btnLoadProjects.Name = "btnLoadProjects";
            this.btnLoadProjects.Size = new System.Drawing.Size(54, 23);
            this.btnLoadProjects.TabIndex = 6;
            this.btnLoadProjects.Text = "Load";
            this.btnLoadProjects.UseVisualStyleBackColor = true;
            this.btnLoadProjects.Click += new System.EventHandler(this.ButtonLoadProjects_Click);
            // 
            // errorProvider
            // 
            this.errorProvider.ContainerControl = this;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(59, 41);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(40, 13);
            this.label1.TabIndex = 11;
            this.label1.Text = "Project";
            // 
            // cboProjects
            // 
            this.cboProjects.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboProjects.FormattingEnabled = true;
            this.cboProjects.Location = new System.Drawing.Point(165, 38);
            this.cboProjects.Name = "cboProjects";
            this.cboProjects.Size = new System.Drawing.Size(200, 21);
            this.cboProjects.TabIndex = 12;
            // 
            // ConfigurationPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoValidate = System.Windows.Forms.AutoValidate.EnableAllowFocusChange;
            this.Controls.Add(this.cboProjects);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnLoadProjects);
            this.Controls.Add(this.textRepositoryUri);
            this.Controls.Add(urlLabel);
            this.Name = "ConfigurationPage";
            this.Size = new System.Drawing.Size(413, 79);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

        private System.Windows.Forms.TextBox textRepositoryUri;
		private System.Windows.Forms.Button btnLoadProjects;
        private System.Windows.Forms.ErrorProvider errorProvider;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cboProjects;
	}
}
