using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGLF;

namespace OsuStoryBroadPlayer
{
    public class SpriteGameObjectDistoryAction : ImmediatelyActionBase
    {
        public SpriteGameObjectDistoryAction(SpriteGameObject gameobject) : base(gameobject) { }

        public override void onUpdate()
        {
            Engine.scene.GameObjectRoot.removeChild(gameObject);
            Log.User("Sprite {0} was distory!",((SpriteGameObject)gameObject)._sbSprite._imgPath);
        }
    }
}
