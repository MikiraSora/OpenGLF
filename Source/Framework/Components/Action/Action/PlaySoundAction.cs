using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IrrKlang;

namespace OpenGLF
{
    public class PlaySoundAction : ImmediatelyActionBase
    {
        ISoundSource _sound;

        bool isTempResource = true;

        float _volume;

        /// <summary>
        /// notice : audio playback length must over 1s
        /// http://www.ambiera.com/forum.php?t=860
        /// </summary>
        /// <param name="audioFilePath"></param>
        /// <param name="normalize_volume"></param>
        /// <param name="gameobject"></param>
        public PlaySoundAction(string audioFilePath,float normalize_volume,GameObject gameobject):base(gameobject)
        {
            _sound = Engine.sound.AddSoundSourceFromFile(audioFilePath);
            _volume = normalize_volume;
        }

        public PlaySoundAction(Audio audioAsset,float normalize_volume,GameObject gameobject) : base(gameobject)
        {
            _sound = Engine.sound.AddSoundSourceFromMemory(audioAsset.data, "PlaySoundAction");
            _volume = normalize_volume;
        }

        public override void onUpdate()
        {
            var audio = Engine.sound.Play2D(_sound, false, false, true);
            audio.Volume = _volume;
        }

        ~PlaySoundAction()
        {
            if (isTempResource)
            {
                _sound.Dispose();
            }
        }
    }
}
