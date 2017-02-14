﻿using System;
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

                var list=initializer.Genarate();

                Schedule.addMainThreadUpdateTask(new Schedule.ScheduleTask(1000, true, null, -1, (refTask, param) =>
                {
                    for(int i = 0; i < list.Count; i++)
                    {
                        var sprite = list[i];

                        if (player.PlayPosition + 1000 > sprite._startTime)
                        {
                            Log.User("playback:{0},show {1} sprite in {2}",player.PlayPosition,sprite.name,sprite._startTime);
                            Engine.scene.GameObjectRoot.addChild(sprite);
                            list.RemoveAt(i);
                            i--;
                        }
                    }

                    if (list.Count == 0)
                        refTask.markLoopDone();
                }));

                //miss plugin
                mp3PlayerSource = Engine.sound.AddSoundSourceFromFile(_mp3FilePath);
                player=Engine.sound.Play2D(mp3PlayerSource,false,false,true);

                initializer.SetPlayer(player);
            }

           
            protected override void OnMouseDown(MouseButtonEventArgs e)
            {
                base.OnMouseDown(e);
            }

            protected override void OnUpdateFrame(FrameEventArgs e)
            {
                base.OnUpdateFrame(e);
                Title = string.Format("StoryBoard Player time:{0} \t fps:{1}", player.PlayPosition , Math.Truncate(UpdateFrequency));
            }
            int i = 0;
            protected override void OnKeyPress(KeyPressEventArgs e)
            {
                base.OnKeyPress(e);
                Singal.registerSingalTrigger("a", (trigger, param) => {
                    Console.WriteLine(i+"aaa");
                });
                Singal.sendSingal("a",null);
                i++;
            }
        }

        public static void Main(string[] argv)
        {
            //path
            string oszPath = /*@"372552 yuiko - Azuma no Sora kara Hajimaru Sekai (Short)\"*/@"G:\osu!\Songs\49106 jun - KIMONO PRINCESS\";
           
            StoryBroadPlayer player = new StoryBroadPlayer(oszPath,oszPath+ @"KIMONO PRINCESS.mp3", oszPath+ @"jun - KIMONO PRINCESS (Philippines).osb", oszPath+ @"jun - KIMONO PRINCESS (Philippines) [Wmf's Taiko].osu");
            player.Run();
        }
    }
}
