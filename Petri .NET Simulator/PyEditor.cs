using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PetriNetSimulator2
{
    public partial class PyEditor : System.Windows.Forms.UserControl
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
                if (pndSelectedDocument != null)
                {
                    this.pyCode.Text = pndSelectedDocument.pyCode;
                    this.pyCode.Enabled = true;
                }
                else
                    this.pyCode.Enabled = false;
            }
		}
		#endregion

        private PetriNetDocument pndSelectedDocument = null;

        public PyEditor()
        {
            InitializeComponent();
            this.pyCode.Enabled = false;

            //ICSharpCode.TextEditor.Document.HighlightingManager.Manager.AddSyntaxModeFileProvider( new PetriNetSimulator2.Syntax.SyntaxModeFileProvider() ); // Attach to the text editor.
            //this.pyCode.SetHighlighting("C#"); // Activate the highlighting, use the name from the SyntaxDefinition node.
            //this.pyCode.SetHighlighting("Python"); // Activate the highlighting, use the name from the SyntaxDefinition node.

        }

        private void pyCode_TextChanged(object sender, EventArgs e)
        {
            if (pndSelectedDocument != null)
                pndSelectedDocument.pyCode = this.pyCode.Text;
        }

        public void Enable()
        {
            pyCode.IsReadOnly = false;
        }

        public void Disable()
        {
            pyCode.IsReadOnly = true;
        }
    }
}