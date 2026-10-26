using System.Drawing;
using KidFansChannel.GalCore.Commands;
using KidFansChannel.GalExt.Effects;
using KidFansChannel.Game2DCore;

namespace KidFansChannel.CloseTo.Effects
{
	internal class CloseToSun : Sun
	{
		private Picture PicSun;

		private bool dataReady;

		public override bool DataReady
		{
			get
			{
				if (dataReady)
				{
					return true;
				}
				if (PicSun.IsCreated)
				{
					for (int i = 0; i < TypeNum; i++)
					{
						PicSuns[i] = ScriptCommand.Graphics.CreateSubPicture(PicSun, new Rectangle(0, 448 * i, 320, 448));
						PicSuns[i].Center = new Point(((i & 1) != 0) ? 320 : 0, 0);
					}
					dataReady = true;
					return true;
				}
				return false;
			}
		}

		public CloseToSun()
		{
			TypeNum = 4;
			ShowHideTime = 1f;
		}

		protected override void InitStyle()
		{
			switch (Style)
			{
			case 0:
			case 2:
				XMax = 150;
				XMin = -50;
				break;
			case 1:
			case 3:
				XMax = 850;
				XMin = 650;
				break;
			}
			YMax = 0;
			YMin = -224;
			AlphaMax = 255;
			AlphaMin = 192;
			TimeMax = 10f;
			TimeMin = 5f;
			NextMax = 5f;
			NextMin = 0f;
		}

		public override void InitData()
		{
			PicSun = ScriptCommand.Data.LoadPicture("system", "sun" + Style + ".png");
			PicSuns = new Picture[TypeNum];
		}

		public override void FinalData()
		{
			Picture[] picSuns = PicSuns;
			foreach (Picture picture in picSuns)
			{
				picture.Dispose();
			}
			PicSun.Dispose();
		}
	}
}
