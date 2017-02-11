using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenGLF
{
    public interface IInterpolator
    {
        float start { get; set; }
        float end { get; set; }

        float calculate(float value);

        IInterpolator reverse();
    }
}
