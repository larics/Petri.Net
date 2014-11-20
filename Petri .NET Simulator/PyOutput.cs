using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;



namespace PetriNetSimulator2
{
    public partial class PyOutput : UserControl
    {

        //WinAPI-Declaration for SendMessage
        [DllImport("user32.dll", EntryPoint="SendMessage")]
        public static extern IntPtr SendMessage(IntPtr window, int message, int wparam, int lparam);
        const int WM_VSCROLL = 0x115;
        const int SB_BOTTOM = 7;


        public PyOutput()
        {
            InitializeComponent();
        }

        public void PrintWithColor(string s, Color c)
        {
            s = s.Replace("\n", "\r\n");
            Color oldC = richTextBox1.SelectionColor;

            richTextBox1.Select(richTextBox1.TextLength, 0);    // To the end.. 
            richTextBox1.SelectionColor = c;                    // Set color of selection.. 
            richTextBox1.SelectedText = s;                      // Place text.. 
            PetriNetSimulator2.PyOutput.SendMessage(richTextBox1.Handle, WM_VSCROLL, SB_BOTTOM, 0); //Scroll to the bottom, without focus
            richTextBox1.SelectionColor = oldC;                 // Restore old color
        }

        public void Print(string s)
        {
            PrintWithColor(s, richTextBox1.SelectionColor);
        }

        public delegate void InvokeDelegatePrint(string input);
        public delegate void InvokeDelegatePrintColor(string input, Color c);

        public void PrintMTColor(string s, Color c)
        {
            object[] obj = new object[2];
            obj[0] = s;
            obj[1] = c;
            BeginInvoke(new InvokeDelegatePrintColor(PrintWithColor), obj);
        }

        public void PrintMT(string s)
        {
            object[] obj = new object[1];
            obj[0] = s;
            BeginInvoke(new InvokeDelegatePrint(Print), obj);
        }

        public void Clear()
        {
            this.richTextBox1.Text = "";
        }
    }
}
