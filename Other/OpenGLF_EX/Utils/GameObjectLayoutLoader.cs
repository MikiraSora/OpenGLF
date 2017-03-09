using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGLF;
using SimpleWidgetsLayoutScript;
using System.Reflection;

namespace OpenGLF_EX
{
	public class GameObjectLayoutLoader
	{
		Dictionary<String, GameObject> saveGameObject = new Dictionary<string, GameObject>();

		void clear()
		{
			saveGameObject.Clear();
		}

		public List<GameObject> loadFromString(string text)
		{
			List<GameObject> result = new List<GameObject>();

			var wrapperResult = ScriptLoader.LoadFromString(text);

			foreach(var element in wrapperResult.Element)
			{
				var gameObject = buildGameObject(element);
				if (gameObject != null)
					result.Add(gameObject);
			}

			return result;
		}

		GameObject buildGameObject(ElementBase element,GameObject parentGameObject=null)
		{
			GameObject createdGameObject=null;
			switch (element.Type)
			{
				case "GameObject": { createdGameObject= new GameObjectElement(element).implementeInstance(); break; }
				default:
					Log.Warn("unknown element type \"{0}\"",element.Type);
					break;
			}

			if (createdGameObject == null)
				return null;

			//setup parent gameobject
			if (parentGameObject != null)
				parentGameObject.addChild(createdGameObject);

			//build children element
			foreach (var child_element in element.ChildrenElement)
			{
				buildGameObject(child_element, createdGameObject);
			}

			return createdGameObject;
		}

		public class ElementImplementBase : ElementBase
		{
			public ElementImplementBase(ElementBase element)
			{
				ElementName = element.ElementName;
				ChildrenElement = element.ChildrenElement;
				ElementProperties = element.ElementProperties;
				ParentElement = element.ParentElement;
			}

			protected virtual void implementeInstance(GameObject gameobject) { }

			public virtual GameObject implementeInstance() { throw new NotImplementedException("You must override this method and not to call it"); }
		}

		public class GameObjectElement : ElementImplementBase
		{
			public GameObjectElement(ElementBase element):base(element) { }

			protected override void implementeInstance(GameObject gameobject)
			{
				base.implementeInstance(gameobject);

				foreach (var property in this.ElementProperties)
				{
					switch (property.Key)
					{
						case "width": _setup_width(gameobject, property.Value);break;
						case "height": _setup_height(gameobject, property.Value); break;
						case "x": _setup_x(gameobject, property.Value); break;
						case "y": _setup_y(gameobject, property.Value); break;
						case "backgroundImage": _setup_backgroundImage(gameobject, property.Value); break;
						default:
							Log.Warn("unknown element property \"{0}\"", ElementName);
							break;
					}
				}
			}

			public override GameObject implementeInstance() {
				var gameobjcet = new GameObject();
				implementeInstance(gameobjcet);
				return gameobjcet;
			}

			void _setup_width(GameObject gameobject,object data)
			{
				if (gameobject.sprite == null)
					gameobject.components.Add(new TextureSprite());

				gameobject.sprite.width = Convert.ToInt32(data);
			}

			void _setup_height(GameObject gameobject, object data)
			{
				if (gameobject.sprite == null)
					gameobject.components.Add(new TextureSprite());

				gameobject.sprite.height = Int32.Parse(data.ToString());
			}

			void _setup_backgroundImage(GameObject gameobject, object data)
			{
				if (gameobject.sprite == null)
					gameobject.components.Add(new TextureSprite());

				((TextureSprite)gameobject.sprite).Texture = new Texture(data.ToString().Trim('"').Trim());
			}

			void _setup_x(GameObject gameobject, object data)
			{
				gameobject.LocalPosition = new Vector(Int32.Parse(data.ToString()), gameobject.LocalPosition.y);
			}

			void _setup_y(GameObject gameobject, object data)
			{
				gameobject.LocalPosition = new Vector(gameobject.LocalPosition.x, Int32.Parse(data.ToString()));
			}
		}

	}
}
