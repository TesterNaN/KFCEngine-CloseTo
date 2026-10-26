using System.Drawing;
using System.Xml;
using KidFansChannel.GalCore;
using KidFansChannel.GalCore.Commands;
using KidFansChannel.GalExt.Effects;

namespace KidFansChannel.CloseTo.Effects
{
	public class CloseToFrame : Frame
	{
		private Color memoryColor = Color.FromArgb(192, 192, 160, 160);

		private bool _memory;

		private bool dataReady;

		public bool Memory
		{
			get
			{
				return _memory;
			}
			set
			{
				_memory = value;
			}
		}

		public override bool DataReady
		{
			get
			{
				if (dataReady)
				{
					return true;
				}
				if (PicFrame.IsCreated)
				{
					PicFrame.Scale = new SizeF((float)BgDisplay.ScreenSize.Width / (float)PicFrame.Width, (float)BgDisplay.ScreenSize.Height / (float)PicFrame.Height);
					dataReady = true;
					return true;
				}
				return false;
			}
		}

		protected override void InitStyle()
		{
		}

		public override void InitData()
		{
			PicFrame = ScriptCommand.Data.LoadPicture("system", "frame.png");
		}

		public override void ConvertXml(XmlNode node, NodeInfo info)
		{
			base.ConvertXml(node, info);
			_memory = ScriptHelper.GetArgumentBool(node, info, "memory", false);
		}

		public override void FinalData()
		{
			PicFrame.Dispose();
			PicFrame = null;
		}

		public override void InitCommand(float speed)
		{
			base.InitCommand(speed);
			if (!_memory)
			{
				return;
			}
			foreach (ScriptCommand command in ScriptCommand.CommandList)
			{
				if (command is BgDisplay)
				{
					((BgDisplay)command).Bg.ArgumentColor = memoryColor;
				}
				else if (command is CharaDisplay)
				{
					((CharaDisplay)command).Chara.ArgumentColor = memoryColor;
				}
			}
		}

		public override void FinalCommand(float speed)
		{
			base.FinalCommand(speed);
			if (!_memory)
			{
				return;
			}
			foreach (ScriptCommand command in ScriptCommand.CommandList)
			{
				if (command is BgDisplay)
				{
					((BgDisplay)command).Bg.ArgumentColor = Color.Transparent;
				}
				else if (command is CharaDisplay)
				{
					((CharaDisplay)command).Chara.ArgumentColor = Color.Transparent;
				}
			}
		}

		public override void Update(double appTime, float elaspedTime)
		{
			base.Update(appTime, elaspedTime);
			if (!_memory)
			{
				return;
			}
			foreach (ScriptCommand command in ScriptCommand.CommandList)
			{
				if (command is BgDisplay)
				{
					((BgDisplay)command).Bg.ArgumentColor = memoryColor;
				}
				else if (command is CharaDisplay)
				{
					((CharaDisplay)command).Chara.ArgumentColor = memoryColor;
				}
			}
		}
	}
}
