using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenGLF
{
    public class ActionBase
    {
        protected IInterpolator interpolator;

        protected GameObject gameObject;

        protected float _timeStart, _timeEnd,_totalTime=0;

        protected bool _done = false;
        public bool Done { get { return _done; } private set { } }

        public ActionBase(float timeStart,float timeEnd,IInterpolator interpolator,GameObject obj)
        {
            gameObject = obj;
            this.interpolator = interpolator;
            _timeEnd = timeEnd;
            _timeStart = timeStart;
            if (interpolator != null)
            {
                this.interpolator.start = _timeStart;
                this.interpolator.end = _timeEnd;
            }
        }

        private ActionBase() : this(0,0,new LinearInterpolator(),null){}

        protected float onInterpolate(float passTime)
        {
            return interpolator.calculate(passTime);
        }

        public virtual void onUpdate(float norValue)
        {
            throw new NotImplementedException();
        }

        public virtual void onStart()
        {
            this.interpolator.start = _timeStart;
            this.interpolator.end = _timeEnd;
        }

        public virtual void onFinish()
        {
            //reset status
            _done = false;
            _totalTime = 0;
        }

        public virtual void onAction(float passTime)
        {
            _totalTime += passTime;
            onUpdate(onInterpolate(_totalTime));
        }

        public void markDone()
        {
            _done = true;
        }

        public virtual ActionBase reverse()
        {
            throw new NotImplementedException();
        }
    }
}
