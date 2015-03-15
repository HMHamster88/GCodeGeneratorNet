using OpenTK;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCodeGeneratorNet.Core.Misc
{
    public static class StreamExtension
    {
        public static void Write(this Stream stream, UInt16 i)
        {
            var buffer = BitConverter.GetBytes(i);
            stream.Write(buffer, 0, buffer.Length);
        }

        public static void Write(this Stream stream, UInt32 i)
        {
            var buffer = BitConverter.GetBytes(i);
            stream.Write(buffer, 0, buffer.Length);
        }

        public static void Write(this Stream stream, float f)
        {
            var buffer = BitConverter.GetBytes(f);
            stream.Write(buffer, 0, buffer.Length);
        }

        public static void Write(this Stream stream, Vector3 v)
        {
            stream.Write(v.X);
            stream.Write(v.Y);
            stream.Write(v.Z);
        }
    }
}
