using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGLF;

namespace OpenGLF_EX
{
    public class TextSprite : TextureSprite
    {
        string text = "";
        int text_size = 0;
        OpenGLF.Color text_color;
        OpenGLF.Font font = null;

        public Font Font {
            get { return font; }
            set {
                font = value;
                updateTexture();
            }
        }

        public int FontWidth
        {
            get { return width; }
            set
            {
				width = value;
                updateTexture();
            }
        }

        public int TextSize
        {
            get { return text_size; }
            set
            {
                text_size = value;
                updateTexture();
            }
        }

        public string Text
        {
            get { return text; }
            set
            {
                text = value;
                updateTexture();
            }
        }

        public Color TextColor
        {
            get { return text_color; }
            set
            {
                text_color = value;
                updateTexture();
            }
        }

        public TextSprite(string text,int size, OpenGLF.Color color, OpenGLF.Font font, int width) :base()
        {
            this.text = text;
            text_size = size;
            text_color = color;
            this.width = width;
            this.font = font;

            Color = new Vec4(1.0f, 1.0f, 1.0f, 1.0f);

            updateTexture();
        }

        protected void updateTexture()
        {
            var real_size = font.calculateSize(text, text_size, width);
            if (this.Texture != null)
            {
                this.Texture.bitmap.Dispose();
                this.Texture.Dispose();
            }
            this.Texture = font.GenTexture(text, (int)real_size.x, (int)real_size.y, text_size, text_color);
            width = Texture.bitmap.Width;
            height = Texture.bitmap.Height;
        }
    }
}
