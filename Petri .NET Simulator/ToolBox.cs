using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace PetriNetSimulator2
{
	public class ToolBox : UserControl
	{
		#region public bool EnabledToolbox
		public bool EnabledToolbox
		{
			get
			{
				return this.bEnabledToolbox;
			}
			set
			{
				this.bEnabledToolbox = value;
				this.tvToolBox.ImageList = value ? this.ilToolBox : this.ilToolBoxDisabled;
				this.tvToolBox.Enabled = value;
			}
		}
		#endregion

		private bool bEnabledToolbox = false;

		private System.Windows.Forms.TreeView tvToolBox;
		private System.Windows.Forms.ImageList ilToolBox;
		private System.Windows.Forms.ImageList ilToolBoxDisabled;
		private System.ComponentModel.IContainer components = null;

		public ToolBox()
		{
			// This call is required by the Windows Form Designer.
			InitializeComponent();

			#region Initialize TreeView
			TreeNode tnSimpleRoot = new TreeNode("Simple  ", 10, 10);
			tnSimpleRoot.NodeFont = new Font(this.Font, FontStyle.Bold);
			tnSimpleRoot.BackColor = SystemColors.ControlDark;
			tnSimpleRoot.ForeColor = Color.White;
			
			TreeNode tn = new TreeNode("Input", 0, 0);
			Place p = new PlaceInput();
			PetriNetEditorMergeModule pnemm = new PetriNetEditorMergeModule(p);
			tn.Tag = pnemm;
			tnSimpleRoot.Nodes.Add(tn);

			tn = new TreeNode("Operation", 1, 1);
			p = new PlaceOperation();
			pnemm = new PetriNetEditorMergeModule(p);
			tn.Tag = pnemm;
			tnSimpleRoot.Nodes.Add(tn);

			tn = new TreeNode("Resource", 2, 2);
			p = new PlaceResource();
			pnemm = new PetriNetEditorMergeModule(p);
			tn.Tag = pnemm;
			tnSimpleRoot.Nodes.Add(tn);

			tn = new TreeNode("Control", 3, 3);
			p = new PlaceControl();
			pnemm = new PetriNetEditorMergeModule(p);
			tn.Tag = pnemm;
			tnSimpleRoot.Nodes.Add(tn);

			tn = new TreeNode("Output", 4, 4);
			p = new PlaceOutput();
			pnemm = new PetriNetEditorMergeModule(p);
			tn.Tag = pnemm;
			tnSimpleRoot.Nodes.Add(tn);

			tn = new TreeNode("Transition", 5, 5);
			Transition t = new Transition();
			pnemm = new PetriNetEditorMergeModule(t);
			tn.Tag = pnemm;
			tnSimpleRoot.Nodes.Add(tn);

			tn = new TreeNode("Label", 6, 6);
			DescriptionLabel dl  = new DescriptionLabel();
			pnemm = new PetriNetEditorMergeModule(dl);
			tn.Tag = pnemm;
			tnSimpleRoot.Nodes.Add(tn);

			TreeNode tnAdvancedRoot = new TreeNode("Advanced  ", 10, 10);
			tnAdvancedRoot.NodeFont = new Font(this.Font, FontStyle.Bold);
			tnAdvancedRoot.BackColor = SystemColors.ControlDark;
			tnAdvancedRoot.ForeColor = Color.White;

			tn = new TreeNode("Subsystem block", 7, 7);
			Subsystem s = new Subsystem();
			pnemm = new PetriNetEditorMergeModule(s);
			tn.Tag = pnemm;
			tnAdvancedRoot.Nodes.Add(tn);

			tn = new TreeNode("In", 8, 8);
			Input i = new Input();
			pnemm = new PetriNetEditorMergeModule(i);
			tn.Tag = pnemm;
			tnAdvancedRoot.Nodes.Add(tn);

			tn = new TreeNode("Out", 9, 9);
			Output o = new Output();
			pnemm = new PetriNetEditorMergeModule(o);
			tn.Tag = pnemm;
			tnAdvancedRoot.Nodes.Add(tn);

			tn = new TreeNode("Resource-operation", 2, 2);
			Transition t1 = new Transition();
			pnemm = new PetriNetEditorMergeModule();
			pnemm.Add(t1);
			PlaceOperation po = new PlaceOperation();
			po.Location = new Point(150, 0);
			pnemm.Add(po);
			PlaceResource pr = new PlaceResource();
			pr.Tokens = 1;
			pr.Location = new Point(150, 100);
			pnemm.Add(pr);
			Transition t2 = new Transition();
			t2.Location = new Point(300, 0);
			pnemm.Add(t2);
			Connection cn = new Connection(t1, po, 1, 1, new Point(t1.Width, (int)(t1.Height/2)), new Point(0, (int)(po.Height/2)), 1, true);
			pnemm.Add(cn);
			cn = new Connection(po, t2, 1, 1, new Point(po.Width, (int)(po.Height/2)), new Point(0, (int)(t2.Height/2)), 1, true);
			pnemm.Add(cn);
			cn = new Connection(t2, pr, 1, 1, new Point(t2.Width, (int)(3*t2.Height/4)), new Point(pr.Width, (int)(pr.Height/2)), 1, true);
			pnemm.Add(cn);
			cn = new Connection(pr, t1, 1, 1, new Point(0, (int)(pr.Height/2)), new Point(0, (int)(3*t1.Height/4)), 1, true);
			pnemm.Add(cn);
			tn.Tag = pnemm;
			tnAdvancedRoot.Nodes.Add(tn);

			tvToolBox.BeginUpdate();
			tvToolBox.Nodes.Add(tnSimpleRoot);
			tvToolBox.Nodes.Add(tnAdvancedRoot);
			
			tvToolBox.ExpandAll();
			
			tvToolBox.EndUpdate();
			tvToolBox.Refresh();
			#endregion

			tvToolBox.BeforeSelect += new System.Windows.Forms.TreeViewCancelEventHandler(this.tvToolBox_BeforeSelect);
			tvToolBox.Click +=new EventHandler(tvToolBox_Click);
			tvToolBox.MouseDown += new MouseEventHandler(tvToolBox_MouseDown);

			//Workaround HScroll bug
			TreeViewHelper.DisableHScroller(this.tvToolBox.Handle);
			TreeViewHelper.EnableHScroller(this.tvToolBox.Handle);
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(ToolBox));
			this.tvToolBox = new System.Windows.Forms.TreeView();
			this.ilToolBoxDisabled = new System.Windows.Forms.ImageList(this.components);
			this.ilToolBox = new System.Windows.Forms.ImageList(this.components);
			this.SuspendLayout();
			// 
			// tvToolBox
			// 
			this.tvToolBox.BackColor = System.Drawing.SystemColors.Control;
			this.tvToolBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tvToolBox.Enabled = false;
			this.tvToolBox.FullRowSelect = true;
			this.tvToolBox.ImageList = this.ilToolBoxDisabled;
			this.tvToolBox.Location = new System.Drawing.Point(0, 0);
			this.tvToolBox.Name = "tvToolBox";
			this.tvToolBox.ShowLines = false;
			this.tvToolBox.ShowRootLines = false;
			this.tvToolBox.Size = new System.Drawing.Size(152, 200);
			this.tvToolBox.TabIndex = 0;
			// 
			// ilToolBoxDisabled
			// 
			this.ilToolBoxDisabled.ColorDepth = System.Windows.Forms.ColorDepth.Depth24Bit;
			this.ilToolBoxDisabled.ImageSize = new System.Drawing.Size(16, 16);
			this.ilToolBoxDisabled.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilToolBoxDisabled.ImageStream")));
			this.ilToolBoxDisabled.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// ilToolBox
			// 
			this.ilToolBox.ColorDepth = System.Windows.Forms.ColorDepth.Depth24Bit;
			this.ilToolBox.ImageSize = new System.Drawing.Size(16, 16);
			this.ilToolBox.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilToolBox.ImageStream")));
			this.ilToolBox.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// ToolBox
			// 
			this.Controls.Add(this.tvToolBox);
			this.Name = "ToolBox";
			this.Size = new System.Drawing.Size(152, 200);
			this.ResumeLayout(false);

		}
		#endregion

		#region private void tvToolBox_BeforeSelect(object sender, System.Windows.Forms.TreeViewCancelEventArgs e)
		private void tvToolBox_BeforeSelect(object sender, System.Windows.Forms.TreeViewCancelEventArgs e)
		{
			if (e.Node.Parent == null)
				e.Cancel = true;

			this.tvToolBox.Focus();
		}
		#endregion

		#region private void tvToolBox_Click(object sender, EventArgs e)
		private void tvToolBox_Click(object sender, EventArgs e)
		{
			TreeNode tn = tvToolBox.GetNodeAt(this.PointToClient(MousePosition));

			if (tn.IsExpanded == true)
				tn.Collapse();
			else
				tn.Expand();
		}
		#endregion

		#region private void tvToolBox_MouseDown(object sender, MouseEventArgs e)
		private void tvToolBox_MouseDown(object sender, MouseEventArgs e)
		{
			tvToolBox.SelectedNode = tvToolBox.GetNodeAt(e.X, e.Y);
			if(tvToolBox.SelectedNode != null)
				this.DoDragDrop(tvToolBox.SelectedNode.Tag, DragDropEffects.Copy | DragDropEffects.Move);
		}
		#endregion


		#region public void ExpandAll()
		public void ExpandAll()
		{
			this.tvToolBox.ExpandAll();
		}
		#endregion

	}
}

