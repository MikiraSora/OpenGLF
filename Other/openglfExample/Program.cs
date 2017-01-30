using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGLF;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenGLF_EX;
using System.Drawing;
using System.Runtime.InteropServices;
using OpenTK.Input;

namespace openglfExample
{

    class GameWindow : Window
    {
        GameObject gameObj,frameGameObj;
        OpenGLF.Font font;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Title = "OpenGLF_Ex test";

            Engine.scene = new Scene();
            engine.resize(800, 600);

            var gameObject = new GameObject();

            string[] texArr = new string[31];
            for (int i = 0; i < 31; i++)
                texArr[i] = string.Format(@"Assets/menu-back-{0}.png", i);

            AnimationTextureSprite sprite = new AnimationTextureSprite(texArr);
            sprite.speed = 6.0f;

            sprite.width = sprite.Textures.frames[0].bitmap.Width;
            sprite.height = sprite.Textures.frames[0].bitmap.Height;
            sprite.scale = new Vector(1.5f, 1.5f);

            gameObject.components.Add(sprite);

            gameObject.angle = 0;
            gameObject.position = new Vector(Width / 2, Height / 2);
            gameObject.sprite.center = new Vector(sprite.Textures.frames[0].bitmap.Width / 2, sprite.Textures.frames[0].bitmap.Height / 2);

            gameObj = new BlurBall(1,Width, Height);
            /*
            frameGameObj = new GameObject();
            frameGameObj.components.Add(new TextureSprite(((BlurBall)gameObj).Frames[0].Texture,Width,Height));
            */
            Engine.scene.GameObjectRoot.GameObjectChildren.Add(gameObj);

            Engine.scene.GameObjectRoot.GameObjectChildren.Add(gameObject);

            CircleBall ball = new CircleBall();

            Engine.scene.GameObjectRoot.GameObjectChildren.Add(ball);

            var rtex = new Texture("Assets/cursor.png");
            ReflectionBall rball;
            for (int i = 0; i < 40; i++) {
                rball = new ReflectionBall(rtex, (float)OpenGLF.Random.range(0+100, Width-100), (float)OpenGLF.Random.range(0+100, Height-100));
                Engine.scene.GameObjectRoot.GameObjectChildren.Add(rball);
            }

            font = new OpenGLF.Font("Assets/OpenSans-Bold.ttf");

            //Draw XY for debug
            engine.beforeDraw += () =>
            {
                //Console.WriteLine(this.Context.GraphicsMode.Buffers);
                Drawing.drawLine(new Vector(10000, Height / 2), new Vector(-10000, Height / 2), 5, new OpenGLF.Color(255, 0, 0, 125));
                Drawing.drawLine(new Vector(Width / 2, 10000), new Vector(Width / 2, -10000), 5, new OpenGLF.Color(0, 255, 0, 125));
             };
            
            engine.afterDraw += () =>
            {
                //Drawing.drawText(new Vector(Width - 250, Height - 30), Vector.zero, new Vector(1, 1), 0, 250, 50, string.Format("r:{0:F2}ms,u:{1:F2}ms", (RenderTime * 100), (UpdateTime * 100)), new OpenGLF.Color(0,125,125,125), 20, font);
            };
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            IntPtr allocMessage = new IntPtr(1024);
            //GL.DebugMessageCallback(onDebugMessage, IntPtr.Zero);
            GameWindow mainWindow = new GameWindow();
            mainWindow.Run();
        }

        public static void onDebugMessage(DebugSource source, DebugType type, int id, DebugSeverity severity, int length, IntPtr message, IntPtr userParam)
        {
            string ss = Marshal.PtrToStringAuto(message,length);
            Console.WriteLine("{0}-{1} id {2}:{3}",source.ToString(),severity.ToString(),id,message);
        }
    }
}
