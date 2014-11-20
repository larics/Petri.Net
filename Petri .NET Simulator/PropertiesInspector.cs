using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace PetriNetSimulator2
{
	/// <summary>
	/// Summary description for PropertiesInspector.
	/// </summary>
	public class PropertiesInspector : System.Windows.Forms.UserControl
	{
		private ArrayList alObjects = new ArrayList();
		
		private System.Windows.Forms.PropertyGrid pgProperties;
		private object oSelectedObject = null;
		private System.Windows.Forms.ComboBox cbObjects;
		private bool bSupressSelectedObjectChangedEvent = false;

		#region public ArrayList Objects
		public ArrayList Objects
		{
			set
			{
				this.alObjects = value;
				
				if (this.alObjects != null)
					this.alObjects.Sort(new PropertiesInspectorSorter());

				cbObjects_Fill();
			}
		}
		#endregion

		#region public object SelectedObject
		public object SelectedObject
		{
			get
			{
				return this.oSelectedObject;
			}
			set
			{
				this.oSelectedObject = value;
				this.cbObjects.SelectedItem = value;
			}
		}
		#endregion
	
		#region public ArrayList SelectedObjects
		public ArrayList SelectedObjects
		{
			get
			{
				ArrayList al = new ArrayList();
				foreach(object o in this.pgProperties.SelectedObjects)
				{
					al.Add(o);
				}

				return al;
			}
			set
			{
				object[] oa = new object[value.Count];
				
				for (int i = 0; i < value.Count; i++)
				{
					oa[i] = value[i];
				}

				this.pgProperties.SelectedObjects = oa;

				this.bSupressSelectedObjectChangedEvent = true;

				if (oa.Length == 1)
					this.cbObjects.SelectedItem = oa[0];
				else
					this.cbObjects.SelectedItem = null;
			}
		}
		#endregion

		#region public AttributeCollection BrowsableAttributes
		public AttributeCollection BrowsableAttributes
		{
			get
			{
				return this.pgProperties.BrowsableAttributes;
			}
			set
			{
				this.pgProperties.BrowsableAttributes = value;

			}
		}
		#endregion

		public event EventHandler SelectedObjectChanged;

		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public PropertiesInspector()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

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
			this.pgProperties = new System.Windows.Forms.PropertyGrid();
			this.cbObjects = new System.Windows.Forms.ComboBox();
			this.SuspendLayout();
			// 
			// pgProperties
			// 
			this.pgProperties.CommandsVisibleIfAvailable = true;
			this.pgProperties.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pgProperties.LargeButtons = false;
			this.pgProperties.LineColor = System.Drawing.SystemColors.ScrollBar;
			this.pgProperties.Location = new System.Drawing.Point(0, 24);
			this.pgProperties.Name = "pgProperties";
			this.pgProperties.Size = new System.Drawing.Size(171, 302);
			this.pgProperties.TabIndex = 0;
			this.pgProperties.Text = "propertyGrid1";
			this.pgProperties.ViewBackColor = System.Drawing.SystemColors.Window;
			this.pgProperties.ViewForeColor = System.Drawing.SystemColors.WindowText;
			// 
			// cbObjects
			// 
			this.cbObjects.Dock = System.Windows.Forms.DockStyle.Top;
			this.cbObjects.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbObjects.DropDownWidth = 250;
			this.cbObjects.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(238)));
			this.cbObjects.Location = new System.Drawing.Point(0, 0);
			this.cbObjects.Name = "cbObjects";
			this.cbObjects.Size = new System.Drawing.Size(171, 24);
			this.cbObjects.TabIndex = 1;
			this.cbObjects.SelectedIndexChanged += new System.EventHandler(this.cbObjects_SelectedIndexChanged);
			// 
			// PropertiesInspector
			// 
			this.Controls.Add(this.pgProperties);
			this.Controls.Add(this.cbObjects);
			this.Name = "PropertiesInspector";
			this.Size = new System.Drawing.Size(171, 326);
			this.ResumeLayout(false);

		}
		#endregion


		#region private void cbObjects_SelectedIndexChanged(object sender, System.EventArgs e)
		private void cbObjects_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (bSupressSelectedObjectChangedEvent != true)
			{
				this.oSelectedObject = cbObjects.SelectedItem;
				this.pgProperties.SelectedObject = this.oSelectedObject;

				if (this.SelectedObjectChanged != null)
					this.SelectedObjectChanged(this, EventArgs.Empty);
			}

			this.bSupressSelectedObjectChangedEvent = false;
		}
		#endregion

		#region private void cbObjects_Fill()
		private void cbObjects_Fill()
		{
			this.cbObjects.Items.Clear();

			if (this.alObjects != null)
			{
				foreach (object o in this.alObjects)
				{
					this.cbObjects.Items.Add(o);
				}
			}
		}
		#endregion

	}

}
