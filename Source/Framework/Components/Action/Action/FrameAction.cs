using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenGLF
{
    public class FrameAction : ActionBase
    {

        protected List<ActionBase> _action_list = new List<ActionBase>();
        public List<ActionBase> ActionList { get { return _action_list; } private set { } }

        int _currentActionIndex = 0;

        public FrameAction():base(0,0,null,null)
        {

        }

        public FrameAction(ActionBase[] init_ActionArray):base(0,0,null,null)
        {
            foreach (var action in init_ActionArray)
                _action_list.Add(action);
        }

        public FrameAction(List<ActionBase> init_ActionList) :this(init_ActionList.ToArray()){}

        public override float timeCast()
        {
            float result = 0;
            foreach (var action in _action_list)
            {
                result += action.timeCast();
            }
            return result;
        }

        public override void onStart()
        {

        }

        public override void onAction(float passTime)
        {
            /*
             不用插值器，直接传tick进去
             */
            onUpdate(passTime);
        }

        public override void onUpdate(float passTime)
        {
            if (_done)
                return;

            var _currentAction = _action_list[_currentActionIndex];
            _currentAction.onAction(passTime);

            if (_currentAction.Done)
            {
                //end and reset
                _currentAction.onFinish();
                _currentActionIndex++;
                if (_currentActionIndex >= _action_list.Count)
                {
                    markDone();
                    return;
                }
                _action_list[_currentActionIndex].onStart();
            }
        }

        public override ActionBase reverse()
        {
            List<ActionBase> action_list = new List<ActionBase>();

            for (int i = _action_list.Count - 1; i >= 0; i--)
            {
                action_list.Add(_action_list[i].reverse());
            }

            FrameAction reverseFrame = new FrameAction(action_list);
            return reverseFrame;
        }
    }
}
