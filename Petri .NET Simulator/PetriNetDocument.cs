using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Globalization;
using System.Xml;
using System.Xml.XPath;

namespace PetriNetSimulator2
{
	/// <summary>
	/// Summary description for PetriNetDocument.
	/// </summary>
	public class PetriNetDocument : System.Windows.Forms.UserControl
	{
        // string
        public string pyCode;
        public PyOutput pyOutput;
        public PyEditor pyEditor;

		// Properties
		#region public IntMatrix Fu
		public IntMatrix Fu
		{
			get
			{
				IntMatrix Fu = CalculateMatrix(this.InputPlaces, this.Transitions);
				return Fu;
			}
		}
		#endregion

		#region public IntMatrix Fv
		public IntMatrix Fv
		{
			get
			{
				IntMatrix Fv = CalculateMatrix(this.OperationPlaces, this.Transitions);
				return Fv;
			}
		}
		#endregion

		#region public IntMatrix Fr
		public IntMatrix Fr
		{
			get
			{
				IntMatrix Fr = CalculateMatrix(this.ResourcePlaces, this.Transitions);
				return Fr;
			}
		}
		#endregion

		#region public IntMatrix Fd
		public IntMatrix Fd
		{
			get
			{
				IntMatrix Fd = CalculateMatrix(this.ControlPlaces, this.Transitions);
				return Fd;
			}
		}
		#endregion

		#region public IntMatrix Fy
		public IntMatrix Fy
		{
			get
			{
				IntMatrix Fy = CalculateMatrix(this.OutputPlaces, this.Transitions);
				return Fy;
			}
		}
		#endregion

		#region public IntMatrix Su
		public IntMatrix Su
		{
			get
			{
				IntMatrix Su = CalculateMatrix(this.Transitions, this.InputPlaces);
				return Su;
			}
		}
		#endregion

		#region public IntMatrix Sv
		public IntMatrix Sv
		{
			get
			{
				IntMatrix Sv = CalculateMatrix(this.Transitions, this.OperationPlaces);
				return Sv;
			}
		}
		#endregion

		#region public IntMatrix Sr
		public IntMatrix Sr
		{
			get
			{
				IntMatrix Sr = CalculateMatrix(this.Transitions, this.ResourcePlaces);
				return Sr;
			}
		}
		#endregion

		#region public IntMatrix Sd
		public IntMatrix Sd
		{
			get
			{
				IntMatrix Sd = CalculateMatrix(this.Transitions, this.ControlPlaces);
				return Sd;
			}
		}
		#endregion

		#region public IntMatrix Sy
		public IntMatrix Sy
		{
			get
			{
				IntMatrix Sy = CalculateMatrix(this.Transitions, this.OutputPlaces);
				return Sy;
			}
		}
		#endregion

		#region public IntMatrix Tv0
		public IntMatrix Tv0
		{
			get
			{
				IntMatrix imTv = new IntMatrix(this.OperationPlaces.Count, this.Transitions.Count);

				for(int i = 0; i < this.OperationPlaces.Count; i++)
				{
					for(int j = 0; j < this.Transitions.Count; j++)
					{
						Transition t = (Transition)this.Transitions[j];
						PlaceOperation po = (PlaceOperation)this.OperationPlaces[i];

						if (t.PlaceChilds.Contains(po))
						{
							imTv[i, j] = po.Duration;
						}
						else
						{
							imTv[i, j] = 0;
						}
					}
				}

				return imTv;
			}
		}
		#endregion

		#region public IntMatrix Tr0
		public IntMatrix Tr0
		{
			get
			{
				IntMatrix imTr = new IntMatrix(this.ResourcePlaces.Count, this.Transitions.Count);

				for(int i = 0; i < this.ResourcePlaces.Count; i++)
				{
					for(int j = 0; j < this.Transitions.Count; j++)
					{
						PlaceResource pr = (PlaceResource)this.ResourcePlaces[i];
						Transition t = (Transition)this.Transitions[j];

						if (t.PlaceChilds.Contains(pr))
						{
							//Find adequate release time
							foreach (ReleaseTime rt in pr.ReleaseTimes)
							{
								if (t.PlaceParents.Contains(rt.OperationPlace))
									imTr[i, j] = rt.Time;
							}
						}
						else
						{
							imTr[i, j] = 0;
						}
					}
				}

				return imTr;
			}
		}
		#endregion

		#region public IntMatrix Td0
		public IntMatrix Td0
		{
			get
			{
				IntMatrix imTd = new IntMatrix(this.ControlPlaces.Count, this.Transitions.Count);

				for(int i = 0; i < this.ControlPlaces.Count; i++)
				{
					for(int j = 0; j < this.Transitions.Count; j++)
					{
						Transition t = (Transition)this.Transitions[j];
						PlaceControl pc = (PlaceControl)this.ControlPlaces[i];

						if (t.PlaceChilds.Contains(pc))
						{
							imTd[i, j] = 1;
						}
						else
						{
							imTd[i, j] = 0;
						}
					}
				}

				return imTd;
			}
		}
		#endregion

		#region public IntMatrix Ty0
		public IntMatrix Ty0
		{
			get
			{
				IntMatrix imTy = new IntMatrix(this.OutputPlaces.Count, this.Transitions.Count);

				for(int i = 0; i < this.OutputPlaces.Count; i++)
				{
					for(int j = 0; j < this.Transitions.Count; j++)
					{
						Transition t = (Transition)this.Transitions[j];
						PlaceOutput po = (PlaceOutput)this.OutputPlaces[i];

						if (t.PlaceChilds.Contains(po))
						{
							imTy[i, j] = 1;
						}
						else
						{
							imTy[i, j] = 0;
						}
					}
				}

				return imTy;
			}
		}
		#endregion

		#region public IntMatrix F
		public IntMatrix F
		{
			get
			{
				IntMatrix F = CalculateMatrix(this.GroupedPlaces, this.Transitions);
				return F;
			}
		}
		#endregion

		#region public IntMatrix S
		public IntMatrix S
		{
			get
			{
				IntMatrix S = CalculateMatrix(this.Transitions, this.GroupedPlaces);
				return S;
			}
		}
		#endregion

		#region public IntMatrix W
		public IntMatrix W
		{
			get
			{
				IntMatrix im = this.S.Transpose() - this.F;
				return im;
			}
		}
		#endregion

		#region public IntMatrix Gw
		public IntMatrix Gw
		{
			get
			{
				IntMatrix Gw = this.Sr & this.Fr;
				return Gw;
			}
		}
		#endregion

		#region public StringMatrix[] CircularWaiting
		public StringMatrix[] CircularWaiting
		{
			get
			{
				return CalculateCircularWaiting();
			}
		}
		#endregion

		#region private IntMatrix CalculateMatrix(ArrayList alObjects, ArrayList alChilds)
		private IntMatrix CalculateMatrix(ArrayList alObjects, ArrayList alChilds)
		{
			object[,] oa = new object[alChilds.Count, alObjects.Count];

			//Initialize oa to zeros
			for(int i = 0; i < alChilds.Count; i++)
				for(int j = 0; j < alObjects.Count; j++)
					oa[i, j] = 0;

			foreach(Connection cn in this.ConnectionsAll)
			{
				if (cn.From is Place && alObjects.Contains(cn.From))
				{
					if (cn.To is Transition && alChilds.Contains(cn.To))
					{
						oa[alChilds.IndexOf(cn.To), alObjects.IndexOf(cn.From)] = cn.Weight;
					}
				}
				else if (cn.From is Transition && alObjects.Contains(cn.From))
				{
					if (cn.To is Place && alChilds.Contains(cn.To))
					{
						oa[alChilds.IndexOf(cn.To), alObjects.IndexOf(cn.From)] = cn.Weight;
					}
					if (cn.To is Subsystem)
					{
						// Find next connectable control
						Subsystem s = (Subsystem)cn.To;

						ArrayList alToControls = new ArrayList();
						ArrayList alWeights = new ArrayList();
						s.GetBaseControlConnectedToInputPort(alToControls, alWeights, cn.ToPort);

						foreach(ConnectableControl cc in alToControls)
						{
							if (alChilds.Contains(cc))
								oa[alChilds.IndexOf(cc), alObjects.IndexOf(cn.From)] = alWeights[alToControls.IndexOf(cc)];
						}
					}
				}
				else if (cn.From is Subsystem)
				{
					if (cn.To is Transition)
					{
						// Find next connectable control
						Subsystem s = (Subsystem)cn.From;

						ArrayList alFromControls = new ArrayList();
						ArrayList alWeights = new ArrayList();
						s.GetBaseControlConnectedToOutputPort(alFromControls, alWeights, cn.FromPort);

						foreach(ConnectableControl cc in alFromControls)
						{
							if (alObjects.Contains(cc))
								oa[alChilds.IndexOf(cn.To), alObjects.IndexOf(cc)] = alWeights[alFromControls.IndexOf(cc)];
						}
					}
				}
			}

			//Walk through all subsystems and add connections weights to matrix
			foreach(object o in this.Objects)
			{
				if (o is Subsystem)
				{
					Subsystem s = (Subsystem)o;
					s.FillInConnectionMatrices(oa, alObjects, alChilds);
				}
			}

			return new IntMatrix(oa);
		}
		#endregion

		#region private StringMatrix[] CalculateCircularWaiting()
		private StringMatrix[] CalculateCircularWaiting()
		{
			StringMatrix[] sma = new StringMatrix[this.ResourcePlaces.Count];

			if (this.ResourcePlaces.Count >= 1)
			{
				Hashtable ht = new Hashtable();

				// Add to hashtable Place with their characters
				char c = '1';
				foreach(Place p in this.ResourcePlaces)
				{
					ht.Add(p, c.ToString());
					c++;
				}

				IntMatrix m = Gw;

				StringMatrix sm = new StringMatrix(this.ResourcePlaces.Count, this.ResourcePlaces.Count);

				for(int i = 0; i < m.Dimensions.Height; i++)
				{
					for(int j = 0; j < m.Dimensions.Width; j++)
					{
						if ((int)m[i, j] == 1)
						{
							Place pi = (Place)this.ResourcePlaces[i];
							Place pj = (Place)this.ResourcePlaces[j];

							string s = (string)ht[pi] + (string)ht[pj];

							sm[i, j] = s;
						}
						else
						{
							sm[i, j] = "0";
						}
					}
				}

				sma[0] = sm;

				// Calculate other CircularWaiting Matrixes
				for (int i = 1; i < this.ResourcePlaces.Count; i++)
				{
					sma[i] = sma[i - 1] * sm;
				}
			}

			return sma;
		}
		#endregion


		#region public string FileName
		public string FileName
		{
			get
			{
				return this.sFileName;
			}
		}
		#endregion

		#region public ArrayList Objects
		public ArrayList Objects
		{
			get
			{
				return this.pneEditor.Objects;
			}
		}
		#endregion

		#region public ArrayList Connections
		public ArrayList Connections
		{
			get
			{
				return this.pneEditor.Connections;
			}
		}
		#endregion

		#region public ArrayList SelectedObjects
		public ArrayList SelectedObjects
		{
			get
			{
				return this.pneEditor.SelectedObjects;
			}
		}
		#endregion

		#region public PetriNetEditor Editor
		public PetriNetEditor Editor
		{
			get
			{
				return this.pneEditor;
			}
		}
		#endregion

		#region public DocumentView View
		public DocumentView View
		{
			get
			{
				if (this.tabMain.SelectedTab == tpPetriNetEditor)
					return DocumentView.Editor;
				else if (this.tabMain.SelectedTab == tpDescription)
					return DocumentView.Description;
				else if (this.tabMain.SelectedTab == tpResponse)
					return DocumentView.Response;
				else
					return DocumentView.None;
			}
		}
		#endregion

		#region public PetriNetType PetriNetType
		[CommonProperties]
		[TimeInvariant]
		[Category("Document")]
		[Description("Type of Petri net.")]
		public PetriNetType PetriNetType
		{
			get
			{
				return this.pntPetriNetType;
			}
			set
			{
				this.pntPetriNetType = value;
				if (this.PropertiesChanged != null)
					PropertiesChanged(this, EventArgs.Empty);
			}
		}
		#endregion

		#region public bool ShowWeight1
		[CommonProperties]
		[TimeInvariant]
		[Category("Drawing options")]
		[Description("If TRUE connections weights of 1 will not be drawn.")]
		public bool ShowWeight1
		{
			get
			{
				return this.pneEditor.ShowWeight1;
			}
			set
			{
				this.pneEditor.ShowWeight1 = value;
			}
		}
		#endregion

		#region public bool TokenGameAnimation
		[CommonProperties]
		[TimeInvariant]
		[Category("Drawing options")]
		[Description("If this is set to true, token moves will animated.")]
		public bool TokenGameAnimation
		{
			get
			{
				return this.bTokenGameAnimation;
			}
			set
			{
				this.bTokenGameAnimation = value;
			}
		}
		#endregion

		#region public int EndTime
		[Category("Simulation options")]
		[Description("Simulation time (in miliseconds).")]
		[CommonProperties]
		public int EndTime
		{
			get
			{
				return this.iEndTime;
			}
			set
			{
				this.iEndTime = value;
			}
		}
		#endregion

                #region public int StepCounter
		[Category("Simulation options")]
                [Description("Current step number (or current time if multiply with interval of discretization).")]
		[CommonProperties]
                public int StepCounter
		{
			get
			{
                                return this.iStepCounter;
			}
			set
			{
                                this.iStepCounter = value;
			}
		}
		#endregion



		#region public int Td
		[Category("Simulation options")]
		[Description("Interval of discretization (in miliseconds).")]
		[CommonProperties]
		public int Td
		{
			get
			{
				return this.iTd;
			}
			set
			{
				this.iTd = value;
			}
		}
		#endregion

		#region public static bool AntiAlias
		public static bool AntiAlias
		{
			get
			{
				return bAntiAlias;
			}
			set
			{
				bAntiAlias = value;
			}
		}
		#endregion

		#region public Simulator Simulator
		public Simulator Simulator
		{
			get
			{
				return this.sSimulator;
			}
			set
			{
				this.sSimulator = value;
			}
		}
		#endregion

		#region public ResponseOptions ResponseOptions
		public ResponseOptions ResponseOptions
		{
			get
			{
				return this.roOptions;
			}
		}
		#endregion

		#region public PetriNetDocumentInstanceCounter InstanceCounter
		public PetriNetDocumentInstanceCounter InstanceCounter
		{
			get
			{
				return this.pndicInstanceCounter;
			}
		}
		#endregion


		#region public ArrayList Places
		public ArrayList Places
		{
			get
			{
				ArrayList alPlaces = new ArrayList();
				foreach(TreeNode tn in this.ObjectsTree.Nodes)
				{
					this.WalkTreePlaces(alPlaces, tn);
				}

				alPlaces.Sort();
				return alPlaces;
			}
		}

		private void WalkTreePlaces(ArrayList al, TreeNode tn)
		{
			if (tn.Tag is Place)
				al.Add(tn.Tag);

			foreach(TreeNode tnn in tn.Nodes)
			{
				this.WalkTreePlaces(al, tnn);
			}
		}

		#endregion

		#region public ArrayList Transitions
		public ArrayList Transitions
		{
			get
			{
				ArrayList alTransitions = new ArrayList();
				foreach(TreeNode tn in this.ObjectsTree.Nodes)
				{
					this.WalkTreeTransitions(alTransitions, tn);
				}

				alTransitions.Sort();
				return alTransitions;
			}
		}

		private void WalkTreeTransitions(ArrayList al, TreeNode tn)
		{
			if (tn.Tag is Transition)
				al.Add(tn.Tag);

			foreach(TreeNode tnn in tn.Nodes)
			{
				this.WalkTreeTransitions(al, tnn);
			}
		}

		#endregion

		#region public ArrayList Subsystems
		public ArrayList Subsystems
		{
			get
			{
				ArrayList alSubsystems = new ArrayList();
				foreach(TreeNode tn in this.ObjectsTree.Nodes)
				{
					this.WalkTreeSubsystems(alSubsystems, tn);
				}

				alSubsystems.Sort();
				return alSubsystems;
			}
		}

		private void WalkTreeSubsystems(ArrayList al, TreeNode tn)
		{
			if (tn.Tag is Subsystem)
				al.Add(tn.Tag);

			foreach(TreeNode tnn in tn.Nodes)
			{
				this.WalkTreeSubsystems(al, tnn);
			}
		}
		#endregion

		#region public ArrayList InputPlaces
		public ArrayList InputPlaces
		{
			get
			{
				ArrayList alInputPlaces = new ArrayList();
				foreach(Place p in this.Places)
				{
					if (p is PlaceInput)
					{
						alInputPlaces.Add(p);
					}
				}

				return alInputPlaces;
			}
		}
		#endregion

		#region public ArrayList OperationPlaces
		public ArrayList OperationPlaces
		{
			get
			{
				ArrayList alOperationPlaces = new ArrayList();
				foreach(Place p in this.Places)
				{
					if (p is PlaceOperation)
					{
						alOperationPlaces.Add(p);
					}
				}

				return alOperationPlaces;
			}
		}
		#endregion

		#region public ArrayList ResourcePlaces
		public ArrayList ResourcePlaces
		{
			get
			{
				ArrayList alResourcePlaces = new ArrayList();
				foreach(Place p in this.Places)
				{
					if (p is PlaceResource)
					{
						alResourcePlaces.Add(p);
					}
				}

				return alResourcePlaces;
			}
		}
		#endregion

		#region public ArrayList ControlPlaces
		public ArrayList ControlPlaces
		{
			get
			{
				ArrayList alControlPlaces = new ArrayList();
				foreach(Place p in this.Places)
				{
					if (p is PlaceControl)
					{
						alControlPlaces.Add(p);
					}
				}

				return alControlPlaces;
			}
		}
		#endregion

		#region public ArrayList OutputPlaces
		public ArrayList OutputPlaces
		{
			get
			{
				ArrayList alOutputPlaces = new ArrayList();
				foreach(Place p in this.Places)
				{
					if (p is PlaceOutput)
					{
						alOutputPlaces.Add(p);
					}
				}

				return alOutputPlaces;
			}
		}
		#endregion

        #region public ArrayList GroupedPlaces
		public ArrayList GroupedPlaces
		{
			get
			{
				ArrayList alPlaces = this.Places;

				ArrayList alGroupedPlaces = new ArrayList();
				foreach(Place p in alPlaces)
				{
					if (p is PlaceInput)
						alGroupedPlaces.Add(p);
				}
				foreach(Place p in alPlaces)
				{
					if (p is PlaceOperation)
						alGroupedPlaces.Add(p);
				}
				foreach(Place p in alPlaces)
				{
					if (p is PlaceResource)
						alGroupedPlaces.Add(p);
				}
				foreach(Place p in alPlaces)
				{
					if (p is PlaceControl)
						alGroupedPlaces.Add(p);
				}
				foreach(Place p in alPlaces)
				{
					if (p is PlaceOutput)
						alGroupedPlaces.Add(p);
				}

                return alGroupedPlaces;
			}
		}
		#endregion

		#region public TreeNode ObjectsTree
		public TreeNode ObjectsTree
		{
			get
			{
				TreeNode tn = new TreeNode(this.ToString());

				foreach(object o in this.Objects)
				{
					this.AddNode(tn, o);
				}
				foreach(object o in this.Connections)
				{
					this.AddNode(tn, o);
				}

				return tn;
			}
		}

		private void AddNode(TreeNode tn, object o)
		{
			TreeNode tnNew = new TreeNode(o.ToString());
			tnNew.Tag = o;

			tn.Nodes.Add(tnNew);

			if (o is Subsystem)
			{
				Subsystem s = (Subsystem)o;
				foreach(object obj in s.Objects)
				{
					this.AddNode(tnNew, obj);
				}
				foreach(object obj in s.Connections)
				{
					this.AddNode(tnNew, obj);
				}

			}
		}

		#endregion

		#region public ArrayList ConnectionsAll
		public ArrayList ConnectionsAll
		{
			get
			{
				ArrayList alConnections = new ArrayList();
				foreach(TreeNode tn in this.ObjectsTree.Nodes)
				{
					this.WalkTreeConnections(alConnections, tn);
				}

				return alConnections;
			}
		}

		private void WalkTreeConnections(ArrayList al, TreeNode tn)
		{
			if (tn.Tag is Connection)
				al.Add(tn.Tag);

			foreach(TreeNode tnn in tn.Nodes)
			{
				this.WalkTreeConnections(al, tnn);
			}
		}

		#endregion

		#region public ArrayList CircularWaitingPlaces
		public ArrayList CircularWaitingPlaces
		{
			get
			{
				Hashtable ht = new Hashtable();

				// Add to hashtable Place with their characters
				char c = '1';
				foreach(Place p in this.ResourcePlaces)
				{
					ht.Add(c, p);
					c++;
				}

				ArrayList alCircularWaitingPlaces = new ArrayList();

				for (int k = 0; k < this.CircularWaiting.Length; k++)
				{
					Matrix m = this.CircularWaiting[k];

					for(int i = 0; i < m.Dimensions.Height; i++)
					{
						for(int j = 0; j < m.Dimensions.Width; j++)
						{
							string s = (string)m[i, j];
							if (s.EndsWith(s[0].ToString()) && s != "0")
							{
								for (int z = 0; z < s.Length; z++)
								{
									if (alCircularWaitingPlaces.Contains(ht[s[z]]) != true)
										alCircularWaitingPlaces.Add(ht[s[z]]);
								}
							}
						}
					}
				}

				return alCircularWaitingPlaces;
			}
		}
		#endregion

		#region public ArrayList Rules
		public ArrayList Rules
		{
			get
			{
				return this.alRules;
			}
		}
		#endregion

		#region public ArrayList ConflictTransitions
		public ArrayList ConflictTransitions
		{
			get
			{
				ArrayList alConflictTransitions = new ArrayList();

				foreach(Transition t in this.Transitions)
				{
					if (t.IsConflicting())
						alConflictTransitions.Add(t);
				}

				return alConflictTransitions;
			}
		}
		#endregion

		#region public ArrayList FireableTransitions
		public ArrayList FireableTransitions
		{
			get
			{
				ArrayList alFireableTransitions = new ArrayList();

				foreach(Transition t in this.Transitions)
				{
					if (t.CanFire())
						alFireableTransitions.Add(t);
				}

				return alFireableTransitions;
			}
		}
		#endregion

		// Events
		public event EventHandler PropertiesChanged;
		public event EventHandler ContentsChanged;
		public event EventHandler SelectedObjectsChanged;
		public event EventHandler PropertiesInspectorChangeRequested;

		// Fields
		private string sFileName = null;
		private PetriNetType pntPetriNetType = PetriNetType.TimeInvariant;
		private int iTd = 50;
                private int iStepCounter = 0;
		private int iEndTime = 20000;
		private ArrayList alRules = new ArrayList();
		private ResponseOptions roOptions;
		private Simulator sSimulator = null;
		private IntMatrix imSimulationResults;
		private TreeView tvObjectsTree = new TreeView();
		private static bool bAntiAlias = true;
		private bool bTokenGameAnimation = false;

		private PetriNetDocumentInstanceCounter pndicInstanceCounter = new PetriNetDocumentInstanceCounter();

		private PetriNetEditor pneEditor;

		private RichTextBox rtbStatistics;

		private System.Windows.Forms.TabControl tabMain;
		private System.Windows.Forms.TabPage tpPetriNetEditor;
		private System.Windows.Forms.TabPage tpDescription;
		private System.Windows.Forms.TabPage tpResponse;
		private System.Windows.Forms.TabControl tabResponse;
		private System.Windows.Forms.TabPage tpMatrixPage;
		private System.Windows.Forms.ListView lvResponseMatrix;
		private System.Windows.Forms.TabPage tpOscillogramPage;
		private System.Windows.Forms.ImageList ilMatrixResponse;
		private System.Windows.Forms.ImageList ilTabImages;
		private System.Windows.Forms.TabPage tpStatisticsPage;
		private System.Windows.Forms.SaveFileDialog sfdExportFile;
		private System.Windows.Forms.ContextMenu cmExport;
		private System.Windows.Forms.MenuItem cmmiExportSpreadsheet;
		private System.ComponentModel.IContainer components;


		#region public PetriNetDocument(string sFileName)
		public PetriNetDocument(string sFileName)
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			this.sFileName = sFileName;

			Panel pnlEditorContainer = new Panel();
			pnlEditorContainer.AutoScroll = true;
			pnlEditorContainer.Dock = DockStyle.Fill;

			this.pneEditor = new PetriNetEditor(this, this.pndicInstanceCounter);
			this.pneEditor.AutoScroll = true;
			this.pneEditor.AutoScrollMinSize = new Size(3000, 3000);
			this.pneEditor.Size = new Size(3000, 3000);
			this.pneEditor.Location = new Point(0, 0);
			this.pneEditor.ContentsChanged += new EventHandler(pneEditor_ContentsChanged);
			this.pneEditor.SelectedObjectsChanged += new EventHandler(pneEditor_SelectedObjectsChanged);
			this.pneEditor.PropertiesChanged += new EventHandler(pneEditor_PropertiesChanged);

			pnlEditorContainer.Controls.Add(pneEditor);
			this.tpPetriNetEditor.Controls.Add(pnlEditorContainer);

			// Create rich text box for statistics
			this.rtbStatistics = new RichTextBox();
			this.rtbStatistics.Dock = DockStyle.Fill;
			tpStatisticsPage.Controls.Add(this.rtbStatistics);

			lvResponseMatrix.Hide();

			this.roOptions = new ResponseOptions(this);
			this.roOptions.PropertiesChanged += new EventHandler(roOptions_PropertiesChanged);
			this.roOptions.SimulationRequired += new EventHandler(roOptions_SimulationRequired);
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

		#region Component Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(PetriNetDocument));
			this.tabMain = new System.Windows.Forms.TabControl();
			this.tpPetriNetEditor = new System.Windows.Forms.TabPage();
			this.tpDescription = new System.Windows.Forms.TabPage();
			this.tpResponse = new System.Windows.Forms.TabPage();
			this.tabResponse = new System.Windows.Forms.TabControl();
			this.tpMatrixPage = new System.Windows.Forms.TabPage();
			this.lvResponseMatrix = new System.Windows.Forms.ListView();
			this.cmExport = new System.Windows.Forms.ContextMenu();
			this.cmmiExportSpreadsheet = new System.Windows.Forms.MenuItem();
			this.ilMatrixResponse = new System.Windows.Forms.ImageList(this.components);
			this.tpOscillogramPage = new System.Windows.Forms.TabPage();
			this.tpStatisticsPage = new System.Windows.Forms.TabPage();
			this.ilTabImages = new System.Windows.Forms.ImageList(this.components);
			this.sfdExportFile = new System.Windows.Forms.SaveFileDialog();
			this.tabMain.SuspendLayout();
			this.tpResponse.SuspendLayout();
			this.tabResponse.SuspendLayout();
			this.tpMatrixPage.SuspendLayout();
			this.SuspendLayout();
			//
			// tabMain
			//
			this.tabMain.Controls.Add(this.tpPetriNetEditor);
			this.tabMain.Controls.Add(this.tpDescription);
			this.tabMain.Controls.Add(this.tpResponse);
			this.tabMain.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabMain.ImageList = this.ilTabImages;
			this.tabMain.ItemSize = new System.Drawing.Size(140, 21);
			this.tabMain.Location = new System.Drawing.Point(0, 0);
			this.tabMain.Multiline = true;
			this.tabMain.Name = "tabMain";
			this.tabMain.SelectedIndex = 0;
			this.tabMain.Size = new System.Drawing.Size(568, 489);
			this.tabMain.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
			this.tabMain.TabIndex = 0;
			this.tabMain.SelectedIndexChanged += new System.EventHandler(this.tabMain_SelectedIndexChanged);
			//
			// tpPetriNetEditor
			//
			this.tpPetriNetEditor.ImageIndex = 0;
			this.tpPetriNetEditor.Location = new System.Drawing.Point(4, 25);
			this.tpPetriNetEditor.Name = "tpPetriNetEditor";
			this.tpPetriNetEditor.Size = new System.Drawing.Size(560, 460);
			this.tpPetriNetEditor.TabIndex = 0;
			this.tpPetriNetEditor.Tag = "Editor";
			this.tpPetriNetEditor.Text = "PetriNet Editor";
			//
			// tpDescription
			//
			this.tpDescription.AutoScroll = true;
			this.tpDescription.ImageIndex = 1;
			this.tpDescription.Location = new System.Drawing.Point(4, 25);
			this.tpDescription.Name = "tpDescription";
			this.tpDescription.Size = new System.Drawing.Size(560, 460);
			this.tpDescription.TabIndex = 1;
			this.tpDescription.Tag = "Description";
			this.tpDescription.Text = "Description";
			//
			// tpResponse
			//
			this.tpResponse.Controls.Add(this.tabResponse);
			this.tpResponse.ImageIndex = 2;
			this.tpResponse.Location = new System.Drawing.Point(4, 25);
			this.tpResponse.Name = "tpResponse";
			this.tpResponse.Size = new System.Drawing.Size(560, 460);
			this.tpResponse.TabIndex = 2;
			this.tpResponse.Tag = "Response";
			this.tpResponse.Text = "Response";
			//
			// tabResponse
			//
			this.tabResponse.Alignment = System.Windows.Forms.TabAlignment.Bottom;
			this.tabResponse.Controls.Add(this.tpMatrixPage);
			this.tabResponse.Controls.Add(this.tpOscillogramPage);
			this.tabResponse.Controls.Add(this.tpStatisticsPage);
			this.tabResponse.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabResponse.ImageList = this.ilTabImages;
			this.tabResponse.ItemSize = new System.Drawing.Size(120, 21);
			this.tabResponse.Location = new System.Drawing.Point(0, 0);
			this.tabResponse.Name = "tabResponse";
			this.tabResponse.SelectedIndex = 0;
			this.tabResponse.Size = new System.Drawing.Size(560, 460);
			this.tabResponse.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
			this.tabResponse.TabIndex = 1;
			this.tabResponse.SelectedIndexChanged += new System.EventHandler(this.tabResponse_SelectedIndexChanged);
			//
			// tpMatrixPage
			//
			this.tpMatrixPage.AutoScroll = true;
			this.tpMatrixPage.Controls.Add(this.lvResponseMatrix);
			this.tpMatrixPage.ImageIndex = 3;
			this.tpMatrixPage.Location = new System.Drawing.Point(4, 4);
			this.tpMatrixPage.Name = "tpMatrixPage";
			this.tpMatrixPage.Size = new System.Drawing.Size(552, 431);
			this.tpMatrixPage.TabIndex = 0;
			this.tpMatrixPage.Text = "Spreadsheet";
			//
			// lvResponseMatrix
			//
			this.lvResponseMatrix.ContextMenu = this.cmExport;
			this.lvResponseMatrix.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lvResponseMatrix.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(238)));
			this.lvResponseMatrix.FullRowSelect = true;
			this.lvResponseMatrix.GridLines = true;
			this.lvResponseMatrix.Location = new System.Drawing.Point(0, 0);
			this.lvResponseMatrix.MultiSelect = false;
			this.lvResponseMatrix.Name = "lvResponseMatrix";
			this.lvResponseMatrix.Size = new System.Drawing.Size(552, 431);
			this.lvResponseMatrix.SmallImageList = this.ilMatrixResponse;
			this.lvResponseMatrix.TabIndex = 0;
			this.lvResponseMatrix.View = System.Windows.Forms.View.Details;
			this.lvResponseMatrix.DoubleClick += new System.EventHandler(this.lvResponseMatrix_DoubleClick);
			//
			// cmExport
			//
			this.cmExport.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					 this.cmmiExportSpreadsheet});
			this.cmExport.Popup += new System.EventHandler(this.cmExport_Popup);
			//
			// cmmiExportSpreadsheet
			//
			this.cmmiExportSpreadsheet.Index = 0;
			this.cmmiExportSpreadsheet.Text = "&Export Spreadsheet";
			this.cmmiExportSpreadsheet.Click += new System.EventHandler(this.cmmiExportSpreadsheet_Click);
			//
			// ilMatrixResponse
			//
			this.ilMatrixResponse.ColorDepth = System.Windows.Forms.ColorDepth.Depth24Bit;
			this.ilMatrixResponse.ImageSize = new System.Drawing.Size(16, 16);
			this.ilMatrixResponse.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilMatrixResponse.ImageStream")));
			this.ilMatrixResponse.TransparentColor = System.Drawing.Color.Transparent;
			//
			// tpOscillogramPage
			//
			this.tpOscillogramPage.AutoScroll = true;
			this.tpOscillogramPage.ImageIndex = 4;
			this.tpOscillogramPage.Location = new System.Drawing.Point(4, 4);
			this.tpOscillogramPage.Name = "tpOscillogramPage";
			this.tpOscillogramPage.Size = new System.Drawing.Size(552, 431);
			this.tpOscillogramPage.TabIndex = 1;
			this.tpOscillogramPage.Text = "Oscillogram";
			this.tpOscillogramPage.Visible = false;
			//
			// tpStatisticsPage
			//
			this.tpStatisticsPage.ImageIndex = 5;
			this.tpStatisticsPage.Location = new System.Drawing.Point(4, 4);
			this.tpStatisticsPage.Name = "tpStatisticsPage";
			this.tpStatisticsPage.Size = new System.Drawing.Size(552, 431);
			this.tpStatisticsPage.TabIndex = 2;
			this.tpStatisticsPage.Text = "Statistics";
			//
			// ilTabImages
			//
			this.ilTabImages.ColorDepth = System.Windows.Forms.ColorDepth.Depth24Bit;
			this.ilTabImages.ImageSize = new System.Drawing.Size(16, 16);
			this.ilTabImages.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilTabImages.ImageStream")));
			this.ilTabImages.TransparentColor = System.Drawing.Color.Transparent;
			//
			// sfdExportFile
			//
			this.sfdExportFile.DefaultExt = "txt";
			this.sfdExportFile.Filter = "Text (Tab delimited) (*.txt)|*.txt";
			this.sfdExportFile.Title = "Export Spreadsheet.";
			//
			// PetriNetDocument
			//
			this.Controls.Add(this.tabMain);
			this.Name = "PetriNetDocument";
			this.Size = new System.Drawing.Size(568, 489);
			this.tabMain.ResumeLayout(false);
			this.tpResponse.ResumeLayout(false);
			this.tabResponse.ResumeLayout(false);
			this.tpMatrixPage.ResumeLayout(false);
			this.ResumeLayout(false);

            this.pyCode = String.Empty;

		}
		#endregion


		#region private void pneEditor_ContentsChanged(object sender, EventArgs e)
		private void pneEditor_ContentsChanged(object sender, EventArgs e)
		{
			if (this.ContentsChanged != null)
				this.ContentsChanged(sender, e);
		}
		#endregion

		#region private void pneEditor_SelectedObjectsChanged(object sender, EventArgs e)
		private void pneEditor_SelectedObjectsChanged(object sender, EventArgs e)
		{
			if (this.SelectedObjectsChanged != null)
				this.SelectedObjectsChanged(this, e);
		}
		#endregion

		#region private void pneEditor_PropertiesChanged(object sender, EventArgs e)
		private void pneEditor_PropertiesChanged(object sender, EventArgs e)
		{
			if (this.PropertiesChanged != null)
				this.PropertiesChanged(sender, e);
		}
		#endregion

		#region public void PerformActivation()
		public void PerformActivation()
		{
			this.pneEditor.PerformActivation();
		}
		#endregion

        #region public void SaveFileCpp(string sFileName)
        public void SaveFileCpp(string sFileName)
        {
                        StreamWriter fs = new StreamWriter (sFileName);
                        //string line;

                        fs.Write("/*-------------------------------------------------------------------------------------------------------------------------------- \n");
                        fs.Write(" C/CPP file generated with Petri.NET Simulator \n");
                        fs.Write("\n");
                        fs.Write(" How to use it:\n");
                        fs.Write(" Include this file somewhere in your project.\n");
                        fs.Write(" Before you include this file, you must define two preprocessor macros: \n");
                        fs.Write("\n");
                        fs.Write("\tppDEFINE_MATRIX             - Define a global matrix variable\n");
                        fs.Write("\tppSET_MATRIX_VALUE(n,x,y,v) - Assign a value to specific matrix element\n");
                        fs.Write("\n");
                        fs.Write(" For example\n");
                        fs.Write("\n");
                        fs.Write(" #define ppDEFINE_MATRIX(n)             YourMatrixClass n\n");
                        fs.Write(" #define ppSET_MATRIX_VALUE(n,x,y,v)    n[x][y] = v\n");
                        fs.Write("\n");
                        fs.Write(" You also must call set_petri_net_matrices() function from somewhere\n");
                        fs.Write(" if you want to fill defined matrices with actual data\n");
                        fs.Write("--------------------------------------------------------------------------------------------------------------------------------*/ \n");
                        fs.Write("\n\n");

                        fs.Write("ppDEFINE_MATRIX( Fv );\n");
                        fs.Write("ppDEFINE_MATRIX( Fr );\n");
                        fs.Write("ppDEFINE_MATRIX( Sv );\n");
                        fs.Write("ppDEFINE_MATRIX( Sr );\n");
                        fs.Write("ppDEFINE_MATRIX( Fu );\n");
                        fs.Write("ppDEFINE_MATRIX( Sy );\n");
                        fs.Write("ppDEFINE_MATRIX( Fy );\n");
                        fs.Write("ppDEFINE_MATRIX( Su );\n");
                        fs.Write("ppDEFINE_MATRIX( Fd );\n");
                        fs.Write("ppDEFINE_MATRIX( Sd );\n");
                        fs.Write("ppDEFINE_MATRIX( F );\n");
                        fs.Write("ppDEFINE_MATRIX( S );\n");
                        fs.Write("ppDEFINE_MATRIX( W );\n");
                        fs.Write("ppDEFINE_MATRIX( Gw );\n");
                        fs.Write("ppDEFINE_MATRIX( Tv0 );\n");
                        fs.Write("ppDEFINE_MATRIX( Tr0 );\n");
                        fs.Write("ppDEFINE_MATRIX( Td0 );\n");
                        fs.Write("ppDEFINE_MATRIX( Ty0 );\n");
                        fs.Write("\n\n");

                        fs.Write("void set_petri_net_matrices(void)\n");
                        fs.Write("{\n");
                        fs.Write("\t/*Fv - Job sequencing matrix*/\n");
                        fs.Write(ExportMatrixCpp("Fv", Fv));
                        fs.Write("\n\n");

                        fs.Write("\t/*Fr - Resource requirement matrix */\n");
                        fs.Write(ExportMatrixCpp("Fr", Fr));
                        fs.Write("\n\n");

                        fs.Write("\t/*Sv - Job start matrix */\n");
                        fs.Write(ExportMatrixCpp("Sv", Sv));
                        fs.Write("\n\n");

                        fs.Write("\t/*Sr - Resource release matrix */\n");
                        fs.Write(ExportMatrixCpp("Sr", Sr));
                        fs.Write("\n\n");

                        fs.Write("\t/*Fu - Input matrix */\n");
                        fs.Write(ExportMatrixCpp("Fu", Fu));
                        fs.Write("\n\n");

                        fs.Write("\t/*Sy - Output matrix */\n");
                        fs.Write(ExportMatrixCpp("Sy", Sy));
                        fs.Write("\n\n");

                        fs.Write("\t/*Fy - Null matrix (used just as helper matrix to construct F matrix) */\n");
                        fs.Write(ExportMatrixCpp("Fy", Fy));
                        fs.Write("\n\n");

                        fs.Write("\t/*Su - Null matrix (used just as helper matrix to construct S matrix) */\n");
                        fs.Write(ExportMatrixCpp("Su", Su));
                        fs.Write("\n\n");

                        fs.Write("\t/*Fd and Sd - Driving related matrix */\n");
                        fs.Write(ExportMatrixCpp("Fd", Fd));
                        fs.Write(ExportMatrixCpp("Sd", Sd));
                        fs.Write("\n\n");

                        fs.Write(ExportMatrixCpp("F", F));
                        fs.Write(ExportMatrixCpp("S", S));
                        fs.Write("\n\n");

                        fs.Write("\t/*W - Matrix W = S'-F */\n");
                        fs.Write(ExportMatrixCpp("W", W));
                        fs.Write("\n\n");

                        fs.Write("\t/*Gw = Resource wait matrix */\n");
                        fs.Write(ExportMatrixCpp("Gw", Gw));
                        fs.Write("\n\n");

                        fs.Write("\t/*T = Timing */\n");
                        fs.Write(ExportMatrixCpp("Tv0", Tv0));
                        fs.Write(ExportMatrixCpp("Tr0", Tr0));
                        fs.Write(ExportMatrixCpp("Td0", Td0));
                        fs.Write(ExportMatrixCpp("Ty0", Ty0));

                        fs.Write("}\n\n");


                        fs.Write("/*String matrices */\n");
                        if (this.OperationPlaces.Count != 0)
                        {
                            fs.Write("LPCSTR JobsNames [] = { ");
                            foreach (Place o in this.OperationPlaces)
                                fs.Write("\"" + o.GetShortString() + "\", ");
                            fs.Write(" NULL };\n");
                        }

                        if (this.ResourcePlaces.Count != 0)
                        {
                            fs.Write("LPCSTR ResourceNames [] = { ");
                            foreach (Place o in this.ResourcePlaces)
                                fs.Write("\"" + o.GetShortString() + "\", ");
                            fs.Write(" NULL };\n");
                        }

                        if (this.InputPlaces.Count != 0)
                        {
                            fs.Write("LPCSTR InputNames [] = {  ");
                            foreach (Place o in this.InputPlaces)
                                fs.Write("\"" + o.GetShortString() + "\", ");
                            fs.Write(" NULL };\n");
                        }

                        if(this.OutputPlaces.Count != 0)
                        {
                            fs.Write("LPCSTR OutputNames[] = { ");
                            foreach (Place o in this.OutputPlaces)
                                fs.Write("\"" + o.GetShortString() + "\", ");
                            fs.Write(" NULL };\n");
                        }
                        if (this.ControlPlaces.Count != 0)
                        {
                            fs.Write("LPCSTR ControlNames[] = { ");
                            foreach (Place o in this.ControlPlaces)
                                fs.Write("\"" + o.GetShortString() + "\", ");
                            fs.Write(" NULL };\n");
                        }
                        if (this.Transitions.Count != 0)
                        {
                            fs.Write("LPCSTR TransitionNames[] = { ");
                            foreach (Transition o in this.Transitions)
                                fs.Write("\"" + o.GetShortString() + "\", ");
                            fs.Write(" NULL };\n");
                        }


            fs.Close();
        }
		#endregion

        #region public void SaveFileMatrix(string sFileName)
        public void SaveFileMatrix(string sFileName)
		{
                        StreamWriter fs = new StreamWriter (sFileName);
                        //string line;

                        fs.Write("%---------------------------------------------------------------- \n");
                        fs.Write("% M file generated with Petri.NET Simulator \n");
                        fs.Write("%---------------------------------------------------------------- \n");
                        fs.Write("%\n");
                        fs.Write("%\n");
                        fs.Write("%Fv - Job sequencing matrix\n");
                        fs.Write("Fv = " + ExportMatrix(Fv));
                        fs.Write("%\n");
                        fs.Write("%\n");

                        fs.Write("%Fr - Resource requirement matrix \n");
                        fs.Write("Fr = " + ExportMatrix(Fr));
                        fs.Write("%\n");
                        fs.Write("%\n");

                        fs.Write("%Sv - Job start matrix\n");
                        fs.Write("Sv = " + ExportMatrix(Sv));
                        fs.Write("%\n");
                        fs.Write("%\n");

                        fs.Write("%Sr - Resource release matrix\n");
                        fs.Write("Sr = " + ExportMatrix(Sr));
                        fs.Write("%\n");
                        fs.Write("%\n");

                        fs.Write("%Fu - Input matrix\n");
                        fs.Write("Fu = " + ExportMatrix(Fu));
                        fs.Write("%\n");
                        fs.Write("%\n");

                        fs.Write("%Sy - Output matrix\n");
                        fs.Write("Sy = " + ExportMatrix(Sy));
                        fs.Write("%\n");
                        fs.Write("%\n");

                        fs.Write("%Fy - Null matrix (used just as helper matrix to construct F matrix)\n");
                        fs.Write("Fy = " + ExportMatrix(Fy));
                        fs.Write("%\n");
                        fs.Write("%\n");

                        fs.Write("%Su - Null matrix (used just as helper matrix to construct S matrix)\n");
                        fs.Write("Su = " + ExportMatrix(Su));
                        fs.Write("%\n");
                        fs.Write("%\n");

                        fs.Write("%Fd and Sd - Driving related matrix\n");
                        fs.Write("Fd = " + ExportMatrix(Fd));
                        fs.Write("Sd = " + ExportMatrix(Sd));
                        fs.Write("%\n");
                        fs.Write("%\n");

                        fs.Write("F = " + ExportMatrix(F));
                        fs.Write("S = " + ExportMatrix(S));
                        fs.Write("%\n");
                        fs.Write("%\n");

                        fs.Write("%W - Matrix W = S'-F\n");
                        fs.Write("W = " + ExportMatrix(W));
                        fs.Write("%\n");
                        fs.Write("%\n");

                        fs.Write("%Gw = Resource wait matrix\n");
                        fs.Write("Gw = " + ExportMatrix(Gw));
                        fs.Write("%\n");
                        fs.Write("%\n");

                        fs.Write("%T = Timing\n");
                        fs.Write("Tv0 = " + ExportMatrix(Tv0));
                        fs.Write("Tr0 = " + ExportMatrix(Tr0));
                        fs.Write("Td0 = " + ExportMatrix(Td0));
                        fs.Write("Ty0 = " + ExportMatrix(Ty0));


                        fs.Write("%String matrices\n");
                        fs.Write("%\n");
                        if (this.OperationPlaces.Count != 0)
                        {
                            fs.Write("%JobsNames = [ ");
                            foreach (Place o in this.OperationPlaces)
                                fs.Write("'" + o.GetShortString() + "'; ");
                            fs.Write(" ];\n");
                        }

                        if (this.ResourcePlaces.Count != 0)
                        {
                            fs.Write("%ResourceNames = [ ");
                            foreach (Place o in this.ResourcePlaces)
                                fs.Write("'" + o.GetShortString() + "'; ");
                            fs.Write(" ];\n");
                        }

                        if (this.InputPlaces.Count != 0)
                        {
                            fs.Write("%InputNames = [ ");
                            foreach (Place o in this.InputPlaces)
                                fs.Write("'" + o.GetShortString() + "'; ");
                            fs.Write(" ];\n");
                        }

                        if(this.OutputPlaces.Count != 0)
                        {
                            fs.Write("%OutputNames = [ ");
                            foreach (Place o in this.OutputPlaces)
                                fs.Write("'" + o.GetShortString() + "'; ");
                            fs.Write(" ];\n");
                        }
                        if (this.ControlPlaces.Count != 0)
                        {
                            fs.Write("%ControlNames = [ ");
                            foreach (Place o in this.ControlPlaces)
                                fs.Write("'" + o.GetShortString() + "'; ");
                            fs.Write(" ];\n");
                        }
                        if (this.Transitions.Count != 0)
                        {
                            fs.Write("%TransitionNames = [ ");
                            foreach (Transition o in this.Transitions)
                                fs.Write("'" + o.GetShortString() + "'; ");
                            fs.Write(" ];\n");
                        }


            fs.Close();
                }
		#endregion


        #region public void SaveFileXML(string sFileName)
        public void SaveFileXML(string sFileName)
		{
            StreamWriter fs = new StreamWriter (sFileName);
            fs.Write("<pnml xmlns=\"http://www.informatik.hu-berlin.de/top/pnml/ptNetb\">\n<net>\n");
            foreach(Transition o in this.Transitions)
            {
                // Save all objects..
                fs.Write(o.GetXMLString ());
            }

            foreach (Place o in this.Places)
            {
                // Save all objects..
                fs.Write(o.GetXMLString());
                //fs.Write("<place id=\"name\" class=\"xxx\">\n");
                //fs.Write("</place>\n");
            }

            foreach (Connection cn in this.Connections)
            {
                // Save all connections
                fs.Write(cn.GetXMLString ());
            }
            fs.Write("</net>\n</pnml>\n");
            fs.Close();
        }
        #endregion

        #region public void SaveFileXML(string sFileName)
        public void LoadFileXML(string sFileName)
        {
            this.SuspendLayout();
            StreamReader sr = new StreamReader (sFileName);
            string xml_content = sr.ReadToEnd();
            sr.Close();
            xml_content = xml_content.Replace(" xmlns=\"http://www.informatik.hu-berlin.de/top/pnml/ptNetb\"", "");
            string tmpname = Path.GetTempFileName();
            StreamWriter sw = new StreamWriter(tmpname);
            sw.Write(xml_content);
            sw.Close();
            try
            {
                XPathDocument doc = new XPathDocument(tmpname);
                XPathNavigator nav = doc.CreateNavigator();
                //XPathExpression expr;
                //XPathNodeIterator iterator;
                int i;

                int num_of_places = Convert.ToInt32(nav.Evaluate("count(.//place)").ToString());
                int num_of_trans = Convert.ToInt32(nav.Evaluate("count(.//transition)").ToString());
                int num_of_arcs = Convert.ToInt32(nav.Evaluate("count(.//arc)").ToString());


                string[]  place_transIDs = new string[num_of_places + num_of_trans];
                ArrayList place_transPtrs = new ArrayList ();
                int counter = 0;


                Hashtable ht = new Hashtable();

                for (i = 0; i < num_of_places; i++)
                {
                    String expr1 = "string(/pnml/net/place[" + (i + 1) + "]/@id)";
                    String expr2 = "string(/pnml/net/place[" + (i + 1) + "]/name/graphics/position/@x)";
                    String expr3 = "string(/pnml/net/place[" + (i + 1) + "]/name/graphics/position/@y)";
                    String expr4 = "string(/pnml/net/place[" + (i + 1) + "]/name/text)";
                    String expr5 = "string(/pnml/net/place[" + (i + 1) + "]/initialMarking/text)";
                    String expr6 = "string(/pnml/net/place[" + (i + 1) + "]/toolspecific[@tool='PNE']/type/text)";

                    Console.WriteLine("Place ID={0}", nav.Evaluate(expr1).ToString());
                    Console.WriteLine("      X={0}", nav.Evaluate(expr2).ToString());
                    Console.WriteLine("      Y={0}", nav.Evaluate(expr3).ToString());
                    Console.WriteLine("      Txt={0}", nav.Evaluate(expr4).ToString());
                    Console.WriteLine("      Marking={0}", nav.Evaluate(expr5).ToString());

                    string object_type = nav.Evaluate(expr6).ToString();
                    Place po = null;
                    switch (object_type)
                    {
                        case "I": po = (Place)new PlaceInput (); break;
                        case "R": po = (Place)new PlaceResource (); break;
                        case "J": po = (Place)new PlaceOperation(); break;
                        case "C": po = (Place)new PlaceControl (); break;
                        default:
                        case "O": po = (Place)new PlaceOutput(); break;
                    }
                    po.Parent = this.Editor;
                    String str   = nav.Evaluate(expr1).ToString();
                    place_transIDs[counter++] = po.NameID = po.Name = str;
                    place_transPtrs.Add (po);
                    po.Index = i+1;
                    po.Location = new Point(Convert.ToInt32(nav.Evaluate(expr2).ToString()),
                                            Convert.ToInt32(nav.Evaluate(expr3).ToString())); // ;
                    po.Tokens     = Convert.ToInt32(nav.Evaluate(expr5).ToString());
                    ht.Add(po.GetType().FullName + po.Index.ToString(), po);
                    this.Editor.AddDeserializedControl(po);
                }
                for (i = 0; i < num_of_trans; i++)
                {
                    String expr1 = "string(/pnml/net/transition[" + (i + 1) + "]/@id)";
                    String expr2 = "string(/pnml/net/transition[" + (i + 1) + "]/graphics/position/@x)";
                    String expr3 = "string(/pnml/net/transition[" + (i + 1) + "]/graphics/position/@y)";

                    Console.WriteLine("Trans ID={0}", nav.Evaluate(expr1).ToString());
                    Console.WriteLine("      X={0}", nav.Evaluate(expr2).ToString());
                    Console.WriteLine("      Y={0}", nav.Evaluate(expr3).ToString());

                    Transition tr = new Transition();
                    tr.Parent   = this.Editor;
                    tr.Name     = nav.Evaluate(expr1).ToString();
                    place_transIDs[counter++] = tr.Name;
                    place_transPtrs.Add (tr);
                    tr.Index = i+1;
                    tr.Location = new Point(Convert.ToInt32(nav.Evaluate(expr2).ToString()),
                                            Convert.ToInt32(nav.Evaluate(expr3).ToString())); // ;
                    ht.Add(tr.GetType().FullName + tr.Index.ToString(), tr);
                    this.Editor.AddDeserializedControl(tr);


                }
                for (i = 0; i < num_of_arcs; i++)
                {
                    String expr1 = "string(/pnml/net/arc[" + (i + 1) + "]/@id)";
                    String expr2 = "string(/pnml/net/arc[" + (i + 1) + "]/@source)";
                    String expr3 = "string(/pnml/net/arc[" + (i + 1) + "]/@target)";
                    String expr4 = "string(/pnml/net/arc[" + (i + 1) + "]/inscription/text)";

                    Console.WriteLine("Arc   ID={0}", nav.Evaluate(expr1).ToString());
                    Console.WriteLine("      source={0}", nav.Evaluate(expr2).ToString());
                    Console.WriteLine("      target={0}", nav.Evaluate(expr3).ToString());
                    Console.WriteLine("      tezina={0}", nav.Evaluate(expr4).ToString());

                    string sourceStr = nav.Evaluate(expr2).ToString();
                    string destStr = nav.Evaluate(expr3).ToString();
                    ConnectableControl  srcCC = null;
                    ConnectableControl destCC = null;

                    for (int index = 0; index < num_of_places + num_of_trans; index++)
                    {
                        if (sourceStr == place_transIDs[index])
                            srcCC = (ConnectableControl)place_transPtrs[index];
                        if (destStr == place_transIDs[index])
                            destCC = (ConnectableControl)place_transPtrs[index];
                    }
                    Point startP = new Point(70, 35);
                    Point endP = new Point(0, 35);

                    if(srcCC is PlaceResource) startP = new Point(0, 35);
                    if(destCC is PlaceResource) endP = new Point(70, 35);

                    Connection cn = new Connection(srcCC, destCC,
                                                   0, 0,
                                                   startP, endP,
                                                   Convert.ToInt32(nav.Evaluate(expr4).ToString()));

                    cn.sFrom = srcCC.GetType().FullName + srcCC.Index.ToString();
                    cn.sTo = destCC.GetType().FullName + destCC.Index.ToString();
                    cn.RestoreLinks(ht);
                    this.Editor.AddDeserializedConnection(cn);
                }

                this.roOptions.RestoreReferences(this, ht);
                this.roOptions.PropertiesChanged += new EventHandler(roOptions_PropertiesChanged);
                this.roOptions.SimulationRequired += new EventHandler(roOptions_SimulationRequired);
            }
            catch { }

            FileInfo fi = new FileInfo(tmpname);
            fi.Delete();


            this.ResumeLayout();

        }
        #endregion




        public string ExportMatrix(IntMatrix x)
        {
            string line = "0;\n";
            if(x.Dimensions.Height > 0 && x.Dimensions.Width > 0)
            {
                line = "[ ";
                for (int i = 0; i < x.Dimensions.Height; i++)   // Kolone
                {
                        for(int j=0;j< x.Dimensions.Width ;j++)    // Redovi
                        {
                        line += x[i, j] + " ";
                        }
                        line += "; ";
                }
                line += "];\n";
            }
            return line;
        }



        public string ExportMatrixCpp(string name, IntMatrix x)
        {
            string line = "\n";
            if(x.Dimensions.Height > 0 && x.Dimensions.Width > 0)
            {
                for (int i = 0; i < x.Dimensions.Height; i++)   // Kolone
                {
                        for(int j=0;j< x.Dimensions.Width ;j++)    // Redovi
                                line +=  "\tppSET_MATRIX_VALUE("+name+"," + i + "," + j + "," + x[i, j] +");\n";
                }
            }
            return line;
        }




        #region public void SaveFile(string sFileName)
        public void SaveFile(string sFileName)
		{
#if !DEMO
			// Save all data to file

			// Save Header
			ArrayList al = new ArrayList();
			al.Add(this.PetriNetType);
			al.Add(this.Objects.Count);
			al.Add(this.Connections.Count);
			al.Add(this.Editor.Zoom);
			al.Add(this.ShowWeight1);
			al.Add(this.EndTime);
			al.Add(this.Td);
			al.Add(this.alRules);
			al.Add(this.roOptions);

			FileStream fs = File.Create(sFileName);
			BinaryFormatter bf = new BinaryFormatter();

			bf.Serialize(fs, al);

			// Save all objects
			foreach(object o in this.Objects)
			{
				if (o is ISerializable)
				{
					bf.Serialize(fs, o);
				}
			}

			// Save all connections
			foreach(Connection cn in this.Connections)
			{
				bf.Serialize(fs, cn);
			}

            bf.Serialize(fs, pyCode);

			fs.Close();

			this.sFileName = sFileName;

			if (this.ContentsChanged != null)
				this.ContentsChanged(this, EventArgs.Empty);
#endif
		}
		#endregion

		#region public void OpenFile(string sFileName)
		public void OpenFile(string sFileName)
		{
			this.sFileName = sFileName;

			this.SuspendLayout();

			//Load all data from file

			FileStream fs = File.OpenRead(sFileName);
			BinaryFormatter bf = new BinaryFormatter();
            
			ArrayList al = (ArrayList)bf.Deserialize(fs);

			// Restore PetriNetType for this object
			this.pntPetriNetType = (PetriNetType)al[0];

			int iObjectsCount = (int)al[1];
			int iConnectionsCount = (int)al[2];

			this.pneEditor.Zoom = (float)al[3];
			this.ShowWeight1 = (bool)al[4];
			this.EndTime = (int)al[5];
			this.Td = (int)al[6];
			this.alRules = (ArrayList)al[7];
			this.roOptions = (ResponseOptions)al[8];

			Hashtable ht = new Hashtable();

			for (int i = 0; i < iObjectsCount; i++)
			{
				object o = bf.Deserialize(fs);

				if (o is ConnectableControl)
				{
					ConnectableControl cc = (ConnectableControl)o;
					ht.Add(cc.GetType().FullName + cc.Index.ToString(), cc);
					this.Editor.AddDeserializedControl(cc);
				}
				else if (o is DescriptionLabel)
				{
					DescriptionLabel dl = (DescriptionLabel)o;
					this.Editor.AddDeserializedControl(dl);
				}
			}

			// Deserialize connections
			for (int i = 1; i <= iConnectionsCount; i++)
			{
				Connection cn = (Connection)bf.Deserialize(fs);
				cn.RestoreLinks(ht);
				this.Editor.AddDeserializedConnection(cn);
			}

			// Restore references for Resource Places
			foreach(PlaceResource pr in this.ResourcePlaces)
			{
				pr.RestoreReleaseTimes();
			}

			// Restore ResponseOptions references and events
			this.roOptions.RestoreReferences(this, ht);
			this.roOptions.PropertiesChanged += new EventHandler(roOptions_PropertiesChanged);
			this.roOptions.SimulationRequired += new EventHandler(roOptions_SimulationRequired);

            try
            {
                pyCode = (string)bf.Deserialize(fs);
            }
            catch
            { 
                // Could be old format of file... 
                pyCode = String.Empty; 
            }

			fs.Close();

			this.ResumeLayout();

			if (this.ContentsChanged != null)
				this.ContentsChanged(this, EventArgs.Empty);
		}
		#endregion


		#region public bool ValidateNameID(string sNameID)
		public bool ValidateNameID(string sNameID)
		{
			bool bCorrect = true;
			foreach(Place p in this.Places)
			{
				if (p.NameID == sNameID)
				{
					bCorrect = false;
					break;
				}
			}

			return bCorrect;
		}
		#endregion

		#region private void tabMain_SelectedIndexChanged(object sender, System.EventArgs e)
		private void tabMain_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			// Adjust properties window
			if (this.PropertiesInspectorChangeRequested != null)
				this.PropertiesInspectorChangeRequested(tabMain.SelectedTab, EventArgs.Empty);

			if(tabMain.SelectedTab == tpPetriNetEditor)
			{

			}

			#region else if(tabMain.SelectedTab == tpDescription)
			else if(tabMain.SelectedTab == tpDescription)
			{
#if !STUDENT
				tpDescription.Controls.Clear();

				tpDescription.SuspendLayout();

				MatrixViewer mwFu = new MatrixViewer("Fu", this.Fu, this.InputPlaces, this.Transitions);
				mwFu.Location = new Point(10, 10);
				tpDescription.Controls.Add(mwFu);
				MatrixViewer mwFv = new MatrixViewer("Fv", this.Fv, this.OperationPlaces, this.Transitions);
				mwFv.Location = new Point(mwFu.Location.X + mwFu.Size.Width, mwFu.Location.Y);
				tpDescription.Controls.Add(mwFv);
				MatrixViewer mwFr = new MatrixViewer("Fr", this.Fr, this.ResourcePlaces, this.Transitions);
				mwFr.Location = new Point(mwFu.Location.X + mwFu.Size.Width + mwFv.Size.Width, mwFu.Location.Y);
				tpDescription.Controls.Add(mwFr);
				MatrixViewer mwFd = new MatrixViewer("Fd", this.Fd, this.ControlPlaces, this.Transitions);
				mwFd.Location = new Point(mwFu.Location.X + mwFu.Size.Width + mwFv.Size.Width + mwFr.Size.Width, mwFu.Location.Y);
				tpDescription.Controls.Add(mwFd);
				MatrixViewer mwFy = new MatrixViewer("Fy", this.Fy, this.OutputPlaces, this.Transitions);
				mwFy.Location = new Point(mwFu.Location.X + mwFu.Size.Width + mwFv.Size.Width + mwFr.Size.Width + mwFd.Size.Width, mwFu.Location.Y);
				tpDescription.Controls.Add(mwFy);

				Panel pnlSeparator1 = new Panel();
				pnlSeparator1.Location = new Point(mwFu.Location.X, mwFu.Location.Y + mwFu.Size.Height + 20);
				pnlSeparator1.Size = new Size(mwFu.Size.Width + mwFv.Size.Width + mwFr.Size.Width + mwFd.Size.Width + mwFy.Width, 4);
				pnlSeparator1.BorderStyle = BorderStyle.Fixed3D;
				tpDescription.Controls.Add(pnlSeparator1);

				MatrixViewer mwSu = new MatrixViewer("Su", this.Su, this.Transitions, this.InputPlaces);
				mwSu.Location = new Point(mwFu.Location.X, mwFu.Location.Y + mwFu.Height + 15);
				tpDescription.Controls.Add(mwSu);
				MatrixViewer mwSv = new MatrixViewer("Sv", this.Sv, this.Transitions, this.OperationPlaces);
				mwSv.Location = new Point(mwFu.Location.X, mwSu.Location.Y + mwSu.Size.Height);
				tpDescription.Controls.Add(mwSv);
				MatrixViewer mwSr = new MatrixViewer("Sr", this.Sr, this.Transitions, this.ResourcePlaces);
				mwSr.Location = new Point(mwFu.Location.X, mwSu.Location.Y + mwSu.Size.Height + mwSv.Size.Height);
				tpDescription.Controls.Add(mwSr);
				MatrixViewer mwSd = new MatrixViewer("Sd", this.Sd, this.Transitions, this.ControlPlaces);
				mwSd.Location = new Point(mwFu.Location.X, mwSu.Location.Y + mwSu.Size.Height + mwSv.Size.Height + mwSr.Size.Height);
				tpDescription.Controls.Add(mwSd);
				MatrixViewer mwSy = new MatrixViewer("Sy", this.Sy, this.Transitions, this.OutputPlaces);
				mwSy.Location = new Point(mwFu.Location.X, mwSu.Location.Y + mwSu.Size.Height + mwSv.Size.Height + mwSr.Size.Height + mwSd.Size.Height);
				tpDescription.Controls.Add(mwSy);

				Panel pnlSeparator2 = new Panel();
				pnlSeparator2.Location = new Point(mwSu.Location.X + mwSu.Width + 20, mwSu.Location.Y + 10);
				pnlSeparator2.Size = new Size(4, mwSu.Size.Height + mwSv.Size.Height + mwSr.Size.Height + mwSd.Size.Height + mwSy.Height);
				pnlSeparator2.BorderStyle = BorderStyle.Fixed3D;
				tpDescription.Controls.Add(pnlSeparator2);

				MatrixViewer mwGw = new MatrixViewer("Gw", this.Gw, this.ResourcePlaces, this.ResourcePlaces);
				mwGw.Location = new Point(mwSu.Location.X + mwSu.Width + 40, mwSu.Location.Y + 20);
				tpDescription.Controls.Add(mwGw);

				//For CircularWaiting
				int iXPosition = mwGw.Location.X;

				for (int i = 0; i < this.CircularWaiting.Length; i++)
				{
					MatrixViewer mwCw = new MatrixViewer("Z" + ((int)(i+1)).ToString() , this.CircularWaiting[i], this.ResourcePlaces, this.ResourcePlaces);
					mwCw.Location = new Point(iXPosition, mwGw.Location.Y + mwGw.Height + 20);
					tpDescription.Controls.Add(mwCw);

					iXPosition += mwCw.Width + 40;
				}

				Panel pnlSeparator3 = new Panel();
				pnlSeparator3.Location = new Point(mwSu.Location.X + mwSu.Width + 20,  mwGw.Location.Y + 2 * mwGw.Height + 40);
				pnlSeparator3.Size = new Size(mwFu.Size.Width + mwFv.Size.Width + mwFr.Size.Width + mwFd.Size.Width + mwFy.Width, 4);
				pnlSeparator3.BorderStyle = BorderStyle.Fixed3D;
				tpDescription.Controls.Add(pnlSeparator3);

				MatrixViewer mwTv = new MatrixViewer("Tv0", this.Tv0, this.Transitions, this.OperationPlaces);
				mwTv.Location = new Point(pnlSeparator3.Location.X + 20, pnlSeparator3.Location.Y);
				tpDescription.Controls.Add(mwTv);

				MatrixViewer mwTr = new MatrixViewer("Tr0", this.Tr0, this.Transitions, this.ResourcePlaces);
				mwTr.Location = new Point(mwTv.Location.X + mwTv.Width + 20, pnlSeparator3.Location.Y);
				tpDescription.Controls.Add(mwTr);

				MatrixViewer mwTd = new MatrixViewer("Td0", this.Td0, this.Transitions, this.ControlPlaces);
				mwTd.Location = new Point(mwTr.Location.X + mwTr.Width + 20, pnlSeparator3.Location.Y);
				tpDescription.Controls.Add(mwTd);

				MatrixViewer mwTy = new MatrixViewer("Ty0", this.Ty0, this.Transitions, this.OutputPlaces);
				mwTy.Location = new Point(mwTd.Location.X + mwTd.Width + 20, pnlSeparator3.Location.Y);
				tpDescription.Controls.Add(mwTy);

				tpDescription.ResumeLayout();
#else
					Label lblWarning = new Label();
					lblWarning.Text = "This is a limited Demo Edition. This feature is not available";
					lblWarning.Size = tpDescription.Size;
					lblWarning.TextAlign = ContentAlignment.MiddleCenter;
					tpDescription.Controls.Add(lblWarning);
#endif
			}
			#endregion

			#region else if (tabMain.SelectedTab == tpResponse)
			else if (tabMain.SelectedTab == tpResponse)
			{
				if (tabResponse.SelectedTab == tpMatrixPage)
				{
					lvResponseMatrix.Clear();
				}
				this.Refresh();

				// Simulate
				this.Cursor = Cursors.WaitCursor;
				this.imSimulationResults = this.sSimulator.SimulateToMatrix(roOptions.EndTime);
				this.Cursor = Cursors.Default;

				tabResponse_SelectedIndexChanged(tabMain, new EventArgs());
			}
			#endregion

		}
		#endregion

		#region private void tabResponse_SelectedIndexChanged(object sender, System.EventArgs e)
		private void tabResponse_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			#region if (tabResponse.SelectedTab == tpMatrixPage)
			if (tabResponse.SelectedTab == tpMatrixPage)
			{
				tpMatrixPage.Controls.Clear();

				lvResponseMatrix.Clear();

				ArrayList alGroupedPlaces = this.GroupedPlaces;
				ArrayList alInputPlaces = this.InputPlaces;
				ArrayList alOperationPlaces = this.OperationPlaces;


				Place[] aResponsePlaces = this.roOptions.Places;

				if (this.roOptions.Places.Length != 0)
				{
					lvResponseMatrix.Show();
					tpMatrixPage.Controls.Add(lvResponseMatrix);

					lvResponseMatrix.BeginUpdate();
					this.Cursor = Cursors.WaitCursor;

					// Set columns
					ColumnHeader ch0 = new ColumnHeader();
					ch0.Text = "No";
					ch0.Width = 60;
					lvResponseMatrix.Columns.Add(ch0);

					// Create ArrayList from Array
					ArrayList al = new ArrayList();
					for (int i = 0; i < aResponsePlaces.Length; i++)
					{
						al.Add(aResponsePlaces[i]);
					}

					foreach(Place p in alGroupedPlaces)
					{
						if (al.Contains(p))
						{
							ColumnHeader ch = new ColumnHeader();
							ch.Text = (p.NameID != "") ? p.NameID :  "(P" + p.Index + ")";
							ch.Width = 60;
							ch.TextAlign = HorizontalAlignment.Center;
							lvResponseMatrix.Columns.Add(ch);
						}
					}

					foreach(Rule r in this.Rules)
					{
						ColumnHeader ch = new ColumnHeader();
						ch.Text = r.Expression.ToString();
						ch.Width = 120;
						ch.TextAlign = HorizontalAlignment.Center;
						lvResponseMatrix.Columns.Add(ch);
					}

					this.Refresh();

					lvResponseMatrix.Items.Clear();

					if (this.PetriNetType == PetriNetType.TimeInvariant)
					{
						for(int i = 0; i < this.imSimulationResults.Dimensions.Width; i++)
						{
							ListViewItem lvi = new ListViewItem(i.ToString());

							for(int j = 0; j < this.imSimulationResults.Dimensions.Height; j++)
							{
								if (j < alGroupedPlaces.Count && al.Contains(alGroupedPlaces[j]))
								{
									int iElement = (int)this.imSimulationResults[j, i];

									// Determine actual conflicts
									if (iElement < 0)
									{
										lvi.ImageIndex = 1;
									}

									lvi.SubItems.Add(iElement.ToString());
								}
							}

							// Evaluate rules and add them to response
							IntMatrix imm = this.imSimulationResults.GetColumn(i);

							foreach(Rule r in this.Rules)
							{
								int[] iac = new int[imm.Dimensions.Height];
								for(int l = 0; l < imm.Dimensions.Height; l++)
								{
									iac[l] = imm[l, 0];
								}

								bool bResult = r.Evaluate(alGroupedPlaces, iac);
								lvi.SubItems.Add(bResult.ToString());
							}

							// Determine lock or end
							if (lvResponseMatrix.Items.Count != 0)
							{
								// Determine lock
								int iCount = 0;
								IntMatrix imk = imSimulationResults.GetColumn(i-1);

								for(int k = 0; k < imk.Dimensions.Height - this.Rules.Count; k++)
								{
									if (imSimulationResults[k, i] == imk[k, 0])
										iCount++;
								}
								if (iCount == imk.Dimensions.Height - this.Rules.Count)
								{
									lvi.ImageIndex = 2;
								}

								// Determine end
								int iCount2 = 0;
								foreach(Place p in alInputPlaces)
								{
									int iIndex = alGroupedPlaces.IndexOf(p);

									if (imSimulationResults[iIndex, i] == 0)
										iCount2++;
								}
								if (iCount2 == alInputPlaces.Count)
								{
									int iCount3 = 0;
									foreach(Place p in alOperationPlaces)
									{
										int iIndex2 = alGroupedPlaces.IndexOf(p);

										if (imSimulationResults[iIndex2, i] == 0)
											iCount3++;
									}
									if (iCount3 == alOperationPlaces.Count)
									{
										lvi.ImageIndex = 0;
									}
								}
							}

							if (i % 2 == 1)
								lvi.BackColor = Color.FromArgb(245, 245, 245);

							lvResponseMatrix.Items.Add(lvi);
						}

						// Select current vector m
						int[] ia = new int[alGroupedPlaces.Count];
						for(int i = 0; i < ia.Length; i++)
						{
							ia[i] = ((Place)alGroupedPlaces[i]).Tokens;
						}

						ListViewItem lviCurrent = null;
						for(int i = 0; i < this.roOptions.EndTime; i++)
						{
							int iCount = 0;
							for (int j = 0; j < ia.Length; j++)
							{
								if (ia[j] == imSimulationResults[j, i])
									iCount++;
							}

							if (iCount == ia.Length)
							{
								lviCurrent = lvResponseMatrix.Items[i];
								break;
							}
						}

						if (lviCurrent != null)
							lviCurrent.BackColor = Color.LightSteelBlue;
					}
					else if (this.PetriNetType == PetriNetType.PTimed)
					{
						for(int i = 0; i < this.imSimulationResults.Dimensions.Width; i++)
						{
							ListViewItem lvi = new ListViewItem(i.ToString());

							for(int j = 0; j < this.imSimulationResults.Dimensions.Height; j++)
							{
								if (j < alGroupedPlaces.Count && al.Contains(alGroupedPlaces[j]))
								{
									int iElement = (int)this.imSimulationResults[j, i];

									// Determine actual conflicts
									if (iElement < 0)
									{
										lvi.ImageIndex = 1;
									}

									lvi.SubItems.Add(iElement.ToString());
								}
							}

							// Evaluate rules and add them to response
							IntMatrix imm = this.imSimulationResults.GetColumn(i);

							foreach(Rule r in this.Rules)
							{
								int[] iac = new int[imm.Dimensions.Height];
								for(int l = 0; l < imm.Dimensions.Height; l++)
								{
									iac[l] = imm[l, 0];
								}

								bool bResult = r.Evaluate(alGroupedPlaces, iac);
								lvi.SubItems.Add(bResult.ToString());
							}

							if (i % 2 == 1)
								lvi.BackColor = Color.FromArgb(245, 245, 245);

							lvResponseMatrix.Items.Add(lvi);
						}

						// Select current vector m
						int[] ia = new int[alGroupedPlaces.Count];
						for(int i = 0; i < ia.Length; i++)
						{
							ia[i] = ((Place)alGroupedPlaces[i]).Tokens;
						}

						ListViewItem lviCurrent = null;
						for(int i = 0; i < this.roOptions.EndTime; i++)
						{
							int iCount = 0;
							for (int j = 0; j < ia.Length; j++)
							{
								if (ia[j] == imSimulationResults[j, i])
									iCount++;
							}

							if (iCount == ia.Length)
							{
								lviCurrent = lvResponseMatrix.Items[i];
								break;
							}
						}

						if (lviCurrent != null)
							lviCurrent.BackColor = Color.LightSteelBlue;
					}

					lvResponseMatrix.EndUpdate();
					this.Cursor = Cursors.Default;
				}
				else
				{
					Label lblOscillogram = new Label();
					lblOscillogram.Text = "Double-click to add places which will be shown in spreadsheet.";
					lblOscillogram.Size = tpMatrixPage.Size;
					lblOscillogram.TextAlign = ContentAlignment.MiddleCenter;
					lblOscillogram.DoubleClick +=new EventHandler(lblOscillogram_DoubleClick); ;
					tpMatrixPage.Controls.Add(lblOscillogram);
				}

			}
			#endregion

			#region else if (tabResponse.SelectedTab == tpOscillogramPage)
			else if (tabResponse.SelectedTab == tpOscillogramPage)
			{
				tpOscillogramPage.Controls.Clear();
				if (this.roOptions.Places.Length != 0)
				{
					this.Cursor = Cursors.WaitCursor;

					Oscillogram os = new Oscillogram(this, this.imSimulationResults);
					tpOscillogramPage.Controls.Add(os);
					os.Focus();

					this.Cursor = Cursors.Default;
				}
				else
				{
					Label lblOscillogram = new Label();
					lblOscillogram.Text = "Double-click to add places which will be shown in oscillogram.";
					lblOscillogram.Size = tpOscillogramPage.Size;
					lblOscillogram.TextAlign = ContentAlignment.MiddleCenter;
					lblOscillogram.DoubleClick +=new EventHandler(lblOscillogram_DoubleClick); ;
					tpOscillogramPage.Controls.Add(lblOscillogram);
				}
			}
			#endregion

			#region else if (tabResponse.SelectedTab == tpStatisticsPage)
			else if (tabResponse.SelectedTab == tpStatisticsPage)
			{
				this.Cursor = Cursors.WaitCursor;

				ArrayList alGroupedPlaces = this.GroupedPlaces;

				int iPeriod = this.roOptions.Selection.Height - this.roOptions.Selection.Width;
				this.rtbStatistics.Text = "T = " + iPeriod.ToString() + "\n\n";
				this.rtbStatistics.Text += "Resources utilization :";

				foreach(PlaceResource pr in this.ResourcePlaces)
				{
					ArrayList alResourceOperationPlaces = pr.ResourceOperationsPlaces;

					this.rtbStatistics.Text += "\n" + pr.ToString();

					// Calculate resource-operations work period
					int iWorkingPeriodCounter = 0;

					for (int i = this.roOptions.Selection.Width; i < this.roOptions.Selection.Height; i++)
					{
						int iCount = 0;
						foreach(PlaceOperation po in alResourceOperationPlaces)
						{
							if(this.imSimulationResults[alGroupedPlaces.IndexOf(po), i] == 0)
							{
								iCount++;
							}
						}

						if (iCount == alResourceOperationPlaces.Count && this.imSimulationResults[alGroupedPlaces.IndexOf(pr), i] == 0)
							iWorkingPeriodCounter++;
					}

					float fUsefullnes = iWorkingPeriodCounter / (float)iPeriod * 100;
					NumberFormatInfo nmi = (NumberFormatInfo)Application.CurrentCulture.NumberFormat.Clone();
					nmi.NumberDecimalDigits = 1;
					this.rtbStatistics.Text += " : " + iWorkingPeriodCounter.ToString() + " (" + fUsefullnes.ToString("N", nmi) + "%)";
				}

				this.Cursor = Cursors.Default;
			}
			#endregion
		}
		#endregion

		#region private void lblOscillogram_DoubleClick(object sender, EventArgs e)
		private void lblOscillogram_DoubleClick(object sender, EventArgs e)
		{
            if (this.sSimulator.resultDataFromSimulation == null)
            {
                MessageBox.Show("There are no recorded data to show !!!\nRun the simulation first",
                                "Petri .NET Simulator 2.0 - Information", 
                                MessageBoxButtons.OK, 
                                MessageBoxIcon.Information);
                return;
            }

			ResponsePlacesEditorForm rpefEditor = new ResponsePlacesEditorForm(this, this.roOptions.Places);
			if (DialogResult.OK == rpefEditor.ShowDialog())
			{
				this.roOptions.Places = rpefEditor.Places;
			}
		}
		#endregion

		#region public override string ToString()
		public override string ToString()
		{
			return Path.GetFileNameWithoutExtension(sFileName) + " (PetriNetDocument)";
		}
		#endregion

		#region private void roOptions_PropertiesChanged(object sender, EventArgs e)
		private void roOptions_PropertiesChanged(object sender, EventArgs e)
		{
			tabResponse_SelectedIndexChanged(tabMain, new EventArgs());

			pneEditor_PropertiesChanged(sender, e);
		}
		#endregion

		#region private void roOptions_SimulationRequired(object sender, EventArgs e)
		private void roOptions_SimulationRequired(object sender, EventArgs e)
		{
			// Simulate
			this.Cursor = Cursors.WaitCursor;
			this.imSimulationResults = sSimulator.SimulateToMatrix(this.roOptions.EndTime);
			this.Cursor = Cursors.Default;
		}
		#endregion


		#region private void lvResponseMatrix_DoubleClick(object sender, System.EventArgs e)
		private void lvResponseMatrix_DoubleClick(object sender, System.EventArgs e)
		{
			if (this.PetriNetType == PetriNetType.TimeInvariant)
			{
				int[] ia = new int[lvResponseMatrix.SelectedItems[0].SubItems.Count - 1 - this.Rules.Count];

				for (int i = 0; i < ia.Length; i++)
				{
					ia[i] = int.Parse(lvResponseMatrix.SelectedItems[0].SubItems[i + 1].Text);
				}

				this.Simulator.GoTo(ia);

//				// Adjust Simulation Toolbar buttons
//				if (lvResponseMatrix.SelectedItems[0] != lvResponseMatrix.Items[0])
//				{
//					tbbReset.Enabled = true;
//				}
//				else
//				{
//					tbbReset.Enabled = false;
//					sim.Reset();
//				}
			}
		}
		#endregion


		#region private void cmmiExportSpreadsheet_Click(object sender, System.EventArgs e)
		private void cmmiExportSpreadsheet_Click(object sender, System.EventArgs e)
		{
#if !DEMO
			this.sfdExportFile.FileName = "";

			if (DialogResult.OK == this.sfdExportFile.ShowDialog())
			{
				// Text (Tab delimited) format

				TextWriter tw = File.CreateText(this.sfdExportFile.FileName);

				foreach(ColumnHeader ch in lvResponseMatrix.Columns)
				{
					tw.Write(ch.Text + "\t");
				}
				tw.WriteLine();

				foreach(ListViewItem lvi in this.lvResponseMatrix.Items)
				{
					for(int i = 0; i < lvi.SubItems.Count; i++)
					{
						tw.Write(lvi.SubItems[i].Text + "\t");
					}
					tw.WriteLine();
				}

				tw.Close();
			}
#else
			MessageBox.Show("DEMO version doesn't have implemented Export function!\nBuy a full version which is capable of exporting spreadsheets.", "Petri .NET Simulator 2.0 - Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
#endif
		}
		#endregion

		#region private void cmExport_Popup(object sender, System.EventArgs e)
		private void cmExport_Popup(object sender, System.EventArgs e)
		{
			if (lvResponseMatrix.SelectedItems.Count != 0)
			{
				cmmiExportSpreadsheet.Visible = true;
			}
			else
			{
				cmmiExportSpreadsheet.Visible = false;
			}
		}
		#endregion


	}
}
