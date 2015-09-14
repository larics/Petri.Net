using System;
using System.Collections.Generic;
using System.Text;
using ICSharpCode.TextEditor.Document;
using System.IO;
using System.Reflection;
using System.Xml;

namespace PetriNetSimulator2.Syntax
{
    public class SyntaxModeFileProvider : ISyntaxModeFileProvider
    {
        List<SyntaxMode> syntaxModes = null;

        public ICollection<SyntaxMode> SyntaxModes
        {
            get
            {
                return syntaxModes;
            }
        }

        public SyntaxModeFileProvider()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            //enumerate resource names if need
            //foreach (string resourceName in assembly.GetManifestResourceNames())
            //    System.Diagnostics.Debug.WriteLine(resourceName);

            //load modes list
            Stream syntaxModeStream = assembly.GetManifestResourceStream("PetriNetSimulator2.Syntax.SyntaxModes.xml");
            if (syntaxModeStream != null)
            {
                syntaxModes = SyntaxMode.GetSyntaxModes(syntaxModeStream);
            }
            else
            {
                syntaxModes = new List<SyntaxMode>();
            }
        }

        public XmlTextReader GetSyntaxModeFile(SyntaxMode syntaxMode)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();

            // load syntax schema
            Stream stream = assembly.GetManifestResourceStream(
              "PetriNetSimulator2.Syntax." + syntaxMode.FileName);
            return new XmlTextReader(stream);
        }

        public void UpdateSyntaxModeList()
        {
            // resources don't change during runtime
        }
    }

}
