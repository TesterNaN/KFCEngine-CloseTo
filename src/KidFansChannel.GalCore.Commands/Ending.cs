using System.Xml;

namespace KidFansChannel.GalCore.Commands
{
	public abstract class Ending : ScriptCommand
	{
		private int _endingIndex;

		public int EndingIndex
		{
			get
			{
				return _endingIndex;
			}
			protected set
			{
				_endingIndex = value;
			}
		}

		public Ending()
		{
			Command = "ending";
			Single = false;
			Lasting = false;
		}

		public override void ConvertXml(XmlNode node, NodeInfo info)
		{
			base.ConvertXml(node, info);
			_endingIndex = ScriptHelper.GetArgumentInteger(node, info, "value", -1);
		}

		protected void UpdateEnding()
		{
			if (_endingIndex >= 0)
			{
				ScriptCommand.GameState.UpdateEnding(_endingIndex);
			}
			ScriptCommand.GalCore.GameState.ConfirmString();
		}
	}
}
