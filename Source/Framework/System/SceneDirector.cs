using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenGLF
{
    public static class SceneDirector
    {
        private static Stack<Scene> _sceneStack = new Stack<Scene>();

        public static Scene CurrentScene { get { return _sceneStack.Count==0?null:_sceneStack.Peek(); } }

        private static bool shouldPopScene = false;

        public static void PushScene(Scene scene/*,SwitchScene transformSwitchScene=null*/)
        {
            _sceneStack.Push(scene);
            Log.Debug("Push scene {0} ,index={1}", scene.name, _sceneStack.Count);
        }

        internal static void update()
        {
            if (shouldPopScene)
            {
                if (_sceneStack.Count != 0)
                {
                    var prev_scene = _sceneStack.Pop();
                    prev_scene.onLeaveScene();
                    Log.User("Pop scene {0},now scene is {1}", prev_scene.name, _sceneStack.Peek().name);
                    _sceneStack.Peek().onEnterScene();
                }
            }
        }

        /// <summary>
        /// 标记当前SCENE退出(，下一循环才真正切换上一个SCENE来循环).
        /// </summary>
        public static void exitCurrentScene()
        {
            shouldPopScene = true;
        }
    }
}
