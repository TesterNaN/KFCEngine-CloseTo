using System.Xml;

namespace KidFansChannel.GalCore.Commands
{
	public class ParagraphClear : ScriptCommand
	{
		private bool _animation;

		public bool Animation
		{
			get
			{
				return _animation;
			}
			protected set
			{
				_animation = value;
			}
		}

		public ParagraphClear()
		{
			Single = true;
			Lasting = true;
			Command = "paragraphclear";
		}

		public override void ConvertXml(XmlNode node, NodeInfo info)
		{
			base.ConvertXml(node, info);
			_animation = ScriptHelper.GetArgumentBool(node, info, "animation", true);
		}

		public override void InitCommand()
		{
			base.InitCommand();
			foreach (ScriptCommand command in ScriptCommand.CommandList)
			{
				if (command is Paragraph)
				{
					((Paragraph)command).FinishString(_animation);
				}
			}
			FinishCommand();
		}
	}
}
