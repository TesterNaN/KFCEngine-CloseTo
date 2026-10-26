using System;
using System.Xml;
using KidFansChannel.GalCore.Commands;

namespace KidFansChannel.GalCore
{
	public static class ScriptHelper
	{
		public static string GetArgumentString(XmlNode node, NodeInfo info, string argumentName, string defaultValue)
		{
			if (node.Attributes[argumentName] == null)
			{
				if (defaultValue == null)
				{
					throw new ScriptArgumentException(node, info, argumentName);
				}
				return defaultValue;
			}
			return node.Attributes[argumentName].Value;
		}

		public static string GetArgumentString(XmlNode node, NodeInfo info, string argumentName)
		{
			return GetArgumentString(node, info, argumentName, null);
		}

		public static int GetArgumentInteger(XmlNode node, NodeInfo info, string argumentName, int? defaultValue)
		{
			if (node.Attributes[argumentName] == null)
			{
				if (!defaultValue.HasValue)
				{
					throw new ScriptArgumentException(node, info, argumentName);
				}
				return defaultValue.Value;
			}
			string value = node.Attributes[argumentName].Value;
			try
			{
				return int.Parse(value);
			}
			catch (Exception innerException)
			{
				throw new ScriptArgumentException(node, info, argumentName, ArgumentType.Integer, innerException);
			}
		}

		public static int GetArgumentInteger(XmlNode node, NodeInfo info, string argumentName)
		{
			return GetArgumentInteger(node, info, argumentName, null);
		}

		public static float GetArgumentFloat(XmlNode node, NodeInfo info, string argumentName, float? defaultValue)
		{
			if (node.Attributes[argumentName] == null)
			{
				if (!defaultValue.HasValue)
				{
					throw new ScriptArgumentException(node, info, argumentName);
				}
				return defaultValue.Value;
			}
			string value = node.Attributes[argumentName].Value;
			try
			{
				return float.Parse(value);
			}
			catch (Exception innerException)
			{
				throw new ScriptArgumentException(node, info, argumentName, ArgumentType.Float, innerException);
			}
		}

		public static float GetArgumentFloat(XmlNode node, NodeInfo info, string argumentName)
		{
			return GetArgumentFloat(node, info, argumentName, null);
		}

		public static bool GetArgumentBool(XmlNode node, NodeInfo info, string argumentName, bool? defaultValue)
		{
			if (node.Attributes[argumentName] == null)
			{
				if (!defaultValue.HasValue)
				{
					throw new ScriptArgumentException(node, info, argumentName);
				}
				return defaultValue.Value;
			}
			string value = node.Attributes[argumentName].Value;
			if (value == "true")
			{
				return true;
			}
			if (value == "false")
			{
				return false;
			}
			throw new ScriptArgumentException(node, info, argumentName, ArgumentType.Bool);
		}

		public static bool GetArgumentBool(XmlNode node, NodeInfo info, string argumentName)
		{
			return GetArgumentBool(node, info, argumentName, null);
		}
	}
}
