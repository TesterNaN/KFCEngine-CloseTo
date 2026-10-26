using System.Xml;

namespace KidFansChannel.GalCore.Commands
{
	public class DialogueClear : ScriptCommand
	{
		public DialogueClear()
		{
			Single = true;
			Lasting = true;
			Command = "dialogueclear";
		}

		public override void ConvertXml(XmlNode node, NodeInfo info)
		{
			base.ConvertXml(node, info);
		}

		public override void InitCommand()
		{
			base.InitCommand();
			foreach (ScriptCommand command in ScriptCommand.CommandList)
			{
				if (command is Dialogue)
				{
					((Dialogue)command).FinishString(true);
				}
			}
			FinishCommand();
		}
	}
}
