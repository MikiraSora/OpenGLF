using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK.Graphics.OpenGL;
using System.Runtime.Serialization;
using System.Reflection;
using System.Dynamic;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace OpenGLF
{
    [Serializable]
    [TypeConverter(typeof(GameObjectConverter))]
    public class GameObject : IDisposable
    {
        bool disposed = false;
        Vector _position = Vector.zero;
        Vector _localPosition = Vector.zero;
        Vector _oldpos = Vector.zero;
        float _angle = 0;
        float _localAngle = 0;
        string _name = "";
        int _id = 0;
        GameObject _parent;
        GameObjectList _children = new GameObjectList();
        public Sprite sprite;
        public Camera camera;
        public Rigidbody rigidbody;

        Vector _localScale=new Vector(1,1), _absoluteScale=new Vector(1,1);

        public Vector Scale { get { return _absoluteScale; } }

        public Vector LocalScale
        {
            get { return _localScale; }
            set
            {
                _localScale = value;
                updateAbsoluteScale();
            }
        }

        void updateAbsoluteScale()
        {
            _absoluteScale = (_parent == null ? new Vector(1,1) : _parent._absoluteScale) * _localScale;
            foreach (var child in _children)
                child.updateAbsoluteScale();

            if (sprite != null)
                sprite.scale = _absoluteScale;
        }

        static int gen_ID =0;

        public delegate bool ForeachCallFunc(GameObject obj, object state = null);
        
        [Category("Transform")]
        public Vector LocalPosition
        {
            get
            {
                return _localPosition;
            }

            set
            {
                _localPosition = value;
                updateWorldPosition();
            }
        }

        [Category("Transform")]
        public Vector WorldPosition
        {
            get { return _position; }
            set
            {
                Vector offset_postion=value- (_parent == null ? Vector.zero : _parent.WorldPosition);
                LocalPosition = offset_postion;
            }
        }

        internal void updateWorldPosition()
        {
            _position = (_parent == null ? Vector.zero : _parent.WorldPosition) + _localPosition;

            if (rigidbody != null)
            {
                rigidbody.position = _position;
            }

            foreach (var child in _children)
                child.updateWorldPosition();
        }

        [Category("Transform")]
        public float Angle {
            get
            {
                return _angle;
            }
            set
            {
                /*
                if (camera == null)
                {
                    List<GameObject> list = getChildren();

                    
                    float _old_angle = _angle;
                    _angle = Mathf.roundDegrees(value);

                    float _new_angle = _angle - _old_angle;

                    for (int i = 0; i < list.Count; i++)
                    {
                        GameObject child = list[i];
                        if (child.camera == null)
                        {
                            child.WorldPosition = Mathf.rotateAround(child.WorldPosition, WorldPosition, (float)Mathf.toRadians(_new_angle));
                            child._angle = child._angle + _new_angle;
                        }
                    }
                    

                    _angle = Mathf.roundDegrees(value);
                    
                }
                */
            }
        }

        public float LocalAngle
        {
            get { return _localAngle; }
            set
            {
                _localAngle = value;
                updateAbsoluteAngle();
            }
        }

        internal void updateAbsoluteAngle()
        {
            _angle = (_parent == null ? 0 : _parent.Angle) + _localAngle;

            foreach (var child in _children)
                child.updateAbsoluteAngle();

            if (rigidbody != null)
            {
                rigidbody.rotation = (float)Mathf.toRadians(_angle);
            }
        }

        [Category("Properties")]
        public string name {
            get 
            {
                return _name; 
            }
            set 
            {
                Regex regex = new Regex(@"\d+");
                string val = regex.Replace(value, "");

                if (_children.Contains(GameObject.find(value)))
                    _name = val + ID;
                else
                    _name = value;

                if (String.IsNullOrEmpty(_name))
                    name = "Unnamed object";
            }
        }

        [Category("Properties")]
        public string tag { get; set; }

        [Browsable(false)]
        public GameObject parent
        {
            get
            {
                return _parent;
            }
            set
            {
                if (_parent != null)
                {
                    _parent.GameObjectChildren.Remove(this);
                }
                _parent = value;

                if(!_parent.GameObjectChildren.Contains(this))
                    _parent.GameObjectChildren.Add(this);

                updateWorldPosition();
                updateAbsoluteAngle();
                updateAbsoluteScale();
            }
        }

        public int ID { get { return _id; } }

        [Category("Components")]
        public ComponentList components {get; set;}

        [Category("GameObject")]
        public GameObjectList GameObjectChildren {
            protected set {
                _children = value;
            }
            get {
                return _children;
            }
        }

        [Category("Properties")]
        public int depth
        {
            get
            {
                return _parent == null ? 0 : _parent._children.IndexOf(this);
            }
            set
            {
                setDepth(value);
            }
        }

        void setDepth(int d)
        {
            try
            {
                var max = _children.Count;
                d = max > d ? max : d;
                _parent.removeChild(this);
                _parent.addChild(this, d);
            }
            catch
            {
                Log.Error("Ошибка изменения глубины");
            }
        }

        public int FullDepth
        {
            get
            {
                int result = 0;
                result = depth + (_parent == null ? 0 : _parent.FullDepth);
                return result+1;
            }
        }

        public GameObject()
        {
            defaultConstructor();
        }

        public GameObject(Prefab prefab)
        {
            defaultConstructor();

            try
            {
                name = prefab.name;
                _angle = prefab.gameObject._angle;

                for (int i = 0; i < prefab.gameObject.components.Count; i++)
                {
                    Component c = prefab.gameObject.components[i].clone();
                    components.Add(c);
                }
            }
            catch
            {
                Log.Error("Ошибка копирования данных");
            }
        }

        void defaultConstructor()
        {
            _position = new Vector(0, 0);
            components = new ComponentList(this);

            name = "Game Object";
            _id = gen_ID++;

            createInstances();
            start();
        }

        public void addChild(GameObject obj,int position=-1)
        {
            obj.parent = this;
            position = position<0? _children.Count:position > _children.Count ? _children.Count : position;
            _children.Insert(position,obj);
        }

        public void removeChild(GameObject obj)
        {
            _children.Remove(obj);
            obj._parent = null;
        }

        public static GameObject getRoot(GameObject obj)
        {
            GameObject root = null;

            if (obj.parent != null)
            {
                GameObject o = obj.parent;
                root = o;

                while (o != null)
                {
                    o = o.parent;
                    if (o != null)
                        root = o;
                }

                return root;
            }

            return null;
        }

        static GameObject getParent(GameObject obj)
        {
            return obj.parent;
        }

        public virtual GameObject clone()
        {
            GameObject obj = new GameObject();
            obj.name = name;      
            obj._angle = _angle;
            obj.tag = tag;
            obj.parent = parent;
            obj._localPosition = _localPosition.clone();

            for (int i = 0; i < components.Count; i++)
            {
                Component c = components[i].clone();
                obj.components.Add(c);
            }

            setComponents();
            /*
            for (int i = 0; i < Engine.scene.gameObjectRoot.getAllChildren().Count; i++)
            {
                if (Engine.scene.gameObjectRoot.getAllChildren()[i].parent == this)
                {
                    GameObject clone = Engine.scene.gameObjectRoot.getAllChildren()[i].clone();
                    clone.parent = obj;
                }
            }
            */
            for (int i = 0; i < _children.Count; i++)
            {
                if (_children[i].parent == this)
                {
                    GameObject clone = _children[i].clone();
                    clone.parent = obj;
                }
            }
            return obj;
        }

        public T getComponent<T>(int position=0) where T : Component
        {
            Component c = null;

            for (int i = 0; i < components.Count; i++)
            {
                if (components[i] is T)
                {
                    c = components[i];
                    position--;
                    if (position < 0)
                        return (T)c;
                }
                else if (components[i] is Behavior)
                {
                    if (((Behavior)components[i]).instance != null)
                    {
                        if (((Behavior)components[i]).instance is T)
                        {
                            c = ((Behavior)components[i]).instance;
                            position--;
                            if (position < 0)
                                return (T)c;
                        }
                    }
                }
            }

            //result maybe is null ref or last find;
            return (T)c; 
        }

        internal void createInstances()
        {
            for (int i = 0; i < components.Count; i++)
            {
                if (components[i] is Behavior)
                {
                    Behavior beh = (Behavior)components[i];
                    beh.createInstance(this);
                }
            }

            foreach (var child in _children)
                child.createInstances();
        }

        public void start()
        {
            for (int j = 0; j < components.Count; j++)
            {
                if (components[j].enabled)
                    components[j].start();
            }

            for (int i = 0; i < components.Count; i++)
            {

                if (components[i] is Behavior)
                {
                    Behavior beh = (Behavior)components[i];

                    if (beh.enabled)
                    {
                        if (beh.isMethodExists("start"))
                            beh.call("start");
                    }
                }
                else if (components[i] is BlueprintBehavior)
                {
                    BlueprintBehavior beh = (BlueprintBehavior)components[i];

                    if (beh.blueprint != null && beh.enabled)
                    {
                        beh.blueprint.start();
                    }
                }
            }

            foreach (var child in _children)
                child.start();
        }

        public virtual void stopAnimations()
        {
            foreach(var i in components)
            {
                if (i is Sprite)
                    ((Sprite)i).stop();
            }
        }

        public virtual void update()
        {
            for (int j = 0; j < components.Count; j++)
            {
                if (components[j].enabled)
                    components[j].update();
            }

            for (int i = 0; i < components.Count; i++)
            {
                if (components[i] is Behavior)
                {
                    Behavior beh = (Behavior)components[i];

                    if (beh.enabled)
                    {
                        if (beh.isMethodExists("update"))
                            beh.call("update");
                    }
                }

                else if (components[i] is BlueprintBehavior)
                {
                    BlueprintBehavior beh = (BlueprintBehavior)components[i];

                    if (beh.blueprint != null && beh.enabled)
                    {
                        beh.blueprint.update();
                    }
                }
            }

            foreach (var child in _children)
                child.update();
        }

        public virtual void beforeDraw()
        {
            /*
            foreach (var child in _children)
                child.beforeDraw();
                */
        }

        public virtual void prepareDraw()
        {
            //todo
            /*
             Move beforeDraw() codes to THIS METHOD because it may want Post-Process for one gameObject.
             */

            for (int i = 0; i < components.Count; i++)
            {
                if (components[i] is Behavior)
                {
                    Behavior beh = (Behavior)components[i];

                    if (beh.enabled)
                    {
                        if (beh.isMethodExists("beforeDraw"))
                            beh.call("beforeDraw");
                    }
                }
                else if (components[i] is BlueprintBehavior)
                {
                    BlueprintBehavior beh = (BlueprintBehavior)components[i];

                    if (beh.blueprint != null && beh.enabled)
                    {
                        beh.blueprint.beforeDraw();
                    }
                }
            }

            foreach (var child in _children)
                child.prepareDraw();
        }

        public virtual void finDraw()
        {
            //todo
            /*
             Move afterDraw() codes to THIS METHOD because it(afterDraw()) may want Post-Process for one gameObject.
             */

            for (int i = 0; i < components.Count; i++)
            {
                if (components[i] is Behavior)
                {
                    Behavior beh = (Behavior)components[i];

                    if (beh.enabled)
                    {
                        if (beh.isMethodExists("afterDraw"))
                            beh.call("afterDraw");
                    }
                }
                else if (components[i] is BlueprintBehavior)
                {
                    BlueprintBehavior beh = (BlueprintBehavior)components[i];

                    if (beh.blueprint != null && beh.enabled)
                    {
                        beh.blueprint.afterDraw();
                    }
                }
            }

            foreach (var child in _children)
                child.finDraw();
        }

        public virtual void ForeachCall(ForeachCallFunc func,object state=null)
        {
            if (func(this,state))
                return;
            foreach (var child in _children)
                child.ForeachCall(func, state);
        }

        public virtual void draw(RenderingMode mode)
        {

            beforeDraw();
            foreach (var c in components)
                if (((Component)c).enabled)
                    ((Component)c).draw(mode);

            foreach (var child in _children)
                child.draw(mode);
            afterDraw();
        }

        public virtual void afterDraw()
        {
            /*
            foreach (var child in _children)
                child.afterDraw();
            */
        }

        public void RemoveChildGameObject(GameObject gameObject)
        {
            if (_children.Contains(gameObject))
            {
                _children.Remove(gameObject);
                gameObject.parent = null;
            }
            else
            {
                foreach(var obj in _children)
                {
                    obj.RemoveChildGameObject(gameObject);
                }
            }
        }

        public void InsertChildGameObject(GameObject gameObject,int position=-1)
        {
            if (_children.Contains(gameObject))
                return;
            if (position < 0)
                _children.Add(gameObject);
            else
                _children.Insert(position, gameObject);
            gameObject.parent = this;
        }

        public List<GameObject> getAllChildren()
        {
            List<GameObject> list = new List<GameObject>();
            _getAllChildren(ref list);
            return list;
        }

        public List<GameObject> getChildren()
        {
            /*
            if (Engine.scene != null)
            {
                List<GameObject> children = new List<GameObject>();

                for (int i = 0; i < Engine.scene.gameObjectRoot.getAllChildren().Count; i++)
                {
                    GameObject obj = Engine.scene.gameObjectRoot.getAllChildren()[i];
                    if (obj._parent == this)
                        children.Add(obj);
                }

                return children;
            }
            else return null;
            */
            return _children;
        }

        void _getAllChildren(ref List<GameObject> children)
        {
            /*
            if (Engine.scene != null)
            {
                for (int i = 0; i < Engine.scene.gameObjectRoot.getAllChildren().Count; i++)
                {
                    GameObject obj = Engine.scene.gameObjectRoot.getAllChildren()[i];
                    if (obj._parent == this)
                    {
                        children.Add(obj);
                        obj._getAllChildren(children);
                    }
                }
            }
            */
            foreach (var obj in _children)
            {
                children.Add(obj);
                obj._getAllChildren(ref children);
            }

        }

        public GameObject findId(int id)
        {
            GameObject result;
            if(this.ID != id)
            {
                foreach(var obj in _children)
                {
                    result = obj.findId(id);
                    if (result != null)
                        return result;
                }
                return null;
            }
            return this;
        }

        public GameObject findName(string name)
        {
            GameObject result;
            if (this.name != name)
            {
                foreach (var obj in _children)
                {
                    result = obj.findName(name);
                    if (result != null)
                        return result;
                }
                return null;
            }
            return this;
        }

        public void findTag(string tag, ref GameObjectList resultList)
        {
            if (this.tag == tag)
            {
                resultList.Add(this);
            }

            foreach(var obj in _children)
            {
                findTag(tag, ref resultList);
            }
        }

        public static GameObject find(int id)
        {
            /*
            GameObject o = null;    
            if (Engine.scene != null)
            {
                for (int i = 0; i < Engine.scene.gameObjectRoot.getAllChildren().Count; i++)
                {
                    if (Engine.scene.gameObjectRoot.getAllChildren()[i] != null)
                    {
                        if (Engine.scene.gameObjectRoot.getAllChildren()[i].ID == id)
                            o = Engine.scene.gameObjectRoot.getAllChildren()[i];
                    }
                }
            }
            return o;
            */
            return Engine.scene.GameObjectRoot.findId(id);
        }

        public static GameObject find(string id)
        {
            /*
            GameObject o = null;

            if (Engine.scene != null)
            {
                for (int i = 0; i < Engine.scene.gameObjectRoot.getAllChildren().Count; i++)
                {
                    if (Engine.scene.gameObjectRoot.getAllChildren()[i] != null)
                    {
                        if (Engine.scene.gameObjectRoot.getAllChildren()[i].name == id)
                            o = Engine.scene.gameObjectRoot.getAllChildren()[i];
                    }
                }
            }
            return o;   
            */
            return Engine.scene!=null?Engine.scene.GameObjectRoot.findName(id):null;
        }

        public static void findWithTag(string tag,ref GameObjectList resultList)
        {
            /*
            GameObject[] o = null;

            if (Engine.scene != null)
            {
                o = Engine.scene.gameObjectRoot.getAllChildren().FindAll(obj => obj.tag == tag).ToArray();
            }

            return o;
            */

            Engine.scene.GameObjectRoot.findTag(tag, ref resultList);
        }

        ~GameObject(){
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    /*
                    if (Engine.scene != null)
                    {
                        if (Engine.scene.gameObjectRoot.getAllChildren( ).Contains(this))
                            Engine.scene.gameObjectRoot.getAllChildren().Remove(this);
                    }*/

                    if (this.parent == null)
                        throw new Exception("Not allow to dispose root GameObject");

                    this.parent._children.Remove(this);

                    foreach (var obj in _children)
                    {
                        obj.Dispose();
                    }
                    
                }

                disposed = true;
            }
        }

        public override string ToString()
        {
            return name;
        }

        internal void setComponents()
        {
            sprite = getComponent<Sprite>();
            if(sprite!=null)
                sprite.scale = _absoluteScale;
            camera = getComponent<Camera>();
            rigidbody = getComponent<Rigidbody>();
        }

        [OnDeserializedAttribute()]
        private void onDeserialized(StreamingContext context)
        {
            setComponents();
        }
    }

    public class GameObjectConverter : TypeConverter
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

            var list = Engine.scene.GameObjectRoot.getAllChildren();

            for (int i = 0; i < list.Count; i++)
            {
                names.Add(list[i].name);
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
            if (value is string)
            {
                foreach (GameObject s in Engine.scene.GameObjectRoot.getAllChildren())
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
