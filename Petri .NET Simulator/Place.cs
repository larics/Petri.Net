using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Data;
using System.Windows.Forms;
using System.Runtime.Serialization;

namespace PetriNetSimulator2
{

	/// <summary>
	/// Summary description for Place.
	/// </summary>
	
	[TypeConverter(typeof(PlaceConverter))]
	public class Place : ConnectableControl, IComparable, ICloneable, IConnector
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>

		// Properties
	
		#region public static bool ShowCircularWaitings
		public static bool ShowCircularWaitings
		{
			get
			{
				return bShowCircularWaitings;
			}
			set
			{
				bShowCircularWaitings = value;
			}
		}
		#endregion

		#region public int Tokens
		[CommonProperties]
		[TimeInvariant]
		[StohasticInputType]
		[PeriodicInputType]
		[Category("Tokens")]
		[Description("Number that represents token value.")]
		public int Tokens
		{
			get 
			{
				return this.iTokens;
			}
			set
			{
				this.iTokens = value;
                try 
                {
                    this.RefreshMT();
                } 
                catch {}
			}
		}
		#endregion

		#region public string NameID
		[CommonProperties]
		[ControlProperties]
		[TimeInvariant]
		[StohasticInputType]
		[PeriodicInputType]
		[Category("Identification")]
		[Description("Name for this place.")]
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
					if (((PetriNetEditor)this.Parent).Document.ValidateNameID(value))
					{
						this.sName = value;

						if (this.NameOrIndexChanged != null)
							this.NameOrIndexChanged(this, new EventArgs());

						this.Refresh();
					}
					else
					{
						MessageBox.Show("Place with this NameID already exists!", "Petri .NET Simulator 2.0 - Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					}
				}
			}
		}
		#endregion

		#region public new Point Location
		[CommonProperties]
		[ControlProperties]
		[TimeInvariant]
		[StohasticInputType]
		[PeriodicInputType]
		[Category("Layout")]
		[Description("Location of place within editor.")]
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

		// Events
		internal event EventHandler NameOrIndexChanged;

		// Fields
		protected int iTokens = 0;
		protected string sName = "";
		protected static bool bShowCircularWaitings = false;

		private System.ComponentModel.Container components = null;

		#region public Place() : base(new Size(72, 72))
		public Place() : base(new Size(72, 72))
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			this.SetStyle(ControlStyles.UserPaint |	ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer, true);
			this.UpdateStyles();

			this.alInputPorts.Add(new Port(1, this.Region));
			this.alOutputPorts.Add(new Port(1, this.Region));

			// To set region, call resize
			this.Place_Resize(this, EventArgs.Empty);
		}
		#endregion

		// Constructor for Deserialization
		#region protected Place(SerializationInfo info, StreamingContext context) : base(info, context)
		protected Place(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			this.SetStyle(ControlStyles.UserPaint |	ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer, true);
			this.UpdateStyles();

			this.alInputPorts.Add(new Port(1, this.Region));
			this.alOutputPorts.Add(new Port(1, this.Region));

			// To set region, call resize
			this.Place_Resize(this, EventArgs.Empty);

			this.iTokens = info.GetInt32("tokens");
			this.sName = info.GetString("name");
			this.Location = (Point)info.GetValue("location", typeof(Point));
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
			// Place
			// 
			this.BackColor = System.Drawing.SystemColors.Control;
			this.Name = "Place";
			this.Size = new System.Drawing.Size(72, 72);
			this.Resize += new System.EventHandler(this.Place_Resize);
			this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Place_MouseUp);
		}
		#endregion


		#region protected new void GetObjectData(SerializationInfo info, StreamingContext context)
		protected new void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);

			info.AddValue("tokens", this.iTokens);
			info.AddValue("name", this.sName);
			info.AddValue("location", this.Location);
		}
		#endregion

        #region public void RefreshMT
        public delegate void InvokeDelegateRefresh();
        public void RefreshMT()
        {
            BeginInvoke(new InvokeDelegateRefresh(Refresh), null);
        }
        #endregion


		#region public static void ClearAll()
		public static void ClearAll()
		{
/*
			for(int i = Place.Instances.Count - 1; i >= 0; i--)
			{
				Place p = (Place)Place.Instances[i];
				p.Dispose();
			}
			
			iIncrementalInstances = 0;
*/
		}
		#endregion


		#region private void Place_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		private void Place_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			// Refresh Location property
			this.OnPropertiesChanged(this, EventArgs.Empty);
		}
		#endregion

		#region private void Place_Resize(object sender, System.EventArgs e)
		private void Place_Resize(object sender, System.EventArgs e)
		{
			GraphicsPath gpPath = new GraphicsPath();
			gpPath.AddEllipse(this.ClientRectangle);
			this.Region = new Region(gpPath);

			Port p = (Port)this.alInputPorts[0];
			p.PortRegion = this.Region;

			p = (Port)this.alOutputPorts[0];
			p.PortRegion = this.Region;

			this.Refresh();
		}
		#endregion


		#region public void DrawTokens(Graphics g, Brush b, RectangleF r, StringFormat sf)
		public void DrawTokens(Graphics g, Brush b, RectangleF r, StringFormat sf)
		{
			PetriNetEditor pne = (PetriNetEditor)this.Parent;

			// Draw tokens
			if (this.iTokens == 1)
			{
				g.FillEllipse(b, r.X + this.Width/2 - pne.Zoom * 4, r.Y + this.Height/2 - pne.Zoom * 4, pne.Zoom * 8, pne.Zoom * 8);
			}
			else if (this.iTokens == 2)
			{
				g.FillEllipse(b, r.X + this.Width/2 - pne.Zoom * 4, r.Y + this.Height/2 - pne.Zoom * 10, pne.Zoom * 8, pne.Zoom * 8);
				g.FillEllipse(b, r.X + this.Width/2 - pne.Zoom * 4, r.Y + this.Height/2 + pne.Zoom * 2, pne.Zoom * 8, pne.Zoom * 8);
			}
			else if (this.iTokens == 3)
			{
				g.FillEllipse(b, r.X + this.Width/2 - pne.Zoom * 4, r.Y + this.Height/2 - pne.Zoom * 10, pne.Zoom * 8, pne.Zoom * 8);
				g.FillEllipse(b, r.X + this.Width/2 - pne.Zoom * 10, r.Y + this.Height/2 + pne.Zoom * 2, pne.Zoom * 8, pne.Zoom * 8);
				g.FillEllipse(b, r.X + this.Width/2 + pne.Zoom * 2, r.Y + this.Height/2 + pne.Zoom * 2, pne.Zoom * 8, pne.Zoom * 8);
			}
			else if (this.iTokens == 4)
			{
				g.FillEllipse(b, r.X + this.Width/2 - pne.Zoom * 10, r.Y + this.Height/2 - pne.Zoom * 10, pne.Zoom * 8, pne.Zoom * 8);
				g.FillEllipse(b, r.X + this.Width/2 + pne.Zoom * 2, r.Y + this.Height/2 - pne.Zoom * 10, pne.Zoom * 8, pne.Zoom * 8);
				g.FillEllipse(b, r.X + this.Width/2 - pne.Zoom * 10, r.Y + this.Height/2 + pne.Zoom * 2, pne.Zoom * 8, pne.Zoom * 8);
				g.FillEllipse(b, r.X + this.Width/2 + pne.Zoom * 2, r.Y + this.Height/2 + pne.Zoom * 2, pne.Zoom * 8, pne.Zoom * 8);
			}
			else if (this.iTokens < 0 || this.iTokens > 4)
			{
				Font fTokens = new Font(this.Parent.Font.FontFamily, pne.Zoom * 12f, FontStyle.Bold);
				g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
				g.DrawString(this.iTokens.ToString(), fTokens, b, r, sf);
			}
		}
		#endregion

		#region public int CompareTo(object o)
		public int CompareTo(object o)
		{
			if (o is Place)
			{
				Place p = (Place)o;
				return this.iIndex.CompareTo(p.Index);
			}

			throw new ArgumentException("Invalid argument");
		}
		#endregion


//		#region public static Place GetByIndexAndName(string sName)
//		public static Place GetByIndexAndName(string sName)
//		{
//			foreach(Place p in Place.Instances)
//			{
//				if ("P" + p.Index + " - " + p.NameID == sName)
//					return p;
//			}
//
//			return null;
//		}
//		#endregion

		#region public virtual object Clone()
		public virtual object Clone()
		{
			Place p = new Place();
			p.Location = this.Location;
			p.NameID = this.NameID;
			p.Tokens = this.Tokens;
			return p;
		}
		#endregion

        public virtual string GetXMLString()
        {
            return "<UNDEFINED>";
        }

		#region *IConnector

		#region public Point GetBeginCenterPoint(Point ptBegin, Size sz)
		public Point GetBeginCenterPoint(Point ptBegin, Size sz)
		{
			return new Point(this.DefaultSize.Width/2, this.DefaultSize.Height/2);
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
			// Modify ptEnd.X coordinate to represent edge
			double alpha = Math.Atan2(this.DefaultSize.Width / 2f - ptEnd.Y, this.DefaultSize.Height / 2f - ptEnd.X) + Math.PI;
			double x = Math.Cos(alpha) * this.DefaultSize.Width / 2f;
			double y = Math.Sin(alpha) * this.DefaultSize.Width / 2f;
				
			return new Point((int)(x + this.DefaultSize.Width / 2f), (int)(y + this.DefaultSize.Width / 2f));
		}
		#endregion

		#region public LineDirection GetBeginDirection(Point ptBegin, Point ptBeginCenter, Size sz)
		public LineDirection GetBeginDirection(Point ptBegin, Point ptBeginCenter, Size sz)
		{
			if ((Math.Abs(ptBegin.X - this.DefaultSize.Width / 2) <= -(ptBegin.Y - this.DefaultSize.Height / 2)))
				return LineDirection.Up;
			else if(ptBegin.X - this.DefaultSize.Width / 2 > Math.Abs(ptBegin.Y - this.DefaultSize.Height / 2))
				return LineDirection.Right;
			else if ((Math.Abs(ptBegin.X - this.DefaultSize.Width / 2) <= (ptBegin.Y - this.DefaultSize.Height / 2)))
				return LineDirection.Down;
			else
				return LineDirection.Left;
		}
		#endregion
		
		#region public LineDirection GetEndDirection(Point ptEnd, Point ptEndCenter, Size sz)
		public LineDirection GetEndDirection(Point ptEnd, Point ptEndCenter, Size sz)
		{
			if ((Math.Abs(ptEnd.X - this.DefaultSize.Width / 2) <= -(ptEnd.Y - this.DefaultSize.Height / 2)))
				return LineDirection.Up;
			else if(ptEnd.X - this.DefaultSize.Width / 2 > Math.Abs(ptEnd.Y - this.DefaultSize.Height / 2))
				return LineDirection.Right;
			else if ((Math.Abs(ptEnd.X - this.DefaultSize.Width / 2) <= (ptEnd.Y - this.DefaultSize.Height / 2)))
				return LineDirection.Down;
			else
				return LineDirection.Left;
		}
		#endregion
	
		#endregion
	}


	public class PlaceConverter : TypeConverter
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
			if(destinationType == typeof(string) && value is Place)
			{
				Place p = (Place)value;

				return "P" + p.Index + " - " + p.NameID;
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
