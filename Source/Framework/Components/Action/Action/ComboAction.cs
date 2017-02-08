using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenGLF
{
    public class ComboAction : ActionBase
    {
        protected List<ActionBase> _action_list = new List<ActionBase>();
        public List<ActionBase> ActionList { get { return _action_list; } private set { } }

        public List<ActionBase> sync_ActionWrapperList = new List<ActionBase>();

        float maxCast = 0;

        public ComboAction(bool mustAlign, List<ActionBase> init_ActionList) : this(mustAlign, init_ActionList.ToArray()) { }

        public ComboAction(bool mustAlign, ActionBase[] init_ActionArray) : base(0, 0, null, null)
        {
            float maxTimeCast = 0, tmp = 0;
            foreach (var action in init_ActionArray)
            {
                _action_list.Add(action);

                tmp = action.timeCast();
                if (tmp > maxTimeCast)
                    maxTimeCast = tmp;
            }
            if (mustAlign)
            {
                foreach (var acion in init_ActionArray)
                {
                    sync_ActionWrapperList.Add(new FrameAction(new ActionBase[] {
                        acion,
                        new WaitAction(maxTimeCast-acion.timeCast())
                    }));
                }
            }
            else
            {
                foreach (var acion in init_ActionArray)
                {
                    sync_ActionWrapperList.Add(acion);
                }
            }
            maxCast = maxTimeCast;
        }

        public override void onStart()
        {
            foreach (var action in sync_ActionWrapperList)
                action.onStart();
        }

        public override void onFinish()
        {
            base.onFinish();

            foreach (var action in sync_ActionWrapperList)
                action.onFinish();
        }

        public override void onAction(float passTime)
        {
            onUpdate(passTime);
        }

        public override void onUpdate(float passTime)
        {
            bool isAllDone = true;
            foreach (var action in sync_ActionWrapperList) {
                action.onAction(passTime);
                if (!action.Done)
                    isAllDone = false;
            }

            if (isAllDone)
                markDone();
        }

        public override ActionBase reverse()
        {
            List<ActionBase> action_list = new List<ActionBase>();

            for (int i = _action_list.Count - 1; i >= 0; i--)
            {
                action_list.Add(_action_list[i].reverse());
            }

            return new ComboAction(false, action_list);
        }
    }
}
