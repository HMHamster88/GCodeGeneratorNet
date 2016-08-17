using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
using System.Windows.Forms;

namespace GCodeGeneratorNet.Graphics
{
    class PathViewControl : GLControl
    {
        bool loaded = false;
        Vector3 viewRotation = new Vector3(-135, -45, 0);
        Vector3 viewPan;
        float viewScale = 1;

        IEnumerable<Path3D> paths;

        System.Drawing.Point lastMousePoint;
        MouseButtons lastButton;

        public PathViewControl()
        {
            MouseMove += PathViewControl_MouseMove;
            MouseWheel += PathViewControl_MouseWheel;
        }

        private void PathViewControl_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Delta > 0)
            {
                viewScale /= 2;
            }
            else
            {
                viewScale *= 2;
            }
            Invalidate();
        }

        private void PathViewControl_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if(lastMousePoint != null && e.Button == lastButton)
            {
                System.Drawing.Point delta = new System.Drawing.Point(e.Location.X - lastMousePoint.X,
                    e.Location.Y - lastMousePoint.Y);
                if (e.Button == MouseButtons.Left)
                {
                    viewRotation.X += delta.X;
                    viewRotation.Y += delta.Y;
                }

                if (e.Button == MouseButtons.Middle)
                {
                    viewPan.X += delta.X;
                    viewPan.Y += delta.Y;
                }
            }
            lastMousePoint = e.Location;
            lastButton = e.Button;
            Invalidate();
        }

        public void LoadPoints(IEnumerable<Path3D> paths)
        {
            if (paths != null)
            {
                foreach (var path in paths)
                {
                    path.Dispose();
                }
            }
            this.paths = paths;
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
            loaded = true;
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if(!loaded)
            {
                return;
            }
            GL.Viewport(0, 0, Width, Height);

            float aspect_ratio = Width / (float)Height;
            Matrix4 perpective = Matrix4.CreateOrthographic(Width, Height, -1000, 1000);//  .CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspect_ratio, 1, 64);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref perpective);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (!loaded)
            {
                return;
            }
            if (paths != null)
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

            GL.Begin(BeginMode.Lines);
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
