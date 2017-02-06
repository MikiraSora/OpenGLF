using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenGLF
{
    public class WaitAction : ActionBase
    {
        public WaitAction(float time):base(0,time,null,null){ }

        public override void onFinish()
        {
            base.onFinish();
        }

        public override void onStart()
        {
            _totalTime = 0;
        }

        public override void onAction(float passTime)
        {
            _totalTime += passTime;
            onUpdate(0);
        }

        public override void onUpdate(float norValue)
        {
            if (_totalTime > _timeEnd)
                markDone(); 
        }

        public override ActionBase reverse()
        {
            return new WaitAction(_timeEnd-_timeStart);
        }
    }
}
