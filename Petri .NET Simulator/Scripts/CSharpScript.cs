using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using System.Reflection;
using System.Threading;

namespace PetriNetSimulator2.Scripts
{
    public class CSharpScript : BaseScript, IScripter
    {
        private Assembly _compiledAssembly = null;
        private object _instance = null;
        private Type _type = null;
        private string _expressionError = "";


        public List<string> names = new List<string>();
        public List<int> states = new List<int>();
        public List<string> types = new List<string>();




        public CSharpScript(PetriNetDocument p)
            : base(p)
        { 
        }

        #region IScripter Members

        public bool InitScript()
        {
            Cursor.Current = Cursors.WaitCursor;

            if (pnd.pyCode.Length > 0)
            {
                try
                {
                    string code = TemplateCode.Replace("/*CODE*/", pnd.pyCode);
                    CSharpCodeProvider provider = new CSharpCodeProvider();
                    ICodeCompiler compiler = provider.CreateCompiler();

                    CompilerParameters compilerparams = new CompilerParameters();
                    compilerparams.GenerateExecutable = false;
                    compilerparams.GenerateInMemory = true;
                    compilerparams.ReferencedAssemblies.Add("System.dll");
                    compilerparams.ReferencedAssemblies.Add("System.Windows.Forms.dll");
                    
                    string thisAss = Assembly.ReflectionOnlyLoad(Assembly.GetExecutingAssembly().FullName).Location;
                    compilerparams.ReferencedAssemblies.Add(thisAss);

                    CompilerResults results = compiler.CompileAssemblyFromSource(compilerparams, code);
                    _compiledAssembly = !results.Errors.HasErrors ? results.CompiledAssembly : null;
                    if (_compiledAssembly != null)
                    {
                        _type = _compiledAssembly.GetType("PetriNetSimulator2.ClassWithScriptCode");
                        _instance = Activator.CreateInstance(_type, this);
                        return true;
                    }
                    else
                    {
                        _expressionError = "";
                        foreach (System.CodeDom.Compiler.CompilerError error in results.Errors)
                            _expressionError += String.Format("Line {0}\t: {1}\n",
                                                        error.Line - 5,
                                                        error.ErrorText);

                        this.Script_OnWriteWithColor(_expressionError + "\n", System.Drawing.Color.Red);
                        return false;
                    }

                    
                    Cursor.Current = Cursors.Default;
                    return true;
                }
                catch (Exception e)
                {
                    this.Script_OnWriteWithColor("Error starting C# module\n", System.Drawing.Color.Red);
                    this.Script_OnWriteWithColor(e.Message + "\n", System.Drawing.Color.Red);
                    _compiledAssembly = null;
                    _instance = null;
                    _type = null;
                }
            }
            Cursor.Current = Cursors.Default;
            return false;
        }

        public void ResetScript(bool be_quiet)
        {
            try
            {
                if (_type != null)
                {
                    MethodInfo method = _type.GetMethod("Reset");
                    if (method != null)
                        method.Invoke(_instance, new object[] { });

                    Thread.Sleep(1000);
                    Application.DoEvents();
                }
            }
            catch (Exception e)
            {
                if (!be_quiet)
                    this.Script_OnWriteWithColor(e.Message + "\n", System.Drawing.Color.Red);
            }
            _compiledAssembly = null;
            _instance = null;
            _type = null;

            pnd.pyEditor.Enable();
        }

        public bool ScriptSingleStep()
        {
            //if (_compiledAssembly == null || _instance == null || _type == null)
            //    InitScript();

            if (_instance != null && _type != null)
            {
                try
                {
                    names = new List<string>();
                    states = new List<int>();
                    types = new List<string>();

                    foreach (Place p in pnd.Places)
                    {
                        string varname = p.GetShortString();

                        names.Add(varname);
                        states.Add(p.Tokens);

                        if (p is PlaceInput)
                            types.Add("Input");
                        else if (p is PlaceOperation)
                            types.Add("Operation");
                        else if (p is PlaceResource)
                            types.Add("Resource");
                        else if (p is PlaceOutput)
                            types.Add("Output");
                        else if (p is PlaceControl)
                            types.Add("Control");
                        else if (p is PlaceConverter)
                            types.Add("Converter");
                        else
                            types.Add("?");
                    }

                    //int td = this.pnd.Td;
                    //pyScope.SetVariable("td", td);

                    
                    int tmp = this.pnd.StepCounter;
                    if (this.pnd.PetriNetType == PetriNetType.PTimed)
                        tmp *= this.pnd.Td;


                    if (_type != null)
                    {
                        MethodInfo method = _type.GetMethod("Step");
                        if (method != null)
                            method.Invoke(_instance, new object[] { tmp });
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


        private static String TemplateCode = @"using System;
                                    using System.Collections.Generic;
                                    using System.Text;
                                    using System.Windows.Forms;
                                    using PetriNetSimulator2.Scripts;

                                    namespace PetriNetSimulator2
                                    {
                                        public class ClassWithScriptCode 
                                        { 
                                            /*CODE*/

                                            CSharpScript _script = null;

                                            public ClassWithScriptCode(CSharpScript s)
                                            {
                                                _script = s;
                                            }

                                            public void Print(string evnt)
                                            {
                                                _script.Script_OnWrite(evnt);
                                            }

                                            public Place FindPlace(string name)
                                            {
                                                return _script.Script_FindPlace(name);
                                            }

                                            public List<string> names_vector { get {return _script.names; } }
                                            public List<int> states_vector   { get {return _script.states; } }
                                            public List<string> types_vector { get {return _script.types; } }

                                        }  
                                     }";

    }
}
