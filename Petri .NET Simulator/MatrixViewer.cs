using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace PetriNetSimulator2
{
	/// <summary>
	/// Summary description for MatrixViewer.
	/// </summary>
	public class MatrixViewer : System.Windows.Forms.UserControl
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		
		// Fields
		private ArrayList alColumns = null;
		private ArrayList alRows = null;
		private string sName = null;
		private Matrix mx = null;
		private int iColumnWidth = 45;
		private int iRowWidth = 20;
		private int iColumnOffset = 128;
		private int iRowOffset = 60;
        private ContextMenu cm = null;
        private MenuItem mi = null;


		private System.ComponentModel.Container components = null;

		public MatrixViewer(string sName, Matrix mx, ArrayList alColumns, ArrayList alRows)
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			this.SetStyle(ControlStyles.UserPaint |	ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer, true);
			this.UpdateStyles();

			this.sName = sName;
			this.alColumns = alColumns;
			this.alRows = alRows;
			this.mx = mx;

			this.Width = this.iColumnOffset + this.iColumnWidth * alColumns.Count;
			this.Height = this.iRowOffset + this.iRowWidth * alRows.Count;
		}

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
			// MatrixViewer
			// 
			this.BackColor = System.Drawing.SystemColors.Control;
			this.Name = "MatrixViewer";
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.MatrixViewer_Paint);

            cm = new ContextMenu();
            mi = new MenuItem("Copy matrix as text");
            cm.MenuItems.Add(mi);
            mi.Click += new EventHandler(mi_Click);
            this.ContextMenu = cm;
		}

        void mi_Click(object sender, EventArgs e)
        {
            String cpc = mx.ToString();
            cpc = cpc.Replace("]\n[", "]\r\n[");
            System.Windows.Forms.Clipboard.SetText(cpc);
        }
		#endregion


		#region private void MatrixViewer_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		private void MatrixViewer_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			Graphics g = e.Graphics;

			Font fnt = new Font(this.Font, FontStyle.Regular);
			Font fntBold = new Font(this.Font, FontStyle.Bold);

			StringFormat sf = new StringFormat();
			sf.LineAlignment = StringAlignment.Center;
			sf.Alignment = StringAlignment.Center;
			sf.FormatFlags = StringFormatFlags.NoClip;
			
			SizeF szf = g.MeasureString(this.sName + " =", fnt);

			g.DrawString(this.sName + " =", fnt, Brushes.Black, new RectangleF(0f, this.iRowOffset - 7, szf.Width, this.Height - this.iRowOffset), sf);

			if (mx.Dimensions.Height != 0 && mx.Dimensions.Width != 0)
			{
				g.DrawLines(Pens.Black, new Point[]{new Point((int)(szf.Width + 10), this.iRowOffset - 10), new Point((int)(szf.Width + 5), this.iRowOffset - 10), new Point((int)(szf.Width + 5), this.Height - 5), new Point((int)(szf.Width + 10), this.Height - 5)});
				g.DrawLines(Pens.Black, new Point[]{new Point((int)(this.Width - 90), this.iRowOffset - 10), new Point((int)(this.Width - 85), this.iRowOffset - 10), new Point((int)(this.Width - 85), this.Height - 5), new Point((int)(this.Width - 90), this.Height - 5)});

				for(int i = 0; i < mx.Dimensions.Height; i++)
				{
					if (alRows[i] is Transition)
					{
						g.DrawString("T" + ((Transition)alRows[i]).Index.ToString(), fnt, Brushes.Black, new RectangleF(new PointF(this.Width - 90, this.iRowOffset - 7 + i * this.iRowWidth), new SizeF(this.iColumnWidth, this.iRowWidth)), sf); 
					}
					else if (alRows[i] is Place)
					{
						g.DrawString(((Place)alRows[i]).NameID, fnt, Brushes.Black, new RectangleF(new PointF(this.Width - 80, this.iRowOffset - 7 + i * this.iRowWidth), new SizeF(this.iColumnWidth, this.iRowWidth)), sf);
						g.DrawString("P" + ((Place)alRows[i]).Index.ToString(), fnt, Brushes.Black, new RectangleF(new PointF(this.Width - 40, this.iRowOffset - 7 + i * this.iRowWidth), new SizeF(this.iColumnWidth, this.iRowWidth)), sf);
					}
					else
					{
						g.DrawString("m" + alRows[i].ToString(), fnt, Brushes.Black, new RectangleF(new PointF(this.Width - 90, this.iRowOffset - 7 + i * this.iRowWidth), new SizeF(this.iColumnWidth, this.iRowWidth)), sf); 
					}

					for(int j = 0; j < mx.Dimensions.Width; j++)
					{
						g.DrawString(mx[i,j].ToString(), fnt, Brushes.Black, new RectangleF(new PointF((int)(szf.Width + 10 + j * this.iColumnWidth), this.iRowOffset - 7 + i * this.iRowWidth), new SizeF(this.iColumnWidth, this.iRowWidth)), sf);
					}
				}

				for(int j = 0; j < mx.Dimensions.Width; j++)
				{
					if (alColumns[j] is Place)
					{
						g.DrawString("P" + ((Place)alColumns[j]).Index.ToString(), fnt, Brushes.Black, new RectangleF(new PointF((int)(szf.Width + 10 + j * this.iColumnWidth), 10), new SizeF(this.iColumnWidth, 20)), sf);
						g.DrawString(((Place)alColumns[j]).NameID, fnt, Brushes.Black, new RectangleF(new PointF((int)(szf.Width + 10 + j * this.iColumnWidth), 30), new SizeF(this.iColumnWidth, 20)), sf);
					}
					else if (alColumns[j] is Transition)
					{
						g.DrawString("T" + ((Transition)alColumns[j]).Index.ToString(), fnt, Brushes.Black, new RectangleF(new PointF((int)(szf.Width + 10 + j * this.iColumnWidth), 30), new SizeF(this.iColumnWidth, 20)), sf);
					}
				}
			}
		}
		#endregion


	}
}
