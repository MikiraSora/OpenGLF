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

            string _mp3FilePath, _osbFilePath;

            ISoundSource mp3Player;

            public StoryBroadPlayer(string oszPath,string mp3FilePath,string osbFilePath):base(640,460)
            {
                string buffer;

                _osbFilePath = osbFilePath;
                _oszPath = oszPath;
                _mp3FilePath = mp3FilePath;

                StreamReader reader = new StreamReader(osbFilePath);

                StoryBroadParser.Sprite sprite;
                Command command;

                List<StoryBroadParser.Sprite> spriteList = new List<StoryBroadParser.Sprite>();

                while (!reader.EndOfStream)
                {
                    buffer = reader.ReadLine();

                    if (Parser.isSpriteLine(buffer))
                    {
                        sprite = Parser.tryParseSprite(buffer);
                        if (sprite != null)
                            spriteList.Add(sprite);
                        continue;
                    }

                    if (Parser.isCommandLine(buffer))
                    {
                        command = Parser.tryParseCommand(buffer);
                        if (command != null)
                        {
                            spriteList[spriteList.Count - 1]._commands.Add(command);
                        }
                        continue;
                    }

                }

                initializer = new StoryBroadInitializer(_oszPath,spriteList);
            }

            protected override void OnLoad(EventArgs e)
            {
                base.OnLoad(e);

                Engine.scene = new Scene();

                initializer.Genarate();

                //mp3Player = Engine.sound.AddSoundSourceFromFile(_mp3FilePath);

                //Engine.sound.Play2D(mp3Player,false,false,true);
            }
            /*
            protected override void OnMouseDown(MouseButtonEventArgs e)
            {
                base.OnMouseDown(e);
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
            }*/
        }

        public static void Main(string[] argv)
        {
            //G:\osu!\Songs\94790 Hatsuki Yura - Fuuga\Hatsuki Yura - Fuuga (Lan wings).osb
            string oszPath = @"G:\";

            StoryBroadPlayer player = new StoryBroadPlayer(oszPath,oszPath+ @"KIMONO PRINCESS.mp3", oszPath+ @"test.osb");
            player.Run();
        }
    }
}
