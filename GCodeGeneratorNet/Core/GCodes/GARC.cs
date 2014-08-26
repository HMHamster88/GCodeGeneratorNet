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
        public float? Z { get; private set; }

        public float I { get; private set; }
        public float J { get; private set; }

        public float? F { get; private set; }

        public GARC(float x, float y, float? z, float i, float J, RotateDirection dir, float? f = null)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.I = i;
            this.J = J;
            this.F = f;
            this.RotateDirection = dir;
        }

        Angle XYAngle(Vector2 v1)
        {
            var dot = Vector2.Dot(v1, new Vector2(0, 1));
            var angle = Vector3.CalculateAngle(new Vector3(v1.X, v1.Y, 0), new Vector3(1, 0, 0));
            if (dot > 0)
                return angle;
            else
                return (float)Math.PI * 2 - angle;
        }

        public IEnumerable<OpenTK.Vector3> GetPoints(OpenTK.Vector3 initPos, bool absolute)
        {
            Vector2 finish;
            if (absolute)
            {
                finish = new Vector2(X, Y);
            }
            else
            {
                finish = new Vector2(X + initPos.X, Y + initPos.Y);
            }

            Vector2 ij = new Vector2(I, J);
            Vector2 center = initPos.Xy + new Vector2(I, J);
            var radius = ij.Length;
            var startAngle = XYAngle(initPos.Xy);
            var stopAngle = XYAngle(finish);
            var dir = (int)RotateDirection;
            var distance = RotateDirection == RotateDirection.CCW ? stopAngle - startAngle : startAngle - stopAngle;
            if(distance == 0)
            {
                distance = Math.PI * 2;
            }
            var steps = 40;
            var step = distance / 40;
            for (int i = 0; i < steps; i++)
            {
                Angle angle = startAngle + step * i * dir;
                yield return new Vector3(center.X, center.Y, initPos.Z) + new Vector3((float)Math.Cos(angle), (float)Math.Sin(angle), 0) * radius;
            }
            yield return new Vector3(finish.X, finish.Y, initPos.Z);
        }
    }
}
