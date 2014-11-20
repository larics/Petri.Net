using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace PetriNetSimulator2
{
	/// <summary>
	/// Summary description for Port.
	/// </summary>
	public class Port
	{
		#region public Region PortRegion
		public Region PortRegion
		{
			get
			{
				return this.rgn;
			}
			set
			{
				this.rgn = value;
			}
		}
		#endregion

		#region public int PortNumber
		public int PortNumber
		{
			get
			{
				return this.iPortNumber;
			}
		}
		#endregion

		private Region rgn;
		private int iPortNumber;

		public Port(int iPortNumber, Region rgn)
		{
			this.rgn = rgn;
			this.iPortNumber = iPortNumber;
		}
	}
}
