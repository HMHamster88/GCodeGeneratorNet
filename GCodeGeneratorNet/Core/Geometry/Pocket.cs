using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCodeGeneratorNet.Core.Geometry
{
    public class Pocket
    {
        public Contour Contour { get; private set; }
        public float Depth { get; private set; }

        public Pocket(float depth, Contour contour)
        {
            this.Depth = depth;
            this.Contour = contour;
        }

        public Pocket(float depth, params IContourPart[] parts)
        {
            this.Depth = depth;
            this.Contour = new Contour(parts);
        }
    }
}
