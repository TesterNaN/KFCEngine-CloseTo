using System;
using System.Drawing;
using System.IO;
using System.Xml;
using KidFansChannel.GalCore.ExtBases;
using KidFansChannel.Game2DCore;

namespace KidFansChannel.GalCore.Commands
{
	public class BgClear : ScriptCommand, IInstanceDataUser, IDrawable, IUpdatable, ISaveable
	{
		private Color _color;

		private Picture _bg;

		private float _speed;

		private string _styleStr;

		private DisplayStyle _style;

		private int _layer;

		private Scene scene;

		private BgDisplay bgDisplay;

		private int saveColor;

		public Picture Bg
		{
			get
			{
				return _bg;
			}
			protected internal set
			{
				_bg = value;
			}
		}

		protected Color Color
		{
			get
			{
				return _color;
			}
			set
			{
				_color = value;
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

		protected string StyleStr
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

		public DisplayStyle Style
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

		public bool DataReady
		{
			get
			{
				if (_style != null)
				{
					return _style.Ready;
				}
				return true;
			}
		}

		public int LayerMajor
		{
			get
			{
				return 2;
			}
		}

		public float LayerMinor
		{
			get
			{
				return 0f;
			}
		}

		public bool CanHide
		{
			get
			{
				return false;
			}
		}

		public BgClear()
		{
			Single = true;
			Lasting = false;
			Command = "bgclear";
		}

		public override void ConvertXml(XmlNode node, NodeInfo info)
		{
			base.ConvertXml(node, info);
			string argumentString = ScriptHelper.GetArgumentString(node, info, "color", string.Empty);
			if (argumentString != string.Empty)
			{
				try
				{
					_color = Color.FromName(argumentString);
				}
				catch (Exception innerException)
				{
					throw new ScriptArgumentException(node, info, "color", innerException);
				}
			}
			_speed = ScriptHelper.GetArgumentFloat(node, info, "speed", BgDisplay.DefaultSpeed);
			if (_speed < 0f)
			{
				throw new ScriptArgumentException(node, info, "Speed cannot be negtive.");
			}
			_styleStr = ScriptHelper.GetArgumentString(node, info, "style", string.Empty);
			if (_styleStr == string.Empty)
			{
				_styleStr = null;
			}
			else if (BgDisplay.Types.ContainsKey(_styleStr))
			{
				Type type = BgDisplay.Types[_styleStr];
				_style = (DisplayStyle)type.Assembly.CreateInstance(type.FullName);
			}
			_layer = ScriptHelper.GetArgumentInteger(node, info, "layer", 0);
			if (_layer == 0)
			{
				if (argumentString == string.Empty)
				{
					_color = Color.Black;
				}
			}
			else
			{
				if (argumentString == string.Empty)
				{
					_color = Color.Transparent;
				}
				if (_color != Color.Transparent)
				{
					throw new ScriptArgumentException(node, info, "color", "Color should be transparent when layer is not 0.");
				}
			}
			Single = _speed > 0f;
		}

		public override void InitCommand()
		{
			base.InitCommand();
			if (_layer == 0)
			{
				_bg = ScriptCommand.GalCore.GameGraphics.CreatePicture(_color);
				_bg.Size = new Size(BgDisplay.ScreenSize.Width, BgDisplay.ScreenSize.Height);
				foreach (ScriptCommand command in ScriptCommand.CommandList)
				{
					if (command is Scene && (!((Scene)command).Continue || ((Scene)command).Load))
					{
						scene = (Scene)command;
						if (_style != null)
						{
							_style.Init(scene);
						}
						scene.Display(_speed, _style);
						break;
					}
				}
				if (scene == null)
				{
					throw new RuntimeException(Node, Info, "No corresponding scene found.");
				}
				return;
			}
			foreach (ScriptCommand command2 in ScriptCommand.CommandList)
			{
				if (command2 is BgDisplay && ((BgDisplay)command2).Layer == _layer)
				{
					bgDisplay = (BgDisplay)command2;
					break;
				}
			}
			if (bgDisplay == null)
			{
				throw new RuntimeException(Node, Info, "No bgdisplay or bgclear with the same layer found.");
			}
			bgDisplay.Hide(_speed);
			FinishCommand();
		}

		public override void FinalCommand()
		{
			if (_bg != null)
			{
				_bg.Dispose();
				_bg = null;
			}
			base.FinalCommand();
		}

		public void InitInstanceData()
		{
			if (_style != null)
			{
				_style.InitData();
			}
		}

		public void FinalInstanceData()
		{
			if (_style != null)
			{
				_style.FinalData();
			}
		}

		public void Draw()
		{
			ScriptCommand.Canvas.DrawPicture(_bg);
		}

		public void Update(double appTime, float elaspedTime)
		{
			if (_bg.Time > 0f)
			{
				if (Lasting && ScriptCommand.PlayModeQuicking)
				{
					_bg.Update(elaspedTime * (float)ScriptCommand.QuickFactor);
				}
				else
				{
					_bg.Update(elaspedTime);
				}
				if (_bg.Time == 0f)
				{
					Lasting = false;
				}
			}
		}

		public void Save(BinaryWriter bw)
		{
			bw.Write(Command);
			bw.Write(Lasting);
			bw.Write(saveColor);
		}

		public void Load(BinaryReader br)
		{
			Lasting = br.ReadBoolean();
			_color = Color.FromArgb(br.ReadInt32());
		}

		public void ConfirmUpdate()
		{
			saveColor = _color.ToArgb();
		}
	}
}
