using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace OpenGLF
{
    public static class Time
    {
        public static long getMsTick()
        {
            return Environment.TickCount;
        }
    }
}
