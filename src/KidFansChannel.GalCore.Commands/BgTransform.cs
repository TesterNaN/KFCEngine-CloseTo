using System;
using System.Collections.Generic;
using System.Drawing;
using System.Xml;
using KidFansChannel.GalCore.ExtBases;

namespace KidFansChannel.GalCore.Commands
{
	public class BgTransform : ScriptCommand
	{
		protected Rectangle _bounds;

		protected float _speed;

		private string _styleStr;

		private TransformStyle<BgDisplay> _style;

		private bool _shock;

		private int _layer;

		private BgDisplay bgDisplay;

		public static Dictionary<string, Type> Types = new Dictionary<string, Type>();

		public Rectangle Bounds
		{
			get
			{
				return _bounds;
			}
			set
			{
				_bounds = value;
			}
		}

		protected float Speed
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

		public TransformStyle<BgDisplay> Style
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

		protected int Layer
		{
			get
			{
				return _layer;
			}
			set
			{
				_layer = value;
			}
		}

		public BgTransform()
		{
			Single = true;
			Lasting = true;
			Command = "bgtransform";
		}

		public override void ConvertXml(XmlNode node, NodeInfo info)
		{
			base.ConvertXml(node, info);
			_styleStr = ScriptHelper.GetArgumentString(node, info, "style", string.Empty);
			if (_styleStr == string.Empty)
			{
				_styleStr = null;
				double top = ScriptHelper.GetArgumentFloat(node, info, "top", 0f);
				double bottom = ScriptHelper.GetArgumentFloat(node, info, "bottom", 1f);
				double left = ScriptHelper.GetArgumentFloat(node, info, "left", 0f);
				double right = ScriptHelper.GetArgumentFloat(node, info, "right", 1f);
				_bounds.Width = (int)(1.0 / (right - left) * (double)BgDisplay.ScreenSize.Width + 0.5);
				_bounds.Height = (int)(1.0 / (bottom - top) * (double)BgDisplay.ScreenSize.Height + 0.5);
				_bounds.X = -(int)(left * (double)_bounds.Width + 0.5);
				_bounds.Y = -(int)(top * (double)_bounds.Height + 0.5);
				_speed = ScriptHelper.GetArgumentFloat(node, info, "speed", BgDisplay.DefaultSpeed);
				if (_speed < 0f)
				{
					throw new ScriptArgumentException(node, info, "Speed cannot be negtive.");
				}
			}
			else
			{
				if (_speed < 0f)
				{
					throw new ScriptArgumentException(node, info, "Speed cannot be negtive.");
				}
				if (!Types.ContainsKey(_styleStr))
				{
					throw new RuntimeException(Node, Info, "Not supportted style.");
				}
				Type type = Types[_styleStr];
				_style = (TransformStyle<BgDisplay>)type.Assembly.CreateInstance(type.FullName);
				_shock = ScriptHelper.GetArgumentBool(node, info, "shock", false);
			}
			_layer = ScriptHelper.GetArgumentInteger(node, info, "layer", 0);
			Single = _speed > 0f;
		}

		public override void InitCommand()
		{
			base.InitCommand();
			foreach (ScriptCommand command in ScriptCommand.CommandList)
			{
				if (command is BgDisplay && ((BgDisplay)command).Layer == _layer)
				{
					bgDisplay = (BgDisplay)command;
					break;
				}
			}
			if (_styleStr != null)
			{
				_style.Init(bgDisplay);
				if (!Lasting)
				{
					_style.Lasting = false;
				}
				bgDisplay.Transform(_style, _shock);
			}
			else
			{
				bgDisplay.Transform(_bounds, _speed);
			}
			FinishCommand();
		}

		public static void AddStyle(Type transformType)
		{
			if (!transformType.IsSubclassOf(typeof(TransformStyle<BgDisplay>)))
			{
				throw new ArgumentException();
			}
			string name = ((TransformStyle<BgDisplay>)transformType.Assembly.CreateInstance(transformType.FullName)).Name;
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
