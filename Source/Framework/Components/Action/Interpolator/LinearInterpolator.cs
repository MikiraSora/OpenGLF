using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenGLF
{
    public class LinearInterpolator : IInterpolator
    {
        float _end, _start;
        public float end { get { return _end; } set { _end = value; } }

        public float start { get { return _start; } set { _start = value; } }

        public float calculate(float value)
        {
            if (value > end)
                return 1;
            if (value < start)
                return 0;

            return value/(end-start);
        }

        public IInterpolator reverse()
        {
            return new LinearInterpolator();
        }
    }
}
