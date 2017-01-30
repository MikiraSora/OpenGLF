using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGLF;
using OpenGLF_EX;

namespace openglfExample
{
    class CircleBall:GameObject
    {
        public CircleBall()
        {
            this.components.Add(new Ball("Assets/cursor.png"));

            this.position = new Vector(Window.CurrentWindow.Width / 2, Window.CurrentWindow.Width / 2);
            this.sprite.center = new Vector(((Ball)this.sprite).Texture.bitmap.Width / 2, ((Ball)this.sprite).Texture.bitmap.Height / 2);
        }

        class Ball : TextureSprite
        {
            public Ball(string path) : base(path) { }

            float angle = 0;

            public override void update()
            {
                base.update();
                angle += 0.02f;
                this.gameObject.position = new Vector(Window.CurrentWindow.Width / 2+(float)Math.Cos(angle)*200, Window.CurrentWindow.Height / 2+(float)Math.Sin(angle)*200);
            }
        }
    }
}
