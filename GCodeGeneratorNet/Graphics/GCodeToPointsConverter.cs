using GCodeGeneratorNet.Core.GCodes;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCodeGeneratorNet.Graphics
{
    public class GCodeToPointsConverter
    {
        public IEnumerable<VertexPositionColor> Convert(IEnumerable<IGCode> codes)
        {
            bool absolute = true;
            var color = System.Drawing.Color.White;
            Vector3 position = new Vector3();
            yield return new VertexPositionColor(position, color);
            foreach(var code in codes)
            {
                var points = code.GetPoints(position, absolute);
                foreach (var p in points)
                {
                    position = p;
                    color = System.Drawing.Color.FromArgb(color.R - 10, color.G - 10, color.B - 10);
                    yield return new VertexPositionColor(position, color);
                }
            }
        }
    }
}
