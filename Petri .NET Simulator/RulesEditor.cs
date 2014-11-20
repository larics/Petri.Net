using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using CDiese.Actions;

namespace PetriNetSimulator2
{
	/// <summary>
	/// Summary description for RulesEditor.
	/// </summary>
	public class RulesEditor : System.Windows.Forms.UserControl
	{
		#region public PetriNetDocument Document
		public PetriNetDocument Document
		{
			get
			{
				return this.pndSelectedDocument;
			}
			set
			{
				this.pndSelectedDocument = value;
				PopulateRules();
			}
		}
		#endregion

		#region public new bool Focused
		public new bool Focused
		{
			get
			{
				return this.lvRules.Focused;
			}
		}
		#endregion

		private PetriNetDocument pndSelectedDocument = null;
		private ActionList alActions = new ActionList();
		private Action acRefreshRules = new Action();

		private System.Windows.Forms.ListView lvRules;
		private System.Windows.Forms.ColumnHeader chExpression;
		private System.Windows.Forms.ColumnHeader chDescription;
		private System.Windows.Forms.ImageList ilImageList;
		private System.Windows.Forms.ContextMenu cmMenu;
		private System.Windows.Forms.MenuItem miEdit;
		private System.Windows.Forms.MenuItem miDelete;
		private System.Windows.Forms.MenuItem miSeparator1;
		private System.Windows.Forms.MenuItem miInsert;
		private System.ComponentModel.IContainer components;

		public RulesEditor()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			this.acRefreshRules.Enabled = false;
			this.acRefreshRules.Update += new EventHandler(acRefreshRules_Update);
			this.alActions.Actions.Add(this.acRefreshRules);
			this.alActions.SetAction(this, this.acRefreshRules);
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

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(RulesEditor));
			this.lvRules = new System.Windows.Forms.ListView();
			this.chExpression = new System.Windows.Forms.ColumnHeader();
			this.chDescription = new System.Windows.Forms.ColumnHeader();
			this.ilImageList = new System.Windows.Forms.ImageList(this.components);
			this.cmMenu = new System.Windows.Forms.ContextMenu();
			this.miEdit = new System.Windows.Forms.MenuItem();
			this.miDelete = new System.Windows.Forms.MenuItem();
			this.miSeparator1 = new System.Windows.Forms.MenuItem();
			this.miInsert = new System.Windows.Forms.MenuItem();
			this.SuspendLayout();
			// 
			// lvRules
			// 
			this.lvRules.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
																					  this.chExpression,
																					  this.chDescription});
			this.lvRules.ContextMenu = this.cmMenu;
			this.lvRules.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lvRules.FullRowSelect = true;
			this.lvRules.GridLines = true;
			this.lvRules.HideSelection = false;
			this.lvRules.Location = new System.Drawing.Point(0, 0);
			this.lvRules.MultiSelect = false;
			this.lvRules.Name = "lvRules";
			this.lvRules.Size = new System.Drawing.Size(640, 150);
			this.lvRules.SmallImageList = this.ilImageList;
			this.lvRules.TabIndex = 0;
			this.lvRules.View = System.Windows.Forms.View.Details;
			this.lvRules.DoubleClick += new System.EventHandler(this.lvRules_DoubleClick);
			// 
			// chExpression
			// 
			this.chExpression.Text = "Expression";
			this.chExpression.Width = 400;
			// 
			// chDescription
			// 
			this.chDescription.Text = "Description";
			this.chDescription.Width = 400;
			// 
			// ilImageList
			// 
			this.ilImageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth24Bit;
			this.ilImageList.ImageSize = new System.Drawing.Size(16, 16);
			this.ilImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilImageList.ImageStream")));
			this.ilImageList.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// cmMenu
			// 
			this.cmMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																				   this.miInsert,
																				   this.miSeparator1,
																				   this.miEdit,
																				   this.miDelete});
			this.cmMenu.Popup += new System.EventHandler(this.cmMenu_Popup);
			// 
			// miEdit
			// 
			this.miEdit.Index = 2;
			this.miEdit.Text = "&Edit rule";
			this.miEdit.Click += new System.EventHandler(this.miEdit_Click);
			// 
			// miDelete
			// 
			this.miDelete.Index = 3;
			this.miDelete.Text = "&Delete rule";
			this.miDelete.Click += new System.EventHandler(this.miDelete_Click);
			// 
			// miSeparator1
			// 
			this.miSeparator1.Index = 1;
			this.miSeparator1.Text = "-";
			// 
			// miInsert
			// 
			this.miInsert.Index = 0;
			this.miInsert.Text = "&Insert rule";
			this.miInsert.Click += new System.EventHandler(this.miInsert_Click);
			// 
			// RulesEditor
			// 
			this.Controls.Add(this.lvRules);
			this.Name = "RulesEditor";
			this.Size = new System.Drawing.Size(640, 150);
			this.ResumeLayout(false);

		}
		#endregion


		#region private void lvRules_DoubleClick(object sender, System.EventArgs e)
		private void lvRules_DoubleClick(object sender, System.EventArgs e)
		{
			if (this.lvRules.SelectedItems.Count != 0)
			{
				// Create new rule
				if (this.lvRules.SelectedIndices[0] == 0)
				{
					RuleEditor re = new RuleEditor(this.pndSelectedDocument);
					if (DialogResult.OK == re.ShowDialog())
					{
						if (re.CreatedRule != null)
						{
							this.pndSelectedDocument.Rules.Add(re.CreatedRule);
							this.PopulateRules();
						}
					}
				}
				else
				{
					// Edit existing rule
					Rule r = (Rule)this.lvRules.SelectedItems[0].Tag;
					RuleEditor re = new RuleEditor(this.pndSelectedDocument, r.Expression, r.Description);
					if (DialogResult.OK == re.ShowDialog())
					{
						if (re.CreatedRule != null)
						{
							this.pndSelectedDocument.Rules[this.pndSelectedDocument.Rules.IndexOf(r)] = re.CreatedRule;
							this.PopulateRules();
						}
					}
				}
			}
		}
		#endregion

		#region private void acRefreshRules_Update(object sender, EventArgs e)
		private void acRefreshRules_Update(object sender, EventArgs e)
		{
			((Action)sender).Enabled = this.pndSelectedDocument != null;
		}
		#endregion

		#region private void PopulateRules()
		private void PopulateRules()
		{
			this.lvRules.BeginUpdate();

			this.lvRules.Items.Clear();

			if (this.pndSelectedDocument != null)
			{
				ListViewItem lviFirst = new ListViewItem("<Double-click here to add a new rule>");
				this.lvRules.Items.Add(lviFirst);

				foreach(Rule r in this.pndSelectedDocument.Rules)
				{
					ListViewItem lvi = new ListViewItem(r.Expression);
					lvi.SubItems.Add(r.Description);
					lvi.ImageIndex = (r.Evaluate(this.pndSelectedDocument)) ? 0 : 1;
					lvi.Tag = r;

					this.lvRules.Items.Add(lvi);
				}
			}

			this.lvRules.EndUpdate();
		}
		#endregion

		#region public void UpdateRules()
		public void UpdateRules()
		{
			if (this.pndSelectedDocument != null)
			{
				for(int i = 0; i < this.pndSelectedDocument.Rules.Count; i++)
				{
					Rule r = (Rule)this.pndSelectedDocument.Rules[i];
					lvRules.Items[i + 1].ImageIndex = (r.Evaluate(this.pndSelectedDocument)) ? 0 : 1;
				}
			}
		}
		#endregion

		#region public void DeleteSelectedRule()
		public void DeleteSelectedRule()
		{
			ListViewItem lvi = this.lvRules.SelectedItems[0];
			if (lvi.Index != 0)
			{
				if (DialogResult.Yes == MessageBox.Show("Are you sure you want to delete selected rule?", "Petri .NET Simulator 2.0 - Question", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question))
				{
					Rule r = (Rule)lvi.Tag;

					this.pndSelectedDocument.Rules.Remove(r);
					this.lvRules.Items.Remove(lvi);
				}
			}
		}
		#endregion


		#region private void cmMenu_Popup(object sender, System.EventArgs e)
		private void cmMenu_Popup(object sender, System.EventArgs e)
		{
			if (this.lvRules.SelectedItems.Count == 0 || this.lvRules.SelectedIndices[0] == 0)
			{
				this.miInsert.Visible = false;
				this.miSeparator1.Visible = false;
				this.miEdit.Visible = false;
				this.miDelete.Visible = false;
			}
			else
			{
				this.miInsert.Visible = true;
				this.miSeparator1.Visible = true;
				this.miEdit.Visible = true;
				this.miDelete.Visible = true;
			}
		}
		#endregion

		#region private void miInsert_Click(object sender, System.EventArgs e)
		private void miInsert_Click(object sender, System.EventArgs e)
		{
			RuleEditor re = new RuleEditor(this.pndSelectedDocument);
			if (DialogResult.OK == re.ShowDialog())
			{
				if (re.CreatedRule != null)
				{
					this.pndSelectedDocument.Rules.Insert(this.lvRules.SelectedIndices[0] - 1, re.CreatedRule);
					this.PopulateRules();
				}
			}
		}
		#endregion

		#region private void miEdit_Click(object sender, System.EventArgs e)
		private void miEdit_Click(object sender, System.EventArgs e)
		{
			lvRules_DoubleClick(sender, e);
		}
		#endregion

		#region private void miDelete_Click(object sender, System.EventArgs e)
		private void miDelete_Click(object sender, System.EventArgs e)
		{
			DeleteSelectedRule();
		}
		#endregion


	}
}
