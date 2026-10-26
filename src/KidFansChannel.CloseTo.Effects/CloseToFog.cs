using System.Drawing;
using KidFansChannel.GalCore.Commands;
using KidFansChannel.GalExt.Effects;

namespace KidFansChannel.CloseTo.Effects
{
	public class CloseToFog : Fog
	{
		private bool dataReady;

		public override bool DataReady
		{
			get
			{
				if (dataReady)
				{
					return true;
				}
				if (PicFogs[0].IsCreated && PicFogs[1].IsCreated)
				{
					for (int i = 0; i < 2; i++)
					{
						PicFogs[i].Alpha = 64;
						PicFogs[i].Scale = new SizeF((float)BgDisplay.ScreenSize.Width / (float)PicFogs[i].Width, (float)BgDisplay.ScreenSize.Height / (float)PicFogs[i].Height);
					}
					dataReady = true;
					return true;
				}
				return false;
			}
		}

		public CloseToFog()
		{
			TypeNum = 2;
		}

		public override void InitData()
		{
			for (int i = 0; i < 2; i++)
			{
				PicFogs[i] = ScriptCommand.Data.LoadPicture("system", "fog.png");
			}
		}

		public override void FinalData()
		{
			for (int i = 0; i < 2; i++)
			{
				PicFogs[i].Dispose();
				PicFogs[i] = null;
			}
		}

		protected override void InitStyle()
		{
			switch (Style)
			{
			case 0:
				Direction = true;
				Speed = 800f;
				break;
			case 1:
				Direction = false;
				Speed = 80f;
				break;
			}
		}
	}
}
