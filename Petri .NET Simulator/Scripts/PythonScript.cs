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


namespace PetriNetSimulator2.Scripts
{
    public class PythonScript : BaseScript, IScripter
	{
        private ScriptEngine pyEngine = null;
        private ScriptScope pyScope = null;

        public PythonScript(PetriNetDocument p) : base(p)
        {
        }

        #region Interface memeber

        public Boolean InitScript()
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
                                    "        mysimulator.Script_OnWrite(str)\n" +
                                    "class StderrCatcher:\n" +
                                    "    def write(self, str):\n" +
                                    "        global mysimulator\n" +
                                    "        mysimulator.Script_OnWriteErr(str)\n" +
                                    "sys.stdout = StdoutCatcher()\n" +
                                    "sys.stderr = StderrCatcher()\n" +
                                    "def FindPlace(str):\n" +
                                    "    global mysimulator\n" +
                                    "    return mysimulator.Script_FindPlace(str)\n";

                    pyEngine.CreateScriptSourceFromString(code, SourceCodeKind.Statements).Compile().Execute(pyScope);
                    pnd.pyEditor.Disable();
                    pnd.pyOutput.Clear();
                    Cursor.Current = Cursors.Default;
                    return true;
                }
                catch (Exception e)
                {
                    this.Script_OnWriteWithColor("Error starting Python module\n", System.Drawing.Color.Red);
                    this.Script_OnWriteWithColor(e.Message + "\n", System.Drawing.Color.Red);
                    pyEngine = null;
                    pyScope = null;
                }
            }
            Cursor.Current = Cursors.Default;
            return false;
        }

        public void ResetScript(bool be_quiet)
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
                                        this.Script_OnWriteWithColor(e.Message + "\n", System.Drawing.Color.Red);
                        }
                        pyEngine = null;
                }

                if (pyScope != null)
                        pyScope = null;

                pnd.pyEditor.Enable();
        }

        public Boolean ScriptSingleStep()
        {
            if (pyEngine == null || pyScope == null)
                InitScript();

            if (pyEngine != null && pyScope != null)
            {
                try
                {
                    this.RecalculateVectors();

                    for(int idx = 0; idx < names.Count; idx++)
                        pyScope.SetVariable(names[idx], states[idx]);

                    pyScope.SetVariable("names_vector", names);
                    pyScope.SetVariable("states_vector", states);
                    pyScope.SetVariable("types_vector", types);

                    pyScope.SetVariable("tstates_vector", states);
                    pyScope.SetVariable("tnames_vector", names);

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
                    this.Script_OnWriteWithColor(ex.Message + "\n", System.Drawing.Color.Red);
                }
            }
            return false;
        }

        #endregion
	}
}
