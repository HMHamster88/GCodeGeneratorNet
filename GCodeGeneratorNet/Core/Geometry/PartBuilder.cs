using GCodeGeneratorNet.Core.Misc;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCodeGeneratorNet.Core.Geometry
{
    public class PartBuilder
    {
        List<IContourPart> contourParts = new List<IContourPart>();
        Contour contour = new Contour();
        List<Contour> holes = new List<Contour>();
        List<Pocket> pockets = new List<Pocket>();

        public void SetContour(Contour contour)
        {
            this.contour = contour;
        }

        public void AddHole(Contour hole)
        {
            holes.Add(hole);
        }

        public void AddPocket(Pocket pocket)
        {
            pockets.Add(pocket);
        }

        public void Add(IContourPart part)
        {
            contourParts.Add(part);
        }

        public void AddPoint(Vector2 position)
        {
            Add(new ContourPoint(position));
        }

        public void AddArc(Vector2 center, float radius, Angle startAngle, Angle stopAngle, RotateDirection direction)
        {
            Add(new Arc(center, radius, startAngle, stopAngle, direction));
        }

        public void AddCircle(Vector2 center, float radius)
        {
            AddArc(center, radius, 0, 0, RotateDirection.CW);
        }

        public void AddRect(Vector2 position, Vector2 size)
        {
            AddPoint(position);
            AddPoint(position + new Vector2(size.X, 0));
            AddPoint(position + size);
            AddPoint(position + new Vector2(0, size.Y));
        }

        public Contour CreateContour()
        {
            contour = new Contour(contourParts);
            contourParts = new List<IContourPart>();
            return contour;
        }

        public void CreateHole()
        {
            holes.Add(new Contour(contourParts));
            contourParts = new List<IContourPart>();
        }

        public void CreatePocket(float depth)
        {
            pockets.Add(new Pocket(depth, new Contour(contourParts)));
            contourParts = new List<IContourPart>();
        }

        public Part25D CreatePart(float thickness)
        {
            var part = new Part25D(thickness, contour, pockets, holes);
            contour = new Contour();
            pockets = new List<Pocket>();
            holes = new List<Contour>();
            return part;
        }
    }
}
