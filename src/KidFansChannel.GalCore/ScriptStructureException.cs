using System;
using System.Xml;

namespace KidFansChannel.GalCore
{
	public class ScriptStructureException : ScriptException
	{
		public ScriptStructureException(XmlNode node, NodeInfo info)
			: base(node, info, "Script's structure is error.")
		{
		}

		public ScriptStructureException(XmlNode node, NodeInfo info, string message)
			: base(node, info, message)
		{
		}

		public ScriptStructureException(XmlNode node, NodeInfo info, string message, Exception innerException)
			: base(node, info, message, innerException)
		{
		}

		public ScriptStructureException(XmlNode node, NodeInfo info, Exception innerException)
			: base(node, info, "Script's structure is error.", innerException)
		{
		}

		protected override string GenerateMessage()
		{
			string outerXml = Node.OuterXml;
			outerXml = outerXml.Substring(0, outerXml.IndexOf(">") + 1);
			return string.Format("File: {0}\nChapter: {1}\nScene {2} Click{3}\n{4}", Info.File, Info.Chapter, Info.Scene, Info.Click, outerXml);
		}
	}
}
