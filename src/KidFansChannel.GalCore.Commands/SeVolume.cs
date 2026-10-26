using System.Xml;

namespace KidFansChannel.GalCore.Commands
{
	public class SeVolume : ScriptCommand
	{
		private float _volume;

		private bool _animation;

		private int _index;

		protected float Volume
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

		public SeVolume()
		{
			Single = false;
			Lasting = false;
			Command = "sevolume";
		}

		public override void ConvertXml(XmlNode node, NodeInfo info)
		{
			base.ConvertXml(node, info);
			_volume = ScriptHelper.GetArgumentFloat(node, info, "volume");
			_animation = ScriptHelper.GetArgumentBool(node, info, "animation", false);
			_index = ScriptHelper.GetArgumentInteger(node, info, "index", 0);
		}

		public override void InitCommand()
		{
			base.InitCommand();
			bool flag = false;
			foreach (ScriptCommand command in ScriptCommand.CommandList)
			{
				if (!(command is SePlay))
				{
					continue;
				}
				((SePlay)command).Volume = _volume;
				if (!((SePlay)command).Loop && _animation)
				{
					throw new RuntimeException(Node, Info, "Unloop Se's animation must be false.");
				}
				if (_animation)
				{
					ScriptCommand.Sound.ChangeVolumeSound(_volume, _index);
				}
				else if (((SePlay)command).HasFile)
				{
					ScriptCommand.Sound.SetVolumeSound(_volume, _index);
				}
				flag = true;
				break;
			}
			FinishCommand();
			if (!flag)
			{
				throw new RuntimeException(Node, Info, "No se is playing now.");
			}
		}
	}
}
