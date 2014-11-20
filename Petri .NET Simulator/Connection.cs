using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace PetriNetSimulator2
{
	/// <summary>
	/// Summary description for Connection.
	/// </summary>
	[Serializable]
	public class Connection : ISelectable, ISerializable, IDisposable
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
			}
		}
		#endregion

		#region public ArrayList TokenPositions
		public ArrayList TokenPositions
		{
			get
			{
				return this.alTokenPos;
			}
			set
			{
				this.alTokenPos = value;
			}
		}
		#endregion

		#region public ConnectableControl From
		public ConnectableControl From
		{
			get
			{
				return ctrlFrom;
			}
		}
		#endregion

		#region public ConnectableControl To
		public ConnectableControl To
		{
			get
			{
				return ctrlTo;
			}
		}
		#endregion

		#region public Region ConnectionRegion
		public Region ConnectionRegion
		{
			get
			{
				return this.rConnectionRegion;
			}
		}
		#endregion

		#region public RectangleF Bounds
		public RectangleF Bounds
		{
			get
			{
				return rectfBounds;
			}
		}
		#endregion

		#region public int Weight
		[ConnectionProperties]
		[Category("Weight")]
		public int Weight
		{
			get
			{
				return this.iWeight;
			}
			set
			{
				this.iWeight = value;

				if (this.ctrlFrom.Parent is PetriNetEditor)
					this.ctrlFrom.Parent.Refresh();
			}
		}
		#endregion

		#region protected string WeightText
		protected string WeightText
		{
			get
			{
				return this.iWeight.ToString();
			}
		}
		#endregion

		#region public bool IsVirtual
		public bool IsVirtual
		{
			get
			{
				return this.bIsVirtual;
			}
		}
		#endregion

		#region Debug properties
//		[CommonPropertiesAttribute]
//		[ConnectionProperties]
//		[Category("Points")]
//		public Point Begin
//		{
//			get
//			{
//				return this.ptBegin;
//			}
//		}
//
//		[CommonPropertiesAttribute]
//		[ConnectionProperties]
//		[Category("Points")]
//		public Point BeginCenter
//		{
//			get
//			{
//				return this.ptBeginCenter;
//			}
//		}
//
//		[CommonPropertiesAttribute]
//		[ConnectionProperties]
//		[Category("Points")]
//		public Point EndCenter
//		{
//			get
//			{
//				return this.ptEndCenter;
//			}
//		}
//
//		[CommonPropertiesAttribute]
//		[ConnectionProperties]
//		[Category("Points")]
//		public Point End
//		{
//			get
//			{
//				return this.ptEnd;
//			}
//		}
//
//		[CommonPropertiesAttribute]
//		[ConnectionProperties]
//		[Category("Points")]
//		public Point EndFixed
//		{
//			get
//			{
//				return this.ptEndFixed;
//			}
//		}
//
//		[CommonPropertiesAttribute]
//		[ConnectionProperties]
//		[Category("Points")]
//		public LineDirection BeginDirection
//		{
//			get
//			{
//				return this.ldBeginDirection;
//			}
//		}
//
//		[CommonPropertiesAttribute]
//		[ConnectionProperties]
//		[Category("Points")]
//		public LineDirection EndDirection
//		{
//			get
//			{
//				return this.ldEndDirection;
//			}
//		}

//		[CommonPropertiesAttribute]
//		[ConnectionProperties]
//		[Category("Points")]
		public int FromPort
		{
			get
			{
				return this.iFromPort;
			}
		}

//		[CommonPropertiesAttribute]
//		[ConnectionProperties]
//		[Category("Points")]
		public int ToPort
		{
			get
			{
				return this.iToPort;
			}
		}

		#endregion

		// Events
		public event SelectionHandler SelectionChanged;

		// Fields
		private Region rConnectionRegion = null;
		private GraphicsPath gpConnectionPath = null;
		private RectangleF rectfBounds = new RectangleF();
		private Color cWeightNumber = Color.Beige;
		private Color cInactiveColor = Color.Black;
		private Color cActiveColor = Color.FromArgb(49, 106, 197);
		private Color cBackgroundColor = Color.Black;
		private bool bSelected = true;
		private bool bIsVirtual = false;
		private string sToString;

		private ArrayList alTokenPos = new ArrayList();

		private Hashtable htFont = new Hashtable(); //for optimization
		private AdjustableArrowCap aacArrowCap = new AdjustableArrowCap(3, 6, true);

		private EventHandler ehOrientation;
		private EventHandler ehResize;
		private ResizedEventHandler rehResized;

		//Points in Clients coordinates of Control
		private Point ptBegin;
		private Point ptBeginPort;
		private Point ptBeginInitial;
		private Point ptBeginCenter;
		private Point ptEnd;
		private Point ptEndPort;
		private Point ptEndInitial;
		private Point ptEndCenter;
		private Point ptEndFixed;
		private ConnectableControl ctrlFrom;
		private ConnectableControl ctrlTo;
		private int iFromPort;
		private int iToPort;
                public string sFrom; //Used for serialization and deserialization (not private anymore - because of PNML import)
                public string sTo; //Used for serialization and deserialization
		private int iWeight;
		private LineDirection ldBeginDirection;
		private LineDirection ldEndDirection;

		#region public void Dispose()
		public void Dispose()
		{
			ctrlFrom.Resize -= ehResize;
			ctrlFrom.Resized -= rehResized;

			ctrlTo.Resize -= ehResize;
			ctrlTo.Resized -= rehResized;

			if (ctrlFrom is Transition)
			{
				Transition t = (Transition)this.ctrlFrom;
				t.OrientationChanged -= ehOrientation;
			}
			else if (ctrlTo is Transition)
			{
				Transition t = (Transition)this.ctrlTo;
				t.OrientationChanged -= ehOrientation;
			}
		}
		#endregion

		#region public Connection(ConnectableControl ctrlFrom, ConnectableControl ctrlTo, int iFromPort, int iToPort, Point ptBeginPort, Point ptEndPort, int iWeight)
		public Connection(ConnectableControl ctrlFrom, ConnectableControl ctrlTo, int iFromPort, int iToPort, Point ptBeginPort, Point ptEndPort, int iWeight)
		{
			this.ptBeginPort = ptBeginPort;
			this.ptEndPort = ptEndPort;
			this.ctrlFrom = ctrlFrom;
			this.ctrlTo = ctrlTo;
			this.iFromPort = iFromPort;
			this.iToPort = iToPort;
			this.iWeight = iWeight;

			//Initialize
			this.Initialize();

			// Adjust because of Zoom of PetriNetEditor
			if (ctrlFrom.Parent != null)
			{
				PetriNetEditor pne = (PetriNetEditor)ctrlFrom.Parent;

				this.ptBegin = new Point(ctrlFrom.GetTopLeftPointOfOutputPort(this.iFromPort).X + ptBeginPort.X, ctrlFrom.GetTopLeftPointOfOutputPort(this.iFromPort).Y + ptBeginPort.Y);
				this.ptEnd = new Point(ctrlTo.GetTopLeftPointOfInputPort(this.iToPort).X + ptEndPort.X, ctrlTo.GetTopLeftPointOfInputPort(this.iToPort).Y + ptEndPort.Y);

				ptBegin = new Point((int)(ptBegin.X * 1 / pne.Zoom), (int)(ptBegin.Y * 1 / pne.Zoom));
				ptEnd = new Point((int)(ptEnd.X * 1 / pne.Zoom), (int)(ptEnd.Y * 1 / pne.Zoom));

				this.ptBeginInitial = ptBegin;
				this.ptEndInitial = ptEnd;

				IConnector ic = (IConnector)ctrlFrom;
				this.ptBeginCenter = ic.GetBeginCenterPoint(this.ptBegin, ctrlFrom.DefaultSize);
				this.ldBeginDirection = ic.GetBeginDirection(this.ptBegin, this.ptBeginCenter, ctrlFrom.DefaultSize);

				ic = (IConnector)ctrlTo;
				this.ptEndCenter = ic.GetEndCenterPoint(this.ptEnd, ctrlTo.DefaultSize);
				this.ptEndFixed = ic.GetEndFixedPoint(this.ptEnd, ctrlTo.DefaultSize);
				this.ldEndDirection = ic.GetEndDirection(this.ptEnd, this.ptEndCenter, ctrlTo.DefaultSize);
			}
		}
		#endregion

		#region public Connection(ConnectableControl ctrlFrom, ConnectableControl ctrlTo, int iFromPort, int iToPort, Point ptBeginPort, Point ptEndPort, int iWeight, bool bIgnored)
		public Connection(ConnectableControl ctrlFrom, ConnectableControl ctrlTo, int iFromPort, int iToPort, Point ptBeginPort, Point ptEndPort, int iWeight, bool bIgnored)
		{
			this.ptBeginPort = ptBeginPort;
			this.ptEndPort = ptEndPort;
			this.ctrlFrom = ctrlFrom;
			this.ctrlTo = ctrlTo;
			this.iFromPort = iFromPort;
			this.iToPort = iToPort;
			this.iWeight = iWeight;

			//Initialize
			this.Initialize();

			// Adjust because of Zoom of PetriNetEditor
			if (ctrlFrom.Parent != null)
			{
				PetriNetEditor pne = (PetriNetEditor)ctrlFrom.Parent;

				this.ptBegin = new Point(ctrlFrom.GetTopLeftPointOfOutputPort(this.iFromPort).X + ptBeginPort.X, ctrlFrom.GetTopLeftPointOfOutputPort(this.iFromPort).Y + ptBeginPort.Y);
				this.ptEnd = new Point(ctrlTo.GetTopLeftPointOfInputPort(this.iToPort).X + ptEndPort.X, ctrlTo.GetTopLeftPointOfInputPort(this.iToPort).Y + ptEndPort.Y);

				this.ptBeginInitial = ptBegin;
				this.ptEndInitial = ptEnd;

				IConnector ic = (IConnector)ctrlFrom;
				this.ptBeginCenter = ic.GetBeginCenterPoint(this.ptBegin, ctrlFrom.DefaultSize);
				this.ldBeginDirection = ic.GetBeginDirection(this.ptBegin, this.ptBeginCenter, ctrlFrom.DefaultSize);

				ic = (IConnector)ctrlTo;
				this.ptEndCenter = ic.GetEndCenterPoint(this.ptEnd, ctrlTo.DefaultSize);
				this.ptEndFixed = ic.GetEndFixedPoint(this.ptEnd, ctrlTo.DefaultSize);
				this.ldEndDirection = ic.GetEndDirection(this.ptEnd, this.ptEndCenter, ctrlTo.DefaultSize);
			}
		}
		#endregion

		#region private void Initialize()
		private void Initialize()
		{
			// Check if connection should be virtual
			if (this.ctrlFrom is Subsystem || this.ctrlTo is Subsystem)
			{
				this.bIsVirtual = true;

				if (this.ctrlTo is Subsystem)
						this.iWeight = this.GetInputWeightsSum((Subsystem)this.ctrlTo);
				else if (this.ctrlFrom is Subsystem)
						this.iWeight = this.GetOutputWeightsSum((Subsystem)this.ctrlFrom);
			}

			this.SetToStringField();

			// Add event handler for orientation change
			ehOrientation = new EventHandler(t_OrientationChanged);
			ehResize = new EventHandler(cc_Resize);
			rehResized = new ResizedEventHandler(cc_Resized);

			// To Adjust Begin point based on Size
			if (ctrlFrom.AllowResizing == true)
			{
				ctrlFrom.Resize += ehResize;
				ctrlFrom.Resized += rehResized;
			}

			// To Adjust Begin point based on Size
			if (ctrlTo.AllowResizing == true)
			{
				ctrlTo.Resize += ehResize;
				ctrlTo.Resized += rehResized;
			}

			// Set all other data
			if (ctrlFrom is Transition)
			{
				Transition t = (Transition)this.ctrlFrom;

				// To Refresh connection
				t.OrientationChanged += ehOrientation;
			}
			else if (ctrlTo is Transition)
			{
				Transition t = (Transition)this.ctrlTo;

				// To Refresh connection
				t.OrientationChanged += ehOrientation;
			}
		}
		#endregion

		// Constructor for Deserialization
		#region protected Connection(SerializationInfo info, StreamingContext context)
		protected Connection(SerializationInfo info, StreamingContext context)
		{
			ehOrientation = new EventHandler(t_OrientationChanged);
			ehResize = new EventHandler(cc_Resize);
			rehResized = new ResizedEventHandler(cc_Resized);

			this.ptBegin = (Point)info.GetValue("ptBegin", typeof(Point));
			this.ptBeginPort = (Point)info.GetValue("ptBeginPort", typeof(Point));
			this.ptBeginCenter = (Point)info.GetValue("ptBeginCenter", typeof(Point));
			this.ptEnd = (Point)info.GetValue("ptEnd", typeof(Point));
			this.ptEndPort = (Point)info.GetValue("ptEndPort", typeof(Point));
			this.ptEndCenter = (Point)info.GetValue("ptEndCenter", typeof(Point));
			this.ptEndFixed = (Point)info.GetValue("ptEndFixed", typeof(Point));
			this.iWeight = info.GetInt32("weight");
			this.sFrom = info.GetString("from");
			this.sTo = info.GetString("to");
			this.iFromPort = info.GetInt32("fromPort");
			this.iToPort = info.GetInt32("toPort");
		}
		#endregion


		#region void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			// Refresh indexes of connecting controls
			this.sFrom = ctrlFrom.GetType().FullName + ctrlFrom.Index;
			this.sTo = ctrlTo.GetType().FullName + ctrlTo.Index;

			info.AddValue("ptBegin", this.ptBegin);
			info.AddValue("ptBeginPort", this.ptBeginPort);
			info.AddValue("ptBeginCenter", this.ptBeginCenter);
			info.AddValue("ptEnd", this.ptEnd);
			info.AddValue("ptEndPort", this.ptEndPort);
			info.AddValue("ptEndCenter", this.ptEndCenter);
			info.AddValue("ptEndFixed", this.ptEndFixed);
			info.AddValue("weight", this.iWeight);
			info.AddValue("from", this.sFrom);
			info.AddValue("to", this.sTo);
			info.AddValue("fromPort", this.iFromPort);
			info.AddValue("toPort", this.iToPort);
		}
		#endregion


		#region public void RestoreLinks(Hashtable ht)
		public void RestoreLinks(Hashtable ht)
		{
			// This function is used only for deserialization in order to restore links
			// between parents and childs
			this.ctrlFrom = (ConnectableControl)ht[this.sFrom];
			this.ctrlTo = (ConnectableControl)ht[this.sTo];

			// Check if connection should be virtual
			if (this.ctrlFrom is Subsystem || this.ctrlTo is Subsystem)
				this.bIsVirtual = true;

			this.SetToStringField();

			ctrlFrom.Childs.Add(ctrlTo);
			ctrlTo.Parents.Add(ctrlFrom);

			if (this.ctrlFrom is Transition)
			{
				Transition t = (Transition)this.ctrlFrom;

				// To Refresh connection
				t.OrientationChanged += ehOrientation;
			}
			else if (this.ctrlTo is Transition)
			{
				Transition t = (Transition)this.ctrlTo;

				// To Refresh connection
				t.OrientationChanged += ehOrientation;
			}

			// To Adjust Begin point based on Size
			if (ctrlFrom.AllowResizing == true)
			{
				ctrlFrom.Resize += ehResize;
				ctrlFrom.Resized += rehResized;
			}

			// To Adjust Begin point based on Size
			if (ctrlTo.AllowResizing == true)
			{
				ctrlTo.Resize += ehResize;
				ctrlTo.Resized += rehResized;
			}

			// Restore line directions
            this.ldBeginDirection = ((IConnector)ctrlFrom).GetBeginDirection(this.ptBegin, this.ptBeginCenter, ctrlFrom.DefaultSize);
			this.ldEndDirection = ((IConnector)ctrlTo).GetEndDirection(this.ptEnd, this.ptEndCenter, ctrlTo.DefaultSize);
		}
		#endregion


		#region public void DrawConnection(Graphics g, PetriNetEditor pne, Color cLineColor, Color cWeightNumberColor)
		public void DrawConnection(Graphics g, PetriNetEditor pne, Color cLineColor, Color cWeightNumberColor)
		{
			// Preparing
			#region Preparing

			Point ptBegin = new Point(0, 0);
			Point ptBeginCenter = new Point(0, 0);
			Point ptEndFixed = new Point(0, 0);
			Point ptEnd = new Point(0, 0);
			Point ptEndCenter = new Point(0, 0);

			ptBegin = ctrlFrom.PointToScreen(new Point((int)(this.ptBegin.X * pne.Zoom), (int)(this.ptBegin.Y * pne.Zoom)));
			ptBegin = pne.PointToClient(ptBegin);

			ptBeginCenter = ctrlFrom.PointToScreen(new Point((int)(this.ptBeginCenter.X * pne.Zoom), (int)(this.ptBeginCenter.Y * pne.Zoom)));
			ptBeginCenter = pne.PointToClient(ptBeginCenter);

			ptEndFixed = ctrlTo.PointToScreen(new Point((int)(this.ptEndFixed.X * pne.Zoom), (int)(this.ptEndFixed.Y * pne.Zoom)));
			ptEndFixed = pne.PointToClient(ptEndFixed);

			ptEnd = ctrlTo.PointToScreen(new Point((int)(this.ptEnd.X * pne.Zoom), (int)(this.ptEnd.Y * pne.Zoom)));
			ptEnd = pne.PointToClient(ptEnd);

			ptEndCenter = ctrlTo.PointToScreen(new Point((int)(this.ptEndCenter.X * pne.Zoom), (int)(this.ptEndCenter.Y * pne.Zoom)));
			ptEndCenter = pne.PointToClient(ptEndCenter);

			#endregion

			// Drawing
			#region Actual drawing

			Font fnt = null;

			// Optimization feature
			if (!htFont.ContainsKey(pne.Zoom))
			{
				fnt = new Font(pne.Font.FontFamily, pne.Zoom * 10f, FontStyle.Bold);
				htFont.Add(pne.Zoom, fnt);
			}
			else
				fnt = (Font)htFont[pne.Zoom];

			g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

			Pen pPen = new Pen(this.cBackgroundColor, pne.Zoom * 3);
			if (this.bIsVirtual == true)
				pPen.DashStyle = DashStyle.Dash;

			pPen.CustomEndCap = this.aacArrowCap;

			Brush bNumberColor = new SolidBrush(cWeightNumberColor);

			Point[] ptaPoints = new Point[4];

			Point ptString = new Point();
			Point[] ptaCircleCenter = new Point[this.alTokenPos.Count];

			this.gpConnectionPath = new GraphicsPath();

			#region if (this.ldBeginDirection == LineDirection.Right && this.ldEndDirection == LineDirection.Left)
			if (this.ldBeginDirection == LineDirection.Right && this.ldEndDirection == LineDirection.Left)
			{
				double dAngle = Math.Atan2(ptEndFixed.Y - ptBeginCenter.Y, ptEndFixed.X - ptBeginCenter.X);

				if (dAngle > -0.1 && dAngle < 0.1)
				{
					g.DrawLine(pPen, ptBeginCenter, ptEndFixed);

					this.gpConnectionPath.AddLine(ptBegin.X, ptBegin.Y, ptEndFixed.X, ptEndFixed.Y);
					this.gpConnectionPath.Widen(pPen);

					if (this.alTokenPos.Count != 0)
					{
						for(int i = 0; i < ptaCircleCenter.Length; i++)
						{
							ptaCircleCenter[i] = new Point((int)(ptBegin.X + (int)this.alTokenPos[i] * (ptEndFixed.X - ptBegin.X)/100f),
								(int)(ptBegin.Y + (int)this.alTokenPos[i] * (ptEndFixed.Y - ptBegin.Y)/100f));
						}
					}

					ptString = new Point(ptBeginCenter.X + (ptEndFixed.X - ptBeginCenter.X)/2, (int)(ptBegin.Y + (ptEndFixed.Y - ptBegin.Y)/2 - pne.Zoom * 20));

					this.rectfBounds = this.gpConnectionPath.GetBounds();
				}
				else
				{
					ptaPoints[0] = ptBeginCenter;
					ptaPoints[1] = new Point(ptBegin.X + (int)(pne.Zoom * 150), ptBegin.Y);
					ptaPoints[2] = new Point(ptEndFixed.X - (int)(pne.Zoom * 150), ptEndFixed.Y);
					ptaPoints[3] = new Point(ptEndFixed.X, ptEndFixed.Y);

					g.DrawBezier(pPen, ptaPoints[0], ptaPoints[1], ptaPoints[2], ptaPoints[3]);

					this.gpConnectionPath.AddBezier(ptaPoints[0], ptaPoints[1], ptaPoints[2], ptaPoints[3]);
					this.rectfBounds = this.gpConnectionPath.GetBounds();

					this.gpConnectionPath.Widen(pPen);

					// Calculate point where to draw line weight based on Bezier curve formula
					int cx = 3*(ptaPoints[1].X - ptaPoints[0].X);
					int bx = 3*(ptaPoints[2].X - ptaPoints[1].X) - cx;
					int ax = ptaPoints[3].X - ptaPoints[0].X - cx - bx;

					int cy = 3*(ptaPoints[1].Y - ptaPoints[0].Y);
					int by = 3*(ptaPoints[2].Y - ptaPoints[1].Y) - cy;
					int ay = ptaPoints[3].Y - ptaPoints[0].Y - cy - by;

					if (this.alTokenPos.Count != 0)
					{
						for(int i = 0; i < ptaCircleCenter.Length; i++)
						{
							ptaCircleCenter[i] = new Point((int)(ax * Math.Pow((int)this.alTokenPos[i]/100f, 3) + bx * Math.Pow((int)this.alTokenPos[i]/100f, 2) + cx * (int)this.alTokenPos[i]/100f + ptaPoints[0].X),
								(int)(ay * Math.Pow((int)this.alTokenPos[i]/100f, 3) + by * Math.Pow((int)this.alTokenPos[i]/100f, 2) + cy * (int)this.alTokenPos[i]/100f + ptaPoints[0].Y));
						}
					}

					ptString = new Point((int)(ax * Math.Pow(0.5, 3) + bx * Math.Pow(0.5, 2) + cx * 0.5 + ptaPoints[0].X),
						(int)(ay * Math.Pow(0.5, 3) + by * Math.Pow(0.5, 2) + cy * 0.5 + ptaPoints[0].Y - pne.Zoom * 20));
				}
			}
			#endregion

			#region else if (this.ldBeginDirection == LineDirection.Right && this.ldEndDirection == LineDirection.Right)
			else if (this.ldBeginDirection == LineDirection.Right && this.ldEndDirection == LineDirection.Right)
			{
				ptaPoints[0] = ptBeginCenter;
				ptaPoints[1] = new Point(ptBegin.X + (int)(pne.Zoom * 0.8 * Math.Abs(ptBegin.X - ptEndFixed.X)), ptBegin.Y);
				ptaPoints[2] = new Point(ptEndFixed.X + (int)(pne.Zoom * 150), ptEndFixed.Y);
				ptaPoints[3] = new Point(ptEndFixed.X, ptEndFixed.Y);

				g.DrawBezier(pPen, ptaPoints[0], ptaPoints[1], ptaPoints[2], ptaPoints[3]);

				this.gpConnectionPath.AddBezier(ptaPoints[0], ptaPoints[1], ptaPoints[2], ptaPoints[3]);
				this.rectfBounds = this.gpConnectionPath.GetBounds();

				this.gpConnectionPath.Widen(pPen);

				// Calculate point where to draw line weight based on Bezier curve formula
				int cx = 3*(ptaPoints[1].X - ptaPoints[0].X);
				int bx = 3*(ptaPoints[2].X - ptaPoints[1].X) - cx;
				int ax = ptaPoints[3].X - ptaPoints[0].X - cx - bx;

				int cy = 3*(ptaPoints[1].Y - ptaPoints[0].Y);
				int by = 3*(ptaPoints[2].Y - ptaPoints[1].Y) - cy;
				int ay = ptaPoints[3].Y - ptaPoints[0].Y - cy - by;

				if (this.alTokenPos.Count != 0)
				{
					for(int i = 0; i < ptaCircleCenter.Length; i++)
					{
						ptaCircleCenter[i] = new Point((int)(ax * Math.Pow((int)this.alTokenPos[i]/100f, 3) + bx * Math.Pow((int)this.alTokenPos[i]/100f, 2) + cx * (int)this.alTokenPos[i]/100f + ptaPoints[0].X),
							(int)(ay * Math.Pow((int)this.alTokenPos[i]/100f, 3) + by * Math.Pow((int)this.alTokenPos[i]/100f, 2) + cy * (int)this.alTokenPos[i]/100f + ptaPoints[0].Y));
					}
				}

				ptString = new Point((int)(ax * Math.Pow(0.5, 3) + bx * Math.Pow(0.5, 2) + cx * 0.5 + ptaPoints[0].X),
					(int)(ay * Math.Pow(0.5, 3) + by * Math.Pow(0.5, 2) + cy * 0.5 + ptaPoints[0].Y - pne.Zoom * 20));

			}
			#endregion

			#region else if (this.ldBeginDirection == LineDirection.Right && this.ldEndDirection == LineDirection.Up)
			else if (this.ldBeginDirection == LineDirection.Right && this.ldEndDirection == LineDirection.Up)
			{
				ptaPoints[0] = ptBeginCenter;
				ptaPoints[1] = new Point(ptBegin.X + (int)(pne.Zoom * 0.8 * Math.Abs(ptBegin.X - ptEndFixed.X)), ptBegin.Y);
				if (this.ctrlFrom.Location.Y >= this.ctrlTo.Location.Y)
					ptaPoints[2] = new Point(ptEndFixed.X, ptEndFixed.Y  - (int)(pne.Zoom * 1.1 * (ptBegin.Y - ptEndFixed.Y)));
				else
					ptaPoints[2] = new Point(ptEndFixed.X, ptEndFixed.Y  - (int)(pne.Zoom * 1.1 * (ptEndFixed.Y - ptBegin.Y)));
				ptaPoints[3] = new Point(ptEndFixed.X, ptEndFixed.Y);

				g.DrawBezier(pPen, ptaPoints[0], ptaPoints[1], ptaPoints[2], ptaPoints[3]);

				this.gpConnectionPath.AddBezier(ptaPoints[0], ptaPoints[1], ptaPoints[2], ptaPoints[3]);
				this.rectfBounds = this.gpConnectionPath.GetBounds();

				this.gpConnectionPath.Widen(pPen);

				// Calculate point where to draw line weight based on Bezier curve formula
				int cx = 3*(ptaPoints[1].X - ptaPoints[0].X);
				int bx = 3*(ptaPoints[2].X - ptaPoints[1].X) - cx;
				int ax = ptaPoints[3].X - ptaPoints[0].X - cx - bx;

				int cy = 3*(ptaPoints[1].Y - ptaPoints[0].Y);
				int by = 3*(ptaPoints[2].Y - ptaPoints[1].Y) - cy;
				int ay = ptaPoints[3].Y - ptaPoints[0].Y - cy - by;

				if (this.alTokenPos.Count != 0)
				{
					for(int i = 0; i < ptaCircleCenter.Length; i++)
					{
						ptaCircleCenter[i] = new Point((int)(ax * Math.Pow((int)this.alTokenPos[i]/100f, 3) + bx * Math.Pow((int)this.alTokenPos[i]/100f, 2) + cx * (int)this.alTokenPos[i]/100f + ptaPoints[0].X),
							(int)(ay * Math.Pow((int)this.alTokenPos[i]/100f, 3) + by * Math.Pow((int)this.alTokenPos[i]/100f, 2) + cy * (int)this.alTokenPos[i]/100f + ptaPoints[0].Y));
					}
				}

				ptString = new Point((int)(ax * Math.Pow(0.5, 3) + bx * Math.Pow(0.5, 2) + cx * 0.5 + ptaPoints[0].X),
					(int)(ay * Math.Pow(0.5, 3) + by * Math.Pow(0.5, 2) + cy * 0.5 + ptaPoints[0].Y - pne.Zoom * 20));

			}
				#endregion

			#region else if (this.ldBeginDirection == LineDirection.Right && this.ldEndDirection == LineDirection.Down)
			else if (this.ldBeginDirection == LineDirection.Right && this.ldEndDirection == LineDirection.Down)
			{
				ptaPoints[0] = ptBeginCenter;
				ptaPoints[1] = new Point(ptBegin.X + (int)(pne.Zoom * 0.8 * Math.Abs(ptBegin.X - ptEndFixed.X)), ptBegin.Y);
				if (this.ctrlFrom.Location.Y >= this.ctrlTo.Location.Y)
					ptaPoints[2] = new Point(ptEndFixed.X, ptEndFixed.Y  + (int)(pne.Zoom * 1.1 * (ptBegin.Y - ptEndFixed.Y)));
				else
					ptaPoints[2] = new Point(ptEndFixed.X, ptEndFixed.Y  + (int)(pne.Zoom * 1.1 * (ptEndFixed.Y - ptBegin.Y)));
				ptaPoints[3] = new Point(ptEndFixed.X, ptEndFixed.Y);

				g.DrawBezier(pPen, ptaPoints[0], ptaPoints[1], ptaPoints[2], ptaPoints[3]);

				this.gpConnectionPath.AddBezier(ptaPoints[0], ptaPoints[1], ptaPoints[2], ptaPoints[3]);
				this.rectfBounds = this.gpConnectionPath.GetBounds();

				this.gpConnectionPath.Widen(pPen);

				// Calculate point where to draw line weight based on Bezier curve formula
				int cx = 3*(ptaPoints[1].X - ptaPoints[0].X);
				int bx = 3*(ptaPoints[2].X - ptaPoints[1].X) - cx;
				int ax = ptaPoints[3].X - ptaPoints[0].X - cx - bx;

				int cy = 3*(ptaPoints[1].Y - ptaPoints[0].Y);
				int by = 3*(ptaPoints[2].Y - ptaPoints[1].Y) - cy;
				int ay = ptaPoints[3].Y - ptaPoints[0].Y - cy - by;

				if (this.alTokenPos.Count != 0)
				{
					for(int i = 0; i < ptaCircleCenter.Length; i++)
					{
						ptaCircleCenter[i] = new Point((int)(ax * Math.Pow((int)this.alTokenPos[i]/100f, 3) + bx * Math.Pow((int)this.alTokenPos[i]/100f, 2) + cx * (int)this.alTokenPos[i]/100f + ptaPoints[0].X),
							(int)(ay * Math.Pow((int)this.alTokenPos[i]/100f, 3) + by * Math.Pow((int)this.alTokenPos[i]/100f, 2) + cy * (int)this.alTokenPos[i]/100f + ptaPoints[0].Y));
					}
				}

				ptString = new Point((int)(ax * Math.Pow(0.5, 3) + bx * Math.Pow(0.5, 2) + cx * 0.5 + ptaPoints[0].X),
					(int)(ay * Math.Pow(0.5, 3) + by * Math.Pow(0.5, 2) + cy * 0.5 + ptaPoints[0].Y - pne.Zoom * 20));

			}
			#endregion

			#region else if (this.ldBeginDirection == LineDirection.Down && this.ldEndDirection == LineDirection.Left)
			else if (this.ldBeginDirection == LineDirection.Down && this.ldEndDirection == LineDirection.Left)
			{
				ptaPoints[0] = ptBeginCenter;
				if (this.ctrlFrom.Location.Y >= this.ctrlTo.Location.Y)
					ptaPoints[1] = new Point(ptBegin.X, ptBegin.Y + (int)(pne.Zoom * 0.8 * (ptBegin.Y - ptEndFixed.Y)));
				else
					ptaPoints[1] = new Point(ptBegin.X, ptBegin.Y + (int)(pne.Zoom * 0.8 * (ptEndFixed.Y - ptBegin.Y)));

				if (this.ctrlFrom.Location.X >= this.ctrlTo.Location.X)
					ptaPoints[2] = new Point(ptEndFixed.X - (int)(pne.Zoom * 1.1 * (ptBegin.X - ptEndFixed.X)), ptEndFixed.Y);
				else
					ptaPoints[2] = new Point(ptEndFixed.X - (int)(pne.Zoom * 1.1 * (ptEndFixed.X - ptBegin.X)), ptEndFixed.Y);
				ptaPoints[3] = new Point(ptEndFixed.X, ptEndFixed.Y);

				g.DrawBezier(pPen, ptaPoints[0], ptaPoints[1], ptaPoints[2], ptaPoints[3]);

				this.gpConnectionPath.AddBezier(ptaPoints[0], ptaPoints[1], ptaPoints[2], ptaPoints[3]);
				this.rectfBounds = this.gpConnectionPath.GetBounds();

				this.gpConnectionPath.Widen(pPen);

				// Calculate point where to draw line weight based on Bezier curve formula
				int cx = 3*(ptaPoints[1].X - ptaPoints[0].X);
				int bx = 3*(ptaPoints[2].X - ptaPoints[1].X) - cx;
				int ax = ptaPoints[3].X - ptaPoints[0].X - cx - bx;

				int cy = 3*(ptaPoints[1].Y - ptaPoints[0].Y);
				int by = 3*(ptaPoints[2].Y - ptaPoints[1].Y) - cy;
				int ay = ptaPoints[3].Y - ptaPoints[0].Y - cy - by;

				if (this.alTokenPos.Count != 0)
				{
					for(int i = 0; i < ptaCircleCenter.Length; i++)
					{
						ptaCircleCenter[i] = new Point((int)(ax * Math.Pow((int)this.alTokenPos[i]/100f, 3) + bx * Math.Pow((int)this.alTokenPos[i]/100f, 2) + cx * (int)this.alTokenPos[i]/100f + ptaPoints[0].X),
							(int)(ay * Math.Pow((int)this.alTokenPos[i]/100f, 3) + by * Math.Pow((int)this.alTokenPos[i]/100f, 2) + cy * (int)this.alTokenPos[i]/100f + ptaPoints[0].Y));
					}
				}

				ptString = new Point((int)(ax * Math.Pow(0.5, 3) + bx * Math.Pow(0.5, 2) + cx * 0.5 + ptaPoints[0].X),
					(int)(ay * Math.Pow(0.5, 3) + by * Math.Pow(0.5, 2) + cy * 0.5 + ptaPoints[0].Y - pne.Zoom * 20));

			}
			#endregion

			#region else if (this.ldBeginDirection == LineDirection.Down && this.ldEndDirection == LineDirection.Right)
			else if (this.ldBeginDirection == LineDirection.Down && this.ldEndDirection == LineDirection.Right)
			{
				ptaPoints[0] = ptBeginCenter;
				if (this.ctrlFrom.Location.Y >= this.ctrlTo.Location.Y)
					ptaPoints[1] = new Point(ptBegin.X, ptBegin.Y + (int)(pne.Zoom * 0.8 * (ptBegin.Y - ptEndFixed.Y)));
				else
					ptaPoints[1] = new Point(ptBegin.X, ptBegin.Y + (int)(pne.Zoom * 0.8 * (ptEndFixed.Y - ptBegin.Y)));

				if (this.ctrlFrom.Location.X >= this.ctrlTo.Location.X)
					ptaPoints[2] = new Point(ptEndFixed.X + (int)(pne.Zoom * 1.1 * (ptBegin.X - ptEndFixed.X)), ptEndFixed.Y);
				else
					ptaPoints[2] = new Point(ptEndFixed.X + (int)(pne.Zoom * 1.1 * (ptEndFixed.X - ptBegin.X)), ptEndFixed.Y);
				ptaPoints[3] = new Point(ptEndFixed.X, ptEndFixed.Y);

				g.DrawBezier(pPen, ptaPoints[0], ptaPoints[1], ptaPoints[2], ptaPoints[3]);

				this.gpConnectionPath.AddBezier(ptaPoints[0], ptaPoints[1], ptaPoints[2], ptaPoints[3]);
				this.rectfBounds = this.gpConnectionPath.GetBounds();

				this.gpConnectionPath.Widen(pPen);

				// Calculate point where to draw line weight based on Bezier curve formula
				int cx = 3*(ptaPoints[1].X - ptaPoints[0].X);
				int bx = 3*(ptaPoints[2].X - ptaPoints[1].X) - cx;
				int ax = ptaPoints[3].X - ptaPoints[0].X - cx - bx;

				int cy = 3*(ptaPoints[1].Y - ptaPoints[0].Y);
				int by = 3*(ptaPoints[2].Y - ptaPoints[1].Y) - cy;
				int ay = ptaPoints[3].Y - ptaPoints[0].Y - cy - by;

				if (this.alTokenPos.Count != 0)
				{
					for(int i = 0; i < ptaCircleCenter.Length; i++)
					{
						ptaCircleCenter[i] = new Point((int)(ax * Math.Pow((int)this.alTokenPos[i]/100f, 3) + bx * Math.Pow((int)this.alTokenPos[i]/100f, 2) + cx * (int)this.alTokenPos[i]/100f + ptaPoints[0].X),
							(int)(ay * Math.Pow((int)this.alTokenPos[i]/100f, 3) + by * Math.Pow((int)this.alTokenPos[i]/100f, 2) + cy * (int)this.alTokenPos[i]/100f + ptaPoints[0].Y));
					}
				}

				ptString = new Point((int)(ax * Math.Pow(0.5, 3) + bx * Math.Pow(0.5, 2) + cx * 0.5 + ptaPoints[0].X),
					(int)(ay * Math.Pow(0.5, 3) + by * Math.Pow(0.5, 2) + cy * 0.5 + ptaPoints[0].Y - pne.Zoom * 20));

			}
			#endregion

			#region else if (this.ldBeginDirection == LineDirection.Down && this.ldEndDirection == LineDirection.Up)
			else if (this.ldBeginDirection == LineDirection.Down && this.ldEndDirection == LineDirection.Up)
			{
				double dAngle = Math.Atan2(ptEndFixed.Y - ptBeginCenter.Y, ptEndFixed.X - ptBeginCenter.X);

				if (dAngle > 1.4 && dAngle < 1.6)
				{
					g.DrawLine(pPen, ptBeginCenter, ptEndFixed);

					this.gpConnectionPath.AddLine(ptBegin.X, ptBegin.Y, ptEndFixed.X, ptEndFixed.Y);
					this.gpConnectionPath.Widen(pPen);

					if (this.alTokenPos.Count != 0)
					{
						for(int i = 0; i < ptaCircleCenter.Length; i++)
						{
							ptaCircleCenter[i] = new Point((int)(ptBegin.X + (int)this.alTokenPos[i] * (ptEndFixed.X - ptBegin.X)/100f),
								(int)(ptBegin.Y + (int)this.alTokenPos[i] * (ptEndFixed.Y - ptBegin.Y)/100f));
						}
					}

					ptString = new Point(ptBeginCenter.X + (ptEndFixed.X - ptBeginCenter.X)/2, (int)(ptBegin.Y + (ptEndFixed.Y - ptBegin.Y)/2 - pne.Zoom * 20));

					this.rectfBounds = this.gpConnectionPath.GetBounds();
				}
				else
				{
					ptaPoints[0] = ptBeginCenter;
					if (this.ctrlFrom.Location.Y >= this.ctrlTo.Location.Y)
						ptaPoints[1] = new Point(ptBegin.X, ptBegin.Y + (int)(pne.Zoom * 0.8 * (ptBegin.Y - ptEndFixed.Y)));
					else
						ptaPoints[1] = new Point(ptBegin.X, ptBegin.Y + (int)(pne.Zoom * 0.8 * (ptEndFixed.Y - ptBegin.Y)));

					if (this.ctrlFrom.Location.Y >= this.ctrlTo.Location.Y)
						ptaPoints[2] = new Point(ptEndFixed.X, ptEndFixed.Y  - (int)(pne.Zoom * 1.1 * (ptBegin.Y - ptEndFixed.Y)));
					else
						ptaPoints[2] = new Point(ptEndFixed.X, ptEndFixed.Y  - (int)(pne.Zoom * 1.1 * (ptEndFixed.Y - ptBegin.Y)));
					ptaPoints[3] = new Point(ptEndFixed.X, ptEndFixed.Y);

					g.DrawBezier(pPen, ptaPoints[0], ptaPoints[1], ptaPoints[2], ptaPoints[3]);

					this.gpConnectionPath.AddBezier(ptaPoints[0], ptaPoints[1], ptaPoints[2], ptaPoints[3]);
					this.rectfBounds = this.gpConnectionPath.GetBounds();

					this.gpConnectionPath.Widen(pPen);

					// Calculate point where to draw line weight based on Bezier curve formula
					int cx = 3*(ptaPoints[1].X - ptaPoints[0].X);
					int bx = 3*(ptaPoints[2].X - ptaPoints[1].X) - cx;
					int ax = ptaPoints[3].X - ptaPoints[0].X - cx - bx;

					int cy = 3*(ptaPoints[1].Y - ptaPoints[0].Y);
					int by = 3*(ptaPoints[2].Y - ptaPoints[1].Y) - cy;
					int ay = ptaPoints[3].Y - ptaPoints[0].Y - cy - by;

					if (this.alTokenPos.Count != 0)
					{
						for(int i = 0; i < ptaCircleCenter.Length; i++)
						{
							ptaCircleCenter[i] = new Point((int)(ax * Math.Pow((int)this.alTokenPos[i]/100f, 3) + bx * Math.Pow((int)this.alTokenPos[i]/100f, 2) + cx * (int)this.alTokenPos[i]/100f + ptaPoints[0].X),
								(int)(ay * Math.Pow((int)this.alTokenPos[i]/100f, 3) + by * Math.Pow((int)this.alTokenPos[i]/100f, 2) + cy * (int)this.alTokenPos[i]/100f + ptaPoints[0].Y));
						}
					}

					ptString = new Point((int)(ax * Math.Pow(0.5, 3) + bx * Math.Pow(0.5, 2) + cx * 0.5 + ptaPoints[0].X),
						(int)(ay * Math.Pow(0.5, 3) + by * Math.Pow(0.5, 2) + cy * 0.5 + ptaPoints[0].Y - pne.Zoom * 20));
				}
			}
			#endregion

			#region else if (this.ldBeginDirection == LineDirection.Down && this.ldEndDirection == LineDirection.Down)
			else if (this.ldBeginDirection == LineDirection.Down && this.ldEndDirection == LineDirection.Down)
			{
				ptaPoints[0] = ptBeginCenter;
				ptaPoints[1] = new Point(ptBegin.X, ptBegin.Y + (int)(pne.Zoom * 0.8 * Math.Abs(ptBegin.Y - ptEndFixed.Y)));
				ptaPoints[2] = new Point(ptEndFixed.X, ptEndFixed.Y + (int)(pne.Zoom * 150));
				ptaPoints[3] = new Point(ptEndFixed.X, ptEndFixed.Y);

				g.DrawBezier(pPen, ptaPoints[0], ptaPoints[1], ptaPoints[2], ptaPoints[3]);

				this.gpConnectionPath.AddBezier(ptaPoints[0], ptaPoints[1], ptaPoints[2], ptaPoints[3]);
				this.rectfBounds = this.gpConnectionPath.GetBounds();

				this.gpConnectionPath.Widen(pPen);

				// Calculate point where to draw line weight based on Bezier curve formula
				int cx = 3*(ptaPoints[1].X - ptaPoints[0].X);
				int bx = 3*(ptaPoints[2].X - ptaPoints[1].X) - cx;
				int ax = ptaPoints[3].X - ptaPoints[0].X - cx - bx;

				int cy = 3*(ptaPoints[1].Y - ptaPoints[0].Y);
				int by = 3*(ptaPoints[2].Y - ptaPoints[1].Y) - cy;
				int ay = ptaPoints[3].Y - ptaPoints[0].Y - cy - by;

				if (this.alTokenPos.Count != 0)
				{
					for(int i = 0; i < ptaCircleCenter.Length; i++)
					{
						ptaCircleCenter[i] = new Point((int)(ax * Math.Pow((int)this.alTokenPos[i]/100f, 3) + bx * Math.Pow((int)this.alTokenPos[i]/100f, 2) + cx * (int)this.alTokenPos[i]/100f + ptaPoints[0].X),
							(int)(ay * Math.Pow((int)this.alTokenPos[i]/100f, 3) + by * Math.Pow((int)this.alTokenPos[i]/100f, 2) + cy * (int)this.alTokenPos[i]/100f + ptaPoints[0].Y));
					}
				}

				ptString = new Point((int)(ax * Math.Pow(0.5, 3) + bx * Math.Pow(0.5, 2) + cx * 0.5 + ptaPoints[0].X),
					(int)(ay * Math.Pow(0.5, 3) + by * Math.Pow(0.5, 2) + cy * 0.5 + ptaPoints[0].Y - pne.Zoom * 20));
			}
			#endregion

			#region else if (this.ldBeginDirection == LineDirection.Left && this.ldEndDirection == LineDirection.Left)
			else if (this.ldBeginDirection == LineDirection.Left && this.ldEndDirection == LineDirection.Left)
			{
				ptaPoints[0] = ptBeginCenter;
				ptaPoints[1] = new Point(ptBegin.X - (int)(pne.Zoom * 0.8 * Math.Abs(ptBegin.X - ptEndFixed.X)), ptBegin.Y);
				ptaPoints[2] = new Point(ptEndFixed.X - (int)(pne.Zoom * 150), ptEndFixed.Y);
				ptaPoints[3] = new Point(ptEndFixed.X, ptEndFixed.Y);

				g.DrawBezier(pPen, ptaPoints[0], ptaPoints[1], ptaPoints[2], ptaPoints[3]);

				this.gpConnectionPath.AddBezier(ptaPoints[0], ptaPoints[1], ptaPoints[2], ptaPoints[3]);
				this.rectfBounds = this.gpConnectionPath.GetBounds();

				this.gpConnectionPath.Widen(pPen);

				// Calculate point where to draw line weight based on Bezier curve formula
				int cx = 3*(ptaPoints[1].X - ptaPoints[0].X);
				int bx = 3*(ptaPoints[2].X - ptaPoints[1].X) - cx;
				int ax = ptaPoints[3].X - ptaPoints[0].X - cx - bx;

				int cy = 3*(ptaPoints[1].Y - ptaPoints[0].Y);
				int by = 3*(ptaPoints[2].Y - ptaPoints[1].Y) - cy;
				int ay = ptaPoints[3].Y - ptaPoints[0].Y - cy - by;

				if (this.alTokenPos.Count != 0)
				{
					for(int i = 0; i < ptaCircleCenter.Length; i++)
					{
						ptaCircleCenter[i] = new Point((int)(ax * Math.Pow((int)this.alTokenPos[i]/100f, 3) + bx * Math.Pow((int)this.alTokenPos[i]/100f, 2) + cx * (int)this.alTokenPos[i]/100f + ptaPoints[0].X),
							(int)(ay * Math.Pow((int)this.alTokenPos[i]/100f, 3) + by * Math.Pow((int)this.alTokenPos[i]/100f, 2) + cy * (int)this.alTokenPos[i]/100f + ptaPoints[0].Y));
					}
				}

				ptString = new Point((int)(ax * Math.Pow(0.5, 3) + bx * Math.Pow(0.5, 2) + cx * 0.5 + ptaPoints[0].X),
					(int)(ay * Math.Pow(0.5, 3) + by * Math.Pow(0.5, 2) + cy * 0.5 + ptaPoints[0].Y - pne.Zoom * 20));

			}
			#endregion

			#region else if (this.ldBeginDirection == LineDirection.Left && this.ldEndDirection == LineDirection.Right)
			else if (this.ldBeginDirection == LineDirection.Left && this.ldEndDirection == LineDirection.Right)
			{
				double dAngle = Math.Atan2(ptEndFixed.Y - ptBeginCenter.Y, ptEndFixed.X - ptBeginCenter.X);

				if ((dAngle > Math.PI-0.1 && dAngle <= Math.PI) || (dAngle >= -Math.PI && dAngle < -Math.PI + 0.1))
				{
					g.DrawLine(pPen, ptBeginCenter, ptEndFixed);

					this.gpConnectionPath.AddLine(ptBegin.X, ptBegin.Y, ptEndFixed.X, ptEndFixed.Y);
					this.gpConnectionPath.Widen(pPen);

					if (this.alTokenPos.Count != 0)
					{
						for(int i = 0; i < ptaCircleCenter.Length; i++)
						{
							ptaCircleCenter[i] = new Point((int)(ptBegin.X + (int)this.alTokenPos[i] * (ptEndFixed.X - ptBegin.X)/100f),
								(int)(ptBegin.Y + (int)this.alTokenPos[i] * (ptEndFixed.Y - ptBegin.Y)/100f));
						}
					}

					ptString = new Point(ptBeginCenter.X + (ptEndFixed.X - ptBeginCenter.X)/2, (int)(ptBegin.Y + (ptEndFixed.Y - ptBegin.Y)/2 - pne.Zoom * 20));

					this.rectfBounds = this.gpConnectionPath.GetBounds();
				}
				else
				{
					ptaPoints[0] = ptBeginCenter;
					ptaPoints[1] = new Point(ptBegin.X - (int)(pne.Zoom * 150), ptBegin.Y);
					ptaPoints[2] = new Point(ptEndFixed.X + (int)(pne.Zoom * 150), ptEndFixed.Y);
					ptaPoints[3] = new Point(ptEndFixed.X, ptEndFixed.Y);

					g.DrawBezier(pPen, ptaPoints[0], ptaPoints[1], ptaPoints[2], ptaPoints[3]);

					this.gpConnectionPath.AddBezier(ptaPoints[0], ptaPoints[1], ptaPoints[2], ptaPoints[3]);
					this.rectfBounds = this.gpConnectionPath.GetBounds();

					this.gpConnectionPath.Widen(pPen);

					// Calculate point where to draw line weight based on Bezier curve formula
					int cx = 3*(ptaPoints[1].X - ptaPoints[0].X);
					int bx = 3*(ptaPoints[2].X - ptaPoints[1].X) - cx;
					int ax = ptaPoints[3].X - ptaPoints[0].X - cx - bx;

					int cy = 3*(ptaPoints[1].Y - ptaPoints[0].Y);
					int by = 3*(ptaPoints[2].Y - ptaPoints[1].Y) - cy;
					int ay = ptaPoints[3].Y - ptaPoints[0].Y - cy - by;

					if (this.alTokenPos.Count != 0)
					{
						for(int i = 0; i < ptaCircleCenter.Length; i++)
						{
							ptaCircleCenter[i] = new Point((int)(ax * Math.Pow((int)this.alTokenPos[i]/100f, 3) + bx * Math.Pow((int)this.alTokenPos[i]/100f, 2) + cx * (int)this.alTokenPos[i]/100f + ptaPoints[0].X),
								(int)(ay * Math.Pow((int)this.alTokenPos[i]/100f, 3) + by * Math.Pow((int)this.alTokenPos[i]/100f, 2) + cy * (int)this.alTokenPos[i]/100f + ptaPoints[0].Y));
						}
					}

					ptString = new Point((int)(ax * Math.Pow(0.5, 3) + bx * Math.Pow(0.5, 2) + cx * 0.5 + ptaPoints[0].X),
						(int)(ay * Math.Pow(0.5, 3) + by * Math.Pow(0.5, 2) + cy * 0.5 + ptaPoints[0].Y - pne.Zoom * 20));
				}
			}
			#endregion

			#region else if (this.ldBeginDirection == LineDirection.Left && this.ldEndDirection == LineDirection.Up)
			else if (this.ldBeginDirection == LineDirection.Left && this.ldEndDirection == LineDirection.Up)
			{
				ptaPoints[0] = ptBeginCenter;
				ptaPoints[1] = new Point(ptBegin.X - (int)(pne.Zoom * 0.8 * Math.Abs(ptBegin.X - ptEndFixed.X)), ptBegin.Y);
				ptaPoints[2] = new Point(ptEndFixed.X, ptEndFixed.Y - (int)(pne.Zoom * 150));
				if (this.ctrlFrom.Location.Y >= this.ctrlTo.Location.Y)
					ptaPoints[2] = new Point(ptEndFixed.X, ptEndFixed.Y  - (int)(pne.Zoom * 1.1 * (ptBegin.Y - ptEndFixed.Y)));
				else
					ptaPoints[2] = new Point(ptEndFixed.X, ptEndFixed.Y  - (int)(pne.Zoom * 1.1 * (ptEndFixed.Y - ptBegin.Y)));
				ptaPoints[3] = new Point(ptEndFixed.X, ptEndFixed.Y);

				g.DrawBezier(pPen, ptaPoints[0], ptaPoints[1], ptaPoints[2], ptaPoints[3]);

				this.gpConnectionPath.AddBezier(ptaPoints[0], ptaPoints[1], ptaPoints[2], ptaPoints[3]);
				this.rectfBounds = this.gpConnectionPath.GetBounds();

				this.gpConnectionPath.Widen(pPen);

				// Calculate point where to draw line weight based on Bezier curve formula
				int cx = 3*(ptaPoints[1].X - ptaPoints[0].X);
				int bx = 3*(ptaPoints[2].X - ptaPoints[1].X) - cx;
				int ax = ptaPoints[3].X - ptaPoints[0].X - cx - bx;

				int cy = 3*(ptaPoints[1].Y - ptaPoints[0].Y);
				int by = 3*(ptaPoints[2].Y - ptaPoints[1].Y) - cy;
				int ay = ptaPoints[3].Y - ptaPoints[0].Y - cy - by;

				if (this.alTokenPos.Count != 0)
				{
					for(int i = 0; i < ptaCircleCenter.Length; i++)
					{
						ptaCircleCenter[i] = new Point((int)(ax * Math.Pow((int)this.alTokenPos[i]/100f, 3) + bx * Math.Pow((int)this.alTokenPos[i]/100f, 2) + cx * (int)this.alTokenPos[i]/100f + ptaPoints[0].X),
							(int)(ay * Math.Pow((int)this.alTokenPos[i]/100f, 3) + by * Math.Pow((int)this.alTokenPos[i]/100f, 2) + cy * (int)this.alTokenPos[i]/100f + ptaPoints[0].Y));
					}
				}

				ptString = new Point((int)(ax * Math.Pow(0.5, 3) + bx * Math.Pow(0.5, 2) + cx * 0.5 + ptaPoints[0].X),
					(int)(ay * Math.Pow(0.5, 3) + by * Math.Pow(0.5, 2) + cy * 0.5 + ptaPoints[0].Y - pne.Zoom * 20));

			}
				#endregion

			#region else if (this.ldBeginDirection == LineDirection.Left && this.ldEndDirection == LineDirection.Down)
			else if (this.ldBeginDirection == LineDirection.Left && this.ldEndDirection == LineDirection.Down)
			{
				ptaPoints[0] = ptBeginCenter;
				ptaPoints[1] = new Point(ptBegin.X - (int)(pne.Zoom * 0.8 * Math.Abs(ptBegin.X - ptEndFixed.X)), ptBegin.Y);
				ptaPoints[2] = new Point(ptEndFixed.X, ptEndFixed.Y - (int)(pne.Zoom * 150));
				if (this.ctrlFrom.Location.Y >= this.ctrlTo.Location.Y)
					ptaPoints[2] = new Point(ptEndFixed.X, ptEndFixed.Y  + (int)(pne.Zoom * 1.1 * (ptBegin.Y - ptEndFixed.Y)));
				else
					ptaPoints[2] = new Point(ptEndFixed.X, ptEndFixed.Y  + (int)(pne.Zoom * 1.1 * (ptEndFixed.Y - ptBegin.Y)));
				ptaPoints[3] = new Point(ptEndFixed.X, ptEndFixed.Y);

				g.DrawBezier(pPen, ptaPoints[0], ptaPoints[1], ptaPoints[2], ptaPoints[3]);

				this.gpConnectionPath.AddBezier(ptaPoints[0], ptaPoints[1], ptaPoints[2], ptaPoints[3]);
				this.rectfBounds = this.gpConnectionPath.GetBounds();

				this.gpConnectionPath.Widen(pPen);

				// Calculate point where to draw line weight based on Bezier curve formula
				int cx = 3*(ptaPoints[1].X - ptaPoints[0].X);
				int bx = 3*(ptaPoints[2].X - ptaPoints[1].X) - cx;
				int ax = ptaPoints[3].X - ptaPoints[0].X - cx - bx;

				int cy = 3*(ptaPoints[1].Y - ptaPoints[0].Y);
				int by = 3*(ptaPoints[2].Y - ptaPoints[1].Y) - cy;
				int ay = ptaPoints[3].Y - ptaPoints[0].Y - cy - by;

				if (this.alTokenPos.Count != 0)
				{
					for(int i = 0; i < ptaCircleCenter.Length; i++)
					{
						ptaCircleCenter[i] = new Point((int)(ax * Math.Pow((int)this.alTokenPos[i]/100f, 3) + bx * Math.Pow((int)this.alTokenPos[i]/100f, 2) + cx * (int)this.alTokenPos[i]/100f + ptaPoints[0].X),
							(int)(ay * Math.Pow((int)this.alTokenPos[i]/100f, 3) + by * Math.Pow((int)this.alTokenPos[i]/100f, 2) + cy * (int)this.alTokenPos[i]/100f + ptaPoints[0].Y));
					}
				}

				ptString = new Point((int)(ax * Math.Pow(0.5, 3) + bx * Math.Pow(0.5, 2) + cx * 0.5 + ptaPoints[0].X),
					(int)(ay * Math.Pow(0.5, 3) + by * Math.Pow(0.5, 2) + cy * 0.5 + ptaPoints[0].Y - pne.Zoom * 20));

			}
			#endregion

			#region else if (this.ldBeginDirection == LineDirection.Up && this.ldEndDirection == LineDirection.Left)
			else if (this.ldBeginDirection == LineDirection.Up && this.ldEndDirection == LineDirection.Left)
			{
				ptaPoints[0] = ptBeginCenter;
				if (this.ctrlFrom.Location.Y >= this.ctrlTo.Location.Y)
					ptaPoints[1] = new Point(ptBegin.X, ptBegin.Y - (int)(pne.Zoom * 0.8 * (ptBegin.Y - ptEndFixed.Y)));
				else
					ptaPoints[1] = new Point(ptBegin.X, ptBegin.Y - (int)(pne.Zoom * 0.8 * (ptEndFixed.Y - ptBegin.Y)));

				if (this.ctrlFrom.Location.X >= this.ctrlTo.Location.X)
					ptaPoints[2] = new Point(ptEndFixed.X - (int)(pne.Zoom * 1.1 * (ptBegin.X - ptEndFixed.X)), ptEndFixed.Y);
				else
					ptaPoints[2] = new Point(ptEndFixed.X - (int)(pne.Zoom * 1.1 * (ptEndFixed.X - ptBegin.X)), ptEndFixed.Y);
				ptaPoints[3] = new Point(ptEndFixed.X, ptEndFixed.Y);

				g.DrawBezier(pPen, ptaPoints[0], ptaPoints[1], ptaPoints[2], ptaPoints[3]);

				this.gpConnectionPath.AddBezier(ptaPoints[0], ptaPoints[1], ptaPoints[2], ptaPoints[3]);
				this.rectfBounds = this.gpConnectionPath.GetBounds();

				this.gpConnectionPath.Widen(pPen);

				// Calculate point where to draw line weight based on Bezier curve formula
				int cx = 3*(ptaPoints[1].X - ptaPoints[0].X);
				int bx = 3*(ptaPoints[2].X - ptaPoints[1].X) - cx;
				int ax = ptaPoints[3].X - ptaPoints[0].X - cx - bx;

				int cy = 3*(ptaPoints[1].Y - ptaPoints[0].Y);
				int by = 3*(ptaPoints[2].Y - ptaPoints[1].Y) - cy;
				int ay = ptaPoints[3].Y - ptaPoints[0].Y - cy - by;

				if (this.alTokenPos.Count != 0)
				{
					for(int i = 0; i < ptaCircleCenter.Length; i++)
					{
						ptaCircleCenter[i] = new Point((int)(ax * Math.Pow((int)this.alTokenPos[i]/100f, 3) + bx * Math.Pow((int)this.alTokenPos[i]/100f, 2) + cx * (int)this.alTokenPos[i]/100f + ptaPoints[0].X),
							(int)(ay * Math.Pow((int)this.alTokenPos[i]/100f, 3) + by * Math.Pow((int)this.alTokenPos[i]/100f, 2) + cy * (int)this.alTokenPos[i]/100f + ptaPoints[0].Y));
					}
				}

				ptString = new Point((int)(ax * Math.Pow(0.5, 3) + bx * Math.Pow(0.5, 2) + cx * 0.5 + ptaPoints[0].X),
					(int)(ay * Math.Pow(0.5, 3) + by * Math.Pow(0.5, 2) + cy * 0.5 + ptaPoints[0].Y - pne.Zoom * 20));

			}
			#endregion

			#region else if (this.ldBeginDirection == LineDirection.Up && this.ldEndDirection == LineDirection.Right)
			else if (this.ldBeginDirection == LineDirection.Up && this.ldEndDirection == LineDirection.Right)
			{
				ptaPoints[0] = ptBeginCenter;
				if (this.ctrlFrom.Location.Y >= this.ctrlTo.Location.Y)
					ptaPoints[1] = new Point(ptBegin.X, ptBegin.Y - (int)(pne.Zoom * 0.8 * (ptBegin.Y - ptEndFixed.Y)));
				else
					ptaPoints[1] = new Point(ptBegin.X, ptBegin.Y - (int)(pne.Zoom * 0.8 * (ptEndFixed.Y - ptBegin.Y)));

				if (this.ctrlFrom.Location.X >= this.ctrlTo.Location.X)
					ptaPoints[2] = new Point(ptEndFixed.X + (int)(pne.Zoom * 1.1 * (ptBegin.X - ptEndFixed.X)), ptEndFixed.Y);
				else
					ptaPoints[2] = new Point(ptEndFixed.X + (int)(pne.Zoom * 1.1 * (ptEndFixed.X - ptBegin.X)), ptEndFixed.Y);
				ptaPoints[3] = new Point(ptEndFixed.X, ptEndFixed.Y);

				g.DrawBezier(pPen, ptaPoints[0], ptaPoints[1], ptaPoints[2], ptaPoints[3]);

				this.gpConnectionPath.AddBezier(ptaPoints[0], ptaPoints[1], ptaPoints[2], ptaPoints[3]);
				this.rectfBounds = this.gpConnectionPath.GetBounds();

				this.gpConnectionPath.Widen(pPen);

				// Calculate point where to draw line weight based on Bezier curve formula
				int cx = 3*(ptaPoints[1].X - ptaPoints[0].X);
				int bx = 3*(ptaPoints[2].X - ptaPoints[1].X) - cx;
				int ax = ptaPoints[3].X - ptaPoints[0].X - cx - bx;

				int cy = 3*(ptaPoints[1].Y - ptaPoints[0].Y);
				int by = 3*(ptaPoints[2].Y - ptaPoints[1].Y) - cy;
				int ay = ptaPoints[3].Y - ptaPoints[0].Y - cy - by;

				if (this.alTokenPos.Count != 0)
				{
					for(int i = 0; i < ptaCircleCenter.Length; i++)
					{
						ptaCircleCenter[i] = new Point((int)(ax * Math.Pow((int)this.alTokenPos[i]/100f, 3) + bx * Math.Pow((int)this.alTokenPos[i]/100f, 2) + cx * (int)this.alTokenPos[i]/100f + ptaPoints[0].X),
							(int)(ay * Math.Pow((int)this.alTokenPos[i]/100f, 3) + by * Math.Pow((int)this.alTokenPos[i]/100f, 2) + cy * (int)this.alTokenPos[i]/100f + ptaPoints[0].Y));
					}
				}

				ptString = new Point((int)(ax * Math.Pow(0.5, 3) + bx * Math.Pow(0.5, 2) + cx * 0.5 + ptaPoints[0].X),
					(int)(ay * Math.Pow(0.5, 3) + by * Math.Pow(0.5, 2) + cy * 0.5 + ptaPoints[0].Y - pne.Zoom * 20));

			}
			#endregion

			#region else if (this.ldBeginDirection == LineDirection.Up && this.ldEndDirection == LineDirection.Up)
			else if (this.ldBeginDirection == LineDirection.Up && this.ldEndDirection == LineDirection.Up)
			{
				ptaPoints[0] = ptBeginCenter;
				ptaPoints[1] = new Point(ptBegin.X, ptBegin.Y - (int)(pne.Zoom * 0.8 * Math.Abs(ptBegin.Y - ptEndFixed.Y)));
				ptaPoints[2] = new Point(ptEndFixed.X, ptEndFixed.Y - (int)(pne.Zoom * 150));
				ptaPoints[3] = new Point(ptEndFixed.X, ptEndFixed.Y);

				g.DrawBezier(pPen, ptaPoints[0], ptaPoints[1], ptaPoints[2], ptaPoints[3]);

				this.gpConnectionPath.AddBezier(ptaPoints[0], ptaPoints[1], ptaPoints[2], ptaPoints[3]);
				this.rectfBounds = this.gpConnectionPath.GetBounds();

				this.gpConnectionPath.Widen(pPen);

				// Calculate point where to draw line weight based on Bezier curve formula
				int cx = 3*(ptaPoints[1].X - ptaPoints[0].X);
				int bx = 3*(ptaPoints[2].X - ptaPoints[1].X) - cx;
				int ax = ptaPoints[3].X - ptaPoints[0].X - cx - bx;

				int cy = 3*(ptaPoints[1].Y - ptaPoints[0].Y);
				int by = 3*(ptaPoints[2].Y - ptaPoints[1].Y) - cy;
				int ay = ptaPoints[3].Y - ptaPoints[0].Y - cy - by;

				if (this.alTokenPos.Count != 0)
				{
					for(int i = 0; i < ptaCircleCenter.Length; i++)
					{
						ptaCircleCenter[i] = new Point((int)(ax * Math.Pow((int)this.alTokenPos[i]/100f, 3) + bx * Math.Pow((int)this.alTokenPos[i]/100f, 2) + cx * (int)this.alTokenPos[i]/100f + ptaPoints[0].X),
							(int)(ay * Math.Pow((int)this.alTokenPos[i]/100f, 3) + by * Math.Pow((int)this.alTokenPos[i]/100f, 2) + cy * (int)this.alTokenPos[i]/100f + ptaPoints[0].Y));
					}
				}

				ptString = new Point((int)(ax * Math.Pow(0.5, 3) + bx * Math.Pow(0.5, 2) + cx * 0.5 + ptaPoints[0].X),
					(int)(ay * Math.Pow(0.5, 3) + by * Math.Pow(0.5, 2) + cy * 0.5 + ptaPoints[0].Y - pne.Zoom * 20));
			}
			#endregion

			#region else if (this.ldBeginDirection == LineDirection.Up && this.ldEndDirection == LineDirection.Down)
			else if (this.ldBeginDirection == LineDirection.Up && this.ldEndDirection == LineDirection.Down)
			{
				double dAngle = Math.Atan2(ptEndFixed.Y - ptBeginCenter.Y, ptEndFixed.X - ptBeginCenter.X);

				if (dAngle + 2 * Math.PI > 4.6 && dAngle + 2 * Math.PI < 4.8)
				{
					g.DrawLine(pPen, ptBeginCenter, ptEndFixed);

					this.gpConnectionPath.AddLine(ptBegin.X, ptBegin.Y, ptEndFixed.X, ptEndFixed.Y);
					this.gpConnectionPath.Widen(pPen);

					if (this.alTokenPos.Count != 0)
					{
						for(int i = 0; i < ptaCircleCenter.Length; i++)
						{
							ptaCircleCenter[i] = new Point((int)(ptBegin.X + (int)this.alTokenPos[i] * (ptEndFixed.X - ptBegin.X)/100f),
								(int)(ptBegin.Y + (int)this.alTokenPos[i] * (ptEndFixed.Y - ptBegin.Y)/100f));
						}
					}

					ptString = new Point(ptBeginCenter.X + (ptEndFixed.X - ptBeginCenter.X)/2, (int)(ptBegin.Y + (ptEndFixed.Y - ptBegin.Y)/2 - pne.Zoom * 20));

					this.rectfBounds = this.gpConnectionPath.GetBounds();
				}
				else
				{
					ptaPoints[0] = ptBeginCenter;
					if (this.ctrlFrom.Location.Y >= this.ctrlTo.Location.Y)
						ptaPoints[1] = new Point(ptBegin.X, ptBegin.Y - (int)(pne.Zoom * 0.8 * (ptBegin.Y - ptEndFixed.Y)));
					else
						ptaPoints[1] = new Point(ptBegin.X, ptBegin.Y - (int)(pne.Zoom * 0.8 * (ptEndFixed.Y - ptBegin.Y)));

					if (this.ctrlFrom.Location.Y >= this.ctrlTo.Location.Y)
						ptaPoints[2] = new Point(ptEndFixed.X, ptEndFixed.Y  + (int)(pne.Zoom * 1.1 * (ptBegin.Y - ptEndFixed.Y)));
					else
						ptaPoints[2] = new Point(ptEndFixed.X, ptEndFixed.Y  + (int)(pne.Zoom * 1.1 * (ptEndFixed.Y - ptBegin.Y)));
					ptaPoints[3] = new Point(ptEndFixed.X, ptEndFixed.Y);

					g.DrawBezier(pPen, ptaPoints[0], ptaPoints[1], ptaPoints[2], ptaPoints[3]);

					this.gpConnectionPath.AddBezier(ptaPoints[0], ptaPoints[1], ptaPoints[2], ptaPoints[3]);
					this.rectfBounds = this.gpConnectionPath.GetBounds();

					this.gpConnectionPath.Widen(pPen);

					// Calculate point where to draw line weight based on Bezier curve formula
					int cx = 3*(ptaPoints[1].X - ptaPoints[0].X);
					int bx = 3*(ptaPoints[2].X - ptaPoints[1].X) - cx;
					int ax = ptaPoints[3].X - ptaPoints[0].X - cx - bx;

					int cy = 3*(ptaPoints[1].Y - ptaPoints[0].Y);
					int by = 3*(ptaPoints[2].Y - ptaPoints[1].Y) - cy;
					int ay = ptaPoints[3].Y - ptaPoints[0].Y - cy - by;

					if (this.alTokenPos.Count != 0)
					{
						for(int i = 0; i < ptaCircleCenter.Length; i++)
						{
							ptaCircleCenter[i] = new Point((int)(ax * Math.Pow((int)this.alTokenPos[i]/100f, 3) + bx * Math.Pow((int)this.alTokenPos[i]/100f, 2) + cx * (int)this.alTokenPos[i]/100f + ptaPoints[0].X),
								(int)(ay * Math.Pow((int)this.alTokenPos[i]/100f, 3) + by * Math.Pow((int)this.alTokenPos[i]/100f, 2) + cy * (int)this.alTokenPos[i]/100f + ptaPoints[0].Y));
						}
					}

					ptString = new Point((int)(ax * Math.Pow(0.5, 3) + bx * Math.Pow(0.5, 2) + cx * 0.5 + ptaPoints[0].X),
						(int)(ay * Math.Pow(0.5, 3) + by * Math.Pow(0.5, 2) + cy * 0.5 + ptaPoints[0].Y - pne.Zoom * 20));
				}
			}
			#endregion

			this.rConnectionRegion = new Region(this.gpConnectionPath);

			if (this.alTokenPos.Count != 0)
			{
				foreach(int i in this.alTokenPos)
				{
					if (i != 0)
					{
						Point ptCircleCenter = ptaCircleCenter[this.alTokenPos.IndexOf(i)];
						g.FillEllipse(Brushes.Black, ptCircleCenter.X - pne.Zoom * 5, ptCircleCenter.Y - pne.Zoom * 5, pne.Zoom * 10, pne.Zoom * 10);
					}
				}
			}

			if (pne.ShowWeight1 == true || this.iWeight != 1)
				g.DrawString(this.WeightText, fnt, bNumberColor, ptString);


			#endregion
		}
		#endregion

		#region public void DrawConnection(Graphics g, PetriNetEditor pne)
		public void DrawConnection(Graphics g, PetriNetEditor pne)
		{
			this.DrawConnection(g, pne, this.cBackgroundColor, cWeightNumber);
		}
		#endregion


                #region public string GetXMLString()
                public string GetXMLString()
                {
                    string s = "\t<arc id=\""+this.GetShortString ()+"\" source=\""+ ctrlFrom.GetShortString() +"\" target=\"" + ctrlTo.GetShortString() +"\">\n";
                    if (this.Weight != 0)
                        s += "\t\t<inscription><text>" + this.Weight + "</text></inscription>\n";
                    s += "\t</arc>\n";

                        return s;
                }
		#endregion


		#region public static Connection GetConnectionBetweenControls(PetriNetEditor pne, ConnectableControl c1, ConnectableControl c2)
		public static Connection GetConnectionBetweenControls(PetriNetEditor pne, ConnectableControl c1, ConnectableControl c2)
		{
			Connection cnFound = null;

			foreach(Connection cn in pne.Connections)
			{
				if ((cn.ctrlFrom == c1 && cn.ctrlTo == c2))
				{
					cnFound = cn;
					break;
				}
			}

			return cnFound;
		}
		#endregion

		#region public static Connection GetConnectionBetweenControls(ConnectableControl c1, ConnectableControl c2)
		public static Connection GetConnectionBetweenControls(ConnectableControl c1, ConnectableControl c2)
		{
			// This function exists only for compatibility

			PetriNetEditor pne = (PetriNetEditor)c1.Parent;

			return GetConnectionBetweenControls(pne, c1, c2);
		}
		#endregion

		#region public int GetInputWeightsSum(Subsystem s)
		public int GetInputWeightsSum(Subsystem s)
		{
			ArrayList alControls = new ArrayList();
			ArrayList alWeights = new ArrayList();

			s.GetBaseControlConnectedToInputPort(alControls, alWeights, this.ToPort);
			int i = 0;
			foreach(int w in alWeights)
				i += w;

			return i;
		}
		#endregion

		#region public int GetOutputWeightsSum(Subsystem s)
		public int GetOutputWeightsSum(Subsystem s)
		{
			ArrayList alControls = new ArrayList();
			ArrayList alWeights = new ArrayList();

			s.GetBaseControlConnectedToOutputPort(alControls, alWeights, this.FromPort);
			int i = 0;
			foreach(int w in alWeights)
				i += w;

			return i;
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

		#region private void SetToStringField()
		private void SetToStringField()
		{
			// Set sToString field
			// this is optimization for ToString method

			string sCtrlFrom = ctrlFrom.ToString();
			int iIndex = sCtrlFrom.IndexOf(" ");
			sCtrlFrom = sCtrlFrom.Remove(iIndex, sCtrlFrom.Length - iIndex);

			string sCtrlTo = ctrlTo.ToString();
			iIndex = sCtrlTo.IndexOf(" ");
			sCtrlTo = sCtrlTo.Remove(iIndex, sCtrlTo.Length - iIndex);

			this.sToString = sCtrlFrom + " -> " + sCtrlTo + " (Connection)";
		}
		#endregion

		#region private void t_OrientationChanged(object sender, EventArgs e)
		private void t_OrientationChanged(object sender, EventArgs e)
		{
			if (sender is Transition)
			{
				Transition t = (Transition)sender;
				if (this.ctrlFrom == t)
				{
					if (this.ldBeginDirection == LineDirection.Left)
					{
						this.ldBeginDirection = LineDirection.Down;
					}
					else if (this.ldBeginDirection == LineDirection.Right)
					{
						this.ldBeginDirection = LineDirection.Up;
					}
					else if (this.ldBeginDirection == LineDirection.Up)
					{
						this.ldBeginDirection = LineDirection.Right;
					}
					else if (this.ldBeginDirection == LineDirection.Down)
					{
						this.ldBeginDirection = LineDirection.Left;
					}

					this.ptBegin = new Point(ptBegin.Y, ptBegin.X);
					this.ptBeginCenter = new Point(ptBeginCenter.Y, ptBeginCenter.X);

				}
				else if (this.ctrlTo == t)
				{
					if (this.ldEndDirection == LineDirection.Left)
					{
						this.ptEndFixed = new Point(ptEndFixed.Y, t.DefaultSize.Height);
						this.ldEndDirection = LineDirection.Down;
					}
					else if (this.ldEndDirection == LineDirection.Right)
					{
						this.ptEndFixed = new Point(ptEndFixed.Y, 0);
						this.ldEndDirection = LineDirection.Up;
					}
					else if (this.ldEndDirection == LineDirection.Up)
					{
						this.ptEndFixed = new Point(t.DefaultSize.Width, ptEndFixed.X);
						this.ldEndDirection = LineDirection.Right;
					}
					else if (this.ldEndDirection == LineDirection.Down)
					{
						this.ptEndFixed = new Point(0, ptEndFixed.X);
						this.ldEndDirection = LineDirection.Left;
					}

					this.ptEndCenter = new Point(this.ptEndCenter.Y, this.ptEndCenter.X);
				}

				this.ctrlFrom.Parent.Refresh();
			}
		}
		#endregion

		#region private void cc_Resize(object sender, EventArgs e)
		private void cc_Resize(object sender, EventArgs e)
		{
			if (this.ctrlFrom.Parent is PetriNetEditor)
			{
				PetriNetEditor pne = (PetriNetEditor)this.ctrlFrom.Parent;

				if (this.ctrlFrom == sender)
				{
					this.ptBegin = new Point(ctrlFrom.GetTopLeftPointOfOutputPort(this.iFromPort).X + ptBeginPort.X, ctrlFrom.GetTopLeftPointOfOutputPort(this.iFromPort).Y + ptBeginPort.Y);

					IConnector ic = (IConnector)ctrlFrom;
					Size sz = new Size((int)(ctrlFrom.Size.Width * 1/pne.Zoom), (int)(ctrlFrom.Size.Height * 1/pne.Zoom));
					this.ptBeginCenter = ic.GetBeginCenterPoint(this.ptBegin, sz);
					this.ldBeginDirection = ic.GetBeginDirection(this.ptBegin, this.ptBeginCenter, sz);
				}
				if (this.ctrlTo == sender)
				{
					this.ptEnd = new Point(ctrlTo.GetTopLeftPointOfInputPort(this.iToPort).X + ptEndPort.X, ctrlTo.GetTopLeftPointOfInputPort(this.iToPort).Y + ptEndPort.Y);

					IConnector ic = (IConnector)ctrlTo;
					Size sz = new Size((int)(ctrlTo.Size.Width * 1/pne.Zoom), (int)(ctrlTo.Size.Height * 1/pne.Zoom));
					this.ptEndCenter = ic.GetEndCenterPoint(this.ptEnd, sz);
					this.ptEndFixed = ic.GetEndFixedPoint(this.ptEnd, sz);
					this.ldEndDirection = ic.GetEndDirection(this.ptEnd, this.ptEndCenter, sz);
				}

				this.ctrlFrom.Parent.Refresh();
			}
		}
		#endregion

		#region private void cc_Resized(object sender, ResizedEventArgs e)
		private void cc_Resized(object sender, ResizedEventArgs e)
		{
			if (this.ctrlFrom == sender)
			{
				this.ptBeginInitial = this.ptBegin;
			}
			if (this.ctrlTo == sender)
			{
				this.ptEndInitial = this.ptEnd;
			}

			this.ctrlFrom.Parent.Refresh();
		}
		#endregion

		#region public override string ToString()
		public override string ToString()
		{
			return this.sToString;
		}
		#endregion


                public string GetShortString()
                {
			// Set sToString field
			// this is optimization for ToString method

			string sCtrlFrom = ctrlFrom.ToString();
			int iIndex = sCtrlFrom.IndexOf(" ");
			sCtrlFrom = sCtrlFrom.Remove(iIndex, sCtrlFrom.Length - iIndex);

			string sCtrlTo = ctrlTo.ToString();
			iIndex = sCtrlTo.IndexOf(" ");
			sCtrlTo = sCtrlTo.Remove(iIndex, sCtrlTo.Length - iIndex);

                        string retval = sCtrlFrom + "_" + sCtrlTo;
                        return retval;
                }

		#region public object Clone(ConnectableControl ccFrom, ConnectableControl ccTo)
		public object Clone(ConnectableControl ccFrom, ConnectableControl ccTo)
		{
			Connection cn = new Connection(ccFrom, ccTo, this.iFromPort, this.iToPort, this.ptBeginPort, this.ptEndPort, this.iWeight, true);
			cn.sFrom = ccFrom.GetType().FullName + ccFrom.Index;
			cn.sTo = ccTo.GetType().FullName + ccTo.Index;

			if (ccFrom.Parent == null)
			{
				cn.ptBegin = this.ptBegin;
				cn.ptBeginCenter = this.ptBeginCenter;
				cn.ptBeginInitial = this.ptBeginInitial;
				cn.ptEnd = this.ptEnd;
				cn.ptEndCenter = this.ptEndCenter;
				cn.ptEndFixed = this.ptEndFixed;
				cn.ptEndInitial = this.ptEndInitial;
				cn.ldBeginDirection = this.ldBeginDirection;
				cn.ldEndDirection = this.ldEndDirection;
			}

			return cn;
		}
		#endregion

	}
}
