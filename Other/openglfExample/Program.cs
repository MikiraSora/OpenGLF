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

        int mx, my;

        public GameWindow(int width=800,int height=600):base(width, height)
        {

        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Engine.debugGameObject = true;

            Title = "OpenGLF_Ex test";

            Engine.scene = new Scene();

            GameObject gameobject = new GameObject();

            gameobject.position = new Vector(Width / 2, Height / 2);
            gameobject.name = "base";
            Engine.scene.GameObjectRoot.addChild(gameobject);

            
            GameObject textGameObject = new GameObject();
            gameobject.addChild(textGameObject);

            TextSprite sprite = new TextSprite("苟利国家生死已，\n岂因祸福避趋之。", 20, new OpenGLF.Color(255, 255, 0, 255),font);

            textGameObject.components.Add(sprite);
            textGameObject.name = "text object";
            textGameObject.localPosition = new Vector(100,-100);
            textGameObject.sprite.center = new Vector(textGameObject.sprite.width / 2, textGameObject.sprite.height / 2);

            //make this gameobject selectable(must after sprite add)
            textGameObject.components.Add(new Selectable());

            setCursorImage("Assets/cursor.png");

            #region debugDraw
            //Draw XY for debug
            engine.beforeDraw += () =>
            {

                Drawing.drawLine(new Vector(10000, my), new Vector(-10000, my), 5, new OpenGLF.Color(255, 255, 0, 125));
                Drawing.drawLine(new Vector(mx, 10000), new Vector(mx, -10000), 5, new OpenGLF.Color(255, 255, 0, 125));

                Drawing.drawLine(new Vector(10000, Height / 2), new Vector(-10000, Height / 2), 5, new OpenGLF.Color(255, 0, 0, 125));
                Drawing.drawLine(new Vector(Width / 2, 10000), new Vector(Width / 2, -10000), 5, new OpenGLF.Color(0, 255, 0, 125));

             };
            
            engine.afterDraw += () =>
            {
                //sprite.Text = string.Format("r:{0:F4}ms,u:{1:F4}ms", (RenderTime * 100), (UpdateTime * 100));
            };
#endregion
        }

        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            base.OnMouseMove(e);

            mx = e.X;
            my = e.Y;
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            GameObject selectObj = SelectManager.selectGameObject(e.X,e.Y);
            if (selectObj == null)
                return;
            Console.WriteLine("select objName = {0}\tx:{1} y:{2} offsetDepth:{3} AbsoluteDepth={4}\n",selectObj.name,selectObj.position.x,selectObj.position.y,selectObj.depth,selectObj.FullDepth);
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
