using System;
using System.Collections;

namespace PetriNetSimulator2
{
	/// <summary>
	/// Summary description for PetriNetEditorMergeModule.
	/// </summary>
	public class PetriNetEditorMergeModule
	{
		// Properties
		#region public ArrayList Objects
		public ArrayList Objects
		{
			get
			{
				return this.alObjects;
			}
		}
		#endregion

		#region public ArrayList Connections
		public ArrayList Connections
		{
			get
			{
				return this.alConnections;
			}
		}
		#endregion

		// Fields
		private ArrayList alObjects = new ArrayList();
		private ArrayList alConnections = new ArrayList();

		public PetriNetEditorMergeModule()
		{

		}

		#region public PetriNetEditorMergeModule(SelectableAndMovableControl smac)
		public PetriNetEditorMergeModule(SelectableAndMovableControl smac)
		{
			this.alObjects.Add(smac);
		}
		#endregion

		#region public void Add(object o)
		public void Add(object o)
		{
			if (o is SelectableAndMovableControl)
			{
				this.alObjects.Add(o);
			}
			else if (o is Connection)
			{
				this.alConnections.Add(o);
			}
			else
				throw new ArgumentException();
		}
		#endregion

	}
}
