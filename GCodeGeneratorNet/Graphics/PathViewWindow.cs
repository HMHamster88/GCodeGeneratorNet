using System;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System.Drawing;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace GCodeGeneratorNet.Graphics
{
    class ViewWindow : GameWindow
    {
        Vector3 viewRotation = new Vector3(-135, -45, 0);
        Vector3 viewPan;
        float viewScale = 1;

        class Vbo { public int VboID, NumElements; }
        Vbo vbo = null;

        IEnumerable<VertexPositionColor> newPonts;

        public ViewWindow()
            : base(800, 600)
        {
            Mouse.Move += Mouse_Move;
            Mouse.WheelChanged += Mouse_WheelChanged;
        }

        public void LoadPoints(IEnumerable<VertexPositionColor> points)
        {
            newPonts = points;
        }

        void Mouse_WheelChanged(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                viewScale /= 2;
            }
            else
            {
                viewScale *= 2;
            }
        }

        void Mouse_Move(object sender, MouseMoveEventArgs e)
        {
            var ms = OpenTK.Input.Mouse.GetState();
            if (ms.LeftButton == ButtonState.Pressed)
            {
                viewRotation.X += e.XDelta;
                viewRotation.Y += e.YDelta;
                Debug.WriteLine(viewRotation);
            }

            if (ms.MiddleButton == ButtonState.Pressed)
            {
                viewPan.X += e.XDelta;
                viewPan.Y += e.YDelta;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Version version = new Version(GL.GetString(StringName.Version).Substring(0, 3));
            Version target = new Version(1, 5);
            if (version < target)
            {
                throw new NotSupportedException(String.Format(
                    "OpenGL {0} is required (you only have {1}).", target, version));
            }

            GL.ClearColor(System.Drawing.Color.MidnightBlue);
            GL.Enable(EnableCap.DepthTest);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(0, 0, Width, Height);

            float aspect_ratio = Width / (float)Height;
            Matrix4 perpective = Matrix4.CreateOrthographic(Width, Height, -1000, 1000);//  .CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspect_ratio, 1, 64);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref perpective);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
            if (Keyboard[OpenTK.Input.Key.Escape])
                this.Exit();
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            if(newPonts != null)
            {
                if (vbo != null)
                    GL.DeleteBuffers(1, ref vbo.VboID);
                vbo = LoadVBO(newPonts.ToArray());
                newPonts = null;
            }

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            Matrix4 lookat = Matrix4.LookAt(5, 5, 5, 0, 0, 0, 0, 0, 1);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref lookat);

            GL.LoadIdentity();
            GL.Translate(viewPan.X, -viewPan.Y, 0);
            GL.Scale(viewScale, viewScale, viewScale);
            GL.Rotate(viewRotation.Y, Vector3d.UnitX);
            GL.Rotate(viewRotation.X, Vector3d.UnitZ);

            GL.Begin(PrimitiveType.Lines);
            GL.Color3(1.0f, 0, 0);

            var length = 1;

            GL.Vertex3(0, 0, 0);
            GL.Vertex3(length, 0, 0);

            GL.Color3(0.0f, 1.0f, 0);
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(0, length, 0);

            GL.Color3(0.0f, 0.0f, 1.0f);
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(0, 0, length);
            GL.End();

            if (vbo != null)
                Draw(vbo);

            SwapBuffers();
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

        void Draw(Vbo handle)
        {
            // To draw a VBO:
            // 1) Ensure that the VertexArray client state is enabled.
            // 2) Bind the vertex and element buffer handles.
            // 3) Set up the data pointers (vertex, normal, color) according to your vertex format.
            // 4) Call DrawElements. (Note: the last parameter is an offset into the element buffer
            //    and will usually be IntPtr.Zero).

            GL.EnableClientState(ArrayCap.ColorArray);
            GL.EnableClientState(ArrayCap.VertexArray);

            GL.BindBuffer(BufferTarget.ArrayBuffer, handle.VboID);
            //GL.BindBuffer(BufferTarget.ElementArrayBuffer, handle.EboID);

            VertexPositionColor[] pc = new VertexPositionColor[] { };
            GL.VertexPointer(3, VertexPointerType.Float, BlittableValueType.StrideOf(pc), new IntPtr(0));
            GL.ColorPointer(4, ColorPointerType.UnsignedByte, BlittableValueType.StrideOf(pc), new IntPtr(12));
            GL.DrawArrays(PrimitiveType.LineStrip, 0, handle.NumElements);
            //GL.DrawElements(PrimitiveType.LineLoop, handle.NumElements/2, DrawElementsType.UnsignedShort, IntPtr.Zero);
        }

    }
}
