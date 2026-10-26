using System.Xml;

namespace KidFansChannel.GalCore.Commands
{
	public class Par : ScriptCommand
	{
		public int Value;

		public Par()
		{
			Single = false;
			Lasting = false;
			Command = "par";
		}

		public override void ConvertXml(XmlNode node, NodeInfo info)
		{
			if (node.Name != Command)
			{
				throw new ScriptCommandException(node, info);
			}
			if (node.Attributes["value"] != null)
			{
				try
				{
					Value = int.Parse(node.Attributes["value"].Value);
					return;
				}
				catch
				{
					throw new ScriptArgumentException(node, info, "value");
				}
			}
			throw new ScriptArgumentException(node, info, "value");
		}
	}
}
