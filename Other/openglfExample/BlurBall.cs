using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGLF;
using OpenTK.Graphics.OpenGL;
using OpenGLF_EX;

namespace openglfExample
{
    class BlurBall : GameObject
    {
        #region BlurFrameBufferWrapper
        public class BlurFrameBufferWrapper
        {
            RawTexture texture = null;
            int _framebufferId = 0;

            public int FrameBufferId { private set { } get { return _framebufferId; } }
            public RawTexture Texture { private set { } get { return texture; } }


            public BlurFrameBufferWrapper(int width, int height)
            {
                //创建纹理
                var texId = GL.GenTexture();
                GL.BindTexture(TextureTarget.Texture2D, texId);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Clamp);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Clamp);

                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, width, height, 0,
                        PixelFormat.Bgra, PixelType.UnsignedByte, (IntPtr)0);
                GL.BindTexture(TextureTarget.Texture2D, 0);

                //创建FBO
                _framebufferId = GL.GenFramebuffer();
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, _framebufferId);
                //绑定纹理到FBO
                GL.FramebufferTexture(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, texId, 0);
                //检查
                var isSuccess = GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer);
                if (isSuccess != FramebufferErrorCode.FramebufferComplete || !GL.IsFramebuffer(_framebufferId))
                {
                    throw new Exception("Create frameBuffer Failed! Code name=" + isSuccess.ToString());
                }
                //解绑FBO
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

                texture = new RawTexture(texId);
            }
        }
        #endregion

        private BlurFrameBufferWrapper[] frames;
        public BlurFrameBufferWrapper[] Frames { get { return frames; } private set { } }

        int current_frame = 0;

        int width, height;

        public BlurBall(int frame,int width,int height)
        {
            this.width = width;
            this.height = height;

            frames = new BlurFrameBufferWrapper[frame];
            for(int i = 0; i < frame; i++)
            {
                frames[i] = new BlurFrameBufferWrapper(width, height);
                
                var frameGameObj = new GameObject();
                frameGameObj.components.Add(new TextureSprite(frames[i].Texture, width, height));
                ((TextureSprite)frameGameObj.sprite).Color = new Vec4(1,1,1,1);
                Engine.scene.GameObjectRoot.GameObjectChildren.Add(frameGameObj);
            }

            BallA balla = new BallA("Assets/cursor.png");
            this.components.Add(balla);
            balla.center = new Vector(balla.Texture.bitmap.Width / 2, balla.Texture.bitmap.Height / 2);
            balla.Color = new Vec4(0, 1, 0,1);
        }

        public override void beforeDraw()
        {
            base.beforeDraw();

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, frames[current_frame].FrameBufferId);

            GL.Clear(ClearBufferMask.ColorBufferBit);

        }

        public override void afterDraw()
        {
            base.afterDraw();

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

            current_frame++;
            if (current_frame >= frames.Length)
                current_frame = 0;
        }

        public override void update()
        {
            base.update();
        }

        class BallA : TextureSprite
        {
            public BallA(string path) : base(path) { }

            public override void draw(RenderingMode renderMode)
            {
                base.draw(renderMode);
            }

            public override void update()
            {
                base.update();
                this.gameObject.position = new Vector((float)(Math.Sin(Environment.TickCount * 0.001)+1)*(Window.CurrentWindow.Width/2), Window.CurrentWindow.Width / 2);
            }
        }
    }
}
