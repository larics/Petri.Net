using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.Windows.Forms;

namespace PetriNetSimulator2
{
	/// <summary>
	/// Summary description for EditorSurface.
	/// </summary>
	public class EditorSurface : System.Windows.Forms.UserControl
	{
		// Properties

		#region public Keys PressedKey
		public Keys PressedKey
		{
			get
			{
				return this.kPressedKey;
			}
			set
			{
				this.kPressedKey = value;
			}
		}
		#endregion

		#region public ArrayList SelectedObjects
		public ArrayList SelectedObjects
		{
			get 
			{
				return this.alSelectedObjects;
			}
		}
		#endregion

		#region public ArrayList Objects
		public ArrayList Objects
		{
			get
			{
				return this.alObjects;
			}
		}
		#endregion

		#region public virtual float Zoom
		public virtual float Zoom
		{
			get
			{
				return this.fZoomLevel;
			}
			set
			{
				this.ZoomAdjust(this.fZoomLevel, value);
			}
		}
		#endregion

		// Events
		public event EventHandler SelectedObjectsChanged;
		public event EventHandler ContentsChanged;

		// Fields
		private ArrayList alSelectedObjects = new ArrayList();
		private ArrayList alObjects = new ArrayList();
		protected MouseEventArgs meaLastMouseDown;
		protected Keys kPressedKey = Keys.None;
		protected bool bSelecting = false;
		public Point ptDragBegin;
		public Point ptDragEnd;
		protected float fZoomLevel = 1f;

		public bool bConnecting = false;

		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public EditorSurface()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			this.SetStyle(ControlStyles.UserPaint |	ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer, true);
			this.UpdateStyles();
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
			// 
			// EditorSurface
			// 
			this.Name = "EditorSurface";
			this.Size = new System.Drawing.Size(136, 128);
			this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.EditorSurface_MouseUp);
			this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.EditorSurface_MouseMove);
			this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.EditorSurface_MouseDown);

		}
		#endregion


		#region protected void EditorSurface_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		protected void EditorSurface_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			this.meaLastMouseDown = e;
			this.ptDragEnd = new Point(e.X, e.Y);
			this.bSelecting = true;

			if (this.meaLastMouseDown.Button == MouseButtons.Left && this.kPressedKey != Keys.ShiftKey)
			{
				this.DeselectControls();
			}
		}
		#endregion

		#region protected void EditorSurface_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
		protected void EditorSurface_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			// Drawing selection rectangle
			if (bSelecting == true)
			{
				if (e.Button == MouseButtons.Left)
				{
					this.ptDragEnd = new Point(e.X, e.Y);

					// Select all components within rectangle
					Point ptBegin = new Point(meaLastMouseDown.X, meaLastMouseDown.Y);
					Point ptEnd = new Point(ptDragEnd.X, ptDragEnd.Y);

					foreach(SelectableAndMovableControl smac in this.Objects)
					{
						Point ptCenter = new Point(smac.Width/2, smac.Height/2);
						ptCenter = smac.PointToScreen(ptCenter);
						ptCenter = this.PointToClient(ptCenter);

						// I quadrant
						if (ptBegin.X <= ptEnd.X && ptBegin.Y > ptEnd.Y)
						{
							if (ptCenter.X > ptBegin.X && ptCenter.X < ptEnd.X && ptCenter.Y < ptBegin.Y && ptCenter.Y > ptEnd.Y)
								smac.Selected = true;
							else
							{
								if (this.kPressedKey != Keys.ShiftKey)
									smac.Selected = false;
							}
						}
							// II quadrant
						else if (ptBegin.X > ptEnd.X && ptBegin.Y > ptEnd.Y)
						{
							if (ptCenter.X < ptBegin.X && ptCenter.X > ptEnd.X && ptCenter.Y < ptBegin.Y && ptCenter.Y > ptEnd.Y)
								smac.Selected = true;
							else
							{
								if (this.kPressedKey != Keys.ShiftKey)
									smac.Selected = false;
							}
						}
							// III quadrant
						else if (ptBegin.X > ptEnd.X && ptBegin.Y <= ptEnd.Y)
						{
							if (ptCenter.X < ptBegin.X && ptCenter.X > ptEnd.X && ptCenter.Y > ptBegin.Y && ptCenter.Y < ptEnd.Y)
								smac.Selected = true;
							else
							{
								if (this.kPressedKey != Keys.ShiftKey)
									smac.Selected = false;
							}
						}
							// IV quadrant
						else if (ptBegin.X <= ptEnd.X && ptBegin.Y <= ptEnd.Y)
						{
							if (ptCenter.X > ptBegin.X && ptCenter.X < ptEnd.X && ptCenter.Y > ptBegin.Y && ptCenter.Y < ptEnd.Y)
								smac.Selected = true;
							else
							{
								if (this.kPressedKey != Keys.ShiftKey)
									smac.Selected = false;
							}
						}				
					}

					this.Refresh();
				}
			}
		}
		#endregion

		#region private void EditorSurface_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		private void EditorSurface_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			// Finish selecting items
			if (bSelecting == true)
			{
				this.bSelecting = false;
				this.Refresh();

				foreach(SelectableAndMovableControl smac in this.Objects)
				{
					if (smac.Selected == true)
					{
						if (!this.SelectedObjects.Contains(smac))
							this.SelectedObjects.Add(smac);
					}
				}

				this.OnSelectionChanged(this, EventArgs.Empty);
			}
		}
		#endregion

		#region protected void EditorSurface_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		protected void EditorSurface_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			Graphics g = e.Graphics;

			if (PetriNetDocument.AntiAlias == true)
				g.SmoothingMode = SmoothingMode.AntiAlias;

			#region if (bConnecting == true)
			// Draw currently dragged line
			if (bConnecting == true)
			{
				Pen pBlack = new Pen(Color.Black, this.fZoomLevel * 3);
				pBlack.CustomEndCap = new AdjustableArrowCap(3, 6, true);
				Pen pYellow = new Pen( Color.Yellow, this.fZoomLevel *3);
				pYellow.CustomEndCap = new AdjustableArrowCap(3, 6, true);
				Pen pRed = new Pen( Color.Red, this.fZoomLevel *3);
				pRed.CustomEndCap = new AdjustableArrowCap(3, 6, true);

				// Draw line as user moves the mouse pointer
				Control cBegin = this.GetChildAtPoint(this.ptDragBegin);
				Control cEnd = this.GetChildAtPoint(this.ptDragEnd);

				if (cBegin is ConnectableControl)
				{
					if (cEnd is ConnectableControl)
					{
						Point ptEndPoint = this.PointToScreen(this.ptDragEnd);
						ptEndPoint = cEnd.PointToClient(ptEndPoint);

						ConnectableControl ccBegin = (ConnectableControl)cBegin;
						ConnectableControl ccEnd = (ConnectableControl)cEnd;

						if (ccBegin.Childs.Contains(cEnd))
							g.DrawLine(pRed, this.ptDragBegin, this.ptDragEnd);
						else
						{
							if (ccBegin.CanConnectTo(cEnd) && ccEnd.CanConnectToThisPoint(ptEndPoint))
								g.DrawLine(pYellow, this.ptDragBegin, this.ptDragEnd);
							else
								g.DrawLine(pBlack, this.ptDragBegin, this.ptDragEnd);
						}
					}
					else
						g.DrawLine(pBlack, this.ptDragBegin, this.ptDragEnd);
				}
			}
			#endregion

			#region if (bSelecting == true)
			// Draw selecting rectangle
			if (bSelecting == true)
			{
				Brush b = new SolidBrush(Color.FromArgb(70, Color.FromArgb(49, 106, 197)));
				Pen p = new Pen(Color.FromArgb(120, SystemColors.HotTrack), 1f);
				// I quadrant
				if (this.meaLastMouseDown.X <= this.ptDragEnd.X && this.meaLastMouseDown.Y > this.ptDragEnd.Y)
				{
					g.FillRectangle(b, this.meaLastMouseDown.X, this.ptDragEnd.Y, this.ptDragEnd.X - this.meaLastMouseDown.X, this.meaLastMouseDown.Y - this.ptDragEnd.Y);
					g.DrawRectangle(p, this.meaLastMouseDown.X, this.ptDragEnd.Y, this.ptDragEnd.X - this.meaLastMouseDown.X, this.meaLastMouseDown.Y - this.ptDragEnd.Y);
				}
					// II quadrant
				else if (this.meaLastMouseDown.X > this.ptDragEnd.X && this.meaLastMouseDown.Y > this.ptDragEnd.Y)
				{
					g.FillRectangle(b, this.ptDragEnd.X, this.ptDragEnd.Y, this.meaLastMouseDown.X - this.ptDragEnd.X, this.meaLastMouseDown.Y - this.ptDragEnd.Y);
					g.DrawRectangle(p, this.ptDragEnd.X, this.ptDragEnd.Y, this.meaLastMouseDown.X - this.ptDragEnd.X, this.meaLastMouseDown.Y - this.ptDragEnd.Y);
				}
					// III quadrant
				else if (this.meaLastMouseDown.X > this.ptDragEnd.X && this.meaLastMouseDown.Y <= this.ptDragEnd.Y)
				{
					g.FillRectangle(b, this.ptDragEnd.X, this.meaLastMouseDown.Y, this.meaLastMouseDown.X - this.ptDragEnd.X, this.ptDragEnd.Y - this.meaLastMouseDown.Y);
					g.DrawRectangle(p, this.ptDragEnd.X, this.meaLastMouseDown.Y, this.meaLastMouseDown.X - this.ptDragEnd.X, this.ptDragEnd.Y - this.meaLastMouseDown.Y);
				}
					// IV quadrant
				else if (this.meaLastMouseDown.X <= this.ptDragEnd.X && this.meaLastMouseDown.Y <= this.ptDragEnd.Y)
				{
					g.FillRectangle(b, this.meaLastMouseDown.X, this.meaLastMouseDown.Y, this.ptDragEnd.X - this.meaLastMouseDown.X, this.ptDragEnd.Y - this.meaLastMouseDown.Y);
					g.DrawRectangle(p, this.meaLastMouseDown.X, this.meaLastMouseDown.Y, this.ptDragEnd.X - this.meaLastMouseDown.X, this.ptDragEnd.Y - this.meaLastMouseDown.Y);
				}
			}
			#endregion
		}
		#endregion

		#region protected void DeselectControls()
		protected void DeselectControls()
		{
			// Deselect currently selected control
			if (this.SelectedObjects.Count != 0)
			{
				foreach(ISelectable s in this.SelectedObjects)
				{
					s.Selected = false;
				}

				this.SelectedObjects.Clear();
			}
		}
		#endregion

		#region protected void OnSelectionChanged(object sender, EventArgs e)
		protected void OnSelectionChanged(object sender, EventArgs e)
		{
			if (this.SelectedObjectsChanged != null)
				this.SelectedObjectsChanged(this, EventArgs.Empty);
		}
		#endregion

		#region protected void OnContentsChanged(object sender, EventArgs e)
		protected void OnContentsChanged(object sender, EventArgs e)
		{
			if (this.ContentsChanged != null)
				this.ContentsChanged(sender, EventArgs.Empty);
		}
		#endregion

		#region private void ZoomAdjust(float fOldZoomValue, float fNewZoomValue)
		private void ZoomAdjust(float fOldZoomValue, float fNewZoomValue)
		{
			// Adjust all childs
			float fZoomRatio = 1f / fOldZoomValue * fNewZoomValue;

			this.fZoomLevel = fNewZoomValue;
			
			this.SuspendLayout();

			foreach(SelectableAndMovableControl smac in this.Objects)
			{
				smac.Location = new Point((int)(smac.Location.X * fZoomRatio), (int)(smac.Location.Y * fZoomRatio));
				smac.Size = new Size((int)(smac.DefaultSize.Width * fNewZoomValue), (int)(smac.DefaultSize.Height * fNewZoomValue));
			}
		
			this.ResumeLayout();

			// Refresh
			this.Invalidate(true);			
		}
		#endregion



	}
}
