using System;
using System.Collections.Generic;
using System.Drawing;
using KidFansChannel.GalCore;
using KidFansChannel.GalCore.Commands;
using KidFansChannel.Game2DCore;

namespace KidFansChannel.CloseTo
{
	public class Label
	{
		private static String2D _stringBase;

		private static int arrowIndex;

		private static Point _location;

		private static Picture[] backBases = new Picture[5];

		private static SubPicture[,] backParts = new SubPicture[5, 9];

		private static Picture[] backArrowBases = new Picture[4];

		private static SubPicture[,] backArrowParts = new SubPicture[4, 4];

		private static List<Picture[,]> backLists = new List<Picture[,]>();

		private static Dictionary<int, int> ids = new Dictionary<int, int>();

		private static int _currentId;

		public static String2D StringBase
		{
			get
			{
				return _stringBase;
			}
			set
			{
				_stringBase = value;
			}
		}

		public static Point Location
		{
			get
			{
				return _location;
			}
		}

		public static Picture[,] PicBacks
		{
			get
			{
				return backLists[ids[_currentId]];
			}
		}

		public static int CurrentId
		{
			get
			{
				return _currentId;
			}
			set
			{
				_currentId = value;
			}
		}

		public static void Update(Point location)
		{
			_location = location;
			int num = _stringBase.TotalLine * 28 + 30;
			int num2 = _stringBase.Width + 60;
			for (int i = 0; i < 5; i++)
			{
				backParts[i, 0].Center = new Point(num2 / 2, num / 2);
				backParts[i, 1].Center = new Point(10, num / 2);
				backParts[i, 2].Center = new Point(30 - num2 / 2, num / 2);
				backParts[i, 3].Center = new Point(num2 / 2, 5);
				backParts[i, 4].Center = new Point(10, 5);
				backParts[i, 5].Center = new Point(30 - num2 / 2, 5);
				backParts[i, 6].Center = new Point(num2 / 2, 25 - num / 2);
				backParts[i, 7].Center = new Point(10, 25 - num / 2);
				backParts[i, 8].Center = new Point(30 - num2 / 2, 25 - num / 2);
				backParts[i, 0].Scale = new SizeF(1f, 1f);
				backParts[i, 1].Scale = new SizeF((float)(num2 - 60) / 20f, 1f);
				backParts[i, 2].Scale = new SizeF(1f, 1f);
				backParts[i, 3].Scale = new SizeF(1f, (float)(num - 50) / 10f);
				backParts[i, 4].Scale = new SizeF((float)(num2 - 60) / 20f, (float)(num - 50) / 10f);
				backParts[i, 5].Scale = new SizeF(1f, (float)(num - 50) / 10f);
				backParts[i, 6].Scale = new SizeF(1f, 1f);
				backParts[i, 7].Scale = new SizeF((float)(num2 - 60) / 20f, 1f);
				backParts[i, 8].Scale = new SizeF(1f, 1f);
			}
			for (int j = 0; j < 9; j++)
			{
				backParts[0, j].Alpha = (int)(GameManager.GalCore.GameOptions.DialogueAlpha * 255f + 0.5f);
			}
			Point position = location;
			Point location2 = location;
			arrowIndex = 0;
			if (location.X < BgDisplay.ScreenSize.Width / 2)
			{
				position.X += num2 / 2 + 20;
				location2.X += 50;
			}
			else
			{
				position.X -= num2 / 2 + 20;
				location2.X -= 50 + _stringBase.Width;
				arrowIndex++;
			}
			if (location.Y < BgDisplay.ScreenSize.Height / 2)
			{
				position.Y += 8;
				location2.Y += 8 - _stringBase.TotalLine * 28 / 2;
				arrowIndex += 2;
			}
			else
			{
				position.Y -= 8;
				location2.Y -= 8 + _stringBase.TotalLine * 28 / 2;
			}
			for (int k = 0; k < 5; k++)
			{
				for (int l = 0; l < 9; l++)
				{
					backParts[k, l].Position = position;
				}
			}
			for (int m = 0; m < 4; m++)
			{
				backArrowParts[m, arrowIndex].Position = location;
			}
			_stringBase.Location = location2;
		}

		public static void SetBackSize(int width, int height)
		{
			for (int i = 0; i < 5; i++)
			{
				PicBacks[i, 0].Center = new Point(width / 2, height / 2);
				PicBacks[i, 1].Center = new Point(10, height / 2);
				PicBacks[i, 2].Center = new Point(30 - width / 2, height / 2);
				PicBacks[i, 3].Center = new Point(width / 2, 5);
				PicBacks[i, 4].Center = new Point(10, 5);
				PicBacks[i, 5].Center = new Point(30 - width / 2, 5);
				PicBacks[i, 6].Center = new Point(width / 2, 25 - height / 2);
				PicBacks[i, 7].Center = new Point(10, 25 - height / 2);
				PicBacks[i, 8].Center = new Point(30 - width / 2, 25 - height / 2);
				PicBacks[i, 0].Scale = new SizeF(1f, 1f);
				PicBacks[i, 1].Scale = new SizeF((float)(width - 60) / 20f, 1f);
				PicBacks[i, 2].Scale = new SizeF(1f, 1f);
				PicBacks[i, 3].Scale = new SizeF(1f, (float)(height - 50) / 10f);
				PicBacks[i, 4].Scale = new SizeF((float)(width - 60) / 20f, (float)(height - 50) / 10f);
				PicBacks[i, 5].Scale = new SizeF(1f, (float)(height - 50) / 10f);
				PicBacks[i, 6].Scale = new SizeF(1f, 1f);
				PicBacks[i, 7].Scale = new SizeF((float)(width - 60) / 20f, 1f);
				PicBacks[i, 8].Scale = new SizeF(1f, 1f);
			}
			for (int j = 0; j < 9; j++)
			{
				PicBacks[0, j].Alpha = (int)(GameManager.GalCore.GameOptions.DialogueAlpha * 255f + 0.5f);
			}
		}

		public static void Update(Point location, string text)
		{
			_stringBase.ClearString();
			_stringBase.AddString(text);
			Update(location);
		}

		public static void Draw()
		{
			if (!GameManager.GalCore.HideString)
			{
				int colorIndex = GetColorIndex();
				for (int i = 0; i < 9; i++)
				{
					GameManager.GalCore.CurrentCanvas.DrawPicture(backParts[0, i]);
					GameManager.GalCore.CurrentCanvas.DrawPicture(backParts[colorIndex, i]);
				}
				GameManager.GalCore.CurrentCanvas.DrawPicture(backArrowParts[colorIndex - 1, arrowIndex]);
				_stringBase.Canvas = GameManager.GalCore.CurrentCanvas;
				_stringBase.DrawString();
			}
		}

		public static int GetColorIndex()
		{
			switch (GameManager.GalCore.GameOptions.DialogueColor)
			{
			case "Blue":
				return 1;
			case "Green":
				return 2;
			case "Pink":
				return 3;
			case "Gray":
				return 4;
			default:
				throw new Exception("Unbelievable!");
			}
		}

		public static void Init()
		{
			for (int i = 0; i < 5; i++)
			{
				backBases[i] = GameManager.Data.LoadPictureInstant("system", "back" + i + ".png");
				backParts[i, 0] = GameManager.Graphics.CreateSubPicture(backBases[i], new Rectangle(0, 0, 30, 25));
				backParts[i, 1] = GameManager.Graphics.CreateSubPicture(backBases[i], new Rectangle(30, 0, 20, 25));
				backParts[i, 2] = GameManager.Graphics.CreateSubPicture(backBases[i], new Rectangle(50, 0, 30, 25));
				backParts[i, 3] = GameManager.Graphics.CreateSubPicture(backBases[i], new Rectangle(0, 25, 30, 10));
				backParts[i, 4] = GameManager.Graphics.CreateSubPicture(backBases[i], new Rectangle(30, 25, 20, 10));
				backParts[i, 5] = GameManager.Graphics.CreateSubPicture(backBases[i], new Rectangle(50, 25, 30, 10));
				backParts[i, 6] = GameManager.Graphics.CreateSubPicture(backBases[i], new Rectangle(0, 35, 30, 25));
				backParts[i, 7] = GameManager.Graphics.CreateSubPicture(backBases[i], new Rectangle(30, 35, 20, 25));
				backParts[i, 8] = GameManager.Graphics.CreateSubPicture(backBases[i], new Rectangle(50, 35, 30, 25));
			}
			for (int j = 0; j < 4; j++)
			{
				backArrowBases[j] = GameManager.Data.LoadPictureInstant("system", "back_arrow" + (j + 1) + ".png");
				for (int k = 0; k < 4; k++)
				{
					backArrowParts[j, k] = GameManager.Graphics.CreateSubPicture(backArrowBases[j], new Rectangle(k % 2 * 20, k / 2 * 16, 20, 16));
					backArrowParts[j, k].Center = new Point(k % 2 * 28 - 4, (1 - k / 2) * 16);
				}
			}
			_stringBase = new String2D();
			_stringBase.Font = GameManager.GalCore.GameFont.GetFont(20);
			_stringBase.LineSize = new Size(800, 28);
			_stringBase.LineNum = int.MaxValue;
			_stringBase.DrawCharCallback = DrawChar.DrawCharShadow;
		}

		public static void Final()
		{
			for (int i = 0; i < 5; i++)
			{
				for (int j = 0; j < 9; j++)
				{
					backParts[i, j].Dispose();
					backParts[i, j] = null;
				}
				backBases[i].Dispose();
				backBases[i] = null;
			}
			for (int k = 0; k < 4; k++)
			{
				for (int l = 0; l < 4; l++)
				{
					backArrowParts[k, l].Dispose();
					backArrowParts[k, l] = null;
				}
				backArrowBases[k].Dispose();
				backArrowBases[k] = null;
			}
		}

		public static int GenerateId()
		{
			int i;
			for (i = 0; ids.ContainsKey(i); i++)
			{
			}
			int count = ids.Count;
			ids.Add(i, count);
			backLists.Add(new Picture[5, 9]);
			for (int j = 0; j < 5; j++)
			{
				for (int k = 0; k < 9; k++)
				{
					backLists[count][j, k] = (Picture)backParts[j, k].Clone();
				}
			}
			_currentId = i;
			return i;
		}

		public static void ReleaseId(int id)
		{
			int num = ids[id];
			for (int i = 0; i < ids.Count; i++)
			{
				if (ids[i] > num)
				{
					ids[i]--;
				}
			}
			ids.Remove(id);
			for (int j = 0; j < 5; j++)
			{
				for (int k = 0; k < 9; k++)
				{
					backLists[num][j, k].Dispose();
					backLists[num][j, k] = null;
				}
			}
			backLists.RemoveAt(num);
		}
	}
}
