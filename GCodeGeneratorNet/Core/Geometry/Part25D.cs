using GCodeGeneratorNet.Graphics;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCodeGeneratorNet.Core.Geometry
{
    public class Part25D
    {
        public Contour Contour { get; private set; }
        public IEnumerable<Pocket> Pockets { get; private set; }
        public IEnumerable<Contour> Holes { get; private set; }
        public float Thickness { get; private set; }

        public Part25D(float thickness, Contour contour, IEnumerable<Pocket> pockets, IEnumerable<Contour> holes)
        {
            this.Thickness = thickness;
            this.Contour = contour;
            this.Pockets = pockets;
            this.Holes = holes;
        }

        IEnumerable<VertexPositionColor> ContourAt(Contour contour, float z)
        {
            return contour.DrawPoints.Select(v => new VertexPositionColor(v.X, v.Y, z, System.Drawing.Color.Red));
        }

        public IEnumerable<Path3D> ToPaths()
        {
            if (Contour.Parts.Count() > 0)
            {
                yield return new Path3D(ContourAt(Contour, 0));
                yield return new Path3D(ContourAt(Contour, Thickness));
            }
            if (Holes != null)
            {
                foreach (var hole in Holes)
                {
                    yield return new Path3D(ContourAt(hole, 0));
                    yield return new Path3D(ContourAt(hole, Thickness));
                }
            }
            if (Pockets != null)
            {
                foreach (var pocket in Pockets)
                {
                    yield return new Path3D(ContourAt(pocket.Contour, Thickness - pocket.Depth));
                    yield return new Path3D(ContourAt(pocket.Contour, Thickness));
                }
            }
        }

        public IEnumerable<Trangle3D> ToTrangles()
        {
            var result = new List<Trangle3D>();
            result.AddRange(Trangle3D.VerticalContour(Contour.DrawPoints, 0, Thickness));
            if (Holes != null)
            {
                foreach (var hole in Holes)
                {
                    result.AddRange(Trangle3D.VerticalContour(hole.DrawPoints, 0, Thickness));
                }
            }
            if (Pockets != null)
            {
                foreach (var pocket in Pockets)
                {
                    result.AddRange(Trangle3D.VerticalContour(pocket.Contour.DrawPoints, Thickness - pocket.Depth, Thickness));
                }
            }
            return result;
        }
    }
}
