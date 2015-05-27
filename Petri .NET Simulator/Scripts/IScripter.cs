using System;
using System.Collections.Generic;
using System.Text;

namespace PetriNetSimulator2.Scripts
{
    public interface IScripter
    {
        Boolean InitScript();
        void ResetScript(bool be_quiet);
        Boolean ScriptSingleStep();
    }
}
