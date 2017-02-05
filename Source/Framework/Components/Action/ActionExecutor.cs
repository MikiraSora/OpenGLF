using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenGLF
{
    public class ActionExecutor : Component
    {
        protected ActionBase _currentAction = null;
        public ActionBase CurrentAction { get { return _currentAction; } private set { } }

        protected long _prev_tick = -1;

        public void executeAction(ActionBase action)
        {
            _currentAction = action;
        }

        public override void update()
        {
            if (_currentAction == null)
                return;
            if (gameObject.sprite == null)
                return;

            if (_prev_tick < 0)
            { 
                //reset and prepare
                _prev_tick = Time.getMsTick();
                _currentAction.onStart();
            }

            var time = Time.getMsTick() - _prev_tick;

            _currentAction.onAction(time);

            if (_currentAction.Done)
            {
                //end and reset
                _currentAction.onFinish();
                _currentAction = null;
                _prev_tick = -1;
            }

            _prev_tick = Time.getMsTick();
        }
    }
}
