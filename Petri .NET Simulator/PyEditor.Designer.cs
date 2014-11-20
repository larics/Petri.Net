namespace PetriNetSimulator2
{
    partial class PyEditor
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.pyCode = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // pyCode
            // 
            this.pyCode.AcceptsTab = true;
            this.pyCode.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pyCode.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.pyCode.Location = new System.Drawing.Point(0, 0);
            this.pyCode.Multiline = true;
            this.pyCode.Name = "pyCode";
            this.pyCode.Size = new System.Drawing.Size(738, 236);
            this.pyCode.TabIndex = 2;
            this.pyCode.WordWrap = false;
            this.pyCode.TextChanged += new System.EventHandler(this.pyCode_TextChanged);
            // 
            // PyEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pyCode);
            this.Name = "PyEditor";
            this.Size = new System.Drawing.Size(738, 236);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox pyCode;
    }
}