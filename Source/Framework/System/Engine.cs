using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using OpenTK.Graphics.OpenGL;
using System.IO;
using FarseerPhysics.Dynamics;
using IrrKlang;
using System.Windows.Forms;

namespace OpenGLF
{
    public class Engine
    {
        int[] viewport;
        bool nodraw = false;
        string _projectPath;
        public static World world;
        public static ISoundEngine sound;
        public static bool debugPhysics = false;
        public static bool debugGUI = false;
        public static bool debugGameObject = false;
        public static Scene scene { get { return SceneDirector.CurrentScene; } /*set { _scene = value; Assets._assets = Assets.items.Count; }*/ }
        public delegate void OnDraw();  
        public event OnDraw afterDraw;
        public event OnDraw beforeDraw;
        public static List<string> dependencies = new List<string>();

        public static Shaders shaders { get; set; }

        public Engine()
        {
            Log.Init();

            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Lequal);

            GL.Enable(EnableCap.PointSmooth);
            GL.Hint(HintTarget.PointSmoothHint, HintMode.Nicest);
            GL.Enable(EnableCap.LineSmooth);
            GL.Hint(HintTarget.LineSmoothHint, HintMode.Nicest); 
            //GL.Enable(EnableCap.PolygonSmooth);
            //GL.Hint(HintTarget.PolygonSmoothHint, HintMode.Nicest);

            sound = new ISoundEngine(SoundOutputDriver.AutoDetect, SoundEngineOptionFlag.DefaultOptions);

            world = new World(new FarseerPhysics.Common.Vector2(0, 9.82f));      

            viewport = new int[4];

            shaders = new Shaders();

            Schedule.init();
        }

        ~Engine()
        {

        }

        public void resize(int w, int h)
        {
            Screen.width = w;
            Screen.height = h;

            GL.Viewport(0, 0, Screen.width, Screen.height);
        }

        public void draw(RenderingMode mode, bool callObjectDraw)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            if (scene != null)
                if (Camera.main != null)
                {
                    Color clr = Camera.main.backColor;
                    GL.ClearColor(1.0f / 255.0f * clr.r, 1.0f / 255.0f * clr.g, 1.0f / 255.0f * clr.b, 1.0f / 255.0f * clr.a);
                }
                else
                    GL.ClearColor(System.Drawing.Color.FromArgb(Color.gray.a, Color.gray.r, Color.gray.g, Color.gray.b));
            else
                GL.ClearColor(System.Drawing.Color.Black);

            GL.ClearDepth(1.0f);

            if (Engine.scene != null)
            {
                if (nodraw == false)
                {
                    GL.MatrixMode(MatrixMode.Projection);
                    GL.LoadIdentity();

                    if (Engine.scene.mainCamera != null)
                    {
                        float xx = -(Screen.width / 2);
                        float yy = -(Screen.height / 2);
                        float ww = Screen.width + xx;
                        float hh = Screen.height + yy;

                        float zoom = Engine.scene.mainCamera.z;
                        if (zoom < 0.01f) zoom = 0.01f;

                        GL.Ortho(xx * zoom, ww * zoom, hh * zoom, yy * zoom, 0, 100);
                        GL.Translate(-Engine.scene.mainCamera.gameObject.LocalPosition.x, -Engine.scene.mainCamera.gameObject.LocalPosition.y, 0);
                    }
                    else
                    {
                        GL.Ortho(0, Screen.width, Screen.height, 0, 0, 100);
                        GL.Translate(0, 0, 0);
                    }

                    GL.MatrixMode(MatrixMode.Modelview);
                    GL.LoadIdentity();

                    if (beforeDraw != null)
                        beforeDraw();
                }
                
                if (callObjectDraw == true)
                {
                    Engine.scene.GameObjectRoot.prepareDraw();
                }

                Engine.scene.GameObjectRoot.draw(mode);

                GL.ActiveTexture(TextureUnit.Texture0);
                GL.BindTexture(TextureTarget.Texture2D, -1);

                if (callObjectDraw == true)
                {
                    Engine.scene.GameObjectRoot.finDraw();
                }

                if (nodraw == false)
                {
                    if (afterDraw != null)
                        afterDraw();
                }
            }
            else
            {
                GL.ActiveTexture(TextureUnit.Texture0);
                GL.BindTexture(TextureTarget.Texture2D, -1);
            }
        }

        public void update(float dt, bool callObjectUpdate)
        {
            SceneDirector.update();

            if (Engine.scene != null)
            {
                if (callObjectUpdate == true)
                {
                    Schedule.mainThreadRun();

                    for (int i = 0; i < 6; i++ )
                        world.Step(dt);

                    
                    SelectManager.updateMove((int)Input.mousePosition.x, (int)Input.mousePosition.y);
                    

                    Engine.scene.GameObjectRoot.update();
                }
            }
        }

        /// <summary>
        /// 从屏幕窗口坐标获取对应点击的GameObject
        /// </summary>
        /// <param name="x">纵坐标，0~Window.CurrentWindow.Width</param>
        /// <param name="y">横坐标，0~Window.CurrentWindow.Height</param>
        /// <returns>对应最上层的GameObject</returns>
        public GameObject selectAt(int x, int y)
        {
            return SelectManager.selectGameObject(x, y);
        }

        void setReferences()
        {
            foreach (Asset a in Assets.items)
            {
                a.Loaded();
            }
        }

        public void loadSeparatedAssets(string projectPath, bool locked)
        {
            _projectPath = projectPath;

            Assets.items.Clear();

            string[] packs = Directory.GetFiles(Path.Combine(projectPath), "*.pak");

            for (int i = 0; i < packs.Length; i++)
            {
                AssetList list = new AssetList();

                if (locked)
                {
                    byte[] key = ByteConverter.GetBytes("dfy3hfi3789y478yhge7y578yrhgiudhr8967498u839udhkjghjk");
                    RC4 rc4 = new RC4(key);

                    byte[] file = File.ReadAllBytes(packs[i]);
                    file = rc4.Decode(file, file.Length);

                    MemoryStream ms = new MemoryStream();
                    ms.Write(file, 0, file.Length);
                    ms.Seek(0, SeekOrigin.Begin);

                    list = (AssetList)Serialization.deserialize(ms);

                    ms.Dispose();
                }
                else
                {
                    list = (AssetList)Serialization.deserialize(packs[i]);
                }

                for (int j = 0; j < list.Count; j++)
                {
                    Assets.items.Add(list[j]);
                }
            }

            if (Assets.items.Count > 0)
                setReferences();
        }

        public void saveAssetsTogether(string projectPath, bool locked)
        {
            if (Assets.items != null)
                Serialization.serialize(Path.Combine(projectPath, "assets.pak"), Assets.items);
        }

        public void saveAssetsSeparately(string projectPath, bool locked)
        {
            if (Assets.items != null)
            {
                List<AssetList> packages = new List<AssetList>();

                for (int i = 0; i < Assets.items.Count; i++)
                {
                    Asset asset = Assets.items[i];
                    AssetList package = findPackage(packages, asset.package);

                    if (package == null)
                    {
                        package = new AssetList();
                        package.name = asset.package;
                        packages.Add(package);
                    }

                    if (package.name == null)
                        package.name = "Unnamed";

                    package.Add(asset);
                }

                for (int i = 0; i < packages.Count; i++)
                {
                    string name = Path.Combine(projectPath, packages[i].name.Replace(" ", "_") + ".pak");
                    Serialization.serialize(name, packages[i]);

                    if (locked)
                    {
                        byte[] key = ByteConverter.GetBytes("dfy3hfi3789y478yhge7y578yrhgiudhr8967498u839udhkjghjk");
                        RC4 rc4 = new RC4(key);
                        byte[] file = File.ReadAllBytes(name);
                        file = rc4.Encode(file, file.Length);
                        File.WriteAllBytes(name, file);
                    }
                }
            }
        }

        public void saveAssets(string projectPath, string name, bool locked)
        {
            if (Assets.items != null)
            {
                AssetList package = new AssetList();

                for (int i = 0; i < Assets.items.Count; i++)
                {
                    Asset asset = Assets.items[i];

                    if (asset.package == name)
                    {
                        package.Add(asset);
                    }
                }

                if (package.Count > 0)
                    Serialization.serialize(Path.Combine(projectPath, name.Replace(" ", "_") + ".pak"), package);
            }
        }

        internal AssetList findPackage(List<AssetList> list, string name)
        {
            AssetList lst = null;

            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].name == name)
                {
                    lst = list[i];
                    break;
                }
            }

            return lst;
        }

        public static void stopAnimations()
        {
            /*
            if (Engine.scene != null)
            {
                for (int i = 0; i < Engine.scene.objects.Count; i++)
                    foreach (Component c in Engine.scene.objects[i].components)
                        if (c is Sprite)
                            ((Sprite)c).stop();
            }
            */
            Engine.scene.GameObjectRoot.stopAnimations();
        }
    }
}