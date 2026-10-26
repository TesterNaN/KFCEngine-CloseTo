using System;
using System.Xml;

namespace KidFansChannel.GalCore
{
	public class ScriptCommandException : ScriptException
	{
		public string CommandName
		{
			get
			{
				return Node.Name;
			}
		}

		public ScriptCommandException(XmlNode node, NodeInfo info)
			: base(node, info, "Command " + node.Name + " is error.")
		{
		}

		public ScriptCommandException(XmlNode node, NodeInfo info, string message)
			: base(node, info, message)
		{
		}

		public ScriptCommandException(XmlNode node, NodeInfo info, string message, Exception innerException)
			: base(node, info, message, innerException)
		{
		}

		public ScriptCommandException(XmlNode node, NodeInfo info, Exception innerException)
			: base(node, info, "Command " + node.Name + " is error.", innerException)
		{
		}

		protected override string GenerateMessage()
		{
			return string.Format("{0}\nCommand: {1}", base.GenerateMessage(), Node.Name);
		}
	}
}
