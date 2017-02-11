using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenGLF
{
    public class ColorToAction : ActionBase
    {
        Vec4 startColor, endColor;

        public ColorToAction(GameObject gameobject, Vec4 endColor, float time, IInterpolator interpolator):this(gameobject,gameobject.sprite.getColor(),endColor,time,interpolator){}

        public ColorToAction(GameObject gameobject,Vec4 startColor,Vec4 endColor,float time,IInterpolator interpolator) : base(0,time,interpolator,gameobject)
        {
            this.startColor = startColor;
            this.endColor = endColor;
        }

        public override void onUpdate(float norValue)
        {
            if (norValue >= 1)
            {
                markDone();
                gameObject.sprite.setColor(endColor.x , endColor.y, endColor.z , endColor.w );
            }

            float r = norValue * (endColor.x - startColor.x);
            float g = norValue * (endColor.y - startColor.y);
            float b = norValue * (endColor.z - startColor.z);
            float a = norValue * (endColor.w - startColor.w);

            gameObject.sprite.setColor(startColor.x+r, startColor.y + g,startColor.z + b,startColor.w+ a);
        }

        public override ActionBase reverse()
        {
            return new ColorToAction(gameObject,endColor,startColor,_timeEnd-_timeStart,interpolator.reverse());
        }
    }
}
