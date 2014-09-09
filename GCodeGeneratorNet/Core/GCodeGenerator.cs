using GCodeGeneratorNet.Core.GCodes;
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
            GoToSafetyHeight();
        }

        public void Pause()
        {
            GoToSafetyHeight();
            codes.Add(new Pause());
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
            gp.AddString(text, fontFamily, style, size, new Point(), StringFormat.GenericDefault);
            var m = new Matrix();
            var bounds = gp.GetBounds();
            if(centerd)
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
