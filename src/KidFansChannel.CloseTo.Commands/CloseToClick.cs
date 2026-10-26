using System;
using System.Drawing;
using KidFansChannel.GalCore;
using KidFansChannel.GalCore.Commands;
using KidFansChannel.Game2DCore;

namespace KidFansChannel.CloseTo.Commands
{
	internal class CloseToClick : Click
	{
		private float time;

		private static Picture clickIconBase;

		private static SubPicture[] clickIcons = new SubPicture[5];

		private static AnimePicture clickIconAnime;

		protected override bool QuickSaveChapter
		{
			get
			{
				switch (ScriptCommand.GalCore.GameState.CurrentGameState.Chapter)
				{
				case "Common_2":
				case "Yuna_3":
				case "Shoko_4":
				case "Koyuki_5":
				case "Ghost_154":
					return false;
				default:
					return true;
				}
			}
		}

		public override void InitCommand()
		{
			base.InitCommand();
			time = 0f;
			clickIconAnime.Position = ClickPosition;
			clickIconAnime.X -= 5;
			clickIconAnime.Y -= 5;
			clickIconAnime.ResetFrame();
		}

		public override void FinalCommand()
		{
			base.FinalCommand();
		}

		public override void Update(double appTime, float elaspedTime)
		{
			base.Update(appTime, elaspedTime);
			time += elaspedTime;
			while (time >= 3f)
			{
				time -= 3f;
			}
			clickIconAnime.X = ClickPosition.X + (int)((0.0 - Math.Sin(Math.PI * (double)time / 0.75)) * 5.0 + 0.5) - 5;
			clickIconAnime.Y = ClickPosition.Y + (int)(Math.Cos(Math.PI * (double)time / 1.5) * 5.0 + 0.5) - 5;
			clickIconAnime.Update(elaspedTime);
		}

		public override void Draw()
		{
			base.Draw();
			if (float.IsNaN(Time))
			{
				ScriptCommand.Graphics.DrawPicture(clickIconAnime);
			}
		}

		public override void InitStaticData()
		{
			clickIconBase = ScriptCommand.GalCore.GameData.LoadPictureInstant("system", "click.png");
			for (int i = 0; i < 5; i++)
			{
				clickIcons[i] = ScriptCommand.GalCore.GameGraphics.CreateSubPicture(clickIconBase, new Rectangle(i % 3 * 40, i / 3 * 40, 40, 40));
			}
			clickIconAnime = ScriptCommand.GalCore.GameGraphics.CreateAnimePicture(new Size(40, 40));
			for (int j = 0; j < 5; j++)
			{
				clickIconAnime.AddFrame(clickIcons[j], 1f / 12f);
			}
		}

		public override void FinalStaticData()
		{
			base.FinalCommand();
			clickIconAnime.Dispose();
			for (int i = 0; i < 5; i++)
			{
				clickIcons[i].Dispose();
				clickIcons[i] = null;
			}
			clickIconBase.Dispose();
			clickIconBase = null;
		}

		protected override float GetAutoTime()
		{
			switch (ScriptCommand.Options.AutoSpeed)
			{
			case GameOptions.Level.High:
				return 0.5f;
			case GameOptions.Level.Mid:
				return 1f;
			case GameOptions.Level.Low:
				return 2f;
			case GameOptions.Level.None:
				return 0f;
			default:
				return 1f;
			}
		}
	}
}
