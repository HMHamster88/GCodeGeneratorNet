using csscript;
using CSScriptLibrary;
using GCodeGeneratorNet.Core.GCodes;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCodeGeneratorNet.Core
{
    public class CsToGCodeCompiler
    {
        public IEnumerable<IGCode> Compile(string code)
        {
            Errors.Clear();
            try
            {
                var script = new AsmHelper(CSScript.LoadMethod(code));
                return script.Invoke("*.Generate") as IEnumerable<IGCode>;
            }
            catch(CompilerException  ex)
            {
                CompilerErrorCollection errs = (CompilerErrorCollection)ex.Data["Errors"];
                foreach (CompilerError err in errs)
                {
                    errors.Add(err);
                }
            }
            return new IGCode[]{};
        }

        ObservableCollection<CompilerError> errors = new ObservableCollection<CompilerError>();
        public ObservableCollection<CompilerError> Errors
        {
            get
            {
                return errors;
            }
        }
    }
}
