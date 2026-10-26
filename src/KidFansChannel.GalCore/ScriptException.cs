using System;
using System.Xml;

namespace KidFansChannel.GalCore
{
	public class ScriptException : GameException
	{
		public ScriptException(XmlNode node, NodeInfo info)
			: base(node, info)
		{
		}

		public ScriptException(XmlNode node, NodeInfo info, string message)
			: base(node, info, message)
		{
		}

		public ScriptException(XmlNode node, NodeInfo info, Exception innerException)
			: base(node, info, innerException)
		{
		}

		public ScriptException(XmlNode node, NodeInfo info, string message, Exception innerException)
			: base(node, info, message, innerException)
		{
		}
	}
}
