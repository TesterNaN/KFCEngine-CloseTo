using System.Drawing;
using KidFansChannel.GalCore.Commands;
using KidFansChannel.GalExt.Effects;
using KidFansChannel.Game2DCore;

namespace KidFansChannel.CloseTo.Effects
{
	internal class CloseToSakura : Sakura
	{
		private float scale = 1f;

		private Picture PicSakura;

		private bool dataReady;

		public override bool DataReady
		{
			get
			{
				if (dataReady)
				{
					return true;
				}
				if (PicSakura.IsCreated)
				{
					for (int i = 0; i < AnimaSteps; i++)
					{
						PicSakuras[0, i] = ScriptCommand.Graphics.CreateSubPicture(PicSakura, new Rectangle(i % 2 * HalfSize * 2, i / 2 * HalfSize * 2, HalfSize * 2, HalfSize * 2));
						PicSakuras[0, i].Center = new Point(HalfSize, HalfSize);
						PicSakuras[0, i].Scale = new SizeF(scale, scale);
					}
					dataReady = true;
					return true;
				}
				return false;
			}
		}

		public CloseToSakura()
		{
			TypeNum = 1;
			AnimaSteps = 4;
			HalfSize = 10;
			SpeedMax = new PointF(160f, 90f);
			ChangeMin = 1f;
			ChangeMax = 3f;
			Acc = new PointF(120f, 60f);
			SideRatio = 1.25f;
			AnimiSpeed = 0.25f;
			NextMax = 0.8f;
			NextMin = 0.2f;
			RotateMax = 0.5f;
			RotateMin = 0f;
		}

		protected override void InitStyle()
		{
			switch (Style / 4)
			{
			case 0:
				scale = 0.75f;
				NumMax = 20;
				break;
			case 1:
				scale = 1f;
				NumMax = 20;
				break;
			case 2:
				scale = 1f;
				NumMax = 15;
				break;
			case 4:
				scale = 1f;
				NumMax = 6;
				break;
			}
			NumMin = NumMax / 2;
		}

		public override void InitData()
		{
			PicSakura = ScriptCommand.Data.LoadPicture("system", "sakura.png");
			PicSakuras = new Picture[TypeNum, AnimaSteps];
		}

		public override void FinalData()
		{
			Picture[,] picSakuras = PicSakuras;
			foreach (Picture picture in picSakuras)
			{
				picture.Dispose();
			}
			PicSakura.Dispose();
		}
	}
}
