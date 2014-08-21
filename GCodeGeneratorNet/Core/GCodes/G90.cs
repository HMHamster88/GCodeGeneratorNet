using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCodeGeneratorNet.Core.GCodes
{
    public class G90 : IGCode
    {
        public Type Type
        {
            get { return GCodes.Type.G; }
        }

        public int Code
        {
            get { return 90; }
        }

        public string Description
        {
            get { return "Set absolute coordinates"; }
        }

        public override string ToString()
        {
            return GCodeStringBuilder.GCodeToString(this);
        }


        public IEnumerable<OpenTK.Vector3> GetPoints(OpenTK.Vector3 initPos, bool absolute)
        {
            yield return initPos;
        }
    }
}
