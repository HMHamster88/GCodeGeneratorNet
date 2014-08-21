using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCodeGeneratorNet.Core.GCodes
{
    public class G02 : IGCode
    {
        public Type Type
        {
            get { return GCodes.Type.G; }
        }

        public int Code
        {
            get { return 2; }
        }

        public string Description
        {
            get { return "Clockwise arc"; }
        }

        public float X { get; private set; }
        public float Y { get; private set; }
        public float Z { get; private set; }

        public float I { get; private set; }
        public float J { get; private set; }

        public G02(float x, float y, float z, float i, float J)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.I = i;
            this.J = J;
        }

        public IEnumerable<OpenTK.Vector3> GetPoints(OpenTK.Vector3 initPos, bool absolute)
        {
            Vector3 finish;
            if (absolute)
            {
                finish = new Vector3(X, Y, Z);
            }
            else
            {
                finish = new Vector3(X + initPos.X, Y + initPos.Y, Z + initPos.Z);
            }

            Vector3 ij = new Vector3(I, J, 0);
            Vector3 center = initPos + new Vector3(I, J, 0);
            var radius = ij.Length;
            var startAngle = Vector3.CalculateAngle(initPos, new Vector3(1, 0, 0));
            var stopAngle = Vector3.CalculateAngle(finish, new Vector3(1, 0, 0));
            var step = -0.1f;
            for (float angle = startAngle; angle > stopAngle; angle+= step )
            {
                yield return center + new Vector3((float)Math.Cos(angle), (float)Math.Sin(angle), 0) * radius;
            }
            yield return finish;
        }
    }
}
