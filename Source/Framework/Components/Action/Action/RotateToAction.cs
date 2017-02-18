using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenGLF
{
    public class RotateToAction : ActionBase
    {
        float _startAngle, _endAngle;

        public RotateToAction(GameObject gameObject,float startAngle,float endAngle,float time,IInterpolator interpolator) : base(0, time, interpolator, gameObject)
        {
            _startAngle = startAngle;
            _endAngle = endAngle;
        }

        public RotateToAction(GameObject gameObject,float endAngle,float time,IInterpolator interpolator) : this(gameObject, gameObject.LocalAngle, endAngle, time, interpolator) { }

        public override void onUpdate(float norValue)
        {
            if (norValue >= 1)
            {
                markDone();
                gameObject.LocalAngle = _endAngle;
            }

            float angle = norValue * (_endAngle - _startAngle);

            gameObject.LocalAngle =/*gameObject.LocalAngle +*/_startAngle+ angle;
        }

        public override ActionBase reverse()
        {
            return new RotateToAction(gameObject,_endAngle, _startAngle, _timeEnd - _timeStart, interpolator.reverse());
        }
    }
}
