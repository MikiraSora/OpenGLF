using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenGLF
{
    class ImmediatelyActionBase : ActionBase
    {
        public ImmediatelyActionBase(GameObject gameobject) : base(0, 0, null, gameobject) {}

        public override void onAction(float passTime)
        {
            onUpdate(0); 
        }

        public override void onStart()
        {
            
        }

        public override float timeCast()
        {
            return 0;
        }

        public override void onFinish()
        {
            
        }

        public override ActionBase reverse()
        {
            return this; //Normally,immediately action can reuse without any resetting operation.
        }

        public override void onUpdate(float norValue)
        {
            onUpdate();
            markDone(); // just run once
        }

        public virtual void onUpdate()
        {

        }
    }
}
