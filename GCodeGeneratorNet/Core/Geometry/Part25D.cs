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
    }
}
