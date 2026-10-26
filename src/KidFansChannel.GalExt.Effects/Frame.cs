using System.IO;
using System.Xml;
using KidFansChannel.GalCore;
using KidFansChannel.GalCore.Commands;
using KidFansChannel.GalCore.ExtBases;
using KidFansChannel.Game2DCore;

namespace KidFansChannel.GalExt.Effects
{
	public abstract class Frame : Effect
	{
		private int _style;

		protected int TypeNum = 1;

		protected Picture PicFrame;

		public int Style
		{
			get
			{
				return _style;
			}
			set
			{
				_style = value;
			}
		}

		public override float Layer
		{
			get
			{
				return 19f;
			}
		}

		public Frame()
		{
			Single = true;
			Lasting = true;
			Name = "frame";
			SupportSpeed = true;
		}

		public override void ConvertXml(XmlNode node, NodeInfo info)
		{
			Single = ScriptHelper.GetArgumentFloat(node, info, "speed", 0.5f) > 0f;
			_style = ScriptHelper.GetArgumentInteger(node, info, "style", 0);
			if (_style >= TypeNum)
			{
				throw new ScriptArgumentException(node, info, "style", "Style's number is not defined.");
			}
			InitStyle();
		}

		protected abstract void InitStyle();

		public override void InitCommand(float speed)
		{
			if (speed > 0f)
			{
				PicFrame.Alpha = 0;
				PicFrame.BeginTransform(speed);
				PicFrame.AlphaTo(255);
				PicFrame.EndTransform();
			}
			else
			{
				PicFrame.Alpha = 255;
			}
		}

		public override void FinalCommand(float speed)
		{
			if (speed > 0f)
			{
				PicFrame.BeginTransform(speed);
				PicFrame.AlphaTo(0);
				PicFrame.EndTransform();
			}
			else
			{
				PicFrame.Alpha = 0;
			}
		}

		public override void Update(double appTime, float elaspedTime)
		{
			if (ScriptCommand.PlayModeQuicking)
			{
				PicFrame.Update(elaspedTime * (float)ScriptCommand.QuickFactor);
			}
			else
			{
				PicFrame.Update(elaspedTime);
			}
			if (PicFrame.Time == 0f)
			{
				Lasting = false;
				if (PicFrame.Alpha == 0)
				{
					PicFrame.Dispose();
					Finished = true;
				}
			}
		}

		public override void Draw()
		{
			ScriptCommand.Graphics.DrawPicture(PicFrame);
		}

		public override void Save(BinaryWriter bw)
		{
			bw.Write(Name);
			bw.Write(_style);
		}

		public override void Load(BinaryReader br)
		{
			_style = br.ReadInt32();
		}

		public override void ConfirmUpdate()
		{
		}
	}
}
