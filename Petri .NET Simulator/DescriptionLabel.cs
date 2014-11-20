using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Runtime.Serialization;

namespace PetriNetSimulator2
{
	[Serializable]
	public class DescriptionLabel : SelectableAndMovableControl, ISerializable, ICloneable, IComparable, IMetafileModel
	{
		#region public int Index
		public int Index
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

		#region public override string Text
		[CommonProperties]
		[Category("Description")]
		[EditorAttribute(typeof(TextPropertyEditor), typeof(UITypeEditor))]
		public override string Text
		{
			get
			{
				return this.sText;
			}
			set
			{
				this.sText = value;

				if (this.LabelTextChanged != null)
					this.LabelTextChanged(this, new EventArgs());

				this.Refresh();
			}
		}
		#endregion

		#region public override Font Font
		[CommonProperties]
		[Category("Appearence")]
		public override Font Font
		{
			get
			{
				return this.fntTextFont;
			}
			set
			{
				this.fntTextFont = value;
				this.Refresh();
			}
		}
		#endregion

		#region public override Color BackColor
		[CommonProperties]
		[Category("Appearence")]
		[DefaultValue("")]
		public override Color BackColor
		{
			get
			{
				return this.cInactiveColor;
			}
			set
			{
				this.cInactiveColor = value;
				this.Refresh();
			}
		}
		#endregion

		#region public Color TextColor
		[CommonProperties]
		[Category("Appearence")]
		[DefaultValue("")]
		public Color TextColor
		{
			get
			{
				return this.cTextColor;
			}
			set
			{
				this.cTextColor = value;
				this.Refresh();
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

		// Events
		public event EventHandler PropertiesChanged;
		public event EventHandler LabelTextChanged;

		protected Color cTextColor = SystemColors.InfoText;
		private Color cBorderColor = Color.Black;

		private string sText = "";
		private int iIndex;
		private string sIndex;

		private Font fntTextFont;

		private System.ComponentModel.IContainer components = null;

		#region public DescriptionLabel() : base(new Size(200, 36))
		public DescriptionLabel() : base(new Size(200, 36))
		{
			// This call is required by the Windows Form Designer.
			InitializeComponent();

			this.SetStyle(ControlStyles.UserPaint |	ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer, true);
			this.UpdateStyles();

			this.AllowResizing = true;

			this.cInactiveColor = SystemColors.Info;
			this.fntTextFont = base.Font;

			this.Paint += new PaintEventHandler(DescriptionLabel_Paint);
			this.MouseUp += new MouseEventHandler(DescriptionLabel_MouseUp);
		}
		#endregion

		#region public DescriptionLabel(SerializationInfo info, StreamingContext context) : base(info, context)
		public DescriptionLabel(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			this.SetStyle(ControlStyles.UserPaint |	ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer, true);
			this.UpdateStyles();

			this.AllowResizing = true;

			this.iIndex = info.GetInt32("index");
			this.Location = (Point)info.GetValue("location", typeof(Point));
			this.sText = info.GetString("text");
			this.BackColor = (Color)info.GetValue("backcolor", typeof(Color));
			this.TextColor = (Color)info.GetValue("textcolor", typeof(Color));
			this.Font = (Font)info.GetValue("font", typeof(Font));

			this.Paint += new PaintEventHandler(DescriptionLabel_Paint);
			this.MouseUp += new MouseEventHandler(DescriptionLabel_MouseUp);
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
			info.AddValue("location", this.Location);
			info.AddValue("text", this.sText);
			info.AddValue("backcolor", this.BackColor);
			info.AddValue("textcolor", this.TextColor);
			info.AddValue("font", this.Font);
		}
		#endregion


		#region private void DescriptionLabel_Paint(object sender, PaintEventArgs e)
		private void DescriptionLabel_Paint(object sender, PaintEventArgs e)
		{
			// Draws DescriptionLabel object
			Graphics g = e.Graphics;
			
			if (PetriNetDocument.AntiAlias == true)
				g.SmoothingMode = SmoothingMode.AntiAlias;

			PetriNetEditor pne = (PetriNetEditor)this.Parent;

			Rectangle rect = this.ClientRectangle;

			LinearGradientBrush lgb = null;

			if (this.cBackgroundColor == this.cActiveColor)
				lgb = new LinearGradientBrush(rect, Color.White, this.cBackgroundColor, LinearGradientMode.ForwardDiagonal);
			else
				lgb = new LinearGradientBrush(rect, this.cBackgroundColor, this.cBackgroundColor, LinearGradientMode.ForwardDiagonal);

            g.FillRectangle(lgb, rect);

			Pen pBlack = new Pen(this.cBorderColor, pne.Zoom * 2);
			g.DrawRectangle(pBlack, 0, 0, this.Width - 1, this.Height - 1);

			Brush bBlack = new SolidBrush(this.cTextColor);
			StringFormat sf = new StringFormat();
			sf.Alignment = StringAlignment.Center;
			sf.LineAlignment = StringAlignment.Center;
			
			Font f = new Font(this.fntTextFont.FontFamily, pne.Zoom * this.fntTextFont.Size, this.fntTextFont.Style);

			g.DrawString(this.Text, f, bBlack, new Rectangle(new Point(0, 0), new Size(this.Width - (int)(pne.Zoom * 2), this.Height)), sf);
		}
		#endregion

		#region private void DescriptionLabel_MouseUp(object sender, MouseEventArgs e)
		private void DescriptionLabel_MouseUp(object sender, MouseEventArgs e)
		{
			// Refresh Location property
			if (this.PropertiesChanged != null)
				this.PropertiesChanged(this, EventArgs.Empty);
		}
		#endregion


		#region public object Clone()
		public object Clone()
		{
			DescriptionLabel dl = new DescriptionLabel();
			dl.Location = this.Location;
			dl.Text = this.Text;
			dl.TextColor = this.TextColor;
			dl.Size = this.Size;

			// Change default size
			if (this.Parent != null && this.Parent is EditorSurface)
			{
				EditorSurface es = (EditorSurface)this.Parent;
				dl.DefaultSize = new Size((int)(this.Width / es.Zoom), (int)(this.Height / es.Zoom));
			}
			else if (this.Parent == null)
			{
				dl.DefaultSize = this.DefaultSize;
			}

			dl.BackColor = this.BackColor;
			dl.Font = this.Font;
						
			return dl;
		}
		#endregion

		#region public override string ToString()
		public override string ToString()
		{
			if (this.Text != "")
			{
				string sClippedName = this.Text.Substring(0, Math.Min(15, this.Text.Length));
				if (sClippedName.Length < this.Text.Length)
					sClippedName = sClippedName.TrimEnd(new char[]{' '}) + "...";
				return "L" + this.sIndex + " - " + sClippedName + " (Label)";
			}
			else
				return "L" + this.sIndex + " (Label)";
		}
		#endregion

		#region public int CompareTo(object o)
		public int CompareTo(object o)
		{
			if (o is DescriptionLabel)
			{
				DescriptionLabel dl = (DescriptionLabel)o;
				return this.iIndex.CompareTo(dl.Index);
			}

			throw new ArgumentException("Invalid argument");
		}
		#endregion


		#region IMetafileModel Members

		public void DrawModel(Graphics g)
		{
			Point pt = this.Location;

			PetriNetEditor pne = (PetriNetEditor)this.Parent;
			Rectangle r = new Rectangle(pt, this.Size);

//			g.FillRectangle(new SolidBrush(SystemColors.Info), r);
//
//			Pen pBlack = new Pen(this.cBorderColor, pne.Zoom * 2);
//			g.DrawRectangle(pBlack, r);

			Brush bBlack = new SolidBrush(this.cTextColor);
			StringFormat sf = new StringFormat();
			sf.Alignment = StringAlignment.Center;
			sf.LineAlignment = StringAlignment.Center;
			
			Font f = new Font(this.fntTextFont.FontFamily, pne.Zoom * this.fntTextFont.Size, this.fntTextFont.Style);

			g.DrawString(this.Text, f, bBlack, new Rectangle(new Point(pt.X, pt.Y), new Size(this.Width - (int)(pne.Zoom * 2), this.Height)), sf);
		}

		#endregion
	}
}

