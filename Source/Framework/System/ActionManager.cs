using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenGLF
{
    class ActionManager
    {
        static List<GameObject> registerGameObject = new List<GameObject>();

        public static void registerSelectObject(GameObject gameObject)
        {
            if (registerGameObject.Contains(gameObject))
                return;
            registerGameObject.Add(gameObject);
        }

        public static void unregisterSelectObject(GameObject gameObject)
        {
            registerGameObject.Remove(gameObject);
        }

        public static void executeAction()
        {

        }
    }
}
