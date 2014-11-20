using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.IO;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace PetriNetSimulator2
{
	/// <summary>
	/// Summary description for Oscilogram.
	/// </summary>
	public class Oscillogram : System.Windows.Forms.UserControl
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		// Properties

		#region public Size Range
		public Size Range
		{
			get
			{
				return this.szRange;
			}
			set
			{
				this.szRange = value;
				this.Refresh();
			}
		}
		#endregion

		#region public int NumbersPerSection
		public int NumbersPerSection
		{
			get
			{
				return this.iNumbersPerSection;
			}
			set
			{
				this.iNumbersPerSection = value;
				this.Refresh();
			}
		}
		#endregion

		#region public bool AllowSelection
		public bool AllowSelection
		{
			get
			{
				return this.bAllowSelection;
			}
			set
			{
				this.bAllowSelection = value;
				this.Refresh();
			}
		}
		#endregion

		#region public int Marker
		public int Marker
		{
			get
			{
				return this.iMarker;
			}
			set
			{
				this.iMarker = value;
				this.Refresh();
			}
		}
		#endregion

		#region public bool AllowMarker
		public bool AllowMarker
		{
			get
			{
				return this.bAllowMarker;
			}
			set
			{
				this.bAllowMarker = value;
				this.Refresh();
			}
		}
		#endregion

		// Fields
		private PetriNetDocument pnd;
		private IntMatrix im;

		private int iTopMargin = 80;
		private int iBottomMargin = 20;
		private int iLeftMargin = 80;
		private int iRightMargin = 40;
		private int iGraphHeight = 100;
		private int iGraphDistance = 30;
		private int iSectionWidth = 40;
		private int iNumbersPerSection = 1;
		private int iSectionLine = 5;
		private Color cGraphBackground = Color.White;
		private Color cGraphBorder = Color.Black;
		private Color cText = Color.Black;
		private System.Windows.Forms.ContextMenu cmMenu;
		private Color cLine = Color.DarkBlue;

		private ArrayList alGroupedPlaces;
		private Place[] aResponsePlaces;
		private Size szRange;
		private Size szSelection;
		private bool bAllowSelection = true;
		private int iMarker = 0;
		private bool bAllowMarker = false;

		private bool bSelecting = false;
		private Oscillogram oscSelectingHelper = null;

		private System.Windows.Forms.MenuItem cmmiExport;
		private System.Windows.Forms.MenuItem cmmiExportWholeGraph;
		private System.Windows.Forms.MenuItem cmmiCopyToClipboard;
		private System.Windows.Forms.MenuItem cmmiExportOnlySelection;
		private System.Windows.Forms.MenuItem cmmiCopyToClipboardWholeGraph;
		private System.Windows.Forms.MenuItem cmmiCopyToClipboardOnlySelection;
		private System.Windows.Forms.SaveFileDialog sfdExportFile;

		#region public Oscillogram(PetriNetDocument pnd, IntMatrix im)
		public Oscillogram(PetriNetDocument pnd, IntMatrix im)
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			this.SetStyle(ControlStyles.UserPaint |	ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer, true);
			this.UpdateStyles();

			this.im = im;
			this.pnd = pnd;
	
			this.iGraphHeight = this.pnd.ResponseOptions.GraphHeight;
			this.iGraphDistance = this.pnd.ResponseOptions.GraphDistance;
			this.iSectionWidth = this.pnd.ResponseOptions.SectionWidth;
			this.iNumbersPerSection = this.pnd.ResponseOptions.NumbersPerSection;
			this.szSelection = pnd.ResponseOptions.Selection;
			this.szRange = new Size(0, pnd.ResponseOptions.EndTime);

			this.Height = iTopMargin + iGraphHeight * this.pnd.ResponseOptions.Places.Length + iGraphDistance * (this.pnd.ResponseOptions.Places.Length - 1) + iBottomMargin;
			this.Width = iLeftMargin + iSectionWidth * (this.szRange.Height - this.szRange.Width) / (this.iNumbersPerSection) + iRightMargin;

			this.alGroupedPlaces = pnd.GroupedPlaces;
			this.aResponsePlaces = pnd.ResponseOptions.Places;
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
			this.cmMenu = new System.Windows.Forms.ContextMenu();
			this.cmmiExport = new System.Windows.Forms.MenuItem();
			this.cmmiExportWholeGraph = new System.Windows.Forms.MenuItem();
			this.cmmiExportOnlySelection = new System.Windows.Forms.MenuItem();
			this.cmmiCopyToClipboard = new System.Windows.Forms.MenuItem();
			this.cmmiCopyToClipboardWholeGraph = new System.Windows.Forms.MenuItem();
			this.cmmiCopyToClipboardOnlySelection = new System.Windows.Forms.MenuItem();
			this.sfdExportFile = new System.Windows.Forms.SaveFileDialog();
			// 
			// cmMenu
			// 
			this.cmMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																				   this.cmmiExport,
																				   this.cmmiCopyToClipboard});
			// 
			// cmmiExport
			// 
			this.cmmiExport.Index = 0;
			this.cmmiExport.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					   this.cmmiExportWholeGraph,
																					   this.cmmiExportOnlySelection});
			this.cmmiExport.Text = "&Export";
			// 
			// cmmiExportWholeGraph
			// 
			this.cmmiExportWholeGraph.Index = 0;
			this.cmmiExportWholeGraph.Text = "Whole &Graph...";
			this.cmmiExportWholeGraph.Click += new System.EventHandler(this.cmmiExportWholeGraph_Click);
			// 
			// cmmiExportOnlySelection
			// 
			this.cmmiExportOnlySelection.Index = 1;
			this.cmmiExportOnlySelection.Text = "Only &Selection...";
			this.cmmiExportOnlySelection.Click += new System.EventHandler(this.cmmiExportOnlySelection_Click);
			// 
			// cmmiCopyToClipboard
			// 
			this.cmmiCopyToClipboard.Index = 1;
			this.cmmiCopyToClipboard.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																								this.cmmiCopyToClipboardWholeGraph,
																								this.cmmiCopyToClipboardOnlySelection});
			this.cmmiCopyToClipboard.Text = "&Copy to Clipboard";
			// 
			// cmmiCopyToClipboardWholeGraph
			// 
			this.cmmiCopyToClipboardWholeGraph.Index = 0;
			this.cmmiCopyToClipboardWholeGraph.Text = "Whole &Graph";
			this.cmmiCopyToClipboardWholeGraph.Click += new System.EventHandler(this.cmmiCopyToClipboardWholeGraph_Click);
			// 
			// cmmiCopyToClipboardOnlySelection
			// 
			this.cmmiCopyToClipboardOnlySelection.Index = 1;
			this.cmmiCopyToClipboardOnlySelection.Text = "Only &Selection";
			this.cmmiCopyToClipboardOnlySelection.Click += new System.EventHandler(this.cmmiCopyToClipboardOnlySelection_Click);
			// 
			// sfdExportFile
			// 
			this.sfdExportFile.DefaultExt = "emf";
			this.sfdExportFile.Filter = "Enhanced metafile (*.emf)|*.emf";
			this.sfdExportFile.Title = "Export Oscillogram.";
			// 
			// Oscillogram
			// 
			this.ContextMenu = this.cmMenu;
			this.Name = "Oscillogram";
			this.Size = new System.Drawing.Size(150, 124);
			this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Oscillogram_MouseUp);
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.Oscillogram_Paint);
			this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Oscillogram_MouseMove);
			this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Oscillogram_MouseDown);

		}
		#endregion

		#region private void Oscilogram_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		private void Oscillogram_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			DrawGraph(e.Graphics, this.szRange, this.bAllowSelection);
		}
		#endregion

		#region private void DrawGraph(Graphics g, Size szRange, bool bUseSelectionLinesAndHatch)
		private void DrawGraph(Graphics g, Size szRange, bool bUseSelectionLinesAndHatch)
		{
			if (PetriNetDocument.AntiAlias == true)
			{
				g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
				g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
			}

			Brush bGraphBackground = new SolidBrush(this.cGraphBackground);
			Brush bSelectionBackground = new HatchBrush(HatchStyle.LightDownwardDiagonal, Color.Gray, cGraphBackground);

			Pen pGraphBorder = new Pen(this.cGraphBorder, 1);
			Pen pLine = new Pen(this.cLine, 2);
			Pen pSelectionLine = new Pen(Color.Red, 1);
			pSelectionLine.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
			pSelectionLine.DashPattern = new float[]{5, 5};

			Brush bText = new SolidBrush(this.cText);

			Font fntBold = new Font(this.Font, FontStyle.Bold);
			Font fnt = new Font(this.Font, FontStyle.Regular);

			StringFormat sfVertical = new StringFormat();
			sfVertical.LineAlignment = StringAlignment.Center;
			sfVertical.Alignment = StringAlignment.Center;
			sfVertical.FormatFlags = StringFormatFlags.DirectionVertical;

			StringFormat sf = new StringFormat();
			sf.LineAlignment = StringAlignment.Center;
			sf.Alignment = StringAlignment.Center;

			// Draw graph backgrounds
			for(int i = 0; i < aResponsePlaces.Length; i++)
			{
				Rectangle rGraphRect = new Rectangle(this.iLeftMargin, this.iTopMargin + i * this.iGraphHeight + i * this.iGraphDistance, this.iSectionWidth * (szRange.Height - szRange.Width ) / this.iNumbersPerSection, this.iGraphHeight);
				g.FillRectangle(bGraphBackground, rGraphRect);

				if (bUseSelectionLinesAndHatch == true)
				{
					Rectangle rInverseSelectionRectLeft = new Rectangle(this.iLeftMargin, this.iTopMargin + i * this.iGraphHeight + i * this.iGraphDistance, this.iSectionWidth * (this.szSelection.Width - szRange.Width ) / this.iNumbersPerSection, this.iGraphHeight);
					Rectangle rInverseSelectionRectRight = new Rectangle(this.iLeftMargin + this.iSectionWidth * (this.szSelection.Height - szRange.Width ) / this.iNumbersPerSection, this.iTopMargin + i * this.iGraphHeight + i * this.iGraphDistance, rGraphRect.Width - + this.iSectionWidth * (this.szSelection.Height - szRange.Width ) / this.iNumbersPerSection, this.iGraphHeight);
					g.FillRectangle(bSelectionBackground, rInverseSelectionRectLeft);
					g.FillRectangle(bSelectionBackground, rInverseSelectionRectRight);
				}

				g.DrawRectangle(pGraphBorder, rGraphRect);

				string sYLabel = (this.pnd.ResponseOptions.ShowIndexes == true) ? ("P" + aResponsePlaces[i].Index + " - " + aResponsePlaces[i].NameID) : aResponsePlaces[i].NameID;

				g.DrawString(sYLabel, fntBold, bText, new Rectangle(this.iLeftMargin - 70, this.iTopMargin + i * this.iGraphHeight + i * this.iGraphDistance, 30, this.iGraphHeight), sfVertical);

				int iMatrixRowIndex = alGroupedPlaces.IndexOf(aResponsePlaces[i]);

				// Determine value range
				int iMax = 0;
				int iMin = 0;

				for (int k = 0; k < im.Dimensions.Width; k++)
				{
					if ((int)im[iMatrixRowIndex, k] > iMax)
						iMax = (int)im[iMatrixRowIndex, k];
					else if ((int)im[iMatrixRowIndex, k] < iMin)
						iMin = (int)im[iMatrixRowIndex, k];
				}
				if (iMax == 0 && iMin == 0)
				{
					iMax = 1;
				}

				// Draw vertical sections
				float fY = 0;
				for(int k = iMax; k >= iMin; k--)
				{
					fY = this.iTopMargin + i * this.iGraphHeight + i * this.iGraphDistance + this.iGraphHeight - (iMax - k) * ((float)this.iGraphHeight/(float)(iMax - iMin));
					g.DrawLine(pGraphBorder, this.iLeftMargin, fY, this.iLeftMargin + this.iSectionLine, fY);

					// Draw zero line
					if (k == (iMax + iMin) && iMin < 0)
					{
						Pen pDashed = new Pen(cGraphBorder);
						pDashed.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
						pDashed.DashPattern = new float[2]{3f, 5f};
						g.DrawLine(pDashed, this.iLeftMargin, fY, this.iLeftMargin + this.iSectionWidth * (this.im.Dimensions.Width - 1), fY);
					}
				}

				// Draw vertical section numbers
				g.DrawString(iMax.ToString(), fnt, bText, new Rectangle(this.iLeftMargin - 40, this.iTopMargin + i * this.iGraphHeight + i * this.iGraphDistance - 6, 40, 14), sf);
				g.DrawString(iMin.ToString(), fnt, bText, new Rectangle(this.iLeftMargin - 40, this.iTopMargin + i * this.iGraphHeight + i * this.iGraphDistance + this.iGraphHeight - 7, 40, 14), sf);

				// Draw horizontal sections
				for(int k = 0; k <= (szRange.Height - szRange.Width) / this.iNumbersPerSection; k++)
				{
					float fX = this.iLeftMargin + k * this.iSectionWidth;
					g.DrawLine(pGraphBorder, fX, this.iTopMargin + i * this.iGraphHeight + i * this.iGraphDistance + this.iGraphHeight - this.iSectionLine, fX, this.iTopMargin + i * this.iGraphHeight + i * this.iGraphDistance + this.iGraphHeight);
					g.DrawString(((int)(szRange.Width + k * this.iNumbersPerSection)).ToString(), fnt, bText, new Rectangle((int)(fX - 25f), this.iTopMargin + i * this.iGraphHeight + i * this.iGraphDistance + this.iGraphHeight, 50, 20), sf);
				}

				// Calculate Points
				PointF[] pa = new PointF[(szRange.Height - szRange.Width) * 2 + 2];
				float fYSection = ((float)this.iGraphHeight/(float)(iMax - iMin));

				pa[0] = new PointF(this.iLeftMargin, this.iTopMargin + i * this.iGraphHeight + i * this.iGraphDistance + this.iGraphHeight - fYSection * ((int)im[iMatrixRowIndex, szRange.Width] - iMin));
				pa[1] = new PointF(this.iLeftMargin + this.iSectionWidth / this.iNumbersPerSection, this.iTopMargin + i * this.iGraphHeight + i * this.iGraphDistance + this.iGraphHeight - fYSection * ((int)im[iMatrixRowIndex, szRange.Width] - iMin));

				for (int z = 1; z <= szRange.Height - szRange.Width; z++)
				{
					pa[2*z] = new PointF(this.iLeftMargin + z * this.iSectionWidth / this.iNumbersPerSection, this.iTopMargin + i * this.iGraphHeight + i * this.iGraphDistance + this.iGraphHeight - fYSection * ((int)im[iMatrixRowIndex, z + szRange.Width - 1] - iMin));
					pa[2*z + 1] = new PointF(this.iLeftMargin + z * this.iSectionWidth / this.iNumbersPerSection, this.iTopMargin + i * this.iGraphHeight + i * this.iGraphDistance + this.iGraphHeight - fYSection * ((int)im[iMatrixRowIndex, z + szRange.Width] - iMin));
				}

				g.DrawLines(pLine, pa);
			}

			if (bUseSelectionLinesAndHatch == true)
			{
				Point ptLeftSelectionLine1 = new Point(this.iLeftMargin + (this.szSelection.Width - szRange.Width) * this.iSectionWidth / this.iNumbersPerSection, 60);
				Point ptLeftSelectionLine2 = new Point(this.iLeftMargin + (this.szSelection.Width - szRange.Width) * this.iSectionWidth / this.iNumbersPerSection, this.Height);
				g.DrawLine(pSelectionLine, ptLeftSelectionLine1, ptLeftSelectionLine2);

				g.DrawString(this.szSelection.Width.ToString(), fnt, bText, new Rectangle(ptLeftSelectionLine1.X - 50, ptLeftSelectionLine1.Y - 40, 100, 50), sf);

				Point ptRightSelectionLine1 = new Point(this.iLeftMargin + (this.szSelection.Height - szRange.Width) * this.iSectionWidth / this.iNumbersPerSection, 60);
				Point ptRightSelectionLine2 = new Point(this.iLeftMargin + (this.szSelection.Height - szRange.Width) * this.iSectionWidth / this.iNumbersPerSection, this.Height);
				g.DrawLine(pSelectionLine, ptRightSelectionLine1, ptRightSelectionLine2);

				g.DrawString(this.szSelection.Height.ToString(), fnt, bText, new Rectangle(ptRightSelectionLine1.X - 50, ptRightSelectionLine1.Y - 40, 100, 50), sf);

				// Draw period
				int iPeriod = szSelection.Height - szSelection.Width;
				g.DrawString("T = " + iPeriod.ToString(), fnt, bText, new Rectangle(new Point(ptLeftSelectionLine1.X, ptLeftSelectionLine1.Y - 40), new Size(ptRightSelectionLine1.X - ptLeftSelectionLine1.X, 50)), sf);

				// Draw arrowed line
				Pen pPeriodLine = new Pen(Color.Black, 1);
				pPeriodLine.CustomStartCap = new AdjustableArrowCap(3, 6, true);
				pPeriodLine.CustomEndCap = new AdjustableArrowCap(3, 6, true);

				g.DrawLine(pPeriodLine, ptLeftSelectionLine1, ptRightSelectionLine1);
			}

			if (this.bAllowMarker == true)
			{
				Point ptMarkerLine1 = new Point(this.iLeftMargin + (this.iMarker - szRange.Width) * this.iSectionWidth / this.iNumbersPerSection, 60);
				Point ptMarkerLine2 = new Point(this.iLeftMargin + (this.iMarker - szRange.Width) * this.iSectionWidth / this.iNumbersPerSection, this.Height);
				g.DrawLine(pSelectionLine, ptMarkerLine1, ptMarkerLine2);
			}
		}
		#endregion


		#region private void Oscillogram_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		private void Oscillogram_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				this.bSelecting = true;

				SetSelectionLine(sender, e);
			}
		}
		#endregion

		#region private void Oscillogram_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
		private void Oscillogram_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (bSelecting == true)
			{
				if (this.oscSelectingHelper == null)
				{
					TabPage tp = (TabPage)this.Parent;

					this.oscSelectingHelper = new Oscillogram(this.pnd, this.im);
					this.oscSelectingHelper.Location = new Point(0, tp.AutoScrollPosition.Y);
					this.oscSelectingHelper.Size = new Size(320, this.Size.Height);
					this.oscSelectingHelper.AllowSelection = false;
					this.oscSelectingHelper.iSectionWidth = 35;
					this.oscSelectingHelper.NumbersPerSection = 1;
					this.oscSelectingHelper.AllowMarker = true;
					this.oscSelectingHelper.cGraphBackground = Color.LightYellow;
					this.Parent.Controls.Add(this.oscSelectingHelper);
					this.oscSelectingHelper.BringToFront();

					this.Cursor = Cursors.SizeWE;
				}

				SetSelectionLine(sender, e);
			}
		}
		#endregion

		#region private void Oscillogram_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		private void Oscillogram_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			this.bSelecting = false;
			this.Parent.Controls.Remove(this.oscSelectingHelper);
			this.oscSelectingHelper = null;

			this.Cursor = Cursors.Default;
		}
		#endregion


		#region public void SetSelectionLine(object sender, MouseEventArgs e)
		public void SetSelectionLine(object sender, MouseEventArgs e)
		{
			int iLeft = this.szSelection.Width;
			int iRight = this.szSelection.Height;

			int i = (e.X - this.iLeftMargin) * this.iNumbersPerSection / this.iSectionWidth + this.szRange.Width;

			if (i <= (iRight - iLeft) / 2 + iLeft)
			{
				iLeft = i;
				if (this.oscSelectingHelper != null)
				{
					this.oscSelectingHelper.iMarker = (i < 0) ? 0 : i;
					this.oscSelectingHelper.Range = new Size((iLeft-3 <= 0) ? 0 : iLeft-3, (iLeft-3 <= 0) ? 6 : iLeft+3);
				}
			}
			else
			{
				iRight = i;

				if (this.oscSelectingHelper != null)
				{
					this.oscSelectingHelper.iMarker = (i > this.pnd.ResponseOptions.EndTime) ? this.pnd.ResponseOptions.EndTime : i;
					this.oscSelectingHelper.Range = new Size((iRight+3 > this.pnd.ResponseOptions.EndTime) ? this.pnd.ResponseOptions.EndTime - 6 : iRight-3, (iRight+3 > this.pnd.ResponseOptions.EndTime) ? this.pnd.ResponseOptions.EndTime : iRight+3);
				}
			}

			if (iLeft - this.szRange.Width < 0)
				iLeft = this.szRange.Width;
			if (iRight > this.pnd.ResponseOptions.EndTime)
				iRight = this.pnd.ResponseOptions.EndTime;

			this.szSelection = new Size(iLeft, iRight);
			this.pnd.ResponseOptions.Selection = this.szSelection;

			this.Refresh();
		}
		#endregion


		#region private void cmmiExportWholeGraph_Click(object sender, System.EventArgs e)
		private void cmmiExportWholeGraph_Click(object sender, System.EventArgs e)
		{
#if !DEMO
			DialogResult dr = MessageBox.Show("Would you like to include selection lines and hatched surface in created metafile?", "Petri .NET Simulator 1.0 - Question", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

			if (dr != DialogResult.Cancel)
			{
				sfdExportFile.FileName = "";

				if (DialogResult.OK == sfdExportFile.ShowDialog())
				{
					FileStream fs = File.Create(sfdExportFile.FileName);

					Graphics g = this.CreateGraphics();
					IntPtr ip = g.GetHdc();
					Metafile mf = new Metafile(fs, ip);
					g.ReleaseHdc(ip);
					g.Dispose();

					Graphics gg = Graphics.FromImage(mf);

					if (dr == DialogResult.Yes)
						DrawGraph(gg, this.szRange, true);
					else
						DrawGraph(gg, this.szRange, false);

					gg.Dispose();

					fs.Close();
				}
			}
#else
			MessageBox.Show("DEMO version doesn't have implemented Export function!\nBuy a full version which is capable of exporting oscillograms.", "Petri .NET Simulator 2.0 - Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
#endif
		}
		#endregion

		#region private void cmmiExportOnlySelection_Click(object sender, System.EventArgs e)
		private void cmmiExportOnlySelection_Click(object sender, System.EventArgs e)
		{
#if !DEMO
			sfdExportFile.FileName = "";

			if (DialogResult.OK == sfdExportFile.ShowDialog())
			{
				FileStream fs = File.Create(sfdExportFile.FileName);

				Graphics g = this.CreateGraphics();
				IntPtr ip = g.GetHdc();
				Metafile mf = new Metafile(fs, ip);
				g.ReleaseHdc(ip);
				g.Dispose();

				Graphics gg = Graphics.FromImage(mf);

				DrawGraph(gg, this.pnd.ResponseOptions.Selection, false);

				gg.Dispose();

				fs.Close();
			}
#else
			MessageBox.Show("DEMO version doesn't have implemented Export function!\nBuy a full version which is capable of exporting oscillograms.", "Petri .NET Simulator 2.0 - Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
#endif
		}
		#endregion

		#region private void cmmiCopyToClipboardWholeGraph_Click(object sender, System.EventArgs e)
		private void cmmiCopyToClipboardWholeGraph_Click(object sender, System.EventArgs e)
		{
#if !DEMO
			DialogResult dr = MessageBox.Show("Would you like to include selection lines and hatched surface in created metafile?", "Petri .NET Simulator 1.0 - Question", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

			if (dr != DialogResult.Cancel)
			{
				MemoryStream ms = new MemoryStream();

				Graphics g = this.CreateGraphics();
				IntPtr ip = g.GetHdc();
				Metafile mf = new Metafile(ms, ip);
				g.ReleaseHdc(ip);
				g.Dispose();

				Graphics gg = Graphics.FromImage(mf);

				if (dr == DialogResult.Yes)
					DrawGraph(gg, this.szRange, true);
				else
					DrawGraph(gg, this.szRange, false);

				gg.Dispose();

				// Copy to clipboard
				ClipboardMetafileHelper.PutEnhMetafileOnClipboard(this.Handle, mf);
			}
#else
			MessageBox.Show("DEMO version doesn't have implemented Copy to Clipboard function!\nBuy a full version which is capable of Copying to Clipboard.", "Petri .NET Simulator 2.0 - Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
#endif
		}
		#endregion

		#region private void cmmiCopyToClipboardOnlySelection_Click(object sender, System.EventArgs e)
		private void cmmiCopyToClipboardOnlySelection_Click(object sender, System.EventArgs e)
		{
#if !DEMO
			MemoryStream ms = new MemoryStream();

			Graphics g = this.CreateGraphics();
			IntPtr ip = g.GetHdc();
			Metafile mf = new Metafile(ms, ip);
			g.ReleaseHdc(ip);
			g.Dispose();

			Graphics gg = Graphics.FromImage(mf);

			DrawGraph(gg, this.pnd.ResponseOptions.Selection, false);

			gg.Dispose();

			// Copy to clipboard
			ClipboardMetafileHelper.PutEnhMetafileOnClipboard(this.Handle, mf);
#else
			MessageBox.Show("DEMO version doesn't have implemented Copy to Clipboard function!\nBuy a full version which is capable of Copying to Clipboard.", "Petri .NET Simulator 2.0 - Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
#endif
		}
		#endregion



	}
}
