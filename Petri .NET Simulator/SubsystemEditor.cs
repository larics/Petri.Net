using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using System.Runtime.Serialization;

namespace PetriNetSimulator2
{
	/// <summary>
	/// Summary description for SubsystemEditor.
	/// </summary>
	[Serializable]
	public class SubsystemEditor : System.Windows.Forms.Form, ISerializable
	{
		#region public PetriNetEditor Editor
		public PetriNetEditor Editor
		{
			get
			{
				return this.pneEditor;
			}
		}
		#endregion

		private PetriNetDocument pnd;
		private PetriNetEditor pneEditor;
		private Subsystem ssOwner;

		private ArrayList alDeserializedObjects = new ArrayList();
		private ArrayList alDeserializedConnections = new ArrayList();
		private object oDeserializedZoomLevel;

		private ToolBox tbToolBox = new ToolBox();
		private PropertiesInspector piPropertiesInspector = new PropertiesInspector();

		protected Crownwood.Magic.Docking.DockingManager dmDockingManager = null;
		private System.Windows.Forms.ImageList ilContent;
		private System.ComponentModel.IContainer components;

		private ToolBar tbToolBar = new ToolBar();
		private ToolBarButton tbbReset = new ToolBarButton();
		private ToolBarButton tbbStart = new ToolBarButton();
		private ToolBarButton tbbPause = new ToolBarButton();
		private ToolBarButton tbbStop = new ToolBarButton();
		private ToolBarButton tbbStep = new ToolBarButton();
		private ToolBarButton tbbSeparator2 = new ToolBarButton();
//		private ToolBarButton tbbConflicts = new ToolBarButton();
//		private ToolBarButton tbbCircularWaits = new ToolBarButton();
//		private ToolBarButton tbbFireable = new ToolBarButton();
//		private ToolBarButton tbbFired = new ToolBarButton();
//		private ToolBarButton tbbSeparator3 = new ToolBarButton();
		private ToolBarButton tbbZoom = new ToolBarButton();
		private ToolBarButton tbbSeparator4 = new ToolBarButton();
		private ToolBarButton tbbUndo = new ToolBarButton();
		private ToolBarButton tbbRedo = new ToolBarButton();

		private ContextMenu cmUndo = new ContextMenu();
		private ContextMenu cmRedo = new ContextMenu();

		private CDiese.Actions.ActionList alMenuActionList = new CDiese.Actions.ActionList();
		private CDiese.Actions.Action aEditUndo = new CDiese.Actions.Action();
		private CDiese.Actions.Action aEditRedo = new CDiese.Actions.Action();
		private CDiese.Actions.Action aEditDelete = new CDiese.Actions.Action();
		private CDiese.Actions.Action aEditCut = new CDiese.Actions.Action();
		private CDiese.Actions.Action aEditCopy = new CDiese.Actions.Action();
		private CDiese.Actions.Action aEditPaste = new CDiese.Actions.Action();
		private CDiese.Actions.Action aEditGroup = new CDiese.Actions.Action();
		private CDiese.Actions.Action aEditSelectAll = new CDiese.Actions.Action();

		private Crownwood.Magic.Menus.MenuControl mcMenuControl = null;
		private Crownwood.Magic.Menus.MenuCommand mcFile = null;
		private Crownwood.Magic.Menus.MenuCommand mcFileExport = null;
		private Crownwood.Magic.Menus.MenuCommand mcFileSeparator1 = null;
		private Crownwood.Magic.Menus.MenuCommand mcFileClose = null;
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
		private Crownwood.Magic.Menus.MenuCommand mcView = null;
		private System.Windows.Forms.ImageList ilMenu;
		private System.Windows.Forms.ImageList ilToolBar;
		private System.Windows.Forms.ContextMenu cmZoom;
		private System.Windows.Forms.MenuItem miZoom100;
		private System.Windows.Forms.MenuItem miZoom90;
		private System.Windows.Forms.MenuItem miZoom80;
		private System.Windows.Forms.MenuItem miZoom70;
		private System.Windows.Forms.MenuItem miZoom60;
		private System.Windows.Forms.SaveFileDialog sfdExportFile;
		private System.Windows.Forms.MenuItem miZoom50;

		#region public SubsystemEditor(PetriNetDocument pnd, Subsystem ssOwner)
		public SubsystemEditor(PetriNetDocument pnd, Subsystem ssOwner)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			Rectangle r = Screen.PrimaryScreen.Bounds;
			this.Size = new Size((int)(r.Width * 0.9), (int)(r.Height * 0.9));

			SubsystemEditor_CreateGUI(pnd, ssOwner);
		}
		#endregion

		// Constructor for Deserialization
		#region public SubsystemEditor(SerializationInfo info, StreamingContext context)
		public SubsystemEditor(SerializationInfo info, StreamingContext context)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			Rectangle r = Screen.PrimaryScreen.Bounds;
			this.Size = new Size((int)(r.Width * 0.9), (int)(r.Height * 0.9));

			ArrayList alHeader = (ArrayList)info.GetValue("header", typeof(ArrayList));

			for(int i = 0; i < (int)alHeader[0]; i++)
			{
				alDeserializedObjects.Add(info.GetValue("O" + i.ToString(), typeof(object)));
			}

			for(int i = 0; i < (int)alHeader[1]; i++)
			{
				alDeserializedConnections.Add(info.GetValue("C" + i.ToString(), typeof(object)));
			}

			this.oDeserializedZoomLevel = (float)info.GetValue("zoomlevel", typeof(float));
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
			this.components = new System.ComponentModel.Container();
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(SubsystemEditor));
			this.ilContent = new System.Windows.Forms.ImageList(this.components);
			this.ilMenu = new System.Windows.Forms.ImageList(this.components);
			this.ilToolBar = new System.Windows.Forms.ImageList(this.components);
			this.cmZoom = new System.Windows.Forms.ContextMenu();
			this.miZoom100 = new System.Windows.Forms.MenuItem();
			this.miZoom90 = new System.Windows.Forms.MenuItem();
			this.miZoom80 = new System.Windows.Forms.MenuItem();
			this.miZoom70 = new System.Windows.Forms.MenuItem();
			this.miZoom60 = new System.Windows.Forms.MenuItem();
			this.miZoom50 = new System.Windows.Forms.MenuItem();
			this.sfdExportFile = new System.Windows.Forms.SaveFileDialog();
			// 
			// ilContent
			// 
			this.ilContent.ColorDepth = System.Windows.Forms.ColorDepth.Depth24Bit;
			this.ilContent.ImageSize = new System.Drawing.Size(16, 16);
			this.ilContent.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilContent.ImageStream")));
			this.ilContent.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// ilMenu
			// 
			this.ilMenu.ColorDepth = System.Windows.Forms.ColorDepth.Depth24Bit;
			this.ilMenu.ImageSize = new System.Drawing.Size(16, 16);
			this.ilMenu.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilMenu.ImageStream")));
			this.ilMenu.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// ilToolBar
			// 
			this.ilToolBar.ColorDepth = System.Windows.Forms.ColorDepth.Depth24Bit;
			this.ilToolBar.ImageSize = new System.Drawing.Size(24, 24);
			this.ilToolBar.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilToolBar.ImageStream")));
			this.ilToolBar.TransparentColor = System.Drawing.Color.Transparent;
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
			// sfdExportFile
			// 
			this.sfdExportFile.DefaultExt = "emf";
			this.sfdExportFile.Filter = "Enhanced metafile (*.emf)|*.emf";
			this.sfdExportFile.Title = "Export Petri .NET subsystem model.";
			// 
			// SubsystemEditor
			// 
			this.AutoScale = false;
			this.AutoScaleBaseSize = new System.Drawing.Size(6, 15);
			this.ClientSize = new System.Drawing.Size(890, 670);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			this.KeyPreview = true;
			this.Name = "SubsystemEditor";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "SubsystemEditor";
			this.Closing += new System.ComponentModel.CancelEventHandler(this.SubsystemEditor_Closing);
			this.Load += new System.EventHandler(this.SubsystemEditor_Load);

		}
		#endregion


		#region public void GetObjectData(SerializationInfo info, StreamingContext context)
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			// Save all data to file

			//Save Editor properties
			info.AddValue("zoomlevel", this.pneEditor.Zoom);

			// Save Header
			ArrayList al = new ArrayList();
			al.Add(this.pneEditor.Objects.Count);
			al.Add(this.pneEditor.Connections.Count);

			info.AddValue("header", al);

			// Save all objects
			for(int i = 0; i < this.pneEditor.Objects.Count; i++)
			{
				info.AddValue("O" + i.ToString(), this.pneEditor.Objects[i]);
			}

			// Save all connections
			for(int i = 0; i < this.pneEditor.Connections.Count; i++)
			{
				info.AddValue("C" + i.ToString(), this.pneEditor.Connections[i]);
			}
		}
		#endregion


		#region private void SubsystemEditor_Load(object sender, System.EventArgs e)
		private void SubsystemEditor_Load(object sender, System.EventArgs e)
		{
			this.tbToolBox.ExpandAll();
		}
		#endregion

		#region private void SubsystemEditor_CreateGUI(PetriNetDocument pnd, Subsystem ssOwner)
		private void SubsystemEditor_CreateGUI(PetriNetDocument pnd, Subsystem ssOwner)
		{
			this.pnd = pnd;
			this.ssOwner = ssOwner;

			Panel pnlEditorContainer = new Panel();
			pnlEditorContainer.AutoScroll = true;
			pnlEditorContainer.Dock = DockStyle.Fill;

			this.pneEditor = new PetriNetEditor(pnd, pnd.InstanceCounter);
			this.pneEditor.AutoScroll = true;
			this.pneEditor.AutoScrollMinSize = new Size(3000, 3000);
			this.pneEditor.Size = new Size(3000, 3000);
			this.pneEditor.Location = new Point(0, 0);
			this.pneEditor.ContentsChanged += new EventHandler(pneEditor_ContentsChanged);
			this.pneEditor.SelectedObjectsChanged += new EventHandler(pneEditor_SelectedObjectsChanged);
			this.pneEditor.PropertiesChanged += new EventHandler(pneEditor_PropertiesChanged);
			
			if (this.oDeserializedZoomLevel != null)
				this.pneEditor.Zoom = (float)this.oDeserializedZoomLevel;

			pnlEditorContainer.Controls.Add(pneEditor);
			this.Controls.Add(pnlEditorContainer);

			// Create the object that manages the docking state
			this.dmDockingManager = new Crownwood.Magic.Docking.DockingManager(this, Crownwood.Magic.Common.VisualStyle.IDE);
			this.dmDockingManager.InnerControl = pnlEditorContainer;
			this.dmDockingManager.OuterControl = tbToolBar;

			Crownwood.Magic.Docking.Content toolbox = dmDockingManager.Contents.Add(this.tbToolBox, "Toolbox", this.ilContent, 1);
			toolbox.AutoHideSize = new Size(200, 300);
			toolbox.DisplaySize = new Size(200, 300);
			toolbox.FloatingSize = new Size(200, 300);
			Crownwood.Magic.Docking.Content properties = dmDockingManager.Contents.Add(this.piPropertiesInspector, "Properties", this.ilContent, 0);
			properties.AutoHideSize = new Size(200, 300);
			properties.DisplaySize = new Size(200, 300);
			properties.FloatingSize = new Size(200, 300);
			Crownwood.Magic.Docking.WindowContent wc = dmDockingManager.AddContentWithState(toolbox, Crownwood.Magic.Docking.State.DockLeft);

			//dmDockingManager.AddContentToWindowContent(notePad2, wc);
			// Add a new WindowContent to the existing Zone already created
			this.dmDockingManager.AddContentToZone(properties, wc.ParentZone, 1);

			this.tbToolBox.EnabledToolbox = true;

			// Load layout
			if (File.Exists(Application.StartupPath + "\\layoutSubsystem.config.xml"))
				this.dmDockingManager.LoadConfigFromFile(Application.StartupPath + "\\layoutSubsystem.config.xml");

			// Initialize piPropertiesInspector
			this.piPropertiesInspector.SelectedObjectChanged += new EventHandler(piPropertiesInspector_SelectedObjectChanged);

			#region Initialize ToolBar control
			this.tbToolBar.Dock = DockStyle.Top;
			this.tbToolBar.ButtonSize = new Size(24, 24);
			this.tbToolBar.ImageList = ilToolBar;
			this.tbToolBar.ButtonClick += new ToolBarButtonClickEventHandler(tbToolBar_ButtonClick);
			this.tbToolBar.Appearance = ToolBarAppearance.Flat;

			this.tbbReset.Enabled = false;
			this.tbbReset.ImageIndex = 3;
			this.tbToolBar.Buttons.Add(tbbReset);

			this.tbbStart.Enabled = false;
			this.tbbStart.ImageIndex = 4;
			this.tbToolBar.Buttons.Add(tbbStart);

			this.tbbPause.Enabled = false;
			this.tbbPause.ImageIndex = 5;
			this.tbToolBar.Buttons.Add(tbbPause);

			this.tbbStop.Enabled = false;
			this.tbbStop.ImageIndex = 6;
			this.tbToolBar.Buttons.Add(tbbStop);

			this.tbbStep.Enabled = false;
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

//			this.tbbConflicts.ImageIndex = 8;
//			this.tbbConflicts.Style = ToolBarButtonStyle.ToggleButton;
//			this.tbToolBar.Buttons.Add(tbbConflicts);
//
//			this.tbbCircularWaits.ImageIndex = 9;
//			this.tbbCircularWaits.Style = ToolBarButtonStyle.ToggleButton;
//			this.tbToolBar.Buttons.Add(tbbCircularWaits);
//
//			this.tbbFireable.ImageIndex = 10;
//			this.tbbFireable.Style = ToolBarButtonStyle.ToggleButton;
//			this.tbbFireable.Pushed = true;
//			this.tbToolBar.Buttons.Add(tbbFireable);
//
//			this.tbbFired.ImageIndex = 11;
//			this.tbbFired.Style = ToolBarButtonStyle.ToggleButton;
//			this.tbbFired.Pushed = true;
//			this.tbToolBar.Buttons.Add(tbbFired);
//
//			this.tbbSeparator3.Style = ToolBarButtonStyle.Separator;
//			this.tbToolBar.Buttons.Add(tbbSeparator3);

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

			mcFileExport = new Crownwood.Magic.Menus.MenuCommand("&Export...");
			mcFileExport.Shortcut = Shortcut.CtrlE;
			mcFileExport.Click += new EventHandler(mcFileExport_Click);
			mcFile.MenuCommands.Add(mcFileExport);

			mcFileSeparator1 = new Crownwood.Magic.Menus.MenuCommand("-");
			mcFile.MenuCommands.Add(mcFileSeparator1);

			mcFileClose = new Crownwood.Magic.Menus.MenuCommand("&Close");
			mcFileClose.Click += new EventHandler(mcFileClose_Click);
			mcFile.MenuCommands.Add(mcFileClose);

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


			#region Initialize ActionList
			// Initialize ActionList
			this.alMenuActionList.ImageList = this.ilMenu;

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

			#endregion

			// Initialize Undo/Redo context menus
			this.cmUndo.Popup += new EventHandler(cmUndo_Popup);
			this.cmRedo.Popup += new EventHandler(cmRedo_Popup);
		}
		#endregion

		#region private void SubsystemEditor_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		private void SubsystemEditor_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			this.ssOwner.OnEditingFinished();

			// Save layout
			this.dmDockingManager.SaveConfigToFile(Application.StartupPath + "\\layoutSubsystem.config.xml");
		}
		#endregion


		#region private void piPropertiesInspector_SelectedObjectChanged(object sender, EventArgs e)
		private void piPropertiesInspector_SelectedObjectChanged(object sender, EventArgs e)
		{
			object o = this.piPropertiesInspector.SelectedObject;

			if (o is Place)
			{
				if (!(o is PlaceInput))
				{
					if (this.pnd.PetriNetType == PetriNetType.TimeInvariant)
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
		}
		#endregion


		#region private void pneEditor_ContentsChanged(object sender, EventArgs e)
		private void pneEditor_ContentsChanged(object sender, EventArgs e)
		{
			ArrayList al = new ArrayList();
			foreach(object o in this.pneEditor.Objects)
			{
				al.Add(o);
			}
			foreach(object o in this.pneEditor.Connections)
			{
				al.Add(o);
			}

			// Because Place name was changed
			// Need to refresh PropertiesInspector combo box
			// Remember previously selected object

			object oo = this.piPropertiesInspector.SelectedObject;
			this.piPropertiesInspector.Objects = al;
			this.piPropertiesInspector.SelectedObject = oo;
		}
		#endregion

		#region private void pneEditor_SelectedObjectsChanged(object sender, EventArgs e)
		private void pneEditor_SelectedObjectsChanged(object sender, EventArgs e)
		{
			if (this.pneEditor.SelectedObjects.Count != 1)
				this.piPropertiesInspector.SelectedObject = null;
			else
				this.piPropertiesInspector.SelectedObject = this.pneEditor.SelectedObjects[0];
		}
		#endregion

		#region private void pneEditor_PropertiesChanged(object sender, EventArgs e)
		private void pneEditor_PropertiesChanged(object sender, EventArgs e)
		{
			this.piPropertiesInspector_SelectedObjectChanged(sender, e);
		}
		#endregion


		#region public void RestoreAfterDeserialization(PetriNetDocument pnd, Subsystem ssOwner)
		public void RestoreAfterDeserialization(PetriNetDocument pnd, Subsystem ssOwner)
		{
			// Create GUI
			SubsystemEditor_CreateGUI(pnd, ssOwner);

			// Restore controls
			RestoreControlsAndConnections();
		}
		#endregion

		#region private void RestoreControlsAndConnections()
		private void RestoreControlsAndConnections()
		{
			Hashtable ht = new Hashtable();

			// Add all objects and connections
			foreach(object o in this.alDeserializedObjects)
			{
				this.pneEditor.AddDeserializedControl((SelectableAndMovableControl)o);

				if (o is ConnectableControl)
				{
					ConnectableControl cc = (ConnectableControl)o;
					ht.Add(cc.GetType().FullName + cc.Index.ToString(), cc);
				}
			}
			foreach(object o in this.alDeserializedConnections)
			{
				Connection cn = (Connection)o;
				cn.RestoreLinks(ht);
				this.pneEditor.AddDeserializedConnection((Connection)o);
			}

			this.ssOwner.OnEditingFinished();

			this.pneEditor_ContentsChanged(this, EventArgs.Empty);
		}
		#endregion


		#region public object Clone(Subsystem s)
		public object Clone(Subsystem s)
		{
			if (this.pnd != null)
			{
				SubsystemEditor se = new SubsystemEditor(this.pnd, s);
			
				Hashtable ht = new Hashtable();

				for(int i = 0; i < this.pneEditor.Objects.Count; i++)
				{
					if (this.pneEditor.Objects[i] is ConnectableControl && this.pneEditor.Objects[i] is ICloneable)
					{
						ConnectableControl cc = (ConnectableControl)((ICloneable)this.pneEditor.Objects[i]).Clone();
						ht.Add(this.pneEditor.Objects[i], cc);
						if (!(cc is Input) && !(cc is Output))
							cc.Index = this.pnd.InstanceCounter.GetAndIncreaseInstanceCount(cc);
						else
						{
							cc.Index = ((ConnectableControl)this.pneEditor.Objects[i]).Index;
						}
						se.alDeserializedObjects.Add(cc);
					}
					else if (this.pneEditor.Objects[i] is DescriptionLabel && this.pneEditor.Objects[i] is ICloneable)
					{
						DescriptionLabel dl = (DescriptionLabel)((ICloneable)this.pneEditor.Objects[i]).Clone();
						ht.Add(this.pneEditor.Objects[i], dl);
						dl.Index = ((DescriptionLabel)this.pneEditor.Objects[i]).Index;
						se.alDeserializedObjects.Add(dl);
					}
				}

				for(int i = 0; i < this.pneEditor.Connections.Count; i++)
				{
					Connection cn = (Connection)this.pneEditor.Connections[i];
					cn = (Connection)cn.Clone((ConnectableControl)ht[cn.From], (ConnectableControl)ht[cn.To]);
					se.alDeserializedConnections.Add(cn);
				}

				foreach(object o in se.alDeserializedObjects)
				{
					if (o is Subsystem)
					{
						Subsystem ss = (Subsystem)o;
						ss.SuppresDeserializationCall();
					}
				}

				se.RestoreControlsAndConnections();

				foreach(object o in se.alDeserializedObjects)
				{
					if (o is Subsystem)
					{
						Subsystem ss = (Subsystem)o;
						ss.EnableDeserializationCall();
					}
				}

				return se;
			}

			return null;
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

				if (this.pneEditor.Zoom == f)
					mi.Checked = true;
				else
					mi.Checked = false;
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

			this.pneEditor.Zoom = f;
		}
		#endregion


		#region private void tbToolBar_ButtonClick(object sender, ToolBarButtonClickEventArgs e)
		private void tbToolBar_ButtonClick(object sender, ToolBarButtonClickEventArgs e)
		{
			if (e.Button == this.tbbUndo)
			{
				mcEditUndo_Click(sender, EventArgs.Empty);
			}
			else if (e.Button == this.tbbRedo)
			{
				mcEditRedo_Click(sender, EventArgs.Empty);
			}
		}
		#endregion


		#region *MainMenu

		#region private void mcFileClose_Click(object sender, System.EventArgs e)
		private void mcFileClose_Click(object sender, System.EventArgs e)
		{
			this.Close();
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
				MetafileExporter.SaveMetafile(fs, this.pneEditor);

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
			UndoRedoItem uri = (UndoRedoItem)this.Editor.UndoList.Pop();
			this.Editor.RedoList.Push(uri);

			uri.Undo();
		}
		#endregion

		#region private void mcEditRedo_Click(object sender, System.EventArgs e)
		private void mcEditRedo_Click(object sender, System.EventArgs e)
		{
			UndoRedoItem uri = (UndoRedoItem)this.Editor.RedoList.Pop();
			this.Editor.UndoList.Push(uri);

			uri.Redo();
		}
		#endregion

		#region private void mcEditCut_Click(object sender, System.EventArgs e)
		private void mcEditCut_Click(object sender, System.EventArgs e)
		{
			Clipboard.Object = this.pneEditor.CopySelection();
			this.pneEditor.DeleteSelectedControls();
		}
		#endregion

		#region private void mcEditCopy_Click(object sender, System.EventArgs e)
		private void mcEditCopy_Click(object sender, System.EventArgs e)
		{
			Clipboard.Object = this.pneEditor.CopySelection();
		}
		#endregion

		#region private void mcEditPaste_Click(object sender, System.EventArgs e)
		private void mcEditPaste_Click(object sender, System.EventArgs e)
		{
			this.pneEditor.Paste(Clipboard.Object);
		}
		#endregion

		#region private void mcEditDelete_Click(object sender, System.EventArgs e)
		private void mcEditDelete_Click(object sender, System.EventArgs e)
		{
			if (DialogResult.Yes == MessageBox.Show("Are you sure you want to delete selected items?", "Petri .NET Simulator 2.0 - Question", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question))
			{
				this.pneEditor.DeleteSelectedControls();
			}
		}
		#endregion

		#region private void mcEditCopyModel_Click(object sender, System.EventArgs e)
		private void mcEditCopyModel_Click(object sender, System.EventArgs e)
		{
#if !DEMO
			MemoryStream ms = new MemoryStream();
			Metafile mf = MetafileExporter.SaveMetafile(ms, this.pneEditor);

			// Copy to clipboard
			ClipboardMetafileHelper.PutEnhMetafileOnClipboard(this.Handle, mf);
#else
			MessageBox.Show("DEMO version doesn't have implemented Copy to Clipboard function!\nBuy a full version which is capable of Copying to Clipboard.", "Petri .NET Simulator 2.0 - Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
#endif
		}
		#endregion

		#region private void mcEditGroup_Click(object sender, System.EventArgs e)
		private void mcEditGroup_Click(object sender, System.EventArgs e)
		{
			this.pneEditor.Group();
		}
		#endregion

		#region private void mcEditSelectAll_Click(object sender, System.EventArgs e)
		private void mcEditSelectAll_Click(object sender, System.EventArgs e)
		{
			this.pneEditor.SelectAll();
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

		#endregion

		#region *Actions

		#region private void aEditUndo_Update(object sender, System.EventArgs e)
		private void aEditUndo_Update(object sender, System.EventArgs e)
		{
			((CDiese.Actions.Action)sender).Enabled = this.Editor.UndoList.Count != 0;
			tbbUndo.Enabled = this.Editor.UndoList.Count != 0;
		}
		#endregion

		#region private void aEditRedo_Update(object sender, System.EventArgs e)
		private void aEditRedo_Update(object sender, System.EventArgs e)
		{
			((CDiese.Actions.Action)sender).Enabled = this.Editor.RedoList.Count != 0;
			tbbRedo.Enabled = this.Editor.RedoList.Count != 0;
		}
		#endregion

		#region private void aEditDelete_Update(object sender, System.EventArgs e)
		private void aEditDelete_Update(object sender, System.EventArgs e)
		{
			((CDiese.Actions.Action)sender).Enabled = this.Editor.SelectedObjects.Count != 0;
		}
		#endregion

		#region private void aEditCut_Update(object sender, System.EventArgs e)
		private void aEditCut_Update(object sender, System.EventArgs e)
		{
			((CDiese.Actions.Action)sender).Enabled = this.Editor.SelectedObjects.Count != 0;
		}
		#endregion

		#region private void aEditCopy_Update(object sender, System.EventArgs e)
		private void aEditCopy_Update(object sender, System.EventArgs e)
		{
			((CDiese.Actions.Action)sender).Enabled = this.Editor.SelectedObjects.Count != 0;
		}
		#endregion

		#region private void aEditGroup_Update(object sender, System.EventArgs e)
		private void aEditGroup_Update(object sender, System.EventArgs e)
		{
			((CDiese.Actions.Action)sender).Enabled = this.Editor.SelectedObjects.Count != 0;
		}
		#endregion

		#region private void aEditPaste_Update(object sender, System.EventArgs e)
		private void aEditPaste_Update(object sender, System.EventArgs e)
		{
			((CDiese.Actions.Action)sender).Enabled = Clipboard.Object != null;
		}
		#endregion

		#endregion

		#region *Undo/Redo

		#region private void cmUndo_Popup(object sender, EventArgs e)
		private void cmUndo_Popup(object sender, EventArgs e)
		{
			cmUndo.MenuItems.Clear();

			foreach(UndoRedoItem uri in this.pneEditor.UndoList)
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

			foreach(UndoRedoItem uri in this.pneEditor.RedoList)
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
			this.pneEditor.SuspendLayout();
			for(int i = 0; i <= mi.Index; i++)
			{
				mcEditUndo_Click(sender, e);
			}
			this.pneEditor.ResumeLayout();
		}
		#endregion

		#region private void miRedo_Click(object sender, EventArgs e)
		private void miRedo_Click(object sender, EventArgs e)
		{
			MenuItem mi = (MenuItem)sender;
			this.pneEditor.SuspendLayout();
			for(int i = 0; i <= mi.Index; i++)
			{
				mcEditRedo_Click(sender, e);
			}
			this.pneEditor.ResumeLayout();
		}
		#endregion

		#endregion

	}
}
