using GCodeGeneratorNet.Core.GCodes;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCodeGeneratorNet.Core.Geometry
{
    public interface IContourPart
    {
        IEnumerable<Vector2> DrawPoints { get; }
        Vector2 FirstPoint { get; }
        Vector2 LastPoint { get; }
        IEnumerable<IContourPart> Inflate(float radius, Vector2 prev, Vector2 next);
        IEnumerable<IGCode> ToGCode(float f);
        IEnumerable<IGCode> ToGCode(float z, float bridgeWidth, float bridgeHeight, int bridgeCount, float f);
    }
}
