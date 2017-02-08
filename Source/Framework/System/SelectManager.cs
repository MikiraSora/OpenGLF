using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL;
using System.Text;
using OpenTK.Input;

namespace OpenGLF
{
    public static class SelectManager
    {
        static List<GameObject> registerGameObject = new List<GameObject>();

        static IntPtr color = Marshal.AllocHGlobal(4);
        static byte[] colorBuffer = new byte[4];

        public static bool isSelecting = false;

        public static void registerSelectObject(GameObject gameObject)
        {
            /*Skip
            if (gameObject.sprite == null)
                throw new Exception("Gameobject must have a sprite (or drawable component)");
            */
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
            isSelecting = true;
            List<GameObject> result = new List<GameObject>();

            Selectable selector;
            GL.Disable(EnableCap.Blend);

            foreach(var obj in registerGameObject)
            {

                if (obj.sprite == null&& obj.getComponent<Selectable>() == null)
                    continue;//skip

                selector = obj.getComponent<Selectable>();
                selector.beforeSelect(ref result, x, y);
                obj.draw(RenderingMode.Render);
                selector.afterSelect();

            }
            
            GL.ReadPixels<byte>(x,Window.CurrentWindow.Height - y, 1, 1, PixelFormat.Rgba, PixelType.UnsignedByte, colorBuffer);

            int id = ByteConverter.byteToInt(colorBuffer);
            GL.Enable(EnableCap.Blend);
            isSelecting = false;
            return id==0?null:Engine.scene.GameObjectRoot.findId(id);
        }

        static GameObject _currentGameObject = null;

        public static void updateMove (int x,int y)
        {

            GameObject selectObj = SelectManager.selectGameObject(x,y);

            if (_currentGameObject != selectObj)
            {
                if (_currentGameObject != null && _currentGameObject.getComponent<Selectable>().Type.HasFlag(Selectable.CALLBACKTYPE.MOVEAREA))
                    _currentGameObject.getComponent<Selectable>().leaveArea();
                if (selectObj != null)
                {
                    if (selectObj.getComponent<Selectable>().Type.HasFlag(Selectable.CALLBACKTYPE.MOVEAREA))
                        selectObj.getComponent<Selectable>().enterArea();
                }
            }

            _currentGameObject = selectObj;
        }

        static GameObject _prevClickGameObject;

        public static void updateClick(MouseEventArgs e)
        {
            GameObject selectObj = selectGameObject(e.X,e.Y);
        }

    }
}
