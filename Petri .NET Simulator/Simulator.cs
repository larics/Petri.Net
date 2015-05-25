using System;
using System.Collections;
using System.Threading;
using System.ComponentModel;
using System.Windows.Forms;

using IronPython.Hosting;
using IronPython.Runtime;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using System.Collections.Generic;


namespace PetriNetSimulator2
{
	/// <summary>
	/// Summary description for Simulator.
	/// </summary>

	public class Simulator : IDisposable 
	{
		// Properties
		#region public bool Running
		public bool Running
		{
			get
			{
				return this.bRunning;
			}
		}
		#endregion

		//Fields
		private PetriNetDocument pnd;
		private Thread tdSimulate;
		private bool bRunning = false;
		private Hashtable ht = new Hashtable(); // Used for saving token data

        private ScriptEngine pyEngine = null;
        private ScriptScope pyScope = null;


		//Events
		public event EventHandler SimulationFinished;
		public event EventHandler SimulationStepFinished;
		public event EventHandler SimulationProcessFinished;


        public int sleepBetweenStep = 1000;
        public bool ignoreLackOfFireableTransition = false;

		public Simulator(PetriNetDocument pnd)
		{
			this.pnd = pnd;
			pnd.Simulator = this;
		}

        ~Simulator()
        {
            Dispose(false);
        }

        private bool IsDisposed = false;
        public void Dispose()
        {
            //Pass true in dispose method to clean managed resources too and say GC to skip finalize in next line.
            Dispose(true);
            //If dispose is called already then say GC to skip finalize on this instance.
            GC.SuppressFinalize(this);
        }
        //Implement dispose to free resources

        protected virtual void Dispose(bool disposedStatus)
        {
            if (!IsDisposed)
            {
                IsDisposed = true;
                // Released unmanaged Resources
                if (disposedStatus)
                {
                    // Released managed Resources
                    try
                    {
                        ResetPython(true);
                    }
                    catch
                    {

                    }
                }
            }
        }

		#region private void Simulate()
		private void Simulate()
		{
			#region if (this.pnd.PetriNetType == PetriNetType.TimeInvariant)
			if (this.pnd.PetriNetType == PetriNetType.TimeInvariant)
			{
				// TODO : Deselect all selected controls in pnd

                SaveDataForCharting(0);
				while(true)
				{
					this.Step();

                    SaveDataForCharting(this.pnd.StepCounter);

					// Find is there any transition that can fire

                    if (this.pnd.FireableTransitions.Count != 0 || 
                        this.ignoreLackOfFireableTransition)
                    {
                        Thread.Sleep(this.sleepBetweenStep);    // 1000 by default.. 
                        Application.DoEvents();
                    }
                    else
                        break;
				}

				this.bRunning = false;

				// Raise SimulationFinished event
				if(this.SimulationFinished != null)
					this.SimulationFinished(this, new EventArgs());
			}
			#endregion

			#region else if (this.pnd.PetriNetType == PetriNetType.PTimed)
			else if (this.pnd.PetriNetType == PetriNetType.PTimed)
			{
				// TODO : Deselect all selected controls in pnd

				// Initialize
				IntMatrix imk = new IntMatrix(this.pnd.GroupedPlaces.Count, 1);

				IntMatrix imF = this.pnd.F;
				IntMatrix Ft = this.pnd.F.Transpose();
				IntMatrix Wt = this.pnd.W.Transpose();

				ArrayList alInputPlaces = this.pnd.InputPlaces;
				ArrayList alOperationPlaces = this.pnd.OperationPlaces;
				ArrayList alResourcePlaces = this.pnd.ResourcePlaces;
				ArrayList alControlPlaces = this.pnd.ControlPlaces;
				ArrayList alOutputPlaces = this.pnd.OutputPlaces;
				ArrayList alGroupedPlaces = this.pnd.GroupedPlaces;
				ArrayList alTransitions = this.pnd.Transitions;

				IntMatrix Sv = this.pnd.Sv;
				IntMatrix Sr = this.pnd.Sr;
				IntMatrix Sd = this.pnd.Sd;
				IntMatrix Sy = this.pnd.Sy;

				IntMatrix Tv0 = this.pnd.Tv0;
				IntMatrix Tr0 = this.pnd.Tr0;
				IntMatrix Td0 = this.pnd.Td0;
				IntMatrix Ty0 = this.pnd.Ty0;

				IntMatrix[] Tv = new IntMatrix[1];
				Tv[0] = new IntMatrix(Tv0.Dimensions.Height, Tv0.Dimensions.Width);

				IntMatrix[] Tr = new IntMatrix[1];
				Tr[0] = new IntMatrix(Tr0.Dimensions.Height, Tr0.Dimensions.Width);

				IntMatrix[] Td = new IntMatrix[1];
				Td[0] = new IntMatrix(Td0.Dimensions.Height, Td0.Dimensions.Width);

				IntMatrix[] Ty = new IntMatrix[1];
				Ty[0] = new IntMatrix(Ty0.Dimensions.Height, Ty0.Dimensions.Width);

				// Initial condition
				for (int i = 0; i < alGroupedPlaces.Count; i++)
				{
					if (ht.Count != 0)
					{
						imk[i, 0] = (int)ht[alGroupedPlaces[i]];
					}
					else
						imk[i, 0] = ((Place)alGroupedPlaces[i]).Tokens;
				}

				// Get mks
				IntMatrix imks = imk;

				// Get index of first control place
				int iIndexOfFirstControlPlace = -1;
				if (alControlPlaces.Count != 0)
				{
					iIndexOfFirstControlPlace = alGroupedPlaces.IndexOf(alControlPlaces[0]);
				}

				#region Set inputs
				// Set inputs
				Hashtable htInputTokens = new Hashtable();
				foreach(PlaceInput pi in pnd.InputPlaces)
				{
					if (pi.InputType == InputType.Periodic)
					{
						ArrayList alInputTimes = new ArrayList();

						int iTime = 0;
						while (iTime < this.pnd.EndTime/this.pnd.Td)
						{
							iTime += pi.Interval;
							alInputTimes.Add(iTime);
						}

						htInputTokens.Add(pi, alInputTimes);
					}
					else if (pi.InputType == InputType.Stohastic)
					{
						ArrayList alInputTimes = new ArrayList();
						Random r = new Random(Environment.TickCount);
						int iTime = 0;
						while (iTime < this.pnd.EndTime/this.pnd.Td)
						{
							int i = r.Next(1, pi.RandomInterval);
							iTime += i;
							alInputTimes.Add(iTime);
						}

						htInputTokens.Add(pi, alInputTimes);
					}
				}
				#endregion

                SaveDataForCharting(0);
				// Simulate
				for (int k = 1; k <= this.pnd.EndTime/this.pnd.Td; k++)
				{
					foreach(Transition t in alTransitions)
					{
                        t.RefreshMT();
					}

                    Thread.Sleep(this.pnd.Td);
                    Application.DoEvents();


					#region Set control vector
					// Set control vector

                    bool isPhytonExecutedOK = PythonSingleStep();
                    Tv0 = this.pnd.Tv0; // Refresh duration information.. 

					if (alControlPlaces.Count != 0)
					{
                        if (isPhytonExecutedOK)
                        {
                            for (int i = 0; i < alControlPlaces.Count; i++)
                            {
                                imk[iIndexOfFirstControlPlace + i, 0] = ((Place)alControlPlaces[i]).Tokens;
                            }
                        }
                        else
                        {
                            // Set all control places tokens to 1 if they don't have parents
                            for (int i = 0; i < alControlPlaces.Count; i++)
                            {
                                if (((Place)alControlPlaces[i]).Parents.Count == 0)
                                    imk[iIndexOfFirstControlPlace + i, 0] = 1;
                            }

                        }

                        this.pnd.StepCounter++;

						foreach(Rule r in this.pnd.Rules)
						{
							int[] iaTokensVector = new int[alGroupedPlaces.Count];

							for (int i = 0; i < iaTokensVector.Length; i++)
							{
								iaTokensVector[i] = (int)imk[i, 0];
							}

							char[] ca = r.ControlVector(alGroupedPlaces, alControlPlaces, iaTokensVector);

							if (ca != null)
							{
								for(int i = 0; i < ca.Length; i++)
								{
									if (ca[i] != 'x')
									{
										if (((Place)this.pnd.ControlPlaces[i]).Parents.Count == 0)
											imk[iIndexOfFirstControlPlace + i, 0] = int.Parse(ca[i].ToString());
									}
								}
							}
						}
					}
					#endregion

					#region Find transitions that can fire
					// Find transitions that can fire
					IntMatrix imT = new IntMatrix(alTransitions.Count, 1);

					for (int i = 0; i < alTransitions.Count; i++)
					{
						Transition t = (Transition)alTransitions[i];
						ArrayList alPlaceParents = t.PlaceParents;

						int iCountMetConditions = 0;
						for (int j = 0; j < alPlaceParents.Count; j++)
						{
							Place p = (Place)alPlaceParents[j];

							if ((int)imk[alGroupedPlaces.IndexOf(p), 0] >= imF[i, alGroupedPlaces.IndexOf(p)])
							{
								iCountMetConditions++;
							}
						}

						if (iCountMetConditions == alPlaceParents.Count && alPlaceParents.Count != 0)
							imT[i, 0] = 1;
					}
					#endregion

					// Calculate mks+1
					imks = imks + Wt * imT;

					#region Determine maximum tokens in one place
					// Determine maximum tokens in one place
					int iMaxTokens = 0;
					IntMatrix imOpsAndRes = new IntMatrix(this.pnd.OperationPlaces.Count + this.pnd.ResourcePlaces.Count, 1);
					for(int z = 0; z < this.pnd.OperationPlaces.Count; z++)
					{
						imOpsAndRes[z, 0] = imks[this.pnd.InputPlaces.Count + z, 0];
					}
					for(int z = 0; z < this.pnd.ResourcePlaces.Count; z++)
					{
						imOpsAndRes[this.pnd.OperationPlaces.Count + z, 0] = imks[this.pnd.InputPlaces.Count + this.pnd.OperationPlaces.Count + z, 0];
					}

					iMaxTokens = imOpsAndRes.Max();
					#endregion

					#region Adjust sizes of Tx matrixes
					// Adjust sizes of Tx matrixes
					if (iMaxTokens > Tv.Length)
					{
						IntMatrix[] Tvx = new IntMatrix[iMaxTokens];
						for (int i = 0; i < iMaxTokens; i++)
						{
							Tvx[i] = new IntMatrix(Tv0.Dimensions.Height, Tv0.Dimensions.Width);
						}
						Tv.CopyTo(Tvx, 0);
						Tv = Tvx;

						IntMatrix[] Trx = new IntMatrix[iMaxTokens];
						for (int i = 0; i < iMaxTokens; i++)
						{
							Trx[i] = new IntMatrix(Tr0.Dimensions.Height, Tr0.Dimensions.Width);
						}
						Tr.CopyTo(Trx, 0);
						Tr = Trx;

						IntMatrix[] Tdx = new IntMatrix[iMaxTokens];
						for (int i = 0; i < iMaxTokens; i++)
						{
							Tdx[i] = new IntMatrix(Td0.Dimensions.Height, Td0.Dimensions.Width);
						}
						Td.CopyTo(Tdx, 0);
						Td = Tdx;

						IntMatrix[] Tyx = new IntMatrix[iMaxTokens];
						for (int i = 0; i < iMaxTokens; i++)
						{
							Tyx[i] = new IntMatrix(Ty0.Dimensions.Height, Ty0.Dimensions.Width);
						}
						Ty.CopyTo(Tyx, 0);
						Ty = Tyx;
					}
					#endregion

					if (this.pnd.Editor.PauseBeforeFiring == true && k != 1)
					{
						if (imT.Max() == 1)
						{
							if(this.SimulationProcessFinished != null)
								this.SimulationProcessFinished(this, EventArgs.Empty);
						}
					}

					// Calculate Tx_temp matrixes
					IntMatrix Tv_temp = IntMatrix.MulTim(Tv0, imT);
					IntMatrix Tr_temp = IntMatrix.MulTim(Tr0, imT);
					IntMatrix Td_temp = IntMatrix.MulTim(Td0, imT);
					IntMatrix Ty_temp = IntMatrix.MulTim(Ty0, imT);

					// Adjust all Connections
					foreach(Connection cn in this.pnd.ConnectionsAll)
					{
						cn.TokenPositions = new ArrayList(Tv.Length);
						for(int h = 0; h < Tv.Length; h++)
							cn.TokenPositions.Add(0);
					}

					try
					{
						#region Adjust Tv matrix
						// Put new values in table
						for (int m = 0; m < Tv0.Dimensions.Height; m++)
						{
							for (int l = 0; l < Tv0.Dimensions.Width; l++)
							{
								if (Tv_temp[m, l] > 0)
								{
									// Find first available position in Tv matrix
									int iAvailable = -1;
									for (int z = 0; z < Tv.Length; z++)
									{
										if (Tv[z][m, l] == 0)
										{
											iAvailable = z;
											break;
										}
									}

									if (iAvailable == -1)
										throw new ArgumentOutOfRangeException("No free layer. Max tokens number in one place has been exceeded.");

									Tv[iAvailable][m, l] = Tv_temp[m, l];
									Tv_temp[m, l] = 0;

									((PlaceOperation)alOperationPlaces[m]).MaxFillAngle = Tv[iAvailable][m, l];
									((PlaceOperation)alOperationPlaces[m]).FillAngle = Tv[iAvailable][m, l];
								}
							}
						}

						// Decrement all values by 1
						for (int ctn = 0; ctn < Tv.Length; ctn++)
						{
							for (int m = 0; m < Tv0.Dimensions.Height; m++)
							{
								for (int l = 0; l < Tv0.Dimensions.Width; l++)
								{
									if (Tv[ctn][m, l] > 0)
									{
										if (Tv[ctn][m, l] == 1)
											imk[m + alInputPlaces.Count, 0] = imk[m + alInputPlaces.Count, 0] + Sv[m, l];

										Tv[ctn][m, l] = Tv[ctn][m, l] - 1;

										if (Tv[ctn][m, l] < ((PlaceOperation)alOperationPlaces[m]).FillAngle || ((PlaceOperation)alOperationPlaces[m]).FillAngle == 0)
										{
											((PlaceOperation)alOperationPlaces[m]).FillAngle = Tv[ctn][m, l];
										}

										if(this.pnd.TokenGameAnimation == true)
										{
											// Get connection
											Connection cn = Connection.GetConnectionBetweenControls((ConnectableControl)alTransitions[l], (ConnectableControl)alOperationPlaces[m]);

											int i = (int)((((PlaceOperation)alOperationPlaces[m]).MaxFillAngle - Tv[ctn][m, l]) * 100f/(float)((PlaceOperation)alOperationPlaces[m]).MaxFillAngle);
											
											if (i < 100)
												cn.TokenPositions[ctn] = i;
											else
												cn.TokenPositions[ctn] = 0;
										}
									}
								}
							}
						}

						#endregion

						#region Adjust Tr matrix
						// Put new values in table
						for (int m = 0; m < Tr0.Dimensions.Height; m++)
						{
							for (int l = 0; l < Tr0.Dimensions.Width; l++)
							{
								if (Tr_temp[m, l] > 0)
								{
									// Find first available position in Tr matrix
									int iAvailable = -1;
									for (int z = 0; z < Tr.Length; z++)
									{
										if (Tr[z][m, l] == 0)
										{
											iAvailable = z;
											break;
										}
									}

									if (iAvailable == -1)
										throw new ArgumentOutOfRangeException("No free layer. Max tokens number in one place has been exceeded.");

									Tr[iAvailable][m, l] = Tr_temp[m, l];
									Tr_temp[m, l] = 0;

									((PlaceResource)alResourcePlaces[m]).MaxFillAngle = Tr[iAvailable][m, l];
									((PlaceResource)alResourcePlaces[m]).FillAngle = Tr[iAvailable][m, l];
								}
							}
						}

						// Decrement all values by 1
						for (int ctn = 0; ctn < Tr.Length; ctn++)
						{
							for (int m = 0; m < Tr0.Dimensions.Height; m++)
							{
								for (int l = 0; l < Tr0.Dimensions.Width; l++)
								{
									if (Tr[ctn][m, l] > 0)
									{
										if (Tr[ctn][m, l] == 1)
											imk[m + alInputPlaces.Count + alOperationPlaces.Count, 0] = imk[m + alInputPlaces.Count + alOperationPlaces.Count, 0] + Sr[m, l];

										Tr[ctn][m, l] = Tr[ctn][m, l] - 1;

										if (Tr[ctn][m, l] < ((PlaceResource)alResourcePlaces[m]).FillAngle || ((PlaceResource)alResourcePlaces[m]).FillAngle == 0)
										{
											((PlaceResource)alResourcePlaces[m]).FillAngle = Tr[ctn][m, l];
										}

										if(this.pnd.TokenGameAnimation == true)
										{
											// Get connection
											Connection cn = Connection.GetConnectionBetweenControls((ConnectableControl)alTransitions[l], (ConnectableControl)alResourcePlaces[m]);

											int i = (int)((((PlaceResource)alResourcePlaces[m]).MaxFillAngle - Tr[ctn][m, l]) * 100f/(float)((PlaceResource)alResourcePlaces[m]).MaxFillAngle);
											
											if (i < 100)
												cn.TokenPositions[ctn] = i;
											else
												cn.TokenPositions[ctn] = 0;
										}
									}
								}
							}
						}
						#endregion

						#region Adjust Td matrix
						for (int ctn = 0; ctn < iMaxTokens; ctn++)
						{
							for (int m = 0; m < Td0.Dimensions.Height; m++)
							{
								for (int l = 0; l < Td0.Dimensions.Width; l++)
								{
									if (Td_temp[m, l] > 0)
									{
										if (Td[ctn][m, l] > 0)
										{
											Td[ctn+1][m, l] = Td_temp[m, l];
											Td_temp[m, l] = 0;
										}
										else
										{
											Td[ctn][m, l] = Td_temp[m, l];
											Td_temp[m, l] = 0;
										}
									}

									if (Td[ctn][m, l] == 1)
									{
										imk[m + alInputPlaces.Count + alOperationPlaces.Count + alResourcePlaces.Count, 0] = imk[m + alInputPlaces.Count + alOperationPlaces.Count + alResourcePlaces.Count, 0] + Sd[m, l];
										Td[ctn][m, l] = Td[ctn][m, l] - 1;
									}
								}
							}
						}
						#endregion

						#region Adjust Ty matrix
						for (int ctn = 0; ctn < iMaxTokens; ctn++)
						{
							for (int m = 0; m < Ty0.Dimensions.Height; m++)
							{
								for (int l = 0; l < Ty0.Dimensions.Width; l++)
								{
									if (Ty_temp[m, l] > 0)
									{
										if (Ty[ctn][m, l] > 0)
										{
											Ty[ctn+1][m, l] = Ty_temp[m, l];
											Ty_temp[m, l] = 0;
										}
										else
										{
											Ty[ctn][m, l] = Ty_temp[m, l];
											Ty_temp[m, l] = 0;
										}
									}

									if (Ty[ctn][m, l] > 0)
									{
										imk[m + alInputPlaces.Count + alOperationPlaces.Count + alResourcePlaces.Count + alControlPlaces.Count, 0] = imk[m + alInputPlaces.Count + alOperationPlaces.Count + alResourcePlaces.Count + alControlPlaces.Count, 0] + Sy[m, l];
										Ty[ctn][m, l] = Ty[ctn][m, l] - 1;
									}
								}
							}
						}
						#endregion
					}
					catch(ArgumentOutOfRangeException e)
					{
						MessageBox.Show(e.ToString());
						break;
					}

					// Calculate mk+1
					imk = imk - Ft * imT;

					#region Set Inputs
					// Set Inputs
					if (pnd.InputPlaces.Count != 0)
					{
						for(int i = 0; i < pnd.InputPlaces.Count; i++)
						{
							PlaceInput pi = (PlaceInput)pnd.InputPlaces[i];
							if (pi.InputType != InputType.Fixed)
							{
								if (((ArrayList)htInputTokens[pi]).Contains(k))
								{
									imk[i, 0] += 1;
									imks[i, 0] += 1;
								}
							}
						}
					}
					#endregion

                    this.pnd.Editor.RefreshMT();

					// Raise SimulationStepFinished event
					if(this.SimulationStepFinished != null)
						this.SimulationStepFinished(this, new EventArgs());

					for(int j = 0; j < this.pnd.Places.Count; j++)
					{
						((Place)this.pnd.GroupedPlaces[j]).Tokens = imk[j, 0];
					}
                    this.pnd.StepCounter++;
                    SaveDataForCharting(this.pnd.StepCounter);
                }

				this.bRunning = false;

				// Raise SimulationFinished event
				if(this.SimulationFinished != null)
					this.SimulationFinished(this, new EventArgs());
			}
			#endregion
		}
		#endregion


        #region Save Data for Charting

        public List<List<int>> resultDataFromSimulation = null;
        public void SaveDataForCharting(int k)
        {
            List<int> row = new List<int>();

            if (k == 0)    // Reinitialize and begin ... 
                resultDataFromSimulation = new List<List<int>>();

            for (int i = 0; i < pnd.GroupedPlaces.Count; i++)
                row.Add(((Place)pnd.GroupedPlaces[i]).Tokens);

            resultDataFromSimulation.Add(row);
        }
        #endregion

        #region public IntMatrix SimulateToMatrix(int iEndTime)
        public IntMatrix SimulateToMatrix(int iEndTime)
		{
            IntMatrix im = new IntMatrix(pnd.GroupedPlaces.Count, iEndTime + 1);
            try
            {
                if (resultDataFromSimulation != null)
                {
                    for (int idx = 0; idx < iEndTime; idx++)
                    {
                        int k = idx;

                        if (k < resultDataFromSimulation.Count)
                        {
                            List<int> row = resultDataFromSimulation[k];
                            for (int i = 0; i < row.Count; i++)
                                im[i, idx] = (int)row[i];
                        }
                    }
                }
            }
            catch (Exception ex)
            { 
            }
            return im;
        }
		#endregion

		#region public void Start()

        public void Python_OnWrite(string s) 
        {
            // pnd.pyOutput.Print(s);
            // pnd.pyOutput.PrintMT(s);
            Python_OnWriteWithColor(s, System.Drawing.Color.Blue);
        }

        public void Python_OnWriteErr(string s)
        {
            // pnd.pyOutput.Print(s);
            //pnd.pyOutput.PrintMT(s);
            Python_OnWriteWithColor(s, System.Drawing.Color.Red);
        }


        private void Python_OnWriteWithColor(string s, System.Drawing.Color c)
        {
            // pnd.pyOutput.PrintMT(s);
            pnd.pyOutput.PrintMTColor(s, c);
        }


        public object Python_FindPlace(string nameID)
        {
            foreach (Place p in pnd.Places)
            {
                if ( 
                    (!String.IsNullOrEmpty(p.NameID) && p.NameID.Equals(nameID) ) || 
                    (!String.IsNullOrEmpty(p.Name) && p.Name.Equals(nameID) )
                   )
                    return p;
            }
            return null;
        }


        public Boolean InitPython()
        {
            Cursor.Current = Cursors.WaitCursor;

            if (pnd.pyCode.Length > 0)
            {
                try
                {
                    pyEngine = Python.CreateEngine();
                    pyScope = pyEngine.CreateScope();

                    //string version = "IronPython "+pyEngine.LanguageVersion.ToString()+"\n\r";
                    //Python_OnWriteWithColor(version, System.Drawing.Color.Green);

                    // Setting the output streams
                    pyEngine.CreateScriptSourceFromString(pnd.pyCode, SourceCodeKind.Statements).Compile().Execute(pyScope);

                    pyScope.SetVariable("mysimulator", this);
                    string code = "import sys\n" +
                                  "class StdoutCatcher:\n" +
                                  "    def write(self, str):\n" +
                                  "        global mysimulator\n" +
                                  "        mysimulator.Python_OnWrite(str)\n" +
                                  "class StderrCatcher:\n" +
                                  "    def write(self, str):\n" +
                                  "        global mysimulator\n" +
                                  "        mysimulator.Python_OnWriteErr(str)\n" +
                                  "sys.stdout = StdoutCatcher()\n" +
                                  "sys.stderr = StderrCatcher()\n" +
                                  "def FindPlace(str):\n" +
                                  "    global mysimulator\n" +
                                  "    return mysimulator.Python_FindPlace(str)\n";

                    pyEngine.CreateScriptSourceFromString(code, SourceCodeKind.Statements).Compile().Execute(pyScope);
                    pnd.pyEditor.Disable();
                    pnd.pyOutput.Clear();
                    Cursor.Current = Cursors.Default;
                    return true;
                }
                catch (Exception e)
                {
                    this.Python_OnWriteWithColor("Error starting Python module\n", System.Drawing.Color.Red);
                    this.Python_OnWriteWithColor(e.Message + "\n", System.Drawing.Color.Red);
                    pyEngine = null;
                    pyScope = null;
                }
            }
            Cursor.Current = Cursors.Default;
            return false;
        }

        public void ResetPython(bool be_quiet)
        {
            if (pyEngine != null)
            {
                try
                {
                    pyEngine.CreateScriptSourceFromString("Reset()", SourceCodeKind.Statements).Compile().Execute(pyScope);
                    Thread.Sleep(1000);
                    Application.DoEvents();
                }
                catch (Exception e)
                {
                    if (!be_quiet)
                        this.Python_OnWriteWithColor(e.Message + "\n", System.Drawing.Color.Red);
                }
                pyEngine = null;
            }

            if (pyScope != null)
                pyScope = null;

            pnd.pyEditor.Enable();
        }

        public Boolean PythonSingleStep()
        {
            if (pyEngine == null || pyScope == null)
                InitPython();

            if (pyEngine != null && pyScope != null)
            {
                try
                {
                    List<string> names = new List<string>();
                    List<int> states = new List<int>();

                    foreach (Place p in pnd.Places)
                    {
                        string varname = p.GetShortString();
                        pyScope.SetVariable(varname, p.Tokens);

                        names.Add(varname);
                        states.Add(p.Tokens);
                    }

                    pyScope.SetVariable("names_vector", names);
                    pyScope.SetVariable("states_vector", states);

                    //int td = this.pnd.Td;
                    //pyScope.SetVariable("td", td);

                    string timeinfo;
                    int tmp = this.pnd.StepCounter;
                    if (this.pnd.PetriNetType == PetriNetType.PTimed)
                        tmp *= this.pnd.Td;

                    timeinfo = tmp.ToString();
                    pyEngine.CreateScriptSourceFromString("Step("+timeinfo+")", SourceCodeKind.Statements).Compile().Execute(pyScope);

                    foreach (Place p in pnd.Places)
                    {
                        string varname = p.GetShortString();
                        p.Tokens = (int)pyScope.GetVariable(varname);
                    }
                    return true;

                }
                catch (Exception ex)
                {
                    this.Python_OnWriteWithColor(ex.Message + "\n", System.Drawing.Color.Red);
                }
            }
            return false;
        }


		public void Start()
		{
			if (bRunning == false)
			{
				// Backup all tokens data that can later be used to restore inital setting
				SaveTokensData();
                this.pnd.StepCounter = 0;
                InitPython();
				// Start simulation thread
				this.tdSimulate = new Thread(new ThreadStart(Simulate));
				tdSimulate.Start();

				this.bRunning = true;
			}
			else
			{
				this.tdSimulate.Resume();
			}
		}
		#endregion

		#region public void Stop()
		public void Stop()
		{
			// Abort simulation thread
			if ((int)tdSimulate.ThreadState == 96 || tdSimulate.ThreadState == ThreadState.Suspended)
				this.tdSimulate.Resume();

			this.tdSimulate.Abort();

			this.bRunning = false;

			this.Reset();
		}
		#endregion

		#region public void Reset()
		public void Reset()
		{
            this.pnd.StepCounter = 0;
			// Restore all tokens data
			RestoreTokensData();

			// Refresh all transitions
			foreach(Transition t in pnd.Transitions)
			{
				t.Fired = false;
			}

			// Refresh all places
			foreach(PlaceOperation po in pnd.OperationPlaces)
			{
				po.FillAngle = 0;
			}
			foreach(PlaceResource pr in pnd.ResourcePlaces)
			{
				pr.FillAngle = 0;
			}

			//RefreshAllConnections
			foreach(Connection cn in this.pnd.Connections)
			{
				cn.TokenPositions.Clear();
			}
			this.pnd.Editor.Refresh();

			// Raise SimulationStepFinished event
			if (this.SimulationStepFinished != null)
				this.SimulationStepFinished(this, new EventArgs());

            ResetPython(false);
		}
		#endregion

		#region public void Pause()
		public void Pause()
		{
			this.tdSimulate.Suspend();
		}
		#endregion

		#region public void Step()
		public void Step()
		{
			// Save all tokens data
			SaveTokensData();

			if (this.pnd.PetriNetType == PetriNetType.TimeInvariant)
			{
                if (PythonSingleStep())
                {
                    // Do nothing.. 
                    // We expect that Python code take care of contol places
                }
                else
                {
                    // Set all control places tokens to value 1 if place doesn't have parents
                    foreach (Place p in pnd.ControlPlaces)
                    {
                        if (p.Parents.Count == 0)
                        {
                            p.Tokens = 1;
                        }
                    }
                }
				// Set TokenPositions in all connections to ArrayLists with size 1
				foreach(Connection cn in this.pnd.ConnectionsAll)
				{
					cn.TokenPositions = new ArrayList();
					cn.TokenPositions.Add(0);
				}

				#region Set control places tokens based on rules evaluation
				// Set control places tokens based on rules evaluation
				foreach(Rule r in this.pnd.Rules)
				{
					char[] ca = r.ControlVector(this.pnd);

					if (ca != null)
					{
						for(int i = 0; i < ca.Length; i++)
						{
							if (ca[i] != 'x')
							{
								if (((Place)pnd.ControlPlaces[i]).Parents.Count == 0)
									((Place)pnd.ControlPlaces[i]).Tokens = int.Parse(ca[i].ToString());
							}
						}
					}
				}
				#endregion

				// Refresh all transitions
				foreach(Transition t in pnd.Transitions)
				{
					t.Fired = false;
				}

				#region Draw token-game animation
				// Draw token-game animation
				if (this.pnd.TokenGameAnimation == true)
				{
					for(int i = 0; i <= 50; i++)
					{
						foreach(Transition t in pnd.FireableTransitions)
						{
							foreach(ConnectableControl cc in t.Childs)
							{
								Connection cn = Connection.GetConnectionBetweenControls(t, cc);
								cn.TokenPositions[0] = (int)cn.TokenPositions[0] + 2;
							}
						}

						this.pnd.Editor.Refresh();
					}

					foreach(Transition t in pnd.FireableTransitions)
					{
						foreach(ConnectableControl cc in t.Childs)
						{
							Connection cn = Connection.GetConnectionBetweenControls(t, cc);
							cn.TokenPositions[0] = 0;
						}
					}
					this.pnd.Editor.Refresh();
				}
				#endregion

				// Fire all fireable transitions
				foreach (Transition t in pnd.FireableTransitions)
				{
					t.Fire();
					t.Fired = true;
				}

				// Refresh all transitions
				foreach(Transition t in pnd.Transitions)
				{
                    try 
                    {
                        t.RefreshMT();
                    }
                    catch { }
				}

				// Raise SimulationStepFinished event
				if (this.SimulationStepFinished != null)
					this.SimulationStepFinished(this, new EventArgs());

                this.pnd.StepCounter++;
			}
		}
		#endregion

		#region private void SaveTokensData()
		private void SaveTokensData()
		{
			foreach(Place p in pnd.Places)
			{
				if (this.ht.ContainsKey(p) != true)
					this.ht.Add(p, p.Tokens);
			}
		}
		#endregion

		#region private void RestoreTokensData()
		private void RestoreTokensData()
		{
			foreach(Place p in pnd.Places)
			{
				if (this.ht.ContainsKey(p) == true)
					p.Tokens = (int)this.ht[p];
			}

			ht.Clear();
		}
		#endregion

		#region public void GoTo(int[] ia)
		public void GoTo(int[] ia)
		{
			SaveTokensData();

			for (int i = 0; i < pnd.GroupedPlaces.Count; i++)
			{
				((Place)pnd.GroupedPlaces[i]).Tokens = ia[i];
			}

			// Raise SimulationStepFinished event
			if (this.SimulationStepFinished != null)
				this.SimulationStepFinished(this, new EventArgs());
		}
		#endregion
	}

	[TypeConverter(typeof(SimulatorOptionsConverter))]
	public class SimulatorOptions
	{
		// Properties
		[CommonProperties]
		public bool StopOnConflicts
		{
			get
			{
				return this.bStopOnConflicts;
			}
			set
			{
				this.bStopOnConflicts = value;
			}
		}

		//Fields
		private bool bStopOnConflicts = true;

		public SimulatorOptions()
		{

		}

	}

	public class SimulatorOptionsConverter : ExpandableObjectConverter
	{

	}
}
