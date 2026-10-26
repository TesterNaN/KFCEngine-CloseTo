using System.Xml;

namespace KidFansChannel.GalCore.Commands
{
	public class EffectStop : ScriptCommand
	{
		private string _typeStr;

		private float _speed;

		private EffectStart effect;

		public string TypeStr
		{
			get
			{
				return _typeStr;
			}
			protected set
			{
				_typeStr = value;
			}
		}

		public float Speed
		{
			get
			{
				return _speed;
			}
			protected set
			{
				_speed = value;
			}
		}

		public EffectStop()
		{
			Single = false;
			Lasting = false;
			Command = "effectstop";
		}

		public override void ConvertXml(XmlNode node, NodeInfo info)
		{
			base.ConvertXml(node, info);
			_typeStr = ScriptHelper.GetArgumentString(node, info, "type");
			if (!EffectStart.Types.ContainsKey(_typeStr))
			{
				return;
			}
			_speed = ScriptHelper.GetArgumentFloat(node, info, "speed", 0.5f);
			bool flag = false;
			foreach (ScriptCommand command in ScriptCommand.CommandList)
			{
				if (command is EffectStart && ((EffectStart)command).TypeStr == _typeStr)
				{
					flag = ((EffectStart)command).Effect.SupportSpeed;
				}
			}
			Single = flag && _speed > 0f;
			Lasting = flag && _speed > 0f;
		}

		public override void InitCommand()
		{
			base.InitCommand();
			foreach (ScriptCommand command in ScriptCommand.CommandList)
			{
				if (command is EffectStart && ((EffectStart)command).TypeStr == _typeStr)
				{
					effect = (EffectStart)command;
					effect.Stop(_speed);
				}
			}
			FinishCommand();
		}
	}
}
