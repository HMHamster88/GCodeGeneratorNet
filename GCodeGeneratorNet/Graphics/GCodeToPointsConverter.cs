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
            var list = new List<Vector3>();

            list.Add(position);

            foreach(var code in codes)
            {
                var points = code.GetPoints(position, absolute);
                foreach (var p in points)
                {
                    position = p;
                    list.Add(position);
                }
            }

            for (int i = 0; i < list.Count; i++ )
            {
                var c = i * 255 / list.Count;
                yield return new VertexPositionColor(list[i], System.Drawing.Color.FromArgb(c, c, c));
            }
        }
    }
}
