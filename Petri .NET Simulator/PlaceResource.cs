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
	/// Summary description for PlaceResource.
	/// </summary>
	[Serializable]
	public class PlaceResource : Place, ICloneable, ISerializable, IMetafileModel
	{
		#region public ReleaseTime[] ReleaseTimes
		[CommonProperties]
		[Category("Release time")]
		[Description("Used in PTyped simulations. Used to set release time of resource place.")]
		[EditorAttribute(typeof(ReleaseTimesEditor), typeof(UITypeEditor))]
		public ReleaseTime[] ReleaseTimes
		{
			get
			{
				ArrayList alResourceOperationsPlaces = this.ResourceOperationsPlaces;

				if (alResourceOperationsPlaces != null)
				{
					// Adjust ReleaseTimes array
					ReleaseTime[] rta = new ReleaseTime[alResourceOperationsPlaces.Count];

					int k = 0;

					foreach (Place p in alResourceOperationsPlaces)
					{
						// Check if this place is in ReleaseTime[]
						bool bFound = false;
						int iFoundIndex = -1;
						for(int i = 0; i < this.rtReleaseTimes.Length; i++)
						{
							if (this.rtReleaseTimes[i].OperationPlace == p)
							{
								bFound = true;
								iFoundIndex = i;
								break;
							}
						}

						if (bFound == true)
						{
							rta[k] = this.rtReleaseTimes[iFoundIndex];
						}
						else
						{
							rta[k] = new ReleaseTime(p, 1);
						}

						k++;
					}

					return rta;
				}

				return null;
			}
			set
			{
				this.rtReleaseTimes = value;
			}
		}
		#endregion

		#region public ArrayList ResourceOperationsPlaces
		public ArrayList ResourceOperationsPlaces
		{
			get
			{
				ArrayList al = new ArrayList();

				PetriNetDocument pnd = ((PetriNetEditor)this.Parent).Document;
				IntMatrix imSrFv = pnd.Sr * pnd.Fv;

				ArrayList alOperationPlaces = pnd.OperationPlaces;
				ArrayList alResourcePlaces = pnd.ResourcePlaces;

				for(int i = 0; i < alOperationPlaces.Count; i++)
				{
					if(imSrFv[alResourcePlaces.IndexOf(this), i] == 1)
						al.Add(alOperationPlaces[i]);
				}

				return al;
			}
		}
		#endregion

		#region public int FillAngle
		public int FillAngle
		{
			get
			{
				return this.iFillAngle;
			}
			set
			{
				this.iFillAngle = value;
				this.RefreshMT();
			}
		}
		#endregion

		#region public int MaxFillAngle
		public int MaxFillAngle
		{
			get
			{
				return this.iMaxFillAngle;
			}
			set
			{
				this.iMaxFillAngle = value;
			}
		}
		#endregion

		// Fields
		private ReleaseTime[] rtReleaseTimes = new ReleaseTime[0];
		private int iFillAngle = 0;
		private int iMaxFillAngle = 5;


		public PlaceResource() : base()
		{
			InitializeComponent();
		}

		#region public PlaceResource(SerializationInfo info, StreamingContext context) : base(info, context)
		public PlaceResource(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			this.rtReleaseTimes = (ReleaseTime[])info.GetValue("releasetimes", typeof(ReleaseTime[]));
		}
		#endregion

		#region public void RestoreReleaseTimes()
		public void RestoreReleaseTimes()
		{
			Hashtable ht = new Hashtable();
			foreach(Place p in this.ResourceOperationsPlaces)
			{
				ht.Add("P" + p.Index, p);
			}

			foreach(ReleaseTime rt in this.rtReleaseTimes)
			{
				rt.RestoreReference(ht);
			}
		}
		#endregion


		#region Component Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			//
			// PlaceResource
			//
			this.Name = "PlaceResource";
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.PlaceResource_Paint);
		}
		#endregion


		#region public new void GetObjectData(SerializationInfo info, StreamingContext context)
		public new void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);

			info.AddValue("releasetimes", this.ReleaseTimes);
		}
		#endregion


		#region private void PlaceResource_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		private void PlaceResource_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			// Draws Place object
			Graphics g = e.Graphics;

			if (PetriNetDocument.AntiAlias == true)
				g.SmoothingMode = SmoothingMode.AntiAlias;

			PetriNetEditor pne = (PetriNetEditor)this.Parent;

			Rectangle r = this.ClientRectangle;
			LinearGradientBrush lgb = new LinearGradientBrush(r, Color.White, this.cBackgroundColor, LinearGradientMode.ForwardDiagonal);
			Brush bFill = new LinearGradientBrush(r, Color.White, Color.Red, LinearGradientMode.ForwardDiagonal);

			// Draw on background a letter which describes PlaceType
			StringFormat sfo = new StringFormat();
			sfo.LineAlignment = StringAlignment.Center;
			sfo.Alignment = StringAlignment.Center;

			Font fo = new Font(this.Parent.Font.FontFamily, pne.Zoom * 10f,  FontStyle.Bold);

			g.DrawString("R", fo, Brushes.Black, new Rectangle(new Point((int)(42 * pne.Zoom), 0), new Size((int)(this.Width - 42 * pne.Zoom), this.Height)), sfo);

			g.FillRectangle(lgb, r);

			if (bShowCircularWaitings == true && pne.Document.CircularWaitingPlaces.Contains(this))
			{
				Pen pRoyalBlue = new Pen(Color.RoyalBlue, pne.Zoom * 7);
				g.DrawEllipse(pRoyalBlue, new Rectangle(r.X + (int)(4 * pne.Zoom), r.Y + (int)(4 * pne.Zoom), r.Width - (int)(8 * pne.Zoom), r.Height - (int)(8 * pne.Zoom)));
			}

			// For showing processing time of each place
			g.FillPie(bFill, r, -90f, 360f*((float)this.iFillAngle / (float)this.iMaxFillAngle));

			Pen pBlack = new Pen(Color.Black, pne.Zoom * 7);
			g.DrawEllipse(pBlack, r);

			Brush bBlack = new SolidBrush(Color.Black);
			StringFormat sf = new StringFormat();
			sf.Alignment = StringAlignment.Center;
			Font f = new Font(this.Parent.Font.FontFamily, pne.Zoom * 7f,  FontStyle.Bold);

			g.DrawString("P" + sIndex, f, bBlack, new RectangleF(new PointF(0f, this.Height - pne.Zoom * 23f), new SizeF(this.Width, pne.Zoom * 20f)), sf);

			sf.LineAlignment = StringAlignment.Center;
			g.DrawString(this.sName, f, bBlack, new RectangleF(new PointF(0f, pne.Zoom * 6f), new SizeF(this.Width, pne.Zoom * 20f)), sf);

			sf.LineAlignment = StringAlignment.Center;
			RectangleF rTokens = new RectangleF(new PointF(0f, 0f), new SizeF(this.Width, this.Height));
			this.DrawTokens(g, bBlack, rTokens, sf);
		}
		#endregion

		#region public override object Clone()
		public override object Clone()
		{
			PlaceResource pr = new PlaceResource();
			pr.Location = this.Location;
			pr.NameID = this.NameID;
			pr.Tokens = this.Tokens;
			return pr;
		}
		#endregion

        #region public void RefreshMT
        public delegate void InvokeDelegateRefresh();
        public void RefreshMT()
        {
            BeginInvoke(new InvokeDelegateRefresh(Refresh), null);
        }
        #endregion


		#region public override string ToString()
		public override string ToString()
		{
			if (this.NameID != null && this.NameID != "")
				return "P" + this.sIndex + " - " + this.NameID + " (Resource)";
			else
				return "P" + this.sIndex + " (Resource)";
		}
		#endregion

        public override string GetShortString()
        {
                if (this.NameID != null && this.NameID != "")
                        return this.NameID;
                else
                        return "P" + this.sIndex;
        }

        #region public string GetXMLString()
        public override string GetXMLString()
        {
            Point pt = this.Location;
            string s = "\t<place id=\"" + this.GetShortString() + "\">\n";

            s += "\t\t<name>\n";
            s += "\t\t\t<graphics><position x=\""+pt.X+"\" y=\""+pt.Y+"\" /></graphics>\n";
            s += "\t\t\t<text>" + this.GetShortString () + "</text>\n";
            s += "\t\t</name>\n";

            if(this.Tokens != 0)
                s += "\t\t<initialMarking><text>"+ this.Tokens  +"</text></initialMarking>\n";

            s += "<toolspecific tool=\"PNE\"><type><text>R</text></type></toolspecific>\n";

            s += "\t</place>\n";
            return s;
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

		#region IMetafileModel Members

		public void DrawModel(Graphics g)
		{
			Point pt = this.Location;

			PetriNetEditor pne = (PetriNetEditor)this.Parent;
			Rectangle r = new Rectangle(pt, this.Size);
			Brush bFill = new LinearGradientBrush(r, Color.White, Color.Red, LinearGradientMode.ForwardDiagonal);

			g.FillEllipse(Brushes.White, r);

			// Draw on background a letter which describes PlaceType
			StringFormat sfo = new StringFormat();
			sfo.LineAlignment = StringAlignment.Center;
			sfo.Alignment = StringAlignment.Center;

			Font fo = new Font(this.Parent.Font.FontFamily, pne.Zoom * 10f,  FontStyle.Bold);

			g.DrawString("R", fo, Brushes.LightGray, new Rectangle(new Point(pt.X + (int)(42 * pne.Zoom), pt.Y), new Size((int)(this.Width - 42 * pne.Zoom), this.Height)), sfo);

			if (bShowCircularWaitings == true && pne.Document.CircularWaitingPlaces.Contains(this))
			{
				Pen pRoyalBlue = new Pen(Color.RoyalBlue, pne.Zoom * 4);
				g.DrawEllipse(pRoyalBlue, new Rectangle(pt.X + (int)(4 * pne.Zoom), pt.Y + (int)(4 * pne.Zoom), r.Width - (int)(8 * pne.Zoom), r.Height - (int)(8 * pne.Zoom)));
			}

			// For showing processing time of each place
			g.FillPie(bFill, r, -90f, 360f*((float)this.iFillAngle / (float)this.iMaxFillAngle));

			Pen pBlack = new Pen(Color.Black, pne.Zoom * 4);
			g.DrawEllipse(pBlack, r);

			Brush bBlack = new SolidBrush(Color.Black);
			StringFormat sf = new StringFormat();
			sf.Alignment = StringAlignment.Center;
			Font f = new Font(this.Parent.Font.FontFamily, pne.Zoom * 7f,  FontStyle.Bold);

			g.DrawString("P" + iIndex.ToString(), f, bBlack, new RectangleF(new PointF(pt.X, pt.Y + this.Height - pne.Zoom * 23f), new SizeF(this.Width, pne.Zoom * 20f)), sf);

			sf.LineAlignment = StringAlignment.Center;
			g.DrawString(this.sName, f, bBlack, new RectangleF(new PointF(pt.X, pt.Y + pne.Zoom * 6f), new SizeF(this.Width, pne.Zoom * 20f)), sf);

			sf.LineAlignment = StringAlignment.Center;
			RectangleF rTokens = new RectangleF(new PointF(pt.X, pt.Y), new SizeF(this.Width, this.Height));
			this.DrawTokens(g, bBlack, rTokens, sf);
		}

		#endregion
	}
}
