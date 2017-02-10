using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenGLF
{
    class RenderSystem
    {

    }

    public class RenderCommandBase
    {
        internal int _renderOrder = 0;

        internal Shader _shader = null;

        internal Dictionary<string, object> _paramters;

        internal Texture texture;

        public virtual void ready()
        {

        }

        public virtual void draw()
        {

        }
    }
}
