using System;
using System.Drawing;

namespace PetriNetSimulator2
{
	/// <summary>
	/// Summary description for Matrix.
	/// </summary>
	public class Matrix
	{
		// Properties

		#region public Size Dimensions
		public Size Dimensions
		{
			get
			{
				return new Size(iColumns, iRows);
			}
		}
		#endregion

		// Fields
		protected object[,] oaElements = null;
		protected int iRows = 0;
		protected int iColumns = 0;

		public Matrix(int iRows, int iColumns)
		{
			this.oaElements = new object[iRows, iColumns];
			this.iRows = iRows;
			this.iColumns = iColumns;

			for(int i = 0; i < iRows; i++)
				for(int j = 0; j < iColumns; j++)
					oaElements[i, j] = null;
		}

		public Matrix(object[,] oa)
		{
			this.oaElements = oa;
			this.iRows = oa.GetLength(0);
			this.iColumns = oa.GetLength(1);
		}


		#region public override string ToString()
		public override string ToString()
		{
			string s = "";

			for(int i = 0; i < this.iRows; i++)
			{
				s += "[";
				for (int j = 0; j < this.iColumns; j++)
				{
					s += " " + oaElements[i,j].ToString();
				}
				s += " ]\n";
			}
			return s;
		}
		#endregion

		#region public object this[int iRowIndex, int iColumnIndex]
		public object this[int iRowIndex, int iColumnIndex]
		{
			get
			{
				return oaElements[iRowIndex, iColumnIndex];
			}
			set
			{
				oaElements[iRowIndex, iColumnIndex] = value;
			}
		}	
		#endregion

		#region public Matrix Transpose()
		public Matrix Transpose()
		{
			Matrix m = new Matrix(this.iColumns, this.iRows);

			for (int i = 0; i < this.iRows; i++)
			{
				for (int j = 0; j < this.iColumns; j++)
				{
					m[j, i] = this[i, j];
				}
			}

			return m;
		}
		#endregion

		#region public Matrix GetColumn(int iColumn)
		public Matrix GetColumn(int iColumn)
		{
			object[,] oa = new object[this.iRows, 1];

			for(int i = 0; i < this.iRows; i++)
			{
				oa[i, 0] = this[i, iColumn];
			}

			return new Matrix(oa);
		}
		#endregion


	}
}
