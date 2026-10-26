using System;
using KidFansChannel.GalCore.Commands;
using KidFansChannel.GalExt.Effects;

namespace KidFansChannel.CloseTo.Effects
{
	internal class CloseToRain : Rain
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
				if (PicRain.IsCreated)
				{
					dataReady = true;
					return true;
				}
				return false;
			}
		}

		public CloseToRain()
		{
			TypeNum = 18;
			LengthMin = 187.5f;
			LengthMax = 250f;
			Speed = 10f;
		}

		protected override void InitStyle()
		{
			int num = Style % 9;
			int num2 = num % 3;
			int num3 = num / 3;
			switch (num2)
			{
			case 0:
				AngleMin = -13f;
				AngleMax = -7f;
				break;
			case 1:
				AngleMin = -3f;
				AngleMax = -3f;
				break;
			case 2:
				AngleMin = 7f;
				AngleMax = 13f;
				break;
			}
			AngleMax *= (float)Math.PI / 180f;
			AngleMin *= (float)Math.PI / 180f;
			switch (num3)
			{
			case 0:
				Num = 170;
				break;
			case 1:
				Num = 341;
				break;
			case 2:
				Num = 682;
				break;
			}
			Back = Style >= 9;
		}

		public override void InitData()
		{
			PicRain = ScriptCommand.Data.LoadPicture("system", "rainline.png");
		}

		public override void FinalData()
		{
			PicRain.Dispose();
		}
	}
}
