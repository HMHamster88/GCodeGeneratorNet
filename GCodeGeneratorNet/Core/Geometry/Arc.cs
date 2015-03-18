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
    public class Arc : IContourPart
    {
        public Vector2 Center { get; private set; }
        public float Radius { get; private set; }
        public Angle StartAngle { get; private set; }
        public Angle StopAngle { get; private set; }
        public RotateDirection Direction { get; private set; }

        public Arc(Vector2 center, float radius, Angle startAngle, Angle stopAngle, RotateDirection direction)
        {
            this.Center = center;
            this.Radius = radius;
            this.StartAngle = startAngle;
            this.StopAngle = stopAngle;
            this.Direction = direction;
        }

        public IEnumerable<Vector2> DrawPoints
        {
            get 
            {
                var dir = (int)Direction;
                var distance = Direction == RotateDirection.CCW ? StopAngle - StartAngle : StartAngle - StopAngle;
                if (distance == 0)
                {
                    distance = Math.PI * 2;
                }
                var steps = 40;
                var step = distance / 40;
                for (int i = 0; i <= steps; i++)
                {
                    Angle angle = StartAngle + step * i * dir;
                    yield return new Vector2(Center.X, Center.Y) + angle.HorizontalVector * Radius;
                }
            }
        }


        public Vector2 FirstPoint
        {
            get 
            {
                return Center + StartAngle.HorizontalVector * Radius;
            }
        }

        public Vector2 LastPoint
        {
            get
            {
                return Center + StopAngle.HorizontalVector * Radius;
            }
        }

        Vector2[] CircleCrossPoint(Vector2 start, Vector2 direction, Vector2 center, float radius)
        {
            start = start - center;
            double x1;
            double x2;
            double y1;
            double y2;
            if (Math.Abs(direction.X) < 0.00000001f )
            {
                var d = radius * radius - start.X * start.X;
                if (d < 0)
                {
                    return null;
                }
                x1 = x2 = start.X;
                y1 = Math.Sqrt(d);
                y2 = -Math.Sqrt(d);
            }
            else
            {
                double k = direction.Y / direction.X;
                double z = start.Y - start.X * k;
                double a = 1 + k * k;
                double b = 2 * k * z;
                double c = z * z - radius * radius;
                double d = b * b - 4 * a * c;
                if (d < 0)
                {
                    return null;
                }
                x1 = (-b - Math.Sqrt(d)) / (2 * a);
                x2 = (-b + Math.Sqrt(d)) / (2 * a);
                y1 = z + x1 * k;
                y2 = z + x2 * k;
            }
            return new Vector2[] { new Vector2((float)x1, (float)y1), new Vector2((float)x2, (float)y2) };
        }

        Vector2 nearest(IEnumerable<Vector2> points, Vector2 to)
        {
            Vector2 result = points.First();
            var distance = (to - result).Length;
            foreach(var p in points)
            {
                var new_distance = (to - p).Length;
                if(new_distance < distance)
                {
                    distance = new_distance;
                    result = p;
                }
            }
            return result;
        }

        public IEnumerable<IContourPart> Inflate(float radius, Vector2 prev, Vector2 next)
        {
            if (StopAngle.EqualEps(StartAngle, 1e-15))
            {
                yield return new Arc(Center, Radius + radius, StartAngle, StartAngle, Direction);
            }
            else
            {
                var startVector = Center + StartAngle.HorizontalVector * Radius;
                var stopVector = Center + StopAngle.HorizontalVector * Radius;

                var startNormal = StartAngle.HorizontalVector.PerpendicularLeft;
                var stopNormal = StopAngle.HorizontalVector.PerpendicularRight;

                var radN = radius;

                if (Direction == RotateDirection.CW)
                {
                    startNormal = -startNormal;
                    stopNormal = -stopNormal;
                    radN = -radN;
                }

                Vector2 prevN;
                if (prev != startVector)
                {
                    prevN = (prev - startVector).Normalized();
                }
                else
                {
                    prevN = new Vector2();
                }

                Vector2 nextN;
                if (next != stopVector)
                {
                    nextN = (next - stopVector).Normalized();
                }
                else
                {
                    nextN = new Vector2();
                }

                IContourPart arc0 = null;
                IContourPart arc1 = null;

                var newStartVector = StartAngle.HorizontalVector;
                var cornerType = ContourPoint.IsOuterCorner(prevN, startNormal, radius < 0);
                if (cornerType == ContourPoint.CornerType.Outer)
                {
                    arc0 = new Arc(startVector, Math.Abs(radius), new Angle(prevN.PerpendicularLeft * Math.Sign(radius)),
                        new Angle(startNormal.PerpendicularRight) * Math.Sign(radius), RotateDirection.CCW);
                }
                else
                {
                    var crosp = CircleCrossPoint(prev + prevN.PerpendicularLeft * radius, prevN, Center, Radius + radN);
                    if (crosp != null)
                    {
                        newStartVector = nearest(crosp, newStartVector * Math.Abs(radius));
                    }
                }
                var newStopVector = StopAngle.HorizontalVector;

                cornerType = ContourPoint.IsOuterCorner(stopNormal, nextN, radius < 0);
                if (cornerType == ContourPoint.CornerType.Outer)
                {
                    arc1 = new Arc(stopVector, Math.Abs(radius), new Angle(stopNormal.PerpendicularLeft * Math.Sign(radius)),
                        new Angle(nextN.PerpendicularRight * Math.Sign(radius)),
                         (RotateDirection)Math.Sign(radius));
                }
                else
                {
                    var crosp = CircleCrossPoint(next + nextN.PerpendicularRight * radius, nextN, Center, Radius + radN);
                    if (crosp != null)
                    {
                        newStopVector = nearest(crosp, newStopVector * Math.Abs(radius));
                    }
                }

                if (arc0 != null)
                    yield return arc0;

                if (Radius + radN == 0)
                {
                    yield return new ContourPoint(Center);
                }
                else
                {
                    yield return new Arc(Center, Radius + radN, new Angle(newStartVector), new Angle(newStopVector), Direction);
                }

                if (arc1 != null)
                    yield return arc1;
            }
        }

        public IEnumerable<GCodes.IGCode> ToGCode()
        {
            Vector2 start = Center + StartAngle.HorizontalVector * Radius;
            Vector2 startOffset = Center - start;
            Vector2 end = Center + StopAngle.HorizontalVector * Radius;
            yield return new GMOVE(false, start.X, start.Y, null);
            yield return new GARC(end.X, end.Y, null, startOffset.X, startOffset.Y, Direction);
        }

        public IEnumerable<GCodes.IGCode> ArcPartToGCode(Angle startAngle, Angle stopAngle, float z)
        {
            Vector2 start = Center + startAngle.HorizontalVector * Radius;
            Vector2 startOffset = Center - start;
            Vector2 end = Center + stopAngle.HorizontalVector * Radius;
            yield return new GMOVE(false, start.X, start.Y, z);
            yield return new GARC(end.X, end.Y, null, startOffset.X, startOffset.Y, Direction);
        }

        public IEnumerable<IGCode> ToGCode(float z, float bridgeWidth, float bridgeHeight, int bridgeCount)
        {
            var list = new List<IGCode>();
            var dir = (int)Direction;
            var distance = Direction == RotateDirection.CCW ? StopAngle - StartAngle : StartAngle - StopAngle;
            if (distance == 0)
            {
                distance = Math.PI * 2;
            }
            var length = distance * Radius;

            if(length < (bridgeWidth ) * bridgeCount)
            {
                return ToGCode();
            }
            
            var delta = distance / (bridgeCount + 1);
            var al = bridgeWidth / (Radius * 2);
            for (int i = 0; i <= bridgeCount; i++)
            {
                var beginAngle = StartAngle + i * delta * dir;
                if (i != 0)
                {
                    beginAngle += al * dir;
                }
                var endAngle = StartAngle + (i + 1) * delta * dir;
                if (i != bridgeCount)
                {
                    endAngle -= al * dir;
                }
                list.AddRange(ArcPartToGCode(beginAngle, endAngle, z));
                if (i != bridgeCount)
                {
                    list.AddRange(ArcPartToGCode(endAngle, endAngle + al * 2 * dir, z + bridgeHeight));
                }
            }

            var stop = new Vector2(Center.X, Center.Y) + StopAngle.HorizontalVector * Radius;
            list.Add(new GMOVE(false, stop.X, stop.Y, z));

            return list;
        }
    }
}
