using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGLF;
using System.Drawing;

namespace OpenGLF_EX
{
    /// <summary>
    /// This class can help you create a sprite with Textures list for sprite animation.
    /// </summary>
    public class AnimationTextureSprite : SpriteExBase
    {
        protected TextureSequence _textureList;

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

        public TextureSequence Textures{
            get { return _textureList; }
            set
            {
                material.parameters["diffuse"] = new Sampler2D(value);
                _textureList = value;
            }
        }

        public AnimationTextureSprite():base()
        {
            Color = new Vec4(1.0f, 1.0f, 1.0f, 1.0f);
            play();
        }

        public AnimationTextureSprite(Texture[] animateTextureList):this()
        {
            Textures = new TextureSequence();
            Textures.frames = new TextureList();

            foreach (var tex in animateTextureList)
                Textures.frames.Add(tex);
        }

        public AnimationTextureSprite(string[] animateFilePaths) : this()
        {
            Texture[] texArray = new Texture[animateFilePaths.Length];

            Textures = new TextureSequence();
            Textures.frames = new TextureList();

            for (int i = 0; i < animateFilePaths.Length; i++)
            {
                Textures.frames.Add(new Texture(animateFilePaths[i]));
            }    
        }

        public void ChangeColor(Vec4 newColor)
        {
            Color = newColor;
        }

        public void ChangeColor(float r, float g, float b, float a = 1.0f)
        {
            Color = new Vec4(r, g, b, a);
        }

        public override void setColor(float r, float g, float b, float a)
        {
            ChangeColor(r, g, b, a);
        }

    }
}
