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
        OpenGLF.Font font = new OpenGLF.Font("Assets/OpenSans-Bold.ttf");
        static byte[] colorBuffer = new byte[4];

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Engine.debugGameObject = true;

            Title = "OpenGLF_Ex test";

            Engine.scene = new Scene();
            engine.resize(800, 600);

            GameObject gameobject = new GameObject();

            gameobject.position = new Vector(Width / 2, Height / 2);
            gameobject.name = "base";
            Engine.scene.GameObjectRoot.addChild(gameobject);

            GameObject textGameObject = new GameObject();
            textGameObject.components.Add(new TextureSprite(font.GenTexture("Hello,OpenGLF!",153,27,20,new OpenGLF.Color(255,0,0,255))));
            gameobject.addChild(textGameObject);
            textGameObject.localPosition = new Vector(0, 0);
            textGameObject.sprite.width = ((TextureSprite)textGameObject.sprite).Texture.bitmap.Width;
            textGameObject.sprite.height = ((TextureSprite)textGameObject.sprite).Texture.bitmap.Height;
            textGameObject.sprite.center = new Vector(textGameObject.sprite.width/2, textGameObject.sprite.height/2);
            textGameObject.name = "text";
            textGameObject.components.Add(new Selectable());

            var size = font.calculateSize("Hello,OpenGLF!", 20);

            Console.WriteLine("font width:{0}\theight:{1}",size.x,size.y);

            #region debugDraw
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
#endregion
        }

        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            base.OnMouseMove(e);

            //GL.ReadPixels<byte>(e.X,e.Y, 1, 1, PixelFormat.Rgba, PixelType.UnsignedByte, colorBuffer);
            //Console.WriteLine("x:{0}\ty:{1}\t---r:{2}g:{3}b:{4}a:{5}",e.X,e.Y,colorBuffer[0], colorBuffer[1],colorBuffer[2],colorBuffer[3]);
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            GameObject selectObj = SelectManager.selectGameObject(e.X,e.Y);
            if (selectObj == null)
                return;
            Console.WriteLine("select objName = {0}\tx:{1} y:{2} offsetDepth:{3} AbsoluteDepth={4}\n-----------------",selectObj.name,selectObj.position.x,selectObj.position.y,selectObj.depth,selectObj.FullDepth);
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

            GameWindow mainWindow = new GameWindow();
            mainWindow.Run();
        }
    }
}
