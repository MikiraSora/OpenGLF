using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGLF;

namespace OpenGLF_EX
{
    public abstract class AnimationBase
    {
        protected Sprite _sprite = null;
        public Sprite Sprite
        {
            get { return _sprite; }
            private set { _sprite = value; }
        }

        protected bool _isDone = false;
        public bool Done
        {
            get { return _isDone; }
            private set { _isDone = value; }
        }

        public AnimationBase() {}

        public abstract void onAction(float passTime);
    }
}
