using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using Crownwood.Magic.Menus;

namespace PetriNetSimulator2
{
    public class RecentFilesManager
    {
        private Form frmParent;
        private MenuCommand recentToolStripMenuItem;
        private List<string> recentFiles = null;
        private string rfFilename = ".recentFiles";
        private string lastSelectedFilename = null;
        private int maxNumOfFiles = 5;

        public event EventHandler OnRecentFileSelected = null;


        public RecentFilesManager(Form parent, MenuCommand rfMenuItem, int maxnumoffiles)
        {
            maxNumOfFiles = maxnumoffiles;
            frmParent = parent;
            recentToolStripMenuItem = rfMenuItem;

            this.InitializeRecentFiles();
        }


        public RecentFilesManager(Form parent, MenuCommand rfMenuItem)
            : this(parent, rfMenuItem, 5)
        {
        }


        private void InitializeRecentFiles()
        {
            try
            {
                Uri u = new Uri(Path.GetDirectoryName(Assembly.GetEntryAssembly().CodeBase));
                string appDirectory = u.LocalPath;
                recentFiles = new List<string>(File.ReadAllLines(Path.Combine(appDirectory, rfFilename)));
            }
            catch
            {
                recentFiles = new List<string>();
            }
            RebuildRecentFiles();
        }

        private void SaveRecentFiles()
        {
            try
            {
                Uri u = new Uri(Path.GetDirectoryName(Assembly.GetEntryAssembly().CodeBase));
                string appDirectory = u.LocalPath;

                FileInfo fi = new FileInfo(Path.Combine(appDirectory, rfFilename));
                if (fi.Exists)
                    fi.Attributes &= ~FileAttributes.Hidden;
                File.WriteAllLines(fi.FullName, recentFiles.ToArray());
                fi.Attributes = FileAttributes.Hidden | FileAttributes.Archive;
            }
            catch 
            { 
            }
        }

        private void RebuildRecentFiles()
        {
            recentToolStripMenuItem.MenuCommands.Clear();
            if (recentFiles.Count > 0)
            {
                MenuCommand ttsm;
                foreach (string fn in recentFiles)
                {
                    ttsm = new MenuCommand(fn);
                    ttsm.Click += new EventHandler(ttsm_Click);
                    recentToolStripMenuItem.MenuCommands.Add(ttsm);
                }

                recentToolStripMenuItem.MenuCommands.Add(new MenuCommand("-"));
                ttsm = new MenuCommand("Clear recent files list");
                recentToolStripMenuItem.MenuCommands.Add(ttsm);
                ttsm.Click += new EventHandler(ttsmClear_Click);


                recentToolStripMenuItem.Text = "Recent files";
                recentToolStripMenuItem.Enabled = true;
            }
            else
            {
                recentToolStripMenuItem.Text = "No recent files";
                recentToolStripMenuItem.Enabled = false;
            }
        }

        private void ttsm_Click(object sender, EventArgs e)
        {
            MenuCommand ttsm = sender as MenuCommand;

            lastSelectedFilename = ttsm.Text;

            // Try to load... 
            if (this.OnRecentFileSelected != null)
                this.OnRecentFileSelected(this, EventArgs.Empty);

            InsertFileToRecentList(ttsm.Text);
            RebuildRecentFiles();
            SaveRecentFiles();
        }

        private void ttsmClear_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Clear recent files list?",
                                "Recent files",
                                MessageBoxButtons.OKCancel,
                                MessageBoxIcon.Question) == DialogResult.OK)
            {
                recentFiles.Clear();
                RebuildRecentFiles();
                SaveRecentFiles();
            }
        }

        public void InsertFileToRecentList(string fn)
        {
            if (recentFiles == null)
                this.InitializeRecentFiles();

            if (recentFiles.Contains(fn))
            {
                recentFiles.Remove(fn);
            }
            else if (recentFiles.Count > maxNumOfFiles)
            {
                recentFiles.RemoveAt(recentFiles.Count);
            }
            recentFiles.Insert(0, fn);
            RebuildRecentFiles();
            SaveRecentFiles();
        }

        public string GetSelectedRecentFile()
        {
            return lastSelectedFilename;
        }
    }
}
