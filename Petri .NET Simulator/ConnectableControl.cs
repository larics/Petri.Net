using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Runtime.Serialization;

namespace PetriNetSimulator2
{
	public class ConnectableControl : SelectableAndMovableControl, ISerializable
	{
		#region public Region ConnectableInputRegion
		public Region ConnectableInputRegion
		{
			get
			{
				//return this.rConnectableInputRegion;
				Region r = new Region();
				r.MakeEmpty();
				foreach(Port p in this.alInputPorts)
				{
					r.Union(p.PortRegion);
				}
				return r;
			}
		}
		#endregion

		#region public Region ConnectableOutputRegion
		public Region ConnectableOutputRegion
		{
			get
			{
				Region r = new Region();
				r.MakeEmpty();
				foreach(Port p in this.alOutputPorts)
				{
					r.Union(p.PortRegion);
				}
				return r;
			}
		}
		#endregion

		#region public ArrayList Childs
		[CommonProperties]
		[TimeInvariant]
		public ArrayList Childs
		{
			get
			{
				return this.alChilds;
			}
		}
		#endregion

		#region public ArrayList Parents
		[CommonProperties]
		[TimeInvariant]
		public ArrayList Parents
		{
			get
			{
				return this.alParents;
			}
		}
		#endregion

		#region public virtual int Index
		public virtual int Index
		{
			get
			{
				return this.iIndex;
			}
			set
			{
				this.iIndex = value;
				this.sIndex = value.ToString();
			}
		}
		#endregion

        public virtual string GetShortString()
        {
            return "Place?";
        }


		internal event EventHandler PropertiesChanged;
		internal event EventHandler ConnectionCreated;

		protected int iIndex;
		protected string sIndex; // Optimization for ToString method

		protected ArrayList alChilds = new ArrayList();
		protected ArrayList alParents = new ArrayList();

		protected ArrayList alInputPorts = new ArrayList();
		protected ArrayList alOutputPorts = new ArrayList();

		private System.ComponentModel.IContainer components = null;

		#region public ConnectableControl(Size szDefaultSize) : base(szDefaultSize)
		public ConnectableControl(Size szDefaultSize) : base(szDefaultSize)
		{
			// This call is required by the Windows Form Designer.
			InitializeComponent();

			this.MouseDown += new MouseEventHandler(ConnectableControl_MouseDown);
			this.MouseMove += new MouseEventHandler(ConnectableControl_MouseMove);
			this.MouseUp += new MouseEventHandler(ConnectableControl_MouseUp);
		}
		#endregion

		#region protected ConnectableControl(SerializationInfo info, StreamingContext context) : base(info, context)
		protected ConnectableControl(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			this.MouseDown += new MouseEventHandler(ConnectableControl_MouseDown);
			this.MouseMove += new MouseEventHandler(ConnectableControl_MouseMove);
			this.MouseUp += new MouseEventHandler(ConnectableControl_MouseUp);

			this.iIndex = info.GetInt32("index");
			this.sIndex = this.iIndex.ToString();
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

			info.AddValue("index", this.iIndex);
		}
		#endregion


		#region private void ConnectableControl_MouseDown(object sender, MouseEventArgs e)
		private void ConnectableControl_MouseDown(object sender, MouseEventArgs e)
		{
			EditorSurface es = (EditorSurface)this.Parent;

			// Sets Default cursor while drawing transition line
			if (e.Button == MouseButtons.Right)
			{
				this.Cursor = Cursors.Default;

				if (this.CanConnectFromThisPoint(new Point(e.X, e.Y)))
					es.bConnecting = true;
			}
		}
		#endregion

		#region private void ConnectableControl_MouseMove(object sender, MouseEventArgs e)
		private void ConnectableControl_MouseMove(object sender, MouseEventArgs e)
		{
			if (this.Capture == true && e.Button == MouseButtons.Right)
			{
				PetriNetEditor pne = (PetriNetEditor)this.Parent;
				Panel pnl = (Panel)pne.Parent;

				Point ptBegin = this.PointToScreen(ptMouseDragOffset);
				ptBegin = pne.PointToClient(ptBegin);
				pne.ptDragBegin = ptBegin;

				Point ptEnd = this.PointToScreen(new Point(e.X, e.Y));
				ptEnd = pne.PointToClient(ptEnd);
				pne.ptDragEnd = ptEnd;

				Control cBegin = this.GetChildAtPoint(ptBegin);
				Control cEnd = this.GetChildAtPoint(ptEnd);

				Point ptNewScrollPosition = new Point(0,0);
				if (ptEnd.X < -pnl.AutoScrollPosition.X || ptEnd.Y < -pnl.AutoScrollPosition.Y)
				{
					ptNewScrollPosition = new Point(Math.Min(ptEnd.X, -pnl.AutoScrollPosition.X), Math.Min(ptEnd.Y, -pnl.AutoScrollPosition.Y));
					pnl.AutoScrollPosition = ptNewScrollPosition;
				}
				else if (ptEnd.X > (pnl.Width-25-pnl.AutoScrollPosition.X) || ptEnd.Y > (pnl.Height+15-pnl.AutoScrollPosition.Y))
				{
					ptNewScrollPosition = new Point(Math.Max(ptEnd.X-pnl.Width+25, -pnl.AutoScrollPosition.X), Math.Min(ptEnd.Y-pnl.Height+15, -pnl.AutoScrollPosition.Y));
					pnl.AutoScrollPosition = ptNewScrollPosition;
				}
				else if (ptEnd.X > (pnl.Width-25-pnl.AutoScrollPosition.X) || ptEnd.Y > (pnl.Height-15-pnl.AutoScrollPosition.Y))
				{
					ptNewScrollPosition = new Point(Math.Max(ptEnd.X-pnl.Width+25, -pnl.AutoScrollPosition.X), Math.Max(ptEnd.Y-pnl.Height+15, -pnl.AutoScrollPosition.Y));
					pnl.AutoScrollPosition = ptNewScrollPosition;
				}

				pne.Refresh();
			}		
		}
		#endregion

		#region private void ConnectableControl_MouseUp(object sender, MouseEventArgs e)
		private void ConnectableControl_MouseUp(object sender, MouseEventArgs e)
		{
			this.Cursor = Cursors.SizeAll;

			PetriNetEditor pne = (PetriNetEditor)this.Parent;
			pne.Invalidate();

			if (pne.bConnecting == true)
			{
				pne.bConnecting = false;

				Point ptEnd = this.PointToScreen(new Point(e.X, e.Y));
				ptEnd = pne.PointToClient(ptEnd);

				Control cEnd = pne.GetChildAtPoint(ptEnd);

				if (cEnd is ConnectableControl && cEnd != this)
				{
					ConnectableControl cc = (ConnectableControl)cEnd;

					if (this.CanConnectTo(cc))
					{
						if (this.alChilds.Contains(cc) != true)
						{
							// Set ptEnd to client coordinates of cc
							ptEnd = pne.PointToScreen(ptEnd);
							ptEnd = cc.PointToClient(ptEnd);

							if (cc.CanConnectToThisPoint(ptEnd))
							{
								if (this is IConnector && cc is IConnector)
								{
									this.alChilds.Add(cc);
									cc.alParents.Add(this);

									#region Determine Output port
									// Determine Output port
									Port pOutput = null;
									foreach(Port p in this.alOutputPorts)
									{
										GraphicsPath gp = new GraphicsPath();
										gp.AddEllipse(ptMouseDragOffset.X-1, ptMouseDragOffset.Y-1, 2, 2);

										Region r = new Region(gp);
										r.Intersect(p.PortRegion);

										RectangleF rf = r.GetBounds(this.CreateGraphics());

										if (rf.Height != 0 || rf.Width != 0)
										{
											pOutput = p;
											break;
										}
									}
									#endregion

									#region Determine Input port
									// Determine Input port
									Port pInput = null;
									foreach(Port p in cc.alInputPorts)
									{
										GraphicsPath gp = new GraphicsPath();
										gp.AddEllipse(ptEnd.X-1, ptEnd.Y-1, 2, 2);

										Region r = new Region(gp);
										r.Intersect(p.PortRegion);

										RectangleF rf = r.GetBounds(this.CreateGraphics());

										if (rf.Height != 0 || rf.Width != 0)
										{
											pInput = p;
											break;
										}
									}
									#endregion

									// Scale to Port Bounds
									RectangleF rfBegin = pOutput.PortRegion.GetBounds(this.CreateGraphics());
									Point ptScaledBegin = new Point(ptMouseDragOffset.X - (int)rfBegin.X, ptMouseDragOffset.Y - (int)rfBegin.Y);

									RectangleF rfEnd = pInput.PortRegion.GetBounds(this.CreateGraphics());
									Point ptScaledEnd = new Point(ptEnd.X - (int)rfEnd.X, ptEnd.Y - (int)rfEnd.Y);

									// Create new Connection Instance and add it
									Connection cn = pne.CreateNewConnection(this, cc, pOutput.PortNumber, pInput.PortNumber, ptScaledBegin, ptScaledEnd, 1);

									pne.Connections.Add(cn);
				
									// To notify parent PetriNetEditor that new connection has been created
									if (this.ConnectionCreated != null)
										this.ConnectionCreated(cn, EventArgs.Empty);
				
									// Raises PropertiesChanged event to notify Parent
									// that Childs property is changed
									if (this.PropertiesChanged != null)
										this.PropertiesChanged(this, new EventArgs());
								}
								else
								{
									throw new InterfaceNotImplementedException("Not all conecting controls implement IConnector interface");
								}
							}
						}
						else
						{
							if (DialogResult.Yes == MessageBox.Show("Are you sure you want to delete this connection?", "Petri .NET Simulator 1.0 - Question", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question))
							{
								// Remove this ConnectableControl from Parents collection of child cc
								int iParentIndex = cc.Parents.IndexOf(this);
								cc.Parents.Remove(this);
		
								// Remove cc from this places Childs collection
								this.alChilds.Remove(cc);
								
								// Remove connection between these two controls
								Connection cn = Connection.GetConnectionBetweenControls(this, cc);
								pne.Connections.Remove(cn);
		
								pne.Refresh();
		
								if (this.PropertiesChanged != null)
									this.PropertiesChanged(this, new EventArgs());
							}
						}
					}
				}
			}
		}
		#endregion

		#region protected void OnPropertiesChanged(object sender, EventArgs e)
		protected void OnPropertiesChanged(object sender, EventArgs e)
		{
			if (this.PropertiesChanged != null)
				this.PropertiesChanged(this, e);
		}
		#endregion


		#region public Point GetTopLeftPointOfInputPort(int iPortNumber)
		public Point GetTopLeftPointOfInputPort(int iPortNumber)
		{
			if (this.Parent is PetriNetEditor)
			{
				PetriNetEditor pne = (PetriNetEditor)this.Parent;

				Port pFound = null;
				foreach(Port p in this.alInputPorts)
				{
					if (p.PortNumber == iPortNumber)
					{
						pFound = p;
						break;
					}
				}

				// Scale to Port Bounds
				if (pFound != null)
				{
					RectangleF rf = pFound.PortRegion.GetBounds(this.CreateGraphics());
					return new Point((int)(rf.X * 1/pne.Zoom), (int)(rf.Y * 1/pne.Zoom));
				}
			}
			return Point.Empty;
		}
		#endregion

		#region public Point GetTopLeftPointOfOutputPort(int iPortNumber)
		public Point GetTopLeftPointOfOutputPort(int iPortNumber)
		{
			if (this.Parent is PetriNetEditor)
			{
				PetriNetEditor pne = (PetriNetEditor)this.Parent;

				Port pFound = null;
				foreach(Port p in this.alOutputPorts)
				{
					if (p.PortNumber == iPortNumber)
					{
						pFound = p;
						break;
					}
				}

				// Scale to Port Bounds
				if (pFound != null)
				{
					RectangleF rf = pFound.PortRegion.GetBounds(this.CreateGraphics());
					return new Point((int)(rf.X * 1/pne.Zoom), (int)(rf.Y * 1/pne.Zoom));
				}
			}
			return Point.Empty;
		}
		#endregion


		#region public bool CanConnectFromThisPoint(Point pt)
		public bool CanConnectFromThisPoint(Point pt)
		{
			GraphicsPath gp = new GraphicsPath();
			gp.AddEllipse(pt.X-1, pt.Y-1, 2, 2);

			Region r = new Region(gp);
			r.Intersect(this.ConnectableOutputRegion);

			RectangleF rf = r.GetBounds(this.CreateGraphics());

			if (rf.Height != 0 || rf.Width != 0)
				return true;
			else
				return false;
		}
		#endregion

		#region public bool CanConnectToThisPoint(Point pt)
		public bool CanConnectToThisPoint(Point pt)
		{
			GraphicsPath gp = new GraphicsPath();
			gp.AddEllipse(pt.X-1, pt.Y-1, 2, 2);

			Region r = new Region(gp);
			r.Intersect(this.ConnectableInputRegion);

			RectangleF rf = r.GetBounds(this.CreateGraphics());

			if (rf.Height != 0 || rf.Width != 0)
				return true;
			else
				return false;
		}
		#endregion

		#region public virtual bool CanConnectTo(object o)
		public virtual bool CanConnectTo(object o)
		{
			return false;
		}
		#endregion


		#region protected bool InputPortNumberExists(int iNumber)
		protected bool InputPortNumberExists(int iNumber)
		{
			bool bFound = false;
			foreach(Port p in alInputPorts)
			{
				if (p.PortNumber == iNumber)
				{
					bFound = true;
					break;
				}
			}

			return bFound;
		}
		#endregion

		#region protected bool OutputPortNumberExists(int iNumber)
		protected bool OutputPortNumberExists(int iNumber)
		{
			bool bFound = false;
			foreach(Port p in alOutputPorts)
			{
				if (p.PortNumber == iNumber)
				{
					bFound = true;
					break;
				}
			}

			return bFound;
		}
		#endregion

	}
}

