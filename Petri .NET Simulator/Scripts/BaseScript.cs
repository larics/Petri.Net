using System;
using System.Collections.Generic;
using System.Text;

namespace PetriNetSimulator2.Scripts
{
    public class BaseScript
    {
        protected PetriNetDocument pnd;

        public List<string> names = new List<string>();
        public List<int> states = new List<int>();
        public List<string> types = new List<string>();
        
        public List<int> tstates = new List<int>();
        public List<string> tnames = new List<string>();

        public BaseScript(PetriNetDocument p)
        {
            pnd = p;
        }

        #region Functions to be called and(or used from Python

        public void Script_OnWrite(string s)
        {
            Script_OnWriteWithColor(s, System.Drawing.Color.Blue);
        }

        public void Script_OnWriteErr(string s)
        {
            Script_OnWriteWithColor(s, System.Drawing.Color.Red);
        }

        public void Script_OnWriteWithColor(string s, System.Drawing.Color c)
        {
            pnd.pyOutput.PrintMTColor(s, c);
        }

        public Place Script_FindPlace(string nameID)
        {
            foreach (Place p in pnd.Places)
            {
                if (
                    (!String.IsNullOrEmpty(p.NameID) && p.NameID.Equals(nameID)) ||
                    (!String.IsNullOrEmpty(p.Name) && p.Name.Equals(nameID))
                   )
                    return p;
            }
            return null;
        }

        #endregion

        public void RecalculateVectors()
        {
            names = new List<string>();
            states = new List<int>();
            types = new List<string>();

            tnames = new List<string>();
            tstates = new List<int>();

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

            foreach(Transition t in pnd.Transitions)
            {
                tstates.Add(pnd.FireableTransitions.Contains(t) ? 1 : 0);
                tnames.Add(t.GetShortString());
            }
        }


    }
}
