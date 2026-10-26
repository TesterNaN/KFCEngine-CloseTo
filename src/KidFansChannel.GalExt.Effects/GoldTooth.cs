using System;
using System.Drawing;
using System.IO;
using System.Xml;
using KidFansChannel.DataCore;
using KidFansChannel.GalCore;
using KidFansChannel.GalCore.Commands;
using KidFansChannel.GalCore.ExtBases;
using KidFansChannel.Game2DCore;

namespace KidFansChannel.GalExt.Effects
{
	public class GoldTooth : Effect
	{
		private float _x;

		private float _y;

		public Picture PicGoldTooth;

		public static string SoundStr = "se14_39";

		private bool hasSound;

		public float X
		{
			get
			{
				return _x;
			}
			set
			{
				_x = value;
			}
		}

		public float Y
		{
			get
			{
				return _y;
			}
			set
			{
				_y = value;
			}
		}

		public override float Layer
		{
			get
			{
				return 2f;
			}
		}

		public override bool DataReady
		{
			get
			{
				if (!hasSound || ScriptCommand.Sound.PreparedSound(8))
				{
					return PicGoldTooth.IsCreated;
				}
				return false;
			}
		}

		public GoldTooth()
		{
			Single = false;
			Lasting = false;
			SupportSpeed = false;
			Name = "goldtooth";
		}

		public override void ConvertXml(XmlNode node, NodeInfo info)
		{
			_x = ScriptHelper.GetArgumentFloat(node, info, "x");
			_y = ScriptHelper.GetArgumentFloat(node, info, "y");
		}

		public override void InitCommand(float speed)
		{
			PicGoldTooth.X = (int)(_x * (float)BgDisplay.ScreenSize.Width);
			PicGoldTooth.Y = (int)(_y * (float)BgDisplay.ScreenSize.Height);
			PicGoldTooth.CenterX = PicGoldTooth.Width / 2;
			PicGoldTooth.CenterY = PicGoldTooth.Height / 2;
			PicGoldTooth.Alpha = 0;
			PicGoldTooth.BeginTransform(0.8f);
			PicGoldTooth.ScaleTo(new SizeF(1.25f, 1.25f), new SizeF(1f, 1f), 0.8f);
			PicGoldTooth.AlphaTo(255, 0, 0.8f);
			PicGoldTooth.RotateTo((float)Math.PI * 3f / 4f);
			PicGoldTooth.EndTransform();
			if (hasSound)
			{
				ScriptCommand.Sound.PlaySound(false, 8);
				ScriptCommand.Sound.SetVolumeSound(1f, 8);
			}
		}

		public override void FinalCommand(float speed)
		{
			Finished = true;
		}

		public override void InitData()
		{
			PicGoldTooth = ScriptCommand.Data.LoadPicture("system", "goldtooth.png");
			switch (ScriptCommand.Data.CheckData("se", SoundStr + ".ogg", KidFansChannel.DataCore.DataCore.DataType.Package))
			{
			case KidFansChannel.DataCore.DataCore.DataType.Package:
				ScriptCommand.Sound.OpenSound(ScriptCommand.Data.GetStream(), 8);
				hasSound = true;
				break;
			case KidFansChannel.DataCore.DataCore.DataType.File:
				ScriptCommand.Sound.OpenSound(ScriptCommand.Data.GetFile(), 8);
				hasSound = true;
				break;
			default:
				hasSound = false;
				break;
			}
		}

		public override void FinalData()
		{
			if (PicGoldTooth != null)
			{
				PicGoldTooth.Dispose();
				PicGoldTooth = null;
			}
			if (hasSound)
			{
				ScriptCommand.Sound.CloseSound(8);
			}
		}

		public override void Load(BinaryReader br)
		{
		}

		public override void Save(BinaryWriter bw)
		{
		}

		public override void ConfirmUpdate()
		{
		}

		public override void Update(double appTime, float elaspedTime)
		{
			if (ScriptCommand.PlayModeQuicking)
			{
				PicGoldTooth.Update(elaspedTime * (float)ScriptCommand.QuickFactor);
			}
			else
			{
				PicGoldTooth.Update(elaspedTime);
			}
			if (PicGoldTooth.Time == 0f)
			{
				Finished = true;
			}
		}

		public override void Draw()
		{
			ScriptCommand.Canvas.DrawPicture(PicGoldTooth);
		}
	}
}
