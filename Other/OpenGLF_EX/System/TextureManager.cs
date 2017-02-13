using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGLF;

namespace OpenGLF_EX
{
    public static class TextureManager
    {
        static Dictionary<String, Texture> _textureMap = new Dictionary<string, Texture>();
        public static Dictionary<String,Texture> TextureMap { get { return _textureMap; } }

        public static void PushTexture(string cacheName,Texture tex)
        {
            Log.User("loaded texture "+cacheName);
            _textureMap[cacheName] = tex;
        }

        public static void removeTexture(string cacheName)
        {
            _textureMap.Remove(cacheName);
        }

        public static Texture cacheTexture(string cacheName, onLoadResourceTexture loader)
        {
            if (_textureMap.ContainsKey(cacheName))
                return _textureMap[cacheName];

            PushTexture(cacheName, loader(cacheName));
            return _textureMap[cacheName];
        }

        public delegate Texture onLoadResourceTexture(string cacheName);
    }
}
