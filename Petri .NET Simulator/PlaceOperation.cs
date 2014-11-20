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
	/// Summary description for PlaceOperation.
	/// </summary>
	[Serializable]
	public class PlaceOperation : Place, ICloneable, ISerializable, IMetafileModel
	{
		#region public int Duration
		[CommonProperties]
		[Category("Processing duration")]
		[Description("Duration for PTyped simulations. Used to set processing time of place.")]
		public int Duration
		{
			get
			{
				return this.iDuration;
			}
			set
			{
				this.iDuration = value;
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
		private int iDuration = 1;
		private int iFillAngle = 0;
		private int iMaxFillAngle = 5;


		public PlaceOperation() : base()
		{
			InitializeComponent();
		}

		// Constructor for Deserialization
		#region public PlaceOperation(SerializationInfo info, StreamingContext context) : base(info, context)
		public PlaceOperation(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			this.iDuration = info.GetInt32("duration");
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
			// PlaceOperation
			//
			this.Name = "PlaceOperation";
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.PlaceOperation_Paint);
		}
		#endregion

		#region public new void GetObjectData(SerializationInfo info, StreamingContext context)
		public new void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);

			info.AddValue("duration", this.iDuration);
		}
		#endregion


		#region private void PlaceOperation_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		private void PlaceOperation_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
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

			g.DrawString("O", fo, Brushes.Black, new Rectangle(new Point((int)(42 * pne.Zoom), 0), new Size((int)(this.Width - 42 * pne.Zoom), this.Height)), sfo);

			g.FillRectangle(lgb, r);

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

        #region public void RefreshMT
        public delegate void InvokeDelegateRefresh();
        public void RefreshMT()
        {
            BeginInvoke(new InvokeDelegateRefresh(Refresh), null);
        }
        #endregion


		#region public override object Clone()
		public override object Clone()
		{
			PlaceOperation po = new PlaceOperation();
			po.Location = this.Location;
			po.NameID = this.NameID;
			po.Duration = this.Duration;
			po.Tokens = this.Tokens;
			return po;
		}
		#endregion

		#region public override string ToString()
		public override string ToString()
		{
			if (this.NameID != null && this.NameID != "")
				return "P" + this.sIndex + " - " + this.NameID + " (Operation)";
			else
				return "P" + this.sIndex + " (Operation)";
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
            string s = "\t<place id=\"" + this.GetShortString ()+ "\">\n";

            s += "\t\t<name>\n";
            s += "\t\t\t<graphics><position x=\""+pt.X+"\" y=\""+pt.Y+"\" /></graphics>\n";
            s += "\t\t\t<text>" + this.GetShortString () + "</text>\n";
            s += "\t\t</name>\n";

            if(this.Tokens != 0)
                s += "\t\t<initialMarking><text>"+ this.Tokens  +"</text></initialMarking>\n";

            s += "<toolspecific tool=\"PNE\"><type><text>J</text></type></toolspecific>\n";
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

			g.DrawString("O", fo, Brushes.LightGray, new Rectangle(new Point(pt.X + (int)(42 * pne.Zoom), pt.Y), new Size((int)(this.Width - 42 * pne.Zoom), this.Height)), sfo);

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
