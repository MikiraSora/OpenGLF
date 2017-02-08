using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StoryBroadParser;
using OpenGLF;
using System.Drawing;
using OpenGLF_EX;

namespace OsuStoryBroadPlayer
{
    class StoryBroadInitializer
    {
        List<StoryBroadParser.Sprite> _spriteList;

        string _oszFilePath;

        public StoryBroadInitializer(string oszFilePath,List<StoryBroadParser.Sprite> spriteList)
        {
            _spriteList = spriteList;
            _oszFilePath = oszFilePath;
        }

        public void Genarate()
        {
            GameObject gameObject;
            TextureSprite sprite;
            StoryBroadParser.Sprite SBSprite;

            foreach (var spriteObject in _spriteList)
            {
                gameObject = new GameObject();

                SBSprite = spriteObject;

                //setup image
                sprite = new TextureSprite(_oszFilePath+spriteObject._imgPath);
                //setup anchor
                sprite.center = new Vector(sprite.Texture.bitmap.Width / 2, sprite.Texture.bitmap.Height/ 2);

                sprite.width = sprite.Texture.bitmap.Width;
                sprite.height = sprite.Texture.bitmap.Height;

                gameObject.components.Add(sprite);
                gameObject.components.Add(new ActionExecutor());

                gameObject.LocalPosition = new Vector(400, 300);

                Engine.scene.GameObjectRoot.addChild(gameObject);

                gameObject.getComponent<ActionExecutor>().executeAction(buildSpriteCommand(ref gameObject,ref SBSprite));
            }
        }

        struct SBActions
        {
            public StoryBroadParser.Sprite sprite;
            public Command command;
            public ActionBase action;
        }

        public ActionBase buildSpriteCommand(ref GameObject gameObject, ref StoryBroadParser.Sprite spriteObject)
        {
            IInterpolator interpolator = null;
            List<SBActions> action_list = new List<SBActions>();

            ActionBase action=null;

            int prev_x=(int)gameObject.LocalPosition.x, prev_y=(int)gameObject.LocalPosition.y;

            float prev_w = gameObject.sprite.scale.x, prev_h = gameObject.sprite.scale.y, startScale = 0, endScale = 0,startAngle,endAngle;

            Vector startScaleVector, endScaleVector;

            float prev_fade = 1;

            Vec4 startColor, endColor;

            spriteObject._commands.Sort();

            SBActions sbAction;

            foreach (var command in spriteObject._commands)
            {

                interpolator = getEasing(command._easing);

                switch (command._event)
                {
                    case Events.Move:
                        action = new MoveToAction(gameObject,
                            (int)Convert.ToSingle(command._params[0]), (int)Convert.ToSingle(command._params[1]),
                            (int)Convert.ToSingle(command._params[2]), (int)Convert.ToSingle(command._params[3]),
                            command._endTime-command._startTime,interpolator);
                        //prev_x = Int32.Parse(command._params[0]);
                        //prev_y = Int32.Parse(command._params[1]);
                        break;

                    case Events.Fade:
                        action = new ColorToAction(gameObject,new Vec4(1,1,1,Convert.ToSingle(command._params[0])),new Vec4(1, 1, 1, Convert.ToSingle(command._params[1])),
                            command._endTime - command._startTime
                            , interpolator);
                        prev_fade = Convert.ToSingle(command._params[1]);
                        break;

                    case Events.Scale:
                        startScale = Convert.ToSingle(command._params[0]);
                        endScale = Convert.ToSingle(command._params[1]);
                        action = new ScaleToAction(gameObject,new Vector(startScale,endScale),new Vector(endScale,endScale),
                            command._endTime - command._startTime
                            , interpolator);
                        break;

                    case Events.VectorScale:
                        startScaleVector = new Vector(Convert.ToSingle(command._params[0]), Convert.ToSingle(command._params[1]));
                        endScaleVector = new Vector(Convert.ToSingle(command._params[2]), Convert.ToSingle(command._params[3]));
                        action = new ScaleToAction(gameObject, startScaleVector,endScaleVector,
                            command._endTime - command._startTime
                            , interpolator);
                        break;

                    case Events.Rotate:
                        startAngle = Convert.ToSingle(command._params[0]);
                        endAngle = Convert.ToSingle(command._params[1]);
                        action = new RotateToAction(gameObject, startAngle, endAngle, command._endTime - command._startTime, interpolator);
                        break;

                    case Events.Color:
                        startColor = new Vec4(Convert.ToSingle(command._params[0]), Convert.ToSingle(command._params[0]), Convert.ToSingle(command._params[0]),prev_fade);
                        endColor = new Vec4(Convert.ToSingle(command._params[3]), Convert.ToSingle(command._params[4]), Convert.ToSingle(command._params[5]), prev_fade);
                        action = new ColorToAction(gameObject, startColor, endColor, command._endTime - command._startTime,interpolator);
                        break;

                    case Events.Parameter:
                        break;
                    case Events.MoveX:
                        break;
                    case Events.MoveY:
                        break;
                    case Events.Loop:
                        break;
                    case Events.Trigger:
                        break;
                    default:
                        throw new Exception("unknown event type");
                }

                sbAction = new SBActions();
                sbAction.action = action;
                sbAction.command = command;
                sbAction.sprite = spriteObject;

                action_list.Add(sbAction);
            }

            int waitOffset = spriteObject._commands.Count != 0 ? spriteObject._commands[0]._startTime<0?0- spriteObject._commands[0]._startTime:0 : 0;

            int prev_time = spriteObject._commands.Count!=0?spriteObject._commands[0]._startTime:-28577582;

            List<List<ActionBase>> actionList = new List<List<ActionBase>>();

            actionList.Add(new List<ActionBase>());
            actionList[actionList.Count - 1].Add(new ColorToAction(gameObject,new Vec4(1,1,1,0),0,new LinearInterpolator()));
            actionList.Add(new List<ActionBase>());
            actionList[actionList.Count - 1].Add(new WaitAction(spriteObject._commands.Count != 0 ? spriteObject._commands[0]._startTime : 0));
            actionList.Add(new List<ActionBase>());
            actionList[actionList.Count - 1].Add(new ColorToAction(gameObject, new Vec4(1, 1, 1, 1), 0, new LinearInterpolator()));

            List<ActionBase> result = new List<ActionBase>();

            for(int i = 0; i < action_list.Count; i++)
            {
                sbAction = action_list[i];

                if (sbAction.command._startTime != prev_time)
                {
                    if (actionList.Count != 0)
                    {
                        actionList.Add(new List<ActionBase>());
                        actionList[actionList.Count - 1].Add(new WaitAction(Math.Abs(sbAction.command._startTime - prev_time)));
                    }

                    actionList.Add(new List<ActionBase>());
                    prev_time = sbAction.command._startTime;
                }

                actionList[actionList.Count - 1].Add(sbAction.action);
            }

            foreach (var list in actionList)
            {
                if (list.Count == 1)
                {
                    result.Add(list[0]);
                    continue;
                }

                result.Add(new ComboAction(false,list));
            }

            if (result.Count == 0)
                return new WaitAction(0);

            return new FrameAction(result);
        }

        public IInterpolator getEasing(Easing easing)
        {
            return new LinearInterpolator();
        }
    }
}
