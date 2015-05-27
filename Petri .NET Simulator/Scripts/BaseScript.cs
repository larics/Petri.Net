using System;
using System.Collections.Generic;
using System.Text;

namespace PetriNetSimulator2.Scripts
{
    public class BaseScript
    {
        protected PetriNetDocument pnd;

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


    }
}
