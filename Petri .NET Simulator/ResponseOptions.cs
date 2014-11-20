using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Drawing;
using System.Collections;
using System.Runtime.Serialization;

namespace PetriNetSimulator2
{
	/// <summary>
	/// Summary description for ResponseOptions.
	/// </summary>
	[Serializable]
	public class ResponseOptions : ISerializable
	{
		// Properties
		#region public int EndTime
		[Category("Response Options")]
		[Description("End time for simulation (in number of descretization intervals).")]
		[CommonProperties]
		public int EndTime
		{
			get
			{
				return this.iEndTime;
			}
			set
			{
				// Validate EndTime
				if (value > 0)
				{
					this.iEndTime = value;

					this.szSelection = new Size((this.szSelection.Width >= value) ? 0 : this.szRange.Width, (this.szSelection.Height >= value) ? value : this.szSelection.Height);

					if (this.SimulationRequired != null)
						this.SimulationRequired(this, new EventArgs());

					// Adjust Range property
					// This also calls PropertiesChanged event, so here it doesn't need to be called
					this.Range = new Size((this.szRange.Width >= value) ? 0 : this.szRange.Width, value);
				}
				else
				{
					return;
				}
			}
		}
		#endregion

		#region public Size Range
		[Category("Oscillogram Options")]
		[Description("Range of steps that will be drawn in oscillogram.")]
//		[CommonProperties]
		public Size Range
		{
			get
			{
				return this.szRange;
			}
			set
			{
				//Validate
				if (value.Width < 0 || value.Height > this.iEndTime || value.Width >= value.Height)
				{
					
					return;
				}

				this.szRange = value;

				if (this.PropertiesChanged != null)
					this.PropertiesChanged(this, new EventArgs());
			}
		}
		#endregion

		#region public Size Selection
		[Category("Oscillogram Options")]
		[Description("Selection of steps that will be used for statistics calculation.")]
		public Size Selection
		{
			get
			{
				return this.szSelection;
			}
			set
			{
				this.szSelection = value;
			}
		}
		#endregion

		#region public Place[] Places
		[CommonProperties]
		[Category("Oscillogram Options")]
		[Description("Set of places which will be shown in oscillogram.")]
		[EditorAttribute(typeof(ResponsePlacesEditor), typeof(UITypeEditor))]
		public Place[] Places
		{
			get
			{
				// Remove non-existing Places from array
				int iCount = 0;
				for (int i = 0; i < this.paPlaces.Length; i++)
				{
					Place p = this.paPlaces[i];

					if (this.pnd.Places.Contains(p))
					{
						iCount++;
					}
				}

				Place[] pa = new Place[iCount];

				int iIndex = 0;
				for (int i = 0; i < this.paPlaces.Length; i++)
				{
					Place p = this.paPlaces[i];

					if (this.pnd.Places.Contains(p))
					{
						pa[iIndex] = p;
						iIndex++;
					}
				}

				return pa;
			}
			set
			{
				this.paPlaces = value;

				if (this.PropertiesChanged != null)
					this.PropertiesChanged(this, new EventArgs());
			}

		}
		#endregion

		#region public int GraphHeight
		[CommonProperties]
		[Category("Oscillogram Options")]
		[Description("Represents the height of each graph in oscillogram.")]
		public int GraphHeight
		{
			get
			{
				return this.iGraphHeight;
			}
			set
			{
				this.iGraphHeight = value;

				if (this.PropertiesChanged != null)
					this.PropertiesChanged(this, new EventArgs());
			}

		}
		#endregion

		#region public int SectionWidth
		[CommonProperties]
		[Category("Oscillogram Options")]
		[Description("Represents the width of each section in oscillogram.")]
		public int SectionWidth
		{
			get
			{
				return this.iSectionWidth;
			}
			set
			{
				this.iSectionWidth = value;

				if (this.PropertiesChanged != null)
					this.PropertiesChanged(this, new EventArgs());
			}

		}
		#endregion

		#region public int GraphDistance
		[CommonProperties]
		[Category("Oscillogram Options")]
		[Description("Represents the distance between each graph in oscillogram.")]
		public int GraphDistance
		{
			get
			{
				return this.iGraphDistance;
			}
			set
			{
				this.iGraphDistance = value;

				if (this.PropertiesChanged != null)
					this.PropertiesChanged(this, new EventArgs());
			}

		}
		#endregion

		#region public int NumbersPerSection
		[CommonProperties]
		[Category("Oscillogram Options")]
		[Description("Represents the number of ticks in one section.")]
		public int NumbersPerSection
		{
			get
			{
				return this.iNumbersPerSection;
			}
			set
			{
				this.iNumbersPerSection = value;

				if (this.PropertiesChanged != null)
					this.PropertiesChanged(this, new EventArgs());
			}

		}
		#endregion

		#region public bool ShowIndexes
		[CommonProperties]
		[Category("Oscillogram Options")]
		[Description("Shows place indexes on y-label of each graph in oscillogram.")]
		public bool ShowIndexes
		{
			get
			{
				return this.bShowIndexes;
			}
			set
			{
				this.bShowIndexes = value;

				if (this.PropertiesChanged != null)
					this.PropertiesChanged(this, new EventArgs());
			}

		}
		#endregion

		#region public PetriNetDocument Document
		public PetriNetDocument Document
		{
			get
			{
				return this.pnd;
			}
		}
		#endregion

		// Events
		public event EventHandler PropertiesChanged;
		public event EventHandler SimulationRequired;

		// Fields
		private PetriNetDocument pnd;
		private int iEndTime = 1000;
		private Place[] paPlaces = new Place[0];
		private int[] iaPlaces;
		
		private int iGraphHeight = 50;
		private int iSectionWidth = 25;
		private int iGraphDistance = 30;
		private int iNumbersPerSection = 1;
		private bool bShowIndexes = false;
		private Size szRange = new Size(0, 1000);
		private Size szSelection = new Size(0, 1000);

		public ResponseOptions(PetriNetDocument pnd)
		{
			this.pnd = pnd;
		}

		// Constructor for Deserialization
		#region protected ResponseOptions(SerializationInfo info, StreamingContext context)
		protected ResponseOptions(SerializationInfo info, StreamingContext context)
		{
			this.iEndTime = info.GetInt32("endtime");
			this.iGraphDistance = info.GetInt32("graphdistance");
			this.iNumbersPerSection = info.GetInt32("numberspersection");
			this.iGraphHeight = info.GetInt32("graphheight");
			this.iSectionWidth = info.GetInt32("sectionwidth");
			this.bShowIndexes = info.GetBoolean("showindexes");
			this.iaPlaces = (int[])info.GetValue("places", typeof(int[]));

			this.szRange = new Size(0, this.iEndTime);
			this.szSelection = new Size(0, this.iEndTime);
		}
		#endregion

		#region public void GetObjectData(SerializationInfo info, StreamingContext context)
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("endtime", this.iEndTime);
			info.AddValue("graphdistance", this.iGraphDistance);
			info.AddValue("numberspersection", this.iNumbersPerSection);
			info.AddValue("graphheight", this.iGraphHeight);
			info.AddValue("sectionwidth", this.iSectionWidth);
			info.AddValue("showindexes", this.bShowIndexes);

			//Serialize places
			int[] ia = new int[this.Places.Length];
			for(int i = 0; i < ia.Length; i++)
			{
				ia[i] = this.Places[i].Index;
			}
		
			info.AddValue("places", ia);
		}
		#endregion


		#region public void RestoreReferences(PetriNetDocument pnd, Hashtable ht)
		public void RestoreReferences(PetriNetDocument pnd, Hashtable ht)
		{
			this.pnd = pnd;

			this.paPlaces = new Place[this.iaPlaces.Length];
			
			for(int i = 0; i < this.iaPlaces.Length; i++)
			{
				this.paPlaces[i] = (Place)ht["P" + this.iaPlaces[i].ToString()];
			}
		}
		#endregion

		#region public override string ToString()
		public override string ToString()
		{
			return "Response Options";
		}
		#endregion

	}
}
