using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenGLF;
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

                mp3Player = Engine.sound.AddSoundSourceFromFile(_mp3FilePath);

                Engine.sound.Play2D(mp3Player,false,false,true);
            }
        }

        public static void Main(string[] argv)
        {
            string oszPath = @"G:\osu!\Songs\94790 Hatsuki Yura - Fuuga\";

            StoryBroadPlayer player = new StoryBroadPlayer(oszPath,oszPath+ @"fuuga.mp3", oszPath+ @"Hatsuki Yura - Fuuga (Lan wings).osb");
            player.Run();
        }
    }
}
