using System;
using System.Drawing;
using KidFansChannel.GalCore.Commands;
using KidFansChannel.Game2DCore;

namespace KidFansChannel.CloseTo.Commands
{
	public class CloseToShowTime : ShowTime
	{
		private static Picture[] timeBacks = new Picture[5];

		private static Picture timeNumBase;

		private static Picture[] timeNums = new Picture[10];

		private static Picture[] timeAMPM = new Picture[2];

		private static Picture timeColon;

		public float ShowHideTime = 0.5f;

		public float LastingTime = 3f;

		public float colonTime = 0.25f;

		private static int currentColorIndex;

		private static string currentColor;

		private float time;

		public override void InitCommand()
		{
			base.InitCommand();
			if (!Hide)
			{
				for (int i = 0; i < 5; i++)
				{
					timeBacks[i].Alpha = 0;
					timeBacks[i].BeginTransform(ShowHideTime);
					timeBacks[i].AlphaTo(255);
					timeBacks[i].EndTransform();
				}
				for (int j = 0; j < 10; j++)
				{
					timeNums[j].Alpha = 0;
					timeNums[j].BeginTransform(ShowHideTime);
					timeNums[j].AlphaTo(255);
					timeNums[j].EndTransform();
				}
				timeColon.Alpha = 0;
				timeColon.BeginTransform(ShowHideTime);
				timeColon.AlphaTo(255);
				timeColon.EndTransform();
				for (int k = 0; k < 2; k++)
				{
					timeAMPM[k].Alpha = 0;
					timeAMPM[k].BeginTransform(ShowHideTime);
					timeAMPM[k].AlphaTo(255);
					timeAMPM[k].EndTransform();
				}
				time = LastingTime;
			}
		}

		public override void Draw()
		{
			int alpha = timeBacks[0].Alpha;
			timeBacks[0].Alpha = (int)((float)alpha * ScriptCommand.Options.DialogueAlpha + 0.5f);
			ScriptCommand.Canvas.DrawPicture(timeBacks[0]);
			timeBacks[0].Alpha = alpha;
			if (ScriptCommand.Options.DialogueColor != currentColor)
			{
				GetColorIndex();
			}
			ScriptCommand.Canvas.DrawPicture(timeBacks[currentColorIndex]);
			int hour = int.Parse(Time.Substring(0, 2));
			if (hour <= 12)
			{
				ScriptCommand.Canvas.DrawPicture(timeAMPM[0]);
			}
			else
			{
				ScriptCommand.Canvas.DrawPicture(timeAMPM[1]);
				hour -= 12;
			}
			if (hour >= 10)
			{
				timeNums[1].X = 630;
				ScriptCommand.Canvas.DrawPicture(timeNums[1]);
				hour -= 10;
			}
			timeNums[hour].X = 650;
			ScriptCommand.Canvas.DrawPicture(timeNums[hour]);
			if (colonTime >= 0.5f)
			{
				ScriptCommand.Canvas.DrawPicture(timeColon);
			}
			int num2 = int.Parse(Time.Substring(3, 2));
			timeNums[num2 / 10].X = 690;
			ScriptCommand.Canvas.DrawPicture(timeNums[num2 / 10]);
			timeNums[num2 % 10].X = 710;
			ScriptCommand.Canvas.DrawPicture(timeNums[num2 % 10]);
		}

		public override void Update(double appTime, float elaspedTime)
		{
			for (int i = 0; i < 5; i++)
			{
				timeBacks[i].Update(elaspedTime);
			}
			for (int j = 0; j < 10; j++)
			{
				timeNums[j].Update(elaspedTime);
			}
			timeColon.Update(elaspedTime);
			for (int k = 0; k < 2; k++)
			{
				timeAMPM[k].Update(elaspedTime);
			}
			if (timeBacks[0].Time == 0f)
			{
				if (timeBacks[0].Alpha > 0)
				{
					time -= elaspedTime;
					if (time < 0f)
					{
						time = 0f;
					}
					if (time == 0f)
					{
						for (int l = 0; l < 5; l++)
						{
							timeBacks[l].BeginTransform(ShowHideTime);
							timeBacks[l].AlphaTo(0);
							timeBacks[l].EndTransform();
						}
						for (int m = 0; m < 10; m++)
						{
							timeNums[m].BeginTransform(ShowHideTime);
							timeNums[m].AlphaTo(0);
							timeNums[m].EndTransform();
						}
						timeColon.BeginTransform(ShowHideTime);
						timeColon.AlphaTo(0);
						timeColon.EndTransform();
						for (int n = 0; n < 2; n++)
						{
							timeAMPM[n].BeginTransform(ShowHideTime);
							timeAMPM[n].AlphaTo(0);
							timeAMPM[n].EndTransform();
						}
					}
				}
				else
				{
					FinishCommand();
				}
			}
			colonTime -= elaspedTime;
			while (colonTime < 0f)
			{
				colonTime++;
			}
		}

		public override void InitStaticData()
		{
			for (int i = 0; i < 5; i++)
			{
				timeBacks[i] = ScriptCommand.Data.LoadPictureInstant("system", "time_back" + i + ".png");
				timeBacks[i].Position = new Point(560, 20);
				timeBacks[i].Alpha = 0;
			}
			timeNumBase = ScriptCommand.Data.LoadPictureInstant("system", "time.png");
			for (int j = 0; j < 10; j++)
			{
				timeNums[j] = ScriptCommand.Graphics.CreateSubPicture(timeNumBase, new Rectangle(j % 5 * 20, j / 5 * 40, 20, 40));
				timeNums[j].Y = 30;
				timeNums[j].Alpha = 0;
			}
			timeColon = ScriptCommand.Graphics.CreateSubPicture(timeNumBase, new Rectangle(100, 0, 20, 40));
			timeColon.Position = new Point(670, 30);
			timeColon.Alpha = 0;
			for (int k = 0; k < 2; k++)
			{
				timeAMPM[k] = ScriptCommand.Graphics.CreateSubPicture(timeNumBase, new Rectangle(120, k * 40, 40, 40));
				timeAMPM[k].Position = new Point(590, 30);
				timeAMPM[k].Alpha = 0;
			}
			currentColor = ScriptCommand.Options.DialogueColor;
			GetColorIndex();
		}

		public override void FinalStaticData()
		{
			for (int i = 0; i < 5; i++)
			{
				timeBacks[i].Dispose();
				timeBacks[i] = null;
			}
			for (int j = 0; j < 10; j++)
			{
				timeNums[j].Dispose();
				timeNums[j] = null;
			}
			for (int k = 0; k < 2; k++)
			{
				timeAMPM[k].Dispose();
				timeAMPM[k] = null;
			}
			timeColon.Dispose();
			timeColon = null;
			timeNumBase.Dispose();
			timeNumBase = null;
		}

		private void GetColorIndex()
		{
			currentColor = ScriptCommand.Options.DialogueColor;
			if (currentColor == "Blue")
			{
				currentColorIndex = 1;
				return;
			}
			if (currentColor == "Green")
			{
				currentColorIndex = 2;
				return;
			}
			if (currentColor == "Pink")
			{
				currentColorIndex = 3;
				return;
			}
			if (currentColor == "Gray")
			{
				currentColorIndex = 4;
				return;
			}
			throw new Exception("Unbelievable!");
		}
	}
}
