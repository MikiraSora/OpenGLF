﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenGLF
{
    public class FadeToAction : ActionBase
    {
        float _startAlpha, _endAlpha;

        public FadeToAction(GameObject gameObject,float startAlpha,float endAlpha,float time,IInterpolator interpolator):base(0,time,interpolator,gameObject)
        {
            _startAlpha = startAlpha;
            _endAlpha = endAlpha;
        }

        public override void onUpdate(float norValue)
        {
            var color = gameObject.sprite.getColor();

            if (norValue >= 1)
            {
                markDone();
                gameObject.sprite.setColor(color.x, color.y, color.z, _endAlpha);
            }

            float a = norValue * (_endAlpha - _startAlpha);

            gameObject.sprite.setColor(color.x, color.y, color.z, _startAlpha+a);
        }

        public override ActionBase reverse()
        {
            return new FadeToAction(gameObject,_endAlpha,_startAlpha,_timeEnd-_timeStart, interpolator.reverse());
        }
    }
}
