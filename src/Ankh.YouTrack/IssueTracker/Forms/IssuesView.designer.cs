namespace Ankh.YouTrack.IssueTracker.Forms
{
	partial class IssuesView
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
            this.btnSearch = new System.Windows.Forms.Button();
            this.pnlButttons = new System.Windows.Forms.Panel();
            this.textQuery = new System.Windows.Forms.TextBox();
            this.dgvList = new System.Windows.Forms.DataGridView();
            this.colSelected = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.columnId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnPriority = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnIssue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnIssueType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnAuthor = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnAssignedTo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn7 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.pnlButttons.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvList)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            this.SuspendLayout();
            // 
            // btnSearch
            // 
            this.btnSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSearch.Location = new System.Drawing.Point(697, 6);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(75, 23);
            this.btnSearch.TabIndex = 1;
            this.btnSearch.Text = "Search";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.BtnSearchClick);
            // 
            // pnlButttons
            // 
            this.pnlButttons.Controls.Add(this.textQuery);
            this.pnlButttons.Controls.Add(this.btnSearch);
            this.pnlButttons.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlButttons.Location = new System.Drawing.Point(0, 0);
            this.pnlButttons.Name = "pnlButttons";
            this.pnlButttons.Size = new System.Drawing.Size(775, 39);
            this.pnlButttons.TabIndex = 2;
            // 
            // textQuery
            // 
            this.textQuery.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textQuery.Location = new System.Drawing.Point(9, 8);
            this.textQuery.Name = "textQuery";
            this.textQuery.Size = new System.Drawing.Size(680, 20);
            this.textQuery.TabIndex = 4;
            // 
            // dgvList
            // 
            this.dgvList.AllowUserToAddRows = false;
            this.dgvList.AllowUserToDeleteRows = false;
            this.dgvList.AllowUserToResizeRows = false;
            this.dgvList.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dgvList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colSelected,
            this.columnId,
            this.columnPriority,
            this.columnIssue,
            this.columnStatus,
            this.columnIssueType,
            this.columnAuthor,
            this.columnAssignedTo});
            this.dgvList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvList.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.dgvList.Location = new System.Drawing.Point(0, 39);
            this.dgvList.MultiSelect = false;
            this.dgvList.Name = "dgvList";
            this.dgvList.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvList.Size = new System.Drawing.Size(775, 337);
            this.dgvList.TabIndex = 3;
            this.dgvList.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvList_CellContentClick);
            this.dgvList.CurrentCellDirtyStateChanged += new System.EventHandler(this.DgvListCurrentCellDirtyStateChanged);
            // 
            // colSelected
            // 
            this.colSelected.HeaderText = "";
            this.colSelected.Name = "colSelected";
            // 
            // columnId
            // 
            this.columnId.DataPropertyName = "Id";
            this.columnId.HeaderText = "Id";
            this.columnId.MinimumWidth = 50;
            this.columnId.Name = "columnId";
            this.columnId.ReadOnly = true;
            this.columnId.Width = 50;
            // 
            // columnPriority
            // 
            this.columnPriority.DataPropertyName = "Priority";
            this.columnPriority.HeaderText = "Priority";
            this.columnPriority.Name = "columnPriority";
            this.columnPriority.ReadOnly = true;
            this.columnPriority.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // columnIssue
            // 
            this.columnIssue.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.columnIssue.DataPropertyName = "Summary";
            this.columnIssue.HeaderText = "Issue";
            this.columnIssue.Name = "columnIssue";
            this.columnIssue.ReadOnly = true;
            // 
            // columnStatus
            // 
            this.columnStatus.DataPropertyName = "State";
            this.columnStatus.HeaderText = "State";
            this.columnStatus.Name = "columnStatus";
            this.columnStatus.ReadOnly = true;
            this.columnStatus.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // columnIssueType
            // 
            this.columnIssueType.DataPropertyName = "Type";
            this.columnIssueType.HeaderText = "Type";
            this.columnIssueType.Name = "columnIssueType";
            this.columnIssueType.ReadOnly = true;
            this.columnIssueType.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // columnAuthor
            // 
            this.columnAuthor.DataPropertyName = "ReporterName";
            this.columnAuthor.HeaderText = "Reporter";
            this.columnAuthor.Name = "columnAuthor";
            this.columnAuthor.ReadOnly = true;
            // 
            // columnAssignedTo
            // 
            this.columnAssignedTo.DataPropertyName = "AssigneeName";
            this.columnAssignedTo.HeaderText = "Assigned to";
            this.columnAssignedTo.Name = "columnAssignedTo";
            this.columnAssignedTo.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.DataPropertyName = "ShortName";
            this.dataGridViewTextBoxColumn1.HeaderText = "ShortName";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.DataPropertyName = "Project";
            this.dataGridViewTextBoxColumn2.HeaderText = "Project";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            // 
            // dataGridViewTextBoxColumn3
            // 
            this.dataGridViewTextBoxColumn3.DataPropertyName = "Summary";
            this.dataGridViewTextBoxColumn3.HeaderText = "Issue";
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            this.dataGridViewTextBoxColumn3.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn4
            // 
            this.dataGridViewTextBoxColumn4.DataPropertyName = "ReporterName";
            this.dataGridViewTextBoxColumn4.HeaderText = "ReporterName";
            this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            this.dataGridViewTextBoxColumn4.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn5
            // 
            this.dataGridViewTextBoxColumn5.DataPropertyName = "AssigneeName";
            this.dataGridViewTextBoxColumn5.HeaderText = "Assigned to";
            this.dataGridViewTextBoxColumn5.Name = "dataGridViewTextBoxColumn5";
            this.dataGridViewTextBoxColumn5.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn6
            // 
            this.dataGridViewTextBoxColumn6.DataPropertyName = "State";
            this.dataGridViewTextBoxColumn6.HeaderText = "State";
            this.dataGridViewTextBoxColumn6.Name = "dataGridViewTextBoxColumn6";
            this.dataGridViewTextBoxColumn6.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn7
            // 
            this.dataGridViewTextBoxColumn7.DataPropertyName = "Priority";
            this.dataGridViewTextBoxColumn7.HeaderText = "Priority";
            this.dataGridViewTextBoxColumn7.Name = "dataGridViewTextBoxColumn7";
            this.dataGridViewTextBoxColumn7.ReadOnly = true;
            // 
            // errorProvider
            // 
            this.errorProvider.ContainerControl = this;
            // 
            // IssuesView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.dgvList);
            this.Controls.Add(this.pnlButttons);
            this.Name = "IssuesView";
            this.Size = new System.Drawing.Size(775, 376);
            this.Load += new System.EventHandler(this.IssuesListViewLoad);
            this.pnlButttons.ResumeLayout(false);
            this.pnlButttons.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvList)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            this.ResumeLayout(false);

		}

		#endregion

        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.Panel pnlButttons;
        private System.Windows.Forms.DataGridView dgvList;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn5;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn6;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn7;
        private System.Windows.Forms.TextBox textQuery;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colSelected;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnId;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnPriority;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnIssue;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnStatus;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnIssueType;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnAuthor;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnAssignedTo;
        private System.Windows.Forms.ErrorProvider errorProvider;
	}
}
