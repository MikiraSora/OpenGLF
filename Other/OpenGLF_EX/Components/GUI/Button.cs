using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK.Graphics.OpenGL;
using System.Runtime.InteropServices;
using OpenGLF;

namespace OpenGLF_EX
{
    public class Button : WidgetObject
    {
        //底色
        public Color Color { get; set; }

        //文字颜色
        Color _textColor = new Color(255, 255, 255, 255);
        public Color TextColor { get { return _textColor; } set { _textColor = value; updateTextTexture(); } }

        //文字
        string _text;
        public string Text { get { return _text; } set { _text = value; updateTextTexture(); } }

        Vector _center = new Vector();
        public Vector Center { get { return _center; } set {
                _center = value;
                vboUpdate();
            } }

        Material material;

        Vec4 debugColor = new Vec4((float)OpenGLF.Random.range(0, 1), (float)OpenGLF.Random.range(0, 1), (float)OpenGLF.Random.range(0, 1), 0.35f);
        Dictionary<string, object> debugParamters = new Dictionary<string, object>();

        //边框
        Vector _bound = new Vector();
        public Vector Bound { get { return _bound; } set { _bound = value; vboUpdate(); } }
        public float Width { get { return Bound.x; } set { _bound.x = value; vboUpdate(); } }
        public float Height { get { return Bound.y; } set { _bound.y = value; vboUpdate(); } }

        float _fontSize = 15;
        public float TextSize { get { return _fontSize; } set { _fontSize = value; } }

        Font _font = null;
        public Font TextFont { get { return _font; } set { _font = value; updateTextTexture(); } }

        Vector[] vertexBuf, textureBuf;
        int vId, tId;

        Texture _textTexture=null,_backgroundTexture=null;
        public Texture TextTexture
        {
            get { return _textTexture; }
        }

        public Texture BackgroundTexture
        {
            get { return _backgroundTexture; }
            set { _backgroundTexture = value; }
        }

        void buildMaterial()
        {
            material = new Material();
            material.shader = new Shader();
            material.shader.vertexProgram = 
                "void main(void)" +
                "{" +
                    "gl_TexCoord[0] = gl_TextureMatrix[0] * gl_MultiTexCoord0;" +
                    "gl_Position = ftransform();" +
                "}";
            // "//uniform sampler2D background;" +
            //"//vec4 color2 = texture2D(background, gl_TexCoord[0].st);" +
            material.shader.fragmentProgram = 
                "uniform sampler2D text;" +
                "uniform sampler2D background;"+
                "uniform vec4 colorkey;" +
                "void main(void)" +
                "{" +
                    "vec4 color1 = texture2D(text, gl_TexCoord[0].st);" +
                    "vec4 color2 = texture2D(background, gl_TexCoord[0].st);" +
                    "gl_FragColor = color1+color2*colorkey;" +
                "}";

            material.shader.compile();
        }

        void updateTextTexture()
        {
            var real_size = TextFont.calculateSize(Text,(int)TextSize,(int)Width);

            if (this._textTexture != null)
            {
                this._textTexture.bitmap.Dispose();
                this._textTexture.Dispose();
            }

            this._textTexture = TextFont.GenTexture(Text,(int)Width, (int)Height,(int) TextSize, TextColor);
        }

        public Button(string text,float width,float height,float size,Font font)
        {
            Color = new Color(255, 255, 255, 255);

            buildMaterial();
            genBuffers();

            Bound = new Vector(width, height);

            _fontSize = size;
            _text = text;
            _font = font;
            updateTextTexture();

            Center = Vector.zero; 

        }

        private void genBuffers()
        {
            vertexBuf = new Vector[4];
            //normalsBuf = new Vector[4];
            textureBuf = new Vector[4];

            vertexBuf[0] = new Vector(0, 0);
            vertexBuf[1] = new Vector(Width, 0);
            vertexBuf[2] = new Vector(Width, Height);
            vertexBuf[3] = new Vector(0, Height);

            textureBuf[0] = new Vector(0, 0);
            textureBuf[1] = new Vector(1, 0);
            textureBuf[2] = new Vector(1, 1);
            textureBuf[3] = new Vector(0, 1);

            //Verts
            vId = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vId);
            GL.BufferData<Vector>(BufferTarget.ArrayBuffer, new IntPtr(Marshal.SizeOf(new Vector()) * 2 * vertexBuf.Length), vertexBuf, BufferUsageHint.StreamDraw);
            GL.BufferSubData<Vector>(BufferTarget.ArrayBuffer, new IntPtr(0), new IntPtr(Marshal.SizeOf(new Vector()) * 2 * vertexBuf.Length), vertexBuf);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            //Textures
            tId = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, tId);
            GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(sizeof(float) * 2 * textureBuf.Length), textureBuf, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        private void vboUpdate()
        {
            vertexBuf[0].x = -Center.x; vertexBuf[0].y = -Center.y;
            vertexBuf[1].x = Width - Center.x; vertexBuf[1].y = -Center.y;
            vertexBuf[2].x = Width - Center.x; vertexBuf[2].y = Height - Center.y;
            vertexBuf[3].x = -Center.x; vertexBuf[3].y = Height - Center.y;

            GL.BindBuffer(BufferTarget.ArrayBuffer, vId);
            GL.BufferSubData<Vector>(BufferTarget.ArrayBuffer, new IntPtr(0), new IntPtr(Marshal.SizeOf(new Vector()) * 2 * vertexBuf.Length), vertexBuf);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vId);
        }

        public override void draw(OpenTK.Graphics.OpenGL.RenderingMode mode)
        {
            if (material == null || material.shader == null)
                return;

            GL.PushMatrix();

            material.shader.begin();

            //pass background image to shader program
            if (_backgroundTexture != null)
                material.shader.pass("background", _backgroundTexture);
            else
                material.shader.passNullTex("background");
            
            if (_textTexture != null)
                material.shader.pass("text", _textTexture);
            else
                material.shader.passNullTex("text");
                
            material.shader.pass("colorkey", (Vec4)Color);

            GL.Translate(WorldPosition.x, WorldPosition.y, 0);
            GL.Rotate(Angle, 0, 0, 1);
            GL.Scale(Scale.x, Scale.y, 1);

            //Биндим текстурные координаты
            GL.EnableClientState(ArrayCap.TextureCoordArray);
            GL.BindBuffer(BufferTarget.ArrayBuffer, tId);
            GL.TexCoordPointer(2, TexCoordPointerType.Float, 0, 0);

            //Биндим геометрию
            GL.EnableClientState(ArrayCap.VertexArray);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vId);
            GL.VertexPointer(2, VertexPointerType.Float, 0, 0);

            //Рисуем
            GL.DrawArrays(PrimitiveType.Quads, 0, vertexBuf.Length);

            if (Engine.debugGameObject && !SelectManager.isSelecting)
            {
                var SaveShader = material.shader;

                //load debugShader
                material.shader = Engine.shaders.debugGameObjectShader;

                object SaveColor = null;

                if (material.parameters.ContainsKey("colorkey"))
                    SaveColor = material.parameters["colorkey"];

                material.shader.begin();
                material.parameters["colorkey"] = debugColor;

                material.shader.pass("colorkey", (Vec4)material.parameters["colorkey"]);
                GL.DrawArrays(PrimitiveType.Quads, 0, vertexBuf.Length);

                if (material.parameters.ContainsKey("colorkey"))
                    material.parameters["colorkey"] = SaveColor;

                material.shader = SaveShader;
            }

            //unload texture
            material.shader.passNullTex("background");
            material.shader.passNullTex("text");

            material.shader.end();

            GL.PopMatrix();

            //Отключаем все
            GL.DisableClientState(ArrayCap.VertexArray);
            GL.DisableClientState(ArrayCap.TextureCoordArray);

            //
            //Drawing.drawText(WorldPosition, Center, Scale, (int)Angle, (int)Width, (int)Height, Text, TextColor, (int)TextSize, TextFont);
        }
    }
}
