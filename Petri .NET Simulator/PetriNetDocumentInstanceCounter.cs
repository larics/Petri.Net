using System;

namespace PetriNetSimulator2
{
	/// <summary>
	/// Summary description for PetriNetDocumentInstanceCounter.
	/// </summary>
	public class PetriNetDocumentInstanceCounter
	{
		// Properties

		#region public int PlaceInstanceCount
		public int PlaceInstanceCount
		{
			get
			{
				return this.iPlaceInstanceCount;
			}
			set
			{
				this.iPlaceInstanceCount = value;
			}
		}
		#endregion

		#region public int TransitionInstanceCount
		public int TransitionInstanceCount
		{
			get
			{
				return this.iTransitionInstanceCount;
			}
			set
			{
				this.iTransitionInstanceCount = value;
			}
		}
		#endregion

		#region public int SubsystemInstanceCount
		public int SubsystemInstanceCount
		{
			get
			{
				return this.iSubsystemInstanceCount;
			}
			set
			{
				this.iSubsystemInstanceCount = value;
			}
		}
		#endregion

		#region public int DescriptionLabelInstanceCount
		public int DescriptionLabelInstanceCount
		{
			get
			{
				return this.iDescriptionLabelInstanceCount;
			}
			set
			{
				this.iDescriptionLabelInstanceCount = value;
			}
		}
		#endregion

		#region public bool FalseIndicesMode
		public bool FalseIndicesMode
		{
			get
			{
				return this.bFalseIndicesMode;
			}
			set
			{
				this.bFalseIndicesMode = value;

				if (value == true)
					this.iFalseCount = int.MinValue;
			}
		}
		#endregion

		// Fields
		private int iPlaceInstanceCount = 0;
		private int iTransitionInstanceCount = 0;
		private int iSubsystemInstanceCount = 0;
		private int iDescriptionLabelInstanceCount = 0;
		private bool bFalseIndicesMode = false;
		private int iFalseCount = int.MinValue;

		public PetriNetDocumentInstanceCounter()
		{

		}

		#region public int IncreasePlaceInstanceCount()
		public int IncreasePlaceInstanceCount()
		{
			if (this.bFalseIndicesMode != true)
				return ++this.iPlaceInstanceCount;
			else
				return ++this.iFalseCount;
		}
		#endregion

		#region public int IncreaseTransitionInstanceCount()
		public int IncreaseTransitionInstanceCount()
		{
			if (this.bFalseIndicesMode != true)
				return ++this.iTransitionInstanceCount;
			else
				return ++this.iFalseCount;
		}
		#endregion

		#region public int IncreaseSubsystemInstanceCount()
		public int IncreaseSubsystemInstanceCount()
		{
			if (this.bFalseIndicesMode != true)
				return ++this.iSubsystemInstanceCount;
			else
				return ++this.iFalseCount;

		}
		#endregion

		#region public int IncreaseDescriptionLabelInstanceCount()
		public int IncreaseDescriptionLabelInstanceCount()
		{
			if (this.bFalseIndicesMode != true)
				return ++this.iDescriptionLabelInstanceCount;
			else
				return ++this.iFalseCount;
		}
		#endregion


		#region public int GetAndIncreaseInstanceCount(object o)
		public int GetAndIncreaseInstanceCount(object o)
		{
			if (o is Place)
				return IncreasePlaceInstanceCount();
			else if (o is Transition)
				return IncreaseTransitionInstanceCount();
			else if (o is Subsystem)
				return IncreaseSubsystemInstanceCount();
			else if (o is DescriptionLabel)
				return IncreaseDescriptionLabelInstanceCount();
			else
				return 0;
		}
		#endregion

	}
}
