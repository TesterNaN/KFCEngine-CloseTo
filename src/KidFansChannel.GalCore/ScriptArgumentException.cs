using System;
using System.Xml;
using KidFansChannel.GalCore.Commands;

namespace KidFansChannel.GalCore
{
	public class ScriptArgumentException : ScriptException
	{
		private string _argumentName;

		private ArgumentType _type;

		public string ArgumentName
		{
			get
			{
				return _argumentName;
			}
			protected set
			{
				_argumentName = value;
			}
		}

		public ArgumentType Type
		{
			get
			{
				return _type;
			}
			protected set
			{
				_type = value;
			}
		}

		public ScriptArgumentException(XmlNode node, NodeInfo info, string argumentName)
			: this(node, info, argumentName, ArgumentType.None, "Attribute " + argumentName + " is error.", null)
		{
		}

		public ScriptArgumentException(XmlNode node, NodeInfo info, string argumentName, string message)
			: this(node, info, argumentName, ArgumentType.None, message, null)
		{
		}

		public ScriptArgumentException(XmlNode node, NodeInfo info, string argumentName, Exception innerException)
			: this(node, info, argumentName, ArgumentType.None, "Attribute " + argumentName + " is error.", innerException)
		{
		}

		public ScriptArgumentException(XmlNode node, NodeInfo info, string argumentName, string message, Exception innerException)
			: this(node, info, argumentName, ArgumentType.None, message, innerException)
		{
		}

		public ScriptArgumentException(XmlNode node, NodeInfo info, string argumentName, ArgumentType argumentType)
			: this(node, info, argumentName, argumentType, "Attribute " + argumentName + " is error.", null)
		{
		}

		public ScriptArgumentException(XmlNode node, NodeInfo info, string argumentName, ArgumentType argumentType, string message)
			: this(node, info, argumentName, argumentType, message, null)
		{
		}

		public ScriptArgumentException(XmlNode node, NodeInfo info, string argumentName, ArgumentType argumentType, Exception innerException)
			: this(node, info, argumentName, argumentType, "Attribute " + argumentName + " is error.", innerException)
		{
		}

		public ScriptArgumentException(XmlNode node, NodeInfo info, string argumentName, ArgumentType argumentType, string message, Exception innerException)
			: base(node, info, message, innerException)
		{
			ArgumentName = argumentName;
		}

		protected override string GenerateMessage()
		{
			XmlAttribute xmlAttribute = Node.Attributes[ArgumentName];
			string text = string.Format("{0}\nArgument: {1} Value: {2}", base.GenerateMessage(), ArgumentName, (xmlAttribute == null) ? "<NULL>" : xmlAttribute.Value);
			if (xmlAttribute == null)
			{
				text = "Argument should be appointed\n" + text;
			}
			else if (Type != ArgumentType.None)
			{
				switch (Type)
				{
				case ArgumentType.Bool:
					text = "Argument should be true or flase.\n" + text;
					break;
				case ArgumentType.Integer:
					text = "Argument should be a integer\n" + text;
					break;
				case ArgumentType.Float:
					text = "Argument should be a number\n" + text;
					break;
				}
			}
			return text;
		}
	}
}
