using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenGLF
{
    class ComboAction : ActionBase
    {
        protected List<ActionBase> _action_list = new List<ActionBase>();
        public List<ActionBase> ActionList { get { return _action_list; } private set { } }

        public List<ActionBase> sync_ActionWrapperList = new List<ActionBase>();

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
    }
}
