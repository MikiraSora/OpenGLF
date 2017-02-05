using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGLF_EX;
using OpenGLF;

namespace openglfExample
{
    class ReflectionBall :GameObject
    {
        public ReflectionBall(Texture texture,float x,float y)
        {
            this.components.Add(new Ball(texture));
            sprite.center = new Vector(((Ball)sprite).Texture.bitmap.Width / 2, ((Ball)sprite).Texture.bitmap.Height / 2);
            ((Ball)sprite).x = x;
            ((Ball)sprite).y = y;
        }

        class Ball : TextureSprite
        {
            public Ball(string path) : base(path) { }
            public Ball(Texture tex) : base(tex) { }

            float move_speed=5;

            public float x = 40;
            public float y = 30;
            bool xAdd = OpenGLF.Random.range(0,100)>50?true:false;
            bool yAdd = OpenGLF.Random.range(0, 100)>50 ? true : false;

            public override void update()
            {
                base.update();
                if (yAdd)
                    y += move_speed;
                else
                    y -= move_speed;
                if (y >=  Window.CurrentWindow.Height - Texture.bitmap.Height/2 || y <= Texture.bitmap.Height / 2)
                    yAdd = !yAdd;
                if (xAdd)
                    x += move_speed;
                else
                    x -= move_speed;
                if (x >= Window.CurrentWindow.Width - Texture.bitmap.Width / 2 || x <= Texture.bitmap.Width / 2)
                    xAdd = !xAdd;

                gameObject.LocalPosition = new Vector(x,y);
            }
        }
    }
}
