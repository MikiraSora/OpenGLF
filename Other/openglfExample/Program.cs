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
            
            //for (int i = 0; i < 100; i++)
            {

                GameObject ballGameObject = new GameObject();
                gameobject.addChild(ballGameObject);

                ballGameObject.components.Add(new Selectable());
                ballGameObject.components.Add(new ActionExecutor());
                ballGameObject.components.Add(new TextureSprite("Assets/cursor.png"));

                ballGameObject.getComponent<Selectable>().onEnterArea += () =>
                {
                    Console.WriteLine("enter " + ballGameObject.ID);
                };

                ballGameObject.getComponent<Selectable>().onLeaveArea += () =>
                {
                    Console.WriteLine("leave " + ballGameObject.ID);
                };

                ballGameObject.sprite.center = new Vector(ballGameObject.sprite.width / 2, ballGameObject.sprite.height / 2);

                MoveToAction action = new MoveToAction(ballGameObject, 0, 0, 200, 200, 2000, new EasingInterpolator(EasingInterpolator.EaseType.Linear));

                ballGameObject.getComponent<ActionExecutor>().executeAction(new LoopAction(true, 500000, new ActionBase[] {
                    action,
                    action.reverse()
                }));

            }

            #region debugDraw
            //Draw XY for debug
            engine.beforeDraw += () =>
            {
                Drawing.drawLine(new Vector(10000, my), new Vector(-10000, my), 5, new OpenGLF.Color(255, 255, 0, 125));
                Drawing.drawLine(new Vector(mx, 10000), new Vector(mx, -10000), 5, new OpenGLF.Color(255, 255, 0, 125));

                Drawing.drawLine(new Vector(10000, Height / 2), new Vector(-10000, Height / 2), 5, new OpenGLF.Color(255, 0, 0, 125));
                Drawing.drawLine(new Vector(Width / 2, 10000), new Vector(Width / 2, -10000), 5, new OpenGLF.Color(0, 255, 0, 125));

            };

            /*
             * 
            FILE* f=fopen("gugugu","r"),*newf = fopen("gegege", "w");

            long fileLength = fseek(f, 0, SEEK_END);

            char* fileBuffer = (char*)malloc(fileLength * sizeof(char));

            for (int i = 0; i < fileLength; i++)
                fileBuffer[i] = getc(f);

            for(int i=fileLength-1;i>=0;i--)
                //putc(fileBuffer[i],newf);
            
             */
            
            engine.afterDraw += () =>
            {
                //sprite.Text = string.Format("r:{0:F4}ms,u:{1:F4}ms", (RenderTime * 100), (UpdateTime * 100));
            };
            #endregion
        }

        GameObject selectobject = null;

        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            base.OnMouseMove(e);

            mx = e.X;
            my = e.Y;

            SelectManager.updateMove(e.X, e.Y);
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

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            Title=string.Format("OpenGLF_Ex test : FPS:{0:F2}",UpdateFrequency);
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
