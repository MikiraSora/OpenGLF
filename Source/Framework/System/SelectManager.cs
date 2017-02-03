using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL;
using System.Text;

namespace OpenGLF
{
    public static class SelectManager
    {
        static List<GameObject> registerGameObject = new List<GameObject>();

        static IntPtr color = Marshal.AllocHGlobal(4);
        static byte[] colorBuffer = new byte[4];

        public static void registerSelectObject(GameObject gameObject)
        {
            if (gameObject.sprite == null)
                throw new Exception("Gameobject must have a sprite (or drawable component)");
            if (registerGameObject.Contains(gameObject))
                return;
            registerGameObject.Add(gameObject);
        }

        public static void unregisterSelectObject(GameObject gameObject)
        {
            registerGameObject.Remove(gameObject);
        }

        public static GameObject selectGameObject(int x,int y)
        {
            List<GameObject> result = new List<GameObject>();

            Selectable selector;
            GL.Disable(EnableCap.Blend);

            foreach(var obj in registerGameObject)
            {
                
                obj.getComponent<Selectable>().beforeSelect(ref result, x, y);
                obj.draw(RenderingMode.Render);
                obj.getComponent<Selectable>().afterSelect();
            }
            
            GL.ReadPixels<byte>(x,y, 1, 1, PixelFormat.Rgba, PixelType.UnsignedByte, colorBuffer);

            int id = ByteConverter.byteToInt(colorBuffer);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            return Engine.scene.GameObjectRoot.findId(id);
        }
    }
}
