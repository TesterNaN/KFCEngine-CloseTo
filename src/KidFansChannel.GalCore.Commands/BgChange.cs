using System.Drawing;
using System.Xml;
using KidFansChannel.Game2DCore;

namespace KidFansChannel.GalCore.Commands
{
	public class BgChange : ScriptCommand, IInstanceDataUser, IDrawable, IUpdatable
	{
		private Picture _bg;

		private string _bgStr;

		private string _bgName;

		private Rectangle? _bounds;

		private float _speed;

		private int _layer;

		private int _albumId;

		private SizeF _size;

		protected BgDisplay bgDisplay;

		private bool bgCreated;

		protected Picture Bg
		{
			get
			{
				return _bg;
			}
			set
			{
				_bg = value;
			}
		}

		protected string BgStr
		{
			get
			{
				return _bgStr;
			}
			set
			{
				_bgStr = value;
			}
		}

		public string BgName
		{
			get
			{
				return _bgName;
			}
			set
			{
				_bgName = value;
			}
		}

		protected Rectangle? Bounds
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

		public int AlbumId
		{
			get
			{
				return _albumId;
			}
			protected set
			{
				_albumId = value;
			}
		}

		public SizeF Size
		{
			get
			{
				return _size;
			}
			set
			{
				_size = value;
			}
		}

		public bool DataReady
		{
			get
			{
				if (!bgCreated && _bg.IsCreated)
				{
					if (_bounds.HasValue)
					{
						_bg.Position = _bounds.Value.Location;
						_bg.Scale = new SizeF((float)_bounds.Value.Width * _size.Width / (float)_bg.Width, (float)_bounds.Value.Height * _size.Height / (float)_bg.Height);
					}
					_bg.Alpha = 0;
					bgCreated = true;
				}
				return _bg.IsCreated;
			}
		}

		public int LayerMajor
		{
			get
			{
				if (_layer < 0)
				{
					return 0;
				}
				if (_layer == 0)
				{
					return 2;
				}
				return 18;
			}
		}

		public float LayerMinor
		{
			get
			{
				if (Layer < int.MaxValue)
				{
					return (float)Layer + 0.5f;
				}
				return 0.5f;
			}
		}

		public bool CanHide
		{
			get
			{
				return false;
			}
		}

		public BgChange()
		{
			Single = true;
			Lasting = true;
			Command = "bgchange";
			DataName = "bg";
		}

		public override void ConvertXml(XmlNode node, NodeInfo info)
		{
			base.ConvertXml(node, info);
			_bgStr = ScriptHelper.GetArgumentString(node, info, "file");
			float? top = null;
			float? bottom = null;
			float? left = null;
			float? right = null;
			try
			{
				top = ScriptHelper.GetArgumentFloat(node, info, "top");
			}
			catch (ScriptArgumentException)
			{
			}
			try
			{
				bottom = ScriptHelper.GetArgumentFloat(node, info, "bottom");
			}
			catch (ScriptArgumentException)
			{
			}
			try
			{
				left = ScriptHelper.GetArgumentFloat(node, info, "left");
			}
			catch (ScriptArgumentException)
			{
			}
			try
			{
				right = ScriptHelper.GetArgumentFloat(node, info, "right");
			}
			catch (ScriptArgumentException)
			{
			}
			if (!top.HasValue && !bottom.HasValue && !left.HasValue && !right.HasValue)
			{
				_bounds = null;
			}
			else
			{
				if (!top.HasValue)
				{
					top = 0f;
				}
				if (!bottom.HasValue)
				{
					bottom = 1f;
				}
				if (!left.HasValue)
				{
					left = 0f;
				}
				if (!right.HasValue)
				{
					right = 1f;
				}
				Rectangle value = default(Rectangle);
				value.Width = (int)((double?)(1f / (right - left) * 800f) + 0.5).Value;
				value.Height = (int)((double?)(1f / (bottom - top) * 600f) + 0.5).Value;
				value.X = -(int)((double?)(left * (float)value.Width) + 0.5).Value;
				value.Y = -(int)((double?)(top * (float)value.Height) + 0.5).Value;
				_bounds = value;
			}
			_speed = ScriptHelper.GetArgumentFloat(node, info, "speed", BgDisplay.DefaultSpeed);
			if (_speed < 0f)
			{
				throw new ScriptArgumentException(node, info, "Speed cannot be negtive.");
			}
			_layer = ScriptHelper.GetArgumentInteger(node, info, "layer", 0);
			LoadDataInfo();
			Single = _speed > 0f;
		}

		protected void LoadDataInfo()
		{
			XmlDocument xmlDocument = new XmlDocument();
			_bgName = null;
			_albumId = -1;
			_size = new SizeF(1f, 1f);
			if (!ScriptCommand.Data.LoadXmlDocument(DataName, _bgStr + ".xml", xmlDocument))
			{
				return;
			}
			XmlNode documentElement = xmlDocument.DocumentElement;
			if (documentElement.Name != "bg")
			{
				throw new ScriptStructureException(documentElement, new NodeInfo(), "Bg's document element should be bg.");
			}
			XmlNode xmlNode;
			if ((xmlNode = documentElement.SelectSingleNode("name")) != null)
			{
				if (!xmlNode.HasChildNodes || xmlNode.ChildNodes.Count != 1)
				{
					throw new ScriptStructureException(xmlNode, new NodeInfo(), "Name should have only 1 child node.");
				}
				_bgName = xmlNode.FirstChild.Value;
			}
			if ((xmlNode = documentElement.SelectSingleNode("album")) != null)
			{
				_albumId = ScriptHelper.GetArgumentInteger(xmlNode, new NodeInfo(), "id");
				if (_albumId < 0)
				{
					throw new ScriptArgumentException(xmlNode, new NodeInfo(), "id", ArgumentType.Integer, "Id should not less than 0.");
				}
				ScriptCommand.GameState.UpdateAlbum(_albumId);
			}
			if ((xmlNode = documentElement.SelectSingleNode("size")) != null)
			{
				_size.Width = ScriptHelper.GetArgumentFloat(xmlNode, new NodeInfo(), "width", _size.Width);
				if (_size.Width <= 0f)
				{
					throw new ScriptArgumentException(xmlNode, new NodeInfo(), "width", ArgumentType.Float, "Width should greater than 0.");
				}
				_size.Height = ScriptHelper.GetArgumentFloat(xmlNode, new NodeInfo(), "height", _size.Height);
				if (_size.Height <= 0f)
				{
					throw new ScriptArgumentException(xmlNode, new NodeInfo(), "height", ArgumentType.Float, "Height should greater than 0.");
				}
			}
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
			if (bgDisplay == null)
			{
				throw new RuntimeException(Node, Info, "No bgdisplay with the same layer found.");
			}
			_bg.BeginTransform(_speed);
			_bg.AlphaTo(255);
			_bg.EndTransform();
		}

		public override void FinalCommand()
		{
			base.FinalCommand();
			FinalInstanceData();
		}

		public void InitInstanceData()
		{
			_bg = ScriptCommand.Data.LoadPicture("bg", _bgStr + ".png");
		}

		public void FinalInstanceData()
		{
		}

		public void Draw()
		{
			if (DataReady)
			{
				ScriptCommand.Canvas.DrawPicture(Bg);
			}
		}

		public void Update(double appTime, float elaspedTime)
		{
			if (Lasting && ScriptCommand.PlayModeQuicking)
			{
				_bg.Update(elaspedTime * (float)ScriptCommand.QuickFactor);
			}
			else
			{
				_bg.Update(elaspedTime);
			}
			if (!_bounds.HasValue)
			{
				_bg.Position = bgDisplay.Bg.Position;
				_bg.ScaleX = bgDisplay.Bg.ScaleX * _size.Width / (float)_bg.Width / bgDisplay.Size.Width * (float)bgDisplay.Bg.Width;
				_bg.ScaleY = bgDisplay.Bg.ScaleY * _size.Height / (float)_bg.Height / bgDisplay.Size.Height * (float)bgDisplay.Bg.Height;
			}
			if (_bg.Time == 0f)
			{
				if (bgDisplay.Bg != null)
				{
					bgDisplay.Bg.Dispose();
					bgDisplay.Bg = null;
				}
				bgDisplay.Bg = _bg;
				if (_bounds.HasValue)
				{
					bgDisplay.Bounds = _bounds.Value;
				}
				bgDisplay.Size = _size;
				bgDisplay.BgStr = _bgStr;
				bgDisplay.BgName = _bgName;
				bgDisplay.AlbumId = _albumId;
				FinishCommand();
			}
		}
	}
}
