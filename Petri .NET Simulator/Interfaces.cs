using System;
using System.Drawing;

namespace PetriNetSimulator2
{
	/// <summary>
	/// Summary description for ISelectable.
	/// </summary>
	interface ISelectable
	{
		bool Selected
		{
			get;
			set;
		}
	}

	/// <summary>
	/// Summary description for IConnector.
	/// </summary>
	interface IConnector
	{
		Point GetBeginCenterPoint(Point ptBegin, Size sz);
		Point GetEndCenterPoint(Point ptEnd, Size sz);
		Point GetEndFixedPoint(Point ptEnd, Size sz);

		LineDirection GetBeginDirection(Point ptBegin, Point ptBeginCenter, Size sz);
		LineDirection GetEndDirection(Point ptEnd, Point ptEndCenter, Size sz);
	}

	/// <summary>
	/// Summary description for IMetafileModel.
	/// </summary>
	interface IMetafileModel
	{
		void DrawModel(Graphics g);
	}

	/// <summary>
	/// Summary description for IUndoRedo.
	/// </summary>
	interface IUndoRedo
	{
		void Undo(object o, UndoRedoAction ura, object oData);
		void Redo(object o, UndoRedoAction ura, object oData);
	}
}
