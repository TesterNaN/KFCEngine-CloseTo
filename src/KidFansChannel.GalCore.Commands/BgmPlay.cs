using System.IO;
using System.Xml;
using KidFansChannel.DataCore;

namespace KidFansChannel.GalCore.Commands
{
	public class BgmPlay : ScriptCommand, IInstanceDataUser, IUpdatable, ISaveable
	{
		private string _bgmStr;

		private float _position;

		private bool _continue;

		private float _volume;

		private bool _animation;

		private string _bgmName;

		private float _repeatPosition;

		private int _musicId;

		private bool hasFile;

		private bool finishing;

		private bool saveFinishing;

		private bool saveLasting;

		private string saveBgmStr;

		private float saveVolume;

		private float savePosition;

		protected string BgmStr
		{
			get
			{
				return _bgmStr;
			}
			set
			{
				_bgmStr = value;
			}
		}

		protected float Position
		{
			get
			{
				return _position;
			}
			set
			{
				_position = value;
			}
		}

		public bool Continue
		{
			get
			{
				return _continue;
			}
			protected set
			{
				_continue = value;
			}
		}

		public float Volume
		{
			get
			{
				return _volume;
			}
			set
			{
				_volume = value;
			}
		}

		public bool Animation
		{
			get
			{
				return _animation;
			}
			set
			{
				_animation = value;
			}
		}

		public string BgmName
		{
			get
			{
				return _bgmName;
			}
			protected set
			{
				_bgmName = value;
			}
		}

		protected float RepeatPosition
		{
			get
			{
				return _repeatPosition;
			}
			set
			{
				_repeatPosition = value;
			}
		}

		public int MusicId
		{
			get
			{
				return _musicId;
			}
			set
			{
				_musicId = value;
			}
		}

		public bool DataReady
		{
			get
			{
				if (hasFile)
				{
					return ScriptCommand.Sound.PreparedBgm;
				}
				return true;
			}
		}

		public BgmPlay()
		{
			Single = false;
			Lasting = false;
			Command = "bgmplay";
			DataName = "bgm";
		}

		public override void ConvertXml(XmlNode node, NodeInfo info)
		{
			base.ConvertXml(node, info);
			_bgmStr = ScriptHelper.GetArgumentString(node, info, "file");
			_position = ScriptHelper.GetArgumentFloat(node, info, "position", 0f);
			_volume = ScriptHelper.GetArgumentFloat(node, info, "volume", 1f);
			_continue = ScriptHelper.GetArgumentBool(node, info, "continue", false);
			_animation = ScriptHelper.GetArgumentBool(node, info, "animation", true);
			LoadDataInfo();
		}

		protected void LoadDataInfo()
		{
			XmlDocument xmlDocument = new XmlDocument();
			_bgmName = null;
			_repeatPosition = 0f;
			_musicId = -1;
			if (!ScriptCommand.Data.LoadXmlDocument(DataName, _bgmStr + ".xml", xmlDocument))
			{
				return;
			}
			XmlNode documentElement = xmlDocument.DocumentElement;
			if (documentElement.Name != "bgm")
			{
				throw new ScriptStructureException(documentElement, new NodeInfo(), "Bgm's document element should be bgm.");
			}
			XmlNode xmlNode;
			if ((xmlNode = documentElement.SelectSingleNode("name")) != null)
			{
				if (!xmlNode.HasChildNodes || xmlNode.ChildNodes.Count != 1)
				{
					throw new ScriptStructureException(xmlNode, new NodeInfo(), "Name should have only 1 child node.");
				}
				_bgmName = xmlNode.FirstChild.Value;
			}
			if ((xmlNode = documentElement.SelectSingleNode("repeatposition")) != null)
			{
				_repeatPosition = ScriptHelper.GetArgumentFloat(xmlNode, new NodeInfo(), "value");
			}
			if ((xmlNode = documentElement.SelectSingleNode("music")) != null)
			{
				_musicId = ScriptHelper.GetArgumentInteger(xmlNode, new NodeInfo(), "id");
				if (_musicId < 0)
				{
					throw new ScriptArgumentException(xmlNode, new NodeInfo(), "id", ArgumentType.Integer, "Id should not less than 0.");
				}
				ScriptCommand.GameState.UpdateMusic(_musicId);
			}
		}

		public override void InitCommand()
		{
			base.InitCommand();
			foreach (ScriptCommand command in ScriptCommand.CommandList)
			{
				if (command is BgmPlay && command != this)
				{
					if (_continue)
					{
						FinishCommand();
						return;
					}
					command.FinishCommand();
					break;
				}
			}
			if (hasFile)
			{
				if (_animation)
				{
					ScriptCommand.Sound.VolumeBgm = 0f;
					ScriptCommand.Sound.ChangeVolumeBgm(_volume);
				}
				else
				{
					ScriptCommand.Sound.VolumeBgm = _volume;
				}
				ScriptCommand.Sound.PlayBgm(_position, _repeatPosition);
			}
		}

		public override void FinishCommand()
		{
			base.FinishCommand();
			ScriptCommand.Sound.StopBgm();
		}

		public void Stop(bool animation)
		{
			if (!hasFile)
			{
				finishing = true;
				FinishCommand();
			}
			else if (animation)
			{
				ScriptCommand.Sound.ChangeVolumeBgm(0f);
				finishing = true;
			}
			else
			{
				ScriptCommand.Sound.VolumeBgm = 0f;
				FinishCommand();
			}
		}

		public void InitInstanceData()
		{
			switch (ScriptCommand.Data.CheckData(DataName, _bgmStr + ".ogg", KidFansChannel.DataCore.DataCore.DataType.File))
			{
			case KidFansChannel.DataCore.DataCore.DataType.File:
				ScriptCommand.Sound.OpenBgm(ScriptCommand.Data.GetFile());
				hasFile = true;
				break;
			case KidFansChannel.DataCore.DataCore.DataType.Package:
				ScriptCommand.Sound.OpenBgm(ScriptCommand.Data.GetStream());
				hasFile = true;
				break;
			default:
				hasFile = false;
				break;
			}
		}

		public void FinalInstanceData()
		{
		}

		public void Update(double appTime, float elaspedTime)
		{
			if (!hasFile)
			{
				if (finishing)
				{
					FinishCommand();
				}
			}
			else if (finishing && ScriptCommand.Sound.VolumeBgm == 0f)
			{
				FinishCommand();
			}
		}

		public void Save(BinaryWriter bw)
		{
			if (!saveFinishing)
			{
				bw.Write(Command);
				bw.Write(saveLasting);
				bw.Write(saveBgmStr);
				bw.Write(saveVolume);
				bw.Write(savePosition);
			}
		}

		public void Load(BinaryReader br)
		{
			Lasting = br.ReadBoolean();
			_bgmStr = br.ReadString();
			_volume = br.ReadSingle();
			_position = br.ReadSingle();
			LoadDataInfo();
		}

		public void ConfirmUpdate()
		{
			saveFinishing = finishing;
			saveLasting = Lasting;
			saveBgmStr = _bgmStr;
			saveVolume = _volume;
			savePosition = _position;
		}
	}
}
