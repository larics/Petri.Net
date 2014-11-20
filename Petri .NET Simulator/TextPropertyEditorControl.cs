using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Windows.Forms.Design;

namespace PetriNetSimulator2
{
	/// <summary>
	/// Summary description for TextPropertyEditor.
	/// </summary>
	public class TextPropertyEditorControl : System.Windows.Forms.UserControl
	{
		#region public override string Text
		public override string Text
		{
			get
			{
				if (this.bForcedClose == true)
				{
					return this.sCached;
				}
				else
					return this.tbTextBox.Text;
			}
		}
		#endregion

		#region public bool ForcedClose
		public bool ForcedClose
		{
			get
			{
				return this.bForcedClose;
			}
		}
		#endregion

		private System.Windows.Forms.TextBox tbTextBox;
		private IWindowsFormsEditorService edSvc;
		private System.Windows.Forms.Label lblStatus;
		private bool bForcedClose = false;
		private string sCached = "";

		private System.ComponentModel.Container components = null;

		public TextPropertyEditorControl(string sText, IWindowsFormsEditorService edSvc)
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			this.tbTextBox.Text = sText;
			this.edSvc = edSvc;
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
			this.tbTextBox = new System.Windows.Forms.TextBox();
			this.lblStatus = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// tbTextBox
			// 
			this.tbTextBox.AcceptsReturn = true;
			this.tbTextBox.Dock = System.Windows.Forms.DockStyle.Top;
			this.tbTextBox.Location = new System.Drawing.Point(0, 0);
			this.tbTextBox.Multiline = true;
			this.tbTextBox.Name = "tbTextBox";
			this.tbTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.tbTextBox.Size = new System.Drawing.Size(312, 112);
			this.tbTextBox.TabIndex = 0;
			this.tbTextBox.Text = "";
			this.tbTextBox.WordWrap = false;
			this.tbTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tbTextBox_KeyDown);
			// 
			// lblStatus
			// 
			this.lblStatus.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.lblStatus.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lblStatus.Location = new System.Drawing.Point(0, 112);
			this.lblStatus.Name = "lblStatus";
			this.lblStatus.Size = new System.Drawing.Size(312, 24);
			this.lblStatus.TabIndex = 1;
			this.lblStatus.Text = "Press CTRL+ENTER to close this editor.";
			this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// TextPropertyEditorControl
			// 
			this.Controls.Add(this.lblStatus);
			this.Controls.Add(this.tbTextBox);
			this.Name = "TextPropertyEditorControl";
			this.Size = new System.Drawing.Size(312, 136);
			this.ResumeLayout(false);

		}
		#endregion

		#region private void tbTextBox_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		private void tbTextBox_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter && e.Control == true)
			{
				this.sCached = this.tbTextBox.Text;
				this.bForcedClose = true;
				this.edSvc.CloseDropDown();
			}
		}
		#endregion
	}
}
