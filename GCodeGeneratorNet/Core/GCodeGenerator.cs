using GCodeGeneratorNet.Core.GCodes;
using GCodeGeneratorNet.Core.Geometry;
using GCodeGeneratorNet.Core.Misc;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCodeGeneratorNet.Core
{
    public enum ToolCompensation
    {
        None = 0,
        In = -1,
        Out = 1
    }

    public class GCodeGenerator
    {
        List<IGCode> codes = new List<IGCode>();
        public IEnumerable<IGCode> Codes
        {
            get
            {
                return codes;
            }
        }

        public Vector3 CurrentPosition { get; set; }

        public float Tolerance { get; set; }
        public float ToolRadius { get; set; }

        public float ToolRadiusAndTolerance { get { return Tolerance + ToolRadius; } }

        public float MaterialHeight { get; set; }
        public float SafetyHeight { get; set; }

        public float RapidMoveRate { get; set; }
        public float HorizontalFeedRate { get; set; }

        public float VerticalFeedRate { get; set; }

        public float VerticalStep { get; set; }

        public float BridgeWidth { get; set; }
        public float BridgeHeidht { get; set; }
        public int BridgeCount { get; set; }

        public GCodeGenerator()
        {
            MaterialHeight = 8;
            SafetyHeight = 4;
            CurrentPosition = new Vector3();
            Tolerance = 0;
            ToolRadius = 1;
            RapidMoveRate = 1000;
            HorizontalFeedRate = 200;
            VerticalFeedRate = 100;
            VerticalStep = 2;
            codes.Add(new G90());
            GoToSafetyHeight();
            BridgeWidth = 2;
            BridgeHeidht = 0.5f;
            BridgeCount = 1;
        }

        public IEnumerable<float> VerticalRange
        {
            get
            {
                for (float z = MaterialHeight - VerticalStep; z >= 0; z -= VerticalStep)
                {
                    yield return z;
                }
            }
        }

        public void DottedLine(Vector2 start, Vector2 end, float z, float dotLength, float dotHeight, int dotCount)
        {
            HorizontalFeedTo(start);
            VerticalFeedTo(z);
            float freeStep = (end - start).Length / (dotCount + 1);
            var direction = end - start;
            direction.Normalize();
            for (int i = 1; i < dotCount + 1; i++)
            {
                HorizontalFeedTo(start + direction * (freeStep * i - dotLength / 2));
                VerticalFeedTo(z + dotHeight);
                HorizontalFeedTo(start + direction * (freeStep * i + dotLength / 2));
                VerticalFeedTo(z);
            }
            HorizontalFeedTo(end);
        }

        public void Pause()
        {
            GoToSafetyHeight();
            codes.Add(new Pause());
        }

        public void Comments(string comments)
        {
            codes.Add(new GComment(comments));
        }

        public void AddCode(IGCode code)
        {
            codes.Add(code);
        }

        public void GoToSafetyHeight()
        {
            codes.Add(new GMOVE(true, null, null, MaterialHeight + SafetyHeight));
        }

        public void RapidMoveTo(Vector2 position)
        {
            codes.Add(new GMOVE(true, position.X, position.Y, null));
        }

        public void VerticalFeedTo(float z)
        {
            codes.Add(new GMOVE(false, null, null, z, VerticalFeedRate));
        }

        public void HorizontalFeedTo(Vector2 position)
        {
            codes.Add(new GMOVE(false, position.X, position.Y, null, HorizontalFeedRate));
        }

        public void ContourAt(Contour contour, float z, float bridgeWidth, float bridgeHeight, int bridgeCount)
        {
            if(bridgeCount == 0)
            {
                ContourAt(contour, z);
            }
            bridgeWidth += ToolRadius * 2;
            var result = new List<IGCode>();
            var start = contour.Parts.First().FirstPoint;
            var prev = start;
            var end = start;
            RapidMoveTo(start);
            VerticalFeedTo(z);
            foreach(var part in contour.Parts)
            {
                end = part.FirstPoint;
                if ((prev - end).Length > bridgeWidth * bridgeCount)
                {
                    DottedLine(prev, end, z, bridgeWidth, bridgeHeight, bridgeCount);
                }
                prev = part.LastPoint;
                codes.AddRange(part.ToGCode(z, bridgeWidth, bridgeHeight, bridgeCount, HorizontalFeedRate));
            }
            end = start;
            if ((prev - end).Length > bridgeWidth * bridgeCount)
            {
                DottedLine(prev, end, z, bridgeWidth, bridgeHeight, bridgeCount);
            }
        }

        public void ContourAt(Contour contour, float z)
        {
            RapidMoveTo(contour.Parts.First().FirstPoint);
            VerticalFeedTo(z);
            codes.AddRange(contour.ToGCode(HorizontalFeedRate));
        }

        public void Part25D(Part25D part)
        {
            if (part.Holes != null)
            {
                foreach (var hole in part.Holes)
                {
                    GoToSafetyHeight();
                    var holePath = hole.Inflate(-ToolRadius);
                    foreach (var z in range(part.Thickness - VerticalStep, 0, VerticalStep))
                    {
                        ContourAt(holePath, z);
                    }
                }
            } 
            
            if (part.Pockets != null)
            {
                foreach (var pocket in part.Pockets)
                {
                    GoToSafetyHeight();
                    var pocketPath = pocket.Contour.Inflate(-ToolRadius);
                    foreach (var z in range(part.Thickness - VerticalStep, part.Thickness - pocket.Depth, VerticalStep))
                    {
                        ContourAt(pocketPath, z);
                    }
                }
            }

            GoToSafetyHeight();
            var contourPath = part.Contour.Inflate(ToolRadius);
            if(contourPath.Parts.Count() == 0)
            {
                return;
            }
            var rng = range(part.Thickness - VerticalStep, 0, VerticalStep);
            foreach (var z in rng)
            {
                if (z == rng.Last())
                {
                    ContourAt(contourPath, z, BridgeWidth, BridgeHeidht, BridgeCount);
                }
                else
                {
                    ContourAt(contourPath, z);
                }
            }
        }

        public Vector2 HorizontalArcStart(Vector2 center, float radius, Angle startAngle, Angle stopAngle, RotateDirection dir, ToolCompensation compensation)
        {
            radius = radius + ToolRadiusAndTolerance * (int)compensation;
            if (startAngle == stopAngle)
                stopAngle = stopAngle + (float)Math.PI * 2 * (int)dir;
            return center + startAngle.HorizontalVector * radius;
        }

        public Vector2 HorizontalArcEnd(Vector2 center, float radius, Angle startAngle, Angle stopAngle, RotateDirection dir, ToolCompensation compensation)
        {
            radius = radius + ToolRadiusAndTolerance * (int)compensation;
            if (startAngle == stopAngle)
                stopAngle = stopAngle + (float)Math.PI * 2 * (int)dir;
            return center + stopAngle.HorizontalVector * radius;
        }

        public void HorizontalArc(Vector2 center, float radius, Angle startAngle, Angle stopAngle, RotateDirection dir, ToolCompensation compensation)
        {
            radius = radius + ToolRadiusAndTolerance * (int)compensation;
            if (startAngle == stopAngle)
                stopAngle = stopAngle + (float)Math.PI * 2 * (int)dir;
            Vector2 start = center + startAngle.HorizontalVector * radius;
            Vector2 startOffset = center - start;
            Vector2 end = center + stopAngle.HorizontalVector * radius;
            HorizontalFeedTo(start);
            codes.Add(new GARC(end.X, end.Y, null, startOffset.X, startOffset.Y, dir, HorizontalFeedRate));
        }

        public void HorizontalCircle(Vector2 center, float radius, RotateDirection dir, ToolCompensation compensation)
        {
            HorizontalArc(center, radius, 0, 0, dir, compensation);
        }

        public void RoundHole(Vector2 center, float radius)
        {
            RoundHole(center, radius, MaterialHeight, 0);
        }

        public void RoundHole(Vector2 center, float radius, float start, float stop)
        {
            GoToSafetyHeight();
            RapidMoveTo(center + new Vector2(1, 0) * (radius - ToolRadiusAndTolerance));
            foreach (var z in range(start - VerticalStep, stop, VerticalStep))
            {
                VerticalFeedTo(z);
                HorizontalCircle(center, radius, RotateDirection.CW, ToolCompensation.In);
            }
            GoToSafetyHeight();
        }

        public void RoundHoles(int holeCount, Vector2 center, float distance, float radius, float start, float stop, float startAngle = 0)
        {
            for (int hole = 0; hole < holeCount; hole++)
            {
                var angle = (Angle)((float)((hole / (float)holeCount) * Math.PI * 2) + startAngle);
                RoundHole(angle.HorizontalVector * distance, radius, start, stop);
            }
        }

        public void RoundHoles(int holeCount, Vector2 center, float distance, float radius)
        {
            RoundHoles(holeCount, center, distance, radius, MaterialHeight, 0);
        }

        public void HorizontalRing(Vector2 center, float radius1, float radius2, RotateDirection dir, float z)
        {
            bool positiveStep = radius1 < radius2;
            float start = radius1 + ToolRadiusAndTolerance * (positiveStep ? 1 : -1);
            float stop = radius2 - ToolRadiusAndTolerance * (positiveStep ? 1 : -1);
            RapidMoveTo(center + new Vector2(1, 0) * (start - ToolRadiusAndTolerance));
            VerticalFeedTo(z);
            foreach (var r in range(start, stop, ToolRadiusAndTolerance))
            {
                HorizontalCircle(center, r, dir, ToolCompensation.None);
            }
        }

        public void HorizontalRing(Vector2 center, float radius1, float radius2, RotateDirection dir, float z_start, float z_stop)
        {
            GoToSafetyHeight();
            foreach (var z in range(z_start, z_stop, VerticalStep))
            {
                HorizontalRing(center, radius1, radius2, dir, z);
            }
            GoToSafetyHeight();
        }

        public void Path(GraphicsPath gp, Matrix matrix, float flatness, float z)
        {
            gp.Flatten(matrix, flatness);
            Vector2 lastPos = new Vector2();
            for (int i = 0; i < gp.PathPoints.Length; i++)
            {
                Vector2 v = new Vector2(gp.PathPoints[i].X, -gp.PathPoints[i].Y);
                if (gp.PathTypes[i] == 0)
                {
                    if (i != 0)
                    {
                        HorizontalFeedTo(lastPos);
                        GoToSafetyHeight();
                        RapidMoveTo(v);
                        VerticalFeedTo(z);
                    }
                    else
                    {
                        GoToSafetyHeight();
                        RapidMoveTo(v);
                        VerticalFeedTo(z);
                    }
                    lastPos = v;
                }
                else
                {
                    HorizontalFeedTo(v);
                }
            }
            HorizontalFeedTo(lastPos);
        }

        public void Text(string text, FontFamily fontFamily, int style, float size, Matrix matrix, float z, float flatness, bool centerd = true)
        {
            GraphicsPath gp = new GraphicsPath();
            gp.AddString(text, fontFamily, style, size, new System.Drawing.Point(), StringFormat.GenericDefault);
            var m = new Matrix();
            var bounds = gp.GetBounds();
            if (centerd)
                m.Translate(-(bounds.X + bounds.Width / 2), -(bounds.Y + gp.GetBounds().Height / 2));
            gp.Transform(m);
            Path(gp, matrix, flatness, z);
        }

        public static IEnumerable<float> range(float start, float stop, float step)
        {
            float last = start;
            if ((start > stop && step > 0) || (stop < start && step < 0))
                step = -step;
            while ((step > 0 && start <= stop) || (step < 0 && start >= stop))
            {
                yield return last = start;
                start += step;
            }
            if (last != stop)
                yield return stop;
        }
    }
}
