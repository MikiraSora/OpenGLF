using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Windows.Forms;
using System.IO;
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using System.Reflection;
using System.Drawing;
using System.Diagnostics;
using System.Drawing.Imaging;
using OpenTK.Input;

namespace OpenGLF
{
    public class Window : OpenTK.GameWindow
    {
        public Engine engine;
        static Window self;
        public static bool showCursor { get { return self.CursorVisible; } set { self.CursorVisible = value; } }

        public static Window CurrentWindow { get { return self; } private set { } }

        public Window(int width=800,int height=600):base(width,height)
        {
            self = this;
            showCursor = true;

            this.VSync = VSyncMode.Adaptive;
            engine = new Engine();

            try
            {
                engine.loadSeparatedAssets("Assets", true);
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
           
            Icon = Icon.ExtractAssociatedIcon(Assembly.GetEntryAssembly().Location);
            engine.resize(Width, Height);
        }

        /// <summary>
        /// 改变窗口大小
        /// </summary>
        /// <param name="width">新窗口的宽度</param>
        /// <param name="height">新窗口的高度</param>
        public void resizeWindow(int width,int height)
        {
            base.Size = new Size(width, height);
            engine.resize(width, height);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            engine.draw(RenderingMode.Render, true);
            SwapBuffers();
        }

        protected override void OnResize(EventArgs e)
        {
            engine.resize(Width, Height);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            int w = 0;
            int h = 0;
            float zoom = 1;
            float x = 0;
            float y = 0;
    
            if (Camera.main != null)
            {
                w = Screen.width / 2;
                h = Screen.height / 2;
                zoom = Camera.main.z;
                x = (float)Camera.main.gameObject.LocalPosition.x;
                y = (float)Camera.main.gameObject.LocalPosition.y;
            }

            Input._mouseMapWindowPos = new Vector(Mouse.X,Mouse.Y);

            Input._mousePos = new Vector(Mouse.X * zoom - w * zoom + x, Mouse.Y * zoom - h * zoom + y);

            Input._mouseInfinityPos = new Vector(OpenTK.Input.Mouse.GetState().X, OpenTK.Input.Mouse.GetState().Y);

            engine.update((float)e.Time, true);

            Thread.Sleep((int)e.Time);
        }

        protected override void OnMouseWheel(OpenTK.Input.MouseWheelEventArgs e)
        {
            if (Input._wheel < e.Value)
                Input._wheelState = -1;
            else
                Input._wheelState = 1;

            Input._wheel = e.Value;
        }

        GameObject downObject=null;

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            downObject=SelectManager.updateClick(e);
        }

        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            base.OnMouseMove(e);
            SelectManager.dragUpdateClick(e);
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);
            downObject = null;
            SelectManager.updateReleaseClick();
        }

        public static void setFullscreen(bool full)
        {
            if (full)
                self.WindowState = OpenTK.WindowState.Fullscreen;
            else
                self.WindowState = OpenTK.WindowState.Normal;
        }

        /// <summary>
        /// 改变在窗口内鼠标的指针图像
        /// </summary>
        /// <param name="filePath">图片文件路径</param>
        /// <param name="centerX">定义指针中心X轴对应的图片坐标，-1自动取中心</param>
        /// <param name="centerY">定义指针中心Y轴对应的图片坐标，-1自动取中心</param>
        public void setCursorImage(string filePath,int centerX=-1,int centerY=-1)
        {
            Bitmap bitmap=null;
            BitmapData data=null;
            try
            {
                bitmap = new Bitmap(filePath);
                centerX = centerX < 0 ? bitmap.Width / 2 : centerX;
                centerY = centerY < 0 ? bitmap.Height / 2 : centerY;
                data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                this.Cursor = new MouseCursor(centerX, centerY, bitmap.Width, bitmap.Height, data.Scan0);
            }
            catch (Exception e)
            {
                Log.Error("set custom cursor failed!{0}", e.Message);
            }
            finally
            {
                if (bitmap != null)
                {
                    if (data !=null)
                    {
                        bitmap.UnlockBits(data);
                    }   
                    bitmap.Dispose();             
                }
            }

        }

        public static void close()
        {
            self.Close();
        }
    }
}
