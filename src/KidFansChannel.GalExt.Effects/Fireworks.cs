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
	public class Fireworks : Effect
	{
		protected enum FireworksState
		{
			Showing,
			Hiding,
			Hiden
		}

		private bool hasSound;

		public Picture PicFireworks;

		public static string FireworksStr = "se11_01";

		protected double NextTime;

		protected Random random = new Random((int)DateTime.Now.Ticks);

		protected float Alpha;

		protected FireworksState State;

		public override bool DataReady
		{
			get
			{
				if (hasSound)
				{
					return ScriptCommand.Sound.PreparedSound(8);
				}
				return true;
			}
		}

		public override float Layer
		{
			get
			{
				return 10f;
			}
		}

		public Fireworks()
		{
			Single = false;
			Lasting = false;
			SupportSpeed = false;
			Name = "fireworks";
		}

		public override void ConvertXml(XmlNode node, NodeInfo info)
		{
		}

		public override void InitCommand(float speed)
		{
			NextTime = random.NextDouble() * (3.0 + random.NextDouble() * 2.0 - 1.0);
			State = FireworksState.Hiden;
			Alpha = 0f;
			PicFireworks.Alpha = 0;
			PicFireworks.ArgumentColor = Color.FromArgb(0, Color.FromArgb(random.Next()));
		}

		public override void FinalCommand(float speed)
		{
			Finished = true;
		}

		public override void InitData()
		{
			PicFireworks = ScriptCommand.Data.LoadPicture("system", "fireworks.png");
			switch (ScriptCommand.Data.CheckData("se", FireworksStr + ".ogg", KidFansChannel.DataCore.DataCore.DataType.Package))
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
			if (PicFireworks != null)
			{
				PicFireworks.Dispose();
				PicFireworks = null;
			}
			if (hasSound)
			{
				ScriptCommand.Sound.CloseSound(8);
			}
		}

		public override void Update(double appTime, float elaspedTime)
		{
			switch (State)
			{
			case FireworksState.Showing:
				Alpha += elaspedTime / 1.5f;
				if (Alpha > 1f)
				{
					Alpha = 1f;
					State = FireworksState.Hiding;
				}
				PicFireworks.Alpha = (int)(Alpha * 255f + 0.5f);
				break;
			case FireworksState.Hiding:
				Alpha -= elaspedTime / 1.5f;
				if (Alpha < 0f)
				{
					Alpha = 0f;
					State = FireworksState.Hiden;
					NextTime = 3.0 + random.NextDouble() * 2.0 - 1.0;
				}
				PicFireworks.Alpha = (int)(Alpha * 255f + 0.5f);
				break;
			case FireworksState.Hiden:
				NextTime -= elaspedTime;
				if (NextTime < 0.0)
				{
					NextTime = 0.0;
					PicFireworks.ArgumentColor = Color.FromArgb(255, Color.FromArgb(random.Next()));
					State = FireworksState.Showing;
					if (hasSound)
					{
						ScriptCommand.Sound.PlaySound(false, false, 8);
						ScriptCommand.Sound.SetVolumeSound(1f, 8);
					}
				}
				break;
			}
		}

		public override void Draw()
		{
			if (ScriptCommand.Options.Graphics != GameOptions.Level.Low)
			{
				ScriptCommand.Canvas.DrawPicture(PicFireworks);
			}
		}

		public override void Save(BinaryWriter bw)
		{
			bw.Write(Name);
		}

		public override void Load(BinaryReader br)
		{
		}

		public override void ConfirmUpdate()
		{
		}
	}
}
