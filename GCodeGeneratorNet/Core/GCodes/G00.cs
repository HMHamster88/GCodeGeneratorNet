using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCodeGeneratorNet.Core.GCodes
{
    public class G00 : IGCode
    {
        public Type Type
        {
            get { return GCodes.Type.G; }
        }

        public int Code
        {
            get { return 0; }
        }

        public string Description
        {
            get { return "Run to"; }
        }

        public float? X { get; private set; }
        public float? Y { get; private set; }
        public float? Z { get; private set; }

        public G00(float? x = null, float? y = null, float? z = null)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public override string ToString()
        {
            return GCodeStringBuilder.GCodeToString(this);
        }


        public IEnumerable<Vector3> GetPoints(Vector3 initPos, bool absolute)
        {
            if(absolute)
            {
                yield return new Vector3(X.HasValue ? X.Value : initPos.X, Y.HasValue ? Y.Value : initPos.Y, Z.HasValue ? Z.Value : initPos.Z);
            }
            else
            {
                yield return new Vector3(X.HasValue ? X.Value : 0 + initPos.X, Y.HasValue ? Y.Value : 0 + initPos.Y, Z.HasValue ? Z.Value : 0 + initPos.Z);
            }
        }
    }
}
