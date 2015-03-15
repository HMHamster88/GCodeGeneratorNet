using GCodeGeneratorNet.Core.Geometry;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GCodeGeneratorNet.Core.Misc;

namespace GCodeGeneratorNet.Core
{
    public class STLWriter
    {
        /// <summary>
        /// Write trangles to STL binary format
        /// </summary>
        /// <param name="stream">Stream to write</param>
        /// <param name="trangles">Trangles to write</param>
        /// STLWriter.WriteSTL(stream, new[]{
        /// new Trangle3D(new Vector3(0,0,0), new Vector3(1,0,0), new Vector3(0,1,0))
        /// });
        public static void WriteSTL(Stream stream, IEnumerable<Trangle3D> trangles)
        {
            var header = new byte[80];
            stream.Write(header, 0, header.Length);
            stream.Write((UInt32)trangles.Count());
            foreach(var trangle in trangles)
            {
                stream.Write(trangle.Normal);
                stream.Write(trangle.V1);
                stream.Write(trangle.V2);
                stream.Write(trangle.V3);
                stream.Write((UInt16)0);
            }
        }
    }
}
