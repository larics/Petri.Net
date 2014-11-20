using System;
using System.Drawing;

namespace PetriNetSimulator2
{
	/// <summary>
	/// Summary description for StringMatrix.
	/// </summary>
	public class StringMatrix : Matrix
	{
		public StringMatrix(int iRows, int iColumns) : base (iRows, iColumns)
		{
			for(int i = 0; i < iRows; i++)
				for(int j = 0; j < iColumns; j++)
					this[i, j] = "0";
		}

		public StringMatrix(object[,] oa) : base(oa)
		{

		}

		public static StringMatrix operator *(StringMatrix lhs, StringMatrix rhs)
		{
			if (lhs.Dimensions.Width != rhs.Dimensions.Height)
			{
				throw new ArgumentException("Invalid parameters dimensions.");
			}
			else
			{
				StringMatrix sm = new StringMatrix(lhs.Dimensions.Height, rhs.Dimensions.Width);

				for (int i = 0; i < sm.Dimensions.Height; i++)
				{
					for (int j = 0; j < sm.Dimensions.Width; j++)
					{
						string sValue = "";
						for (int u = 0; u < lhs.Dimensions.Width; u++)
						{
							string slhs = (string)lhs[i, u];
							string srhs = (string)rhs[u, j];
							if (slhs.Length >= 2 && srhs.Length >= 2)
							{
								if (srhs.StartsWith(slhs[slhs.Length - 1].ToString()))
								{
									sValue += slhs + srhs.Substring(1);
								}
							}
						}

						if (sValue == "")
							sValue = "0";

						sm[i, j] = sValue;
					}
				}
				
				return sm;
			}		
		}


	}
}
