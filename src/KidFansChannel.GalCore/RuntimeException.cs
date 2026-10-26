using System;
using System.Xml;

namespace KidFansChannel.GalCore
{
	public class RuntimeException : GameException
	{
		public RuntimeException(XmlNode node, NodeInfo info)
			: base(node, info)
		{
		}

		public RuntimeException(XmlNode node, NodeInfo info, string message)
			: base(node, info, message)
		{
		}

		public RuntimeException(XmlNode node, NodeInfo info, Exception innerException)
			: base(node, info, innerException)
		{
		}

		public RuntimeException(XmlNode node, NodeInfo info, string message, Exception innerException)
			: base(node, info, message, innerException)
		{
		}
	}
}
