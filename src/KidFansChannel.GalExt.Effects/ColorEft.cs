using System.Drawing;
using System.Globalization;
using System.IO;
using System.Xml;
using KidFansChannel.GalCore;
using KidFansChannel.GalCore.Commands;
using KidFansChannel.GalCore.ExtBases;
using KidFansChannel.Game2DCore;

namespace KidFansChannel.GalExt.Effects
{
	public class ColorEft : Effect
	{
		private Color _value;

		protected Picture PicColor;

		public Color Value
		{
			get
			{
				return _value;
			}
			protected set
			{
				_value = value;
			}
		}

		public override bool DataReady
		{
			get
			{
				return true;
			}
		}

		public override float Layer
		{
			get
			{
				return 22f;
			}
		}

		public ColorEft()
		{
			Single = true;
			Lasting = true;
			Name = "color";
			SupportSpeed = true;
		}

		public override void ConvertXml(XmlNode node, NodeInfo info)
		{
			string argumentString = ScriptHelper.GetArgumentString(node, info, "color");
			int alpha = 0;
			int red = 0;
			int green = 0;
			int blue = 0;
			if (argumentString.Length > 6)
			{
				alpha = int.Parse(argumentString.Substring(0, argumentString.Length - 6), NumberStyles.HexNumber);
			}
			red = int.Parse(argumentString.Substring(argumentString.Length - 6, 2), NumberStyles.HexNumber);
			green = int.Parse(argumentString.Substring(argumentString.Length - 4, 2), NumberStyles.HexNumber);
			blue = int.Parse(argumentString.Substring(argumentString.Length - 2, 2), NumberStyles.HexNumber);
			_value = Color.FromArgb(alpha, red, green, blue);
			Single = ScriptHelper.GetArgumentFloat(node, info, "speed", 0.5f) > 0f;
		}

		public override void InitData()
		{
		}

		public override void FinalData()
		{
		}

		public override void InitCommand(float speed)
		{
			PicColor = ScriptCommand.Graphics.CreatePicture(Value);
			PicColor.Size = BgDisplay.ScreenSize;
			if (speed > 0f)
			{
				PicColor.Alpha = 0;
				PicColor.BeginTransform(speed);
				PicColor.AlphaTo(255);
				PicColor.EndTransform();
			}
			else
			{
				PicColor.Alpha = 255;
			}
		}

		public override void FinalCommand(float speed)
		{
			if (speed > 0f)
			{
				PicColor.BeginTransform(speed);
				PicColor.AlphaTo(0);
				PicColor.EndTransform();
			}
			else
			{
				PicColor.Alpha = 0;
			}
		}

		public override void Update(double appTime, float elaspedTime)
		{
			if (ScriptCommand.PlayModeQuicking)
			{
				PicColor.Update(elaspedTime * (float)ScriptCommand.QuickFactor);
			}
			else
			{
				PicColor.Update(elaspedTime);
			}
			if (PicColor.Time == 0f)
			{
				Lasting = false;
				if (PicColor.Alpha == 0)
				{
					PicColor.Dispose();
					Finished = true;
				}
			}
		}

		public override void Draw()
		{
			ScriptCommand.Graphics.DrawPicture(PicColor);
		}

		public override void Save(BinaryWriter bw)
		{
			bw.Write(Name);
			bw.Write(_value.ToArgb());
		}

		public override void Load(BinaryReader br)
		{
			_value = Color.FromArgb(br.ReadInt32());
		}

		public override void ConfirmUpdate()
		{
		}
	}
}
