using System.IO;
using System.Windows.Forms;
using KidFansChannel.DataCore;
using KidFansChannel.Game2DCore;

namespace KidFansChannel.GalCore.Screens
{
	public abstract class LogoScreen : GameScreen
	{
		protected string[] Files;

		private int currentIndex;

		private float time;

		private Picture picLogo;

		public override bool DataReady
		{
			get
			{
				if (base.DataReady)
				{
					return true;
				}
				DataReady = true;
				return true;
			}
		}

		public override bool FinalScreen()
		{
			if (!base.FinalScreen())
			{
				return false;
			}
			FinalData();
			Finished = true;
			return true;
		}

		public override void FinalData()
		{
			base.FinalData();
			if (picLogo != null)
			{
				picLogo.Dispose();
				picLogo = null;
			}
		}

		public override void Update(float elaspedTime)
		{
			if (time > 0f)
			{
				time -= elaspedTime;
				if (time > 1.5f)
				{
					picLogo.Alpha = (int)((1f - (time - 1.5f) / 0.5f) * 255f + 0.5f);
				}
				else if (time > 0.5f)
				{
					picLogo.Alpha = 255;
				}
				else if (time > 0f)
				{
					picLogo.Alpha = (int)(time / 0.5f * 255f + 0.5f);
				}
				else
				{
					picLogo.Alpha = 0;
				}
				return;
			}
			if (currentIndex == Files.Length)
			{
				FinalScreen();
				GameManager.GalCore.ReturnTitleScreen();
				return;
			}
			if (Files[currentIndex].EndsWith("wmv"))
			{
				KidFansChannel.DataCore.DataCore.DataType dataType = GameManager.Data.CheckData("movie", Files[currentIndex], KidFansChannel.DataCore.DataCore.DataType.File);
				if (dataType == KidFansChannel.DataCore.DataCore.DataType.File)
				{
					string text = GameManager.Data.GetFile();
					if (!GameManager.GalCore.GameOptions.SubtitleShow && text.EndsWith(GameManager.Data.LanguageExt + "\\" + Files[currentIndex]) && File.Exists(text.Replace(GameManager.Data.LanguageExt + "\\" + Files[currentIndex], Files[currentIndex])))
					{
						text = text.Replace(GameManager.Data.LanguageExt + "\\" + Files[currentIndex], Files[currentIndex]);
					}
					if (GameManager.GalCore.GameVideo != null)
					{
						GameManager.GalCore.GameVideo.Play(text);
					}
				}
			}
			else if (Files[currentIndex].EndsWith("png"))
			{
				if (picLogo != null)
				{
					picLogo.Dispose();
					picLogo = null;
				}
				try
				{
					picLogo = GameManager.Data.LoadPictureInstant("system", Files[currentIndex]);
					picLogo.Alpha = 0;
					time = 2f;
				}
				catch
				{
				}
			}
			currentIndex++;
		}

		public override void Draw()
		{
			base.Draw();
			if (time > 0f)
			{
				GameManager.GalCore.CurrentCanvas.DrawPicture(picLogo);
			}
		}

		public override void OnKeyPress(Keys keyData)
		{
			if (keyData == Keys.Space)
			{
				if (time > 0.5f)
				{
					if (time > 1.5f)
					{
						time = 2f - time;
					}
					else
					{
						time = 0.5f;
					}
				}
			}
			else
			{
				base.OnKeyPress(keyData);
			}
		}
	}
}
