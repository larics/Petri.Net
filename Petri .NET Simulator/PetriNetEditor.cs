using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Design;
using System.Data;
using System.Windows.Forms;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace PetriNetSimulator2
{
	/// <summary>
	/// Summary description for PetriNetEditor.
	/// </summary>
	public class PetriNetEditor : EditorSurface, IUndoRedo
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>

		// Properties

		#region public bool ShowWeight1
		public bool ShowWeight1
		{
			get
			{
				return this.bShowWeight1;
			}
			set
			{
				this.bShowWeight1 = value;
				this.Refresh();
			}
		}
		#endregion

		#region public bool PauseBeforeFiring
		public bool PauseBeforeFiring
		{
			get
			{
				return this.bPauseBeforeFiring;
			}
			set
			{
				this.bPauseBeforeFiring = value;
			}
		}
		#endregion

		#region public ArrayList Connections
		public ArrayList Connections
		{
			get
			{
				return this.alConnections;
			}
		}
		#endregion

		#region public PetriNetDocument Document
		public PetriNetDocument Document
		{
			get
			{
				return this.pndDocument;
			}
		}
		#endregion

		#region public Stack UndoList
		public Stack UndoList
		{
			get
			{
				return this.stackUndoActionList;
			}
		}
		#endregion

		#region public Stack RedoList
		public Stack RedoList
		{
			get
			{
				return this.stackRedoActionList;
			}
		}
		#endregion

		#region public override float Zoom
		public override float Zoom
		{
			get
			{
				return base.Zoom;
			}
			set
			{
				base.Zoom = value;

				// Clear UndoActionList
				this.stackUndoActionList.Clear();

				// Clear RedoActionList
				this.stackRedoActionList.Clear();
			}
		}
		#endregion

                #region public void RefreshMT
                public delegate void InvokeDelegateRefresh();
                public void RefreshMT()
                {
                        BeginInvoke(new InvokeDelegateRefresh(Refresh), null);
                }
                #endregion



		// Events
		internal event EventHandler PropertiesChanged;

		// Fields
		private PetriNetDocument pndDocument = null;
		private bool bShowWeight1 = false;
		private bool bPauseBeforeFiring = false;
		private bool bDragingDroping = false;
		private PetriNetEditorMergeModule pnemmDragged = null;
		private DragEventArgs dea = null;
		
		private ArrayList alConnections = new ArrayList();
		private PetriNetDocumentInstanceCounter pndicInstanceCounter;
		private int iInputInstanceCount = 0;
		private int iOutputInstanceCount = 0;

		private bool bSuppresUndo = false;

		private Stack stackUndoActionList = new Stack();
		private Stack stackRedoActionList = new Stack();

		private System.ComponentModel.Container components = null;

		#region public PetriNetEditor(PetriNetDocument pnd, PetriNetDocumentInstanceCounter pndic)
		public PetriNetEditor(PetriNetDocument pnd, PetriNetDocumentInstanceCounter pndic)
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			this.SetStyle(ControlStyles.UserPaint |	ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer, true);
			this.UpdateStyles();

			this.pndDocument = pnd;
			this.pndicInstanceCounter = pndic;
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PetriNetEditor));
            this.SuspendLayout();
            // 
            // PetriNetEditor
            // 
            this.AllowDrop = true;
            this.BackColor = System.Drawing.SystemColors.ControlDark;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.Name = "PetriNetEditor";
            this.Size = new System.Drawing.Size(678, 482);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.PetriNetEditor_Paint);
            this.DragOver += new System.Windows.Forms.DragEventHandler(this.PetriNetEditor_DragOver);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.PetriNetEditor_MouseMove);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.PetriNetEditor_DragDrop);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.PetriNetEditor_KeyUp);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PetriNetEditor_MouseDown);
            this.DragLeave += new System.EventHandler(this.PetriNetEditor_DragLeave);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.PetriNetEditor_KeyDown);
            this.ResumeLayout(false);

		}
		#endregion


		#region private void PetriNetEditor_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		private void PetriNetEditor_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			this.ptDragEnd = new Point(e.X, e.Y);
			this.bSelecting = true;

			if (this.meaLastMouseDown.Button == MouseButtons.Left)
			{
				// Select topmost connection
				Connection cnLast = null;

				// Iterate through all connections to find clicked one
				foreach(Connection cn in this.Connections)
				{
					if(meaLastMouseDown.X >= cn.Bounds.X && meaLastMouseDown.X <= (cn.Bounds.X + cn.Bounds.Width) && meaLastMouseDown.Y >= cn.Bounds.Y && meaLastMouseDown.Y <= (cn.Bounds.Y + cn.Bounds.Height))
					{
						// Check if there is a connection bellow
						GraphicsPath gp = new GraphicsPath();
						gp.AddEllipse(meaLastMouseDown.X-2, meaLastMouseDown.Y-2, 4, 4);

						Region r = new Region(gp);
						r.Intersect(cn.ConnectionRegion);
		
						RectangleF rf = r.GetBounds(this.CreateGraphics());
						if (rf.Height != 0 || rf.Width != 0)
						{
							cnLast = cn;
						}
					}
				}

				if (cnLast != null)
				{
					this.ctrl_SelectionChanged(cnLast, new SelectionEventArgs(this.kPressedKey));
				}
				else
				{
					this.OnSelectionChanged(this, EventArgs.Empty);
				}
			}			
			
		}
		#endregion

		#region private void PetriNetEditor_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
		private void PetriNetEditor_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (this.bSelecting == true)
			{
				if (e.Button == MouseButtons.Left)
				{
					Point ptNewScrollPosition = new Point(0,0);
					Panel pnl = (Panel)this.Parent;
					if (ptDragEnd.X < -pnl.AutoScrollPosition.X || ptDragEnd.Y < -pnl.AutoScrollPosition.Y)
					{
						ptNewScrollPosition = new Point(Math.Min(ptDragEnd.X, -pnl.AutoScrollPosition.X), Math.Min(ptDragEnd.Y, -pnl.AutoScrollPosition.Y));
						pnl.AutoScrollPosition = ptNewScrollPosition;
					}
					else if (ptDragEnd.X > (pnl.Width-25-pnl.AutoScrollPosition.X) || ptDragEnd.Y > (pnl.Height-15-pnl.AutoScrollPosition.Y))
					{
						ptNewScrollPosition = new Point(Math.Max(ptDragEnd.X-pnl.Width+25, -pnl.AutoScrollPosition.X), Math.Max(ptDragEnd.Y-pnl.Height+15, -pnl.AutoScrollPosition.Y));
						pnl.AutoScrollPosition = ptNewScrollPosition;
					}
				}
			}
		}
		#endregion

		#region private void PetriNetEditor_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		private void PetriNetEditor_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			Graphics g = e.Graphics;

			if (PetriNetDocument.AntiAlias == true)
				g.SmoothingMode = SmoothingMode.AntiAlias;

			// Draw connections for every Place
			foreach(Connection cn in this.Connections)
			{
				cn.DrawConnection(g, this);
			}

			#region if (bDragingDroping == true)
			if (bDragingDroping == true)
			{
				Point pt = PointToClient(new Point(dea.X, dea.Y));

				Color cInactiveColor = Color.FromArgb(80, 97, 190, 103);
				Pen pBlack = new Pen(Color.FromArgb(80, Color.Black), this.fZoomLevel * 3);
				Brush bBlack = new SolidBrush(Color.FromArgb(80, Color.Black));

				foreach(object o in this.pnemmDragged.Objects)
				{
					SelectableAndMovableControl smac = (SelectableAndMovableControl)o;

					Rectangle r = new Rectangle(pt.X + (int)((smac.Location.X + 2 - smac.DefaultSize.Width/2) * this.fZoomLevel) , pt.Y + (int)((smac.Location.Y + 2 - smac.DefaultSize.Height/2) * this.fZoomLevel), (int)(smac.DefaultSize.Width * this.fZoomLevel), (int)(smac.DefaultSize.Height * this.fZoomLevel));
					LinearGradientBrush lgb = new LinearGradientBrush(r, Color.FromArgb(80, Color.White), cInactiveColor, LinearGradientMode.ForwardDiagonal);

					if (o is Place)
					{
						Place p = (Place)o;

						Pen pSign = new Pen(Color.FromArgb(30, Color.Black), 3 * this.fZoomLevel);
						Point p0 = new Point(pt.X + (int)((smac.Location.X + 2 - smac.DefaultSize.Width/2) * this.fZoomLevel), pt.Y + (int)((smac.Location.Y + 2 - smac.DefaultSize.Height/2) * this.fZoomLevel));

						g.FillEllipse(lgb, pt.X + (int)((p.Location.X + 2 - p.DefaultSize.Width/2) * this.fZoomLevel) , pt.Y + (int)((p.Location.Y + 2 - p.DefaultSize.Height/2) * this.fZoomLevel), (int)((p.DefaultSize.Width - 4) * this.fZoomLevel), (int)((p.DefaultSize.Height - 4) * this.fZoomLevel));

						#region Draw Signs
						if (p is PlaceInput)
						{
							Point[] pa = new Point[4]
							{
								new Point((int)(p0.X + 52 * this.fZoomLevel), (int)(p0.Y + 28 * this.fZoomLevel)),
								new Point((int)(p0.X + 62 * this.fZoomLevel), (int)(p0.Y + 28 * this.fZoomLevel)),
								new Point((int)(p0.X + 62 * this.fZoomLevel), (int)(p0.Y + 42 * this.fZoomLevel)),
								new Point((int)(p0.X + 52 * this.fZoomLevel), (int)(p0.Y + 42 * this.fZoomLevel)),
							};

							g.DrawLines(pSign, pa);

							pSign.CustomEndCap = new AdjustableArrowCap(4 * this.fZoomLevel, 3 * this.fZoomLevel, true);
							pSign.Width = (int)(2 * this.fZoomLevel);
							g.DrawLine(pSign, new Point((int)(p0.X + 50 * this.fZoomLevel), (int)(p0.Y + 35 * this.fZoomLevel)), new Point((int)(p0.X + 60 * this.fZoomLevel), (int)(p0.Y + 35 * this.fZoomLevel)));
						}
						else if (p is PlaceOperation)
						{
							StringFormat sfo = new StringFormat();
							sfo.LineAlignment = StringAlignment.Center;
							sfo.Alignment = StringAlignment.Center;

							Font fo = new Font(this.Parent.Font.FontFamily, this.fZoomLevel * 10f,  FontStyle.Bold);

							g.DrawString("O", fo, new SolidBrush(pSign.Color), new Rectangle(new Point((int)(p0.X + 38 * this.fZoomLevel), p0.Y - 2), new Size((int)((int)((p.DefaultSize.Width - 38) * this.fZoomLevel)), (int)(p.DefaultSize.Height * this.fZoomLevel))), sfo);
						}
						else if (p is PlaceResource)
						{
							StringFormat sfo = new StringFormat();
							sfo.LineAlignment = StringAlignment.Center;
							sfo.Alignment = StringAlignment.Center;

							Font fo = new Font(this.Parent.Font.FontFamily, this.fZoomLevel * 10f,  FontStyle.Bold);

							g.DrawString("R", fo, new SolidBrush(pSign.Color), new Rectangle(new Point((int)(p0.X + 38 * this.fZoomLevel), p0.Y - 2), new Size((int)((int)((p.DefaultSize.Width - 38) * this.fZoomLevel)), (int)(p.DefaultSize.Height * this.fZoomLevel))), sfo);
						}
						else if (p is PlaceControl)
						{
							StringFormat sfo = new StringFormat();
							sfo.LineAlignment = StringAlignment.Center;
							sfo.Alignment = StringAlignment.Center;

							Font fo = new Font(this.Parent.Font.FontFamily, this.fZoomLevel * 10f,  FontStyle.Bold);

							g.DrawEllipse(new Pen(Color.FromArgb(80, Color.Black), this.fZoomLevel * 2), pt.X + (int)((p.Location.X + 6 - p.DefaultSize.Width/2) * this.fZoomLevel) , pt.Y + (int)((p.Location.Y + 6 - p.DefaultSize.Height/2) * this.fZoomLevel), (int)((p.DefaultSize.Width - 12) * this.fZoomLevel), (int)((p.DefaultSize.Height - 12) * this.fZoomLevel));
						}
						else if (p is PlaceOutput)
						{
							Point[] pa = new Point[4]
							{
								new Point((int)(p0.X + 62 * this.fZoomLevel), (int)(p0.Y + 28 * this.fZoomLevel)),
								new Point((int)(p0.X + 52 * this.fZoomLevel), (int)(p0.Y + 28 * this.fZoomLevel)),
								new Point((int)(p0.X + 52 * this.fZoomLevel), (int)(p0.Y + 42 * this.fZoomLevel)),
								new Point((int)(p0.X + 62 * this.fZoomLevel), (int)(p0.Y + 42 * this.fZoomLevel)),
							};
							g.DrawLines(pSign, pa);

							pSign.CustomEndCap = new AdjustableArrowCap(4 * this.fZoomLevel, 3 * this.fZoomLevel, true);
							pSign.Width = (int)(2 * this.fZoomLevel);
							g.DrawLine(pSign, new Point((int)(p0.X + 55 * this.fZoomLevel), (int)(p0.Y + 35 * this.fZoomLevel)), new Point((int)(p0.X + 65 * this.fZoomLevel), (int)(p0.Y + 35 * this.fZoomLevel)));
						}
						#endregion

						g.DrawEllipse(pBlack, pt.X + (int)((p.Location.X + 2 - p.DefaultSize.Width/2) * this.fZoomLevel) , pt.Y + (int)((p.Location.Y + 2 - p.DefaultSize.Height/2) * this.fZoomLevel), (int)((p.DefaultSize.Width - 4) * this.fZoomLevel), (int)((p.DefaultSize.Height - 4) * this.fZoomLevel));

						#region Draw tokens
						if (p.Tokens == 1)
						{
							g.FillEllipse(bBlack, p0.X - 2 + this.fZoomLevel * (p.DefaultSize.Width/2 - 4), p0.Y - 2 + this.fZoomLevel * (p.DefaultSize.Height/2 - 4), this.fZoomLevel * 8, this.fZoomLevel * 8);
						}
						else if (p.Tokens == 2)
						{
							g.FillEllipse(bBlack, p0.X - 2 + this.fZoomLevel * (p.DefaultSize.Width/2 - 4),  p0.Y - 2 + this.fZoomLevel * (p.DefaultSize.Height/2 - 10), this.fZoomLevel * 8, this.fZoomLevel * 8);
							g.FillEllipse(bBlack, p0.X - 2 + this.fZoomLevel * (p.DefaultSize.Width/2 - 4),  p0.Y - 2 + this.fZoomLevel * (p.DefaultSize.Height/2 + 2), this.fZoomLevel * 8, this.fZoomLevel * 8);
						}
						else if (p.Tokens == 3)
						{
							g.FillEllipse(bBlack, p0.X - 2 + this.fZoomLevel * (p.DefaultSize.Width/2 - 4), p0.Y - 2 + this.fZoomLevel * (p.DefaultSize.Height/2 - 10), this.fZoomLevel * 8, this.fZoomLevel * 8);
							g.FillEllipse(bBlack, p0.X - 2 + this.fZoomLevel * (p.DefaultSize.Width/2 - 10), p0.Y - 2 + this.fZoomLevel * (p.DefaultSize.Height/2 + 2), this.fZoomLevel * 8, this.fZoomLevel * 8);
							g.FillEllipse(bBlack, p0.X - 2 + this.fZoomLevel * (p.DefaultSize.Width/2 + 2), p0.Y - 2 + this.fZoomLevel * (p.DefaultSize.Height/2 + 2), this.fZoomLevel * 8, this.fZoomLevel * 8);
						}
						else if (p.Tokens == 4)
						{
							g.FillEllipse(bBlack, p0.X - 2 + this.fZoomLevel * (p.DefaultSize.Width/2 - 10), p0.Y - 2 + this.fZoomLevel * (p.DefaultSize.Height/2 - 10), this.fZoomLevel * 8, this.fZoomLevel * 8);
							g.FillEllipse(bBlack, p0.X - 2 + this.fZoomLevel * (p.DefaultSize.Width/2 + 2), p0.Y - 2 + this.fZoomLevel * (p.DefaultSize.Height/2 - 10), this.fZoomLevel * 8, this.fZoomLevel * 8);
							g.FillEllipse(bBlack, p0.X - 2 + this.fZoomLevel * (p.DefaultSize.Width/2 - 10), p0.Y - 2 + this.fZoomLevel * (p.DefaultSize.Height/2 + 2), this.fZoomLevel * 8, this.fZoomLevel * 8);
							g.FillEllipse(bBlack, p0.X - 2 + this.fZoomLevel * (p.DefaultSize.Width/2 + 2), p0.Y - 2 + this.fZoomLevel * (p.DefaultSize.Height/2 + 2), this.fZoomLevel * 8, this.fZoomLevel * 8);
						}
						else if (p.Tokens < 0 || p.Tokens > 4)
						{
							Font fTokens = new Font(this.Parent.Font.FontFamily, this.fZoomLevel * 12f, FontStyle.Bold);
							StringFormat sf = new StringFormat();
							sf.LineAlignment = StringAlignment.Center;
							sf.Alignment = StringAlignment.Center;
							g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
							g.DrawString(p.Tokens.ToString(), fTokens, bBlack, new RectangleF(new PointF(p0.X - 2, p0.Y - 2 + this.fZoomLevel * 4f), new SizeF(p.DefaultSize.Width * this.fZoomLevel, (p.DefaultSize.Height - 2f) * this.fZoomLevel)), sf);
						}
						#endregion
					}
					else if (o is Transition)
					{
						Transition t = (Transition)o;

						g.FillRectangle(lgb, pt.X + (int)((t.Location.X + 2 - t.DefaultSize.Width/2) * this.fZoomLevel), pt.Y + (int)((t.Location.Y + 2 - t.DefaultSize.Height/2) * this.fZoomLevel), (int)((t.DefaultSize.Width - 4) * this.fZoomLevel), (int)((t.DefaultSize.Height - 4) * this.fZoomLevel));
						g.DrawRectangle(pBlack, pt.X + (int)((t.Location.X + 2 - t.DefaultSize.Width/2) * this.fZoomLevel), pt.Y + (int)((t.Location.Y + 2 - t.DefaultSize.Height/2) * this.fZoomLevel), (int)((t.DefaultSize.Width - 4) * this.fZoomLevel), (int)((t.DefaultSize.Height - 4) * this.fZoomLevel));
					}
					else if (o is Subsystem)
					{
						Subsystem s = (Subsystem)o;

						g.FillRectangle(lgb, pt.X + (int)((s.Location.X + 2 - s.DefaultSize.Width/2) * this.fZoomLevel), pt.Y + (int)((s.Location.Y + 2 - s.DefaultSize.Height/2) * this.fZoomLevel), (int)((s.DefaultSize.Width - 4) * this.fZoomLevel), (int)((s.DefaultSize.Height - 4) * this.fZoomLevel));
						g.DrawRectangle(pBlack, pt.X + (int)((s.Location.X + 2 - s.DefaultSize.Width/2) * this.fZoomLevel), pt.Y + (int)((s.Location.Y + 2 - s.DefaultSize.Height/2) * this.fZoomLevel), (int)((s.DefaultSize.Width - 4) * this.fZoomLevel), (int)((s.DefaultSize.Height - 4) * this.fZoomLevel));
					}
					else if (o is DescriptionLabel)
					{
						DescriptionLabel dl = (DescriptionLabel)o;

						Pen pBlackLabel = new Pen(Color.FromArgb(80, Color.Black), this.fZoomLevel * 1.7f);

						g.FillRectangle(lgb, pt.X + (int)((dl.Location.X + 2 - dl.DefaultSize.Width/2) * this.fZoomLevel), pt.Y + (int)((dl.Location.Y + 2 - dl.DefaultSize.Height/2) * this.fZoomLevel), (int)((dl.DefaultSize.Width - 4) * this.fZoomLevel), (int)((dl.DefaultSize.Height - 4) * this.fZoomLevel));
						g.DrawRectangle(pBlackLabel, pt.X + (int)((dl.Location.X + 2 - dl.DefaultSize.Width/2) * this.fZoomLevel), pt.Y + (int)((dl.Location.Y + 2 - dl.DefaultSize.Height/2) * this.fZoomLevel), (int)((dl.DefaultSize.Width - 4) * this.fZoomLevel), (int)((dl.DefaultSize.Height - 4) * this.fZoomLevel));
					}
					else if (o is Input || o is Output)
					{
						SelectableAndMovableControl smacc = (SelectableAndMovableControl)o;

						Pen pBlackLabel = new Pen(Color.FromArgb(80, Color.Black), this.fZoomLevel * 1.7f);

						g.FillEllipse(lgb, pt.X + (int)((smacc.Location.X + 2 - smacc.DefaultSize.Width/2) * this.fZoomLevel), pt.Y + (int)((smacc.Location.Y + 2 - smacc.DefaultSize.Height/2) * this.fZoomLevel), (int)((smacc.DefaultSize.Width - 4) * this.fZoomLevel), (int)((smacc.DefaultSize.Height - 4) * this.fZoomLevel));
						g.DrawEllipse(pBlack, pt.X + (int)((smacc.Location.X + 2 - smacc.DefaultSize.Width/2) * this.fZoomLevel) , pt.Y + (int)((smacc.Location.Y + 2 - smacc.DefaultSize.Height/2) * this.fZoomLevel), (int)((smacc.DefaultSize.Width - 4) * this.fZoomLevel), (int)((smacc.DefaultSize.Height - 4) * this.fZoomLevel));
					}
				}

				// Draw connections while dragging
//				foreach(Connection cn in this.pnemmDragged.Connections)
//				{
//
//				}
			}
			#endregion

			base.EditorSurface_Paint(sender, e);
		}
		#endregion


		#region private void AddControl(SelectableAndMovableControl smac)
		private void AddControl(SelectableAndMovableControl smac)
		{
			smac.Size = new Size((int)(smac.DefaultSize.Width * this.fZoomLevel), (int)(smac.DefaultSize.Height * this.fZoomLevel));

			if (smac is Place)
			{
				Place p = (Place)smac;
				p.Index = pndicInstanceCounter.IncreasePlaceInstanceCount();
			}
			else if (smac is Transition)
			{
				Transition t = (Transition)smac;
				t.Index = pndicInstanceCounter.IncreaseTransitionInstanceCount();
			}
			else if (smac is Subsystem)
			{
				Subsystem s = (Subsystem)smac;
				s.Index = pndicInstanceCounter.IncreaseSubsystemInstanceCount();
			}
			else if (smac is DescriptionLabel)
			{
				DescriptionLabel dl = (DescriptionLabel)smac;
				dl.Index = pndicInstanceCounter.IncreaseDescriptionLabelInstanceCount();
			}
			else if (smac is Input)
			{
				Input i = (Input)smac;
				i.Index = ++iInputInstanceCount;
			}
			else if (smac is Output)
			{
				Output ot = (Output)smac;
				ot.Index = ++iOutputInstanceCount;
			}

			this.Controls.Add(smac);

			smac.BringToFront();

			this.Objects.Add(smac);
		}
		#endregion

		#region private void AddMergeModule(PetriNetEditorMergeModule pnemm)
		private void AddMergeModule(PetriNetEditorMergeModule pnemm)
		{
			this.SuspendLayout();

			ArrayList alNewObjects = new ArrayList();

			Hashtable ht = new Hashtable(); // Used to restore connections

			foreach(object o in pnemm.Objects)
			{
				if (o is Place)
				{
					Place p = (Place)((Place)o).Clone();
					ht.Add(o, p);
					p.Location = new Point((int)(this.meaLastMouseDown.X + (p.Location.X - p.DefaultSize.Width / 2) * this.fZoomLevel), (int)(this.meaLastMouseDown.Y + (p.Location.Y - p.DefaultSize.Height / 2) * this.fZoomLevel));

					this.AddControl(p);
					alNewObjects.Add(p);

					this.AttachControlToEditor(p);
				}
				else if (o is Transition)
				{
					Transition t = (Transition)((Transition)o).Clone();
					ht.Add(o, t);
					t.Location = new Point((int)(this.meaLastMouseDown.X + (t.Location.X - t.DefaultSize.Width / 2) * this.fZoomLevel), (int)(this.meaLastMouseDown.Y + (t.Location.Y - t.DefaultSize.Height / 2) * this.fZoomLevel));

					this.AddControl(t);
					alNewObjects.Add(t);

					this.AttachControlToEditor(t);
				}
				else if (o is Subsystem)
				{
					Subsystem s = (Subsystem)((Subsystem)o).Clone();
					ht.Add(o, s);
					s.Location = new Point((int)(this.meaLastMouseDown.X + (s.Location.X - s.DefaultSize.Width / 2) * this.fZoomLevel), (int)(this.meaLastMouseDown.Y + (s.Location.Y - s.DefaultSize.Height / 2) * this.fZoomLevel));

					s.SuppresDeserializationCall();
					this.AddControl(s);
					alNewObjects.Add(s);

					this.AttachControlToEditor(s);
					s.EnableDeserializationCall();
					
					// Adjust connectable regions of all inside subsystems
					// because this cannot be done before subsystems are attached
					// to PetriNetEditor
					s.AdjustConnectableRegions();
					foreach(Subsystem ss in this.pndDocument.Subsystems)
					{
						ss.AdjustConnectableRegions();
					}
				}
				else if (o is DescriptionLabel)
				{
					DescriptionLabel dl = (DescriptionLabel)((DescriptionLabel)o).Clone();
					dl.Location = new Point((int)(this.meaLastMouseDown.X + (dl.Location.X - dl.DefaultSize.Width / 2) * this.fZoomLevel), (int)(this.meaLastMouseDown.Y + (dl.Location.Y - dl.DefaultSize.Height / 2) * this.fZoomLevel));

					this.AddControl(dl);
					alNewObjects.Add(dl);

					this.AttachControlToEditor(dl);
				}
				else if (o is Input)
				{
					Input i = (Input)((Input)o).Clone();
					ht.Add(o, i);
					i.Location = new Point((int)(this.meaLastMouseDown.X + (i.Location.X - i.DefaultSize.Width / 2) * this.fZoomLevel), (int)(this.meaLastMouseDown.Y + (i.Location.Y - i.DefaultSize.Height / 2) * this.fZoomLevel));

					this.AddControl(i);
					alNewObjects.Add(i);

					this.AttachControlToEditor(i);
				}
				else if (o is Output)
				{
					Output ot = (Output)((Output)o).Clone();
					ht.Add(o, ot);
					ot.Location = new Point((int)(this.meaLastMouseDown.X + (ot.Location.X - ot.DefaultSize.Width / 2) * this.fZoomLevel), (int)(this.meaLastMouseDown.Y + (ot.Location.Y - ot.DefaultSize.Height / 2) * this.fZoomLevel));

					this.AddControl(ot);
					alNewObjects.Add(ot);

					this.AttachControlToEditor(ot);
				}
			}

			foreach(Connection cn in pnemm.Connections)
			{
				Connection cnNew = (Connection)cn.Clone((ConnectableControl)ht[cn.From], (ConnectableControl)ht[cn.To]);
				
				((ConnectableControl)ht[cn.From]).Childs.Add(ht[cn.To]);
				((ConnectableControl)ht[cn.To]).Parents.Add(ht[cn.From]);

				cnNew.SelectionChanged += new SelectionHandler(ctrl_SelectionChanged);

				this.Connections.Add(cnNew);
				alNewObjects.Add(cnNew);
			}

			this.SelectObjects(alNewObjects);
			this.ResumeLayout();

			// Push UndoRedoItem to stack
			this.stackUndoActionList.Push(new UndoRedoItem(alNewObjects, this, UndoRedoAction.Created, null));
			// Clear RedoActionList
			this.stackRedoActionList.Clear();

			this.OnContentsChanged(this, EventArgs.Empty);
		}
		#endregion


		#region public void AttachControlToEditor(object o)
		public void AttachControlToEditor(object o)
		{
			if (o is SelectableAndMovableControl)
			{
				SelectableAndMovableControl smac = (SelectableAndMovableControl)o;

				KeyEventHandler keh = new KeyEventHandler(PetriNetEditor_KeyDown);
				smac.KeyDown += keh;
				smac.EventHandlers.Add("KeyDown", keh);

				SelectionHandler sh = new SelectionHandler(ctrl_SelectionChanged);
				smac.SelectionChanged += sh;
				smac.EventHandlers.Add("SelectionChanged", sh);

				ResizedEventHandler reh = new ResizedEventHandler(ctrl_Resized);
				smac.Resized += reh;
				smac.EventHandlers.Add("Resized", reh);

				LocationChangedEventHandler lceh = new LocationChangedEventHandler(ctrl_LocationChanged);
				smac.LocationChanged += lceh;
				smac.EventHandlers.Add("LocationChanged", lceh);
			}

			if (o is Place)
			{
				Place p = (Place)o;

				EventHandler eh = new EventHandler(ctrl_PropertiesChanged);
				p.PropertiesChanged += eh;
				p.EventHandlers.Add("PropertiesChanged", eh);

				eh = new EventHandler(ctrl_NameOrIndexChanged);
				p.NameOrIndexChanged += eh;
				p.EventHandlers.Add("NameOrIndexChanged", eh);

				eh = new EventHandler(ctrl_ConnectionCreated);
				p.ConnectionCreated += eh;
				p.EventHandlers.Add("ConnectionCreated", eh);
			}
			else if (o is Transition)
			{
				Transition t = (Transition)o;

				EventHandler eh = new EventHandler(ctrl_PropertiesChanged);
				t.PropertiesChanged += eh;
				t.EventHandlers.Add("PropertiesChanged", eh);

				eh = new EventHandler(ctrl_ConnectionCreated);
				t.ConnectionCreated += eh;
				t.EventHandlers.Add("ConnectionCreated", eh);
			}
			else if (o is Subsystem)
			{
				Subsystem s = (Subsystem)o;

				EventHandler eh = new EventHandler(ctrl_PropertiesChanged);
				s.PropertiesChanged += eh;
				s.EventHandlers.Add("PropertiesChanged", eh);

				eh = new EventHandler(ctrl_NameOrIndexChanged);
				s.NameOrIndexChanged += eh;
				s.EventHandlers.Add("NameOrIndexChanged", eh);

				eh = new EventHandler(ctrl_ConnectionCreated);
				s.ConnectionCreated += eh;
				s.EventHandlers.Add("ConnectionCreated", eh);
			}
			else if (o is DescriptionLabel)
			{
				DescriptionLabel dl = (DescriptionLabel)o;

				EventHandler eh = new EventHandler(ctrl_PropertiesChanged);
				dl.PropertiesChanged += eh;
				dl.EventHandlers.Add("PropertiesChanged", eh);

				eh = new EventHandler(ctrl_NameOrIndexChanged);
				dl.LabelTextChanged += eh;
				dl.EventHandlers.Add("LabelTextChanged", eh);
			}
			else if (o is Input)
			{
				Input i = (Input)o;

				EventHandler eh = new EventHandler(ctrl_PropertiesChanged);
				i.PropertiesChanged += eh;
				i.EventHandlers.Add("PropertiesChanged", eh);

				eh = new EventHandler(ctrl_NameOrIndexChanged);
				i.IndexChanged += eh;
				i.EventHandlers.Add("IndexChanged", eh);

				eh = new EventHandler(ctrl_ConnectionCreated);
				i.ConnectionCreated += eh;
				i.EventHandlers.Add("ConnectionCreated", eh);
			}
			else if (o is Output)
			{
				Output ot = (Output)o;

				EventHandler eh = new EventHandler(ctrl_PropertiesChanged);
				ot.PropertiesChanged += eh;
				ot.EventHandlers.Add("PropertiesChanged", eh);

				eh = new EventHandler(ctrl_NameOrIndexChanged);
				ot.IndexChanged += eh;
				ot.EventHandlers.Add("IndexChanged", eh);

				eh = new EventHandler(ctrl_ConnectionCreated);
				ot.ConnectionCreated += eh;
				ot.EventHandlers.Add("ConnectionCreated", eh);
			}
		}
		#endregion

		#region public void DettachControlFromEditor(object o)
		public void DettachControlFromEditor(object o)
		{
			if (o is Place)
			{
				Place p = (Place)o;

				p.PropertiesChanged -= (EventHandler)p.EventHandlers["PropertiesChanged"];
				p.NameOrIndexChanged -= (EventHandler)p.EventHandlers["NameOrIndexChanged"];
				p.ConnectionCreated -= (EventHandler)p.EventHandlers["ConnectionCreated"];;
			}
			else if (o is Transition)
			{
				Transition t = (Transition)o;

				t.PropertiesChanged -= (EventHandler)t.EventHandlers["PropertiesChanged"];
				t.ConnectionCreated -= (EventHandler)t.EventHandlers["ConnectionCreated"];;
			}
			else if (o is Subsystem)
			{
				Subsystem s = (Subsystem)o;

				s.PropertiesChanged -= (EventHandler)s.EventHandlers["PropertiesChanged"];
				s.NameOrIndexChanged -= (EventHandler)s.EventHandlers["NameOrIndexChanged"];
				s.ConnectionCreated -= (EventHandler)s.EventHandlers["ConnectionCreated"];;
			}
			else if (o is DescriptionLabel)
			{
				DescriptionLabel dl = (DescriptionLabel)o;

				dl.PropertiesChanged -= (EventHandler)dl.EventHandlers["PropertiesChanged"];
				dl.LabelTextChanged -= (EventHandler)dl.EventHandlers["LabelTextChanged"];
			}
			else if (o is Input)
			{
				Input i = (Input)o;

				i.PropertiesChanged -= (EventHandler)i.EventHandlers["PropertiesChanged"];
				i.IndexChanged -= (EventHandler)i.EventHandlers["IndexChanged"];
				i.ConnectionCreated -= (EventHandler)i.EventHandlers["ConnectionCreated"];;
			}
			else if (o is Output)
			{
				Output ot = (Output)o;

				ot.PropertiesChanged -= (EventHandler)ot.EventHandlers["PropertiesChanged"];
				ot.IndexChanged -= (EventHandler)ot.EventHandlers["IndexChanged"];
				ot.ConnectionCreated -= (EventHandler)ot.EventHandlers["ConnectionCreated"];;
			}

			if (o is SelectableAndMovableControl)
			{
				SelectableAndMovableControl smac = (SelectableAndMovableControl)o;

				smac.KeyDown -= (KeyEventHandler)smac.EventHandlers["KeyDown"];
				smac.SelectionChanged -= (SelectionHandler)smac.EventHandlers["SelectionChanged"];
				smac.Resized -= (ResizedEventHandler)smac.EventHandlers["Resized"];
				smac.LocationChanged -= (LocationChangedEventHandler)smac.EventHandlers["LocationChanged"];

				smac.EventHandlers.Clear();
			}
		}
		#endregion


		#region public void DeleteSelectedControls()
		public void DeleteSelectedControls()
		{
			ArrayList alDeletedObjects = new ArrayList();

			foreach(object o in this.SelectedObjects)
			{
				if (o is ConnectableControl)
				{
					ConnectableControl ccControl = (ConnectableControl)o;
		
					// Delete from all parents Childs Collections
					foreach(ConnectableControl cc in ccControl.Parents)
					{
						cc.Childs.Remove(ccControl);
					}
		
					// Delete from all childs Parents Collections
					foreach(ConnectableControl cc in ccControl.Childs)
					{
						cc.Parents.Remove(ccControl);
					}
		
					// Remove connections bound to this Place
					foreach(ConnectableControl cc in ccControl.Childs)
					{
						Connection cn = Connection.GetConnectionBetweenControls(ccControl, cc);
						this.Connections.Remove(cn);
						alDeletedObjects.Add(cn);
					}
					foreach(ConnectableControl cc in ccControl.Parents)
					{
						Connection cn = Connection.GetConnectionBetweenControls(cc, ccControl);
						this.Connections.Remove(cn);
						alDeletedObjects.Add(cn);
					}
					
					this.Controls.Remove(ccControl);
					this.Objects.Remove(ccControl);
					this.DettachControlFromEditor(ccControl);

					ccControl.Selected = false;

					alDeletedObjects.Add(ccControl);
				}

				else if (o is Connection)
				{
					Connection cn = (Connection)o;
					cn.Selected = false;
		
					ConnectableControl ccFrom = cn.From;
					ConnectableControl ccTo = cn.To;
					
					// Dispose connection
					ccFrom.Childs.Remove(ccTo);
					ccTo.Parents.Remove(ccFrom);
		
					this.Connections.Remove(cn);
					alDeletedObjects.Add(cn);
				}
				else
				{
					SelectableAndMovableControl smac = (SelectableAndMovableControl)o;
					
					smac.Selected = false;

					this.Controls.Remove(smac);
					this.Objects.Remove(smac);
					this.DettachControlFromEditor(smac);

					alDeletedObjects.Add(smac);
				}
			}
		
			// Push UndoRedoItem to stack
			if (this.bSuppresUndo != true)
			{
				this.stackUndoActionList.Push(new UndoRedoItem(alDeletedObjects, this, UndoRedoAction.Deleted, null));
			
				// Clear RedoActionList
				this.stackRedoActionList.Clear();
			}

			this.SelectedObjects.Clear();
		
			this.OnContentsChanged(this, EventArgs.Empty);

			this.PerformActivation();

			this.Refresh();
		}
		#endregion

		#region public void SelectAll()
		public void SelectAll()
		{
			this.SelectedObjects.Clear();

			foreach (object o in this.Objects)
			{
				ISelectable i = (ISelectable)o;
				i.Selected = true;

				this.SelectedObjects.Add(o);
			}
			foreach (object o in this.Connections)
			{
				ISelectable i = (ISelectable)o;
				i.Selected = true;

				this.SelectedObjects.Add(o);
			}

			this.Refresh();
			this.OnSelectionChanged(this, EventArgs.Empty);
		}
		#endregion
		
		#region public void SelectObjects(ArrayList al)
		public void SelectObjects(ArrayList al)
		{
			this.SelectedObjects.Clear();

			// Deselect all controls
			foreach (object o in this.Objects)
			{
				ISelectable i = (ISelectable)o;
				i.Selected = false;
			}

			// Deselect all connections
			foreach (object o in this.Connections)
			{
				ISelectable i = (ISelectable)o;
				i.Selected = false;
			}

			// Select new controls
			foreach (object o in al)
			{
				ISelectable i = (ISelectable)o;
				i.Selected = true;

				this.SelectedObjects.Add(o);
			}

			this.Refresh();
			this.OnSelectionChanged(this, EventArgs.Empty);
		}
		#endregion

		#region public PetriNetEditorMergeModule CopySelection()
		public PetriNetEditorMergeModule CopySelection()
		{
			// Returns false indices to prevent from incresing instance counter twice
			this.pndicInstanceCounter.FalseIndicesMode = true;

			PetriNetEditorMergeModule pnemm = new PetriNetEditorMergeModule();

			int iXMin = int.MaxValue;
			int iYMin = int.MaxValue; 
			foreach(object o in this.SelectedObjects)
			{
				if (o is SelectableAndMovableControl)
				{
					SelectableAndMovableControl smc = (SelectableAndMovableControl)o;
					if (smc.Location.X < iXMin)
						iXMin = smc.Location.X;
					if (smc.Location.Y < iYMin)
						iYMin = smc.Location.Y;
				}
			}

			// Add objects to merge module
			foreach(object o in this.SelectedObjects)
			{
				if (o is Place)
				{
					Place p = (Place)((Place)o).Clone();
					p.Location = new Point((int)((p.Location.X - iXMin + p.DefaultSize.Width / 2) / this.fZoomLevel), (int)((p.Location.Y - iYMin + p.DefaultSize.Height / 2) / this.fZoomLevel));
					pnemm.Add(p);
				}
				else if (o is Transition)
				{
					Transition t = (Transition)((Transition)o).Clone();
					t.Location = new Point((int)((t.Location.X - iXMin + t.DefaultSize.Width / 2) / this.fZoomLevel), (int)((t.Location.Y - iYMin + t.DefaultSize.Height / 2) / this.fZoomLevel));
					pnemm.Add(t);
				}
				else if (o is DescriptionLabel)
				{
					DescriptionLabel dl = (DescriptionLabel)((DescriptionLabel)o).Clone();
					dl.Location = new Point((int)((dl.Location.X - iXMin + dl.DefaultSize.Width / 2) / this.fZoomLevel), (int)((dl.Location.Y - iYMin + dl.DefaultSize.Height / 2) / this.fZoomLevel));
					pnemm.Add(dl);
				}
				else if (o is Subsystem)
				{
					Subsystem s = (Subsystem)((Subsystem)o).Clone();
					s.Location = new Point((int)((s.Location.X - iXMin + s.DefaultSize.Width / 2) / this.fZoomLevel), (int)((s.Location.Y - iYMin + s.DefaultSize.Height / 2) / this.fZoomLevel));
					pnemm.Add(s);
				}
				else if (o is Input)
				{
					Input i = (Input)((Input)o).Clone();
					i.Location = new Point((int)((i.Location.X - iXMin + i.DefaultSize.Width / 2) / this.fZoomLevel), (int)((i.Location.Y - iYMin + i.DefaultSize.Height / 2) / this.fZoomLevel));
					pnemm.Add(i);
				}
				else if (o is Output)
				{
					Output op = (Output)((Output)o).Clone();
					op.Location = new Point((int)((op.Location.X - iXMin + op.DefaultSize.Width / 2) / this.fZoomLevel), (int)((op.Location.Y - iYMin + op.DefaultSize.Height / 2) / this.fZoomLevel));
					pnemm.Add(op);
				}
			}

			// Add connections between objects to merge module
			ArrayList alObjects = new ArrayList();
			foreach(object o in this.SelectedObjects)
			{
				if (!(o is Connection))
					alObjects.Add(o);
			}

			for(int i = 0; i < alObjects.Count; i++)
			{
				for (int j = i+1; j < alObjects.Count; j++)
				{
					Connection cn = Connection.GetConnectionBetweenControls(this, (ConnectableControl)alObjects[i], (ConnectableControl)alObjects[j]);
					if (cn != null)
					{
						pnemm.Add(cn.Clone((ConnectableControl)pnemm.Objects[i], (ConnectableControl)pnemm.Objects[j]));
					}

					cn = Connection.GetConnectionBetweenControls(this, (ConnectableControl)alObjects[j], (ConnectableControl)alObjects[i]);
					if (cn != null)
					{
						pnemm.Add(cn.Clone((ConnectableControl)pnemm.Objects[j], (ConnectableControl)pnemm.Objects[i]));
					}

				}
			}

			this.pndicInstanceCounter.FalseIndicesMode = false;

			return pnemm;
		}
		#endregion

		#region public void Paste(PetriNetEditorMergeModule pnemm)
		public void Paste(PetriNetEditorMergeModule pnemm)
		{
			this.meaLastMouseDown = new MouseEventArgs(MouseButtons.Left, 1, (int)(0 * this.fZoomLevel), (int)(0 * this.fZoomLevel), 0);
			this.AddMergeModule(pnemm);
		}
		#endregion

		#region public void Group()
		public void Group()
		{
			// Remember initial controls
			ArrayList alSelectedObjects = (ArrayList)this.SelectedObjects.Clone();

			this.SuspendLayout();

			PetriNetEditorMergeModule pnemmSubsystem = new PetriNetEditorMergeModule();
			
			#region Find Location for new control
			// Find Location for new control
			int iX = int.MaxValue;
			int iY = int.MaxValue;
			int iWidth = 0;
			int iHeight = 0;
			foreach(object o in this.SelectedObjects)
			{
				if (o is SelectableAndMovableControl)
				{
					SelectableAndMovableControl smac = (SelectableAndMovableControl)o;

					if (smac.Location.X < iX)
						iX = smac.Location.X;
					if (smac.Location.Y < iY)
						iY = smac.Location.Y;
					if (smac.Location.X + smac.Width > iWidth)
						iWidth = smac.Location.X + smac.Width;
					if (smac.Location.Y + smac.Height > iHeight)
						iHeight = smac.Location.Y + smac.Height;
				}
			}

			iWidth -= iX;
			iHeight -= iY;
			#endregion

			// Create subsystem
			Subsystem s = new Subsystem();
			pnemmSubsystem.Add(s);
			s.Location = new Point((int)((iX + iWidth/2) * 1/this.fZoomLevel), (int)((iY + iHeight/2) * 1/this.fZoomLevel));

			this.Paste(pnemmSubsystem);

			Subsystem sCreated = (Subsystem)this.SelectedObjects[0];

			#region Transfer controls to subsystem
			// Transfer controls to subsystem
			foreach(object o in alSelectedObjects)
			{
				if (o is SelectableAndMovableControl)
				{
					SelectableAndMovableControl smac = (SelectableAndMovableControl)o;

					if (smac is Subsystem)
					{
						Subsystem ssmac = (Subsystem)smac;
						ssmac.SuppresDeserializationCall();
					}

					this.DettachControlFromEditor(o);
					this.Controls.Remove((Control)o);
					this.Objects.Remove(o);

					sCreated.SubsystemEditor.Editor.AttachControlToEditor(o);
					sCreated.SubsystemEditor.Editor.Controls.Add((Control)o);
					sCreated.SubsystemEditor.Editor.Objects.Add(o);

					// Adjust because of different zoom
					// If this Editor has zoom level other than 1.0f - Subsystem's Editor has zoom level 1.0f
					// Possible Size and Location rounding errors
					smac.Size = new Size((int)(smac.Width * 1/this.fZoomLevel), (int)(smac.Height * 1/this.fZoomLevel));
					smac.Location = new Point((int)(smac.Location.X * 1/this.fZoomLevel), (int)(smac.Location.Y * 1/this.fZoomLevel));

					if (smac is Subsystem)
					{
						Subsystem ssmac = (Subsystem)smac;
						ssmac.EnableDeserializationCall();
					}

					smac.Location = new Point(smac.Location.X + 100 - (int)(iX * 1/this.fZoomLevel), smac.Location.Y + 100 - (int)(iY * 1/this.fZoomLevel));
				}
			}
			#endregion

			#region Add connections between objects to subsystem
			// Add connections between objects to subsystem
			ArrayList alObjects = new ArrayList();
			foreach(object o in alSelectedObjects)
			{
				if (!(o is Connection))
					alObjects.Add(o);
			}

			for(int i = 0; i < alObjects.Count; i++)
			{
				for (int j = i+1; j < alObjects.Count; j++)
				{
					if (alObjects[i] is ConnectableControl && alObjects[j] is ConnectableControl)
					{
						Connection cn = Connection.GetConnectionBetweenControls(this, (ConnectableControl)alObjects[i], (ConnectableControl)alObjects[j]);
						if (cn != null)
						{
							this.Connections.Remove(cn);
							sCreated.SubsystemEditor.Editor.Connections.Add(cn);
						}

						cn = Connection.GetConnectionBetweenControls(this, (ConnectableControl)alObjects[j], (ConnectableControl)alObjects[i]);
						if (cn != null)
						{
							this.Connections.Remove(cn);
							sCreated.SubsystemEditor.Editor.Connections.Add(cn);
						}
					}
				}
			}
			#endregion

			// Remove all connections that are not valid anymore
			for(int i = this.Connections.Count - 1; i >= 0; i--)
			{
				Connection cn = (Connection)this.Connections[i];

				if (!this.Objects.Contains(cn.From) || !this.Objects.Contains(cn.To))
				{
					cn.From.Childs.Remove(cn.To);
					cn.To.Parents.Remove(cn.From);
					this.Connections.Remove(cn);
					cn.Dispose();
				}
			}

			sCreated.AdjustConnectableRegions();
			sCreated.RefreshContents();

			this.ResumeLayout();

			// Clear Undo/Redo structures to prevent errors
			// TODO : implement Undo of Group (Ungroup)
			this.stackUndoActionList.Clear();
			this.stackRedoActionList.Clear();

			this.OnContentsChanged();
		}
		#endregion


		#region public Connection CreateNewConnection(ConnectableControl ctrlFrom, ConnectableControl ctrlTo, int iFromPort, int iToPort, Point ptBeginPort, Point ptEndPort, int iWeight)
		public Connection CreateNewConnection(ConnectableControl ctrlFrom, ConnectableControl ctrlTo, int iFromPort, int iToPort, Point ptBeginPort, Point ptEndPort, int iWeight)
		{
			// Create new connection from ConnectableControl_MouseUp event handler

			return new Connection(ctrlFrom, ctrlTo, iFromPort, iToPort, ptBeginPort, ptEndPort, iWeight);
		}
		#endregion

		#region public void AddDeserializedControl(SelectableAndMovableControl smac)
		public void AddDeserializedControl(SelectableAndMovableControl smac)
		{
			this.Controls.Add(smac);
			this.Objects.Add(smac);

			smac.Size = new Size((int)(smac.DefaultSize.Width * this.fZoomLevel), (int)(smac.DefaultSize.Height * this.fZoomLevel));

			if (smac is Place)
			{
				Place p = (Place)smac;
				p.Selected = false;

				if (p.Index >= this.pndicInstanceCounter.PlaceInstanceCount)
					this.pndicInstanceCounter.PlaceInstanceCount = p.Index;
			}
			else if (smac is Transition)
			{
				Transition t = (Transition)smac;
				t.Selected = false;

				if (t.Index >= this.pndicInstanceCounter.TransitionInstanceCount)
					this.pndicInstanceCounter.TransitionInstanceCount = t.Index;
			}
			else if (smac is Subsystem)
			{
				Subsystem s = (Subsystem)smac;
				s.Selected = false;

				if (s.Index >= this.pndicInstanceCounter.SubsystemInstanceCount)
					this.pndicInstanceCounter.SubsystemInstanceCount = s.Index;
			}
			else if (smac is DescriptionLabel)
			{
				DescriptionLabel dl = (DescriptionLabel)smac;
				dl.Selected = false;

				if (dl.Index >= this.pndicInstanceCounter.DescriptionLabelInstanceCount)
					this.pndicInstanceCounter.DescriptionLabelInstanceCount = dl.Index;
			}
			else if (smac is Input)
			{
				Input i = (Input)smac;
				i.Selected = false;

				if (i.Index >= this.iInputInstanceCount)
					this.iInputInstanceCount = i.Index;
			}
			else if (smac is Output)
			{
				Output ot = (Output)smac;
				ot.Selected = false;

				if (ot.Index >= this.iOutputInstanceCount)
					this.iOutputInstanceCount = ot.Index;
			}

			this.AttachControlToEditor(smac);
		}
		#endregion

		#region public void AddDeserializedConnection(Connection cn)
		public void AddDeserializedConnection(Connection cn)
		{
			cn.SelectionChanged += new SelectionHandler(ctrl_SelectionChanged);
			this.Connections.Add(cn);
		}
		#endregion

		#region private void PetriNetEditor_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		private void PetriNetEditor_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			this.kPressedKey = e.KeyCode;
		}
		#endregion

		#region private void PetriNetEditor_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
		private void PetriNetEditor_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			this.kPressedKey = Keys.None;
		}
		#endregion


		#region public void PerformActivation()
		public void PerformActivation()
		{
			this.DeselectControls();

			this.OnSelectionChanged(this, EventArgs.Empty);

			this.Refresh();
		}
		#endregion

		#region public void OnContentsChanged()
		public void OnContentsChanged()
		{
			base.OnContentsChanged(this, EventArgs.Empty);
		}
		#endregion

		#region private void PetriNetEditor_DragDrop(object sender, System.Windows.Forms.DragEventArgs e)
		private void PetriNetEditor_DragDrop(object sender, System.Windows.Forms.DragEventArgs e)
		{
			PetriNetEditorMergeModule pnemm = (PetriNetEditorMergeModule)e.Data.GetData(typeof(PetriNetEditorMergeModule));

			this.meaLastMouseDown = new MouseEventArgs(MouseButtons.Left, 1, (this.PointToClient(new Point(e.X, e.Y))).X, (this.PointToClient(new Point(e.X, e.Y))).Y , 0);

			this.AddMergeModule(pnemm);

			this.bDragingDroping = false;
			this.Refresh();
		}
		#endregion

		#region private void PetriNetEditor_DragOver(object sender, System.Windows.Forms.DragEventArgs e)
		private void PetriNetEditor_DragOver(object sender, System.Windows.Forms.DragEventArgs e)
		{
			if (e.Data.GetDataPresent(typeof(PetriNetEditorMergeModule)) != true)
			{
				e.Effect = DragDropEffects.None;
			}
			else
			{
				e.Effect = DragDropEffects.Move;
				this.bDragingDroping = true;
				this.dea = e;
				this.pnemmDragged = (PetriNetEditorMergeModule)e.Data.GetData(typeof(PetriNetEditorMergeModule));
				this.Refresh();
			}			
		}
		#endregion

		#region private void PetriNetEditor_DragLeave(object sender, System.EventArgs e)
		private void PetriNetEditor_DragLeave(object sender, System.EventArgs e)
		{
			this.bDragingDroping = false;
			this.Refresh();
		}
		#endregion


		#region private void ctrl_PropertiesChanged(object sender, EventArgs ea)
		private void ctrl_PropertiesChanged(object sender, EventArgs ea)
		{
			if (this.PropertiesChanged != null)
				this.PropertiesChanged(sender, ea);
		}
		#endregion

		#region private void ctrl_NameOrIndexChanged(object sender, EventArgs e)
		private void ctrl_NameOrIndexChanged(object sender, EventArgs e)
		{
			// Raise ContentsChanged event
			this.OnContentsChanged(sender, e);
		}
		#endregion

		#region private void ctrl_SelectionChanged(object sender, SelectionEventArgs sea)
		private void ctrl_SelectionChanged(object sender, SelectionEventArgs sea)
		{
			if (sea.Modifier != Keys.ShiftKey)
			{
				foreach(ISelectable samc in this.Objects)
				{
					if (samc != sender)
						samc.Selected = false;
					else
						samc.Selected = true;
				}

				foreach(Connection cn in this.Connections)
				{
					if (cn != sender)
						cn.Selected = false;
					else
						cn.Selected = true;
				}

				this.SelectedObjects.Clear();
				this.SelectedObjects.Add(sender);
			}
			else
			{
				ISelectable samc = (ISelectable)sender;

				samc.Selected = !samc.Selected;

				if (samc.Selected == true)
					this.SelectedObjects.Add(sender);
				else 
					this.SelectedObjects.Remove(sender);
			}

			this.Refresh();
			this.OnSelectionChanged(this, EventArgs.Empty);
		}
		#endregion

		#region private void ctrl_ConnectionCreated(object sender, EventArgs e)
		private void ctrl_ConnectionCreated(object sender, EventArgs e)
		{
			Connection cn = (Connection)sender;
			cn.SelectionChanged += new SelectionHandler(ctrl_SelectionChanged);

			// For Undo/Redo
			ArrayList alNewObjects = new ArrayList();
			alNewObjects.Add(cn);
			// Push UndoRedoItem to stack
			this.stackUndoActionList.Push(new UndoRedoItem(alNewObjects, this, UndoRedoAction.Created, null));
			// Clear RedoActionList
			this.stackRedoActionList.Clear();

			this.OnContentsChanged(this, EventArgs.Empty);

			cn.PerformActivation();
		}
		#endregion

		#region private void ctrl_Resized(object sender, ResizedEventArgs e)
		private void ctrl_Resized(object sender, ResizedEventArgs e)
		{
			ResizableControl rc = (ResizableControl)sender;
			Size[] sza = new Size[2]{e.OldValue, rc.Size};

			// Push UndoRedoItem to stack
			this.stackUndoActionList.Push(new UndoRedoItem(sender, this, UndoRedoAction.Resized, sza));
			
			// Clear RedoActionList
			this.stackRedoActionList.Clear();
		}
		#endregion

		#region private void ctrl_LocationChanged(object sender, LocationChangedEventArgs e)
		private void ctrl_LocationChanged(object sender, LocationChangedEventArgs e)
		{
			// Create new oData object for UndoRedoItem
			ArrayList alNewValues = new ArrayList();
			foreach(object o in e.Objects)
			{	
				SelectableAndMovableControl smac = (SelectableAndMovableControl)o;
				alNewValues.Add(smac.Location);
			}

			ArrayList al = new ArrayList();
			al.Add(e.Objects);
			al.Add(e.OldValues);
			al.Add(alNewValues);

			// This if is to prevent empty Undo objects collection, after 
			// opening the document with double-click, if mouse was directly
			// on top of some control
			if (e.Objects.Count != 0)
			{
				// Push UndoRedoItem to stack
				this.stackUndoActionList.Push(new UndoRedoItem(sender, this, UndoRedoAction.LocationChanged, al));
					
				// Clear RedoActionList
				this.stackRedoActionList.Clear();
			}
		}
		#endregion


		#region IUndoRedo Members

		#region public void Undo(object o, UndoRedoAction ura, object oData)
		public void Undo(object o, UndoRedoAction ura, object oData)
		{
			#region if (ura == UndoRedoAction.Created)
			if (ura == UndoRedoAction.Created)
			{
				ArrayList alNewObjects = (ArrayList)o;
				
				this.SelectedObjects.Clear();

				foreach(object obj in this.Objects)
				{
					SelectableAndMovableControl smac = (SelectableAndMovableControl)obj;
					smac.Selected = false;
				}
				
				foreach(Connection cn in this.Connections)
				{
					cn.Selected = false;
				}

				foreach(object obj in alNewObjects)
				{
					this.SelectedObjects.Add(obj);
				}

				this.bSuppresUndo = true;
				this.DeleteSelectedControls();
				this.bSuppresUndo = false;
			}
			#endregion

			#region else if (ura == UndoRedoAction.Deleted)
			else if (ura == UndoRedoAction.Deleted)
			{
				ArrayList alDeletedObjects = (ArrayList)o;

				foreach(object obj in alDeletedObjects)
				{
					if (obj is SelectableAndMovableControl)
					{
						SelectableAndMovableControl smac = (SelectableAndMovableControl)obj;

						this.Objects.Add(smac);

						if (smac is Subsystem)
						{
							Subsystem s = (Subsystem)smac;
							s.SuppresDeserializationCall();
						}

						this.Controls.Add(smac);

						if (smac is Subsystem)
						{
							Subsystem s = (Subsystem)smac;
							s.SuppresDeserializationCall();
						}

						this.AttachControlToEditor(smac);
					}
					else if (obj is Connection)
					{
						Connection cn = (Connection)obj;
						this.Connections.Add(cn);

						if (!cn.From.Childs.Contains(cn.To))
							cn.From.Childs.Add(cn.To);
						if (!cn.To.Parents.Contains(cn.From))
							cn.To.Parents.Add(cn.From);
					}
				}
		
				this.OnContentsChanged();

				this.Refresh();
			}
			#endregion

			#region else if (ura == UndoRedoAction.Resized)
			else if (ura == UndoRedoAction.Resized)
			{
				ResizableControl rc = (ResizableControl)o;
				Size[] sza = (Size[])oData;
				rc.Size = sza[0];

				if (this.PropertiesChanged != null)
					this.PropertiesChanged(rc, EventArgs.Empty);
			}
			#endregion

			#region else if (ura == UndoRedoAction.LocationChanged)
			else if (ura == UndoRedoAction.LocationChanged)
			{
				//Retrieve object info
				ArrayList al = (ArrayList)oData;
				ArrayList alObjects = (ArrayList)al[0];
				ArrayList alOldValues = (ArrayList)al[1];

				for(int i = 0; i < alObjects.Count; i++)
				{
					SelectableAndMovableControl smac = (SelectableAndMovableControl)alObjects[i];
					smac.Location = (Point)alOldValues[i];
				}
				
				this.Refresh();

				if (this.PropertiesChanged != null)
					this.PropertiesChanged(o, EventArgs.Empty);
			}
			#endregion
		}
		#endregion

		#region public void Redo(object o, UndoRedoAction ura, object oData)
		public void Redo(object o, UndoRedoAction ura, object oData)
		{
			if (ura == UndoRedoAction.Created)
			{
				this.Undo(o, UndoRedoAction.Deleted, oData);
			}
			else if (ura == UndoRedoAction.Deleted)
			{
				this.Undo(o, UndoRedoAction.Created, oData);
			}
			else if (ura == UndoRedoAction.Resized)
			{
				ResizableControl rc = (ResizableControl)o;
				Size[] sza = (Size[])oData;
				rc.Size = sza[1];

				if (this.PropertiesChanged != null)
					this.PropertiesChanged(rc, EventArgs.Empty);
			}
			else if (ura == UndoRedoAction.LocationChanged)
			{
				//Retrieve object info
				ArrayList al = (ArrayList)oData;
				ArrayList alObjects = (ArrayList)al[0];
				ArrayList alNewValues = (ArrayList)al[2];

				for(int i = 0; i < alObjects.Count; i++)
				{
					SelectableAndMovableControl smac = (SelectableAndMovableControl)alObjects[i];
					smac.Location = (Point)alNewValues[i];
				}
				
				this.Refresh();

				if (this.PropertiesChanged != null)
					this.PropertiesChanged(o, EventArgs.Empty);
			}
		}
		#endregion

		#endregion
	}
}
