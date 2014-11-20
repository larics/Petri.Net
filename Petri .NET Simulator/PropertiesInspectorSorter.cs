using System;
using System.Collections;

namespace PetriNetSimulator2
{

	public class PropertiesInspectorSorter : IComparer
	{
		#region public int Compare(object x, object y)
		public int Compare(object x, object y)
		{
			if (x is PetriNetDocument && y is Place)
				return -1;
			else if (y is PetriNetDocument && x is Place)
				return 1;
			else if (x is PetriNetDocument && y is Transition)
				return -1;
			else if (y is PetriNetDocument && x is Transition)
				return 1;
			else if (x is PetriNetDocument && y is Connection)
				return -1;
			else if (y is PetriNetDocument && x is Connection)
				return 1;
			else if (x is PetriNetDocument && y is DescriptionLabel)
				return -1;
			else if (y is PetriNetDocument && x is DescriptionLabel)
				return 1;
			else if (x is PetriNetDocument && y is Subsystem)
				return -1;
			else if (y is PetriNetDocument && x is Subsystem)
				return 1;
			else if (x is PetriNetDocument && y is Input)
				return -1;
			else if (y is PetriNetDocument && x is Input)
				return 1;
			else if (x is PetriNetDocument && y is Output)
				return -1;
			else if (y is PetriNetDocument && x is Output)
				return 1;

			else if (x is Place && y is Transition)
				return -1;
			else if (y is Place && x is Transition)
				return 1;
			else if (x is Place && y is Connection)
				return -1;
			else if (y is Place && x is Connection)
				return 1;
			else if (x is Place && y is DescriptionLabel)
				return -1;
			else if (y is Place && x is DescriptionLabel)
				return 1;
			else if (x is Place && y is Subsystem)
				return -1;
			else if (y is Place && x is Subsystem)
				return 1;
			else if (x is Place && y is Input)
				return -1;
			else if (y is Place && x is Input)
				return 1;
			else if (x is Place && y is Output)
				return -1;
			else if (y is Place && x is Output)
				return 1;
			else if (x is Place && y is Place)
			{
				Place px = (Place)x;
				return px.CompareTo(y);
			}

			else if (x is Transition && y is Connection)
				return -1;
			else if (y is Transition && x is Connection)
				return 1;
			else if (x is Transition && y is DescriptionLabel)
				return -1;
			else if (y is Transition && x is DescriptionLabel)
				return 1;
			else if (x is Transition && y is Subsystem)
				return -1;
			else if (y is Transition && x is Subsystem)
				return 1;
			else if (x is Transition && y is Input)
				return -1;
			else if (y is Transition && x is Input)
				return 1;
			else if (x is Transition && y is Output)
				return -1;
			else if (y is Transition && x is Output)
				return 1;
			else if (x is Transition && y is Transition)
			{
				Transition tx = (Transition)x;
				return tx.CompareTo(y);
			}

			else if (x is Connection && y is Subsystem)
				return 1;
			else if (y is Connection && x is Subsystem)
				return -1;
			else if (x is Connection && y is Input)
				return 1;
			else if (y is Connection && x is Input)
				return -1;
			else if (x is Connection && y is Output)
				return 1;
			else if (y is Connection && x is Output)
				return -1;
			else if (x is Connection && y is DescriptionLabel)
				return -1;
			else if (y is Connection && x is DescriptionLabel)
				return 1;

			else if (x is Subsystem && y is DescriptionLabel)
				return -1;
			else if (y is Subsystem && x is DescriptionLabel)
				return 1;
			else if (x is Subsystem && y is Input)
				return -1;
			else if (y is Subsystem && x is Input)
				return 1;
			else if (x is Subsystem && y is Output)
				return -1;
			else if (y is Subsystem && x is Output)
				return 1;
			else if (x is Subsystem && y is Subsystem)
			{
				Subsystem sx = (Subsystem)x;
				return sx.CompareTo(y);
			}

			else if (x is Input && y is Output)
				return -1;
			else if (y is Input && x is Output)
				return 1;
			else if (x is Input && y is DescriptionLabel)
				return -1;
			else if (y is Input && x is DescriptionLabel)
				return 1;
			else if (x is Input && y is Input)
			{
				Input ix = (Input)x;
				return ix.CompareTo(y);
			}

			else if (x is Output && y is DescriptionLabel)
				return -1;
			else if (y is Output && x is DescriptionLabel)
				return 1;
			else if (x is Output && y is Output)
			{
				Output ox = (Output)x;
				return ox.CompareTo(y);
			}

			else if (x is DescriptionLabel && y is DescriptionLabel)
			{
				DescriptionLabel dlx = (DescriptionLabel)x;
				return dlx.CompareTo(y);
			}

			return 0;
		}
		#endregion
	}
}