using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCodeGeneratorNet.Core.GCodes
{
    public class GMOVE : IGCode
    {
        public Type Type
        {
            get { return GCodes.Type.G; }
        }

        public int Code
        {
            get { return Rapid ? 0 : 1; }
        }

        public string Description
        {
            get 
            { 
                if(Rapid)
                    return "Rapid positioning";
                return "Linear interpolation";
            }
        }

        public bool Rapid { get; private set; }
        
        public float? X { get; private set; }
        public float? Y { get; private set; }
        public float? Z { get; private set; }

        public float? F { get; private set; }

        public GMOVE(bool rapid, float? x = null, float? y = null, float? z = null, float? f = null)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.F = f;
            this.Rapid = rapid;
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
