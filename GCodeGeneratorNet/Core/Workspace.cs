using GCodeGeneratorNet.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCodeGeneratorNet.Core
{
    public class Workspace
    {
        public CsToGCodeCompiler Compiler { get; private set; }
        public TextEditManager TextEditManager { get; private set; }
        public GCodeToPointsConverter GCodeToPointsConverter { get; private set; }
        public Workspace()
        {
            this.Compiler = new CsToGCodeCompiler();
            this.TextEditManager = new TextEditManager();
            this.GCodeToPointsConverter = new GCodeToPointsConverter();
        }
    }
}
