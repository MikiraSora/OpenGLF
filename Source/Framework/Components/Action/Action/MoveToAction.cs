using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenGLF
{
    public class MoveToAction : ActionBase
    {
        int endX,endY,startX,startY;
        float angle;

        public MoveToAction(GameObject gameObject, int endX, int endY, float time, IInterpolator interpolatior) : this(gameObject, (int)gameObject.LocalPosition.x, (int)gameObject.LocalPosition.y, endX, endY, time, interpolatior) { }


        public MoveToAction(GameObject gameObject,int startX,int startY,int endX,int endY,float time,IInterpolator interpolatior) : base(0, time, interpolatior,gameObject)
        {
            this.endX = endX;
            this.endY = endY;

            this.startX = startX;
            this.startY = startY;
        }

        public override void onUpdate(float norValue)
        {
            if (norValue >= 1)
            {
                markDone();
            }

            float x = norValue * (endX - startX);
            float y = norValue * (endY - startY);
            //Console.WriteLine("time : {0:F2}\tx : {1:F2}\ty :　{2:F2}",passTime,x,y);

            gameObject.LocalPosition=new Vector(x+startX,y+startY);
        }

        public override ActionBase reverse()
        {
            return new MoveToAction(gameObject,endX,endY,startX,startY,_timeEnd-_timeStart,interpolator);
        }

    }
}
