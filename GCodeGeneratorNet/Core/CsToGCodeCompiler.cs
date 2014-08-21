using CSScriptLibrary;
using GCodeGeneratorNet.Core.GCodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCodeGeneratorNet.Core
{
    public class CsToGCodeCompiler
    {
        public IEnumerable<IGCode> Compile(string code)
        {
            var script = new AsmHelper(CSScript.LoadMethod(code));
            return script.Invoke("*.Generate") as IEnumerable<IGCode>;
        }
    }
}
