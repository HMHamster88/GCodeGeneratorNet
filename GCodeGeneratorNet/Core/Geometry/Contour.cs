using GCodeGeneratorNet.Core.GCodes;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCodeGeneratorNet.Core.Geometry
{
    public enum ContourType
    {
        In,
        Out
    }
    public class Contour
    {
        public IEnumerable<IContourPart> Parts { get; private set; }

        public Contour(IEnumerable<IContourPart> parts)
        {
            this.Parts = parts;
        }

        public Contour Inflate(float radius)
        {
            var result = new List<IContourPart>();
            for (int i = 0; i < Parts.Count(); i++)
            {
                Vector2 prev;
                if (i == 0)
                {
                    prev = Parts.Last().LastPoint;
                }
                else
                {
                    prev = Parts.ElementAt(i - 1).LastPoint;
                }
                Vector2 next;
                if (i >= Parts.Count() - 1)
                {
                    next = Parts.First().FirstPoint;
                }
                else
                {
                    next = Parts.ElementAt(i + 1).FirstPoint;
                }

                result.AddRange(Parts.ElementAt(i).Inflate(radius, prev, next));
            }
            return new Contour(result);
        }

        public IEnumerable<Vector2> DrawPoints
        {
            get 
            {
                return Parts.SelectMany(p => p.DrawPoints).Concat(new Vector2[] { Parts.First().DrawPoints.First() });
            }
        }

        public IEnumerable<IGCode> ToGCode()
        {
            var start = Parts.First().FirstPoint;
            return Parts.SelectMany(p => p.ToGCode()).Concat(new IGCode[]
            {
                new GMOVE(false, start.X, start.Y, null)
            });
        }
    }
}
