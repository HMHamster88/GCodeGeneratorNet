using GCodeGeneratorNet.Core.GCodes;
using GCodeGeneratorNet.Core.Geometry;
using GCodeGeneratorNet.Graphics;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCodeGeneratorNet.Core
{
    public class GScriptResult
    {
        public IEnumerable<Part25D> Parts { get; private set; }
        public IEnumerable<IGCode> Codes { get; private set; }

        public GScriptResult(GCodeGenerator gcg, IEnumerable<Part25D> parts)
        {
            this.Parts = parts;
            foreach (Part25D p in parts)
            {
                gcg.Part25D(p);
            }
            this.Codes = gcg.Codes;
        }

        public GScriptResult(IEnumerable<Part25D> parts, IEnumerable<IGCode> codes)
        {
            this.Parts = parts;
            this.Codes = codes;
        }

        IEnumerable<VertexPositionColor> ConvertGCodes(IEnumerable<IGCode> codes)
        {
            bool absolute = true;
            var color = System.Drawing.Color.White;
            Vector3 position = new Vector3();
            var list = new List<Vector3>();

            list.Add(position);

            foreach (var code in codes)
            {
                var points = code.GetPoints(position, absolute);
                foreach (var p in points)
                {
                    position = p;
                    list.Add(position);
                }
            }

            for (int i = 0; i < list.Count; i++)
            {
                var c = i * 255 / list.Count;
                yield return new VertexPositionColor(list[i], System.Drawing.Color.FromArgb(c, c, c));
            }
        }

        public IEnumerable<Path3D> ToPaths()
        {
            if (Codes != null)
            {
                yield return new Path3D(ConvertGCodes(Codes));
            }
            if (Parts != null)
            {
                foreach (var path in Parts.SelectMany(p => p.ToPaths()))
                {
                    yield return path;
                }
            }
        }
    }
}
