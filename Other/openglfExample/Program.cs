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

        GameObject gameobject = null;

        int mx, my;

        public GameWindow(int width=800,int height=600):base(width, height)
        {

        }

        void init()
        {
            Engine.debugGameObject = true;

            Title = "OpenGLF_Ex test";

            Engine.scene = new Scene();

            setCursorImage("Assets/cursor.png");

            gameobject = new GameObject();

            gameobject.LocalPosition = new Vector(Width / 2, Height / 2);
            gameobject.name = "base";
            Engine.scene.GameObjectRoot.addChild(gameobject);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            init();

            GameObject ballGameObject = new GameObject();
            gameobject.addChild(ballGameObject);

            ballGameObject.components.Add(new Selectable());
            ballGameObject.components.Add(new ActionExecutor());
            ballGameObject.components.Add(new TextureSprite("Assets/cursor.png"));

            ballGameObject.LocalPosition = new Vector(0,0);
            ballGameObject.sprite.center = new Vector(ballGameObject.sprite.width / 2, ballGameObject.sprite.height / 2);

            LoopAction loopAction = new LoopAction(50000,new ActionBase[]{
                new ScaleToAction(ballGameObject,new Vector(2,2),500,new LinearInterpolator()),
                new ScaleToAction(ballGameObject,new Vector(2,2),500,new LinearInterpolator()).reverse()
            });

            ballGameObject.getComponent<ActionExecutor>().executeAction(
                loopAction
                /*
                new MoveToAction(ballGameObject, (int)ballGameObject.LocalPosition.x, (int)ballGameObject.LocalPosition.y, 200,100,2000,new LinearInterpolator())
                .reverse()  
                */
                /*
                new ColorToAction(ballGameObject,new Vec4(0,0,0,0),new Vec4(1,1,1,1),1000,new LinearInterpolator()).reverse()
                */
                /*
                new ScaleToAction(ballGameObject,new Vector(1,1),new Vector(2,2),3000,new LinearInterpolator()).reverse()
                */
                );
            
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
            Console.WriteLine("select objName = {0}\tx:{5}({1}) l_y:{6}({2}) offsetDepth:{3} AbsoluteDepth={4}\n",
                selectObj.name,
                selectObj.LocalPosition.x,selectObj.LocalPosition.y,
                selectObj.depth,selectObj.FullDepth,
                selectObj.WorldPosition.x, selectObj.WorldPosition.y
                );
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
