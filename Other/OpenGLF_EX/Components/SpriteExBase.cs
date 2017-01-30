using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenGLF;
using OpenTK.Graphics.OpenGL;
using System.Threading.Tasks;

namespace OpenGLF_EX
{
    public class SpriteExBase : Sprite
    {
        public SpriteExBase()
        {
            material = new Material();
            material.shader = Engine.shaders.defaultShader;
        }
    }
}
