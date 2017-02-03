using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenGLF
{
    public class Shaders
    {
        Shader _defaultShader = null;
        public Shader defaultShader { get { return _defaultShader; } protected set { _defaultShader = value; } }
        public Shader guiShader { get; protected set; }

        public string selectFragShader =
            "uniform vec4 code;"+
            "void main()" +
            "{" +
            "gl_FragColor=vec4(code.r/255.0,code.g/255.0,code.b/255.0,code.a/255.0);" +
            "}";

        public Shaders()
        {
            _defaultShader = new Shader();
            _defaultShader.compile();

            guiShader = new Shader();

            guiShader.vertexProgram =
                        "void main(void)" +
                        "{" +
                            "gl_TexCoord[0] = gl_TextureMatrix[0] * gl_MultiTexCoord0;" +
                            "gl_Position = ftransform();" +
                        "}";

            guiShader.fragmentProgram =
                        "uniform sampler2D texture;" +
                        "uniform vec4 color;" +
                        "void main(void)" +
                        "{" +
                            "vec4 col = texture2D(texture, gl_TexCoord[0].st);" +
                            "gl_FragColor = col * color;" +
                        "}";

            guiShader.compile();
        }
    }
}
