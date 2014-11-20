using System;

namespace PetriNetSimulator2
{
	/// <summary>
	/// Summary description for Clipboard.
	/// </summary>
	public class Clipboard
	{
		#region public static PetriNetEditorMergeModule Object
		public static PetriNetEditorMergeModule Object
		{
			get
			{
				return pnemmClipboard;
			}
			set
			{
				pnemmClipboard = value;
			}
		}
		#endregion

		private static PetriNetEditorMergeModule pnemmClipboard;

		public Clipboard()
		{

		}


	}
}
