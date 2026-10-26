using System;
using System.Xml;

namespace KidFansChannel.GalCore
{
	public class GameException : Exception
	{
		private XmlNode _node;

		private NodeInfo _info;

		protected XmlNode Node
		{
			get
			{
				return _node;
			}
			set
			{
				_node = value;
			}
		}

		protected NodeInfo Info
		{
			get
			{
				return _info;
			}
			set
			{
				_info = value;
			}
		}

		public override string Message
		{
			get
			{
				return GenerateMessage();
			}
		}

		public GameException(XmlNode node, NodeInfo info)
			: this(node, info, "", null)
		{
		}

		public GameException(XmlNode node, NodeInfo info, string message)
			: this(node, info, message, null)
		{
		}

		public GameException(XmlNode node, NodeInfo info, Exception innerException)
			: this(node, info, "", innerException)
		{
		}

		public GameException(XmlNode node, NodeInfo info, string message, Exception innerException)
			: base(message, innerException)
		{
			Node = node;
			Info = info;
		}

		protected virtual string GenerateMessage()
		{
			return string.Format("File: {0}\nChapter: {1}\nScene {2} Click{3}\n{4}\n{5}", Info.File, Info.Chapter, Info.Scene, Info.Click, (Node == null) ? "" : Node.OuterXml, base.Message);
		}
	}
}
