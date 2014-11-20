using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace PetriNetSimulator2
{
	/// <summary>
	/// Summary description for DocumentExplorer.
	/// </summary>
	public class DocumentExplorer : System.Windows.Forms.UserControl
	{
		#region public PetriNetDocument Document
		public PetriNetDocument Document
		{
			set
			{
				this.pndDocument = value;
				this.PopulateExplorer();
			}
		}
		#endregion

		private PetriNetDocument pndDocument;

		private System.Windows.Forms.TreeView tvDocumentExplorer;
		private System.Windows.Forms.ImageList ilNodeImages;
		private System.ComponentModel.IContainer components;

		#region public DocumentExplorer()
		public DocumentExplorer()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			//Workaround HScroll bug
			TreeViewHelper.DisableHScroller(this.tvDocumentExplorer.Handle);
			TreeViewHelper.EnableHScroller(this.tvDocumentExplorer.Handle);
		}
		#endregion

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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(DocumentExplorer));
			this.tvDocumentExplorer = new System.Windows.Forms.TreeView();
			this.ilNodeImages = new System.Windows.Forms.ImageList(this.components);
			this.SuspendLayout();
			// 
			// tvDocumentExplorer
			// 
			this.tvDocumentExplorer.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tvDocumentExplorer.ImageList = this.ilNodeImages;
			this.tvDocumentExplorer.Location = new System.Drawing.Point(0, 0);
			this.tvDocumentExplorer.Name = "tvDocumentExplorer";
			this.tvDocumentExplorer.Size = new System.Drawing.Size(150, 256);
			this.tvDocumentExplorer.TabIndex = 0;
			// 
			// ilNodeImages
			// 
			this.ilNodeImages.ColorDepth = System.Windows.Forms.ColorDepth.Depth24Bit;
			this.ilNodeImages.ImageSize = new System.Drawing.Size(16, 16);
			this.ilNodeImages.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilNodeImages.ImageStream")));
			this.ilNodeImages.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// DocumentExplorer
			// 
			this.Controls.Add(this.tvDocumentExplorer);
			this.Name = "DocumentExplorer";
			this.Size = new System.Drawing.Size(150, 256);
			this.ResumeLayout(false);

		}
		#endregion


		#region private void PopulateExplorer()
		private void PopulateExplorer()
		{
			this.tvDocumentExplorer.BeginUpdate();

			this.tvDocumentExplorer.Nodes.Clear();

			if (this.pndDocument != null)
			{ 
				TreeNode tnTo = new TreeNode(this.pndDocument.ObjectsTree.Text, 0, 0);
				tvDocumentExplorer.Nodes.Add(tnTo);

				foreach(TreeNode tn in this.pndDocument.ObjectsTree.Nodes)
				{
					this.AddNode(tn, tnTo);
				}
			}

			this.tvDocumentExplorer.ExpandAll();

			this.tvDocumentExplorer.EndUpdate();
		}
		#endregion

		#region private void AddNode(TreeNode tn, TreeNode tnTo)
		private void AddNode(TreeNode tn, TreeNode tnTo)
		{
			int iImageIndex = 0;
			object o = tn.Tag;
			if (o is PlaceInput)
				iImageIndex = 1;
			else if (o is PlaceOperation)
				iImageIndex = 2;
			else if (o is PlaceResource)
				iImageIndex = 3;
			else if (o is PlaceControl)
				iImageIndex = 4;
			else if (o is PlaceOutput)
				iImageIndex = 5;
			else if (o is Transition)
				iImageIndex = 6;
			else if (o is DescriptionLabel)
				iImageIndex = 7;
			else if (o is Subsystem)
				iImageIndex = 8;
			else if (o is Input)
				iImageIndex = 9;
			else if (o is Output)
				iImageIndex = 10;
			else if (o is Connection)
				iImageIndex = 11;

			TreeNode tnNew = new TreeNode(o.ToString(), iImageIndex, iImageIndex);
			tnTo.Nodes.Add(tnNew);

			if (tn.Nodes.Count != 0)
			{
				foreach(TreeNode tnn in tn.Nodes)
				{

					this.AddNode(tnn, tnNew);
				}
			}
		}
		#endregion

	}
}
