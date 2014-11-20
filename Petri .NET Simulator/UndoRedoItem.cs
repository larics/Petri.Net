using System;
using System.Collections;

namespace PetriNetSimulator2
{
	/// <summary>
	/// Summary description for UndoRedoItem.
	/// </summary>
	public class UndoRedoItem
	{
		object o;
		object oUndoRedoHandler;
		UndoRedoAction ura;
		object oData;

		#region public UndoRedoItem(object o, object oUndoRedoHandler, UndoRedoAction ura, object oData)
		public UndoRedoItem(object o, object oUndoRedoHandler, UndoRedoAction ura, object oData)
		{
			this.o = o;
			this.oUndoRedoHandler = oUndoRedoHandler;
			this.ura = ura;
			this.oData = oData;
		}
		#endregion

		#region public void Undo()
		public void Undo()
		{
			if (this.oUndoRedoHandler is IUndoRedo)
			{
				IUndoRedo iur = (IUndoRedo)this.oUndoRedoHandler;
				iur.Undo(this.o, this.ura, this.oData);
			}
			else
				throw new InterfaceNotImplementedException("IUndoRedo in " + this.oUndoRedoHandler.GetType().ToString() + " not implemented!");
		}
		#endregion

		#region public void Redo()
		public void Redo()
		{
			if (this.oUndoRedoHandler is IUndoRedo)
			{
				IUndoRedo iur = (IUndoRedo)this.oUndoRedoHandler;
				iur.Redo(this.o, this.ura, this.oData);
			}
			else
				throw new InterfaceNotImplementedException("IUndoRedo in " + this.oUndoRedoHandler.GetType().ToString() + " not implemented!");
		}
		#endregion

		#region public override string ToString()
		public override string ToString()
		{
			#region if (ura == UndoRedoAction.Created)
			if (ura == UndoRedoAction.Created || ura == UndoRedoAction.Deleted)
			{
				ArrayList al = (ArrayList)this.o;
				int iObjects = 0;
				int iConnections = 0;
				foreach(object oo in al)
				{
					if (oo is SelectableAndMovableControl)
						iObjects++;
					else if (oo is Connection)
						iConnections++;
				}

				if (iObjects > 1)
					return ura + " - " + iObjects.ToString() + " objects";
				else
				{
					//Find first object
					int iFound = 0;
					for(int i = 0; i < al.Count; i++)
					{
						if (al[i] is SelectableAndMovableControl)
						{
							iFound = i;
							break;
						}
					}
					return ura + " - " + al[iFound].ToString();
				}
			}
			#endregion

			#region else if (ura == UndoRedoAction.LocationChanged)
			else if (ura == UndoRedoAction.LocationChanged)
			{
				ArrayList al = (ArrayList)oData;
				ArrayList alObjects = (ArrayList)al[0];

				if (alObjects.Count > 1)
					return ura + " - " + alObjects.Count.ToString() + " objects";
				else
					return ura + " - " + alObjects[0].ToString();
			}
			#endregion

			return this.ura + " - " + o.ToString();
		}
		#endregion

	}
}
