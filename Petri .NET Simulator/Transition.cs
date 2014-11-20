using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.Windows.Forms;
using System.Runtime.Serialization;

namespace PetriNetSimulator2
{
	/// <summary>
	/// Summary description for Transition.
	/// </summary>
	[Serializable]
	[TypeConverter(typeof(TransitionConverter))]
	public class Transition : ConnectableControl, ISerializable, IComparable, ICloneable, IConnector, IMetafileModel
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>

		// Properties

		#region public bool Fired
		public bool Fired
		{
			get
			{
				return this.bFired;
			}
			set
			{
				this.bFired = value;
                try 
                {
                    this.RefreshMT();
                }
                catch { }
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



		#region public ControlOrientation Orientation
		[CommonPropertiesAttribute]
		[Category("Layout")]
		public ControlOrientation Orientation
		{
			get
			{
				return this.coOrientation;
			}
			set
			{

				if (value != this.coOrientation)
					this.DefaultSize = new Size(this.DefaultSize.Height, this.DefaultSize.Width);

				this.coOrientation = value;
				this.Size = new Size(this.Height, this.Width);

				if (this.OrientationChanged != null)
					OrientationChanged(this, EventArgs.Empty);
			}
		}
		#endregion

		#region public static bool ShowConflicts
		public static bool ShowConflicts
		{
			get
			{
				return bShowConflicts;
			}
			set
			{
				bShowConflicts = value;
			}
		}
		#endregion

		#region public static bool ShowFireableTransitions
		public static bool ShowFireableTransitions
		{
			get
			{
				return bShowFireableTransitions;
			}
			set
			{
				bShowFireableTransitions = value;
			}
		}
		#endregion

		#region public static bool ShowFiredTransitions
		public static bool ShowFiredTransitions
		{
			get
			{
				return bShowFiredTransitions;
			}
			set
			{
				bShowFiredTransitions = value;
			}
		}
		#endregion

		#region public new Point Location
		[CommonProperties]
		[Category("Layout")]
		[Description("Location of transition within editor.")]
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

		#region public ArrayList PlaceParents
		public ArrayList PlaceParents
		{
			get
			{
				ArrayList al = new ArrayList();

				foreach(object o in this.Parents)
				{
					if (o is Place)
						al.Add(o);
					else if (o is Subsystem)
					{
						// Find next connectable control
						Subsystem s = (Subsystem)o;

						Connection cn = Connection.GetConnectionBetweenControls(s, this);

						ArrayList alFromControls = new ArrayList();
						ArrayList alWeights = new ArrayList();
						s.GetBaseControlConnectedToOutputPort(alFromControls, alWeights, cn.FromPort);

						foreach(ConnectableControl cc in alFromControls)
							al.Add(cc);
					}
				}

				return al;
			}
		}
		#endregion

		#region public ArrayList PlaceChilds
		public ArrayList PlaceChilds
		{
			get
			{
				ArrayList al = new ArrayList();

				foreach(object o in this.Childs)
				{
					if (o is Place)
						al.Add(o);
					else if (o is Subsystem)
					{
						// Find next connectable control
						Subsystem s = (Subsystem)o;

						Connection cn = Connection.GetConnectionBetweenControls(this, s);

						ArrayList alToControls = new ArrayList();
						ArrayList alWeights = new ArrayList();
						s.GetBaseControlConnectedToInputPort(alToControls, alWeights, cn.ToPort);

						foreach(ConnectableControl cc in alToControls)
							al.Add(cc);
					}
				}

				return al;
			}
		}
		#endregion

		// Events
		internal event EventHandler OrientationChanged;

		// Fields
		private Color cBorderColor = Color.Black;
		private Color cDefaultBorderColor = Color.Black;
		private Color cFiredBackgroundColor = Color.Orange;
		private Color cFireableBackgroundColor = Color.Red;

		private static bool bShowConflicts = false;
		private static bool bShowFireableTransitions = true;
		private static bool bShowFiredTransitions = true;

		private bool bFired = false;
		private ControlOrientation coOrientation = ControlOrientation.Vertical;

		private System.ComponentModel.Container components = null;

		#region public Transition() : base(new Size(26, 72))
		public Transition() : base(new Size(26, 72))
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			this.SetStyle(ControlStyles.UserPaint |	ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer, true);
			this.UpdateStyles();

			this.alInputPorts.Add(new Port(1, this.Region));
			this.alOutputPorts.Add(new Port(1, this.Region));
		}
		#endregion

		// Constructor for Deserialization
		#region public Transition(SerializationInfo info, StreamingContext context) : base(info, context)
		public Transition(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			this.SetStyle(ControlStyles.UserPaint |	ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer, true);
			this.UpdateStyles();

			this.Location = (Point)info.GetValue("location", typeof(Point));
			this.coOrientation = (ControlOrientation)info.GetValue("orientation", typeof(ControlOrientation));

			this.alInputPorts.Add(new Port(1, this.Region));
			this.alOutputPorts.Add(new Port(1, this.Region));

			// To set region, call resize
			this.Transition_Resize(this, EventArgs.Empty);
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
			//
			// Transition
			//
			this.Name = "Transition";
			this.Size = new System.Drawing.Size(26, 72);
			this.Resize += new System.EventHandler(this.Transition_Resize);
			this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Transition_MouseUp);
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.Transition_Paint);
		}
		#endregion

                #region public string GetXMLString()
                public string GetXMLString()
                {
                        Point pt = this.Location;
                        string s = "\t<transition id=\""+ this.GetShortString ()+"\">\n";

                        s += "\t\t<graphics><position x=\""+pt.X+"\" y=\""+pt.Y+"\" /></graphics>\n";
                        s += "\t</transition>\n";
                        return s;
                }
		#endregion

		#region public new void GetObjectData(SerializationInfo info, StreamingContext context)
		public new void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);

			info.AddValue("location", this.Location);
			info.AddValue("orientation", this.coOrientation);
		}
		#endregion


		#region private void Transition_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		private void Transition_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			// Draws Transition object
			Graphics g = e.Graphics;

			if (PetriNetDocument.AntiAlias == true)
				g.SmoothingMode = SmoothingMode.AntiAlias;

			PetriNetEditor pne = (PetriNetEditor)this.Parent;

			Rectangle rect = this.ClientRectangle;

			// Get background color based on properties
			cBackgroundColor = this.GetBackgroundColor();

			LinearGradientBrush lgb = new LinearGradientBrush(rect, Color.White, this.cBackgroundColor, LinearGradientMode.ForwardDiagonal);

			g.FillRectangle(lgb, rect);

			if (bShowConflicts == true && this.IsConflicting())
			{
				Pen pRed = new Pen(Color.Red, pne.Zoom * 7);
				g.DrawRectangle(pRed, new Rectangle(rect.X + (int)(3 * pne.Zoom), rect.Y + (int)(3 * pne.Zoom), rect.Width - (int)(6 * pne.Zoom), rect.Height - (int)(6 * pne.Zoom)));
			}

			Pen pBlack = new Pen(this.cBorderColor, pne.Zoom * 5);
			g.DrawRectangle(pBlack, 0, 0, this.Width - 1, this.Height - 1);

			Brush bBlack = new SolidBrush(Color.Black);
			StringFormat sf = new StringFormat();
			sf.Alignment = StringAlignment.Center;
			sf.LineAlignment = StringAlignment.Center;

			if (this.Orientation == ControlOrientation.Vertical)
				sf.FormatFlags = StringFormatFlags.DirectionVertical;

			Font f = new Font(this.Parent.Font.FontFamily, pne.Zoom * 7f, FontStyle.Bold);

			g.DrawString("T" + this.sIndex, f, bBlack, new Rectangle(new Point(0, 0), new Size(this.Width - (int)(pne.Zoom * 2), this.Height)), sf);
		}
		#endregion


		#region private void Transition_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		private void Transition_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			// Refresh Location property
			this.OnPropertiesChanged(this, EventArgs.Empty);
		}
		#endregion

		#region private void Transition_Resize(object sender, System.EventArgs e)
		private void Transition_Resize(object sender, System.EventArgs e)
		{
			GraphicsPath gpPath = new GraphicsPath();
			gpPath.AddRectangle(this.ClientRectangle);
			this.Region = new Region(gpPath);

			Port p = (Port)this.alInputPorts[0];
			p.PortRegion = this.Region;

			p = (Port)this.alOutputPorts[0];
			p.PortRegion = this.Region;

			this.Refresh();
		}
		#endregion


		#region public bool IsConflicting()
		public bool IsConflicting()
		{
			bool bIsConflicting = false;
			foreach(Place p in this.PlaceParents)
			{
				if (p.Childs.Count > 1)
					bIsConflicting = true;
			}

			return bIsConflicting;
		}
		#endregion


		#region public bool CanFire()
		public bool CanFire()
		{
			PetriNetDocument pnd = ((PetriNetEditor)this.Parent).Document;
			IntMatrix imF = pnd.F;

			int iCountMetConditions = 0;
			foreach(Place p in this.PlaceParents)
			{
				if (p.Tokens >= imF[pnd.Transitions.IndexOf(this), pnd.GroupedPlaces.IndexOf(p)])
				{
					iCountMetConditions++;
				}
			}

			if (iCountMetConditions == this.PlaceParents.Count && this.PlaceParents.Count != 0)
			{
				return true;
			}

			return false;
		}
		#endregion

		#region public void Fire()
		public void Fire()
		{
			PetriNetDocument pnd = ((PetriNetEditor)this.Parent).Document;
			IntMatrix imF = pnd.F;
			IntMatrix imS = pnd.S;

			// Get all tokens in parents
			foreach(Place p in this.PlaceParents)
			{
				p.Tokens -= imF[pnd.Transitions.IndexOf(this), pnd.GroupedPlaces.IndexOf(p)];
			}

			// And put them in childs
			foreach(Place p in this.PlaceChilds)
			{
				p.Tokens += imS[pnd.GroupedPlaces.IndexOf(p), pnd.Transitions.IndexOf(this)];
			}
		}
		#endregion

		#region public Color GetBackgroundColor()
		public Color GetBackgroundColor()
		{
			if (this.bSelected == true)
			{
				return this.cActiveColor;
			}
			else
			{
				if (this.CanFire() == true && bShowFireableTransitions == true)
					return this.cFireableBackgroundColor;
				else
				{
					if (this.bFired == true && bShowFiredTransitions == true)
						return this.cFiredBackgroundColor;
					else
						return this.cInactiveColor;
				}
			}
		}
		#endregion

		#region public object Clone()
		public object Clone()
		{
			Transition t = new Transition();
			t.Location = this.Location;
			t.Orientation = this.Orientation;

			return t;
		}
		#endregion

		#region public int CompareTo(object o)
		public int CompareTo(object o)
		{
			if (o is Transition)
			{
				Transition t = (Transition)o;
				return this.iIndex.CompareTo(t.Index);
			}

			throw new ArgumentException("object is not a Transition");
		}
		#endregion

		#region public override string ToString()
		public override string ToString()
		{
			return "T" + this.sIndex + " (Transition)";
		}
		#endregion

                public override string GetShortString()
                {
                                return "T" + this.sIndex;
                }


		#region *IConnector

		#region public Point GetBeginCenterPoint(Point ptBegin, Size sz)
		public Point GetBeginCenterPoint(Point ptBegin, Size sz)
		{
			// This is not actually center
			if (this.Orientation == ControlOrientation.Vertical)
				return new Point((int)(this.DefaultSize.Width/2), ptBegin.Y);
			else
				return new Point(ptBegin.X, (int)(this.DefaultSize.Height/2));
		}
		#endregion

		#region public Point GetEndCenterPoint(Point ptEnd, Size sz)
		public Point GetEndCenterPoint(Point ptEnd, Size sz)
		{
			return new Point(this.DefaultSize.Width/2, this.DefaultSize.Height/2);
		}
		#endregion

		#region public Point GetEndFixedPoint(Point ptEnd, Size sz)
		public Point GetEndFixedPoint(Point ptEnd, Size sz)
		{
			Point ptEndCenter = GetEndCenterPoint(ptEnd, sz);

			// Modify ptEnd.X coordinate to represent edge
			if (this.Orientation == ControlOrientation.Vertical)
				return new Point(ptEnd.X < ptEndCenter.X ? 0 : this.DefaultSize.Width, ptEnd.Y);
			else
				return new Point(ptEnd.X, ptEnd.Y < ptEndCenter.Y ? 0 : this.DefaultSize.Height);
		}
		#endregion

		#region public LineDirection GetBeginDirection(Point ptBegin, Point ptBeginCenter, Size sz)
		public LineDirection GetBeginDirection(Point ptBegin, Point ptBeginCenter, Size sz)
		{
			if (this.Orientation == ControlOrientation.Vertical)
			{
				if (ptBegin.X <= ptBeginCenter.X)
					return LineDirection.Left;
				else
					return LineDirection.Right;
			}
			else
			{
				if (ptBegin.Y <= ptBeginCenter.Y)
					return LineDirection.Up;
				else
					return LineDirection.Down;
			}
		}
		#endregion

		#region public LineDirection GetEndDirection(Point ptEnd, Point ptEndCenter, Size sz)
		public LineDirection GetEndDirection(Point ptEnd, Point ptEndCenter, Size sz)
		{
			if (this.Orientation == ControlOrientation.Vertical)
			{
				if (ptEnd.X <= ptEndCenter.X)
					return LineDirection.Left;
				else
					return LineDirection.Right;
			}
			else
			{
				if (ptEnd.Y <= ptEndCenter.Y)
					return LineDirection.Up;
				else
					return LineDirection.Down;
			}
		}
		#endregion

		#endregion

		#region public override bool CanConnectTo(object o)
		public override bool CanConnectTo(object o)
		{
			if (o is PlaceOperation ||
				o is PlaceResource ||
				o is PlaceControl ||
				o is PlaceOutput ||
				o is Subsystem)

				return true;

			else
				return false;
		}
		#endregion

		#region IMetafileModel Members

		public void DrawModel(Graphics g)
		{
			Point pt = this.Location;

			PetriNetEditor pne = (PetriNetEditor)this.Parent;
			Rectangle r = new Rectangle(pt, this.Size);

			LinearGradientBrush lgb = new LinearGradientBrush(r, Color.White, this.cBackgroundColor, LinearGradientMode.ForwardDiagonal);

			Brush bBack = (bShowFireableTransitions == true && this.CanFire()) ? lgb : Brushes.White;
			g.FillRectangle(bBack, r);

			if (bShowConflicts == true && this.IsConflicting())
			{
				Pen pRed = new Pen(Color.Red, pne.Zoom * 4);
				g.DrawRectangle(pRed, new Rectangle(r.X + (int)(3 * pne.Zoom), r.Y + (int)(3 * pne.Zoom), r.Width - (int)(6 * pne.Zoom), r.Height - (int)(6 * pne.Zoom)));
			}

			Pen pBlack = new Pen(this.cBorderColor, pne.Zoom * 3);
			g.DrawRectangle(pBlack, pt.X, pt.Y, this.Width, this.Height);

			Brush bBlack = new SolidBrush(Color.Black);
			StringFormat sf = new StringFormat();
			sf.Alignment = StringAlignment.Center;
			sf.LineAlignment = StringAlignment.Center;

			if (this.Orientation == ControlOrientation.Vertical)
				sf.FormatFlags = StringFormatFlags.DirectionVertical;

			Font f = new Font(this.Parent.Font.FontFamily, pne.Zoom * 7f, FontStyle.Bold);

			g.DrawString("T" + this.sIndex, f, bBlack, new Rectangle(new Point(pt.X, pt.Y), new Size(this.Width - (int)(pne.Zoom * 2), this.Height)), sf);
		}

		#endregion
	}

	public class TransitionConverter : TypeConverter
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
				Transition t = (Transition)value;

				return "T" + t.Index;
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
