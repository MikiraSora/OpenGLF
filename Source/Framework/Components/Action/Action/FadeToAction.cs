using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenGLF
{
    class FadeToAction : ActionBase
    {
        float _startAlpha, _endAlpha;

        public FadeToAction(GameObject gameObject,float startAlpha,float endAlpha,float time,IInterpolator interpolator):base(0,time,interpolator,gameObject)
        {
            _startAlpha = startAlpha;
            _endAlpha = endAlpha;
        }

        public override void onUpdate(float norValue)
        {
            if (norValue >= 1)
            {
                markDone();
            }

            float a = norValue * (_endAlpha - _startAlpha);

            var color = gameObject.sprite.getColor();
            gameObject.sprite.setColor(color.x, color.y, color.z, a);
        }

        public override ActionBase reverse()
        {
            return base.reverse();
        }
    }
}
