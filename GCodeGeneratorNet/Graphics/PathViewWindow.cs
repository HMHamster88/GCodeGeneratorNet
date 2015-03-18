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

        IEnumerable<Path3D> paths;

        public ViewWindow()
            : base(800, 600)
        {
            Mouse.Move += Mouse_Move;
            Mouse.WheelChanged += Mouse_WheelChanged;
            this.Title = "3D View";
        }

        public void LoadPoints(IEnumerable<Path3D> paths)
        {
            if(paths != null)
            {
                foreach (var path in paths)
                {
                    path.Dispose();
                }
            }
            this.paths = paths;
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

            if(paths != null)
            foreach (var path in paths)
            {
                if (!path.Initalized)
                {
                    path.Init();
                }
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

            if (paths != null)
            foreach (var path in paths)
            {
                path.Draw();
            }

            SwapBuffers();
        }
    }
}
