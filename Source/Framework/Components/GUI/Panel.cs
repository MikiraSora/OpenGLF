using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenGLF.Components.GUI
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
