using System;
using System.Collections.Generic;
using System.Xml;
using KidFansChannel.GalCore.ExtBases;

namespace KidFansChannel.GalCore.Commands
{
	public class CharaTransform : ScriptCommand
	{
		private int? _x;

		private int? _y;

		private float? _rotate;

		private float? _zoom;

		private int? _alpha;

		private float _speed;

		private int _index;

		private string _styleStr;

		private TransformStyle<CharaDisplay> _style;

		private bool _shock;

		private CharaDisplay charaDisplay;

		public static Dictionary<string, Type> Types = new Dictionary<string, Type>();

		public int? X
		{
			get
			{
				return _x;
			}
			set
			{
				_x = value;
			}
		}

		public int? Y
		{
			get
			{
				return _y;
			}
			set
			{
				_y = value;
			}
		}

		public float? Rotate
		{
			get
			{
				return _rotate;
			}
			protected set
			{
				_rotate = value;
			}
		}

		public float? Zoom
		{
			get
			{
				return _zoom;
			}
			protected set
			{
				_zoom = value;
			}
		}

		public int? Alpha
		{
			get
			{
				return _alpha;
			}
			protected set
			{
				_alpha = value;
			}
		}

		public float Speed
		{
			get
			{
				return _speed;
			}
			set
			{
				_speed = value;
			}
		}

		public int Index
		{
			get
			{
				return _index;
			}
			protected set
			{
				_index = value;
			}
		}

		public string StyleStr
		{
			get
			{
				return _styleStr;
			}
			set
			{
				_styleStr = value;
			}
		}

		public TransformStyle<CharaDisplay> Style
		{
			get
			{
				return _style;
			}
			protected set
			{
				_style = value;
			}
		}

		public bool Shock
		{
			get
			{
				return _shock;
			}
			protected set
			{
				_shock = value;
			}
		}

		public CharaTransform()
		{
			Single = true;
			Lasting = true;
			Command = "charatransform";
		}

		public override void ConvertXml(XmlNode node, NodeInfo info)
		{
			base.ConvertXml(node, info);
			_styleStr = ScriptHelper.GetArgumentString(node, info, "style", string.Empty);
			if (_styleStr == string.Empty)
			{
				_styleStr = null;
				try
				{
					_x = (int)(ScriptHelper.GetArgumentFloat(node, info, "x") * (float)CharaDisplay.StepX + (float)(BgDisplay.ScreenSize.Width / 2));
				}
				catch (ScriptArgumentException)
				{
					_x = null;
				}
				try
				{
					_y = (int)(ScriptHelper.GetArgumentFloat(node, info, "y") * (float)CharaDisplay.StepY + (float)BgDisplay.ScreenSize.Height);
				}
				catch (ScriptArgumentException)
				{
					_y = null;
				}
				try
				{
					_rotate = (float)((double)ScriptHelper.GetArgumentFloat(node, info, "rotate") * Math.PI / 2.0);
				}
				catch (ScriptArgumentException)
				{
					_rotate = null;
				}
				try
				{
					_zoom = ScriptHelper.GetArgumentFloat(node, info, "zoom");
				}
				catch (ScriptArgumentException)
				{
					_zoom = null;
				}
				try
				{
					_alpha = ScriptHelper.GetArgumentInteger(node, info, "alpha");
				}
				catch (ScriptArgumentException)
				{
					_alpha = null;
				}
				_speed = ScriptHelper.GetArgumentFloat(node, info, "speed");
			}
			else
			{
				if (!Types.ContainsKey(_styleStr))
				{
					throw new RuntimeException(Node, Info, "Not supportted style.");
				}
				Type type = Types[_styleStr];
				_style = (TransformStyle<CharaDisplay>)type.Assembly.CreateInstance(type.FullName);
				_shock = ScriptHelper.GetArgumentBool(node, info, "shock", false);
			}
			_index = ScriptHelper.GetArgumentInteger(node, info, "index");
			if (_index < -1)
			{
				throw new ScriptArgumentException(node, info, "index", ArgumentType.Integer, "Index should not less than -1.");
			}
			Single = Speed > 0f;
			Lasting = Speed > 0f;
		}

		public override void InitCommand()
		{
			base.InitCommand();
			foreach (ScriptCommand command in ScriptCommand.CommandList)
			{
				if (command is CharaDisplay && ((CharaDisplay)command).Index == _index)
				{
					charaDisplay = (CharaDisplay)command;
					break;
				}
			}
			if (charaDisplay == null)
			{
				throw new RuntimeException(Node, Info, "No corresponding charadisplay found.");
			}
			if (_style != null)
			{
				_style.Init(charaDisplay);
				if (!Lasting)
				{
					_style.Lasting = false;
				}
				charaDisplay.Transform(_style, _shock);
			}
			else
			{
				if (!_x.HasValue)
				{
					_x = charaDisplay.X;
				}
				if (!_y.HasValue)
				{
					_y = charaDisplay.Y;
				}
				if (!_rotate.HasValue)
				{
					_rotate = charaDisplay.Rotate;
				}
				if (!_zoom.HasValue)
				{
					_zoom = charaDisplay.Zoom;
				}
				if (!_alpha.HasValue)
				{
					_alpha = charaDisplay.Alpha;
				}
				charaDisplay.Transform(_x.Value, _y.Value, _rotate.Value, _zoom.Value, _alpha.Value, _speed, Lasting);
			}
			FinishCommand();
		}

		public static void AddStyle(Type transformType)
		{
			if (!transformType.IsSubclassOf(typeof(TransformStyle<CharaDisplay>)))
			{
				throw new ArgumentException();
			}
			string name = ((TransformStyle<CharaDisplay>)transformType.Assembly.CreateInstance(transformType.FullName)).Name;
			if (Types.ContainsKey(name))
			{
				Types.Remove(name);
			}
			Types.Add(name, transformType);
		}

		public static void ClearStyle()
		{
			Types.Clear();
		}
	}
}
