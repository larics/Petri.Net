using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.Serialization;

namespace PetriNetSimulator2
{
	public class SelectableAndMovableControl : ResizableControl, ISelectable, ISerializable
	{
		// Properties
		#region public bool Selected
		public bool Selected
		{
			get
			{
				return this.bSelected;
			}
			set
			{
				this.bSelected = value;
				if (value == true)
				{
					this.cBackgroundColor = this.cActiveColor;
				}
				else
				{
					this.cBackgroundColor = this.cInactiveColor;
				}

				this.Refresh();
			}
		}
		#endregion

		//Events
		public event SelectionHandler SelectionChanged;
		public new event LocationChangedEventHandler LocationChanged;

		//Fields
		protected Color cActiveColor = Color.FromArgb(220, Color.SteelBlue);
		protected Color cInactiveColor = Color.FromArgb(220, 97, 190, 103);
		protected Color cBackgroundColor = Color.SteelBlue;
		protected bool bSelected = true;
		protected Point ptMouseDragOffset;
		protected Point ptInitialLocation;

		private ArrayList alObjects = new ArrayList();
		private ArrayList alOldValues = new ArrayList();

		private System.ComponentModel.IContainer components = null;

		#region public SelectableAndMovableControl(Size szDefaultSize) : base(szDefaultSize)
		public SelectableAndMovableControl(Size szDefaultSize) : base(szDefaultSize)
		{
			// This call is required by the Windows Form Designer.
			InitializeComponent();

			this.Cursor = System.Windows.Forms.Cursors.SizeAll;
			this.MouseUp += new MouseEventHandler(SelectableAndMovableControl_MouseUp);
		}
		#endregion

		#region protected SelectableAndMovableControl(SerializationInfo info, StreamingContext context) : base(info, context)
		protected SelectableAndMovableControl(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			this.Cursor = System.Windows.Forms.Cursors.SizeAll;
			this.MouseUp += new MouseEventHandler(SelectableAndMovableControl_MouseUp);
		}
		#endregion


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
			// 
			// SelectableAndMovableControl
			// 
			this.Name = "SelectableAndMovableControl";
			this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.SelectableAndMovableControl_KeyUp);
			this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.SelectableAndMovableControl_MouseMove);
			this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.SelectableAndMovableControl_MouseDown);

		}
		#endregion


		#region public new void GetObjectData(SerializationInfo info, StreamingContext context)
		public new void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
		#endregion


		#region protected void SelectableAndMovableControl_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		protected void SelectableAndMovableControl_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			// Remembers point where mouse was clicked in Place control
			ptMouseDragOffset = new Point(e.X, e.Y);

			// Remembers initial Location
			this.ptInitialLocation = this.Location;

			// Enables mouse capture
			this.Capture = true;

			PetriNetEditor pne = (PetriNetEditor)this.Parent;

			// if this condition is true allow selecting object, else moving objects is available
			if (!pne.SelectedObjects.Contains(this) || pne.PressedKey == Keys.ShiftKey)
			{
				if (this.SelectionChanged != null)
					this.SelectionChanged(this, new SelectionEventArgs(pne.PressedKey));
			}

			// Remember all objects in selection initial positions for Undo/Redo
			this.alObjects.Clear();
			this.alOldValues.Clear();
			foreach(object o in pne.SelectedObjects)
			{
				if (o is SelectableAndMovableControl)
				{
					this.alObjects.Add(o);

					SelectableAndMovableControl smac = (SelectableAndMovableControl)o;
					this.alOldValues.Add(smac.Location);
				}
			}

			// Brings control to top of z-order
			this.BringToFront();

		}
		#endregion

		#region private void SelectableAndMovableControl_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
		private void SelectableAndMovableControl_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			// Moves Place in PetriNetEditor control
			if (this.Capture == true && this.bResizing != true && e.Button == MouseButtons.Left)
			{
				EditorSurface es = (EditorSurface)this.Parent;

				Point pt = this.PointToScreen(new Point(e.X, e.Y));
				pt = es.PointToClient(pt);
				pt.X -= ptMouseDragOffset.X;
				pt.Y -= ptMouseDragOffset.Y;

				// If AutoScroll size is larger than editor control size
				if (es.AutoScroll == true)
				{
					pt.X = Math.Max(es.AutoScrollPosition.X, pt.X);
					pt.Y = Math.Max(es.AutoScrollPosition.Y, pt.Y);

					pt.X = Math.Min(pt.X, es.AutoScrollMinSize.Width + es.AutoScrollPosition.X - this.Bounds.Width);
					pt.Y = Math.Min(pt.Y, es.AutoScrollMinSize.Height + es.AutoScrollPosition.Y - this.Bounds.Height);
				}
				else
				{
					pt.X = Math.Max(0, pt.X);
					pt.Y = Math.Max(0, pt.Y);

					pt.X = Math.Min(pt.X, es.Width - this.Bounds.Width);
					pt.Y = Math.Min(pt.Y, es.Height - this.Bounds.Height);
				}

				foreach(object o in es.SelectedObjects)
				{
					if (o is Control)
					{
						Control c = (Control)o;

						if (c != this)
						{
							c.Location = new Point(c.Location.X + (pt.X - this.Location.X), c.Location.Y + (pt.Y - this.Location.Y));
						}
					}
				}

				this.Location = pt;

				// To slow down scrolling
				es.Refresh();
			}
		}
		#endregion

		#region private void SelectableAndMovableControl_MouseUp(object sender, MouseEventArgs e)
		private void SelectableAndMovableControl_MouseUp(object sender, MouseEventArgs e)
		{
			if (this.Capture == true)
				this.Capture = false;

			if (this.ptInitialLocation != this.Location)
			{
				// ArrayLists must be cloned because they are fields of this class, 
				// so any change to them in this class would affect UndoRedoItem objects
				if (this.LocationChanged != null)
					this.LocationChanged(this, new LocationChangedEventArgs((ArrayList)this.alObjects.Clone(), (ArrayList)this.alOldValues.Clone()));
			}
		}
		#endregion

		#region private void SelectableAndMovableControl_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
		private void SelectableAndMovableControl_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			// This function is used to reset PetriNetEditor PressedKey property to None
			// on KeyUp event. This is used for selection of objects with Shift key

			PetriNetEditor pne = (PetriNetEditor)this.Parent;
			pne.PressedKey = Keys.None;
		}
		#endregion

		#region public void PerformActivation()
		public void PerformActivation()
		{
			this.Selected = true;
			if (this.SelectionChanged != null)
				this.SelectionChanged(this, new SelectionEventArgs(Keys.None));
		}
		#endregion

	}


	public class SelectionEventArgs
	{
		public Keys Modifier
		{
			get
			{
				return this.kModifier;
			}
		}

		private Keys kModifier = Keys.None;

		public SelectionEventArgs(Keys kModifier)
		{
			this.kModifier = kModifier;
		}
	}

	public class LocationChangedEventArgs
	{
		public ArrayList Objects
		{
			get
			{
				return this.alObjects;
			}
		}

		public ArrayList OldValues
		{
			get
			{
				return this.alptOldValues;
			}
		}

		private ArrayList alptOldValues;
		private ArrayList alObjects;

		public LocationChangedEventArgs(ArrayList alObjects, ArrayList alptOldValues)
		{
			this.alptOldValues = alptOldValues;
			this.alObjects = alObjects;
		}
	}

}

