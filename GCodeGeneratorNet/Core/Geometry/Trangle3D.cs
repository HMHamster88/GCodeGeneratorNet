using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCodeGeneratorNet.Core.Geometry
{
    public class Trangle3D
    {
        public Vector3 V1 { get; private set; }
        public Vector3 V2 { get; private set; }
        public Vector3 V3 { get; private set; }

        public Vector3 Normal { get; private set; }

        public Trangle3D(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 normal = new Vector3())
        {
            V1 = v1;
            V2 = v2;
            V3 = v3;
            Normal = normal;
        }

        public static IEnumerable<Trangle3D> VerticalPlane(Vector2 v1, Vector2 v2, float h1, float h2)
        {
            var v31 = new Vector3(v1.X, v1.Y, h1);
            var v32 = new Vector3(v2.X, v2.Y, h1);
            var v33 = new Vector3(v2.X, v2.Y, h2);
            var v34 = new Vector3(v1.X, v1.Y, h2);
            return new[] 
            { 
                new Trangle3D(v31, v32, v33),
                new Trangle3D(v33, v34, v31),
            };
        }

        public static IEnumerable<Trangle3D> VerticalContour(IEnumerable<Vector2> points, float h1, float h2)
        {
            var result = new List<Trangle3D>();
            Vector2 prev = points.First();
            foreach (var curr in points)
            {
                if (curr != prev)
                {
                    result.AddRange(Trangle3D.VerticalPlane(prev, curr, h1, h2));
                }
                prev = curr;
            }
            return result;
        }
    }
}
