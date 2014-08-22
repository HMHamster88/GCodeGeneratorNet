using GCodeGeneratorNet.Core.Misc;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCodeGeneratorNet.Core.GCodes
{
    public class GARC : IGCode
    {
        public Type Type
        {
            get { return GCodes.Type.G; }
        }

        public int Code
        {
            get { return RotateDirection == Misc.RotateDirection.CW? 2 : 3; }
        }

        public string Description
        {
            get { return "Circular interpolation " + RotateDirection.ToString(); }
        }

        public RotateDirection RotateDirection { get; private set; }

        public float X { get; private set; }
        public float Y { get; private set; }
        public float Z { get; private set; }

        public float I { get; private set; }
        public float J { get; private set; }

        public float? F { get; private set; }

        public GARC(float x, float y, float z, float i, float J, RotateDirection dir, float? f = null)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.I = i;
            this.J = J;
            this.F = f;
            this.RotateDirection = dir;
        }

        Angle XYAngle(Vector3 v1)
        {
            var dot = Vector3.Dot(v1, new Vector3(0, 1, 0));
            var angle = Vector3.CalculateAngle(v1, new Vector3(1, 0, 0));
            if (dot > 0)
                return angle;
            else
                return (float)Math.PI * 2 - angle;
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
            var startAngle = XYAngle(initPos);
            var stopAngle = XYAngle(finish);
            var dir = (int)RotateDirection;
            var distance = Math.Abs(startAngle - stopAngle);
            if(distance == 0)
            {
                distance = Math.PI * 2;
            }
            var step = distance / 40;
            for (double a = 0; a <= distance; a += step )
            {
                Angle angle = startAngle + a * dir;
                System.Diagnostics.Debug.WriteLine(center + new Vector3((float)Math.Cos(angle), (float)Math.Sin(angle), 0) * radius);
                yield return center + new Vector3((float)Math.Cos(angle), (float)Math.Sin(angle), 0) * radius;
            }
            yield return finish;
        }
    }
}
