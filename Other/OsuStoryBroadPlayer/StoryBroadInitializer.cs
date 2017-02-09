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
                sprite = new TextureSprite(TextureManager.cacheTexture(_oszFilePath + spriteObject._imgPath,name=> { return new Texture(name); }));
                //setup anchor
                sprite.center = new Vector(sprite.Texture.bitmap.Width / 2, sprite.Texture.bitmap.Height/ 2);

                sprite.width = sprite.Texture.bitmap.Width;
                sprite.height = sprite.Texture.bitmap.Height;

                gameObject.name = spriteObject._imgPath;

                gameObject.components.Add(sprite);
                gameObject.components.Add(new ActionExecutor());

#if DEBUG
                //gameObject.components.Add(new Selectable());
#endif

                gameObject.LocalPosition = new Vector(Window.CurrentWindow.Width/2, Window.CurrentWindow.Height / 2);

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
                        break;

                    case Events.Fade:
                        action = new FadeToAction(gameObject,
                            Convert.ToSingle(command._params[0]),Convert.ToSingle(command._params[1]),
                            command._endTime - command._startTime
                            , interpolator);
                        break;

                    case Events.Scale:
                        startScale = Convert.ToSingle(command._params[0]);
                        endScale = Convert.ToSingle(command._params[1]);
                        action = new ScaleToAction(gameObject,new Vector(startScale,startScale),new Vector(endScale,endScale),
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

            Command prev_command=null;

            gameObject.sprite.setColor(1, 1, 1, 0);

            Dictionary<Type, List<SBActions>> map = new Dictionary<Type, List<SBActions>>();

            Type type;

            foreach(SBActions sbaction in action_list)
            {
                type = sbaction.action.GetType();
                if (!map.ContainsKey(type))
                    map.Add(type, new List<SBActions>());

                map[sbaction.action.GetType()].Add(sbaction);
            }

            List<ActionBase> result = new List<ActionBase>();

            /*
            List<List<ActionBase>> actionList = new List<List<ActionBase>>();

            actionList.Add(new List<ActionBase>());
            actionList[actionList.Count - 1].Add(new WaitAction(spriteObject._commands.Count != 0 ? spriteObject._commands[0]._startTime : 0));

            List<ActionBase> result = new List<ActionBase>();

            for(int i = 0; i < action_list.Count; i++)
            {
                sbAction = action_list[i];

                if (sbAction.command._startTime != (prev_command==null?0:prev_command._startTime))
                {
                    if (actionList.Count != 0)
                    {
                        int waitTime = Math.Abs(sbAction.command._startTime - (prev_command == null ? 0 : prev_command._startTime));
                        if (waitTime != 0)
                        {
                            actionList.Add(new List<ActionBase>());
                            actionList[actionList.Count - 1].Add(new WaitAction(0));
                        }
                    }

                    actionList.Add(new List<ActionBase>());
                    prev_command = sbAction.command;
                }

                actionList[actionList.Count - 1].Add(sbAction.action);
            }
            */

            foreach (var list in map)
            {
                //Multi-Command process

                List<ActionBase> actionbaseList = new List<ActionBase>();

                //Pick all command up;
                for (int i=0;i<list.Value.Count;i++)
                {
                    sbAction = list.Value[i];
                    int offsetTime;
                    if (i != 0)
                    {
                        offsetTime =Math.Abs(list.Value[i - 1].command._endTime - sbAction.command._startTime);
                    }
                    else
                    {
                        offsetTime = sbAction.command._startTime;
                    }

                    if (offsetTime != 0)
                    {
                        actionbaseList.Add(new WaitAction(offsetTime)); //wait for next same event
                    }
                    actionbaseList.Add(sbAction.action);
                }

                result.Add(new FrameAction(actionbaseList));
            }

            if (result.Count == 0)
                return new WaitAction(0);

            return new ComboAction(false,result);
        }

        public IInterpolator getEasing(Easing easing)
        {
            return new LinearInterpolator();
        }
    }
}
