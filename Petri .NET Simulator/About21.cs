using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Reflection;

namespace PetriNetSimulator2
{
    public partial class About21 : Form
    {
        public static Image SplashScreen = null;
        

        public About21()
        {
            InitializeComponent();

            SplashScreen = Image.FromStream(GetResource("splash.png"));
        }


        public static System.IO.Stream GetResource(string fileName)
        {
            Assembly a = Assembly.GetExecutingAssembly();
            string[] resNames = a.GetManifestResourceNames();
            foreach (string s in resNames)
            {
                if (s.EndsWith(fileName))
                {
                    return a.GetManifestResourceStream(s);
                }
            }
            return null;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            //  Do nothing here!
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            Graphics gfx = e.Graphics;

            gfx.DrawImage(SplashScreen, new Rectangle(0, 0, this.Width, this.Height));

        }

        private void About21_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void About21_Load(object sender, EventArgs e)
        {
            labelProgBy.Text = "Copyright && program by Goran Genter, 2004 and Ivica Sindicic, 2008-2010";
            lblAppVersion.Text = "Version:" + Application.ProductVersion;

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

        private void lblAppVersion_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Clipboard.SetText("("+lblAppVersion.Text+")");
            System.Media.SystemSounds.Beep.Play();
        }

    }
}