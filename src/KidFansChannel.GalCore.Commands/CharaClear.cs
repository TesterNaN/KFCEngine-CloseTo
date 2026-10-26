using System.Xml;

namespace KidFansChannel.GalCore.Commands
{
	public class CharaClear : ScriptCommand
	{
		private bool _animation;

		private int _index;

		public bool Animation
		{
			get
			{
				return _animation;
			}
			protected set
			{
				_animation = value;
			}
		}

		public int Index
		{
			get
			{
				return _index;
			}
			protected set
			{
				_index = value;
			}
		}

		public CharaClear()
		{
			Single = true;
			Lasting = true;
			Command = "characlear";
		}

		public override void ConvertXml(XmlNode node, NodeInfo info)
		{
			base.ConvertXml(node, info);
			_animation = ScriptHelper.GetArgumentBool(node, info, "animation", true);
			_index = ScriptHelper.GetArgumentInteger(node, info, "index");
			if (_index < -1)
			{
				throw new ScriptArgumentException(node, info, "index", ArgumentType.Integer, "Index should not less than -1.");
			}
			Single = _animation;
			Lasting = _animation;
		}

		public override void InitCommand()
		{
			base.InitCommand();
			int charaIndex = -1;
			for (int i = 0; i < ScriptCommand.CommandList.Count; i++)
			{
				ScriptCommand scriptCommand = ScriptCommand.CommandList[i];
				if (scriptCommand is CharaDisplay && ((CharaDisplay)scriptCommand).Index == _index)
				{
					charaIndex = i;
				}
			}
			if (charaIndex == -1)
			{
				throw new RuntimeException(Node, Info, "No corresponding charadisplay found.");
			}
			CharaDisplay charaDisplay = (CharaDisplay)ScriptCommand.CommandList[charaIndex];
			if (_animation)
			{
				charaDisplay.Hide(Lasting, Animation);
			}
			else
			{
				charaDisplay.FinishCommand();
			}
			FinishCommand();
		}
	}
}
