using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGLF;
using System.Drawing;
using OpenTK.Graphics.OpenGL;

namespace OpenGLF_EX
{
    /// <summary>
    /// TextureSprite can help you create a sprite conveniently.Auto setup material and shader as default;
    /// </summary>
    public class TextureSprite : SpriteExBase
    {
        protected Texture _texture;

        public Texture Texture
        {
            get { return _texture; }
            set {
                material.parameters["diffuse"] = new Sampler2D(value);
                _texture = value;
            }
        }

        protected Vec4 _color;

        public Vec4 Color
        {
            get { return _color; }
            set
            {
                material.parameters["colorkey"] = value;
                _color = value;
            }
        }

        public TextureSprite() : base()
        {

        }

        public TextureSprite(Texture texture,int width=-1,int height=-1):this()
        {
            Texture = texture;
            Color = new Vec4(1.0f, 1.0f, 1.0f, 1.0f);
            if (width < 0 || height < 0)
            {
                width = Texture.bitmap.Width;
                height = Texture.bitmap.Height;
            }
            else
            {
                this.width = width;
                this.height = height;
            }
        }

        public TextureSprite(string picFilePath):this(new Texture(picFilePath))
        {

        }

        public void ChangeColor(Vec4 newColor)
        {
            Color = newColor;
        }

        public void ChangeColor(float r,float g,float b,float a = 1.0f)
        {
            Color = new Vec4(r, g, b, a);
        }

        public override void setColor(float r, float g, float b, float a)
        {
            ChangeColor(r, g, b, a);
        }
    }
}
