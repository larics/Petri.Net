using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Net;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;

namespace PetriNetSimulator2
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class MainWindow : System.Windows.Forms.Form
	{
		private string sApplicationName = "Petri .NET Simulator";
		private int iNextUntitledDocumentIndex = 1;
		private PetriNetDocument pndSelectedDocument = null;
		private ArrayList alDocuments = new ArrayList();
		private RulesEditor reRulesEditor = new RulesEditor();
        private PyEditor    rePythonEditor = new PyEditor();
        private PyOutput rePythonOutput = new PyOutput();
		private Simulator sSimulator = null;
		private ProgressBar pbSimulationProgress;
        private RecentFilesManager rfm = null;

		protected Crownwood.Magic.Docking.DockingManager dmDockingManager = null;

		private Crownwood.Magic.Menus.MenuControl mcMenuControl = null;
		private Crownwood.Magic.Menus.MenuCommand mcFile = null;
		private Crownwood.Magic.Menus.MenuCommand mcFileNew = null;
		private Crownwood.Magic.Menus.MenuCommand mcFileOpen = null;
		private Crownwood.Magic.Menus.MenuCommand mcFileClose = null;
		private Crownwood.Magic.Menus.MenuCommand mcFileSeparator1 = null;
		private Crownwood.Magic.Menus.MenuCommand mcFileSave = null;
		private Crownwood.Magic.Menus.MenuCommand mcFileSaveAs = null;
        private Crownwood.Magic.Menus.MenuCommand mcFileSaveXML = null;
        private Crownwood.Magic.Menus.MenuCommand mcFileLoadXML = null;
        private Crownwood.Magic.Menus.MenuCommand mcFileSaveMatrix = null;
        private Crownwood.Magic.Menus.MenuCommand mcFileSaveCpp = null;
		private Crownwood.Magic.Menus.MenuCommand mcFileSeparator2 = null;
		private Crownwood.Magic.Menus.MenuCommand mcFileExport = null;
		private Crownwood.Magic.Menus.MenuCommand mcFileSeparator3 = null;
        private Crownwood.Magic.Menus.MenuCommand mcFileRecentFiles = null;
        private Crownwood.Magic.Menus.MenuCommand mcFileSeparator4 = null;
		private Crownwood.Magic.Menus.MenuCommand mcFileExit = null;


		private Crownwood.Magic.Menus.MenuCommand mcEdit = null;
		private Crownwood.Magic.Menus.MenuCommand mcEditUndo = null;
		private Crownwood.Magic.Menus.MenuCommand mcEditRedo = null;
		private Crownwood.Magic.Menus.MenuCommand mcEditSeparator1 = null;
		private Crownwood.Magic.Menus.MenuCommand mcEditCut = null;
		private Crownwood.Magic.Menus.MenuCommand mcEditCopy = null;
		private Crownwood.Magic.Menus.MenuCommand mcEditPaste = null;
		private Crownwood.Magic.Menus.MenuCommand mcEditDelete = null;
		private Crownwood.Magic.Menus.MenuCommand mcEditSeparator2 = null;
		private Crownwood.Magic.Menus.MenuCommand mcEditCopyModel = null;
		private Crownwood.Magic.Menus.MenuCommand mcEditSeparator3 = null;
		private Crownwood.Magic.Menus.MenuCommand mcEditGroup = null;
		private Crownwood.Magic.Menus.MenuCommand mcEditSeparator4 = null;
		private Crownwood.Magic.Menus.MenuCommand mcEditSelectAll = null;

        private Crownwood.Magic.Menus.MenuCommand mcSimulate = null;
        private Crownwood.Magic.Menus.MenuCommand mcSimulateStart = null;
        private Crownwood.Magic.Menus.MenuCommand mcSimulateStartWithParam = null;
        private Crownwood.Magic.Menus.MenuCommand mcSimulateStep = null;
        private Crownwood.Magic.Menus.MenuCommand mcSimulateStop = null;
        private Crownwood.Magic.Menus.MenuCommand mcSimulateReset = null;

		private Crownwood.Magic.Menus.MenuCommand mcView = null;
		private Crownwood.Magic.Menus.MenuCommand mcHelp = null;
		private Crownwood.Magic.Menus.MenuCommand mcHelpManual = null;
		private Crownwood.Magic.Menus.MenuCommand mcHelpSeparator1 = null;
		private Crownwood.Magic.Menus.MenuCommand mcHelpCheckForUpdates = null;
		private Crownwood.Magic.Menus.MenuCommand mcHelpAbout = null;

		private ToolBar tbToolBar = new ToolBar();
		private ToolBarButton tbbNew = new ToolBarButton();
		private ToolBarButton tbbOpen = new ToolBarButton();
		private ToolBarButton tbbSave = new ToolBarButton();
		private ToolBarButton tbbSeparator1 = new ToolBarButton();
		private ToolBarButton tbbReset = new ToolBarButton();
		private ToolBarButton tbbStart = new ToolBarButton();
		private ToolBarButton tbbPause = new ToolBarButton();
		private ToolBarButton tbbStop = new ToolBarButton();
		private ToolBarButton tbbStep = new ToolBarButton();
		private ToolBarButton tbbSeparator2 = new ToolBarButton();
		private ToolBarButton tbbConflicts = new ToolBarButton();
		private ToolBarButton tbbCircularWaits = new ToolBarButton();
		private ToolBarButton tbbFireable = new ToolBarButton();
		private ToolBarButton tbbFired = new ToolBarButton();
		private ToolBarButton tbbSeparator3 = new ToolBarButton();
		private ToolBarButton tbbZoom = new ToolBarButton();
		private ToolBarButton tbbSeparator4 = new ToolBarButton();
		private ToolBarButton tbbUndo = new ToolBarButton();
		private ToolBarButton tbbRedo = new ToolBarButton();

		private ContextMenu cmUndo = new ContextMenu();
		private ContextMenu cmRedo = new ContextMenu();

		private PropertiesInspector piPropertiesInspector = new PropertiesInspector();
		private ToolBox tbToolBox = new ToolBox();
		private DocumentExplorer deDocumentExplorer = new DocumentExplorer();

		private CDiese.Actions.ActionList alMenuActionList = new CDiese.Actions.ActionList();
		private CDiese.Actions.Action aCloseFile = new CDiese.Actions.Action();
		private CDiese.Actions.Action aSaveFile = new CDiese.Actions.Action();
		private CDiese.Actions.Action aSaveAsFile = new CDiese.Actions.Action();
        private CDiese.Actions.Action aSaveXML = new CDiese.Actions.Action();
        private CDiese.Actions.Action aLoadXML = new CDiese.Actions.Action();
        private CDiese.Actions.Action aSaveMatrix = new CDiese.Actions.Action();
        private CDiese.Actions.Action aSaveCpp = new CDiese.Actions.Action();
		private CDiese.Actions.Action aEditUndo = new CDiese.Actions.Action();
		private CDiese.Actions.Action aEditRedo = new CDiese.Actions.Action();
		private CDiese.Actions.Action aEditDelete = new CDiese.Actions.Action();
		private CDiese.Actions.Action aEditCut = new CDiese.Actions.Action();
		private CDiese.Actions.Action aEditCopy = new CDiese.Actions.Action();
		private CDiese.Actions.Action aEditPaste = new CDiese.Actions.Action();
		private CDiese.Actions.Action aEditGroup = new CDiese.Actions.Action();
		private CDiese.Actions.Action aEditSelectAll = new CDiese.Actions.Action();

#if DEMO
		private System.Windows.Forms.Timer tmrDemoTimer = new System.Windows.Forms.Timer();
		private int iTimeout = 900;
#endif

		private System.Windows.Forms.StatusBar sbStatusBar;
		private Crownwood.Magic.Controls.TabControl tabMagicTab;
        private Crownwood.Magic.Docking.Content pythoneditor;
        private Crownwood.Magic.Docking.Content pythoneoutput;
		private System.Windows.Forms.ImageList ilToolBar;
		private System.Windows.Forms.ImageList ilTabImages;
		private System.Windows.Forms.Panel pnlDocumentPanel;
		private System.Windows.Forms.ImageList ilMenu;
		private System.Windows.Forms.ContextMenu cmZoom;
		private System.Windows.Forms.MenuItem miZoom100;
		private System.Windows.Forms.MenuItem miZoom90;
		private System.Windows.Forms.MenuItem miZoom70;
		private System.Windows.Forms.MenuItem miZoom60;
		private System.Windows.Forms.MenuItem miZoom50;
		private System.Windows.Forms.MenuItem miZoom80;
        private System.Windows.Forms.SaveFileDialog sfdSaveXMLFile;
        private System.Windows.Forms.OpenFileDialog sfdLoadXMLFile;
        private System.Windows.Forms.SaveFileDialog sfdSaveMatrixFile;
        private System.Windows.Forms.SaveFileDialog sfdSaveCppFile;
		private System.Windows.Forms.SaveFileDialog sfdSaveFile;
		private System.Windows.Forms.OpenFileDialog ofdOpenFile;
		private System.Windows.Forms.StatusBarPanel sbpPanelMain;
		private System.Windows.Forms.StatusBarPanel sbpSimulationTimePanel;
		private System.Windows.Forms.SaveFileDialog sfdExportFile;
		private System.ComponentModel.IContainer components;

        private Boolean loadCronwoodFail;   // Handle problems.. 

		#region private PetriNetDocument SelectedDocument
		private PetriNetDocument SelectedDocument
		{
			get
			{
				return this.pndSelectedDocument;
			}
			set
			{
				this.pndSelectedDocument = value;

				this.MainWindow_SetCaption();
			}
		}
		#endregion

		#region private ArrayList Documents
		private ArrayList Documents
		{
			get
			{
				return this.alDocuments;
			}
		}
		#endregion

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
                if (this.sSimulator != null)
                {
                    this.sSimulator.Dispose();
                }

				if (components != null)
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            this.ilToolBar = new System.Windows.Forms.ImageList(this.components);
            this.sbStatusBar = new System.Windows.Forms.StatusBar();
            this.sbpPanelMain = new System.Windows.Forms.StatusBarPanel();
            this.sbpSimulationTimePanel = new System.Windows.Forms.StatusBarPanel();
            this.tabMagicTab = new Crownwood.Magic.Controls.TabControl();
            this.ilTabImages = new System.Windows.Forms.ImageList(this.components);
            this.pnlDocumentPanel = new System.Windows.Forms.Panel();
            this.ilMenu = new System.Windows.Forms.ImageList(this.components);
            this.aCloseFile = new CDiese.Actions.Action(this.components);
            this.cmZoom = new System.Windows.Forms.ContextMenu();
            this.miZoom100 = new System.Windows.Forms.MenuItem();
            this.miZoom90 = new System.Windows.Forms.MenuItem();
            this.miZoom80 = new System.Windows.Forms.MenuItem();
            this.miZoom70 = new System.Windows.Forms.MenuItem();
            this.miZoom60 = new System.Windows.Forms.MenuItem();
            this.miZoom50 = new System.Windows.Forms.MenuItem();
            this.sfdSaveFile = new System.Windows.Forms.SaveFileDialog();
            this.sfdSaveXMLFile = new System.Windows.Forms.SaveFileDialog();
            this.sfdLoadXMLFile = new System.Windows.Forms.OpenFileDialog();
            this.sfdSaveMatrixFile = new System.Windows.Forms.SaveFileDialog();
            this.sfdSaveCppFile = new System.Windows.Forms.SaveFileDialog();
            this.ofdOpenFile = new System.Windows.Forms.OpenFileDialog();
            this.sfdExportFile = new System.Windows.Forms.SaveFileDialog();
            ((System.ComponentModel.ISupportInitialize)(this.sbpPanelMain)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.sbpSimulationTimePanel)).BeginInit();
            this.pnlDocumentPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // ilToolBar
            // 
            this.ilToolBar.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilToolBar.ImageStream")));
            this.ilToolBar.TransparentColor = System.Drawing.Color.Transparent;
            this.ilToolBar.Images.SetKeyName(0, "");
            this.ilToolBar.Images.SetKeyName(1, "");
            this.ilToolBar.Images.SetKeyName(2, "");
            this.ilToolBar.Images.SetKeyName(3, "");
            this.ilToolBar.Images.SetKeyName(4, "");
            this.ilToolBar.Images.SetKeyName(5, "");
            this.ilToolBar.Images.SetKeyName(6, "");
            this.ilToolBar.Images.SetKeyName(7, "");
            this.ilToolBar.Images.SetKeyName(8, "");
            this.ilToolBar.Images.SetKeyName(9, "");
            this.ilToolBar.Images.SetKeyName(10, "");
            this.ilToolBar.Images.SetKeyName(11, "");
            this.ilToolBar.Images.SetKeyName(12, "");
            this.ilToolBar.Images.SetKeyName(13, "");
            this.ilToolBar.Images.SetKeyName(14, "");
            // 
            // sbStatusBar
            // 
            this.sbStatusBar.Location = new System.Drawing.Point(0, 450);
            this.sbStatusBar.Name = "sbStatusBar";
            this.sbStatusBar.Panels.AddRange(new System.Windows.Forms.StatusBarPanel[] {
            this.sbpPanelMain,
            this.sbpSimulationTimePanel});
            this.sbStatusBar.ShowPanels = true;
            this.sbStatusBar.Size = new System.Drawing.Size(876, 21);
            this.sbStatusBar.TabIndex = 2;
            this.sbStatusBar.Text = "statusBar";
            // 
            // sbpPanelMain
            // 
            this.sbpPanelMain.Name = "sbpPanelMain";
            this.sbpPanelMain.Text = "Ready";
            // 
            // sbpSimulationTimePanel
            // 
            this.sbpSimulationTimePanel.Name = "sbpSimulationTimePanel";
            this.sbpSimulationTimePanel.Text = "SimulationTimePanel";
            // 
            // tabMagicTab
            // 
            this.tabMagicTab.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabMagicTab.HideTabsMode = Crownwood.Magic.Controls.TabControl.HideTabsModes.ShowAlways;
            this.tabMagicTab.Location = new System.Drawing.Point(0, 0);
            this.tabMagicTab.Name = "tabMagicTab";
            this.tabMagicTab.Size = new System.Drawing.Size(872, 446);
            this.tabMagicTab.TabIndex = 3;
            this.tabMagicTab.Visible = false;
            this.tabMagicTab.SelectionChanged += new System.EventHandler(this.tabMagicTab_SelectionChanged);
            this.tabMagicTab.ClosePressed += new System.EventHandler(this.tabMagicTab_ClosePressed);
            // 
            // ilTabImages
            // 
            this.ilTabImages.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilTabImages.ImageStream")));
            this.ilTabImages.TransparentColor = System.Drawing.Color.Transparent;
            this.ilTabImages.Images.SetKeyName(0, "");
            // 
            // pnlDocumentPanel
            // 
            this.pnlDocumentPanel.BackColor = System.Drawing.SystemColors.ControlDark;
            this.pnlDocumentPanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pnlDocumentPanel.Controls.Add(this.tabMagicTab);
            this.pnlDocumentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlDocumentPanel.Location = new System.Drawing.Point(0, 0);
            this.pnlDocumentPanel.Name = "pnlDocumentPanel";
            this.pnlDocumentPanel.Size = new System.Drawing.Size(876, 450);
            this.pnlDocumentPanel.TabIndex = 5;
            // 
            // ilMenu
            // 
            this.ilMenu.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilMenu.ImageStream")));
            this.ilMenu.TransparentColor = System.Drawing.Color.Transparent;
            this.ilMenu.Images.SetKeyName(0, "");
            this.ilMenu.Images.SetKeyName(1, "");
            this.ilMenu.Images.SetKeyName(2, "");
            this.ilMenu.Images.SetKeyName(3, "");
            this.ilMenu.Images.SetKeyName(4, "");
            this.ilMenu.Images.SetKeyName(5, "");
            this.ilMenu.Images.SetKeyName(6, "");
            this.ilMenu.Images.SetKeyName(7, "");
            this.ilMenu.Images.SetKeyName(8, "");
            this.ilMenu.Images.SetKeyName(9, "");
            this.ilMenu.Images.SetKeyName(10, "");
            this.ilMenu.Images.SetKeyName(11, "");
            this.ilMenu.Images.SetKeyName(12, "");
            this.ilMenu.Images.SetKeyName(13, "");
            this.ilMenu.Images.SetKeyName(14, "");
            this.ilMenu.Images.SetKeyName(15, "python-o.png");
            // 
            // aCloseFile
            // 
            this.aCloseFile.Checked = false;
            this.aCloseFile.Enabled = true;
            this.aCloseFile.Hint = null;
            this.aCloseFile.Shortcut = System.Windows.Forms.Shortcut.None;
            this.aCloseFile.Tag = null;
            this.aCloseFile.Text = null;
            this.aCloseFile.Visible = true;
            // 
            // cmZoom
            // 
            this.cmZoom.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.miZoom100,
            this.miZoom90,
            this.miZoom80,
            this.miZoom70,
            this.miZoom60,
            this.miZoom50});
            this.cmZoom.Popup += new System.EventHandler(this.cmZoom_Popup);
            // 
            // miZoom100
            // 
            this.miZoom100.Checked = true;
            this.miZoom100.Index = 0;
            this.miZoom100.RadioCheck = true;
            this.miZoom100.Text = "100%";
            this.miZoom100.Click += new System.EventHandler(this.miZoom_Click);
            // 
            // miZoom90
            // 
            this.miZoom90.Index = 1;
            this.miZoom90.RadioCheck = true;
            this.miZoom90.Text = "90%";
            this.miZoom90.Click += new System.EventHandler(this.miZoom_Click);
            // 
            // miZoom80
            // 
            this.miZoom80.Index = 2;
            this.miZoom80.RadioCheck = true;
            this.miZoom80.Text = "80%";
            this.miZoom80.Click += new System.EventHandler(this.miZoom_Click);
            // 
            // miZoom70
            // 
            this.miZoom70.Index = 3;
            this.miZoom70.RadioCheck = true;
            this.miZoom70.Text = "70%";
            this.miZoom70.Click += new System.EventHandler(this.miZoom_Click);
            // 
            // miZoom60
            // 
            this.miZoom60.Index = 4;
            this.miZoom60.RadioCheck = true;
            this.miZoom60.Text = "60%";
            this.miZoom60.Click += new System.EventHandler(this.miZoom_Click);
            // 
            // miZoom50
            // 
            this.miZoom50.Index = 5;
            this.miZoom50.RadioCheck = true;
            this.miZoom50.Text = "50%";
            this.miZoom50.Click += new System.EventHandler(this.miZoom_Click);
            // 
            // sfdSaveFile
            // 
            this.sfdSaveFile.DefaultExt = "pnd";
            this.sfdSaveFile.Filter = "Petri .NET Document (*.pnd)|*.pnd";
            this.sfdSaveFile.Title = "Save PetriNET to file.";
            // 
            // sfdSaveXMLFile
            // 
            this.sfdSaveXMLFile.DefaultExt = "pnml";
            this.sfdSaveXMLFile.Filter = "Petri .NET XML Document (*.pnml)|*.pnml";
            this.sfdSaveXMLFile.Title = "Save PetriNET to XML file.";
            // 
            // sfdLoadXMLFile
            // 
            this.sfdLoadXMLFile.DefaultExt = "pnml";
            this.sfdLoadXMLFile.Filter = "Petri .NET XML Document (*.pnml)|*.pnml";
            this.sfdLoadXMLFile.Title = "Load PetriNET from XML file.";
            // 
            // sfdSaveMatrixFile
            // 
            this.sfdSaveMatrixFile.DefaultExt = "m";
            this.sfdSaveMatrixFile.Filter = "Matlab file (*.m)|*.m";
            this.sfdSaveMatrixFile.Title = "Save PetriNET to M file.";
            // 
            // sfdSaveCppFile
            // 
            this.sfdSaveCppFile.DefaultExt = "cpp";
            this.sfdSaveCppFile.Filter = "C++ file (*.cpp)|*.cpp";
            this.sfdSaveCppFile.Title = "Save PetriNET to C/CPP file.";
            // 
            // ofdOpenFile
            // 
            this.ofdOpenFile.DefaultExt = "pnd";
            this.ofdOpenFile.Filter = "Petri .NET Document (*.pnd)|*.pnd";
            this.ofdOpenFile.Title = "Open PetriNET from file.";
            // 
            // sfdExportFile
            // 
            this.sfdExportFile.DefaultExt = "emf";
            this.sfdExportFile.Filter = "Enhanced metafile (*.emf)|*.emf";
            this.sfdExportFile.Title = "Export Petri .NET model.";
            // 
            // MainWindow
            // 
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(876, 471);
            this.Controls.Add(this.pnlDocumentPanel);
            this.Controls.Add(this.sbStatusBar);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.MainWindow_Load);
            this.Closing += new System.ComponentModel.CancelEventHandler(this.MainWindow_Closing);
            ((System.ComponentModel.ISupportInitialize)(this.sbpPanelMain)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.sbpSimulationTimePanel)).EndInit();
            this.pnlDocumentPanel.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
            try
            {
                Application.Run(new MainWindow());
            }
            catch (Exception e)
            {
                String poruka = "Message:\n";
                poruka += e.Message;
                poruka += "\n-------------------------------------------------------\nSource:\n";
                poruka += e.Source;
                poruka += "\n-------------------------------------------------------\nStack Trace:\n";
                poruka += e.StackTrace;

                Exception ei = e.InnerException;
                if (ei != null)
                {
                    poruka += "\n======================================================\nInner exception:\n";
                    poruka += ei.Message;
                }
                MessageBox.Show(poruka, "Exception");
            }
		}

		#region public MainWindow()
		public MainWindow()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			// Create the object that manages the docking state
			this.dmDockingManager = new Crownwood.Magic.Docking.DockingManager(this, Crownwood.Magic.Common.VisualStyle.IDE);
			this.dmDockingManager.InnerControl = this.pnlDocumentPanel;
			this.dmDockingManager.OuterControl = sbStatusBar;

			Crownwood.Magic.Docking.Content toolbox = dmDockingManager.Contents.Add(this.tbToolBox, "Toolbox", this.ilMenu, 1);
			toolbox.AutoHideSize = new Size(200, 300);
			toolbox.DisplaySize = new Size(200, 300);
			toolbox.FloatingSize = new Size(200, 300);
			Crownwood.Magic.Docking.Content properties = dmDockingManager.Contents.Add(this.piPropertiesInspector, "Properties", this.ilMenu, 0);
			properties.AutoHideSize = new Size(200, 300);
			properties.DisplaySize = new Size(200, 300);
			properties.FloatingSize = new Size(200, 300);
			Crownwood.Magic.Docking.WindowContent wc = dmDockingManager.AddContentWithState(toolbox, Crownwood.Magic.Docking.State.DockLeft);

			Crownwood.Magic.Docking.Content documentExplorer = dmDockingManager.Contents.Add(this.deDocumentExplorer, "Document Explorer", this.ilMenu, 13);
			properties.AutoHideSize = new Size(200, 300);
			properties.DisplaySize = new Size(200, 300);
			properties.FloatingSize = new Size(200, 300);

			dmDockingManager.AddContentToWindowContent(documentExplorer, wc);

			// Add a new WindowContent to the existing Zone already created
			this.dmDockingManager.AddContentToZone(properties, wc.ParentZone, 1);

            Crownwood.Magic.Docking.Content ruleseditor = dmDockingManager.Contents.Add(this.reRulesEditor, "Rules Editor", this.ilMenu, 10);
			ruleseditor.AutoHideSize = new Size(200, 180);
			ruleseditor.DisplaySize = new Size(200, 180);
			ruleseditor.FloatingSize = new Size(200, 180);
			wc = dmDockingManager.AddContentWithState(ruleseditor, Crownwood.Magic.Docking.State.DockBottom);

			// PY
            pythoneditor = dmDockingManager.Contents.Add(this.rePythonEditor, "Python (or C#) code editor", this.ilMenu, 14);
            pythoneditor.AutoHideSize = new Size(200, 180);
            pythoneditor.DisplaySize = new Size(200, 180);
            pythoneditor.FloatingSize = new Size(200, 180);
            dmDockingManager.AddContentWithState(pythoneditor, Crownwood.Magic.Docking.State.DockBottom);


            pythoneoutput = dmDockingManager.Contents.Add(this.rePythonOutput, "Python (or C#) output", this.ilMenu, 15);
            pythoneoutput.AutoHideSize = new Size(200, 180);
            pythoneoutput.DisplaySize = new Size(200, 180);
            pythoneoutput.FloatingSize = new Size(200, 180);
            dmDockingManager.AddContentWithState(pythoneoutput, Crownwood.Magic.Docking.State.DockBottom);


            // PY


			#region Initialize ToolBar control
			this.tbToolBar.Dock = DockStyle.Top;
			this.tbToolBar.ButtonSize = new Size(24, 24);
			this.tbToolBar.ImageList = ilToolBar;
			this.tbToolBar.ButtonClick += new ToolBarButtonClickEventHandler(tbToolBar_ButtonClick);
			this.tbToolBar.Appearance = ToolBarAppearance.Flat;

			this.tbbNew.ImageIndex = 0;
			this.tbToolBar.Buttons.Add(tbbNew);

			this.tbbOpen.ImageIndex = 1;
			this.tbToolBar.Buttons.Add(tbbOpen);

			this.tbbSave.ImageIndex = 2;
			this.tbToolBar.Buttons.Add(tbbSave);

			this.tbbSeparator1.Style = ToolBarButtonStyle.Separator;
			this.tbToolBar.Buttons.Add(tbbSeparator1);

			this.tbbReset.Enabled = false;
			this.tbbReset.ImageIndex = 3;
			this.tbToolBar.Buttons.Add(tbbReset);

			this.tbbStart.ImageIndex = 4;
			this.tbToolBar.Buttons.Add(tbbStart);

			this.tbbPause.Enabled = false;
			this.tbbPause.ImageIndex = 5;
			this.tbToolBar.Buttons.Add(tbbPause);

			this.tbbStop.Enabled = false;
			this.tbbStop.ImageIndex = 6;
			this.tbToolBar.Buttons.Add(tbbStop);

			this.tbbStep.ImageIndex = 7;
			this.tbToolBar.Buttons.Add(tbbStep);

			this.tbbSeparator4.Style = ToolBarButtonStyle.Separator;
			this.tbToolBar.Buttons.Add(tbbSeparator4);

			this.tbbUndo.ImageIndex = 13;
			this.tbbUndo.ToolTipText = "Undo";
			this.tbbUndo.Style = ToolBarButtonStyle.DropDownButton;
			this.tbbUndo.DropDownMenu = this.cmUndo;
			this.tbToolBar.Buttons.Add(tbbUndo);

			this.tbbRedo.ImageIndex = 14;
			this.tbbRedo.ToolTipText = "Redo";
			this.tbbRedo.Style = ToolBarButtonStyle.DropDownButton;
			this.tbbRedo.DropDownMenu = this.cmRedo;
			this.tbToolBar.Buttons.Add(tbbRedo);

			this.tbbSeparator2.Style = ToolBarButtonStyle.Separator;
			this.tbToolBar.Buttons.Add(tbbSeparator2);

			this.tbbConflicts.ImageIndex = 8;
			this.tbbConflicts.ToolTipText = "Show transitions in possible conflict.";
			this.tbbConflicts.Style = ToolBarButtonStyle.ToggleButton;
			this.tbToolBar.Buttons.Add(tbbConflicts);

			this.tbbCircularWaits.ImageIndex = 9;
			this.tbbCircularWaits.ToolTipText = "Show Resources in circular waits.";
			this.tbbCircularWaits.Style = ToolBarButtonStyle.ToggleButton;
			this.tbToolBar.Buttons.Add(tbbCircularWaits);

			this.tbbFireable.ImageIndex = 10;
			this.tbbFireable.ToolTipText = "Show fireable transitions.";
			this.tbbFireable.Style = ToolBarButtonStyle.ToggleButton;
			this.tbbFireable.Pushed = true;
			this.tbToolBar.Buttons.Add(tbbFireable);

			this.tbbFired.ImageIndex = 11;
			this.tbbFired.ToolTipText = "Show fired transitions.";
			this.tbbFired.Style = ToolBarButtonStyle.ToggleButton;
			this.tbbFired.Pushed = true;
			this.tbToolBar.Buttons.Add(tbbFired);

			this.tbbSeparator3.Style = ToolBarButtonStyle.Separator;
			this.tbToolBar.Buttons.Add(tbbSeparator3);

			this.tbbZoom.ImageIndex = 12;
			this.tbbZoom.Style = ToolBarButtonStyle.DropDownButton;
			this.tbbZoom.DropDownMenu = this.cmZoom;
			this.tbToolBar.Buttons.Add(tbbZoom);
			this.Controls.Add(this.tbToolBar);
			#endregion

			// Initialize MenuControl
			this.mcMenuControl = new Crownwood.Magic.Menus.MenuControl();
			this.mcMenuControl.Dock = DockStyle.Top;
			this.Controls.Add(mcMenuControl);

			#region Initialize this.mcFile

			mcFile = new Crownwood.Magic.Menus.MenuCommand("&File");

			mcFileNew = new Crownwood.Magic.Menus.MenuCommand("&New");
			mcFileNew.Shortcut = Shortcut.CtrlN;
			mcFileNew.Click += new EventHandler(mcFileNew_Click);
			mcFile.MenuCommands.Add(mcFileNew);

			mcFileOpen = new Crownwood.Magic.Menus.MenuCommand("&Open...");
			mcFileOpen.Shortcut = Shortcut.CtrlO;
			mcFileOpen.Click += new EventHandler(mcFileOpen_Click);
			mcFileOpen.ImageList = ilMenu;
			mcFileOpen.ImageIndex = 2;
			mcFile.MenuCommands.Add(mcFileOpen);

			mcFileClose = new Crownwood.Magic.Menus.MenuCommand("&Close");
			mcFileClose.Click += new EventHandler(mcFileClose_Click);
			mcFile.MenuCommands.Add(mcFileClose);

			mcFileSeparator1 = new Crownwood.Magic.Menus.MenuCommand("-");
			mcFile.MenuCommands.Add(mcFileSeparator1);

			mcFileSave = new Crownwood.Magic.Menus.MenuCommand("&Save");
			mcFileSave.Click += new EventHandler(mcFileSave_Click);
			mcFileSave.Shortcut = Shortcut.CtrlS;
			mcFileSave.ImageList = ilMenu;
			mcFileSave.ImageIndex = 3;
			mcFile.MenuCommands.Add(mcFileSave);

			mcFileSaveAs = new Crownwood.Magic.Menus.MenuCommand("Save &As...");
			mcFileSaveAs.Click += new EventHandler(mcFileSaveAs_Click);
			mcFile.MenuCommands.Add(mcFileSaveAs);

			mcFileSeparator2 = new Crownwood.Magic.Menus.MenuCommand("-");
			mcFile.MenuCommands.Add(mcFileSeparator2);

            mcFileSaveXML = new Crownwood.Magic.Menus.MenuCommand("Export PN model to &PNML file...");
                        mcFileSaveXML.Click += new EventHandler(mcFileSaveXML_Click);
                        mcFile.MenuCommands.Add(mcFileSaveXML);

                        mcFileLoadXML = new Crownwood.Magic.Menus.MenuCommand("Import PN model from &PNML file...");
                        mcFileLoadXML.Click += new EventHandler(mcFileLoadXML_Click);
                        mcFile.MenuCommands.Add(mcFileLoadXML);

                        mcFileSaveMatrix = new Crownwood.Magic.Menus.MenuCommand("Export matrix model to &Matlab M file...");
                        mcFileSaveMatrix.Click += new EventHandler(mcFileSaveMatrix_Click);
                        mcFile.MenuCommands.Add(mcFileSaveMatrix);


                        mcFileSaveCpp = new Crownwood.Magic.Menus.MenuCommand("Export matrix model to C/C++...");
                        mcFileSaveCpp.Click += new EventHandler(mcFileSaveCpp_Click);
                        mcFile.MenuCommands.Add(mcFileSaveCpp);

			mcFileExport = new Crownwood.Magic.Menus.MenuCommand("&Export to metafile...");
			mcFileExport.Shortcut = Shortcut.CtrlE;
			mcFileExport.Click += new EventHandler(mcFileExport_Click);
			mcFile.MenuCommands.Add(mcFileExport);

			mcFileSeparator3 = new Crownwood.Magic.Menus.MenuCommand("-");
			mcFile.MenuCommands.Add(mcFileSeparator3);


            mcFileRecentFiles = new Crownwood.Magic.Menus.MenuCommand("Recent files");
            mcFile.MenuCommands.Add(mcFileRecentFiles);

            mcFileSeparator4 = new Crownwood.Magic.Menus.MenuCommand("-");
            mcFile.MenuCommands.Add(mcFileSeparator4);

			mcFileExit = new Crownwood.Magic.Menus.MenuCommand("E&xit");
			mcFileExit.Click += new EventHandler(mcFileExit_Click);
			mcFile.MenuCommands.Add(mcFileExit);

			this.mcMenuControl.MenuCommands.Add(mcFile);

			#endregion

			#region Initialize this.mcEdit

			mcEdit = new Crownwood.Magic.Menus.MenuCommand("&Edit");

			mcEditUndo = new Crownwood.Magic.Menus.MenuCommand("&Undo");
			mcEditUndo.Click += new EventHandler(mcEditUndo_Click);
			mcEditUndo.ImageList = ilMenu;
			mcEditUndo.ImageIndex = 4;
			mcEditUndo.Shortcut = Shortcut.CtrlZ;
			mcEditUndo.Enabled = false;
			mcEdit.MenuCommands.Add(mcEditUndo);

			mcEditRedo = new Crownwood.Magic.Menus.MenuCommand("&Redo");
			mcEditRedo.Click += new EventHandler(mcEditRedo_Click);
			mcEditRedo.ImageList = ilMenu;
			mcEditRedo.ImageIndex = 5;
			mcEditRedo.Shortcut = Shortcut.CtrlY;
			mcEditRedo.Enabled = false;
			mcEdit.MenuCommands.Add(mcEditRedo);

			mcEditSeparator1 = new Crownwood.Magic.Menus.MenuCommand("-");
			mcEdit.MenuCommands.Add(mcEditSeparator1);

			mcEditCut = new Crownwood.Magic.Menus.MenuCommand("Cu&t");
			mcEditCut.Click += new EventHandler(mcEditCut_Click);
			mcEditCut.ImageList = ilMenu;
			mcEditCut.ImageIndex = 6;
			mcEditCut.Shortcut = Shortcut.CtrlX;
			mcEdit.MenuCommands.Add(mcEditCut);

			mcEditCopy = new Crownwood.Magic.Menus.MenuCommand("&Copy");
			mcEditCopy.Click += new EventHandler(mcEditCopy_Click);
			mcEditCopy.ImageList = ilMenu;
			mcEditCopy.ImageIndex = 7;
			mcEditCopy.Shortcut = Shortcut.CtrlC;
			mcEdit.MenuCommands.Add(mcEditCopy);

			mcEditPaste = new Crownwood.Magic.Menus.MenuCommand("&Paste");
			mcEditPaste.Click += new EventHandler(mcEditPaste_Click);
			mcEditPaste.ImageList = ilMenu;
			mcEditPaste.ImageIndex = 8;
			mcEditPaste.Shortcut = Shortcut.CtrlV;
			mcEdit.MenuCommands.Add(mcEditPaste);

			mcEditDelete = new Crownwood.Magic.Menus.MenuCommand("&Delete");
			mcEditDelete.Click += new EventHandler(mcEditDelete_Click);
			mcEditDelete.ImageList = ilMenu;
			mcEditDelete.ImageIndex = 9;
			mcEditDelete.Shortcut = Shortcut.Del;
			mcEdit.MenuCommands.Add(mcEditDelete);

			mcEditSeparator2 = new Crownwood.Magic.Menus.MenuCommand("-");
			mcEdit.MenuCommands.Add(mcEditSeparator2);

			mcEditCopyModel = new Crownwood.Magic.Menus.MenuCommand("Copy &model");
			mcEditCopyModel.Click += new EventHandler(mcEditCopyModel_Click);
			mcEditCopyModel.ImageList = ilMenu;
			//mcEditCopyModel.ImageIndex = 9;
			//mcEditCopyModel.Shortcut = Shortcut.Del;
			mcEdit.MenuCommands.Add(mcEditCopyModel);

			mcEditSeparator3 = new Crownwood.Magic.Menus.MenuCommand("-");
			mcEdit.MenuCommands.Add(mcEditSeparator3);

			mcEditGroup = new Crownwood.Magic.Menus.MenuCommand("&Group");
			mcEditGroup.Click += new EventHandler(mcEditGroup_Click);
			mcEditGroup.ImageList = ilMenu;
			//mcEditGroup.ImageIndex = 8;
			mcEditGroup.Shortcut = Shortcut.CtrlG;
			mcEdit.MenuCommands.Add(mcEditGroup);

			mcEditSeparator4 = new Crownwood.Magic.Menus.MenuCommand("-");
			mcEdit.MenuCommands.Add(mcEditSeparator4);

			mcEditSelectAll = new Crownwood.Magic.Menus.MenuCommand("Select &All");
			mcEditSelectAll.Click += new EventHandler(mcEditSelectAll_Click);
			mcEditSelectAll.ImageList = ilMenu;
			mcEditSelectAll.Shortcut = Shortcut.CtrlA;
			mcEdit.MenuCommands.Add(mcEditSelectAll);

			this.mcMenuControl.MenuCommands.Add(mcEdit);

			#endregion

            #region Initialize this.mcSimulate

            mcSimulate = new Crownwood.Magic.Menus.MenuCommand("&Simulate");

            mcSimulateStart = new Crownwood.Magic.Menus.MenuCommand("Start");
            mcSimulateStart.Click += new EventHandler(mcSimulateStart_Click);
            mcSimulateStart.ImageList = ilToolBar;
            mcSimulateStart.ImageIndex = 4;
            mcSimulateStart.Shortcut = Shortcut.F5;
            mcSimulateStart.Enabled = false;
            mcSimulate.MenuCommands.Add(mcSimulateStart);

            mcSimulateStartWithParam = new Crownwood.Magic.Menus.MenuCommand("Start with param");
            mcSimulateStartWithParam.Click += new EventHandler(mcSimulateStartWithParam_Click);
            mcSimulateStartWithParam.ImageList = null;
            mcSimulateStartWithParam.ImageIndex = 0;
            mcSimulateStartWithParam.Shortcut = Shortcut.CtrlF5;
            mcSimulateStartWithParam.Enabled = false;
            mcSimulate.MenuCommands.Add(mcSimulateStartWithParam);

            mcSimulateStep = new Crownwood.Magic.Menus.MenuCommand("Step");
            mcSimulateStep.Click += new EventHandler(mcSimulateStep_Click);
            mcSimulateStep.ImageList = ilToolBar;
            mcSimulateStep.ImageIndex = 7;
            mcSimulateStep.Shortcut = Shortcut.F8;
            mcSimulateStep.Enabled = false;
            mcSimulate.MenuCommands.Add(mcSimulateStep);


            mcSimulateStop = new Crownwood.Magic.Menus.MenuCommand("Stop");
            mcSimulateStop.Click += new EventHandler(mcSimulateStop_Click);
            mcSimulateStop.ImageList = ilToolBar;
            mcSimulateStop.ImageIndex = 6;
            mcSimulateStop.Shortcut = Shortcut.ShiftF5;
            mcSimulateStop.Enabled = false;
            mcSimulate.MenuCommands.Add(mcSimulateStop);

            mcSimulateReset = new Crownwood.Magic.Menus.MenuCommand("Reset");
            mcSimulateReset.Click += new EventHandler(mcSimulateReset_Click);
            mcSimulateReset.ImageList = ilToolBar;
            mcSimulateReset.ImageIndex = 3;
            mcSimulateReset.Shortcut = Shortcut.CtrlShiftF5;
            mcSimulateReset.Enabled = false;
            mcSimulate.MenuCommands.Add(mcSimulateReset);


            this.mcMenuControl.MenuCommands.Add(mcSimulate);
            #endregion

            #region Initialize this.mcView

            mcView = new Crownwood.Magic.Menus.MenuCommand("&View");

			foreach(Crownwood.Magic.Docking.Content c in this.dmDockingManager.Contents)
			{
				Crownwood.Magic.Menus.MenuCommand mc = new Crownwood.Magic.Menus.MenuCommand(c.Title);
				mc.Image = c.ImageList.Images[c.ImageIndex];
				mc.Click += new EventHandler(mcViewContents_Click);
				mcView.MenuCommands.Add(mc);
			}

			this.mcMenuControl.MenuCommands.Add(mcView);

			#endregion

			#region Initialize this.mcHelp

			mcHelp = new Crownwood.Magic.Menus.MenuCommand("&Help");

			mcHelpManual = new Crownwood.Magic.Menus.MenuCommand("&Manual");
			mcHelpManual.Click += new EventHandler(mcHelpManual_Click);
			mcHelpManual.Shortcut = Shortcut.F1;
			mcHelpManual.ImageList = ilMenu;
			mcHelpManual.ImageIndex = 11;
			mcHelp.MenuCommands.Add(mcHelpManual);

			mcHelpSeparator1 = new Crownwood.Magic.Menus.MenuCommand("-");
			mcHelp.MenuCommands.Add(mcHelpSeparator1);

			mcHelpCheckForUpdates = new Crownwood.Magic.Menus.MenuCommand("&Check for updates...");
			mcHelpCheckForUpdates.Click += new EventHandler(mcHelpCheckForUpdates_Click);
			mcHelp.MenuCommands.Add(mcHelpCheckForUpdates);

			mcHelpAbout = new Crownwood.Magic.Menus.MenuCommand("&About...");
			mcHelpAbout.Click += new EventHandler(mcHelpAbout_Click);
			mcHelp.MenuCommands.Add(mcHelpAbout);

			this.mcMenuControl.MenuCommands.Add(mcHelp);

			#endregion


			#region Initialize ActionList
			// Initialize ActionList
			this.alMenuActionList.ImageList = this.ilMenu;

			this.aCloseFile.Checked = false;
			this.aCloseFile.Enabled = true;
			this.aCloseFile.Hint = null;
			this.aCloseFile.Shortcut = System.Windows.Forms.Shortcut.None;
			this.aCloseFile.Tag = null;
			this.aCloseFile.Text = mcFileClose.Text;
			this.aCloseFile.Visible = true;
			this.aCloseFile.Update += new System.EventHandler(this.aCloseFile_Update);
			this.alMenuActionList.Actions.Add(this.aCloseFile);
			this.alMenuActionList.SetAction(this.mcFileClose, this.aCloseFile);

			this.aSaveFile.Checked = false;
			this.aSaveFile.Enabled = true;
			this.aSaveFile.Hint = null;
			this.aSaveFile.Shortcut = mcFileSave.Shortcut;
			this.aSaveFile.Tag = null;
			this.aSaveFile.Text = mcFileSave.Text;
			this.aSaveFile.Visible = true;
			this.aSaveFile.Update += new System.EventHandler(this.aSaveFile_Update);
			this.alMenuActionList.Actions.Add(this.aSaveFile);
			this.alMenuActionList.SetAction(this.mcFileSave, this.aSaveFile);

			this.aSaveAsFile.Checked = false;
			this.aSaveAsFile.Enabled = true;
			this.aSaveAsFile.Hint = null;
			this.aSaveAsFile.Shortcut = mcFileSaveAs.Shortcut;
			this.aSaveAsFile.Tag = null;
			this.aSaveAsFile.Text = mcFileSaveAs.Text;
			this.aSaveAsFile.Visible = true;
			this.aSaveAsFile.Update += new System.EventHandler(this.aSaveAsFile_Update);
			this.alMenuActionList.Actions.Add(this.aSaveAsFile);
			this.alMenuActionList.SetAction(this.mcFileSaveAs, this.aSaveAsFile);


                        this.aSaveXML.Checked = false;
                        this.aSaveXML.Enabled = true;
                        this.aSaveXML.Hint = null;
                        this.aSaveXML.Shortcut = mcFileSaveXML.Shortcut;
                        this.aSaveXML.Tag = null;
                        this.aSaveXML.Text = mcFileSaveXML.Text;
                        this.aSaveXML.Visible = true;
                        this.aSaveXML.Update += new System.EventHandler(this.aSaveXML_Update);
                        this.alMenuActionList.Actions.Add(this.aSaveXML);
                        this.alMenuActionList.SetAction(this.mcFileSaveXML, this.aSaveXML);


                        this.aSaveMatrix.Checked = false;
                        this.aSaveMatrix.Enabled = true;
                        this.aSaveMatrix.Hint = null;
                        this.aSaveMatrix.Shortcut = mcFileSaveMatrix.Shortcut;
                        this.aSaveMatrix.Tag = null;
                        this.aSaveMatrix.Text = mcFileSaveMatrix.Text;
                        this.aSaveMatrix.Visible = true;
                        this.aSaveMatrix.Update += new System.EventHandler(this.aSaveXML_Update);
                        this.alMenuActionList.Actions.Add(this.aSaveMatrix);
                        this.alMenuActionList.SetAction(this.mcFileSaveMatrix, this.aSaveMatrix);


                        this.aLoadXML.Checked = false;
                        this.aLoadXML.Enabled = true;
                        this.aLoadXML.Hint = null;
                        this.aLoadXML.Shortcut = mcFileLoadXML.Shortcut;
                        this.aLoadXML.Tag = null;
                        this.aLoadXML.Text = mcFileLoadXML.Text;
                        this.aLoadXML.Visible = true;
                        this.aLoadXML.Update += new System.EventHandler(this.aLoadXML_Update);
                        this.alMenuActionList.Actions.Add(this.aLoadXML);
                        this.alMenuActionList.SetAction(this.mcFileLoadXML, this.aLoadXML);

                        this.aSaveCpp.Checked = false;
                        this.aSaveCpp.Enabled = true;
                        this.aSaveCpp.Hint = null;
                        this.aSaveCpp.Shortcut = mcFileSaveCpp.Shortcut;
                        this.aSaveCpp.Tag = null;
                        this.aSaveCpp.Text = mcFileSaveCpp.Text;
                        this.aSaveCpp.Visible = true;
                        this.aSaveCpp.Update += new System.EventHandler(this.aSaveXML_Update);
                        this.alMenuActionList.Actions.Add(this.aSaveCpp);
                        this.alMenuActionList.SetAction(this.mcFileSaveCpp, this.aSaveCpp);


			this.aEditUndo.Checked = false;
			this.aEditUndo.Enabled = false;
			this.aEditUndo.Hint = null;
			this.aEditUndo.Shortcut = System.Windows.Forms.Shortcut.CtrlZ;
			this.aEditUndo.Tag = null;
			this.aEditUndo.ImageIndex = 4;
			this.aEditUndo.Text = mcEditUndo.Text;
			this.aEditUndo.Visible = true;
			this.aEditUndo.Update += new System.EventHandler(this.aEditUndo_Update);
			this.alMenuActionList.Actions.Add(this.aEditUndo);
			this.alMenuActionList.SetAction(this.mcEditUndo, this.aEditUndo);

			this.aEditRedo.Checked = false;
			this.aEditRedo.Enabled = false;
			this.aEditRedo.Hint = null;
			this.aEditRedo.Shortcut = System.Windows.Forms.Shortcut.CtrlY;
			this.aEditRedo.Tag = null;
			this.aEditRedo.ImageIndex = 5;
			this.aEditRedo.Text = mcEditRedo.Text;
			this.aEditRedo.Visible = true;
			this.aEditRedo.Update += new System.EventHandler(this.aEditRedo_Update);
			this.alMenuActionList.Actions.Add(this.aEditRedo);
			this.alMenuActionList.SetAction(this.mcEditRedo, this.aEditRedo);

			this.aEditDelete.Checked = false;
			this.aEditDelete.Enabled = false;
			this.aEditDelete.Hint = null;
			this.aEditDelete.Shortcut = System.Windows.Forms.Shortcut.Del;
			this.aEditDelete.Tag = null;
			this.aEditDelete.ImageIndex = 9;
			this.aEditDelete.Text = mcEditDelete.Text;
			this.aEditDelete.Visible = true;
			this.aEditDelete.Update += new System.EventHandler(this.aEditDelete_Update);
			this.alMenuActionList.Actions.Add(this.aEditDelete);
			this.alMenuActionList.SetAction(this.mcEditDelete, this.aEditDelete);

			this.aEditCut.Checked = false;
			this.aEditCut.Enabled = false;
			this.aEditCut.Hint = null;
			this.aEditCut.Shortcut = System.Windows.Forms.Shortcut.CtrlX;
			this.aEditCut.ImageIndex = 6;
			this.aEditCut.Tag = null;
			this.aEditCut.Text = mcEditCut.Text;
			this.aEditCut.Visible = true;
			this.aEditCut.Update += new System.EventHandler(this.aEditCut_Update);
			this.alMenuActionList.Actions.Add(this.aEditCut);
			this.alMenuActionList.SetAction(this.mcEditCut, this.aEditCut);

			this.aEditCopy.Checked = false;
			this.aEditCopy.Enabled = false;
			this.aEditCopy.Hint = null;
			this.aEditCopy.Shortcut = System.Windows.Forms.Shortcut.CtrlC;
			this.aEditCopy.ImageIndex = 7;
			this.aEditCopy.Tag = null;
			this.aEditCopy.Text = mcEditCopy.Text;
			this.aEditCopy.Visible = true;
			this.aEditCopy.Update += new System.EventHandler(this.aEditCopy_Update);
			this.alMenuActionList.Actions.Add(this.aEditCopy);
			this.alMenuActionList.SetAction(this.mcEditCopy, this.aEditCopy);

			this.aEditPaste.Checked = false;
			this.aEditPaste.Enabled = false;
			this.aEditPaste.Hint = null;
			this.aEditPaste.Shortcut = System.Windows.Forms.Shortcut.CtrlV;
			this.aEditPaste.ImageIndex = 8;
			this.aEditPaste.Tag = null;
			this.aEditPaste.Text = mcEditPaste.Text;
			this.aEditPaste.Visible = true;
			this.aEditPaste.Update += new System.EventHandler(this.aEditPaste_Update);
			this.alMenuActionList.Actions.Add(this.aEditPaste);
			this.alMenuActionList.SetAction(this.mcEditPaste, this.aEditPaste);

			this.aEditGroup.Checked = false;
			this.aEditGroup.Enabled = false;
			this.aEditGroup.Hint = null;
			this.aEditGroup.Shortcut = System.Windows.Forms.Shortcut.CtrlG;
			this.aEditGroup.ImageIndex = 12;
			this.aEditGroup.Tag = null;
			this.aEditGroup.Text = mcEditGroup.Text;
			this.aEditGroup.Visible = true;
			this.aEditGroup.Update += new System.EventHandler(this.aEditGroup_Update);
			this.alMenuActionList.Actions.Add(this.aEditGroup);
			this.alMenuActionList.SetAction(this.mcEditGroup, this.aEditGroup);

			this.aEditSelectAll.Checked = false;
			this.aEditSelectAll.Enabled = false;
			this.aEditSelectAll.Hint = null;
			this.aEditSelectAll.Shortcut = this.mcEditSelectAll.Shortcut;
			this.aEditSelectAll.Tag = null;
			this.aEditSelectAll.Text = mcEditSelectAll.Text;
			this.aEditSelectAll.Visible = true;
			this.aEditSelectAll.Update += new System.EventHandler(this.aEditSelectAll_Update);
			this.alMenuActionList.Actions.Add(this.aEditSelectAll);
			this.alMenuActionList.SetAction(this.mcEditSelectAll, this.aEditSelectAll);

			#endregion

			// Initialize Undo/Redo context menus
			this.cmUndo.Popup += new EventHandler(cmUndo_Popup);
			this.cmRedo.Popup += new EventHandler(cmRedo_Popup);

			// Initialize tabMagicTab
			this.tabMagicTab.IDEPixelBorder = true;
			this.tabMagicTab.PositionTop = true;
			this.tabMagicTab.BoldSelectedPage = true;
			this.tabMagicTab.Appearance = Crownwood.Magic.Controls.TabControl.VisualAppearance.MultiDocument;
			this.tabMagicTab.ShowArrows = true;
			this.tabMagicTab.ShowClose = true;
			this.tabMagicTab.ImageList = this.ilTabImages;

			// Initialize piPropertiesInspector
			this.piPropertiesInspector.SelectedObjectChanged += new EventHandler(piPropertiesInspector_SelectedObjectChanged);

			// To set MainWindow.Text
			this.SelectedDocument = null;

			// Create ProgressBar
			this.pbSimulationProgress = new ProgressBar();
			this.pbSimulationProgress.Hide();
#if DEMO
			// Initialize tmrDemoTimer
			this.tmrDemoTimer.Interval = 1000;
			this.tmrDemoTimer.Tick += new EventHandler(tmrDemoTimer_Tick);
			this.tmrDemoTimer.Start();
#endif
            loadCronwoodFail = true;
            try
            {
                // Load layout
                if (File.Exists(Application.StartupPath + "\\layout.config.xml"))
                    this.dmDockingManager.LoadConfigFromFile(Application.StartupPath + "\\layout.config.xml");
            }
            catch
            {
                loadCronwoodFail = false;
            }
		}

		#endregion

		#region private void MainWindow_Load(object sender, System.EventArgs e)
		private void MainWindow_Load(object sender, System.EventArgs e)
		{
            // Initialize recent files list...
            this.rfm = new RecentFilesManager(this, mcFileRecentFiles, 10);
            this.rfm.OnRecentFileSelected += new EventHandler(rfm_OnRecentFileSelected);

			// Initialize StatusBar
			sbpPanelMain.Width = this.Width - 200;
			sbpSimulationTimePanel.Width = 200;

			#region Initialize pbSimulationProgress ProgressBar
			// Initialize pbSimulationProgress ProgressBar

			this.sbpSimulationTimePanel.MinWidth = 100;
			this.sbpSimulationTimePanel.Text = "0";
			this.sbpSimulationTimePanel.ToolTipText = "Simulation time [s]";

			int iWidth = 0;
			foreach(StatusBarPanel sbp in sbStatusBar.Panels)
			{
				iWidth += sbp.Width;
			}
			pbSimulationProgress.Width = sbpSimulationTimePanel.Width - 60;
			pbSimulationProgress.Height = sbStatusBar.Height - 6;
			pbSimulationProgress.Location = new Point(iWidth - sbpSimulationTimePanel.Width + 50, 4);
			sbStatusBar.Controls.Add(pbSimulationProgress);
			#endregion

			// If there are any command-line arguments, load that file
			string[] sa = Environment.GetCommandLineArgs();
			if (sa.Length > 1)
			{
				this.OpenFile(sa[1]);
			}

#if DEMO
			MessageBox.Show("This is DEMO version of Petri .NET Simulator 2.0.\n\nDEMO version doesn't have implemented Save and Export functions and it will    \nautomatically close after expiring DEMO time in status bar!\n\nBuy a full version which has no limits. Full version can be purchased at\nBIGENERIC website (http://petrinet.bigeneric.com)", "Petri .NET Simulator 2.0 - DEMO WARNING", MessageBoxButtons.OK, MessageBoxIcon.Warning);
#endif
		}
		#endregion

		#region private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (this.sSimulator != null)
			{
				if (this.sSimulator.Running == true)
					this.sSimulator.Stop();
			}

			// Save layout
			this.dmDockingManager.SaveConfigToFile(Application.StartupPath + "\\layout.config.xml");

			Application.Exit();
		}
		#endregion

		#region private void MainWindow_SetCaption()
		private void MainWindow_SetCaption()
		{
			// Set MainWindow.Text
			string sBuild = " - ";
#if DEMO
			sBuild += "DEMO ";
#elif STUDENT
            sBuild += "STUDENT ";
#else
			sBuild += "FULL VERSION ";
#endif

			if (this.pndSelectedDocument != null)
			{
				sBuild += "- ";
				this.Text = this.sApplicationName + " " + Application.ProductVersion + sBuild + pndSelectedDocument.FileName;
			}
			else
			{
				this.Text = this.sApplicationName + " " + Application.ProductVersion + sBuild;
			}
		}
		#endregion


		#region *MainMenu

		#region private void mcFileNew_Click(object sender, System.EventArgs e)
		private void mcFileNew_Click(object sender, System.EventArgs e)
		{
			// if tabMagicTab is not visible, show it
			if (this.tabMagicTab.Visible != true)
			{
				this.pnlDocumentPanel.BorderStyle = BorderStyle.None;
				this.tabMagicTab.Visible = true;
			}

			PetriNetDocument pnd = new PetriNetDocument(this, "Untitled" + this.iNextUntitledDocumentIndex.ToString());
			pnd.PropertiesChanged += new EventHandler(pnd_PropertiesChanged);
			pnd.ContentsChanged += new EventHandler(pnd_ContentsChanged);
			pnd.SelectedObjectsChanged += new EventHandler(pnd_SelectedObjectsChanged);
			pnd.PropertiesInspectorChangeRequested += new EventHandler(pnd_PropertiesInspectorChangeRequested);

			pnd.Dock = DockStyle.Fill;
			this.Documents.Add(pnd);

			// Add new TabPage
			Crownwood.Magic.Controls.TabPage tp = new Crownwood.Magic.Controls.TabPage("Untitled" + this.iNextUntitledDocumentIndex.ToString() + "  &");
			tp.ImageIndex = 0;
			tp.Controls.Add(pnd);
			tabMagicTab.TabPages.Add(tp);

			this.iNextUntitledDocumentIndex++;

			tabMagicTab.SelectedIndex = tabMagicTab.TabPages.Count - 1;
			pnd.PerformActivation();

            mcSimulateStartWithParam.Enabled = mcSimulateStart.Enabled = tbbStart.Enabled = true;
            mcSimulateStep.Enabled = tbbStep.Enabled = true;
		}
		#endregion

		#region private void mcFileOpen_Click(object sender, System.EventArgs e)
		private void mcFileOpen_Click(object sender, System.EventArgs e)
		{
			ofdOpenFile.FileName = "";

			if (DialogResult.OK == ofdOpenFile.ShowDialog())
			{
				this.OpenFile(ofdOpenFile.FileName);

                mcSimulateStartWithParam.Enabled = mcSimulateStart.Enabled = tbbStart.Enabled = true;
                mcSimulateStep.Enabled = tbbStep.Enabled = true;
			}
		}
		#endregion

		#region private void OpenFile(string sFileName)
		private void OpenFile(string sFileName)
		{
			// if tabMagicTab is not visible, show it
			if (this.tabMagicTab.Visible != true)
			{
				this.pnlDocumentPanel.BorderStyle = BorderStyle.None;
				this.tabMagicTab.Visible = true;
			}

			PetriNetDocument pnd = new PetriNetDocument(this, "Untitled" + this.iNextUntitledDocumentIndex.ToString());
			pnd.PropertiesChanged += new EventHandler(pnd_PropertiesChanged);
			pnd.ContentsChanged += new EventHandler(pnd_ContentsChanged);
			pnd.SelectedObjectsChanged += new EventHandler(pnd_SelectedObjectsChanged);
			pnd.PropertiesInspectorChangeRequested += new EventHandler(pnd_PropertiesInspectorChangeRequested);

			pnd.Dock = DockStyle.Fill;
			this.Documents.Add(pnd);

			pnd.OpenFile(sFileName);

			// Add new TabPage
			string sStripedFileName = Path.GetFileNameWithoutExtension(pnd.FileName);
			Crownwood.Magic.Controls.TabPage tp = new Crownwood.Magic.Controls.TabPage(sStripedFileName + "  &");
			tp.ImageIndex = 0;
			tp.Controls.Add(pnd);
			tp.Title = sStripedFileName + "  &";
			tabMagicTab.TabPages.Add(tp);

			this.MainWindow_SetCaption();

			tabMagicTab.SelectedIndex = tabMagicTab.TabPages.Count - 1;
			pnd.PerformActivation();

            rfm.InsertFileToRecentList(sFileName);
		}
		#endregion

		#region private void mcFileClose_Click(object sender, System.EventArgs e)
		private void mcFileClose_Click(object sender, System.EventArgs e)
		{
			tabMagicTab_ClosePressed(sender, EventArgs.Empty);
		}
		#endregion

		#region private void mcFileSave_Click(object sender, System.EventArgs e)
		private void mcFileSave_Click(object sender, System.EventArgs e)
		{
#if !DEMO
			if (this.SelectedDocument != null)
			{
				if (this.SelectedDocument.FileName.Remove(this.SelectedDocument.FileName.Length - 1, 1) == "Untitled")
				{
					mcFileSaveAs_Click(sender, e);
				}
				else
				{
					this.SelectedDocument.SaveFile(this.SelectedDocument.FileName);
				}
			}
#else
                        MessageBox.Show("This is a limited DEMO version. This feature is not available.", "Petri .NET Simulator 2.0 - Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
#endif

		}
		#endregion

        #region private void mcFileSaveXML_Click(object sender, System.EventArgs e)
        private void mcFileSaveXML_Click(object sender, System.EventArgs e)
        {
#if (!DEMO && !STUDENT)
	    	if (this.SelectedDocument != null)
    		{
                sfdSaveXMLFile.FileName = "";

                if (DialogResult.OK == sfdSaveXMLFile.ShowDialog())
                {
                    this.SelectedDocument.SaveFileXML(sfdSaveXMLFile.FileName);
                }
			}
#else
            MessageBox.Show("This is a limited DEMO version. This feature is not available.", "Petri .NET Simulator 2.0 - Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
#endif
		}
		#endregion


                #region private void mcFileLoadXML_Click(object sender, System.EventArgs e)
                private void mcFileLoadXML_Click(object sender, System.EventArgs e)
                {
#if (!DEMO && !STUDENT)
                        if (DialogResult.OK == sfdLoadXMLFile.ShowDialog())
                        {

                            // if tabMagicTab is not visible, show it
                            if (this.tabMagicTab.Visible != true)
                            {
                                this.pnlDocumentPanel.BorderStyle = BorderStyle.None;
                                this.tabMagicTab.Visible = true;
                            }

                            PetriNetDocument pnd = new PetriNetDocument(this, "Untitled" + this.iNextUntitledDocumentIndex.ToString());
                            pnd.PropertiesChanged += new EventHandler(pnd_PropertiesChanged);
                            pnd.ContentsChanged += new EventHandler(pnd_ContentsChanged);
                            pnd.SelectedObjectsChanged += new EventHandler(pnd_SelectedObjectsChanged);
                            pnd.PropertiesInspectorChangeRequested += new EventHandler(pnd_PropertiesInspectorChangeRequested);

                            pnd.Dock = DockStyle.Fill;
                            this.Documents.Add(pnd);
                            pnd.LoadFileXML(sfdLoadXMLFile.FileName);


                            // Add new TabPage
                            Crownwood.Magic.Controls.TabPage tp = new Crownwood.Magic.Controls.TabPage("Untitled" + this.iNextUntitledDocumentIndex.ToString() + "  &");
                            tp.ImageIndex = 0;
                            tp.Controls.Add(pnd);
                            tabMagicTab.TabPages.Add(tp);

                            this.iNextUntitledDocumentIndex++;

                            tabMagicTab.SelectedIndex = tabMagicTab.TabPages.Count - 1;
                            pnd.PerformActivation();

                            mcSimulateStartWithParam.Enabled = mcSimulateStart.Enabled = tbbStart.Enabled = true;
                            mcSimulateStep.Enabled = tbbStep.Enabled = true;
                        }
#else
#if (STUDENT)
                    MessageBox.Show("This is a limited STUDENT version. This feature is not available.", "Petri .NET Simulator 2.1 - Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
#else
                    MessageBox.Show("This is a limited DEMO version. This feature is not available.", "Petri .NET Simulator 2.1 - Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
#endif
#endif
                }
		#endregion




                #region private void mcFileSaveMatrix_Click(object sender, System.EventArgs e)
                private void mcFileSaveMatrix_Click(object sender, System.EventArgs e)
		{
#if (!DEMO && !STUDENT)
			if (this.SelectedDocument != null)
			{
                                sfdSaveMatrixFile.FileName = "";

                                if (DialogResult.OK == sfdSaveMatrixFile.ShowDialog())
                                {
                                        this.SelectedDocument.SaveFileMatrix(sfdSaveMatrixFile.FileName);
                                }
			}
#else
            MessageBox.Show("This is a limited DEMO version. This feature is not available.", "Petri .NET Simulator 2.0 - Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
#endif
		}
		#endregion



                #region private void mcFileSaveCpp_Click(object sender, System.EventArgs e)
                private void mcFileSaveCpp_Click(object sender, System.EventArgs e)
		{

#if (!DEMO && !STUDENT)
			if (this.SelectedDocument != null)
			{
                                sfdSaveCppFile.FileName = "";

                                if (DialogResult.OK == sfdSaveCppFile.ShowDialog())
                                {
                                        this.SelectedDocument.SaveFileCpp(sfdSaveCppFile.FileName);
                                }
			}
#else
            MessageBox.Show("This is a limited DEMO version. This feature is not available.", "Petri .NET Simulator 2.0 - Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
#endif
		}
		#endregion




		#region private void mcFileSaveAs_Click(object sender, System.EventArgs e)
		private void mcFileSaveAs_Click(object sender, System.EventArgs e)
		{
#if !DEMO
			sfdSaveFile.FileName = "";

			if (DialogResult.OK == sfdSaveFile.ShowDialog())
			{
				this.SelectedDocument.SaveFile(sfdSaveFile.FileName);
			}

			// Refresh MainWindow caption, and MagicTab tabs, to reflect current filename changes
			this.MainWindow_SetCaption();
			string sStripedFileName = Path.GetFileNameWithoutExtension(this.SelectedDocument.FileName);
			tabMagicTab.SelectedTab.Title = sStripedFileName + "  &";
#else
			MessageBox.Show("DEMO version doesn't have implemented Save function!\nBuy a full version which is capable of saving files.", "Petri .NET Simulator 2.0 - Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
#endif
		}
		#endregion

		#region private void mcFileExit_Click(object sender, System.EventArgs e)
		private void mcFileExit_Click(object sender, System.EventArgs e)
		{
			this.Close();
			Application.Exit();
		}
		#endregion

		#region private void mcFileExport_Click(object sender, System.EventArgs e)
		private void mcFileExport_Click(object sender, System.EventArgs e)
		{
#if !DEMO
			sfdExportFile.FileName = "";

			if (DialogResult.OK == sfdExportFile.ShowDialog())
			{
				FileStream fs = File.Create(sfdExportFile.FileName);
				MetafileExporter.SaveMetafile(fs, this.SelectedDocument.Editor);

				fs.Close();
			}
#else
			MessageBox.Show("DEMO version doesn't have implemented Export function!\nBuy a full version which is capable of exporting models.", "Petri .NET Simulator 2.0 - Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
#endif
		}
		#endregion


		#region private void mcEditUndo_Click(object sender, System.EventArgs e)
		private void mcEditUndo_Click(object sender, System.EventArgs e)
		{
			if (this.SelectedDocument != null)
			{
				UndoRedoItem uri = (UndoRedoItem)this.SelectedDocument.Editor.UndoList.Pop();
				this.SelectedDocument.Editor.RedoList.Push(uri);

				uri.Undo();
			}
		}
		#endregion

		#region private void mcEditRedo_Click(object sender, System.EventArgs e)
		private void mcEditRedo_Click(object sender, System.EventArgs e)
		{
			if (this.SelectedDocument != null)
			{
				UndoRedoItem uri = (UndoRedoItem)this.SelectedDocument.Editor.RedoList.Pop();
				this.SelectedDocument.Editor.UndoList.Push(uri);

				uri.Redo();
			}
		}
		#endregion

		#region private void mcEditCut_Click(object sender, System.EventArgs e)
		private void mcEditCut_Click(object sender, System.EventArgs e)
		{
			if (this.SelectedDocument != null)
			{
				Clipboard.Object = this.SelectedDocument.Editor.CopySelection();
				this.SelectedDocument.Editor.DeleteSelectedControls();
			}
		}
		#endregion

		#region private void mcEditCopy_Click(object sender, System.EventArgs e)
		private void mcEditCopy_Click(object sender, System.EventArgs e)
		{
			if (this.SelectedDocument != null)
				Clipboard.Object = this.SelectedDocument.Editor.CopySelection();
		}
		#endregion

		#region private void mcEditPaste_Click(object sender, System.EventArgs e)
		private void mcEditPaste_Click(object sender, System.EventArgs e)
		{
			if (this.SelectedDocument != null)
				this.SelectedDocument.Editor.Paste(Clipboard.Object);
		}
		#endregion

		#region private void mcEditDelete_Click(object sender, System.EventArgs e)
		private void mcEditDelete_Click(object sender, System.EventArgs e)
		{
			if (this.SelectedDocument != null)
			{
				if (this.reRulesEditor.Focused != true)
				{
					if (DialogResult.Yes == MessageBox.Show("Are you sure you want to delete selected items?", "Petri .NET Simulator 2.0 - Question", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question))
					{
						this.SelectedDocument.Editor.DeleteSelectedControls();
					}
				}
				else
				{
					this.reRulesEditor.DeleteSelectedRule();
				}
			}
		}
		#endregion

		#region private void mcEditCopyModel_Click(object sender, System.EventArgs e)
		private void mcEditCopyModel_Click(object sender, System.EventArgs e)
		{
#if !DEMO
			if (this.SelectedDocument != null)
			{
				MemoryStream ms = new MemoryStream();
				Metafile mf = MetafileExporter.SaveMetafile(ms, this.SelectedDocument.Editor);

				// Copy to clipboard
				ClipboardMetafileHelper.PutEnhMetafileOnClipboard(this.Handle, mf);
			}
#else
			MessageBox.Show("DEMO version doesn't have implemented Copy to Clipboard function!\nBuy a full version which is capable of Copying to Clipboard.", "Petri .NET Simulator 2.0 - Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
#endif
		}
		#endregion

		#region private void mcEditGroup_Click(object sender, System.EventArgs e)
		private void mcEditGroup_Click(object sender, System.EventArgs e)
		{
			if (this.SelectedDocument != null)
				this.SelectedDocument.Editor.Group();
		}
		#endregion

		#region private void mcEditSelectAll_Click(object sender, System.EventArgs e)
		private void mcEditSelectAll_Click(object sender, System.EventArgs e)
		{
			if (this.SelectedDocument != null)
				this.SelectedDocument.Editor.SelectAll();
		}
		#endregion

		#region private void mcViewContents_Click(object sender, EventArgs e)
		private void mcViewContents_Click(object sender, EventArgs e)
		{
			Crownwood.Magic.Menus.MenuCommand mc = (Crownwood.Magic.Menus.MenuCommand)sender;
			this.dmDockingManager.ShowContent(this.dmDockingManager.Contents[mc.Text]);
			this.dmDockingManager.BringAutoHideIntoView(this.dmDockingManager.Contents[mc.Text]);
		}
		#endregion

		#region private void mcHelpManual_Click(object sender, System.EventArgs e)
		private void mcHelpManual_Click(object sender, System.EventArgs e)
		{
			// API.ShellExecute(IntPtr.Zero, "Open", Application.StartupPath + "\\Manual.pdf", null, null, 3);
            API.ShellExecute(IntPtr.Zero, "Open", "https://github.com/larics/Petri.NET/wiki", null, null, 3);

		}
		#endregion

		#region private void mcHelpCheckForUpdates_Click(object sender, System.EventArgs e)
		private void mcHelpCheckForUpdates_Click(object sender, System.EventArgs e)
		{
            if (isInternetAvailable())
            {
                string sVersion = String.Empty;
                bool bIsNewAvailable = isNewVersionAvailable(ref sVersion);
                if (!bIsNewAvailable)
                    MessageBox.Show("You already have the latest version of Petri .NET Simulator!\nUpdate is not needed!", "Petri .NET Simulator 2.0 Information - Version up-to-date", MessageBoxButtons.OK, MessageBoxIcon.Information);
                else
                {
                    if (DialogResult.Yes == MessageBox.Show("There is new version available: " + sVersion + "  !\nDo you wish to go to Petri .NET Simulator web page and check for it ?", "Petri .NET Simulator 2.0 Information - new version available", MessageBoxButtons.YesNo, MessageBoxIcon.Information))
                        API.ShellExecute(IntPtr.Zero, "Open", "https://github.com/larics/Petri.Net/releases", null, null, 3);
                }
			}
            else
			{
				MessageBox.Show("There was an error trying to connect to the Internet!\nPlease connect to the Internet!", "Petri .NET Simulator 2.0 Error - No Internet connection", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}


        private bool isInternetAvailable()
        {
            try
            {
                using (WebClient client = new WebClient())
                {
                    client.UseDefaultCredentials = true;
                    client.Proxy = WebRequest.GetSystemWebProxy();
                    client.Proxy.Credentials = CredentialCache.DefaultNetworkCredentials;

                    using (Stream stream = client.OpenRead("http://github.com"))
                    {
                        return true;
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        private bool isNewVersionAvailable(ref string sVersion)
        {
            bool bIsNewAvailable = false;
            sVersion = String.Empty; ;

            try
            {
                this.Cursor = Cursors.WaitCursor;

                Version version = System.Reflection.Assembly.GetEntryAssembly().GetName().Version;
                DateTime buildDateTime = new DateTime(2000, 1, 1).Add(new TimeSpan(
                                                        TimeSpan.TicksPerDay * version.Build + // days since 1 January 2000
                                                        TimeSpan.TicksPerSecond * 2 * version.Revision));

                // Initialize the WebRequest.
                WebRequest myRequest = WebRequest.Create("https://github.com/larics/Petri.Net/releases");

                WebProxy proxyObject = WebProxy.GetDefaultProxy();
                proxyObject.Credentials = System.Net.CredentialCache.DefaultCredentials;
                myRequest.Proxy = proxyObject;

                // Return the response.
                WebResponse myResponse = myRequest.GetResponse();

                // Code to use the WebResponse goes here.
                Stream sContents = myResponse.GetResponseStream();
                StreamReader reader = new StreamReader(sContents);
                String sHtmlCode = reader.ReadToEnd();

                // Close the response to free resources.
                myResponse.Close();
                try
                {
                    // Find (Version:XXXXXXXXXXXXX)
                    string pattern = @"\(Version:([^)]*)\)";
                    Regex regex = new Regex(pattern);
                    Match match = regex.Match(sHtmlCode);
                    if (match.Success)
                        sVersion = match.Value.Replace("(Version:", "").Replace(")", "").Trim();

                    //determine latest version;
                    string[] sLatestVersion, sCurrentVersion;
                    sLatestVersion = sVersion.Split(new char[] { '.' });
                    sCurrentVersion = Application.ProductVersion.Split(new char[] { '.' });

                    //Check if there is newer version
                    if (int.Parse(sLatestVersion[0]) > int.Parse(sCurrentVersion[0]))
                        bIsNewAvailable = true;
                    else if (int.Parse(sLatestVersion[0]) == int.Parse(sCurrentVersion[0]))
                        if (int.Parse(sLatestVersion[1]) > int.Parse(sCurrentVersion[1]))
                            bIsNewAvailable = true;
                        else if (int.Parse(sLatestVersion[1]) == int.Parse(sCurrentVersion[1]))
                            if (int.Parse(sLatestVersion[2]) > int.Parse(sCurrentVersion[2]))
                                bIsNewAvailable = true;
                            else if (int.Parse(sLatestVersion[2]) == int.Parse(sCurrentVersion[2]))
                                if (int.Parse(sLatestVersion[3]) > int.Parse(sCurrentVersion[3]))
                                    bIsNewAvailable = true;
                }
                catch (Exception ex)
                {
                    bIsNewAvailable = false;
                }
            }
            catch (WebException)
            {
                bIsNewAvailable = false;
                this.Cursor = Cursors.Default;
                MessageBox.Show("There was an error trying to connect to the Internet!\nPlease connect to the Internet!", "Petri .NET Simulator 2.0 Error - No Internet connection", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            this.Cursor = Cursors.Default;
            return bIsNewAvailable;
        }


		#endregion

		#region private void mcHelpAbout_Click(object sender, System.EventArgs e)
		private void mcHelpAbout_Click(object sender, System.EventArgs e)
		{
			About21 a = new About21();
			a.ShowDialog();
		}
		#endregion



		private void mcSimulateStart_Click(object sender, System.EventArgs e)
        {
            ToolBarButtonClickEventArgs ee = new ToolBarButtonClickEventArgs (tbbStart);
            this.sSimulator.sleepBetweenStep = 1000;
            this.sSimulator.ignoreLackOfFireableTransition = false;
            tbToolBar_ButtonClick(sender, ee);
        }

        private void mcSimulateStartWithParam_Click(object sender, System.EventArgs e)
        {
            if (this.SelectedDocument != null)
            {
                if (this.SelectedDocument.PetriNetType == PetriNetType.TimeInvariant)
                { 
                    SimulateParamForm spf = new SimulateParamForm(this.sSimulator);
                    if (spf.ShowDialog() != DialogResult.OK)
                        return;
                }

                ToolBarButtonClickEventArgs ee = new ToolBarButtonClickEventArgs(tbbStart);
                tbToolBar_ButtonClick(sender, ee);
            }
        }

        private void mcSimulateStep_Click(object sender, System.EventArgs e)
        {
            ToolBarButtonClickEventArgs ee = new ToolBarButtonClickEventArgs(tbbStep);
            tbToolBar_ButtonClick(sender, ee);
        }

        private void mcSimulateStop_Click(object sender, System.EventArgs e)
        {
            ToolBarButtonClickEventArgs ee = new ToolBarButtonClickEventArgs(tbbStop);
            tbToolBar_ButtonClick(sender, ee);
        }

        private void mcSimulateReset_Click(object sender, System.EventArgs e)
        {
            ToolBarButtonClickEventArgs ee = new ToolBarButtonClickEventArgs(tbbReset);
            tbToolBar_ButtonClick(sender, ee);
        }

        #endregion

        #region private void cmZoom_Popup(object sender, System.EventArgs e)
        private void cmZoom_Popup(object sender, System.EventArgs e)
		{
			foreach(MenuItem mi in cmZoom.MenuItems)
			{
				string s = mi.Text;

				int i = int.Parse(s.Replace("%", ""));
				float f = i / 100f;

				if (this.SelectedDocument != null)
				{
					if (this.SelectedDocument.Editor.Zoom == f)
						mi.Checked = true;
					else
						mi.Checked = false;
				}
			}
		}
		#endregion

		#region private void miZoom_Click(object sender, System.EventArgs e)
		private void miZoom_Click(object sender, System.EventArgs e)
		{
			MenuItem mi = (MenuItem)sender;
			string s = mi.Text;

			int i = int.Parse(s.Replace("%", ""));
			float f = i / 100f;

			this.SelectedDocument.Editor.Zoom = f;
		}
		#endregion


		#region private void tabMagicTab_SelectionChanged(object sender, System.EventArgs e)
		private void tabMagicTab_SelectionChanged(object sender, System.EventArgs e)
		{
			if (this.tabMagicTab.TabPages.Count != 0)
			{
				this.SelectedDocument = (PetriNetDocument)this.Documents[this.tabMagicTab.SelectedIndex];

				if (sender is Crownwood.Magic.Controls.TabControl)
				{
					// Stop current simulation
					if (this.tbbStop.Enabled)
						this.tbToolBar_ButtonClick(tbbStop, new ToolBarButtonClickEventArgs(tbbStop));
					else if (this.tbbReset.Enabled)
						this.tbToolBar_ButtonClick(tbbReset, new ToolBarButtonClickEventArgs(tbbReset));

					this.sSimulator = new Simulator(this.SelectedDocument);
					this.sSimulator.SimulationFinished += new EventHandler(sSimulator_SimulationFinished);
					this.sSimulator.SimulationStepFinished += new EventHandler(sSimulator_SimulationStepFinished);
					this.sSimulator.SimulationProcessFinished += new EventHandler(sSimulator_SimulationProcessFinished);
				}

				if (this.SelectedDocument.View == DocumentView.Editor)
				{
					ArrayList al = new ArrayList();
					foreach(object o in this.SelectedDocument.Objects)
					{
						al.Add(o);
					}
					foreach(object o in this.SelectedDocument.Connections)
					{
						al.Add(o);
					}
					al.Add(this.SelectedDocument);

					// Because Place name was changed
					// Need to refresh PropertiesInspector combo box
					if (!(sender is TabPage))
					{
						// Remember previously selected object
						object o = this.piPropertiesInspector.SelectedObject;
						this.piPropertiesInspector.Objects = al;
						this.piPropertiesInspector.SelectedObject = o;
					}
					else
						this.piPropertiesInspector.Objects = al;

					if (sender is Crownwood.Magic.Controls.TabControl)
						this.SelectedDocument.PerformActivation();
				}
				else if (this.SelectedDocument.View == DocumentView.Description)
				{
					this.pnd_PropertiesInspectorChangeRequested(sender, e);
				}
				else if (this.SelectedDocument.View == DocumentView.Response)
				{
					this.pnd_PropertiesInspectorChangeRequested(sender, e);
				}
			}
			else
			{
				this.SelectedDocument = null;
				this.piPropertiesInspector.SelectedObject = null;
				this.piPropertiesInspector.Objects = null;
			}

			// Refresh RulesEditor
			this.reRulesEditor.Document = this.SelectedDocument;
			this.deDocumentExplorer.Document = this.SelectedDocument;
            this.rePythonEditor.Document = this.SelectedDocument;
            if(this.SelectedDocument  != null)
            {
               this.SelectedDocument.pyOutput = this.rePythonOutput;
               this.SelectedDocument.pyEditor = this.rePythonEditor;
            }
		}
		#endregion

		#region private void tabMagicTab_ClosePressed(object sender, System.EventArgs e)
		private void tabMagicTab_ClosePressed(object sender, System.EventArgs e)
		{
			if (tabMagicTab.SelectedTab != null)
			{
				this.Documents.Remove(this.SelectedDocument);
				tabMagicTab.TabPages.Remove(tabMagicTab.SelectedTab);

				if (tabMagicTab.TabPages.Count == 0)
				{
					this.tabMagicTab.Visible = false;
					this.pnlDocumentPanel.BorderStyle = BorderStyle.Fixed3D;
				}
			}
		}
		#endregion


		#region private void tbToolBar_ButtonClick(object sender, ToolBarButtonClickEventArgs e)
		private void tbToolBar_ButtonClick(object sender, ToolBarButtonClickEventArgs e)
		{
			if (e.Button == this.tbbNew)
			{
				mcFileNew_Click(sender, EventArgs.Empty);
			}
			else if (e.Button == this.tbbOpen)
			{
				mcFileOpen_Click(sender, EventArgs.Empty);
			}
			else if (e.Button == this.tbbSave)
			{
				mcFileSave_Click(sender, EventArgs.Empty);
			}
			else if (e.Button == tbbReset)
			{
				this.sSimulator.Reset();
				tbbReset.Enabled = false;

                mcSimulateStartWithParam.Enabled = mcSimulateStart.Enabled = tbbStart.Enabled = true;
				mcSimulateStop.Enabled = tbbStop.Enabled = false;
                mcSimulateStep.Enabled = tbbStep.Enabled = true;

				pbSimulationProgress.Hide();

				sbpSimulationTimePanel.Text = "0";
				pbSimulationProgress.Value = 0;
			}
			else if (e.Button == tbbStart)
			{
				if (this.SelectedDocument != null)
				{
					this.SelectedDocument.Editor.PauseBeforeFiring = false;
					this.sSimulator.Start();

					mcSimulateReset.Enabled = tbbReset.Enabled = false;
                    mcSimulateStartWithParam.Enabled = mcSimulateStart.Enabled = tbbStart.Enabled = false;

					tbbPause.Enabled = true;
                    mcSimulateStop.Enabled = tbbStop.Enabled = true;
                    mcSimulateStep.Enabled = tbbStep.Enabled = false;

					if (this.SelectedDocument.PetriNetType == PetriNetType.PTimed)
						pbSimulationProgress.Show();

					pbSimulationProgress.Maximum = (int)(this.SelectedDocument.EndTime / this.SelectedDocument.Td) + 2;
				}
			}
			else if (e.Button == tbbPause)
			{
                mcSimulateStartWithParam.Enabled = mcSimulateStart.Enabled = tbbStart.Enabled = true;
				tbbPause.Enabled = false;

                mcSimulateStep.Enabled = tbbStep.Enabled = true;
				this.sSimulator.Pause();
			}
			else if (e.Button == tbbStop)
			{
				this.sSimulator.Stop();

                mcSimulateStartWithParam.Enabled = mcSimulateStart.Enabled = tbbStart.Enabled = true;

                mcSimulateStop.Enabled = tbbStop.Enabled = false;
				tbbPause.Enabled = false;
                mcSimulateReset.Enabled = tbbReset.Enabled = false;
                mcSimulateStep.Enabled = tbbStep.Enabled = true;

				pbSimulationProgress.Hide();

				sbpSimulationTimePanel.Text = "0";
				pbSimulationProgress.Value = 0;
			}
			else if (e.Button == tbbStep)
			{
				if (this.SelectedDocument != null)
				{
					if (this.SelectedDocument.PetriNetType == PetriNetType.TimeInvariant)
					{
						this.sSimulator.Step();
						tbbReset.Enabled = true;
					}
					else if (this.SelectedDocument.PetriNetType == PetriNetType.PTimed)
					{
						this.SelectedDocument.Editor.PauseBeforeFiring = true;
						this.sSimulator.Start();

                        mcSimulateReset.Enabled = tbbReset.Enabled = false;

                        mcSimulateStartWithParam.Enabled = mcSimulateStart.Enabled = tbbStart.Enabled = false;
						tbbPause.Enabled = true;
                        mcSimulateStop.Enabled = tbbStop.Enabled = true;
                        mcSimulateStep.Enabled = tbbStep.Enabled = false;

						pbSimulationProgress.Show();

						pbSimulationProgress.Maximum = (int)(this.SelectedDocument.EndTime / this.SelectedDocument.Td) + 2;
					}
				}
			}
			else if (e.Button == this.tbbConflicts)
			{
				Transition.ShowConflicts = tbbConflicts.Pushed == true;

				foreach(Transition t in this.SelectedDocument.Transitions)
				{
					t.Refresh();
				}
			}
			else if (e.Button == this.tbbCircularWaits)
			{
				Place.ShowCircularWaitings = tbbCircularWaits.Pushed == true;

				foreach(Place p in this.SelectedDocument.ResourcePlaces)
				{
					p.Refresh();
				}
			}
			else if (e.Button == this.tbbFireable)
			{
				Transition.ShowFireableTransitions = tbbFireable.Pushed == true;

				foreach(Transition t in this.SelectedDocument.Transitions)
				{
					t.Refresh();
				}
			}
			else if (e.Button == this.tbbFired)
			{
				Transition.ShowFiredTransitions = tbbFired.Pushed == true;

				foreach(Transition t in this.SelectedDocument.Transitions)
				{
					t.Refresh();
				}
			}
			else if (e.Button == this.tbbUndo)
			{
				mcEditUndo_Click(sender, EventArgs.Empty);
			}
			else if (e.Button == this.tbbRedo)
			{
				mcEditRedo_Click(sender, EventArgs.Empty);
			}
		}
		#endregion


		#region *Actions

		#region private void aCloseFile_Update(object sender, System.EventArgs e)
		private void aCloseFile_Update(object sender, System.EventArgs e)
		{
			((CDiese.Actions.Action)sender).Enabled = this.Documents.Count != 0;
			tbToolBox.EnabledToolbox = this.Documents.Count != 0;

			if (this.Documents.Count == 0)
			{
                mcSimulateStartWithParam.Enabled = mcSimulateStart.Enabled = tbbStart.Enabled = false;
                mcSimulateStep.Enabled = tbbStep.Enabled = false;

                mcSimulateReset.Enabled = tbbReset.Enabled = false;
				tbbPause.Enabled = false;
                mcSimulateStop.Enabled = tbbStop.Enabled = false;
			}

			tbbFireable.Enabled = this.Documents.Count != 0;
			tbbFired.Enabled = this.Documents.Count != 0;
			tbbCircularWaits.Enabled = this.Documents.Count != 0;
			tbbConflicts.Enabled = this.Documents.Count != 0;
			tbbZoom.Enabled = this.Documents.Count != 0;

		}
		#endregion

		#region private void aSaveFile_Update(object sender, System.EventArgs e)
		private void aSaveFile_Update(object sender, System.EventArgs e)
		{
			((CDiese.Actions.Action)sender).Enabled = this.SelectedDocument != null;
			tbbSave.Enabled = this.SelectedDocument != null;
			mcFileExport.Enabled = this.SelectedDocument != null;
		}
		#endregion

		#region private void aSaveAsFile_Update(object sender, System.EventArgs e)
		private void aSaveAsFile_Update(object sender, System.EventArgs e)
		{
			((CDiese.Actions.Action)sender).Enabled = this.SelectedDocument != null;
		}
		#endregion

                #region private void aSaveXML_Update(object sender, System.EventArgs e)
                private void aSaveXML_Update(object sender, System.EventArgs e)
		{
			((CDiese.Actions.Action)sender).Enabled = this.SelectedDocument != null;
		}
		#endregion

        #region private void aLoadXML_Update(object sender, System.EventArgs e)
        private void aLoadXML_Update(object sender, System.EventArgs e)
        {
            ((CDiese.Actions.Action)sender).Enabled = true;
        }
        #endregion



		#region private void aEditUndo_Update(object sender, System.EventArgs e)
		private void aEditUndo_Update(object sender, System.EventArgs e)
		{
			if (this.SelectedDocument != null)
			{
				((CDiese.Actions.Action)sender).Enabled = this.SelectedDocument.Editor.UndoList.Count != 0;
				tbbUndo.Enabled = this.SelectedDocument.Editor.UndoList.Count != 0;
			}
			else
			{
				((CDiese.Actions.Action)sender).Enabled = false;
				tbbUndo.Enabled = false;
			}
		}
		#endregion

		#region private void aEditRedo_Update(object sender, System.EventArgs e)
		private void aEditRedo_Update(object sender, System.EventArgs e)
		{
			if (this.SelectedDocument != null)
			{
				((CDiese.Actions.Action)sender).Enabled = this.SelectedDocument.Editor.RedoList.Count != 0;
				tbbRedo.Enabled = this.SelectedDocument.Editor.RedoList.Count != 0;
			}
			else
			{
				((CDiese.Actions.Action)sender).Enabled = false;
				tbbRedo.Enabled = false;
			}
		}
		#endregion

		#region private void aEditDelete_Update(object sender, System.EventArgs e)
		private void aEditDelete_Update(object sender, System.EventArgs e)
		{
			if (this.SelectedDocument != null)
				((CDiese.Actions.Action)sender).Enabled = this.SelectedDocument.SelectedObjects.Count != 0;
			else
				((CDiese.Actions.Action)sender).Enabled = false;
		}
		#endregion

		#region private void aEditCut_Update(object sender, System.EventArgs e)
		private void aEditCut_Update(object sender, System.EventArgs e)
		{
			if (this.SelectedDocument != null)
				((CDiese.Actions.Action)sender).Enabled = this.SelectedDocument.SelectedObjects.Count != 0;
			else
				((CDiese.Actions.Action)sender).Enabled = false;
		}
		#endregion

		#region private void aEditCopy_Update(object sender, System.EventArgs e)
		private void aEditCopy_Update(object sender, System.EventArgs e)
		{
			if (this.SelectedDocument != null)
				((CDiese.Actions.Action)sender).Enabled = this.SelectedDocument.SelectedObjects.Count != 0;
			else
				((CDiese.Actions.Action)sender).Enabled = false;
		}
		#endregion

		#region private void aEditGroup_Update(object sender, System.EventArgs e)
		private void aEditGroup_Update(object sender, System.EventArgs e)
		{
			if (this.SelectedDocument != null)
				((CDiese.Actions.Action)sender).Enabled = this.SelectedDocument.SelectedObjects.Count != 0;
			else
				((CDiese.Actions.Action)sender).Enabled = false;
		}
		#endregion

		#region private void aEditPaste_Update(object sender, System.EventArgs e)
		private void aEditPaste_Update(object sender, System.EventArgs e)
		{
			if (this.SelectedDocument != null)
				((CDiese.Actions.Action)sender).Enabled = Clipboard.Object != null;
			else
				((CDiese.Actions.Action)sender).Enabled = false;
		}
		#endregion

		#region private void aEditSelectAll_Update(object sender, System.EventArgs e)
		private void aEditSelectAll_Update(object sender, System.EventArgs e)
		{
			((CDiese.Actions.Action)sender).Enabled = this.SelectedDocument != null;
		}
		#endregion

		#endregion

		#region *PetriNetDocument Events

		#region private void pnd_PropertiesChanged(object sender, EventArgs e)
		private void pnd_PropertiesChanged(object sender, EventArgs e)
		{
			piPropertiesInspector_SelectedObjectChanged(sender, e);

			// If place tokens number has changed update Rules Editor
			// to reflect changes regarding Rule evaluation
			if (sender is Place)
				this.reRulesEditor.UpdateRules();
		}
		#endregion

		#region private void pnd_ContentsChanged(object sender, EventArgs e)
		private void pnd_ContentsChanged(object sender, EventArgs e)
		{
			tabMagicTab_SelectionChanged(sender, e);
		}
		#endregion

		#region private void pnd_SelectedObjectsChanged(object sender, EventArgs e)
		private void pnd_SelectedObjectsChanged(object sender, EventArgs e)
		{
			if (this.SelectedDocument.SelectedObjects.Count > 1)
				this.piPropertiesInspector.SelectedObject = null;
			else if (this.SelectedDocument.SelectedObjects.Count == 1)
				this.piPropertiesInspector.SelectedObject = this.SelectedDocument.SelectedObjects[0];
			else
				this.piPropertiesInspector.SelectedObject = this.SelectedDocument;
		}
		#endregion

		#region private void pnd_PropertiesInspectorChangeRequested(object sender, EventArgs e)
		private void pnd_PropertiesInspectorChangeRequested(object sender, EventArgs e)
		{
			if (this.SelectedDocument.View == DocumentView.Editor)
			{
				ArrayList al = new ArrayList();
				foreach(object o in this.SelectedDocument.Objects)
				{
					al.Add(o);
				}
				foreach(object o in this.SelectedDocument.Connections)
				{
					al.Add(o);
				}
				al.Add(this.SelectedDocument);

				this.piPropertiesInspector.Objects = al;
				pnd_SelectedObjectsChanged(sender, e);
			}
			else if (this.SelectedDocument.View == DocumentView.Description)
				this.piPropertiesInspector.SelectedObject = null;
			else if (this.SelectedDocument.View == DocumentView.Response)
			{
				ArrayList al = new ArrayList();
				al.Add(this.SelectedDocument.ResponseOptions);

				this.piPropertiesInspector.Objects = al;
				this.piPropertiesInspector.SelectedObject = this.SelectedDocument.ResponseOptions;
			}
		}
		#endregion

		#endregion


		#region private void piPropertiesInspector_SelectedObjectChanged(object sender, EventArgs e)
		private void piPropertiesInspector_SelectedObjectChanged(object sender, EventArgs e)
		{
			object o = this.piPropertiesInspector.SelectedObject;

			if (o is PetriNetDocument)
			{
				PetriNetDocument pnd = (PetriNetDocument)o;

				if (pnd.PetriNetType == PetriNetType.TimeInvariant)
					this.piPropertiesInspector.BrowsableAttributes = new AttributeCollection(new Attribute[]{new TimeInvariantAttribute()});
				else
					this.piPropertiesInspector.BrowsableAttributes = new AttributeCollection(new Attribute[]{new CommonPropertiesAttribute()});
			}
			else if (o is Place)
			{
				if (!(o is PlaceInput))
				{
					if (this.SelectedDocument.PetriNetType == PetriNetType.TimeInvariant)
					{
						this.piPropertiesInspector.BrowsableAttributes = new AttributeCollection(new Attribute[]{new TimeInvariantAttribute()});
					}
					else
					{
						this.piPropertiesInspector.BrowsableAttributes = new AttributeCollection(new Attribute[]{new CommonPropertiesAttribute()});
					}
				}
				else
				{
					PlaceInput pi = (PlaceInput)o;

					if (pi.InputType == InputType.Fixed)
						this.piPropertiesInspector.BrowsableAttributes = new AttributeCollection(new Attribute[]{new TimeInvariantAttribute(), new PeriodicInputTypeAttribute(), new StohasticInputTypeAttribute()});
					else if (pi.InputType == InputType.Periodic)
						this.piPropertiesInspector.BrowsableAttributes = new AttributeCollection(new Attribute[]{new TimeInvariantAttribute(), new PeriodicInputTypeAttribute()});
					else if (pi.InputType == InputType.Stohastic)
						this.piPropertiesInspector.BrowsableAttributes = new AttributeCollection(new Attribute[]{new TimeInvariantAttribute(), new StohasticInputTypeAttribute()});
				}
			}
			else
			{
				this.piPropertiesInspector.BrowsableAttributes = new AttributeCollection(new Attribute[]{new CommonPropertiesAttribute()});
			}

			if (o is SelectableAndMovableControl)
			{
				SelectableAndMovableControl s = (SelectableAndMovableControl)o;
				s.PerformActivation();
			}
			else if (o is Connection)
			{
				Connection cn = (Connection)o;
				if (cn.IsVirtual == true)
				{
					this.piPropertiesInspector.BrowsableAttributes = new AttributeCollection(new Attribute[]{new CommonPropertiesAttribute()});
				}
				else
				{
					this.piPropertiesInspector.BrowsableAttributes = new AttributeCollection(new Attribute[]{new ConnectionPropertiesAttribute()});
				}

				cn.PerformActivation();
			}
			else if (o is PetriNetDocument)
			{
				PetriNetDocument pnd = (PetriNetDocument)o;
				pnd.PerformActivation();
			}
		}
		#endregion


		#region *Simulator Events

		#region private void sSimulator_SimulationFinished(object sender, EventArgs e)
		private void sSimulator_SimulationFinished(object sender, EventArgs e)
		{
System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;
            mcSimulateStartWithParam.Enabled = mcSimulateStart.Enabled = tbbStart.Enabled = false;
			tbbStop.Enabled = false;
			tbbPause.Enabled = false;
			tbbReset.Enabled = true;
            mcSimulateStep.Enabled = tbbStep.Enabled = false;
			MessageBox.Show(this, "End of simulation!", "Petri .NET Simulator 2.0 - Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = true;

        }
		#endregion

        #region public void PtimedStepFinishedMT
        public delegate void InvokeDelegatePtimedStepFinished(string s);
        public void PtimedStepFinishedMT(string s)
        {
            object[] obj = new object[1];
            obj[0] = s;
            BeginInvoke(new InvokeDelegatePtimedStepFinished(PtimedStepFinished), obj);
        }

        public void PtimedStepFinished(string s)
        { 
		    sbpSimulationTimePanel.Text = s;
		    pbSimulationProgress.Value++;
		    sbStatusBar.Refresh();
        }
        #endregion



		#region private void sSimulator_SimulationStepFinished(object sender, EventArgs e)
		private void sSimulator_SimulationStepFinished(object sender, EventArgs e)
		{
			// Refresh Rules
			this.reRulesEditor.UpdateRules();

			// Refresh time
			if (this.SelectedDocument.PetriNetType == PetriNetType.PTimed)
			{
				float fTime = float.Parse(sbpSimulationTimePanel.Text);
				fTime += this.SelectedDocument.Td / 1000f;

				NumberFormatInfo nmi = (NumberFormatInfo)Application.CurrentCulture.NumberFormat.Clone();
				nmi.NumberDecimalDigits = 2;

                //System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;

				//sbpSimulationTimePanel.Text = fTime.ToString("N", nmi);
                //pbSimulationProgress.Value++;
                //sbStatusBar.Refresh();

                this.PtimedStepFinishedMT(fTime.ToString("N", nmi));

                //System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = true;

			}


//			if (this.SelectedDocument.View == DocumentView.Response && tabResponse.SelectedTab == tpMatrixPage)
//			{
//				// Select current vector m
//				lvResponseMatrix.BeginUpdate();
//
//				foreach(ListViewItem lvi in lvResponseMatrix.Items)
//				{
//					if (lvi.Index % 2 == 1)
//						lvi.BackColor = Color.FromArgb(245, 245, 245);
//					else
//						lvi.BackColor = SystemColors.Window;
//				}
//
//				int[] ia = new int[Place.GroupedPlaces.Count];
//				for(int i = 0; i < ia.Length; i++)
//				{
//					ia[i] = ((Place)Place.GroupedPlaces[i]).Tokens;
//				}
//
//				ListViewItem lviCurrent = null;
//				foreach(ListViewItem lvi in lvResponseMatrix.Items)
//				{
//					int iCount = 0;
//					for (int i = 0; i < ia.Length; i++)
//					{
//						if (ia[i] == int.Parse(lvi.SubItems[i + 1].Text))
//							iCount++;
//					}
//
//					if (iCount == ia.Length)
//					{
//						lviCurrent = lvi;
//						break;
//					}
//				}
//
//				if (lviCurrent != null)
//					lviCurrent.BackColor = Color.LightSteelBlue;
//
//				lvResponseMatrix.EndUpdate();
//			}
//
//			else if (tabMain.SelectedTab == tpResponsePage && tabResponse.SelectedTab == tpOscillogramPage)
//			{
//				foreach(Control c in tpOscillogramPage.Controls)
//				{
//					if (c is Oscillogram)
//					{
//						// TODO: add oscilogram marker
//					}
//				}
//			}

		}
		#endregion

		#region private void sSimulator_SimulationProcessFinished(object sender, EventArgs e)
		private void sSimulator_SimulationProcessFinished(object sender, EventArgs e)
		{
			tbToolBar_ButtonClick(this, new ToolBarButtonClickEventArgs(tbbPause));
		}
		#endregion

		#endregion

		#region *Undo/Redo

		#region private void cmUndo_Popup(object sender, EventArgs e)
		private void cmUndo_Popup(object sender, EventArgs e)
		{
			cmUndo.MenuItems.Clear();

			foreach(UndoRedoItem uri in this.SelectedDocument.Editor.UndoList)
			{
				MenuItem mi = new MenuItem(uri.ToString());
				mi.Click += new EventHandler(miUndo_Click);
				cmUndo.MenuItems.Add(mi);
			}
		}
		#endregion

		#region private void cmRedo_Popup(object sender, EventArgs e)
		private void cmRedo_Popup(object sender, EventArgs e)
		{
			cmRedo.MenuItems.Clear();

			foreach(UndoRedoItem uri in this.SelectedDocument.Editor.RedoList)
			{
				MenuItem mi = new MenuItem(uri.ToString());
				mi.Click += new EventHandler(miRedo_Click);
				cmRedo.MenuItems.Add(mi);
			}

		}
		#endregion

		#region private void miUndo_Click(object sender, EventArgs e)
		private void miUndo_Click(object sender, EventArgs e)
		{
			MenuItem mi = (MenuItem)sender;
			this.SelectedDocument.Editor.SuspendLayout();
			for(int i = 0; i <= mi.Index; i++)
			{
				mcEditUndo_Click(sender, e);
			}
			this.SelectedDocument.Editor.ResumeLayout();
		}
		#endregion

		#region private void miRedo_Click(object sender, EventArgs e)
		private void miRedo_Click(object sender, EventArgs e)
		{
			MenuItem mi = (MenuItem)sender;
			this.SelectedDocument.Editor.SuspendLayout();
			for(int i = 0; i <= mi.Index; i++)
			{
				mcEditRedo_Click(sender, e);
			}
			this.SelectedDocument.Editor.ResumeLayout();
		}
		#endregion

		#endregion


		#region private void tmrDemoTimer_Tick(object sender, EventArgs e)
#if DEMO
		private void tmrDemoTimer_Tick(object sender, EventArgs e)
		{
			sbpPanelMain.Text = "DEMO Timeout = " + this.iTimeout/60 + ":" + ((this.iTimeout % 60 < 10) ? "0" + ((int)(this.iTimeout % 60)).ToString() : ((int)(this.iTimeout % 60)).ToString());

			if (this.iTimeout == 0)
			{
				tmrDemoTimer.Stop();
				MessageBox.Show("DEMO version timeout has just expired. Application will now terminate!", "Petri .NET Simulator 2.0 - Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);

				Application.Exit();
			}

			this.iTimeout--;
		}
#endif
		#endregion


        public void SetScriptName(string sn)
        {
            this.pythoneditor.FullTitle = sn + " editor";    //
            this.pythoneoutput.FullTitle = sn + " output";
        }

        void rfm_OnRecentFileSelected(object sender, EventArgs e)
        {
            string sf = this.rfm.GetSelectedRecentFile();
            this.OpenFile(sf);
        }
	}
}
