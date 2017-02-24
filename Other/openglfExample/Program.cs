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

        GameObject gameobject = null,cameraGameObject;

        public GameWindow(int width=800,int height=600):base(width, height)
        {
            VSync = VSyncMode.Off;
        }

        void init()
        {
            Engine.debugPhysics = true;

            SceneDirector.PushScene(new Scene());

            cameraGameObject = new GameObject();
            cameraGameObject.components.Add(new Camera());

            Engine.scene.GameObjectRoot.addChild(cameraGameObject);

            Engine.scene.mainCamera = cameraGameObject.camera;

            setCursorImage("Assets/cursor.png");
            cameraGameObject.LocalPosition = new Vector(Width / 2, Height / 2);

            gameobject = new GameObject();

            gameobject.LocalPosition = new Vector(Width / 2, Height / 2);
            gameobject.name = "base";
            Engine.scene.GameObjectRoot.addChild(gameobject);

            #region debugDraw
            //Draw XY for debug
            engine.beforeDraw += () =>
            {
                /*
                Drawing.drawLine(new Vector(10000, my), new Vector(-10000, my), 5, new OpenGLF.Color(255, 255, 0, 125));
                Drawing.drawLine(new Vector(mx, 10000), new Vector(mx, -10000), 5, new OpenGLF.Color(255, 255, 0, 125));
                */

                Drawing.drawLine(new Vector(10000, Height / 2), new Vector(-10000, Height / 2), 5, new OpenGLF.Color(255, 0, 0, 125));
                Drawing.drawLine(new Vector(Width / 2, 10000), new Vector(Width / 2, -10000), 5, new OpenGLF.Color(0, 255, 0, 125));

            };

            engine.afterDraw += () =>
            {
                //sprite.Text = string.Format("r:{0:F4}ms,u:{1:F4}ms", (RenderTime * 100), (UpdateTime * 100));
            };
            #endregion
        }

        GameObject obj;

        protected override void OnLoad(EventArgs e)
        {
            init();

            obj = new GameObject();
            gameobject.addChild(obj);
            obj.components.Add(new TextureSprite("Assets/circle.png"));
            obj.components.Add(new Rigidbody());
            obj.WorldPosition = new Vector(400, 0);
            obj.rigidbody.buildCircleShape(obj.sprite.width/2);
            obj.sprite.center = new Vector(obj.sprite.width / 2, obj.sprite.height / 2);
            obj.rigidbody.Body.SleepingAllowed = false;
            obj.rigidbody.restitution = 0.5f;

            GameObject blank = new GameObject();
            gameobject.addChild(blank);
            blank.components.Add(new TextureSprite("Assets/blank.png"));
          
            blank.components.Add(new Rigidbody());
            blank.sprite.width = 800;
            blank.rigidbody.buildRectangleShape(blank.sprite.width, blank.sprite.height-60);
            
            blank.sprite.center = new Vector(blank.sprite.width / 2, blank.sprite.height / 2);

            blank.WorldPosition = new Vector(400, 550);
            blank.LocalAngle = 10;

            base.OnLoad(e);

            blank.rigidbody.bodyType = BodyType.Static;
        }

        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            base.OnMouseMove(e);
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            obj.WorldPosition = new Vector(e.X, e.Y);

            GameObject selectObj = SelectManager.selectGameObject(e.X,e.Y);
            if (selectObj == null)
                return;
            Log.Debug("select objName = {0}\tx:{5}({1}) l_y:{6}({2}) offsetDepth:{3} AbsoluteDepth={4}\n",
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

            Title=string.Format("OpenGLF_Ex test : FPS:{0:F2} ",UpdateFrequency);
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);

            switch (e.KeyChar)
            {
                case 'w':
                    obj.rigidbody.addForce(new Vector(-100,-100), new Vector(obj.rigidbody.Body.WorldCenter.X, obj.rigidbody.Body.WorldCenter.Y));
                    Log.User("body x={0}\ty={1}", obj.rigidbody.Body.WorldCenter.X, obj.rigidbody.Body.WorldCenter.Y);
                    break;
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            GameWindow mainWindow = new GameWindow();
            mainWindow.Run(120,120);
        }
    }
}
