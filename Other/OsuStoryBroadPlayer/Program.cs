using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenGLF;
using OpenTK.Input;
using IrrKlang;
using OpenTK;
using System.IO;
using System.Threading.Tasks;
using StoryBroadParser;

namespace OsuStoryBroadPlayer
{
    class Program
    {
        class StoryBroadPlayer : Window
        {
            string _oszPath;

            StoryBroadInitializer initializer;

            string _mp3FilePath, _osbFilePath,_osuFilePath;

            ISoundSource mp3PlayerSource;
            ISound player;

            public StoryBroadPlayer(string oszPath,string mp3FilePath,string osbFilePath,string osuFilePath):base(640,460)
            {
                string buffer;

                _osbFilePath = osbFilePath;
                _oszPath = oszPath;
                _mp3FilePath = mp3FilePath;
                _osuFilePath = osuFilePath;

                StreamReader reader;

                List<string> strList = new List<string>();

                List<StoryBroadParser.Sprite> spriteList = new List<StoryBroadParser.Sprite>();

                if (_osuFilePath != null)
                {
                    using (reader = new StreamReader(_osuFilePath))
                    {
                        while (!reader.EndOfStream)
                        {
                            buffer = reader.ReadLine();
                            strList.Add(buffer);
                        }
                    }
                }

                using (reader = new StreamReader(osbFilePath))
                {
                    while (!reader.EndOfStream)
                    {
                        buffer = reader.ReadLine();

                        strList.Add(buffer);
                    }
                }

                spriteList = Parser.parseStrings(strList.ToArray());

                initializer = new StoryBroadInitializer(_oszPath, spriteList);
            }

            protected override void OnLoad(EventArgs e)
            {
                base.OnLoad(e);

                Engine.scene = new Scene();

                initializer.Genarate();

                //miss plugin
                mp3PlayerSource = Engine.sound.AddSoundSourceFromFile(_mp3FilePath);
                player=Engine.sound.Play2D(mp3PlayerSource,false,false,true);
            }

           
            protected override void OnMouseDown(MouseButtonEventArgs e)
            {
                base.OnMouseDown(e);
                /*
                GameObject selectObj = SelectManager.selectGameObject(e.X, e.Y);
                if (selectObj == null)
                    return;
                Console.WriteLine("select objName = {0}\tx:{5}({1}) l_y:{6}({2}) offsetDepth:{3} AbsoluteDepth={4}\n CurrentAction:{7}",
                    selectObj.name,
                    selectObj.LocalPosition.x, selectObj.LocalPosition.y,
                    selectObj.depth, selectObj.FullDepth,
                    selectObj.WorldPosition.x, selectObj.WorldPosition.y,
                    selectObj.getComponent<ActionExecutor>() != null ? (selectObj.getComponent<ActionExecutor>().CurrentAction==null?"NoAction": selectObj.getComponent<ActionExecutor>().CurrentAction.GetType().Name) : "??"
                    );
                    */
            }

            protected override void OnUpdateFrame(FrameEventArgs e)
            {
                base.OnUpdateFrame(e);
                Title = string.Format("StoryBoard Player time:{0} \t fps:{1}", player.PlayPosition , Math.Truncate(UpdateFrequency));
            }
        }

        public static void Main(string[] argv)
        {
            //path
            string oszPath = @"372552 yuiko - Azuma no Sora kara Hajimaru Sekai (Short)\";
            

            StoryBroadPlayer player = new StoryBroadPlayer(oszPath,oszPath+ @"Azuma no sora kara hajimaru sekai (Short).mp3", oszPath+ @"yuiko - Azuma no Sora kara Hajimaru Sekai (Short) (KaedekaShizuru).osb",oszPath+ @"yuiko - Azuma no Sora kara Hajimaru Sekai (Short) (KaedekaShizuru) [Yomi's Hard].osu");
            player.Run();
        }
    }
}
