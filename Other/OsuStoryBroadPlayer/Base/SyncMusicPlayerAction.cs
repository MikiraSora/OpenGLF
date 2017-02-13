using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGLF;
using IrrKlang;

namespace OsuStoryBroadPlayer
{
    class SyncMusicPlayerAction:ActionBase
    {
        StoryBroadInitializer _initializer = null;
        long _trigger = 0;

        public SyncMusicPlayerAction(GameObject gameobject,StoryBroadInitializer initializer,long trigger):base(0,float.MaxValue,null,gameobject)
        {

        }

        public override void onAction(float passTime)
        {
            onUpdate(0);
        }

        public override void onUpdate(float norValue)
        {
            if(_initializer._currentPlayer.PlayPosition>=_trigger)
            {
                markDone();
            }
        }
    }
}
