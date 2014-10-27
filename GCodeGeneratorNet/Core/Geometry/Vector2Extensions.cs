using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCodeGeneratorNet.Core.Geometry
{
    public static class Vector2Extensions
    {
        public static bool EqualEps(this Vector2 v1, Vector2 v2, float eps)
        {
            return (v1 - v2).Length < eps;
        }
    }
}
