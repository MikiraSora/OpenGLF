using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGLF;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace OpenGLF_EX
{
    public class Button:GameObject
    {
        GameObject _textGameObject;
        GameObject _backgroundGameObject=null;

        int _width, _height;

        public Button(string text,int size,Color color,int width,int height,Font font,Texture background=null)
        {
            _width = width;
            _height = height;

            if (background != null)
            {
                _backgroundGameObject = new GameObject();
                _backgroundGameObject.components.Add(new TextureSprite(background));
                _backgroundGameObject.sprite.width = width;
                _backgroundGameObject.sprite.height = height;
                _backgroundGameObject.sprite.center = new Vector(width / 2, height / 2);
                _backgroundGameObject.LocalPosition = new Vector(0, 0);

                this.addChild(_backgroundGameObject);
            }

            
            _textGameObject = new GameObject();
            _textGameObject.components.Add(new TextSprite(text,size,color,font));
            /*
            _textGameObject.sprite.width = width;
            _textGameObject.sprite.height = height;
            */
            _textGameObject.sprite.center = new Vector(_textGameObject.sprite.width / 2, _textGameObject.sprite.height / 2);
            
            
            this.addChild(_textGameObject);
            _textGameObject.LocalPosition = new Vector(width / 2, height / 2);


            var sprite = new TextureSprite();
            Shader shader = new Shader();
            shader.vertexProgram = _textGameObject.sprite.material.shader.vertexProgram;
            shader.fragmentProgram = _textGameObject.sprite.material.shader.fragmentProgram;
            shader.compile();
            sprite.material.shader = shader;

            this.components.Add(sprite);
            sprite.width = width;
            sprite.height = height;


            components.Add(new Selectable(Selectable.CALLBACKTYPE.OPAREA));
            _textGameObject.components.Add(new Selectable(Selectable.CALLBACKTYPE.OPAREA));

            getComponent<Selectable>().onDragArea += (e)=> { onDrag(e); };
            _textGameObject.getComponent<Selectable>().onDragArea += (e) => { onDrag(e); };
        }

        public override void draw(RenderingMode mode)
        {
            base.draw(mode);
        }

        public delegate void OnButtonClickFunc(MouseEventArgs e);
        public event OnButtonClickFunc onClick;

        public delegate void OnButtonDragkFunc(MouseMoveEventArgs e);
        public event OnButtonClickFunc onDrag;
    }
}
