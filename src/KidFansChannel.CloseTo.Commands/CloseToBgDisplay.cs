using System.Xml;
using KidFansChannel.CloseTo.Commands.Room;
using KidFansChannel.GalCore;
using KidFansChannel.GalCore.Commands;

namespace KidFansChannel.CloseTo.Commands
{
	public class CloseToBgDisplay : BgDisplay
	{
		private RoomBg _room;

		public RoomBg Room
		{
			get
			{
				return _room;
			}
			set
			{
				_room = value;
			}
		}

		public override void ConvertXml(XmlNode node, NodeInfo info)
		{
			base.ConvertXml(node, info);
			if (ScriptHelper.GetArgumentBool(node, info, "room", false))
			{
				_room = new RoomBg(this);
			}
		}

		public override void InitCommand()
		{
			base.InitCommand();
			if (_room != null)
			{
				_room = new RoomBg(this);
				_room.Init();
			}
		}

		public override void FinalCommand()
		{
			if (_room != null)
			{
				_room.Final();
			}
			base.FinalCommand();
		}

		public override void Draw()
		{
			if (_room != null)
			{
				_room.Draw(ScriptCommand.Canvas);
			}
			else
			{
				base.Draw();
			}
		}
	}
}
