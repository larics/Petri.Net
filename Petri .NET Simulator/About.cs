using System;
using System.IO;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace PetriNetSimulator2
{
	/// <summary>
	/// Summary description for About.
	/// </summary>
	public class About : System.Windows.Forms.Form
	{
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.Label lblCopyright;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.GroupBox gbSplitter;
		private System.Windows.Forms.Label lblAppName;
		private System.Windows.Forms.Label lblAppVersion;
		private System.Windows.Forms.Label lblWarning1;
		private System.Windows.Forms.Label lblWarning2;
		private System.Windows.Forms.Label lblCheckHere;
		private System.Windows.Forms.LinkLabel linkWeb;
		private System.Windows.Forms.Label lblMail;
		private System.Windows.Forms.LinkLabel linkMail;
		private System.Windows.Forms.GroupBox gbSplitter2;
		private System.Windows.Forms.Label lblSpecialThanks;
		private System.Windows.Forms.Label lblLARICS;
		private System.Windows.Forms.LinkLabel linkLarics;
		private System.Windows.Forms.LinkLabel linkDotNetMagic;
		private System.Windows.Forms.Label lblDotNetMagic;
		private System.Windows.Forms.Label lblEdition;
		private System.Windows.Forms.PictureBox pictureBox2;
        private Label label1;
        private Label label2;
        private LinkLabel linkLabel1;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public About()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
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

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(About));
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.gbSplitter = new System.Windows.Forms.GroupBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.lblCopyright = new System.Windows.Forms.Label();
            this.lblAppName = new System.Windows.Forms.Label();
            this.lblAppVersion = new System.Windows.Forms.Label();
            this.lblWarning1 = new System.Windows.Forms.Label();
            this.lblWarning2 = new System.Windows.Forms.Label();
            this.lblCheckHere = new System.Windows.Forms.Label();
            this.linkWeb = new System.Windows.Forms.LinkLabel();
            this.lblMail = new System.Windows.Forms.Label();
            this.linkMail = new System.Windows.Forms.LinkLabel();
            this.gbSplitter2 = new System.Windows.Forms.GroupBox();
            this.lblSpecialThanks = new System.Windows.Forms.Label();
            this.linkLarics = new System.Windows.Forms.LinkLabel();
            this.lblLARICS = new System.Windows.Forms.Label();
            this.linkDotNetMagic = new System.Windows.Forms.LinkLabel();
            this.lblDotNetMagic = new System.Windows.Forms.Label();
            this.lblEdition = new System.Windows.Forms.Label();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(20, 14);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(120, 250);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // gbSplitter
            // 
            this.gbSplitter.Location = new System.Drawing.Point(15, 309);
            this.gbSplitter.Name = "gbSplitter";
            this.gbSplitter.Size = new System.Drawing.Size(430, 6);
            this.gbSplitter.TabIndex = 1;
            this.gbSplitter.TabStop = false;
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnOK.Location = new System.Drawing.Point(362, 498);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(83, 24);
            this.btnOK.TabIndex = 2;
            this.btnOK.Text = "&OK";
            // 
            // lblCopyright
            // 
            this.lblCopyright.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.lblCopyright.Location = new System.Drawing.Point(13, 482);
            this.lblCopyright.Name = "lblCopyright";
            this.lblCopyright.Size = new System.Drawing.Size(335, 20);
            this.lblCopyright.TabIndex = 3;
            this.lblCopyright.Text = "Copyright © 2004.  BIGENERIC (Goran Genter).  All rights reserved.";
            // 
            // lblAppName
            // 
            this.lblAppName.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.lblAppName.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblAppName.Location = new System.Drawing.Point(143, 12);
            this.lblAppName.Name = "lblAppName";
            this.lblAppName.Size = new System.Drawing.Size(140, 20);
            this.lblAppName.TabIndex = 4;
            this.lblAppName.Text = "Petri .NET Simulator";
            // 
            // lblAppVersion
            // 
            this.lblAppVersion.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.lblAppVersion.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblAppVersion.Location = new System.Drawing.Point(290, 12);
            this.lblAppVersion.Name = "lblAppVersion";
            this.lblAppVersion.Size = new System.Drawing.Size(153, 21);
            this.lblAppVersion.TabIndex = 5;
            this.lblAppVersion.Text = "Version : ";
            // 
            // lblWarning1
            // 
            this.lblWarning1.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.lblWarning1.Location = new System.Drawing.Point(156, 58);
            this.lblWarning1.Name = "lblWarning1";
            this.lblWarning1.Size = new System.Drawing.Size(300, 62);
            this.lblWarning1.TabIndex = 6;
            this.lblWarning1.Text = "Warning 1 : This program is product of BIGENERIC Technologies and any kind of rev" +
                "ersed engineering or disassembling is strongly prohibited and would not be toler" +
                "ated!";
            // 
            // lblWarning2
            // 
            this.lblWarning2.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.lblWarning2.Location = new System.Drawing.Point(156, 120);
            this.lblWarning2.Name = "lblWarning2";
            this.lblWarning2.Size = new System.Drawing.Size(300, 54);
            this.lblWarning2.TabIndex = 7;
            this.lblWarning2.Text = "Warning 2 : You are using this program at your own risk! Any damage it could do t" +
                "o your computer (or you) in any way is not our responsibility!";
            // 
            // lblCheckHere
            // 
            this.lblCheckHere.AutoSize = true;
            this.lblCheckHere.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.lblCheckHere.Location = new System.Drawing.Point(156, 193);
            this.lblCheckHere.Name = "lblCheckHere";
            this.lblCheckHere.Size = new System.Drawing.Size(165, 13);
            this.lblCheckHere.TabIndex = 8;
            this.lblCheckHere.Text = "Check here for newest versions : ";
            // 
            // linkWeb
            // 
            this.linkWeb.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.linkWeb.Location = new System.Drawing.Point(156, 217);
            this.linkWeb.Name = "linkWeb";
            this.linkWeb.Size = new System.Drawing.Size(269, 17);
            this.linkWeb.TabIndex = 9;
            this.linkWeb.TabStop = true;
            this.linkWeb.Text = "http://petrinet.bigeneric.com";
            this.linkWeb.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkWeb_LinkClicked);
            // 
            // lblMail
            // 
            this.lblMail.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.lblMail.Location = new System.Drawing.Point(157, 243);
            this.lblMail.Name = "lblMail";
            this.lblMail.Size = new System.Drawing.Size(299, 29);
            this.lblMail.TabIndex = 10;
            this.lblMail.Text = "Mail us bugs or features that you would like to see in the next versions of this " +
                "utility : ";
            // 
            // linkMail
            // 
            this.linkMail.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.linkMail.Location = new System.Drawing.Point(157, 288);
            this.linkMail.Name = "linkMail";
            this.linkMail.Size = new System.Drawing.Size(269, 18);
            this.linkMail.TabIndex = 11;
            this.linkMail.TabStop = true;
            this.linkMail.Text = "support@petrinet.bigeneric.com";
            this.linkMail.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkMail_LinkClicked);
            // 
            // gbSplitter2
            // 
            this.gbSplitter2.Location = new System.Drawing.Point(15, 472);
            this.gbSplitter2.Name = "gbSplitter2";
            this.gbSplitter2.Size = new System.Drawing.Size(430, 7);
            this.gbSplitter2.TabIndex = 12;
            this.gbSplitter2.TabStop = false;
            // 
            // lblSpecialThanks
            // 
            this.lblSpecialThanks.AutoSize = true;
            this.lblSpecialThanks.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.lblSpecialThanks.Location = new System.Drawing.Point(17, 329);
            this.lblSpecialThanks.Name = "lblSpecialThanks";
            this.lblSpecialThanks.Size = new System.Drawing.Size(98, 13);
            this.lblSpecialThanks.TabIndex = 13;
            this.lblSpecialThanks.Text = "Special thanks to : ";
            // 
            // linkLarics
            // 
            this.linkLarics.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.linkLarics.Location = new System.Drawing.Point(28, 364);
            this.linkLarics.Name = "linkLarics";
            this.linkLarics.Size = new System.Drawing.Size(165, 17);
            this.linkLarics.TabIndex = 15;
            this.linkLarics.TabStop = true;
            this.linkLarics.Text = "http://flrcg.rasip.fer.hr";
            this.linkLarics.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLarics_LinkClicked);
            // 
            // lblLARICS
            // 
            this.lblLARICS.AutoSize = true;
            this.lblLARICS.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.lblLARICS.Location = new System.Drawing.Point(28, 348);
            this.lblLARICS.Name = "lblLARICS";
            this.lblLARICS.Size = new System.Drawing.Size(324, 13);
            this.lblLARICS.TabIndex = 14;
            this.lblLARICS.Text = "LARICS (Laboratory For Robotics And Intelligent Control Systems) : ";
            // 
            // linkDotNetMagic
            // 
            this.linkDotNetMagic.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.linkDotNetMagic.Location = new System.Drawing.Point(28, 400);
            this.linkDotNetMagic.Name = "linkDotNetMagic";
            this.linkDotNetMagic.Size = new System.Drawing.Size(165, 18);
            this.linkDotNetMagic.TabIndex = 17;
            this.linkDotNetMagic.TabStop = true;
            this.linkDotNetMagic.Text = "http://www.dotnetmagic.com";
            this.linkDotNetMagic.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkDotNetMagic_LinkClicked);
            // 
            // lblDotNetMagic
            // 
            this.lblDotNetMagic.AutoSize = true;
            this.lblDotNetMagic.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.lblDotNetMagic.Location = new System.Drawing.Point(28, 385);
            this.lblDotNetMagic.Name = "lblDotNetMagic";
            this.lblDotNetMagic.Size = new System.Drawing.Size(235, 13);
            this.lblDotNetMagic.TabIndex = 16;
            this.lblDotNetMagic.Text = "Crownwood Software Ltd. - DotNetMagic Library";
            // 
            // lblEdition
            // 
            this.lblEdition.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.lblEdition.Font = new System.Drawing.Font("Arial Black", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblEdition.ForeColor = System.Drawing.Color.Black;
            this.lblEdition.Location = new System.Drawing.Point(159, 32);
            this.lblEdition.Name = "lblEdition";
            this.lblEdition.Size = new System.Drawing.Size(267, 20);
            this.lblEdition.TabIndex = 18;
            this.lblEdition.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox2.Image")));
            this.pictureBox2.Location = new System.Drawing.Point(20, 243);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(120, 70);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox2.TabIndex = 19;
            this.pictureBox2.TabStop = false;
            // 
            // label1
            // 
            this.label1.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.label1.Location = new System.Drawing.Point(13, 502);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(335, 20);
            this.label1.TabIndex = 3;
            this.label1.Text = "Copyright © 2008,2010.  Ivica Sindicic.  All rights reserved.";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.label2.Location = new System.Drawing.Point(28, 429);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(213, 13);
            this.label2.TabIndex = 16;
            this.label2.Text = "IronPython - Python for the .NET framework";
            // 
            // linkLabel1
            // 
            this.linkLabel1.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.linkLabel1.Location = new System.Drawing.Point(28, 444);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(165, 18);
            this.linkLabel1.TabIndex = 17;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "http://ironpython.net/";
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkDotNetMagic_LinkClicked);
            // 
            // About
            // 
            this.AcceptButton = this.btnOK;
            this.CancelButton = this.btnOK;
            this.ClientSize = new System.Drawing.Size(471, 548);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.lblEdition);
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this.linkDotNetMagic);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lblDotNetMagic);
            this.Controls.Add(this.lblLARICS);
            this.Controls.Add(this.lblSpecialThanks);
            this.Controls.Add(this.lblCheckHere);
            this.Controls.Add(this.linkLarics);
            this.Controls.Add(this.gbSplitter2);
            this.Controls.Add(this.linkMail);
            this.Controls.Add(this.lblMail);
            this.Controls.Add(this.linkWeb);
            this.Controls.Add(this.lblWarning2);
            this.Controls.Add(this.lblWarning1);
            this.Controls.Add(this.lblAppVersion);
            this.Controls.Add(this.lblAppName);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblCopyright);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.gbSplitter);
            this.Controls.Add(this.pictureBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "About";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "About Petri .NET Simulator 2.0";
            this.Load += new System.EventHandler(this.About_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		#region private void About_Load(object sender, System.EventArgs e)
		private void About_Load(object sender, System.EventArgs e)
		{
			lblAppVersion.Text = "Version : " + Application.ProductVersion;

#if DEMO
			lblEdition.ForeColor = Color.Red;
			lblEdition.Text = "DEMO VERSION";
#elif STUDENT
            lblEdition.ForeColor = Color.Red;
            lblEdition.Text = "STUDENT VERSION";
#else
            lblEdition.Text = "FULL VERSION";
#endif
        }
		#endregion

		#region private void linkWeb_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
		private void linkWeb_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
		{
			API.ShellExecute(IntPtr.Zero, "Open", "http://petrinet.bigeneric.com", null, null, 3);
		}
		#endregion

		#region private void linkMail_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
		private void linkMail_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
		{
			API.ShellExecute(IntPtr.Zero, "Open", "mailto:support@petrinet.bigeneric.com?subject=Petri .NET Simulator " + Application.ProductVersion + "&body=<your message here>%0a%0a%0a%0a%0a%0a%0a%0a---------------------------------------------------------%0aOS version : " + Environment.OSVersion + "%0a.NET Framework version : " + Environment.Version, null, null, 3);
		}
		#endregion

		#region private void linkLarics_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
		private void linkLarics_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
		{
			API.ShellExecute(IntPtr.Zero, "Open", "http://flrcg.rasip.fer.hr", null, null, 3);
		}
		#endregion

		#region private void linkDotNetMagic_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
		private void linkDotNetMagic_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
		{
			API.ShellExecute(IntPtr.Zero, "Open", "http://www.dotnetmagic.com", null, null, 3);
		}
		#endregion



	}
}
