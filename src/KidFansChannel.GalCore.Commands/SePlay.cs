using System.IO;
using System.Xml;
using KidFansChannel.DataCore;

namespace KidFansChannel.GalCore.Commands
{
	public class SePlay : ScriptCommand, IInstanceDataUser, IUpdatable, ISaveable
	{
		private string _seStr;

		private bool _loop;

		private float _volume;

		private bool _animation;

		private int _index;

		private float _repeatPosition;

		private bool _hasFile;

		private bool finishing;

		private bool saveLasting;

		private string saveSeStr;

		private float saveVolume;

		private int saveIndex;

		public string SeStr
		{
			get
			{
				return _seStr;
			}
			set
			{
				_seStr = value;
			}
		}

		public bool Loop
		{
			get
			{
				return _loop;
			}
			protected set
			{
				_loop = value;
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

		public int Index
		{
			get
			{
				return _index;
			}
			set
			{
				_index = value;
			}
		}

		public bool HasFile
		{
			get
			{
				return _hasFile;
			}
		}

		public bool DataReady
		{
			get
			{
				if (_hasFile)
				{
					return ScriptCommand.Sound.PreparedSound(_index);
				}
				return true;
			}
		}

		public SePlay()
		{
			Single = true;
			Lasting = true;
			Command = "seplay";
			DataName = "se";
		}

		public override void ConvertXml(XmlNode node, NodeInfo info)
		{
			base.ConvertXml(node, info);
			_seStr = ScriptHelper.GetArgumentString(node, info, "file");
			_loop = ScriptHelper.GetArgumentBool(node, info, "loop", false);
			_volume = ScriptHelper.GetArgumentFloat(node, info, "volume", 1f);
			_animation = ScriptHelper.GetArgumentBool(node, info, "animation", !_loop);
			_index = ScriptHelper.GetArgumentInteger(node, info, "index", 0);
			if (_index < 0 || _index >= 2)
			{
				throw new ScriptArgumentException(node, info, "index", ArgumentType.Integer, "Index out of range.");
			}
			Single = !_loop && _animation;
			Lasting = !_loop && _animation;
			LoadDataInfo();
		}

		protected void LoadDataInfo()
		{
			XmlDocument xmlDocument = new XmlDocument();
			_repeatPosition = 0f;
			if (ScriptCommand.Data.LoadXmlDocument(DataName, _seStr + ".xml", xmlDocument))
			{
				XmlNode documentElement = xmlDocument.DocumentElement;
				if (documentElement.Name != "se")
				{
					throw new ScriptStructureException(documentElement, new NodeInfo(), "Se's document element should be se.");
				}
				XmlNode node;
				if ((node = documentElement.SelectSingleNode("repeatposition")) != null)
				{
					_repeatPosition = ScriptHelper.GetArgumentFloat(node, new NodeInfo(), "value");
				}
			}
		}

		public override void InitCommand()
		{
			base.InitCommand();
			foreach (ScriptCommand command in ScriptCommand.CommandList)
			{
				if (command is SePlay && ((SePlay)command).Index == _index && command != this)
				{
					((SePlay)command)._hasFile = false;
					command.FinishCommand();
					break;
				}
			}
			if (_hasFile)
			{
				if (_loop && _animation)
				{
					ScriptCommand.Sound.SetVolumeSound(0f, _index);
					ScriptCommand.Sound.ChangeVolumeSound(_volume, _index);
				}
				else
				{
					ScriptCommand.Sound.SetVolumeSound(_volume, _index);
				}
				ScriptCommand.Sound.PlaySound(_loop, _repeatPosition, _index);
			}
		}

		public override void FinishCommand()
		{
			base.FinishCommand();
			if (_hasFile)
			{
				ScriptCommand.Sound.StopSound(_index);
			}
		}

		public void Stop(bool animation)
		{
			if (_hasFile && animation)
			{
				ScriptCommand.Sound.ChangeVolumeSound(0f, _index);
				finishing = true;
				return;
			}
			if (_hasFile)
			{
				ScriptCommand.Sound.SetVolumeSound(0f, _index);
			}
			FinishCommand();
		}

		public void InitInstanceData()
		{
			switch (ScriptCommand.Data.CheckData("se", _seStr + ".ogg", KidFansChannel.DataCore.DataCore.DataType.Package))
			{
			case KidFansChannel.DataCore.DataCore.DataType.Package:
				ScriptCommand.Sound.OpenSound(ScriptCommand.Data.GetStream(), _index);
				_hasFile = true;
				break;
			case KidFansChannel.DataCore.DataCore.DataType.File:
				ScriptCommand.Sound.OpenSound(ScriptCommand.Data.GetFile(), _index);
				_hasFile = true;
				break;
			default:
				_hasFile = false;
				break;
			}
		}

		public void FinalInstanceData()
		{
		}

		public void Update(double appTime, float elaspedTime)
		{
			if (!_hasFile)
			{
				if (!_loop)
				{
					FinishCommand();
				}
			}
			else if (!_loop)
			{
				if (!ScriptCommand.Sound.IsPlayingSound(_index))
				{
					FinishCommand();
				}
				if (ScriptCommand.PlayModeQuicking)
				{
					Lasting = false;
				}
			}
			else if (finishing && ScriptCommand.Sound.GetVolumeSound(_index) == 0f)
			{
				FinishCommand();
			}
		}

		public void Save(BinaryWriter bw)
		{
			if (_loop)
			{
				bw.Write(Command);
				bw.Write(saveLasting);
				bw.Write(saveSeStr);
				bw.Write(saveVolume);
				bw.Write(saveIndex);
			}
		}

		public void Load(BinaryReader br)
		{
			Lasting = br.ReadBoolean();
			_seStr = br.ReadString();
			_volume = br.ReadSingle();
			_index = br.ReadInt32();
			_loop = true;
			LoadDataInfo();
		}

		public void ConfirmUpdate()
		{
			saveLasting = Lasting;
			saveSeStr = SeStr;
			saveVolume = _volume;
			saveIndex = _index;
		}
	}
}
