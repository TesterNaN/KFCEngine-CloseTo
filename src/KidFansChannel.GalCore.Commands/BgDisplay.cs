using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Xml;
using KidFansChannel.GalCore.ExtBases;
using KidFansChannel.Game2DCore;

namespace KidFansChannel.GalCore.Commands
{
	public class BgDisplay : ScriptCommand, IDrawable, IUpdatable, IInstanceDataUser, ISaveable
	{
		private Rectangle _bounds;

		private Picture _bg;

		private string _bgStr;

		private string _bgName;

		private float _speed;

		private string _styleStr;

		private DisplayStyle _style;

		private int _layer;

		private int _albumId;

		private SizeF _size;

		private Scene scene;

		public static float DefaultSpeed = 1f;

		public static Size ScreenSize = new Size(800, 600);

		private TransformStyle<BgDisplay> _transformStyle;

		private bool _shocking;

		private bool bgCreated;

		public static Dictionary<string, Type> Types = new Dictionary<string, Type>();

		private bool loading;

		private bool saveLasting;

		private string saveBgStr;

		private int saveX;

		private int saveY;

		private int saveWidth;

		private int saveHeight;

		private int saveLayer;

		private string saveTransformStyle;

		private bool saveShocking;

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

		public Picture Bg
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

		public string BgStr
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
			set
			{
				_style = value;
			}
		}

		public int Layer
		{
			get
			{
				return _layer;
			}
			protected set
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
			set
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

		protected TransformStyle<BgDisplay> TransformStyle
		{
			get
			{
				return _transformStyle;
			}
		}

		protected bool Shocking
		{
			get
			{
				return _shocking;
			}
		}

		public bool DataReady
		{
			get
			{
				if (!bgCreated && _bg.IsCreated)
				{
					_bg.Position = _bounds.Location;
					_bg.Scale = new SizeF((float)_bounds.Width * _size.Width / (float)_bg.Width, (float)_bounds.Height * _size.Height / (float)_bg.Height);
					bgCreated = true;
				}
				if (_bg.IsCreated)
				{
					if (_style != null)
					{
						return _style.Ready;
					}
					return true;
				}
				return false;
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
				return _layer;
			}
		}

		public bool CanHide
		{
			get
			{
				return false;
			}
		}

		public BgDisplay()
		{
			Single = true;
			Lasting = false;
			Command = "bgdisplay";
			DataName = "bg";
		}

		public override void ConvertXml(XmlNode node, NodeInfo info)
		{
			base.ConvertXml(node, info);
			_bgStr = ScriptHelper.GetArgumentString(node, info, "file");
			double top = ScriptHelper.GetArgumentFloat(node, info, "top", 0f);
			double bottom = ScriptHelper.GetArgumentFloat(node, info, "bottom", 1f);
			double left = ScriptHelper.GetArgumentFloat(node, info, "left", 0f);
			double right = ScriptHelper.GetArgumentFloat(node, info, "right", 1f);
			_bounds.Width = (int)(1.0 / (right - left) * (double)ScreenSize.Width + 0.5);
			_bounds.Height = (int)(1.0 / (bottom - top) * (double)ScreenSize.Height + 0.5);
			_bounds.X = -(int)(left * (double)_bounds.Width + 0.5);
			_bounds.Y = -(int)(top * (double)_bounds.Height + 0.5);
			_speed = ScriptHelper.GetArgumentFloat(node, info, "speed", DefaultSpeed);
			if (_speed < 0f)
			{
				throw new ScriptArgumentException(node, info, "Speed cannot be negtive.");
			}
			_styleStr = ScriptHelper.GetArgumentString(node, info, "style", string.Empty);
			if (_styleStr == string.Empty)
			{
				_styleStr = null;
			}
			else if (Types.ContainsKey(_styleStr))
			{
				Type type = Types[_styleStr];
				_style = (DisplayStyle)type.Assembly.CreateInstance(type.FullName);
			}
			_layer = ScriptHelper.GetArgumentInteger(node, info, "layer", 0);
			LoadDataInfo();
			if (_layer != 0)
			{
				Lasting = true;
			}
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
			if (_layer == 0)
			{
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
			}
			else
			{
				_bg.Alpha = 0;
				if (_speed > 0f)
				{
					_bg.BeginTransform(_speed);
					_bg.AlphaTo(255);
					_bg.EndTransform();
				}
				else
				{
					_bg.Alpha = 255;
					Lasting = false;
				}
			}
			if (!loading)
			{
				return;
			}
			if (_transformStyle != null)
			{
				if (_shocking)
				{
					ScriptCommand.GalCore.StartShock();
				}
				_transformStyle.InitStyle();
			}
			loading = false;
		}

		public override void FinalCommand()
		{
			base.FinalCommand();
			if (_shocking)
			{
				ScriptCommand.GalCore.StopShock();
				_shocking = false;
			}
			FinalInstanceData();
		}

		public void Transform(Rectangle bounds, double speed)
		{
			if (_shocking)
			{
				ScriptCommand.GalCore.StopShock();
				_shocking = false;
			}
			else if (_shocking)
			{
				ScriptCommand.GalCore.StopShock();
				_shocking = false;
			}
			_transformStyle = null;
			_bounds = bounds;
			_bg.BeginTransform((float)speed);
			_bg.PositionTo(bounds.Location);
			_bg.ScaleTo(new SizeF((float)_bounds.Width * _size.Width / (float)_bg.Width, (float)_bounds.Height * _size.Height / (float)_bg.Height));
			_bg.EndTransform();
			Lasting = true;
		}

		public virtual void Transform(TransformStyle<BgDisplay> style, bool shock)
		{
			if (shock)
			{
				_shocking = true;
				ScriptCommand.GalCore.StartShock();
			}
			_transformStyle = style;
			_transformStyle.InitStyle();
			Lasting = _transformStyle.Lasting;
		}

		public void Hide(float speed)
		{
			if (_shocking)
			{
				ScriptCommand.GalCore.StopShock();
				_shocking = false;
			}
			if (speed == 0f)
			{
				FinishCommand();
				return;
			}
			_bg.BeginTransform(speed);
			_bg.AlphaTo(0);
			_bg.EndTransform();
			Lasting = true;
		}

		public void InitInstanceData()
		{
			if (_bgStr != null)
			{
				_bg = ScriptCommand.Data.LoadPicture(DataName, _bgStr + ".png");
			}
			if (_style != null)
			{
				_style.InitData();
			}
		}

		public void FinalInstanceData()
		{
			if (_shocking)
			{
				ScriptCommand.GalCore.StopShock();
				_shocking = false;
			}
			if (_bg != null)
			{
				_bg.Dispose();
				_bg = null;
			}
			if (_style != null)
			{
				_style.FinalData();
			}
		}

		public virtual void Draw()
		{
			if (DataReady)
			{
				ScriptCommand.Canvas.DrawPicture(_bg);
			}
		}

		public virtual void Update(double appTime, float elaspedTime)
		{
			if (_bg.Time > 0f)
			{
				if (ScriptCommand.PlayModeQuicking)
				{
					_bg.Update(elaspedTime * (float)ScriptCommand.QuickFactor);
				}
				else
				{
					_bg.Update(elaspedTime);
				}
				if (_bg.Time == 0f)
				{
					if (_bg.Alpha == 0)
					{
						FinishCommand();
					}
					else
					{
						Lasting = false;
					}
				}
			}
			if (_transformStyle == null)
			{
				return;
			}
			_transformStyle.UpdateStyle(elaspedTime);
			if (_transformStyle.Finished)
			{
				if (_shocking)
				{
					ScriptCommand.GalCore.StopShock();
					_shocking = false;
				}
				_transformStyle = null;
				Lasting = false;
			}
		}

		public static void AddStyle(Type displayType)
		{
			if (!displayType.IsSubclassOf(typeof(DisplayStyle)))
			{
				throw new ArgumentException();
			}
			string name = ((DisplayStyle)displayType.Assembly.CreateInstance(displayType.FullName)).Name;
			if (Types.ContainsKey(name))
			{
				Types.Remove(name);
			}
			Types.Add(name, displayType);
		}

		public static void ClearStyle()
		{
			Types.Clear();
		}

		public void Save(BinaryWriter bw)
		{
			bw.Write(Command);
			bw.Write(saveLasting);
			bw.Write(saveBgStr);
			bw.Write(saveX);
			bw.Write(saveY);
			bw.Write(saveWidth);
			bw.Write(saveHeight);
			bw.Write(saveLayer);
			bw.Write(saveTransformStyle);
			bw.Write(saveShocking);
		}

		public void Load(BinaryReader br)
		{
			Lasting = br.ReadBoolean();
			_bgStr = br.ReadString();
			_bounds.X = br.ReadInt32();
			_bounds.Y = br.ReadInt32();
			_bounds.Width = br.ReadInt32();
			_bounds.Height = br.ReadInt32();
			_layer = br.ReadInt32();
			string text = br.ReadString();
			if (text != string.Empty)
			{
				Type type = BgTransform.Types[text];
				_transformStyle = (TransformStyle<BgDisplay>)type.Assembly.CreateInstance(type.FullName);
				_transformStyle.Init(this);
				loading = true;
			}
			_shocking = br.ReadBoolean();
			LoadDataInfo();
			if (_layer == 0)
			{
				_speed = DefaultSpeed;
			}
			else
			{
				_speed = 0f;
			}
		}

		public void ConfirmUpdate()
		{
			saveLasting = Lasting;
			saveBgStr = _bgStr;
			saveX = _bounds.X;
			saveY = _bounds.Y;
			saveWidth = _bounds.Width;
			saveHeight = _bounds.Height;
			saveLayer = _layer;
			saveTransformStyle = ((_transformStyle == null) ? string.Empty : _transformStyle.Name);
			saveShocking = _shocking;
		}
	}
}
