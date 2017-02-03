﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Dynamic;

namespace OpenGLF
{
    [Serializable]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class Component
    {
        bool _enabled = true;
        [Browsable(false)]
        public GameObject gameObject { get; internal set; }
        [Category("Properties")]
        public bool enabled { get { return _enabled; } set { _enabled = value; } }
        public virtual void start() { }
        public virtual void update() { }
        public virtual void draw(RenderingMode mode) { }
        
        public virtual void attach()
        {

        }

        public virtual void detach()
        {

        }

        public Component()
        {

        }

        public virtual Component clone()
        {
            return new Component();
        }

        internal virtual bool multiple()
        {
            return false;
        }

        public override string ToString()
        {
            return GetType().Name;
        }

        public virtual void Loaded() { }

        [OnDeserializedAttribute()]
        private void onDeserialized(StreamingContext context)
        {
            
        }

    }
}
