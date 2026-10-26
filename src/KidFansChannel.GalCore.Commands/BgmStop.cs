using System.Xml;

namespace KidFansChannel.GalCore.Commands
{
	public class BgmStop : ScriptCommand
	{
		private bool _animation;

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

		public BgmStop()
		{
			Single = false;
			Lasting = false;
			Command = "bgmstop";
		}

		public override void ConvertXml(XmlNode node, NodeInfo info)
		{
			base.ConvertXml(node, info);
			_animation = ScriptHelper.GetArgumentBool(node, info, "animation", true);
		}

		public override void InitCommand()
		{
			base.InitCommand();
			bool flag = false;
			foreach (ScriptCommand command in ScriptCommand.CommandList)
			{
				if (command is BgmPlay)
				{
					((BgmPlay)command).Stop(_animation);
					flag = true;
					break;
				}
			}
			FinishCommand();
			if (!flag)
			{
				throw new RuntimeException(Node, Info, "No bgm is playing now.");
			}
		}
	}
}
