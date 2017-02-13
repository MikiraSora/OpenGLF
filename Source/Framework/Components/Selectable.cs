using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenGLF
{
    public class Selectable:Component
    {
        Shader currentShader = null;
        Shader saveShader = null;
        int hashSave = 0;

        CALLBACKTYPE _type;

        public CALLBACKTYPE Type
        {
            get { return _type;}
            set { _type = value; }
        }
        
        [Flags]
        public enum CALLBACKTYPE
        {
            MOVEAREA=1,
            OPAREA=1<<2
        } 

        public Selectable(CALLBACKTYPE type=CALLBACKTYPE.MOVEAREA|CALLBACKTYPE.OPAREA)
        {
            _type = type;
        }

        public override void attach()
        {
            base.attach();
            SelectManager.registerSelectObject(gameObject);
            Log.Debug("Selectable Object is attached !");
        }

        public override void detach()
        {
            base.detach();
            SelectManager.unregisterSelectObject(gameObject);
            Log.Debug("Selectable Object is detached !");
        }

        public virtual void beforeSelect(ref List<GameObject> selectList, int x, int y)
        {
            if (hashSave != gameObject.sprite.material.shader.GetHashCode())
            {
                Log.Debug("build new shader");
                if (currentShader != null)
                    currentShader.Dispose();
                buildShader();
            }
            saveShader = gameObject.sprite.material.shader;
            gameObject.sprite.material.shader = currentShader;

            byte[] codeArr = ByteConverter.intToBytes(gameObject.ID);//int to byte[4]

            gameObject.sprite.material.parameters["code"] = new Vec4(codeArr[0], codeArr[1], codeArr[2],codeArr[3]);

            hashSave = saveShader.GetHashCode();
        }

        public void buildShader()
        {
            var vertShader = gameObject.sprite.material.shader.vertexProgram;
            var fragShader = Engine.shaders.selectFragShader;
            currentShader = new Shader();
            currentShader.fragmentProgram = fragShader;
            currentShader.vertexProgram = vertShader;
            currentShader.compile();
        }

        public virtual void afterSelect()
        {
            gameObject.sprite.material.shader = saveShader;
        }

        public virtual void enterArea()
        {
            onEnterArea();
        }

        public virtual void leaveArea()
        {
            onLeaveArea();
        }

        public virtual void clickArea()
        {
            onClickArea();
        }

        public virtual void dragArea()
        {
            onDragArea();
        }

        public virtual void repeatArea()
        {
            onRepeatArea();
        }

        public delegate void EnterArea();
        public delegate void LeaveArea();
        public delegate void ClickArea();
        public delegate void DragArea();
        public delegate void RepeatArea();

        public event EnterArea onEnterArea;
        public event LeaveArea onLeaveArea;
        public event ClickArea onClickArea;
        public event DragArea onDragArea;
        public event RepeatArea onRepeatArea;
    }
}
