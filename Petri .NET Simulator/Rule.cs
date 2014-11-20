using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Collections;

namespace PetriNetSimulator2
{
	/// <summary>
	/// Summary description for Rule.
	/// </summary>


	[Serializable]
	public class Rule : ISerializable
	{
		// Properties
		#region public string Expression
		public string Expression
		{
			get
			{
				return this.sRuleExpression;
			}
			set
			{
				this.sRuleExpression = value;
			}
		}
		#endregion

		#region public string Description
		public string Description
		{
			get
			{
				return this.sRuleDescription;
			}
			set
			{
				this.sRuleDescription = value;
			}
		}
		#endregion

		// Fields
		private string sRuleExpression;
		private string sRuleDescription;
		private string[,] saConditions;
		private string[,] saResults;

		#region public Rule(string sRuleExpression, string sRuleDescription)
		public Rule(string sRuleExpression, string sRuleDescription)
		{
			this.sRuleExpression = sRuleExpression;
			this.sRuleDescription = sRuleDescription;

			this.Parse();
		}
		#endregion

		// Constructor for Deserialization
		#region protected Rule(SerializationInfo info, StreamingContext context)
		protected Rule(SerializationInfo info, StreamingContext context)
		{
			this.sRuleExpression = info.GetString("expression");
			this.sRuleDescription = info.GetString("description");

			this.Parse();
		}
		#endregion

		#region void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("expression", this.sRuleExpression);
			info.AddValue("description", this.sRuleDescription);
		}
		#endregion


		#region private void Parse()
		private void Parse()
		{
			string sRuleConditionFixed = this.sRuleExpression.Remove(0, 3);

			// Split by THEN
			sRuleConditionFixed = sRuleConditionFixed.Replace("THEN", "$");
			sRuleConditionFixed = sRuleConditionFixed.Replace("AND", "&");

			string[] saParts = sRuleConditionFixed.Split(new char[]{'$'});

			for(int i = 0; i < saParts.Length; i++)
			{
				saParts[i] = saParts[i].Replace("(", "");
				saParts[i] = saParts[i].Replace(")", "");
				saParts[i] = saParts[i].Replace(" ", "");
			}

			// Split conditions
			string[] saGroupedConditions = saParts[0].Split(new char[]{'&'});
			this.saConditions = new string[saGroupedConditions.Length, 3];

			for(int i = 0; i < saGroupedConditions.Length; i++)
			{
				string sGroupedCondition = saGroupedConditions[i];

				sGroupedCondition = sGroupedCondition.Replace("==", "=");
				sGroupedCondition = sGroupedCondition.Replace("!=", "!");
				sGroupedCondition = sGroupedCondition.Replace(">=", "?");
				sGroupedCondition = sGroupedCondition.Replace("<=", "#");
				
				string[] saSeparatedCondition = sGroupedCondition.Split(new char[]{'=', '!', '?', '#', '<', '>'});
				this.saConditions[i, 0] = saSeparatedCondition[0];

				string sOperator = saGroupedConditions[i].Replace(saSeparatedCondition[0], "");
				sOperator = sOperator.Replace(saSeparatedCondition[1], "");
				sOperator = sOperator.Replace(" ", "");

				this.saConditions[i, 1] = sOperator;
				this.saConditions[i, 2] = saSeparatedCondition[1];
			}

			// Split results
			string sResults = saParts[1].Replace("AND", "&");
			string[] saGroupedResults = sResults.Split(new char[]{'&'});
			this.saResults = new string[saGroupedResults.Length, 2];

			for(int i = 0; i < saGroupedResults.Length; i++)
			{
				string sGroupedResult = saGroupedResults[i];

				// If someone enters == instead of =
				sGroupedResult = sGroupedResult.Replace("==", "=");

				string[] saSeparatedResult = sGroupedResult.Split(new char[]{'='});
				this.saResults[i, 0] = saSeparatedResult[0];
				this.saResults[i, 1] = saSeparatedResult[1];
			}
		}
		#endregion


		#region public bool Evaluate(PetriNetDocument pnd)
		public bool Evaluate(PetriNetDocument pnd)
		{
			bool bCondition = true;

			for(int i = 0; i < saConditions.GetLength(0); i++)
			{
				#region Calculate LeftSide of Condition

				// Calculate LeftSide of Condition
				string[] saSeparatedOperands = saConditions[i, 0].Split(new char[]{'+', '-'});
				string[] saOperations = new string[saSeparatedOperands.Length - 1];

				string sConditionLeft = saConditions[i, 0];
				for(int j = 0; j < saSeparatedOperands.Length - 1; j++)
				{
					sConditionLeft = sConditionLeft.Replace(saSeparatedOperands[j], "");
					saOperations[j] = sConditionLeft[0].ToString();
					sConditionLeft = sConditionLeft.Remove(0, 1);
				}

				int iLeftSum = 0;
				for(int k = 0; k < saSeparatedOperands.Length; k++)
				{
					// Find if place exists
					Place pFound = null;
					foreach(Place p in pnd.Places)
					{
						if (p.NameID == saSeparatedOperands[k] || "P" + p.Index.ToString() == saSeparatedOperands[k])
						{
							pFound = p;
							break;
						}
					}
					
					if (pFound != null)
					{
						if (k != 0)
						{
							switch(saOperations[k - 1])
							{
								case "+" :
								{
									iLeftSum += pFound.Tokens;
									break;
								}
								case "-" :
								{
									iLeftSum -= pFound.Tokens;
									break;
								}
							}
						}
						else
						{
							foreach(Place p in pnd.Places)
							{
								if (p.NameID == saSeparatedOperands[k] || "P" + p.Index.ToString() == saSeparatedOperands[k])
								{
									iLeftSum += p.Tokens;
									break;
								}
							}
						}
					}
					else
					{
						// If operand is not found in places then it must be either a constant or it
						// is a place that has been deleted!
						// In second case, implement code that will disable or delete this rule
						// TODO : Implement rule consistency check code
						if (k != 0)
						{
							switch(saOperations[k - 1])
							{
								case "+" :
								{
									iLeftSum += int.Parse(saSeparatedOperands[k]);
									break;
								}
								case "-" :
								{
									iLeftSum -= int.Parse(saSeparatedOperands[k]);
									break;
								}
							}
						}
						else
						{
							iLeftSum += int.Parse(saSeparatedOperands[k]);
						}
					}
				}
				#endregion

				#region Calculate RightSide of Condition

				// Calculate RightSide of Condition
				saSeparatedOperands = saConditions[i, 2].Split(new char[]{'+', '-'});
				saOperations = new string[saSeparatedOperands.Length - 1];

				string sConditionRight = saConditions[i, 2];
				for(int j = 0; j < saSeparatedOperands.Length - 1; j++)
				{
					sConditionRight = sConditionRight.Replace(saSeparatedOperands[j], "");
					saOperations[j] = sConditionRight[0].ToString();
					sConditionRight = sConditionRight.Remove(0, 1);
				}

				int iRightSum = 0;
				for(int k = 0; k < saSeparatedOperands.Length; k++)
				{
					// Find if place exists
					Place pFound = null;
					foreach(Place p in pnd.Places)
					{
						if (p.NameID == saSeparatedOperands[k] || "P" + p.Index.ToString() == saSeparatedOperands[k])
						{
							pFound = p;
							break;
						}
					}
					
					if (pFound != null)
					{
						if (k != 0)
						{
							switch(saOperations[k - 1])
							{
								case "+" :
								{
									iRightSum += pFound.Tokens;
									break;
								}
								case "-" :
								{
									iRightSum -= pFound.Tokens;
									break;
								}
							}
						}
						else
						{
							foreach(Place p in pnd.Places)
							{
								if (p.NameID == saSeparatedOperands[k] || "P" + p.Index.ToString() == saSeparatedOperands[k])
								{
									iRightSum += p.Tokens;
									break;
								}
							}
						}
					}
					else
					{
						if (k != 0)
						{
							switch(saOperations[k - 1])
							{
								case "+" :
								{
									iRightSum += int.Parse(saSeparatedOperands[k]);
									break;
								}
								case "-" :
								{
									iRightSum -= int.Parse(saSeparatedOperands[k]);
									break;
								}
							}
						}
						else
						{
							iRightSum += int.Parse(saSeparatedOperands[k]);
						}
					}
				}
				#endregion

				#region switch(saConditions[i, 1])
				switch(saConditions[i, 1])
				{
					case "==" :
					{
						bool bCompare = (iLeftSum == iRightSum);
						bCondition &= bCompare;
						break;
					}
					case "!=" :
					{
						bool bCompare = (iLeftSum != iRightSum);
						bCondition &= bCompare;
						break;
					}
					case ">=" :
					{
						bool bCompare = (iLeftSum >= iRightSum);
						bCondition &= bCompare;
						break;
					}
					case "<=" :
					{
						bool bCompare = (iLeftSum <= iRightSum);
						bCondition &= bCompare;
						break;
					}
					case ">" :
					{
						bool bCompare = (iLeftSum > iRightSum);
						bCondition &= bCompare;
						break;
					}
					case "<" :
					{
						bool bCompare = (iLeftSum < iRightSum);
						bCondition &= bCompare;
						break;
					}
				}
				#endregion
			}

			return bCondition;
		}
		#endregion

		#region public bool Evaluate(ArrayList alGroupedPlaces, int[] iaTokensVector)
		public bool Evaluate(ArrayList alGroupedPlaces, int[] iaTokensVector)
		{
			bool bCondition = true;

			for(int i = 0; i < saConditions.GetLength(0); i++)
			{
				#region Calculate LeftSide of Condition

				// Calculate LeftSide of Condition
				string[] saSeparatedOperands = saConditions[i, 0].Split(new char[]{'+', '-'});
				string[] saOperations = new string[saSeparatedOperands.Length - 1];

				string sConditionLeft = saConditions[i, 0];
				for(int j = 0; j < saSeparatedOperands.Length - 1; j++)
				{
					sConditionLeft = sConditionLeft.Replace(saSeparatedOperands[j], "");
					saOperations[j] = sConditionLeft[0].ToString();
					sConditionLeft = sConditionLeft.Remove(0, 1);
				}

				int iLeftSum = 0;
				for(int k = 0; k < saSeparatedOperands.Length; k++)
				{
					// Find if place exists
					Place pFound = null;
					foreach(Place p in alGroupedPlaces)
					{
						if (p.NameID == saSeparatedOperands[k] || "P" + p.Index.ToString() == saSeparatedOperands[k])
						{
							pFound = p;
							break;
						}
					}
					
					if (pFound != null)
					{
						int iPlaceIndex = alGroupedPlaces.IndexOf(pFound);

						if (k != 0)
						{
							switch(saOperations[k - 1])
							{
								case "+" :
								{
									iLeftSum += iaTokensVector[iPlaceIndex];
									break;
								}
								case "-" :
								{
									iLeftSum -= iaTokensVector[iPlaceIndex];
									break;
								}
							}
						}
						else
						{
							foreach(Place p in alGroupedPlaces)
							{
								if (p.NameID == saSeparatedOperands[k] || "P" + p.Index.ToString() == saSeparatedOperands[k])
								{
									iLeftSum += iaTokensVector[iPlaceIndex];
									break;
								}
							}
						}
					}
					else
					{
						if (k != 0)
						{
							switch(saOperations[k - 1])
							{
								case "+" :
								{
									iLeftSum += int.Parse(saSeparatedOperands[k]);
									break;
								}
								case "-" :
								{
									iLeftSum -= int.Parse(saSeparatedOperands[k]);
									break;
								}
							}
						}
						else
						{
							iLeftSum += int.Parse(saSeparatedOperands[k]);
						}
					}
				}
				#endregion

				#region Calculate RightSide of Condition

				// Calculate RightSide of Condition
				saSeparatedOperands = saConditions[i, 2].Split(new char[]{'+', '-'});
				saOperations = new string[saSeparatedOperands.Length - 1];

				string sConditionRight = saConditions[i, 2];
				for(int j = 0; j < saSeparatedOperands.Length - 1; j++)
				{
					sConditionRight = sConditionRight.Replace(saSeparatedOperands[j], "");
					saOperations[j] = sConditionRight[0].ToString();
					sConditionRight = sConditionRight.Remove(0, 1);
				}

				int iRightSum = 0;
				for(int k = 0; k < saSeparatedOperands.Length; k++)
				{
					// Find if place exists
					Place pFound = null;
					foreach(Place p in alGroupedPlaces)
					{
						if (p.NameID == saSeparatedOperands[k] || "P" + p.Index.ToString() == saSeparatedOperands[k])
						{
							pFound = p;
							break;
						}
					}
					
					if (pFound != null)
					{
						int iPlaceIndex = alGroupedPlaces.IndexOf(pFound);

						if (k != 0)
						{
							switch(saOperations[k - 1])
							{
								case "+" :
								{
									iRightSum += iaTokensVector[iPlaceIndex];
									break;
								}
								case "-" :
								{
									iRightSum -= iaTokensVector[iPlaceIndex];
									break;
								}
							}
						}
						else
						{
							foreach(Place p in alGroupedPlaces)
							{
								if (p.NameID == saSeparatedOperands[k] || "P" + p.Index.ToString() == saSeparatedOperands[k])
								{
									iRightSum += iaTokensVector[iPlaceIndex];
									break;
								}
							}
						}
					}
					else
					{
						if (k != 0)
						{
							switch(saOperations[k - 1])
							{
								case "+" :
								{
									iRightSum += int.Parse(saSeparatedOperands[k]);
									break;
								}
								case "-" :
								{
									iRightSum -= int.Parse(saSeparatedOperands[k]);
									break;
								}
							}
						}
						else
						{
							iRightSum += int.Parse(saSeparatedOperands[k]);
						}
					}
				}
				#endregion

				#region switch(saConditions[i, 1])
				switch(saConditions[i, 1])
				{
					case "==" :
					{
						bool bCompare = (iLeftSum == iRightSum);
						bCondition &= bCompare;
						break;
					}
					case "!=" :
					{
						bool bCompare = (iLeftSum != iRightSum);
						bCondition &= bCompare;
						break;
					}
					case ">=" :
					{
						bool bCompare = (iLeftSum >= iRightSum);
						bCondition &= bCompare;
						break;
					}
					case "<=" :
					{
						bool bCompare = (iLeftSum <= iRightSum);
						bCondition &= bCompare;
						break;
					}
					case ">" :
					{
						bool bCompare = (iLeftSum > iRightSum);
						bCondition &= bCompare;
						break;
					}
					case "<" :
					{
						bool bCompare = (iLeftSum < iRightSum);
						bCondition &= bCompare;
						break;
					}
				}
				#endregion
			}

			return bCondition;
		}
		#endregion

		#region public char[] ControlVector(PetriNetDocument pnd)
		public char[] ControlVector(PetriNetDocument pnd)
		{
			if (this.Evaluate(pnd) == true)
			{
				ArrayList alControlPlaces = pnd.ControlPlaces;

				char[] caControlVector = new char[alControlPlaces.Count];

				// Initialize all to x
				for(int i = 0; i < caControlVector.Length; i++)
				{
					caControlVector[i] = 'x';
				}

				// Set elements based on rule
				for(int i = 0; i < saResults.GetLength(0); i++)
				{
					foreach(Place p in alControlPlaces)
					{
						if (p.NameID == this.saResults[i, 0] || "P" + p.Index.ToString() == this.saResults[i, 0])
						{
							caControlVector[alControlPlaces.IndexOf(p)] = char.Parse(saResults[i, 1]);
							break;
						}
					}
				}

				return caControlVector;
			}

			return null;
		}
		#endregion

		#region public char[] ControlVector(ArrayList alGroupedPlaces, ArrayList alControlPlaces, int[] iaTokensVector)
		public char[] ControlVector(ArrayList alGroupedPlaces, ArrayList alControlPlaces, int[] iaTokensVector)
		{
			if (this.Evaluate(alGroupedPlaces, iaTokensVector) == true)
			{
				char[] caControlVector = new char[alControlPlaces.Count];

				// Initialize all to x
				for(int i = 0; i < caControlVector.Length; i++)
				{
					caControlVector[i] = 'x';
				}

				// Set elements based on rule
				for(int i = 0; i < saResults.GetLength(0); i++)
				{
					foreach(Place p in alControlPlaces)
					{
						if (p.NameID == this.saResults[i, 0] || "P" + p.Index.ToString() == this.saResults[i, 0])
						{
							caControlVector[alControlPlaces.IndexOf(p)] = char.Parse(saResults[i, 1]);
							break;
						}
					}
				}

				return caControlVector;
			}

			return null;
		}
		#endregion

		#region public bool IsValidCondition(PetriNetDocument pnd)
		public bool IsValidCondition(PetriNetDocument pnd)
		{
			bool bValid = true;

			for (int i = 0; i < this.saConditions.GetLength(0); i++)
			{
				#region Calculate LeftSide of Condition
				// Calculate LeftSide of Condition
				string[] saSeparatedOperands = saConditions[i, 0].Split(new char[]{'+', '-'});

				foreach(string s in saSeparatedOperands)
				{
					int iFound = 0;

					foreach(Place p in pnd.Places)
					{
						if (p.NameID == s  || "P" + p.Index.ToString() == s)
						{
							iFound = 1;
							break;
						}
					}

					if (iFound == 0)
					{
						try
						{
							int.Parse(s);
							iFound = 1;
						}
						catch(FormatException)
						{
							iFound = 0;
						}
					}

					if (iFound == 0)
						bValid = false;
				}
				#endregion

				#region Calculate RightSide of Condition
				// Calculate RightSide of Condition
				saSeparatedOperands = saConditions[i, 2].Split(new char[]{'+', '-'});

				foreach(string s in saSeparatedOperands)
				{
					int iFound = 0;

					foreach(Place p in pnd.Places)
					{
						if (p.NameID == s  || "P" + p.Index.ToString() == s)
						{
							iFound = 1;
							break;
						}
					}

					if (iFound == 0)
					{
						try
						{
							int.Parse(s);
							iFound = 1;
						}
						catch(FormatException)
						{
							iFound = 0;
						}
					}

					if (iFound == 0)
						bValid = false;
				}
				#endregion
			}

			return bValid;
		}
		#endregion
		
		#region public bool IsValidResult(PetriNetDocument pnd)
		public bool IsValidResult(PetriNetDocument pnd)
		{
			bool bValid = true;

			for (int i = 0; i < this.saResults.GetLength(0); i++)
			{
				//Calculate LeftSide of Result
				string s = this.saResults[i, 0];

				int iFound = 0;
				foreach(Place p in pnd.Places)
				{
					if (p.NameID == s  || "P" + p.Index.ToString() == s)
					{
						iFound = 1;
						break;
					}
				}

				if (iFound == 0)
					bValid = false;

				//Calculate RightSide of Result
				s = this.saResults[i, 1];

				iFound = 0;
				try
				{
					int.Parse(s);
					iFound = 1;
				}
				catch(FormatException)
				{
					iFound = 0;
				}

				if (iFound == 0)
					bValid = false;
			}
	
			return bValid;
		}
		#endregion


	}



}
