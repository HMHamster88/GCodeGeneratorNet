using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCodeGeneratorNet.Core.GCodes
{
    public enum Type
    {
        G,
        M
    }

    public interface IGCode
    {
        Type Type { get; }
        int Code { get; }
        string Description { get; }
        IEnumerable<Vector3> GetPoints(Vector3 initPos, bool absolute);
    }
}
