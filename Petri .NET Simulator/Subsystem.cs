using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Runtime.Serialization;

namespace PetriNetSimulator2
{
	[TypeConverter(typeof(SubsystemConverter))]
	[Serializable]
	public class Subsystem : ConnectableControl, ISerializable, ICloneable, IComparable, IConnector, IMetafileModel
	{
		// Properties

		#region public string NameID
		[CommonProperties]
		[Category("Identification")]
		[Description("Name for this subsystem.")]
		public string NameID
		{
			get
			{
				return this.sName;
			}
			set
			{
				if (this.Parent != null)
				{
					this.sName = value;

					if (this.NameOrIndexChanged != null)
						this.NameOrIndexChanged(this, new EventArgs());

					this.Refresh();
				}
			}
		}
		#endregion

		#region public new Point Location
		[CommonProperties]
		[Category("Layout")]
		[Description("Location of label within editor.")]
		public new Point Location
		{
			get
			{
				return base.Location;
			}
			set
			{
				base.Location = value;

				if (this.Parent != null)
					this.Parent.Refresh();
			}
		}
		#endregion

		#region public new Size Size
		[CommonProperties]
		[Category("Layout")]
		[Description("Size of label in the editor.")]
		public new Size Size
		{
			get
			{
				return base.Size;
			}
			set
			{
				base.Size = value;

				if (this.Parent != null)
					this.Parent.Refresh();
			}
		}
		#endregion

		#region public ArrayList Objects
		public ArrayList Objects
		{
			get
			{
				if (this.seSubsystemEditor != null)
					return this.seSubsystemEditor.Editor.Objects;
				else
					return null;
			}
		}
		#endregion

		#region public ArrayList Connections
		public ArrayList Connections
		{
			get
			{
				if (this.seSubsystemEditor != null)
					return this.seSubsystemEditor.Editor.Connections;
				else
					return null;
			}
		}
		#endregion

		#region public SubsystemEditor SubsystemEditor
		public SubsystemEditor SubsystemEditor
		{
			get
			{
				return this.seSubsystemEditor;
			}
		}
		#endregion

		// Events
		internal event EventHandler NameOrIndexChanged;

		// Fields
		private Color cBorderColor = Color.Black;
		private string sName = "";
		private ArrayList alObjects = new ArrayList();
		private ArrayList alConnections = new ArrayList();
		private Hashtable htInputGraphicsPaths = new Hashtable();
		private Hashtable htInputInputPoints = new Hashtable();
		private Hashtable htOutputGraphicsPaths = new Hashtable();
		private Hashtable htOutputOutputPoints = new Hashtable();
		private bool bSuppresDeserializationCall = false;
		
		private SubsystemEditor seSubsystemEditor = null;

		private System.ComponentModel.IContainer components = null;

		#region public Subsystem() : base(new Size(72, 72))
		public Subsystem() : base(new Size(72, 72))
		{
			// This call is required by the Windows Form Designer.
			InitializeComponent();

			this.SetStyle(ControlStyles.UserPaint |	ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer, true);
			this.UpdateStyles();

			this.AllowResizing = true;
			this.Paint += new PaintEventHandler(Subsystem_Paint);
			this.MouseUp += new MouseEventHandler(Subsystem_MouseUp);
			this.Resize += new EventHandler(Subsystem_Resize);
			this.DoubleClick += new EventHandler(Subsystem_DoubleClick);
			this.ParentChanged += new EventHandler(Subsystem_ParentChanged);
		}
		#endregion

		// Constructor for Deserialization
		#region public Subsystem(SerializationInfo info, StreamingContext context) : base(info, context)
		public Subsystem(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			this.SetStyle(ControlStyles.UserPaint |	ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer, true);
			this.UpdateStyles();

			this.AllowResizing = true;
			this.Paint += new PaintEventHandler(Subsystem_Paint);
			this.MouseUp += new MouseEventHandler(Subsystem_MouseUp);
			this.Resize += new EventHandler(Subsystem_Resize);
			this.DoubleClick += new EventHandler(Subsystem_DoubleClick);
			this.ParentChanged += new EventHandler(Subsystem_ParentChanged);

			this.Location = (Point)info.GetValue("location", typeof(Point));
			this.seSubsystemEditor = (SubsystemEditor)info.GetValue("editor", typeof(SubsystemEditor));

			// To set region, call resize
//			this.Transition_Resize(this, EventArgs.Empty);
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
			components = new System.ComponentModel.Container();
		}
		#endregion


		#region public new void GetObjectData(SerializationInfo info, StreamingContext context)
		public new void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);

			info.AddValue("location", this.Location);
			info.AddValue("editor", this.seSubsystemEditor);
		}
		#endregion


		#region private void Subsystem_Paint(object sender, PaintEventArgs e)
        private void Subsystem_Paint(object sender, PaintEventArgs e)
		{
			// Draws Subsystem object
			Graphics g = e.Graphics;

			if (PetriNetDocument.AntiAlias == true)
				g.SmoothingMode = SmoothingMode.AntiAlias;

			PetriNetEditor pne = (PetriNetEditor)this.Parent;

			Rectangle rect = this.ClientRectangle;

			LinearGradientBrush lgb = new LinearGradientBrush(rect, Color.White, this.cBackgroundColor, LinearGradientMode.ForwardDiagonal);
            
			g.FillRectangle(lgb, rect);

			Pen pBlack = new Pen(this.cBorderColor, pne.Zoom * 5);
			g.DrawRectangle(pBlack, 0, 0, this.Width - 1, this.Height - 1);

			g.FillRegion(Brushes.Red, this.ConnectableInputRegion);
			g.FillRegion(Brushes.Red, this.ConnectableOutputRegion);

			Brush bBlack = new SolidBrush(Color.Black);
			StringFormat sf = new StringFormat();
			sf.Alignment = StringAlignment.Center;
			sf.LineAlignment = StringAlignment.Center;
			
			Font f = new Font(this.Parent.Font.FontFamily, pne.Zoom * 7f, FontStyle.Bold);
			
			string sText = (this.sName != "") ? this.sName + "\n\n" + "S" + this.sIndex : "S" + this.sIndex.ToString();
			g.DrawString(sText, f, bBlack, new Rectangle(new Point(0, 0), new Size(this.Width - (int)(pne.Zoom * 2), this.Height)), sf);

			foreach(object o in this.htInputGraphicsPaths.Keys)
			{
				GraphicsPath gp = (GraphicsPath)this.htInputGraphicsPaths[o];
				g.DrawPath(Pens.Black, gp);

				string sIndex = ((Input)o).Index.ToString();
				Point p = (Point)this.htInputInputPoints[o];
				
				g.DrawString(sIndex, f, bBlack, p, sf);
			}

			foreach(object o in this.htOutputGraphicsPaths.Keys)
			{
				GraphicsPath gp = (GraphicsPath)this.htOutputGraphicsPaths[o];
				g.DrawPath(Pens.Black, gp);

				string sIndex = ((Output)o).Index.ToString();
				Point p = (Point)this.htOutputOutputPoints[o];
				
				g.DrawString(sIndex, f, bBlack, p, sf);
			}

		}
		#endregion

		#region private void Subsystem_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		private void Subsystem_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			// Refresh Location property
			this.OnPropertiesChanged(this, EventArgs.Empty);
		}
		#endregion

		#region private void Subsystem_Resize(object sender, EventArgs e)
		private void Subsystem_Resize(object sender, EventArgs e)
		{
			this.AdjustConnectableRegions();	
		}
		#endregion

		#region private void Subsystem_DoubleClick(object sender, EventArgs e)
		private void Subsystem_DoubleClick(object sender, EventArgs e)
		{
			this.seSubsystemEditor.Text = "Subsystem Editor : " + this.ToString();
			this.seSubsystemEditor.ShowDialog();
		}
		#endregion

		#region private void Subsystem_ParentChanged(object sender, EventArgs e)
		private void Subsystem_ParentChanged(object sender, EventArgs e)
		{
			if (this.Parent is PetriNetEditor)
			{
				if (this.seSubsystemEditor == null)
				{
					this.seSubsystemEditor = new SubsystemEditor(((PetriNetEditor)this.Parent).Document, this);
				}
				else
				{
					if (this.bSuppresDeserializationCall != true)
						this.seSubsystemEditor.RestoreAfterDeserialization(((PetriNetEditor)this.Parent).Document, this);
				}
			}
		}
		#endregion


		#region public void AddObjectReference(object o)
		public void AddObjectReference(object o)
		{
			if (o is SelectableAndMovableControl)
			{
				this.Objects.Add(o);
				this.seSubsystemEditor.Editor.Controls.Add((Control)o);
			}
			else if (o is Connection)
				this.Connections.Add(o);
		}
		#endregion

		#region public void AdjustConnectableRegions()
		public void AdjustConnectableRegions()
		{
			if (this.Parent is PetriNetEditor)
			{
				PetriNetEditor pne = (PetriNetEditor)this.Parent;

				this.alInputPorts.Clear();
				this.alOutputPorts.Clear();

				ArrayList alInputPoints = new ArrayList();
				ArrayList alOutputPoints = new ArrayList();
				ArrayList alInputs = new ArrayList();
				ArrayList alOutputs = new ArrayList();

				int iX = int.MaxValue;
				int iY = int.MaxValue;
				int iWidth = 0;
				int iHeight = 0;

				int iHalfArrowWidth = (int)(7 * pne.Zoom);

				foreach(object o in this.Objects)
				{
					SelectableAndMovableControl smac = (SelectableAndMovableControl)o;

					if (o is Input)
					{
						alInputs.Add(o);
						alInputPoints.Add(smac.Location);
					}
					if (o is Output)
					{
						alOutputs.Add(o);
						alOutputPoints.Add(smac.Location);
					}

					if (smac.Location.X < iX)
						iX = smac.Location.X;
					if (smac.Location.Y < iY)
						iY = smac.Location.Y;
					if (smac.Location.X + smac.Width > iWidth)
						iWidth = smac.Location.X + smac.Width;
					if (smac.Location.Y + smac.Height > iHeight)
						iHeight = smac.Location.Y + smac.Height;
				}

				Rectangle rect = new Rectangle(iX, iY, iWidth - iX, iHeight - iY);

				// Transform rect coordinates in client coordinates
				float iXScaler = (float)(this.Size.Width) / (float)rect.Width;
				float iYScaler = (float)(this.Size.Height) / (float)rect.Height;

				// Smaller rect
				float iXScaler2 = (float)(this.Width - 20 * pne.Zoom) / (float)this.Width;
				float iYScaler2 = (float)(this.Height - 20 * pne.Zoom) / (float)this.Height;

				GraphicsPath gpInputs = new GraphicsPath();
				GraphicsPath gpOutputs = new GraphicsPath();

				this.htInputGraphicsPaths.Clear();
				this.htInputInputPoints.Clear();
				this.htOutputGraphicsPaths.Clear();
				this.htOutputOutputPoints.Clear();

				for(int i = 0; i < alInputPoints.Count; i++)
				{
					object o = alInputPoints[i];

					Point p = (Point)o;
					p.X -= iX;
					p.Y -= iY;

					Point pFixed = this.GetFixedPoint(p, rect);
					LineDirection ldEnd = this.GetDirection(p, this.GetCenterPoint(p, rect), rect);

					p = pFixed;

					p.X = (int)(p.X * iXScaler);
					p.Y = (int)(p.Y * iYScaler);

					//Scale to smaller rect
					p.X = (int)((p.X)* iXScaler2) + (int)(10 * pne.Zoom);
					p.Y = (int)((p.Y)* iYScaler2) + (int)(10 * pne.Zoom);


					GraphicsPath gp = new GraphicsPath();
					if (ldEnd == LineDirection.Left)
					{
						gp.AddPolygon(new Point[]{new Point(0, p.Y - iHalfArrowWidth), new Point(iHalfArrowWidth, p.Y), new Point(0, p.Y + iHalfArrowWidth)});
						p.X += 3;
					}
					if (ldEnd == LineDirection.Up)
					{
						gp.AddPolygon(new Point[]{new Point(p.X - iHalfArrowWidth, 0), new Point(p.X , iHalfArrowWidth), new Point(p.X + iHalfArrowWidth, 0)});
						p.Y += 3;
					}
					if (ldEnd == LineDirection.Down)
					{
						gp.AddPolygon(new Point[]{new Point(p.X - iHalfArrowWidth, this.Height - 1), new Point(p.X, this.Height - 1 - iHalfArrowWidth), new Point(p.X + iHalfArrowWidth, this.Height - 1)});
						p.Y -= 3;
					}
					if (ldEnd == LineDirection.Right)
					{
						gp.AddPolygon(new Point[]{new Point(this.Width - 1, p.Y - iHalfArrowWidth), new Point(this.Width - 1 - iHalfArrowWidth, p.Y), new Point(this.Width - 1, p.Y + iHalfArrowWidth)});
						p.X -= 3;
					}

					this.htInputInputPoints.Add(alInputs[alInputPoints.IndexOf(o)], p);

					gpInputs.AddPath(gp, false);
					this.htInputGraphicsPaths.Add(alInputs[alInputPoints.IndexOf(o)], gp);

					Port port = new Port(((Input)alInputs[i]).Index, new Region(gp));
					this.alInputPorts.Add(port);
				}

				for(int i = 0; i < alOutputPoints.Count; i++)
				{
					object o = alOutputPoints[i];

					Point p = (Point)o;
					p.X -= iX;
					p.Y -= iY;

					Point pFixed = this.GetFixedPoint(p, rect);
					LineDirection ldEnd = this.GetDirection(p, this.GetCenterPoint(p, rect), rect);

					p = pFixed;

					p.X = (int)(p.X * iXScaler);
					p.Y = (int)(p.Y * iYScaler);

					//Scale to smaller rect
					p.X = (int)((p.X)* iXScaler2) + (int)(10 * pne.Zoom);
					p.Y = (int)((p.Y)* iYScaler2) + (int)(10 * pne.Zoom);


					GraphicsPath gp = new GraphicsPath();
					if (ldEnd == LineDirection.Left)
					{
						gp.AddPolygon(new Point[]{new Point(iHalfArrowWidth, p.Y - iHalfArrowWidth), new Point(0, p.Y), new Point(iHalfArrowWidth, p.Y + iHalfArrowWidth)});
						p.X += 3;
					}
					if (ldEnd == LineDirection.Up)
					{
						gp.AddPolygon(new Point[]{new Point(p.X - iHalfArrowWidth, iHalfArrowWidth), new Point(p.X , 0), new Point(p.X + iHalfArrowWidth, iHalfArrowWidth)});
						p.Y += 3;
					}
					if (ldEnd == LineDirection.Down)
					{
						gp.AddPolygon(new Point[]{new Point(p.X - iHalfArrowWidth, this.Height - 1 - iHalfArrowWidth), new Point(p.X, this.Height - 1), new Point(p.X + iHalfArrowWidth, this.Height - 1 - iHalfArrowWidth)});
						p.Y -= 3;
					}
					if (ldEnd == LineDirection.Right)
					{
						gp.AddPolygon(new Point[]{new Point(this.Width - 1 - iHalfArrowWidth, p.Y - iHalfArrowWidth), new Point(this.Width - 1, p.Y), new Point(this.Width - 1 - iHalfArrowWidth, p.Y + iHalfArrowWidth)});
						p.X -= 3;
					}

					this.htOutputOutputPoints.Add(alOutputs[alOutputPoints.IndexOf(o)], p);

					gpOutputs.AddPath(gp, false);
					this.htOutputGraphicsPaths.Add(alOutputs[alOutputPoints.IndexOf(o)], gp);

					Port port = new Port(((Output)alOutputs[i]).Index, new Region(gp));
					this.alOutputPorts.Add(port);
				}

				this.Refresh();
			}
		}
		#endregion

		#region public int CompareTo(object o)
		public int CompareTo(object o)
		{
			if (o is Subsystem)
			{
				Subsystem s = (Subsystem)o;
				return this.iIndex.CompareTo(s.Index);
			}

			throw new ArgumentException("Invalid argument");
		}
		#endregion

		#region public object Clone()
		public object Clone()
		{
			Subsystem s = new Subsystem();
			s.Location = this.Location;

			s.Size = this.Size;

			// Change default size
			if (this.Parent != null && this.Parent is EditorSurface)
			{
				EditorSurface es = (EditorSurface)this.Parent;
				s.DefaultSize = new Size((int)(this.Width / es.Zoom), (int)(this.Height / es.Zoom));
			}
			else if (this.Parent == null)
			{
				s.DefaultSize = this.DefaultSize;
			}

			if (this.seSubsystemEditor != null)
				s.seSubsystemEditor = (SubsystemEditor)this.seSubsystemEditor.Clone(s);

			return s;
		}
		#endregion

		#region public override string ToString()
		public override string ToString()
		{
			if (this.NameID != null && this.NameID != "")
				return "S" + this.sIndex + " - " + this.NameID + " (Subsystem)";
			else
				return "S" + this.sIndex + " (Subsystem)";
		}
		#endregion

		#region public override bool CanConnectTo(object o)
		public override bool CanConnectTo(object o)
		{
			if (o is Transition || o is Output)
				return true;
			else 
				return false;
		}
		#endregion

		#region *IConnector

		#region public Point GetBeginCenterPoint(Point ptBegin, Size sz)
		public Point GetBeginCenterPoint(Point ptBegin, Size sz)
		{
			if ((((float)sz.Height/(float)sz.Width * (float)ptBegin.X) < (float)ptBegin.Y) && ((-(float)sz.Height/(float)sz.Width * (float)ptBegin.X + (float)sz.Height) < (float)ptBegin.Y))
				return new Point(ptBegin.X, (int)(sz.Height/2));
			else if ((((float)sz.Height/(float)sz.Width * (float)ptBegin.X) >= (float)ptBegin.Y) && ((-(float)sz.Height/(float)sz.Width * (float)ptBegin.X + (float)sz.Height) <= (float)ptBegin.Y))
				return new Point((int)(sz.Width/2), ptBegin.Y);
			else if ((((float)sz.Height/(float)sz.Width * (float)ptBegin.X) > (float)ptBegin.Y) && ((-(float)sz.Height/(float)sz.Width * (float)ptBegin.X + (float)sz.Height) > (float)ptBegin.Y))
				return new Point(ptBegin.X, (int)(sz.Height/2));
			else 
				return new Point((int)(sz.Width/2), ptBegin.Y);
		}
		#endregion

		#region public Point GetEndCenterPoint(Point ptEnd, Size sz)
		public Point GetEndCenterPoint(Point ptEnd, Size sz)
		{
			if ((((float)sz.Height/(float)sz.Width * (float)ptEnd.X) < (float)ptEnd.Y) && ((-(float)sz.Height/(float)sz.Width * (float)ptEnd.X + (float)sz.Height) < (float)ptEnd.Y))
				return new Point(ptEnd.X, (int)(sz.Height/2));
			else if ((((float)sz.Height/(float)sz.Width * (float)ptEnd.X) >= (float)ptEnd.Y) && ((-(float)sz.Height/(float)sz.Width * (float)ptEnd.X + (float)sz.Height) <= (float)ptEnd.Y))
				return new Point((int)(sz.Width/2), ptEnd.Y);
			else if ((((float)sz.Height/(float)sz.Width * (float)ptEnd.X) > (float)ptEnd.Y) && ((-(float)sz.Height/(float)sz.Width * (float)ptEnd.X + (float)sz.Height) > (float)ptEnd.Y))
				return new Point(ptEnd.X, (int)(sz.Height/2));
			else 
				return new Point((int)(sz.Width/2), ptEnd.Y);
		}
		#endregion

		#region public Point GetEndFixedPoint(Point ptEnd, Size sz)
		public Point GetEndFixedPoint(Point ptEnd, Size sz)
		{
			if ((((float)sz.Height/(float)sz.Width * (float)ptEnd.X) < (float)ptEnd.Y) && ((-(float)sz.Height/(float)sz.Width * (float)ptEnd.X + (float)sz.Height) < (float)ptEnd.Y))
				return new Point(ptEnd.X, sz.Height);
			else if ((((float)sz.Height/(float)sz.Width * (float)ptEnd.X) >= (float)ptEnd.Y) && ((-(float)sz.Height/(float)sz.Width * (float)ptEnd.X + (float)sz.Height) <= (float)ptEnd.Y))
				return new Point(sz.Width, ptEnd.Y);
			else if ((((float)sz.Height/(float)sz.Width * (float)ptEnd.X) > (float)ptEnd.Y) && ((-(float)sz.Height/(float)sz.Width * (float)ptEnd.X + (float)sz.Height) > (float)ptEnd.Y))
				return new Point(ptEnd.X, 0);
			else
				return new Point(0, ptEnd.Y);

		}
		#endregion

		#region public LineDirection GetBeginDirection(Point ptBegin, Point ptBeginCenter, Size sz)
		public LineDirection GetBeginDirection(Point ptBegin, Point ptBeginCenter, Size sz)
		{
			if ((((float)sz.Height/(float)sz.Width * (float)ptBegin.X) < (float)ptBegin.Y) && ((-(float)sz.Height/(float)sz.Width * (float)ptBegin.X + (float)sz.Height) < (float)ptBegin.Y))
				return LineDirection.Down;
			else if ((((float)sz.Height/(float)sz.Width * (float)ptBegin.X) >= (float)ptBegin.Y) && ((-(float)sz.Height/(float)sz.Width * (float)ptBegin.X + (float)sz.Height) <= (float)ptBegin.Y))
				return LineDirection.Right;
			else if ((((float)sz.Height/(float)sz.Width * (float)ptBegin.X) > (float)ptBegin.Y) && ((-(float)sz.Height/(float)sz.Width * (float)ptBegin.X + (float)sz.Height) > (float)ptBegin.Y))
				return LineDirection.Up;
			else
				return LineDirection.Left;
		}
		#endregion

		#region public LineDirection GetEndDirection(Point ptEnd, Point ptEndCenter, Size sz)
		public LineDirection GetEndDirection(Point ptEnd, Point ptEndCenter, Size sz)
		{
			if ((((float)sz.Height/(float)sz.Width * (float)ptEnd.X) < (float)ptEnd.Y) && ((-(float)sz.Height/(float)sz.Width * (float)ptEnd.X + (float)sz.Height) < (float)ptEnd.Y))
				return LineDirection.Down;
			else if ((((float)sz.Height/(float)sz.Width * (float)ptEnd.X) >= (float)ptEnd.Y) && ((-(float)sz.Height/(float)sz.Width * (float)ptEnd.X + (float)sz.Height) <= (float)ptEnd.Y))
				return LineDirection.Right;
			else if ((((float)sz.Height/(float)sz.Width * (float)ptEnd.X) > (float)ptEnd.Y) && ((-(float)sz.Height/(float)sz.Width * (float)ptEnd.X + (float)sz.Height) > (float)ptEnd.Y))
				return LineDirection.Up;
			else
				return LineDirection.Left;
		}
		#endregion

		#endregion

		#region public Point GetFixedPoint(Point pt, Rectangle r)
		public Point GetFixedPoint(Point pt, Rectangle r)
		{
			Size sz = new Size(r.Width, r.Height);
			return this.GetEndFixedPoint(pt, sz);
		}
		#endregion

		#region public Point GetCenterPoint(Point pt, Rectangle r)
		public Point GetCenterPoint(Point pt, Rectangle r)
		{
			Size sz = new Size(r.Width, r.Height);
			return this.GetEndCenterPoint(pt, sz);
		}
		#endregion

		#region public LineDirection GetDirection(Point pt, Point ptCenter, Rectangle r)
		public LineDirection GetDirection(Point pt, Point ptEndCenter, Rectangle r)
		{
			Size sz = new Size(r.Width, r.Height);
			return this.GetEndDirection(pt, this.GetEndCenterPoint(pt, sz),sz);
		}
		#endregion

		#region public void OnEditingFinished()
		public void OnEditingFinished()
		{
			this.OnResize(EventArgs.Empty);

			if (this.Parent is PetriNetEditor)
			{
				PetriNetEditor pne = (PetriNetEditor)this.Parent;

				//Check for valid connections
				for(int i = this.Parents.Count - 1; i >= 0; i--)
				{
					ConnectableControl cc = (ConnectableControl)this.Parents[i];
					Connection cn = Connection.GetConnectionBetweenControls(cc, this);
	                
					//Find input port with same number as connection output port
					if (!this.InputPortNumberExists(cn.ToPort))
					{
						ConnectableControl ccFrom = cn.From;
						ConnectableControl ccTo = cn.To;
					
						// Dispose connection
						ccFrom.Childs.Remove(ccTo);
						ccTo.Parents.Remove(ccFrom);
		
						pne.Connections.Remove(cn);
						cn.Dispose();
					}
				}

				for(int i = this.Childs.Count - 1; i >= 0; i--)
				{
					ConnectableControl cc = (ConnectableControl)this.Childs[i];
					Connection cn = Connection.GetConnectionBetweenControls(this, cc);
	                
					//Find input port with same number as connection output port
					if (!this.OutputPortNumberExists(cn.FromPort))
					{
						ConnectableControl ccFrom = cn.From;
						ConnectableControl ccTo = cn.To;
					
						// Dispose connection
						ccFrom.Childs.Remove(ccTo);
						ccTo.Parents.Remove(ccFrom);
		
						pne.Connections.Remove(cn);
						cn.Dispose();
					}
				}

				// Adjust Virtual connections weights
				foreach(Connection cn in pne.Connections)
				{
					if (cn.From == this)
					{
						cn.Weight = cn.GetOutputWeightsSum(this);
					}
					else if (cn.To == this)
					{
						cn.Weight = cn.GetInputWeightsSum(this);
					}
				}

				pne.OnContentsChanged();
			}
		}
		#endregion

		#region public void RefreshContents()
		public void RefreshContents()
		{
			this.seSubsystemEditor.Editor.OnContentsChanged();
		}
		#endregion

		#region public void InsertMergeModule(PetriNetEditorMergeModule pnemm)
		public void InsertMergeModule(PetriNetEditorMergeModule pnemm)
		{
			this.seSubsystemEditor.Editor.Paste(pnemm);
		}
		#endregion


		#region public void GetBaseControlConnectedToInputPort(ArrayList al, int iPortNumber)
		public void GetBaseControlConnectedToInputPort(ArrayList alControls, ArrayList alWeights, int iPortNumber)
		{
			// Finds next base control in subsystem hierarchy (Place or Transition)

			foreach(object o in this.Objects)
			{
				if (o is Input)
				{
					Input i = (Input)o;
					if (i.Index == iPortNumber)
					{
						foreach(ConnectableControl cc in i.Childs)
						{
							if (cc is Place)
							{
								alControls.Add(cc);
								alWeights.Add(Connection.GetConnectionBetweenControls(i, cc).Weight);
							}
							else if (cc is Subsystem)
							{
								Subsystem s = (Subsystem)cc;
								s.GetBaseControlConnectedToInputPort(alControls, alWeights, Connection.GetConnectionBetweenControls(i, cc).ToPort);
							}
						}
					}
				}
			}
		}
		#endregion

		#region public void GetBaseControlConnectedToOutputPort(ArrayList alControls, ArrayList alWeights, int iPortNumber)
		public void GetBaseControlConnectedToOutputPort(ArrayList alControls, ArrayList alWeights, int iPortNumber)
		{
			// Finds next base control in subsystem hierarchy (Place or Transition)

			foreach(object o in this.Objects)
			{
				if (o is Output)
				{
					Output ot = (Output)o;
					if (ot.Index == iPortNumber)
					{
						foreach(ConnectableControl cc in ot.Parents)
						{
							if (cc is Place)
							{
								alControls.Add(cc);
								alWeights.Add(Connection.GetConnectionBetweenControls(cc, ot).Weight);
							}
							else if (cc is Subsystem)
							{
								Subsystem s = (Subsystem)cc;
								s.GetBaseControlConnectedToOutputPort(alControls, alWeights, Connection.GetConnectionBetweenControls(cc, ot).FromPort);
							}
						}
					}
				}
			}
		}
		#endregion

		#region public void FillInConnectionMatrices(object[,] oa, ArrayList alObjects, ArrayList alChilds)
		public void FillInConnectionMatrices(object[,] oa, ArrayList alObjects, ArrayList alChilds)
		{
			foreach(Connection cn in this.Connections)
			{
				if (cn.From is Place && alObjects.Contains(cn.From))
				{
					if (cn.To is Transition && alChilds.Contains(cn.To))
					{
						oa[alChilds.IndexOf(cn.To), alObjects.IndexOf(cn.From)] = cn.Weight;
					}
				}
				else if (cn.From is Transition && alObjects.Contains(cn.From))
				{
					if (cn.To is Place && alChilds.Contains(cn.To))
					{
						oa[alChilds.IndexOf(cn.To), alObjects.IndexOf(cn.From)] = cn.Weight;
					}
					if (cn.To is Subsystem)
					{
						// Find next connectable control
						Subsystem s = (Subsystem)cn.To;

						ArrayList alToControls = new ArrayList();
						ArrayList alWeights = new ArrayList();
						s.GetBaseControlConnectedToInputPort(alToControls, alWeights, cn.ToPort);

						foreach(ConnectableControl cc in alToControls)
						{
							if (alChilds.Contains(cc))
								oa[alChilds.IndexOf(cc), alObjects.IndexOf(cn.From)] = alWeights[alToControls.IndexOf(cc)];
						}
					}
				}
				else if (cn.From is Subsystem)
				{
					if (cn.To is Transition)
					{
						// Find next connectable control
						Subsystem s = (Subsystem)cn.From;

						ArrayList alFromControls = new ArrayList();
						ArrayList alWeights = new ArrayList();
						s.GetBaseControlConnectedToOutputPort(alFromControls, alWeights, cn.FromPort);

						foreach(ConnectableControl cc in alFromControls)
						{
							if (alObjects.Contains(cc))
								oa[alChilds.IndexOf(cn.To), alObjects.IndexOf(cc)] = cn.Weight;
						}
					}
				}

				//Walk through all subsystems and add connections weights to matrix
				foreach(object o in this.Objects)
				{
					if (o is Subsystem)
					{
						Subsystem s = (Subsystem)o;
						s.FillInConnectionMatrices(oa, alObjects, alChilds);
					}
				}
			}
		}
		#endregion

		#region public void SuppresDeserializationCall()
		public void SuppresDeserializationCall()
		{
			this.bSuppresDeserializationCall = true;
		}
		#endregion

		#region public void EnableDeserializationCall()
		public void EnableDeserializationCall()
		{
			this.bSuppresDeserializationCall = false;
		}
		#endregion


		#region IMetafileModel Members

		public void DrawModel(Graphics g)
		{
			Point pt = this.Location;

			PetriNetEditor pne = (PetriNetEditor)this.Parent;
			Rectangle r = new Rectangle(pt, this.Size);

			g.FillRectangle(Brushes.White, r);

			Pen pBlack = new Pen(this.cBorderColor, pne.Zoom * 3);
			g.DrawRectangle(pBlack, pt.X, pt.Y, this.Width - 1, this.Height - 1);

			Region ri = this.ConnectableInputRegion.Clone();
			Region ro = this.ConnectableOutputRegion.Clone();

			ri.Translate(pt.X, pt.Y);
			ro.Translate(pt.X, pt.Y);

			g.FillRegion(Brushes.Red, ri);
			g.FillRegion(Brushes.Red, ro);

			Brush bBlack = new SolidBrush(Color.Black);
			StringFormat sf = new StringFormat();
			sf.Alignment = StringAlignment.Center;
			sf.LineAlignment = StringAlignment.Center;
			
			Font f = new Font(this.Parent.Font.FontFamily, pne.Zoom * 7f, FontStyle.Bold);
			
			string sText = (this.sName != "") ? this.sName + "\n\n" + "S" + this.sIndex : "S" + this.sIndex.ToString();
			g.DrawString(sText, f, bBlack, new Rectangle(new Point(pt.X, pt.Y), new Size(this.Width - (int)(pne.Zoom * 2), this.Height)), sf);

			foreach(object o in this.htInputGraphicsPaths.Keys)
			{
				GraphicsPath gp = (GraphicsPath)((GraphicsPath)this.htInputGraphicsPaths[o]).Clone();
				System.Drawing.Drawing2D.Matrix m = new System.Drawing.Drawing2D.Matrix();
				m.Translate(pt.X, pt.Y);
				gp.Transform(m);
				g.DrawPath(Pens.Black, gp);

				string sIndex = ((Input)o).Index.ToString();
				Point p = (Point)this.htInputInputPoints[o];
				
				p.X += pt.X - (int)(pne.Zoom * 5f);
				p.Y += pt.Y - (int)(pne.Zoom * 7f);

				g.DrawString(sIndex, f, bBlack, p);
			}

			foreach(object o in this.htOutputGraphicsPaths.Keys)
			{
				GraphicsPath gp = (GraphicsPath)((GraphicsPath)this.htOutputGraphicsPaths[o]).Clone();
				System.Drawing.Drawing2D.Matrix m = new System.Drawing.Drawing2D.Matrix();
				m.Translate(pt.X, pt.Y);
				gp.Transform(m);
				g.DrawPath(Pens.Black, gp);

				string sIndex = ((Output)o).Index.ToString();
				Point p = (Point)this.htOutputOutputPoints[o];

				p.X += pt.X - (int)(pne.Zoom * 5f);
				p.Y += pt.Y - (int)(pne.Zoom * 7f);

				g.DrawString(sIndex, f, bBlack, p);
			}

		}

		#endregion
	}

	public class SubsystemConverter : TypeConverter
	{
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			if (destinationType == typeof(string))
			{
				return true;
			}
			return base.CanConvertTo (context, destinationType);
		}

		public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
		{
			if(destinationType == typeof(string) && value is Transition)
			{
				Subsystem s = (Subsystem)value;

				return "S" + s.Index;
			}

			return base.ConvertTo (context, culture, value, destinationType);
		}

//		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
//		{
//			if (sourceType == typeof(string))
//				return true;
//
//			return base.CanConvertFrom (context, sourceType);
//		}
	}
}

