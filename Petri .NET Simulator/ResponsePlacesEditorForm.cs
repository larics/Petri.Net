using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace PetriNetSimulator2
{
	/// <summary>
	/// Summary description for ResponsePlacesEditorForm.
	/// </summary>
	public class ResponsePlacesEditorForm : System.Windows.Forms.Form
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.Panel pnlBottom;
		private System.Windows.Forms.Panel pnlSeparator;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.GroupBox gbAll;
		private System.Windows.Forms.GroupBox gbSelectedPlaces;
		private System.Windows.Forms.Button btnAddAll;
		private System.Windows.Forms.ColumnHeader ColumnName;
		private System.Windows.Forms.ColumnHeader ColumnID;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private System.Windows.Forms.ListView lvAllPlaces;
		private System.Windows.Forms.ListView lvSelectedPlaces;
		private System.Windows.Forms.Button btnRemoveAll;
		private System.Windows.Forms.Button btnAddOne;
		private System.Windows.Forms.Button btnRemoveOne;
		private System.Windows.Forms.Button btnDown;
		private System.Windows.Forms.Button btnUp;

		// Properties
		public Place[] Places
		{
			get
			{
				return this.paPlaces;
			}
		}

		// Fields
		private Place[] paPlaces;
		private PetriNetDocument pnd;

		public ResponsePlacesEditorForm(PetriNetDocument pnd, Place[] pa)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			this.paPlaces = pa;
			this.pnd = pnd;
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.pnlBottom = new System.Windows.Forms.Panel();
			this.pnlSeparator = new System.Windows.Forms.Panel();
			this.btnCancel = new System.Windows.Forms.Button();
			this.btnOK = new System.Windows.Forms.Button();
			this.gbAll = new System.Windows.Forms.GroupBox();
			this.lvAllPlaces = new System.Windows.Forms.ListView();
			this.ColumnID = new System.Windows.Forms.ColumnHeader();
			this.ColumnName = new System.Windows.Forms.ColumnHeader();
			this.gbSelectedPlaces = new System.Windows.Forms.GroupBox();
			this.lvSelectedPlaces = new System.Windows.Forms.ListView();
			this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
			this.btnAddAll = new System.Windows.Forms.Button();
			this.btnRemoveAll = new System.Windows.Forms.Button();
			this.btnAddOne = new System.Windows.Forms.Button();
			this.btnRemoveOne = new System.Windows.Forms.Button();
			this.btnDown = new System.Windows.Forms.Button();
			this.btnUp = new System.Windows.Forms.Button();
			this.pnlBottom.SuspendLayout();
			this.gbAll.SuspendLayout();
			this.gbSelectedPlaces.SuspendLayout();
			this.SuspendLayout();
			// 
			// pnlBottom
			// 
			this.pnlBottom.Controls.Add(this.pnlSeparator);
			this.pnlBottom.Controls.Add(this.btnCancel);
			this.pnlBottom.Controls.Add(this.btnOK);
			this.pnlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.pnlBottom.Location = new System.Drawing.Point(0, 398);
			this.pnlBottom.Name = "pnlBottom";
			this.pnlBottom.Size = new System.Drawing.Size(458, 56);
			this.pnlBottom.TabIndex = 4;
			// 
			// pnlSeparator
			// 
			this.pnlSeparator.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.pnlSeparator.Dock = System.Windows.Forms.DockStyle.Top;
			this.pnlSeparator.Location = new System.Drawing.Point(0, 0);
			this.pnlSeparator.Name = "pnlSeparator";
			this.pnlSeparator.Size = new System.Drawing.Size(458, 4);
			this.pnlSeparator.TabIndex = 3;
			this.pnlSeparator.Visible = false;
			// 
			// btnCancel
			// 
			this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnCancel.Location = new System.Drawing.Point(344, 16);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(100, 28);
			this.btnCancel.TabIndex = 2;
			this.btnCancel.Text = "&Cancel";
			// 
			// btnOK
			// 
			this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOK.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnOK.Location = new System.Drawing.Point(232, 16);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(100, 28);
			this.btnOK.TabIndex = 1;
			this.btnOK.Text = "&OK";
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// gbAll
			// 
			this.gbAll.Controls.Add(this.lvAllPlaces);
			this.gbAll.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.gbAll.Location = new System.Drawing.Point(16, 16);
			this.gbAll.Name = "gbAll";
			this.gbAll.Size = new System.Drawing.Size(176, 368);
			this.gbAll.TabIndex = 6;
			this.gbAll.TabStop = false;
			this.gbAll.Text = "All places : ";
			// 
			// lvAllPlaces
			// 
			this.lvAllPlaces.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
																						  this.ColumnID,
																						  this.ColumnName});
			this.lvAllPlaces.FullRowSelect = true;
			this.lvAllPlaces.GridLines = true;
			this.lvAllPlaces.Location = new System.Drawing.Point(16, 32);
			this.lvAllPlaces.Name = "lvAllPlaces";
			this.lvAllPlaces.Size = new System.Drawing.Size(144, 320);
			this.lvAllPlaces.TabIndex = 0;
			this.lvAllPlaces.View = System.Windows.Forms.View.Details;
			this.lvAllPlaces.SelectedIndexChanged += new System.EventHandler(this.lvAllPlaces_SelectedIndexChanged);
			// 
			// ColumnID
			// 
			this.ColumnID.Text = "ID";
			this.ColumnID.Width = 40;
			// 
			// ColumnName
			// 
			this.ColumnName.Text = "NameID";
			this.ColumnName.Width = 80;
			// 
			// gbSelectedPlaces
			// 
			this.gbSelectedPlaces.Controls.Add(this.lvSelectedPlaces);
			this.gbSelectedPlaces.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.gbSelectedPlaces.Location = new System.Drawing.Point(264, 16);
			this.gbSelectedPlaces.Name = "gbSelectedPlaces";
			this.gbSelectedPlaces.Size = new System.Drawing.Size(176, 368);
			this.gbSelectedPlaces.TabIndex = 7;
			this.gbSelectedPlaces.TabStop = false;
			this.gbSelectedPlaces.Text = "Selected places : ";
			// 
			// lvSelectedPlaces
			// 
			this.lvSelectedPlaces.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
																							   this.columnHeader1,
																							   this.columnHeader2});
			this.lvSelectedPlaces.FullRowSelect = true;
			this.lvSelectedPlaces.GridLines = true;
			this.lvSelectedPlaces.Location = new System.Drawing.Point(16, 32);
			this.lvSelectedPlaces.Name = "lvSelectedPlaces";
			this.lvSelectedPlaces.Size = new System.Drawing.Size(144, 320);
			this.lvSelectedPlaces.TabIndex = 1;
			this.lvSelectedPlaces.View = System.Windows.Forms.View.Details;
			this.lvSelectedPlaces.SelectedIndexChanged += new System.EventHandler(this.lvSelectedPlaces_SelectedIndexChanged);
			// 
			// columnHeader1
			// 
			this.columnHeader1.Text = "ID";
			this.columnHeader1.Width = 40;
			// 
			// columnHeader2
			// 
			this.columnHeader2.Text = "NameID";
			this.columnHeader2.Width = 80;
			// 
			// btnAddAll
			// 
			this.btnAddAll.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnAddAll.Location = new System.Drawing.Point(212, 184);
			this.btnAddAll.Name = "btnAddAll";
			this.btnAddAll.Size = new System.Drawing.Size(32, 32);
			this.btnAddAll.TabIndex = 8;
			this.btnAddAll.Text = ">>";
			this.btnAddAll.Click += new System.EventHandler(this.btnAddAll_Click);
			// 
			// btnRemoveAll
			// 
			this.btnRemoveAll.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnRemoveAll.Location = new System.Drawing.Point(212, 304);
			this.btnRemoveAll.Name = "btnRemoveAll";
			this.btnRemoveAll.Size = new System.Drawing.Size(32, 32);
			this.btnRemoveAll.TabIndex = 9;
			this.btnRemoveAll.Text = "<<";
			this.btnRemoveAll.Click += new System.EventHandler(this.btnRemoveAll_Click);
			// 
			// btnAddOne
			// 
			this.btnAddOne.Enabled = false;
			this.btnAddOne.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnAddOne.Location = new System.Drawing.Point(212, 224);
			this.btnAddOne.Name = "btnAddOne";
			this.btnAddOne.Size = new System.Drawing.Size(32, 32);
			this.btnAddOne.TabIndex = 10;
			this.btnAddOne.Text = ">";
			this.btnAddOne.Click += new System.EventHandler(this.btnAddOne_Click);
			// 
			// btnRemoveOne
			// 
			this.btnRemoveOne.Enabled = false;
			this.btnRemoveOne.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnRemoveOne.Location = new System.Drawing.Point(212, 264);
			this.btnRemoveOne.Name = "btnRemoveOne";
			this.btnRemoveOne.Size = new System.Drawing.Size(32, 32);
			this.btnRemoveOne.TabIndex = 11;
			this.btnRemoveOne.Text = "<";
			this.btnRemoveOne.Click += new System.EventHandler(this.btnRemoveOne_Click);
			// 
			// btnDown
			// 
			this.btnDown.Enabled = false;
			this.btnDown.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnDown.Location = new System.Drawing.Point(200, 112);
			this.btnDown.Name = "btnDown";
			this.btnDown.Size = new System.Drawing.Size(56, 32);
			this.btnDown.TabIndex = 13;
			this.btnDown.Text = "Down";
			this.btnDown.Click += new System.EventHandler(this.btnDown_Click);
			// 
			// btnUp
			// 
			this.btnUp.Enabled = false;
			this.btnUp.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnUp.Location = new System.Drawing.Point(200, 72);
			this.btnUp.Name = "btnUp";
			this.btnUp.Size = new System.Drawing.Size(56, 32);
			this.btnUp.TabIndex = 12;
			this.btnUp.Text = "Up";
			this.btnUp.Click += new System.EventHandler(this.btnUp_Click);
			// 
			// ResponsePlacesEditorForm
			// 
			this.AcceptButton = this.btnOK;
			this.AutoScale = false;
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(458, 454);
			this.Controls.Add(this.btnDown);
			this.Controls.Add(this.btnUp);
			this.Controls.Add(this.btnRemoveOne);
			this.Controls.Add(this.btnAddOne);
			this.Controls.Add(this.btnRemoveAll);
			this.Controls.Add(this.btnAddAll);
			this.Controls.Add(this.gbSelectedPlaces);
			this.Controls.Add(this.gbAll);
			this.Controls.Add(this.pnlBottom);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "ResponsePlacesEditorForm";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Response Places Editor";
			this.Load += new System.EventHandler(this.ResponsePlacesEditorForm_Load);
			this.pnlBottom.ResumeLayout(false);
			this.gbAll.ResumeLayout(false);
			this.gbSelectedPlaces.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion


		#region private void ResponsePlacesEditorForm_Load(object sender, System.EventArgs e)
		private void ResponsePlacesEditorForm_Load(object sender, System.EventArgs e)
		{
			foreach(Place p in this.pnd.GroupedPlaces)
			{
				// Check if place p already exists in this.paPlaces
				bool bFound = false;
				for (int k = 0; k < this.paPlaces.Length; k++)
				{
					if (p == this.paPlaces[k])
					{
						bFound = true;
						break;
					}
				}

				// if not, add it to lvAllPlaces
				if (bFound != true)
				{
					ListViewItem lvi = new ListViewItem("P" + p.Index);
					lvi.SubItems.Add(p.NameID);

					lvi.Tag = p;

					lvAllPlaces.Items.Add(lvi);
				}
			}

			for (int i = 0; i < this.paPlaces.Length; i++)
			{
				Place p = this.paPlaces[i];

				if (this.pnd.Places.Contains(p))
				{
					ListViewItem lvi = new ListViewItem("P" + p.Index);
					lvi.SubItems.Add(p.NameID);

					lvi.Tag = p;

					lvSelectedPlaces.Items.Add(lvi);
				}
			}
		}
		#endregion


		#region private void btnAddAll_Click(object sender, System.EventArgs e)
		private void btnAddAll_Click(object sender, System.EventArgs e)
		{
			foreach(ListViewItem lvi in lvAllPlaces.Items)
			{
				lvAllPlaces.Items.Remove(lvi);
				lvSelectedPlaces.Items.Add(lvi);
			}
		}
		#endregion

		#region private void btnAddOne_Click(object sender, System.EventArgs e)
		private void btnAddOne_Click(object sender, System.EventArgs e)
		{
			foreach(ListViewItem lvi in lvAllPlaces.SelectedItems)
			{
				lvAllPlaces.Items.Remove(lvi);
				lvSelectedPlaces.Items.Add(lvi);
			}

			lvSelectedPlaces.SelectedItems.Clear();
		}
		#endregion

		#region private void lvAllPlaces_SelectedIndexChanged(object sender, System.EventArgs e)
		private void lvAllPlaces_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (lvAllPlaces.SelectedItems.Count != 0)
			{
				btnAddOne.Enabled = true;
			}
			else
			{
				btnAddOne.Enabled = false;
				
				foreach(ListViewItem lvi in lvAllPlaces.Items)
				{
					lvi.Focused = false;
				}
			}
		}
		#endregion

		#region private void btnRemoveAll_Click(object sender, System.EventArgs e)
		private void btnRemoveAll_Click(object sender, System.EventArgs e)
		{
			foreach(ListViewItem lvi in lvSelectedPlaces.Items)
			{
				lvSelectedPlaces.Items.Remove(lvi);
				lvAllPlaces.Items.Add(lvi);
			}
		}
		#endregion

		#region private void btnRemoveOne_Click(object sender, System.EventArgs e)
		private void btnRemoveOne_Click(object sender, System.EventArgs e)
		{
			foreach(ListViewItem lvi in lvSelectedPlaces.SelectedItems)
			{
				lvSelectedPlaces.Items.Remove(lvi);
				lvAllPlaces.Items.Add(lvi);
			}

			lvAllPlaces.SelectedItems.Clear();
		}
		#endregion

		#region private void lvSelectedPlaces_SelectedIndexChanged(object sender, System.EventArgs e)
		private void lvSelectedPlaces_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (lvSelectedPlaces.SelectedItems.Count != 0)
			{
				btnRemoveOne.Enabled = true;

				if (lvSelectedPlaces.SelectedItems[0].Index != 0)
				{
					btnUp.Enabled = true;
				}
				else
				{
					btnUp.Enabled = false;
				}

				if (lvSelectedPlaces.SelectedItems[lvSelectedPlaces.SelectedItems.Count - 1].Index != lvSelectedPlaces.Items.Count - 1)
				{
					btnDown.Enabled = true;
				}
				else
				{
					btnDown.Enabled = false;
				}

			}
			else
			{
				btnUp.Enabled = false;
				btnDown.Enabled = false;

				btnRemoveOne.Enabled = false;

				foreach(ListViewItem lvi in lvSelectedPlaces.Items)
				{
					lvi.Focused = false;
				}
			}
		}
		#endregion

		private void btnOK_Click(object sender, System.EventArgs e)
		{
			this.paPlaces = new Place[lvSelectedPlaces.Items.Count];

			for (int i = 0; i < lvSelectedPlaces.Items.Count; i++)
			{
				paPlaces[i] = (Place)lvSelectedPlaces.Items[i].Tag;
			}
		}

		#region private void btnUp_Click(object sender, System.EventArgs e)
		private void btnUp_Click(object sender, System.EventArgs e)
		{
			int[] ia = new int[lvSelectedPlaces.SelectedItems.Count];

			for(int i = 0; i < lvSelectedPlaces.SelectedItems.Count; i++)
			{
				ia[i] = lvSelectedPlaces.SelectedItems[i].Index;
			}

			for(int i = 0; i < ia.Length; i++)
			{
				ListViewItem lvi = lvSelectedPlaces.Items[ia[i]];
				lvSelectedPlaces.Items.Remove(lvi);
				lvSelectedPlaces.Items.Insert(ia[i] - 1, lvi);
			}

			lvSelectedPlaces.SelectedItems.Clear();

			for(int i = 0; i < ia.Length; i++)
			{
				lvSelectedPlaces.Items[ia[i] - 1].Selected = true;
			}

			lvSelectedPlaces.Focus();

			foreach(ListViewItem lvi in lvSelectedPlaces.Items)
			{
				lvi.Focused = false;
			}
		}
		#endregion

		#region private void btnDown_Click(object sender, System.EventArgs e)
		private void btnDown_Click(object sender, System.EventArgs e)
		{
			int[] ia = new int[lvSelectedPlaces.SelectedItems.Count];

			for(int i = 0; i < lvSelectedPlaces.SelectedItems.Count; i++)
			{
				ia[i] = lvSelectedPlaces.SelectedItems[i].Index;
			}

			for(int i = 0; i < ia.Length; i++)
			{
				ListViewItem lvi = lvSelectedPlaces.Items[ia[i]];
				lvSelectedPlaces.Items.Remove(lvi);
				lvSelectedPlaces.Items.Insert(ia[i] + 1, lvi);
			}

			lvSelectedPlaces.SelectedItems.Clear();

			for(int i = 0; i < ia.Length; i++)
			{
				lvSelectedPlaces.Items[ia[i] + 1].Selected = true;
			}

			lvSelectedPlaces.Focus();

			foreach(ListViewItem lvi in lvSelectedPlaces.Items)
			{
				lvi.Focused = false;
			}
		}
		#endregion

	}
}
