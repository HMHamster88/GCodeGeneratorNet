using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCodeGeneratorNet.Core.GCodes
{
    public class Pause: IGCode
    {
        public Type Type
        {
            get { return GCodes.Type.M; }
        }

        public int Code
        {
            get { return 0; }
        }

        public string Description
        {
            get { return "Pause"; }
        }

        public IEnumerable<OpenTK.Vector3> GetPoints(OpenTK.Vector3 initPos, bool absolute)
        {
            return new OpenTK.Vector3[] { };
        }

        public override string ToString()
        {
            return Type.ToString() + Code.ToString("D2");
        }
    }
}
