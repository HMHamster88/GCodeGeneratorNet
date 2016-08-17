using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using GCodeGeneratorNet.Core.Misc;

namespace GCodeGeneratorNet.Core.GCodes
{
    public class GSpindle : IGCode
    {
        public RotateDirection RotateDirection { get; private set; }

        public float S { get; private set; }

        public GSpindle(float rpm, RotateDirection direction = RotateDirection.CW)
        {
            S = rpm;
            RotateDirection = direction;
        }

        public int Code
        {
            get
            {
                if(RotateDirection == RotateDirection.CW)
                {
                    return 3;
                }
                return 4;
            }
        }

        public string Description
        {
            get
            {
                return $"Set spindle direction to {RotateDirection} and RPM to {S}";
            }
        }

        public Type Type
        {
            get
            {
                return Type.M;
            }
        }

        public IEnumerable<Vector3> GetPoints(Vector3 initPos, bool absolute)
        {
            return new Vector3[]{ };
        }

        public override string ToString()
        {
            return GCodeStringBuilder.GCodeToString(this);
        }
    }
}
