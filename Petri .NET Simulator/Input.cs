using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Runtime.Serialization;

namespace PetriNetSimulator2
{
	[Serializable]
	public class Input : ConnectableControl, ISerializable, ICloneable, IComparable, IConnector, IMetafileModel
	{
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

		#region public override int Index
		[CommonProperties]
		[Category("Identification")]
		[Description("Index of this input inside editor or subsystem.")]
		public override int Index
		{
			get
			{
				return base.Index;
			}
			set
			{
				// Validate range
				if (value > 0)
				{
					// Validate if already exists
					if (this.Parent is PetriNetEditor)
					{
						PetriNetEditor pne = (PetriNetEditor)this.Parent;
						
						Input iFound = null;
						foreach(object o in pne.Objects)
						{
							if (o is Input)
							{
								Input i = (Input)o;
								if (i.Index == value)
								{
									iFound = i;
									break;
								}
							}
						}
						if (iFound != null)
						{
							MessageBox.Show("Input with this Index already exists!", "Petri .NET Simulator 2.0 - Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
							return;
						}
					}
					
					base.Index = value;
					this.Refresh();

					if (this.IndexChanged != null)
						this.IndexChanged(this, EventArgs.Empty);
				}
				else
				{
					MessageBox.Show("Index must be an integer greater than 0!", "Petri .NET Simulator 2.0 - Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}
			}
		}
		#endregion

		public event EventHandler IndexChanged;

		private System.ComponentModel.IContainer components = null;

		#region public Input() : base(new Size(48, 26))
		public Input() : base(new Size(48, 26))
		{
			// This call is required by the Windows Form Designer.
			InitializeComponent();

			this.SetStyle(ControlStyles.UserPaint |	ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer, true);
			this.UpdateStyles();

			this.alInputPorts.Add(new Port(1, this.Region));
			this.alOutputPorts.Add(new Port(1, this.Region));

			this.Resize += new EventHandler(Input_Resize);
			this.Paint += new PaintEventHandler(Input_Paint);
			this.MouseUp += new MouseEventHandler(Input_MouseUp);
		}
		#endregion

		// Constructor for Deserialization
		#region public Input(SerializationInfo info, StreamingContext context) : base(info, context)
		public Input(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			this.SetStyle(ControlStyles.UserPaint |	ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer, true);
			this.UpdateStyles();

			this.alInputPorts.Add(new Port(1, this.Region));
			this.alOutputPorts.Add(new Port(1, this.Region));

			this.Resize += new EventHandler(Input_Resize);
			this.Paint += new PaintEventHandler(Input_Paint);
			this.MouseUp += new MouseEventHandler(Input_MouseUp);

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
		}
		#endregion


		#region private void Input_Resize(object sender, EventArgs e)
		private void Input_Resize(object sender, EventArgs e)
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

		#region private void Input_Paint(object sender, PaintEventArgs e)
		private void Input_Paint(object sender, PaintEventArgs e)
		{
			Graphics g = e.Graphics;
			
			if (PetriNetDocument.AntiAlias == true)
				g.SmoothingMode = SmoothingMode.AntiAlias;

			PetriNetEditor pne = (PetriNetEditor)this.Parent;

			Rectangle r = this.ClientRectangle;
			LinearGradientBrush lgb = new LinearGradientBrush(r, Color.White, this.cBackgroundColor, LinearGradientMode.ForwardDiagonal);
	
			g.FillRectangle(lgb, r);

			Pen pBlack = new Pen(Color.Black, pne.Zoom * 7);
			g.DrawEllipse(pBlack, r);

			Brush bBlack = new SolidBrush(Color.Black);
			StringFormat sf = new StringFormat();
			sf.Alignment = StringAlignment.Center;
			sf.LineAlignment = StringAlignment.Center;
			Font f = new Font(this.Parent.Font.FontFamily, pne.Zoom * 7f,  FontStyle.Bold);
	
			g.DrawString("In" + sIndex, f, bBlack, r, sf);
		}
		#endregion

		#region private void Input_MouseUp(object sender, MouseEventArgs e)
		private void Input_MouseUp(object sender, MouseEventArgs e)
		{
			// Refresh Location property
			this.OnPropertiesChanged(this, EventArgs.Empty);
		}
		#endregion


		#region public object Clone()
		public object Clone()
		{
			Input i = new Input();
			i.Location = this.Location;

			return i;
		}
		#endregion

		#region public override string ToString()
		public override string ToString()
		{
			return "In" + this.sIndex.ToString() + " (In)";
		}
		#endregion

		#region public int CompareTo(object o)
		public int CompareTo(object o)
		{
			if (o is Input)
			{
				Input i = (Input)o;
				return this.iIndex.CompareTo(i.Index);
			}

			throw new ArgumentException("object is not a Input");
		}
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
			double alpha = Math.Atan2(this.DefaultSize.Width / 2f - ptEnd.Y, this.DefaultSize.Width / 2f - ptEnd.X) + Math.PI;
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

		#region IMetafileModel Members

		public void DrawModel(Graphics g)
		{
			Point pt = this.Location;
			Rectangle r = new Rectangle(pt, this.Size);

			PetriNetEditor pne = (PetriNetEditor)this.Parent;

			Pen pBlack = new Pen(Color.Black, pne.Zoom * 4);
			g.FillEllipse(Brushes.White, r);
			g.DrawEllipse(pBlack, r);

			Brush bBlack = new SolidBrush(Color.Black);
			StringFormat sf = new StringFormat();
			sf.Alignment = StringAlignment.Center;
			sf.LineAlignment = StringAlignment.Center;
			Font f = new Font(this.Parent.Font.FontFamily, pne.Zoom * 7f,  FontStyle.Bold);
	
			g.DrawString("In" + sIndex, f, bBlack, r, sf);
		}

		#endregion

	}
}

