using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenGLF
{
    public class ScaleToAction : ActionBase
    {
        Vector startScale, endScale;

        public ScaleToAction(GameObject gameobject, Vector endScale, float time, IInterpolator interpolator) :this(gameobject,gameobject.LocalScale,endScale,time,interpolator){ }

        public ScaleToAction(GameObject gameobject, Vector startScale, Vector endScale, float time, IInterpolator interpolator) : base(0, time, interpolator, gameobject)
        {
            this.startScale = startScale;
            this.endScale = endScale;
        }

        public override void onUpdate(float norValue)
        {
            if (norValue >= 1)
            {
                markDone();

                gameObject.LocalScale = endScale;
            }

            float sx = norValue * (endScale.x - startScale.x);
            float sy = norValue * (endScale.y - startScale.y);

            gameObject.LocalScale = new Vector(startScale.x + sx, startScale.x + sy);
        }

        public override ActionBase reverse()
        {
            return new ScaleToAction(gameObject, endScale, startScale, _timeEnd - _timeStart, interpolator.reverse());
        }
    }
}
