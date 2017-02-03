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

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Title = "OpenGLF_Ex test";

            Engine.scene = new Scene();
            engine.resize(800, 600);

            GameObject gameobject = new GameObject();

            gameobject.position = new Vector(Width / 2, Height / 2);
            gameobject.name = "base";
            Engine.scene.GameObjectRoot.addChild(gameobject);

            GameObject cursor = new GameObject();
            gameobject.addChild(cursor);

            cursor.localPosition = new Vector(-100, 0);
            cursor.name = "red cursor";
            cursor.components.Add(new TextureSprite("Assets/cursor.png"));
            cursor.sprite.width = ((TextureSprite)cursor.sprite).Texture.bitmap.Width;
            cursor.sprite.height = ((TextureSprite)cursor.sprite).Texture.bitmap.Height;
            cursor.sprite.center = new Vector(cursor.sprite.width/2, cursor.sprite.height/2);

            cursor.components.Add(new Selectable());

            GameObject cursor2 = new GameObject();
            gameobject.addChild(cursor2);

            cursor2.localPosition = new Vector(100, 0);
            cursor2.name = "green cursor";
            cursor2.components.Add(new TextureSprite("Assets/cursor.png"));
            cursor2.sprite.width = ((TextureSprite)cursor.sprite).Texture.bitmap.Width;
            cursor2.sprite.height = ((TextureSprite)cursor.sprite).Texture.bitmap.Height;
            cursor2.sprite.center = new Vector(cursor.sprite.width / 2, cursor.sprite.height / 2);

            cursor2.components.Add(new Selectable());

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
