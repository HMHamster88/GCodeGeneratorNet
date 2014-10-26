using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace GCodeGeneratorNet.Graphics
{
    public class Path3D: IDisposable
    {
        class Vbo { public int VboID, NumElements; }
        Vbo vbo = null;
        IEnumerable<VertexPositionColor> points;

        public bool Initalized { get; private set; }

        public Path3D(IEnumerable<VertexPositionColor> points)
        {
            this.points = points;
            Initalized = false;
        }

        public void Init()
        {
            if (!Initalized)
            {
                vbo = LoadVBO<VertexPositionColor>(points.ToArray());
                Initalized = true;
            }
        }

        Vbo LoadVBO<TVertex>(TVertex[] vertices) where TVertex : struct
        {
            Vbo handle = new Vbo();
            int size;

            GL.GenBuffers(1, out handle.VboID);
            GL.BindBuffer(BufferTarget.ArrayBuffer, handle.VboID);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(vertices.Length * BlittableValueType.StrideOf(vertices)), vertices,
                          BufferUsageHint.StaticDraw);
            GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, out size);
            if (vertices.Length * BlittableValueType.StrideOf(vertices) != size)
                throw new ApplicationException("Vertex data not uploaded correctly");

            handle.NumElements = vertices.Length;
            return handle;
        }

        public void Draw()
        {
            if (vbo == null)
            {
                return;
            }
            GL.EnableClientState(ArrayCap.ColorArray);
            GL.EnableClientState(ArrayCap.VertexArray);

            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo.VboID);

            VertexPositionColor[] pc = new VertexPositionColor[] { };
            GL.VertexPointer(3, VertexPointerType.Float, BlittableValueType.StrideOf(pc), new IntPtr(0));
            GL.ColorPointer(4, ColorPointerType.UnsignedByte, BlittableValueType.StrideOf(pc), new IntPtr(12));
            GL.DrawArrays(PrimitiveType.LineStrip, 0, vbo.NumElements);
        }

        public void Dispose()
        {
            if (vbo != null)
            {
                GL.DeleteBuffers(1, ref vbo.VboID);
            }
        }
    }
}
