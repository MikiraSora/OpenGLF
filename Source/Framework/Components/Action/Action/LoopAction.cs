using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenGLF
{
    public class LoopAction:ActionBase
    {
        protected List<ActionBase> _action_list = new List<ActionBase>();
        public List<ActionBase> ActionList { get { return _action_list; }private set { } }

        bool _mustFinish;

        int _currentActionIndex = 0;

        public LoopAction(bool mustFinish,float time,ActionBase[] init_ActionArray):base(0,time,null,null)
        {
            _mustFinish = mustFinish;

            foreach (var action in init_ActionArray)
                _action_list.Add(action);
        }

        public LoopAction(bool mustFinish,float time, List<ActionBase> init_ActionList) :this(mustFinish,time,init_ActionList.ToArray()){}

        public override void onStart()
        {
            
        }

        public override void onAction(float passTime)
        {
            /*
             不用插值器，直接传tick进去
             */
            _totalTime += passTime;
            onUpdate(passTime);
        }

        public override void onUpdate(float passTime)
        {
            if (_totalTime > _timeEnd)
            {
                markDone();
            }

            var _currentAction = _action_list[_currentActionIndex];
            _currentAction.onAction(passTime);

            if (_currentAction.Done)
            {
                //end and reset
                _currentAction.onFinish();
                _currentActionIndex++;
                if(_currentActionIndex>= _action_list.Count)
                {
                    _currentActionIndex = 0;
                }
                _action_list[_currentActionIndex].onStart();
            }
        }

        public override ActionBase reverse()
        {
            List<ActionBase> action_list = new List<ActionBase>();

            for(int i = _action_list.Count - 1; i >= 0; i--)
            {
                action_list.Add(_action_list[i].reverse());
            }

            LoopAction reverseLoop = new LoopAction(_mustFinish,_timeEnd - _timeStart, action_list);
            return reverseLoop;
        }
    }
}
