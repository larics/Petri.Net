using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PetriNetSimulator2
{
    public partial class SimulateParamForm : Form
    {
        private Simulator simulator;

        public SimulateParamForm(Simulator s)
        {
            InitializeComponent();
            simulator = s;
        }

        private void SimulateParamForm_Load(object sender, EventArgs e)
        {
            tbPause.Text = simulator.sleepBetweenStep.ToString();
            cbStopIfNoFireable.Checked = !simulator.ignoreLackOfFireableTransition;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            int v;
            if (!Int32.TryParse(tbPause.Text, out v))
            {
                tbPause.SelectAll();
                tbPause.Focus();
                return;
            }
            simulator.sleepBetweenStep = v > 5 ? v : 5;
            simulator.ignoreLackOfFireableTransition = !cbStopIfNoFireable.Checked;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}