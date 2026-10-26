using System.Xml;

namespace KidFansChannel.GalCore.Commands
{
	public class SeStop : ScriptCommand
	{
		private bool _animation;

		private int _index;

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

		public SeStop()
		{
			Single = false;
			Lasting = false;
			Command = "sestop";
		}

		public override void ConvertXml(XmlNode node, NodeInfo info)
		{
			base.ConvertXml(node, info);
			_animation = ScriptHelper.GetArgumentBool(node, info, "animation", false);
			_index = ScriptHelper.GetArgumentInteger(node, info, "index", 0);
			if (_index < 0 || _index >= 2)
			{
				throw new ScriptArgumentException(node, info, "index", ArgumentType.Integer, "Index out of range.");
			}
		}

		public override void InitCommand()
		{
			base.InitCommand();
			foreach (ScriptCommand command in ScriptCommand.CommandList)
			{
				if (command is SePlay && ((SePlay)command).Index == _index)
				{
					if (!((SePlay)command).Loop && _animation)
					{
						throw new RuntimeException(Node, Info, "Unloop Se's animation must be false.");
					}
					((SePlay)command).Stop(_animation);
					break;
				}
			}
			FinishCommand();
		}
	}
}
