using System.IO;
using System.Xml;

namespace KidFansChannel.GalCore.Commands
{
	public class Timer : ScriptCommand, IUpdatable, ISaveable
	{
		private bool _start;

		private float _time;

		private float saveTime;

		public bool Start
		{
			get
			{
				return _start;
			}
			set
			{
				_start = value;
			}
		}

		public float Time
		{
			get
			{
				return _time;
			}
			set
			{
				_time = value;
			}
		}

		public Timer()
		{
			Single = false;
			Lasting = false;
			Command = "timer";
		}

		public override void ConvertXml(XmlNode node, NodeInfo info)
		{
			base.ConvertXml(node, info);
			_start = ScriptHelper.GetArgumentBool(node, info, "start");
		}

		public override void InitCommand()
		{
			base.InitCommand();
			Timer timer = null;
			foreach (ScriptCommand command in ScriptCommand.CommandList)
			{
				if (command is Timer && command != this)
				{
					timer = (Timer)command;
					break;
				}
			}
			if (_start)
			{
				if (timer != null)
				{
					FinishCommand();
				}
				if (ScriptCommand.GalCore.CurrentPlayMode != PlayMode.Normal && ScriptCommand.GalCore.CurrentPlayMode != PlayMode.Auto)
				{
					ScriptCommand.GalCore.CurrentPlayMode = PlayMode.Normal;
				}
			}
			else
			{
				if (timer == null)
				{
					throw new RuntimeException(Node, Info, "No timer is running.");
				}
				timer.FinishCommand();
				FinishCommand();
			}
		}

		public void Update(double appTime, float elaspedTime)
		{
			_time += elaspedTime;
		}

		public void Save(BinaryWriter bw)
		{
			bw.Write(Command);
			bw.Write(saveTime);
		}

		public void Load(BinaryReader br)
		{
			_time = br.ReadSingle();
			_start = true;
		}

		public void ConfirmUpdate()
		{
			saveTime = _time;
		}
	}
}
