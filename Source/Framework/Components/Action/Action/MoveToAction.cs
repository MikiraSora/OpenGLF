using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenGLF
{
    public class MoveToAction : ActionBase
    {
        int endX, endY,startX,startY;
        float time = 0;
        float angle;

        public MoveToAction(GameObject gameObject,int startX,int startY,int endX,int endY,float time,IInterpolator interpolatior) : base(0, time, interpolatior,gameObject)
        {
            this.endX = endX;
            this.endY = endY;

            this.startX = startX;
            this.startY = startY;

            this.time = time;

            interpolatior.start = 0;
            interpolatior.end = time;
        }

        public override void onStart()
        {
            //recordPos = gameObject.LocalPosition;
            Console.WriteLine("local position={0},{1}", startX, startY);
        }

        public override void onUpdate(float passTime)
        {
            
            float x = passTime * (endX - startX);
            float y = passTime * (endY - startY);
            Console.WriteLine("time : {0:F2}\tx : {1:F2}\ty :　{2:F2}",passTime,x,y);

            gameObject.LocalPosition=new Vector(/*passTime * (endX - startX)*/x+startX, /*passTime * (endY - startY)*/y+startY);

            if (passTime >= 1)
            {
                markDone();
                Console.WriteLine("Done");
            }
        }

        public override ActionBase reverse()
        {
            return new MoveToAction(gameObject,endX,endY,startX,startY,time,interpolator);
        }

    }
}
