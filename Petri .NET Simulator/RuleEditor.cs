using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace PetriNetSimulator2
{
	/// <summary>
	/// Summary description for NewRuleEditor.
	/// </summary>
	public class RuleEditor : System.Windows.Forms.Form
	{
		#region public PetriNetDocument Document
		public PetriNetDocument Document
		{
			get
			{
				return this.pndSelectedDocument;
			}
			set
			{
				this.pndSelectedDocument = value;
			}
		}
		#endregion

		#region public Rule CreatedRule
		public Rule CreatedRule
		{
			get
			{
				return this.rCreatedRule;
			}
		}
		#endregion

		private PetriNetDocument pndSelectedDocument = null;

		private System.Windows.Forms.GroupBox gbNewRule;
		private System.Windows.Forms.TextBox tbRuleCondition;
		private System.Windows.Forms.Label lblRuleCondition;
		private System.Windows.Forms.TextBox tbRuleDescription;
		private System.Windows.Forms.Label lblOpacity;
		private System.Windows.Forms.TrackBar tbOpacity;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Label lblRuleDescription;

		private Rule rCreatedRule = null;

		private System.ComponentModel.Container components = null;

		#region public RuleEditor(PetriNetDocument pndSelectedDocument)
		public RuleEditor(PetriNetDocument pndSelectedDocument)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			this.pndSelectedDocument = pndSelectedDocument;
		}
		#endregion

		#region public RuleEditor(PetriNetDocument pndSelectedDocument, string sExpression, string sDescription)
		public RuleEditor(PetriNetDocument pndSelectedDocument, string sExpression, string sDescription)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			this.pndSelectedDocument = pndSelectedDocument;
			this.tbRuleCondition.Text = sExpression;
			this.tbRuleDescription.Text = sDescription;
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

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.gbNewRule = new System.Windows.Forms.GroupBox();
			this.tbRuleDescription = new System.Windows.Forms.TextBox();
			this.lblRuleDescription = new System.Windows.Forms.Label();
			this.lblRuleCondition = new System.Windows.Forms.Label();
			this.tbRuleCondition = new System.Windows.Forms.TextBox();
			this.lblOpacity = new System.Windows.Forms.Label();
			this.tbOpacity = new System.Windows.Forms.TrackBar();
			this.btnCancel = new System.Windows.Forms.Button();
			this.btnOK = new System.Windows.Forms.Button();
			this.gbNewRule.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.tbOpacity)).BeginInit();
			this.SuspendLayout();
			// 
			// gbNewRule
			// 
			this.gbNewRule.Controls.Add(this.tbRuleDescription);
			this.gbNewRule.Controls.Add(this.lblRuleDescription);
			this.gbNewRule.Controls.Add(this.lblRuleCondition);
			this.gbNewRule.Controls.Add(this.tbRuleCondition);
			this.gbNewRule.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.gbNewRule.Location = new System.Drawing.Point(16, 16);
			this.gbNewRule.Name = "gbNewRule";
			this.gbNewRule.Size = new System.Drawing.Size(696, 128);
			this.gbNewRule.TabIndex = 4;
			this.gbNewRule.TabStop = false;
			this.gbNewRule.Text = "New rule : ";
			// 
			// tbRuleDescription
			// 
			this.tbRuleDescription.AcceptsReturn = true;
			this.tbRuleDescription.Location = new System.Drawing.Point(126, 62);
			this.tbRuleDescription.Multiline = true;
			this.tbRuleDescription.Name = "tbRuleDescription";
			this.tbRuleDescription.Size = new System.Drawing.Size(560, 54);
			this.tbRuleDescription.TabIndex = 6;
			this.tbRuleDescription.Text = "";
			// 
			// lblRuleDescription
			// 
			this.lblRuleDescription.AutoSize = true;
			this.lblRuleDescription.Location = new System.Drawing.Point(12, 64);
			this.lblRuleDescription.Name = "lblRuleDescription";
			this.lblRuleDescription.Size = new System.Drawing.Size(112, 18);
			this.lblRuleDescription.TabIndex = 5;
			this.lblRuleDescription.Text = "Rule description : ";
			// 
			// lblRuleCondition
			// 
			this.lblRuleCondition.AutoSize = true;
			this.lblRuleCondition.Location = new System.Drawing.Point(12, 31);
			this.lblRuleCondition.Name = "lblRuleCondition";
			this.lblRuleCondition.Size = new System.Drawing.Size(112, 18);
			this.lblRuleCondition.TabIndex = 4;
			this.lblRuleCondition.Text = "Rule expression : ";
			// 
			// tbRuleCondition
			// 
			this.tbRuleCondition.Location = new System.Drawing.Point(126, 28);
			this.tbRuleCondition.Name = "tbRuleCondition";
			this.tbRuleCondition.Size = new System.Drawing.Size(560, 22);
			this.tbRuleCondition.TabIndex = 3;
			this.tbRuleCondition.Text = "";
			// 
			// lblOpacity
			// 
			this.lblOpacity.AutoSize = true;
			this.lblOpacity.Location = new System.Drawing.Point(17, 165);
			this.lblOpacity.Name = "lblOpacity";
			this.lblOpacity.Size = new System.Drawing.Size(109, 18);
			this.lblOpacity.TabIndex = 9;
			this.lblOpacity.Text = "Window opacity : ";
			// 
			// tbOpacity
			// 
			this.tbOpacity.Location = new System.Drawing.Point(129, 157);
			this.tbOpacity.Maximum = 100;
			this.tbOpacity.Minimum = 30;
			this.tbOpacity.Name = "tbOpacity";
			this.tbOpacity.Size = new System.Drawing.Size(232, 48);
			this.tbOpacity.TabIndex = 8;
			this.tbOpacity.TickFrequency = 5;
			this.tbOpacity.Value = 100;
			this.tbOpacity.Scroll += new System.EventHandler(this.tbOpacity_Scroll);
			// 
			// btnCancel
			// 
			this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnCancel.Location = new System.Drawing.Point(611, 157);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(100, 28);
			this.btnCancel.TabIndex = 7;
			this.btnCancel.Text = "&Cancel";
			// 
			// btnOK
			// 
			this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOK.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnOK.Location = new System.Drawing.Point(499, 157);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(100, 28);
			this.btnOK.TabIndex = 6;
			this.btnOK.Text = "&OK";
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// RuleEditor
			// 
			this.AcceptButton = this.btnOK;
			this.AutoScale = false;
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(722, 206);
			this.Controls.Add(this.lblOpacity);
			this.Controls.Add(this.tbOpacity);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.gbNewRule);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "RuleEditor";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "New Rule Editor";
			this.Closing += new System.ComponentModel.CancelEventHandler(this.RuleEditor_Closing);
			this.gbNewRule.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.tbOpacity)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion


		#region private void btnOK_Click(object sender, System.EventArgs e)
		private void btnOK_Click(object sender, System.EventArgs e)
		{
			try
			{
				this.rCreatedRule = new Rule(tbRuleCondition.Text, tbRuleDescription.Text);

				if (this.rCreatedRule.IsValidCondition(this.pndSelectedDocument) == false)
				{
					MessageBox.Show("Condition part of expression contains non-existing place NameIDs or constants!", "Petri .NET Simulator 1.0 - Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					this.rCreatedRule = null;
					return;
				}
				else
				{
					if (this.rCreatedRule.IsValidResult(this.pndSelectedDocument) == false)
					{
						MessageBox.Show("Statement part of expression contains non-existing control place NameIDs or constants!", "Petri .NET Simulator 1.0 - Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
						this.rCreatedRule = null;
						return;
					}
				}

			}
			catch(Exception)
			{
				MessageBox.Show("Expression part of rule is invalid!", "Petri .NET Simulator 1.0 - Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				this.rCreatedRule = null;
				return;
			}
		}
		#endregion

		#region private void tbOpacity_Scroll(object sender, System.EventArgs e)
		private void tbOpacity_Scroll(object sender, System.EventArgs e)
		{
			this.Opacity = this.tbOpacity.Value / 100f;
		}
		#endregion

		#region private void RuleEditor_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		private void RuleEditor_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (this.DialogResult == DialogResult.OK && this.CreatedRule == null)
			{
				e.Cancel = true;
			}
		}
		#endregion

	

	}
}
