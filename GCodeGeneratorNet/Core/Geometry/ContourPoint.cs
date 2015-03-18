using GCodeGeneratorNet.Core.GCodes;
using GCodeGeneratorNet.Core.Misc;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCodeGeneratorNet.Core.Geometry
{
    public class ContourPoint : IContourPart
    {
        public Vector2 Position { get; private set; }

        public ContourPoint(Vector2 position)
        {
            this.Position = position;
        }

        public IEnumerable<Vector2> DrawPoints
        {
            get { yield return Position; }
        }


        public Vector2 FirstPoint
        {
            get { return Position; }
        }

        public Vector2 LastPoint
        {
            get { return Position; }
        }

        public enum CornerType
        {
            Outer,
            Inner,
            CoDirected,
            OpDirected
        }

        static float eps = 1e-15f;

        public static CornerType IsOuterCorner(Vector2 prev, Vector2 next, bool reverse)
        {
            if (prev.EqualEps(next, eps))
                return CornerType.CoDirected;
            if (prev.EqualEps(-next, eps))
                return CornerType.OpDirected;

            if (reverse)
            {
                var tmp = prev;
                prev = next;
                next = tmp;
            }

            if (Vector2.Dot(prev, next.PerpendicularLeft) > 6.123032E-17)
            {
                return CornerType.Outer;
            }
            return CornerType.Inner;
        }

        public static Vector2 BisR(float radius, Vector2 prev, Vector2 next)
        {
            Vector2 bis = (prev + next).Normalized();
            return bis * Math.Abs(radius) / Math.Abs(Vector2.Dot(prev.PerpendicularLeft, bis));
        }

        public IEnumerable<IContourPart> Inflate(float radius, Vector2 prev, Vector2 next)
        {
            prev = (prev - Position).Normalized();
            next = (next - Position).Normalized();
            var cornerType = IsOuterCorner(prev, next, radius < 0);

            if (cornerType == CornerType.Outer)// outer corner
            {
                yield return new Arc(Position, radius, new Angle(prev.PerpendicularLeft), new Angle(next.PerpendicularRight), 
                    radius > 0 ? RotateDirection.CCW : RotateDirection.CW);
            }
            else if (cornerType == CornerType.OpDirected)
            {
                yield return new ContourPoint(Position + next.PerpendicularRight * radius);
            }
            else
            {
                yield return new ContourPoint(Position + BisR(radius, prev, next));
            }
        }


        public IEnumerable<GCodes.IGCode> ToGCode(float f)
        {
            yield return new GMOVE(false, Position.X, Position.Y, null,f);
        }


        public IEnumerable<IGCode> ToGCode(float z, float bridgeWidth, float bridgeHeight, int bridgeCount, float f)
        {

            yield return new GMOVE(false, Position.X, Position.Y, null, f);
        }
    }
}
