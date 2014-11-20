using System;

namespace PetriNetSimulator2
{
	/// <summary>
	/// Summary description for IntMatrix.
	/// </summary>
	public class IntMatrix : Matrix
	{
		public IntMatrix(int iRows, int iColumns) : base(iRows, iColumns)
		{
			for(int i = 0; i < iRows; i++)
				for(int j = 0; j < iColumns; j++)
					this[i, j] = 0;
		}

		public IntMatrix(object[,] oa) : base (oa)
		{

		}

		#region public static IntMatrix operator &(IntMatrix lhs, IntMatrix rhs)
		public static IntMatrix operator &(IntMatrix lhs, IntMatrix rhs)
		{
			if (lhs.Dimensions.Width != rhs.Dimensions.Height)
			{
				throw new ArgumentException("Invalid parameters dimensions.");
			}
			else
			{
				IntMatrix m = new IntMatrix(lhs.Dimensions.Height, rhs.Dimensions.Width);

				for (int i = 0; i < m.Dimensions.Height; i++)
				{
					for (int j = 0; j < m.Dimensions.Width; j++)
					{
						int iValue = 0;
						for (int u = 0; u < lhs.Dimensions.Width; u++)
						{
							iValue |= (int)lhs[i, u] & (int)rhs[u, j];
						}

						m[i, j] = iValue;
					}
				}
				
				return m;
			}
		}
		#endregion

		#region public static IntMatrix operator *(IntMatrix lhs, IntMatrix rhs)
		public static IntMatrix operator *(IntMatrix lhs, IntMatrix rhs)
		{
			if (lhs.Dimensions.Width != rhs.Dimensions.Height)
			{
				throw new ArgumentException("Invalid parameters dimensions.");
			}
			else
			{
				IntMatrix m = new IntMatrix(lhs.Dimensions.Height, rhs.Dimensions.Width);

				for (int i = 0; i < m.Dimensions.Height; i++)
				{
					for (int j = 0; j < m.Dimensions.Width; j++)
					{
						int iValue = 0;
						for (int u = 0; u < lhs.Dimensions.Width; u++)
						{
							iValue += (int)lhs[i, u] * (int)rhs[u, j];
						}

						m[i, j] = iValue;
					}
				}
				
				return m;
			}		
		}
		#endregion

		#region public static IntMatrix operator +(IntMatrix lhs, IntMatrix rhs)
		public static IntMatrix operator +(IntMatrix lhs, IntMatrix rhs)
		{
			if (lhs.Dimensions != rhs.Dimensions)
			{
				throw new ArgumentException("Invalid parameters dimensions.");
			}
			else
			{
				IntMatrix m = new IntMatrix(lhs.Dimensions.Height, lhs.Dimensions.Width);

				for (int i = 0; i < m.Dimensions.Height; i++)
				{
					for (int j = 0; j < m.Dimensions.Width; j++)
					{
						m[i, j] = (int)lhs[i, j] + (int)rhs[i, j];
					}
				}
				
				return m;
			}		
		}
		#endregion

		#region public static IntMatrix operator -(IntMatrix lhs, IntMatrix rhs)
		public static IntMatrix operator -(IntMatrix lhs, IntMatrix rhs)
		{
			if (lhs.Dimensions != rhs.Dimensions)
			{
				throw new ArgumentException("Invalid parameters dimensions.");
			}
			else
			{
				IntMatrix m = new IntMatrix(lhs.Dimensions.Height, lhs.Dimensions.Width);

				for (int i = 0; i < m.Dimensions.Height; i++)
				{
					for (int j = 0; j < m.Dimensions.Width; j++)
					{
						m[i, j] = (int)lhs[i, j] - (int)rhs[i, j];
					}
				}
				
				return m;
			}		
		}
		#endregion

		#region public static IntMatrix MulTim (IntMatrix lhs, IntMatrix rhs)
		public static IntMatrix MulTim (IntMatrix lhs, IntMatrix rhs)
		{
			if (lhs.Dimensions.Width != rhs.Dimensions.Height)
			{
				throw new ArgumentException("Invalid parameters dimensions.");
			}
			else
			{
				IntMatrix m = new IntMatrix(lhs.Dimensions.Height, lhs.Dimensions.Width);

				for (int i = 0; i < m.Dimensions.Height; i++)
				{
					for (int j = 0; j < m.Dimensions.Width; j++)
					{
						m[i, j] = (int)lhs[i, j] * (int)rhs[j, 0];
					}
				}
				
				return m;
			}		
		}
		#endregion

		#region public new IntMatrix Transpose()
		public new IntMatrix Transpose()
		{
			IntMatrix im = new IntMatrix(this.Dimensions.Width, this.Dimensions.Height);

			for (int i = 0; i < this.Dimensions.Height; i++)
			{
				for (int j = 0; j < this.Dimensions.Width; j++)
				{
					im[j, i] = this[i, j];
				}
			}

			return im;
		}
		#endregion

		#region public new IntMatrix GetColumn(int iColumn)
		public new IntMatrix GetColumn(int iColumn)
		{
			object[,] oa = new object[this.iRows, 1];

			for(int i = 0; i < this.iRows; i++)
			{
				oa[i, 0] = this[i, iColumn];
			}

			return new IntMatrix(oa);
		}
		#endregion

		#region public int Max()
		public int Max()
		{
			int iMax = 0;
			for (int i = 0; i < this.Dimensions.Height; i++)
			{
				for (int j = 0; j < this.Dimensions.Width; j++)
				{
					if (this[i, j] > iMax)
						iMax = this[i, j];
				}
			}

			return iMax;
		}
		#endregion

		#region public new int this[int iRowIndex, int iColumnIndex]
		public new int this[int iRowIndex, int iColumnIndex]
		{
			get
			{
				return (int)oaElements[iRowIndex, iColumnIndex];
			}
			set
			{
				oaElements[iRowIndex, iColumnIndex] = value;
			}
		}	
		#endregion
	}
}
