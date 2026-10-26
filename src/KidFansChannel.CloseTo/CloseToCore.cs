using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using KidFansChannel.CloseTo.Screens;
using KidFansChannel.GalCore;
using KidFansChannel.GalCore.Commands;
using KidFansChannel.GalCore.Screens;
using KidFansChannel.Game2DCore;
using KidFansChannel.GameCore;

namespace KidFansChannel.CloseTo
{
	public class CloseToCore : KidFansChannel.GalCore.GalCore
	{
		private Picture picPauseBack;

		private bool? toPauseValue = null;

		private Picture picStateIconBase;

		private Picture[] picStateIcons = new Picture[8];

		private Picture[] picAnimeStateIcon = new Picture[2];

		private bool lastAutoOrQuick;

		private Picture[] picQuickSaveIcons = new Picture[4];

		private string titleChara;

		private bool returnTitleScreen;

		private String2D strComment;

		private Picture picCommentBack;

		private string commentStr;

		private float time;

		private int lineNum;

		private bool formClosing;

		private Picture picCursorBase;

		private Picture[] picCursors = new Picture[3];

		private AnimePicture picCursorAnime;

		private Picture currentCursor;

		private bool _alwaysShowCursor;

		private Point lastMouse = Point.Empty;

		private float unmoveTime;

		private PointF mouseLocation;

		private PointF mouseChange;

		private float mouseTime;

		public bool CursorActive
		{
			get
			{
				return currentCursor == picCursorAnime;
			}
			set
			{
				currentCursor = (value ? picCursorAnime : picCursors[0]);
			}
		}

		public bool AlwaysShowCursor
		{
			get
			{
				return _alwaysShowCursor;
			}
			set
			{
				_alwaysShowCursor = value;
				if (value)
				{
					picCursorAnime.Alpha = 255;
					picCursors[0].Alpha = 255;
					if (currentCursor.Time > 0f)
					{
						picCursorAnime.Update(picCursorAnime.Time);
						picCursors[0].Update(picCursors[0].Time);
					}
					unmoveTime = 0f;
				}
			}
		}

		public CloseToCore()
			: base("CloseToStr", "CloseToIcon")
		{
		}

		public override void Initialize()
		{
			base.Initialize();
			picStateIconBase = GameData.LoadPictureInstant("system", "state_icon.png");
			for (int i = 0; i < 4; i++)
			{
				picQuickSaveIcons[i] = GameData.LoadPictureInstant("system", "quicksave_icon" + (i + 1) + ".png");
			}
			picCommentBack = GameData.LoadPictureInstant("system", "comment.png");
			for (int j = 0; j < 2; j++)
			{
				picAnimeStateIcon[j] = GameGraphics.CreateAnimePicture(new Size(40, 40));
				picAnimeStateIcon[j].Position = new Point(40, 30);
				for (int k = 0; k < 4; k++)
				{
					picStateIcons[j * 4 + k] = GameGraphics.CreateSubPicture(picStateIconBase, new Rectangle(k * 40, j * 40, 40, 40));
					if (k < 3)
					{
						((AnimePicture)picAnimeStateIcon[j]).AddFrame(picStateIcons[j * 4 + k], 1f / 6f);
					}
					else
					{
						picStateIcons[j * 4 + k].Position = new Point(40, 30);
					}
				}
			}
			picStateIcons[3].Alpha = 0;
			for (int l = 0; l < 4; l++)
			{
				picQuickSaveIcons[l].Position = new Point(560, 20);
				picQuickSaveIcons[l].Alpha = 0;
			}
			strComment = new String2D();
			strComment.Font = GameFont.GetFont(20);
			GameGraphics.FullscreenToggled += GameGraphics_FullscreenToggled;
			picCommentBack.ScaleX = (float)GameGraphics.Resolution.CommentArea.Width / GameGraphics.TargetBounds.Width / 40f;
			picCommentBack.ScaleY = (float)GameGraphics.Resolution.CommentArea.Height / GameGraphics.TargetBounds.Height / 40f;
			picCommentBack.X = (int)(((float)GameGraphics.Resolution.CommentArea.X - GameGraphics.TargetBounds.X) / GameGraphics.TargetBounds.Width + 0.5f);
			picCommentBack.Y = (int)(((float)GameGraphics.Resolution.CommentArea.Y - GameGraphics.TargetBounds.Y) / GameGraphics.TargetBounds.Height + 0.5f);
			strComment.Location = new Point((int)(((float)GameGraphics.Resolution.CommentArea.X - GameGraphics.TargetBounds.X) / GameGraphics.TargetBounds.Width + 0.5f), (int)(((float)GameGraphics.Resolution.CommentArea.Y - GameGraphics.TargetBounds.Y) / GameGraphics.TargetBounds.Height + 0.5f) + 2);
			strComment.LineSize = new Size((int)((float)GameGraphics.Resolution.CommentArea.Width / GameGraphics.TargetBounds.Width + 0.5f), 40);
			lineNum = (int)((float)GameGraphics.Resolution.CommentArea.Height / GameGraphics.TargetBounds.Height / 40f);
			strComment.LineNum = int.MaxValue;
			picCursorBase = GameData.LoadPictureInstant("system", "cursor.png");
			for (int m = 0; m < 3; m++)
			{
				picCursors[m] = GameGraphics.CreateSubPicture(picCursorBase, new Rectangle(m % 2 * 40, m / 2 * 40, 40, 40));
				picCursors[m].Center = new Point(10, 10);
				picCursors[m].Z = 0f;
			}
			picCursorAnime = GameGraphics.CreateAnimePicture(new Size(40, 40));
			picCursorAnime.AddFrame(picCursors[1], 0.25f);
			picCursorAnime.AddFrame(picCursors[2], 0.25f);
			picCursorAnime.Center = new Point(10, 10);
			picCursorAnime.Z = 0f;
			currentCursor = picCursors[0];
			GameGraphics.GameWindow.Cursor = new Cursor(new MemoryStream((byte[])GameLanguage.ResourceManager.GetObject("Transparent")));
		}

		protected override void ChangePauseState(bool value)
		{
			if (!(picPauseBack.Time > 0f) || !toPauseValue.HasValue || toPauseValue.Value != value)
			{
				if (value)
				{
					Pause = true;
					picPauseBack.Alpha = 0;
					picPauseBack.BeginTransform(0.5f);
					picPauseBack.AlphaTo(128);
					picPauseBack.EndTransform();
					toPauseValue = true;
					GameSound.StopVoice();
					GameSound.ChangeVolumeTempBgm(0.5f);
				}
				else
				{
					picPauseBack.Alpha = 128;
					picPauseBack.BeginTransform(0.5f);
					picPauseBack.AlphaTo(0);
					picPauseBack.EndTransform();
					toPauseValue = false;
					GameSound.ChangeVolumeTempBgm(1f);
				}
			}
		}

		protected override bool UpdatePause(double appTime, float elaspedTime)
		{
			base.UpdatePause(appTime, elaspedTime);
			picPauseBack.Update(elaspedTime);
			if (picPauseBack.Time == 0f)
			{
				toPauseValue = null;
				if (picPauseBack.Alpha == 0)
				{
					Pause = false;
				}
			}
			return picPauseBack.Alpha == 128;
		}

		protected override void DrawPause()
		{
			base.DrawPause();
			CurrentCanvas.DrawPicture(picPauseBack);
		}

		protected override void PreparePause()
		{
			base.PreparePause();
			picPauseBack = GameGraphics.CreatePicture(Color.Black);
			picPauseBack.Size = BgDisplay.ScreenSize;
		}

		public override void ShowSaveLoadScreen(bool isSave)
		{
			GameScreen screen = new CloseToSaveLoadScreen(isSave, GameScript.CurrentGameScript == null);
			ShowScreen(screen);
		}

		public override void ShowMessageScreen()
		{
			GameScreen screen = new CloseToMessageScreen();
			ShowScreen(screen);
		}

		public override void ShowPauseScreen()
		{
			GameScreen screen = new CloseToPauseScreen();
			ShowScreen(screen);
		}

		public override void ShowOptionsScreen()
		{
			GameScreen screen = new CloseToOptionsScreen(GameScript.CurrentGameScript == null);
			ShowScreen(screen);
		}

		public override void ShowTitleScreen()
		{
			GameScreen gameScreen = new CloseToTitleScreen();
			((CloseToTitleScreen)gameScreen).Chara = titleChara;
			ShowScreen(gameScreen);
		}

		public override void ShowEncyListScreen()
		{
			GameScreen screen = new CloseToEncyListScreen(GameScript.CurrentGameScript == null);
			ShowScreen(screen);
		}

		public override void ShowEncyContentScreen(int index)
		{
			GameScreen screen = new CloseToEncyContentScreen(index, GameScript.CurrentGameScript == null);
			ShowScreen(screen);
		}

		public override void ShowAlbumScreen()
		{
			GameScreen screen = new CloseToAlbumScreen();
			ShowScreen(screen);
		}

		public override void ShowMusicScreen()
		{
			GameScreen screen = new CloseToMusicScreen();
			ShowScreen(screen);
		}

		public override void ShowMovieScreen()
		{
			GameScreen screen = new CloseToMovieScreen();
			ShowScreen(screen);
		}

		public override void ShowClearListScreen()
		{
			GameScreen screen = new CloseToClearListScreen();
			ShowScreen(screen);
		}

		public override void ShowShortcutScreen()
		{
			GameScreen screen = new CloseToShortcutScreen();
			ShowScreen(screen);
		}

		public override void ShowLogoScreen()
		{
			GameScreen screen = new CloseToLogoScreen();
			ShowScreen(screen);
		}

		protected override void UpdateStateIcon(float elapsedTime)
		{
			base.UpdateStateIcon(elapsedTime);
			if (!ToNormal)
			{
				switch (CurrentPlayMode)
				{
				case PlayMode.Auto:
					picAnimeStateIcon[0].Update(elapsedTime);
					if (!lastAutoOrQuick)
					{
						GameData.LoadAndPlaySystemSound("skip.ogg");
						lastAutoOrQuick = true;
					}
					break;
				case PlayMode.Quick:
					picAnimeStateIcon[1].Update(elapsedTime);
					if (!lastAutoOrQuick)
					{
						GameData.LoadAndPlaySystemSound("skip.ogg");
						lastAutoOrQuick = true;
					}
					break;
				default:
					if (lastAutoOrQuick)
					{
						GameData.LoadAndPlaySystemSound("cancel.ogg");
						lastAutoOrQuick = false;
						picStateIcons[3].Alpha = 255;
						picStateIcons[3].BeginTransform(1f);
						picStateIcons[3].AlphaTo(0);
						picStateIcons[3].EndTransform();
					}
					else if (picStateIcons[3].Alpha > 0)
					{
						picStateIcons[3].Update(elapsedTime);
					}
					break;
				}
			}
			else if (lastAutoOrQuick)
			{
				bool succeeded = false;
				foreach (ScriptCommand currentCommand in CurrentCommandList)
				{
					if (currentCommand is Select)
					{
						succeeded = true;
					}
				}
				if (!succeeded)
				{
					GameData.LoadAndPlaySystemSound("cancel.ogg");
				}
				lastAutoOrQuick = false;
				picStateIcons[3].Alpha = 255;
				picStateIcons[3].BeginTransform(1f);
				picStateIcons[3].AlphaTo(0);
				picStateIcons[3].EndTransform();
			}
			else if (picStateIcons[3].Alpha > 0)
			{
				picStateIcons[3].Update(elapsedTime);
			}
			for (int i = 0; i < 4; i++)
			{
				if (picQuickSaveIcons[i].Alpha > 0)
				{
					picQuickSaveIcons[i].Update(elapsedTime);
				}
			}
		}

		private int GetColorIndex()
		{
			if (GameOptions.DialogueColor == "Blue")
			{
				return 1;
			}
			if (GameOptions.DialogueColor == "Green")
			{
				return 2;
			}
			if (GameOptions.DialogueColor == "Pink")
			{
				return 3;
			}
			if (GameOptions.DialogueColor == "Gray")
			{
				return 4;
			}
			throw new Exception("Unbelievable!");
		}

		protected override void DrawStateIcon()
		{
			base.DrawStateIcon();
			if (!ToNormal)
			{
				switch (CurrentPlayMode)
				{
				case PlayMode.Auto:
					GameGraphics.DrawPicture(picAnimeStateIcon[0]);
					break;
				case PlayMode.Quick:
					GameGraphics.DrawPicture(picAnimeStateIcon[1]);
					break;
				default:
					if (picStateIcons[3].Alpha > 0)
					{
						GameGraphics.DrawPicture(picStateIcons[3]);
					}
					break;
				}
			}
			else if (picStateIcons[3].Alpha > 0)
			{
				GameGraphics.DrawPicture(picStateIcons[3]);
			}
			if (picQuickSaveIcons[GetColorIndex() - 1].Alpha > 0)
			{
				GameGraphics.DrawPicture(picQuickSaveIcons[GetColorIndex() - 1]);
			}
		}

		public override void ReturnTitleScreen(bool save)
		{
			if (returnTitleScreen)
			{
				base.ReturnTitleScreen(save);
				ResetScreenElement();
				returnTitleScreen = false;
				return;
			}
			if (GameScript.CurrentGameScript != null)
			{
				titleChara = ((CloseToInstanceScript)GameScript.CurrentGameScript).ChapterChara;
			}
			else
			{
				titleChara = null;
			}
			if (GameScript.CurrentGameScript != null)
			{
				GameState.SaveState();
				returnTitleScreen = true;
			}
			else
			{
				base.ReturnTitleScreen(save);
				ResetScreenElement();
			}
		}

		public void ResetScreenElement()
		{
			lastAutoOrQuick = false;
			if (picStateIcons[3].Alpha > 0)
			{
				picStateIcons[3].Update(picStateIcons[3].Time);
			}
			for (int i = 0; i < 4; i++)
			{
				if (picQuickSaveIcons[i].Alpha > 0)
				{
					picQuickSaveIcons[i].Update(picQuickSaveIcons[i].Time);
				}
			}
			currentCursor = picCursors[0];
		}

		public void StartQuickSaveIcon()
		{
			for (int i = 0; i < 4; i++)
			{
				picQuickSaveIcons[i].Alpha = 255;
				picQuickSaveIcons[i].BeginTransform(2f);
				picQuickSaveIcons[i].AlphaTo(0);
				picQuickSaveIcons[i].EndTransform();
			}
		}

		public override void ShowComment(string comment)
		{
			base.ShowComment(comment);
			picCommentBack.Alpha = 0;
			picCommentBack.BeginTransform(0.5f);
			picCommentBack.AlphaTo(255);
			picCommentBack.EndTransform();
			strComment.ClearString();
			commentStr = comment;
			time = 0f;
		}

		public override void HideComment()
		{
			base.HideComment();
			strComment.ClearString();
			commentStr = null;
		}

		protected override void UpdateComment(float elapsedTime)
		{
			base.UpdateComment(elapsedTime);
			picCommentBack.Update(elapsedTime);
			strComment.ClearString();
			strComment.AddString(commentStr, ContentAlignment.TopLeft);
			time += elapsedTime;
			while ((double)time > Math.Ceiling((float)strComment.TotalLine / (float)lineNum) * 3.0)
			{
				time -= (float)Math.Ceiling((float)strComment.TotalLine / (float)lineNum) * 3f;
			}
			strComment.CutLine((int)(time / 3f) * lineNum, (int)(time / 3f) * lineNum + lineNum - 1);
		}

		protected override void DrawComment()
		{
			base.DrawComment();
			GameGraphics.DrawPicture(picCommentBack);
			strComment.Canvas = CurrentCanvas;
			strComment.DrawString((float)picCommentBack.Alpha / 255f);
		}

		private void GameGraphics_FullscreenToggled(object sender, EventArgs e)
		{
			picCommentBack.ScaleX = (float)GameGraphics.Resolution.CommentArea.Width / GameGraphics.TargetBounds.Width / 40f;
			picCommentBack.ScaleY = (float)GameGraphics.Resolution.CommentArea.Height / GameGraphics.TargetBounds.Height / 40f;
			picCommentBack.X = (int)(((float)GameGraphics.Resolution.CommentArea.X - GameGraphics.TargetBounds.X) / GameGraphics.TargetBounds.Width + 0.5f);
			picCommentBack.Y = (int)(((float)GameGraphics.Resolution.CommentArea.Y - GameGraphics.TargetBounds.Y) / GameGraphics.TargetBounds.Height + 0.5f);
			strComment.Location = new Point((int)(((float)GameGraphics.Resolution.CommentArea.X - GameGraphics.TargetBounds.X) / GameGraphics.TargetBounds.Width + 0.5f), (int)(((float)GameGraphics.Resolution.CommentArea.Y - GameGraphics.TargetBounds.Y) / GameGraphics.TargetBounds.Height + 0.5f) + 2);
			strComment.LineSize = new Size((int)((float)GameGraphics.Resolution.CommentArea.Width / GameGraphics.TargetBounds.Width + 0.5f), 40);
			lineNum = (int)((float)GameGraphics.Resolution.CommentArea.Height / GameGraphics.TargetBounds.Height / 40f);
			strComment.LineNum = int.MaxValue;
			string text = strComment.String;
			if (text != string.Empty)
			{
				strComment.ClearString();
				strComment.AddString(text, ContentAlignment.TopCenter);
			}
		}

		protected override void GameWindow_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (formClosing)
			{
				GameOptions.Save();
				GameSound.Dispose();
				formClosing = false;
			}
			else if (!CanExitNow)
			{
				if (GameVideo == null || !GameVideo.IsPlaying)
				{
					ShowMessageBox(new CloseToMessageBoxScreen(GameLanguage.ResourceManager.GetString("ExitStr"), MessageBoxButtons.YesNo, CloseToFormClosing, new object[2] { sender, e }));
				}
				e.Cancel = true;
			}
			else
			{
				if (GameScript.CurrentGameScript != null)
				{
					GameScript.SaveLastGame();
				}
				GameState.SaveState();
				formClosing = true;
				e.Cancel = true;
			}
		}

		private void CloseToFormClosing(object[] param)
		{
			CanExitNow = true;
			Application.Exit();
		}

		protected override void LoadDropSave(string fileName)
		{
			GameManager.GalCore.ShowMessageBox(new CloseToMessageBoxScreen(GameManager.GalCore.GameLanguage.ResourceManager.GetString("LoadStr"), MessageBoxButtons.YesNo, CloseToLoadDropSave, new object[1] { fileName }));
		}

		private void CloseToLoadDropSave(object[] param)
		{
			bool succeeded = false;
			if (DropScreen != null && ((CloseToSaveLoadScreen)DropScreen).Back)
			{
				succeeded = true;
			}
			base.LoadDropSave((string)param[0]);
			if (succeeded)
			{
				GameManager.GalCore.HideTitleScreen();
			}
		}

		protected override void OnPeripheryRender(object sender, RenderEventArgs e)
		{
			base.OnPeripheryRender(sender, e);
			Point point = GameGraphics.GameWindow.PointToClient(Cursor.Position);
			currentCursor.Position = new Point((int)(((float)point.X - GameGraphics.TargetBounds.X) / GameGraphics.TargetBounds.Width + 0.5f), (int)(((float)point.Y - GameGraphics.TargetBounds.Y) / GameGraphics.TargetBounds.Height + 0.5f));
			GameGraphics.DrawPicture(currentCursor);
		}

		protected override void OnFrameMove(object sender, RenderEventArgs e)
		{
			base.OnFrameMove(sender, e);
			if (returnTitleScreen && (CurrentScreen == null || !(CurrentScreen is MessageBoxScreen)))
			{
				ReturnTitleScreen();
			}
			if (formClosing && (CurrentScreen == null || !(CurrentScreen is MessageBoxScreen)))
			{
				Application.Exit();
			}
			if (GameVideo != null && GameVideo.IsPlaying)
			{
				picCursorAnime.Alpha = 0;
				picCursors[0].Alpha = 0;
			}
			picCursorAnime.Update(e.ElapsedTime);
			picCursors[0].Update(e.ElapsedTime);
			if (Pause || GameScript.CurrentGameScript == null || CurrentScreen != null)
			{
				currentCursor = picCursors[0];
			}
			if (mouseTime > 0f)
			{
				if (mouseTime < e.ElapsedTime)
				{
					mouseTime = 0f;
					mouseLocation.X += mouseChange.X * mouseTime;
					mouseLocation.Y += mouseChange.Y * mouseTime;
				}
				else
				{
					mouseTime -= e.ElapsedTime;
					mouseLocation.X += mouseChange.X * e.ElapsedTime;
					mouseLocation.Y += mouseChange.Y * e.ElapsedTime;
				}
				Cursor.Position = GameGraphics.GameWindow.PointToScreen(new Point((int)(mouseLocation.X + 0.5f), (int)(mouseLocation.Y + 0.5f)));
			}
			else if (currentCursor.Alpha == 255)
			{
				if (!_alwaysShowCursor)
				{
					if (Cursor.Position == lastMouse)
					{
						unmoveTime += e.ElapsedTime;
						if (unmoveTime > 10f)
						{
							picCursorAnime.BeginTransform(1f);
							picCursorAnime.AlphaTo(0);
							picCursorAnime.EndTransform();
							picCursors[0].BeginTransform(1f);
							picCursors[0].AlphaTo(0);
							picCursors[0].EndTransform();
							unmoveTime = 0f;
						}
					}
					else
					{
						unmoveTime = 0f;
					}
				}
			}
			else if (Cursor.Position != lastMouse)
			{
				unmoveTime = 0f;
				picCursorAnime.Alpha = 255;
				picCursors[0].Alpha = 255;
				if (currentCursor.Time > 0f)
				{
					picCursorAnime.Update(picCursorAnime.Time);
					picCursors[0].Update(picCursors[0].Time);
				}
			}
			lastMouse = Cursor.Position;
		}

		public void MoveMouse(Point location)
		{
			mouseTime = 0.15f;
			Point point = new Point((int)((float)location.X * GameGraphics.TargetBounds.Width + GameGraphics.TargetBounds.X + 0.5f), (int)((float)location.Y * GameGraphics.TargetBounds.Height + GameGraphics.TargetBounds.Y + 0.5f));
			Point point2 = GameGraphics.GameWindow.PointToClient(Cursor.Position);
			mouseLocation = new PointF(point2.X, point2.Y);
			mouseChange = new PointF((float)(point.X - point2.X) / mouseTime, (float)(point.Y - point2.Y) / mouseTime);
			picCursorAnime.Alpha = 255;
			picCursors[0].Alpha = 255;
			if (currentCursor.Time > 0f)
			{
				picCursorAnime.Update(picCursorAnime.Time);
				picCursors[0].Update(picCursors[0].Time);
			}
		}

		public void MoveMouseOffset(Point offset)
		{
			Point point = GameGraphics.GameWindow.PointToClient(Cursor.Position);
			Point location = new Point((int)((float)point.X + (float)offset.X * GameGraphics.TargetBounds.Width + 0.5f), (int)((float)point.Y + (float)offset.Y * GameGraphics.TargetBounds.Height + 0.5f));
			MoveMouse(location);
		}
	}
}
