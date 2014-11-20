using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace PetriNetSimulator2
{
	/// <summary>
	/// Summary description for ReleaseTimesEditorForm.
	/// </summary>
	public class ReleaseTimesEditorForm : System.Windows.Forms.Form
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		
		// Properties
		public ReleaseTime[] ReleaseTimes
		{
			get
			{
				return this.rta;
			}
		}

		// Fields
		PlaceResource prOwner;
		ReleaseTime[] rta;
		private System.Windows.Forms.Panel pnlBottom;
		private System.Windows.Forms.Panel pnlSeparator;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.GroupBox gbTimes;
		private System.Windows.Forms.ListView lvOperationsPlaces;
		private System.Windows.Forms.ColumnHeader ColumnPlace;
		private System.Windows.Forms.ColumnHeader ColumnReleaseTime;
		private System.Windows.Forms.ColumnHeader ColumnNumber;
		private System.Windows.Forms.Label lblReleaseTime;
		private System.Windows.Forms.TextBox tbReleaseTime;
		private System.Windows.Forms.Button btnSave;

		private System.ComponentModel.Container components = null;

		public ReleaseTimesEditorForm(ReleaseTime[] rta, PlaceResource prOwner)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			this.rta = rta;
			this.prOwner = prOwner;
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
			this.gbTimes = new System.Windows.Forms.GroupBox();
			this.btnSave = new System.Windows.Forms.Button();
			this.tbReleaseTime = new System.Windows.Forms.TextBox();
			this.lblReleaseTime = new System.Windows.Forms.Label();
			this.lvOperationsPlaces = new System.Windows.Forms.ListView();
			this.ColumnNumber = new System.Windows.Forms.ColumnHeader();
			this.ColumnPlace = new System.Windows.Forms.ColumnHeader();
			this.ColumnReleaseTime = new System.Windows.Forms.ColumnHeader();
			this.pnlBottom.SuspendLayout();
			this.gbTimes.SuspendLayout();
			this.SuspendLayout();
			// 
			// pnlBottom
			// 
			this.pnlBottom.Controls.Add(this.pnlSeparator);
			this.pnlBottom.Controls.Add(this.btnCancel);
			this.pnlBottom.Controls.Add(this.btnOK);
			this.pnlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.pnlBottom.Location = new System.Drawing.Point(0, 191);
			this.pnlBottom.Name = "pnlBottom";
			this.pnlBottom.Size = new System.Drawing.Size(350, 49);
			this.pnlBottom.TabIndex = 4;
			// 
			// pnlSeparator
			// 
			this.pnlSeparator.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.pnlSeparator.Dock = System.Windows.Forms.DockStyle.Top;
			this.pnlSeparator.Location = new System.Drawing.Point(0, 0);
			this.pnlSeparator.Name = "pnlSeparator";
			this.pnlSeparator.Size = new System.Drawing.Size(350, 3);
			this.pnlSeparator.TabIndex = 3;
			this.pnlSeparator.Visible = false;
			// 
			// btnCancel
			// 
			this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnCancel.Location = new System.Drawing.Point(236, 5);
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
			this.btnOK.Location = new System.Drawing.Point(128, 5);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(100, 28);
			this.btnOK.TabIndex = 1;
			this.btnOK.Text = "&OK";
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// gbTimes
			// 
			this.gbTimes.Controls.Add(this.btnSave);
			this.gbTimes.Controls.Add(this.tbReleaseTime);
			this.gbTimes.Controls.Add(this.lblReleaseTime);
			this.gbTimes.Controls.Add(this.lvOperationsPlaces);
			this.gbTimes.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.gbTimes.Location = new System.Drawing.Point(13, 14);
			this.gbTimes.Name = "gbTimes";
			this.gbTimes.Size = new System.Drawing.Size(323, 166);
			this.gbTimes.TabIndex = 5;
			this.gbTimes.TabStop = false;
			this.gbTimes.Text = "Release times for resource : ";
			// 
			// btnSave
			// 
			this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnSave.Enabled = false;
			this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnSave.Location = new System.Drawing.Point(210, 129);
			this.btnSave.Name = "btnSave";
			this.btnSave.Size = new System.Drawing.Size(100, 28);
			this.btnSave.TabIndex = 4;
			this.btnSave.Text = "&Save";
			this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
			// 
			// tbReleaseTime
			// 
			this.tbReleaseTime.Enabled = false;
			this.tbReleaseTime.Location = new System.Drawing.Point(113, 132);
			this.tbReleaseTime.Name = "tbReleaseTime";
			this.tbReleaseTime.Size = new System.Drawing.Size(85, 22);
			this.tbReleaseTime.TabIndex = 3;
			this.tbReleaseTime.Text = "";
			// 
			// lblReleaseTime
			// 
			this.lblReleaseTime.Location = new System.Drawing.Point(13, 134);
			this.lblReleaseTime.Name = "lblReleaseTime";
			this.lblReleaseTime.Size = new System.Drawing.Size(94, 20);
			this.lblReleaseTime.TabIndex = 2;
			this.lblReleaseTime.Text = "Release time : ";
			// 
			// lvOperationsPlaces
			// 
			this.lvOperationsPlaces.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
																								 this.ColumnNumber,
																								 this.ColumnPlace,
																								 this.ColumnReleaseTime});
			this.lvOperationsPlaces.FullRowSelect = true;
			this.lvOperationsPlaces.GridLines = true;
			this.lvOperationsPlaces.Location = new System.Drawing.Point(13, 21);
			this.lvOperationsPlaces.MultiSelect = false;
			this.lvOperationsPlaces.Name = "lvOperationsPlaces";
			this.lvOperationsPlaces.Size = new System.Drawing.Size(297, 97);
			this.lvOperationsPlaces.TabIndex = 1;
			this.lvOperationsPlaces.View = System.Windows.Forms.View.Details;
			this.lvOperationsPlaces.SelectedIndexChanged += new System.EventHandler(this.lvOperationsPlaces_SelectedIndexChanged);
			// 
			// ColumnNumber
			// 
			this.ColumnNumber.Text = "No";
			this.ColumnNumber.Width = 40;
			// 
			// ColumnPlace
			// 
			this.ColumnPlace.Text = "Place";
			this.ColumnPlace.Width = 90;
			// 
			// ColumnReleaseTime
			// 
			this.ColumnReleaseTime.Text = "Release Time";
			this.ColumnReleaseTime.Width = 120;
			// 
			// ReleaseTimesEditorForm
			// 
			this.AutoScale = false;
			this.AutoScaleBaseSize = new System.Drawing.Size(6, 15);
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(350, 240);
			this.Controls.Add(this.gbTimes);
			this.Controls.Add(this.pnlBottom);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "ReleaseTimesEditorForm";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Release Times Editor";
			this.Load += new System.EventHandler(this.ReleaseTimesEditorForm_Load);
			this.pnlBottom.ResumeLayout(false);
			this.gbTimes.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion


		#region private void ReleaseTimesEditorForm_Load(object sender, System.EventArgs e)
		private void ReleaseTimesEditorForm_Load(object sender, System.EventArgs e)
		{
			foreach(PlaceOperation po in this.prOwner.ResourceOperationsPlaces)
			{
				ListViewItem lvi = new ListViewItem(((int)lvOperationsPlaces.Items.Count+1).ToString());
				lvi.SubItems.Add("P" + po.Index + " - " + po.NameID);
				lvi.SubItems.Add(rta[prOwner.ResourceOperationsPlaces.IndexOf(po)].Time.ToString());

				lvOperationsPlaces.Items.Add(lvi);
			}
		}
		#endregion

		#region private void btnSave_Click(object sender, System.EventArgs e)
		private void btnSave_Click(object sender, System.EventArgs e)
		{
			lvOperationsPlaces.SelectedItems[0].SubItems[2].Text = tbReleaseTime.Text;
			lvOperationsPlaces.SelectedItems.Clear();
		}
		#endregion

		#region private void lvOperationsPlaces_SelectedIndexChanged(object sender, System.EventArgs e)
		private void lvOperationsPlaces_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (lvOperationsPlaces.SelectedItems.Count != 0)
			{
				tbReleaseTime.Text = lvOperationsPlaces.SelectedItems[0].SubItems[2].Text;
				lvOperationsPlaces.SelectedItems[0].BackColor = Color.LightSteelBlue;

				btnSave.Enabled = true;
				tbReleaseTime.Enabled = true;
			}
			else
			{
				foreach(ListViewItem lvi in lvOperationsPlaces.Items)
				{
					lvi.BackColor = SystemColors.Window;
				}

				tbReleaseTime.Text = "";

				btnSave.Enabled = false;
				tbReleaseTime.Enabled = false;
			}
		}
		#endregion

		#region private void btnOK_Click(object sender, System.EventArgs e)
		private void btnOK_Click(object sender, System.EventArgs e)
		{
			this.rta = new ReleaseTime[lvOperationsPlaces.Items.Count];
			foreach(ListViewItem lvi in lvOperationsPlaces.Items)
			{
				rta[lvi.Index] = new ReleaseTime((Place)this.prOwner.ResourceOperationsPlaces[lvi.Index], int.Parse(lvi.SubItems[2].Text));
			}
		}
		#endregion




	}
}
