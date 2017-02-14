using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenGLF
{
    public static class SceneDirector
    {
        static Stack<Scene> _sceneStack = new Stack<Scene>();
        static bool _isInit = false;

        static Scene prev_Scene = null;

        public static Scene CurrentScene { get { return _sceneStack.Peek(); } }

        public static void PushScene(Scene scene,SwitchScene transformSwitchScene=null)
        {
            
        }

        public static void exitScene()
        {

        }
    }
}
