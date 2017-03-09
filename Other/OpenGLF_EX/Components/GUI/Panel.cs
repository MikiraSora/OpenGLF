using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenGLF;

namespace OpenGLF_EX
{
    class Panel : WidgetObject
    {
        public void addWidget(WidgetObject widget)
        {
            this.addChild(widget);
        }

        public void removeWidget(WidgetObject widget)
        {
            this.removeChild(widget);
        }
    }
}
