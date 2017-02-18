﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Design;
using System.ComponentModel;
using System.ComponentModel.Design;

namespace OpenGLF
{
    [Serializable]
    [TypeConverter(typeof(SceneEditor))]
    public class Scene : Asset
    {
        internal int objCount = 0;
        //public GameObjectList objects;
        protected GameObject gameObjectRoot;
        public Camera mainCamera;
        public Dictionary<string, object> data;

        public GameObject GameObjectRoot { private set { } get { return gameObjectRoot; } }

        public Scene(){
            name = "Default Scene";
            gameObjectRoot = new GameObject();
            gameObjectRoot.name = "root";
            data = new Dictionary<string, object>();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        public override string ToString()
        {
            return name;
        }

        public override void Loaded()
        {
            foreach (GameObject obj in gameObjectRoot.getAllChildren())
            {
                if (obj.ID > objCount)
                    objCount = obj.ID;
            }

            foreach (GameObject o in gameObjectRoot.getAllChildren())
            {
                foreach (Component c in o.components)
                {
                    c.Loaded();
                }
            }
        }

        //SceneDirector callback
        //此Scene正式成为当前Scene前最后的callback,通常用于显示物件之类即时东西
        public virtual void onEnterScene(){}

        //结束此Scene后第一个callBack,通常用于释放资源操作
        public virtual void onLeaveScene() {}

        public void exitScene()
        {
            SceneDirector.exitCurrentScene(); //自我退出
        }

        [OnDeserialized()]
        private void onDeserialized(StreamingContext context)
        {
            
        }
    }

    public class SceneEditor : TypeConverter
    {
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return true;
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            List<string> names = new List<string>();
            names.Add("");

            for (int i = 0; i < Assets.items.Count; i++)
            {
                if (Assets.items[i] is Scene)
                    names.Add(Assets.items[i].name);
            }

            return new StandardValuesCollection(names);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }

            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            List<Scene> names = new List<Scene>();
            for (int i = 0; i < Assets.items.Count; i++)
            {
                if (Assets.items[i] is Scene)
                    names.Add((Scene)Assets.items[i]);
            }

            if (value is string)
            {
                foreach (Scene s in names)
                {
                    if (s.name == (string)value)
                    {
                        return s;
                    }
                }

                if (String.IsNullOrEmpty((string)value))
                    return null;
            }

            return base.ConvertFrom(context, culture, value);
        }
    }
}
