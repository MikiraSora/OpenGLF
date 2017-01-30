using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGLF;

namespace OpenGLF_EX
{
    public class RawTexture : Texture
    {
        public RawTexture(int textureId)
        {
            _id = textureId;
        }

        public void ChangeTextureID(int newTextureID)
        {
            _id = newTextureID;
        }
    }
}
