using System;
using System.Drawing;
using System.Windows.Forms;
using KidFansChannel.InputCore;

namespace KidFansChannel.GalCore
{
	public class GameInput : KidFansChannel.InputCore.InputCore
	{
		public enum Actions
		{
			Ok,
			Cancel,
			SkipRead,
			SkipAll,
			Auto,
			Quick,
			Hide,
			Message,
			PageUp,
			PageDown,
			Option,
			Save,
			Load,
			QuickSave,
			QuickLoad,
			Snap,
			Test
		}

		private GalCore galCore;

		private PointF mousePosition;

		private Point lastMouseLocation = Point.Empty;

		public void SetCore(GalCore gc)
		{
			galCore = gc;
			galCore.GameGraphics.MouseDown += OnMouseDown;
			galCore.GameGraphics.MouseUp += OnMouseUp;
			galCore.GameGraphics.MouseMove += OnMouseMove;
			galCore.GameGraphics.MouseWheel += OnMouseWheel;
		}

		public void SetGamePad(GamePadInput.GamePadInstance gamePadInstance)
		{
			Initialize(gamePadInstance, galCore.GameGraphics.GameWindow);
			SetUpMap();
		}

		protected virtual void SetUpMap()
		{
			AddMap(0, galCore.GameOptions.GamePadMap.OK);
			AddMap(0, KeyBoardInput.Key.Return);
			AddMap(1, galCore.GameOptions.GamePadMap.Cancel);
			AddMap(1, KeyBoardInput.Key.Space);
			AddMap(2, galCore.GameOptions.GamePadMap.SkipRead);
			AddMap(2, KeyBoardInput.Key.LeftControl);
			AddMap(3, galCore.GameOptions.GamePadMap.SkipAll);
			AddMap(3, KeyBoardInput.Key.RightControl);
			AddMap(4, galCore.GameOptions.GamePadMap.Auto);
			AddMap(4, KeyBoardInput.Key.F4);
			AddMap(5, galCore.GameOptions.GamePadMap.Quick);
			AddMap(5, KeyBoardInput.Key.F3);
			AddMap(6, galCore.GameOptions.GamePadMap.Hide);
			AddMap(6, KeyBoardInput.Key.F2);
			AddMap(7, galCore.GameOptions.GamePadMap.Message);
			AddMap(8, KeyBoardInput.Key.PageUp);
			AddMap(8, galCore.GameOptions.GamePadMap.PageUp);
			AddMap(9, KeyBoardInput.Key.Next);
			AddMap(9, galCore.GameOptions.GamePadMap.PageDown);
			AddMap(10, KeyBoardInput.Key.F9);
			AddMap(11, KeyBoardInput.Key.F5);
			AddMap(12, KeyBoardInput.Key.F6);
			AddMap(13, KeyBoardInput.Key.F7);
			AddMap(14, KeyBoardInput.Key.F8);
			AddMap(15, KeyBoardInput.Key.F10);
			FinishMap();
		}

		public void ResponseInput(float elapsedTime)
		{
			if (galCore.GameGraphics.WindowActive)
			{
				Update();
			}
			else
			{
				Clear();
			}
			if (galCore.GameVideo != null && galCore.GameVideo.IsPlaying)
			{
				if (CheckPress(1) && galCore.GameVideo.IsPlaying)
				{
					galCore.NextStopVideo();
					Message message = Message.Create(galCore.GameGraphics.GameWindow.Handle, 0, IntPtr.Zero, IntPtr.Zero);
					NativeMethods.PostMessage(message.HWnd, message.Msg, message.WParam, message.LParam);
				}
			}
			else
			{
				if (galCore.CurrentScreen == null)
				{
					if (CheckPress(1))
					{
						if (galCore.CurrentPlayMode == PlayMode.Normal)
						{
							galCore.TogglePause();
						}
						else if (galCore.CurrentPlayMode == PlayMode.Auto)
						{
							galCore.ToggleAuto();
						}
						else if (galCore.CurrentPlayMode == PlayMode.Quick)
						{
							galCore.ToggleQuick();
						}
					}
					if (CheckPress(0))
					{
						if (galCore.WaitForClick)
						{
							galCore.Click();
						}
						else
						{
							galCore.QuickOnce();
						}
					}
					if (CheckDown(3))
					{
						galCore.QuickAll = true;
					}
					else if (CheckDown(2))
					{
						galCore.QuickRead = true;
					}
					else if (galCore.CurrentPlayMode == PlayMode.QuickAll || galCore.CurrentPlayMode == PlayMode.QuickRead)
					{
						galCore.ToNormal = true;
					}
					if (CheckPress(0) && !galCore.WaitForClick)
					{
						galCore.QuickOnce();
					}
					if (CheckPress(4))
					{
						galCore.ToggleAuto();
					}
					if (CheckPress(5))
					{
						galCore.ToggleQuick();
					}
					if (CheckPress(6))
					{
						if (galCore.CurrentPlayMode != PlayMode.Normal)
						{
							galCore.CurrentPlayMode = PlayMode.Normal;
						}
						else
						{
							galCore.ToggleHide();
						}
					}
					if (CheckPress(8) || CheckPress(9) || (CheckPress(7) && galCore.CurrentPlayState == PlayState.ToClick && !galCore.Pause))
					{
						galCore.ShowMessageScreen();
					}
					if (CheckPress(10) && galCore.CurrentPlayState == PlayState.ToClick && !galCore.Pause)
					{
						galCore.ShowOptionsScreen();
					}
					if (CheckPress(11) && galCore.CurrentPlayState == PlayState.ToClick && !galCore.Pause)
					{
						galCore.ShowSaveLoadScreen(true);
					}
					if (CheckPress(12) && galCore.CurrentPlayState == PlayState.ToClick && !galCore.Pause)
					{
						galCore.ShowSaveLoadScreen(false);
					}
					if (CheckPress(13) && galCore.CurrentPlayState == PlayState.ToClick && !galCore.Pause)
					{
						galCore.GameScript.QuickSave(QuickSaveType.Quick);
					}
					if (CheckPress(14) && galCore.CurrentPlayState == PlayState.ToClick && !galCore.Pause)
					{
						galCore.GameScript.QuickLoad();
					}
				}
				else if (galCore.CurrentScreen.Initialized)
				{
					if (CheckPress(0))
					{
						galCore.CurrentScreen.OnKeyPress(Keys.Return);
					}
					if (CheckPress(1))
					{
						galCore.CurrentScreen.OnKeyPress(Keys.Space);
					}
					if (CheckPress(8))
					{
						galCore.CurrentScreen.OnKeyPress(Keys.Prior);
					}
					if (CheckPress(9))
					{
						galCore.CurrentScreen.OnKeyPress(Keys.Next);
					}
					if (CheckDirectionPress(0))
					{
						galCore.CurrentScreen.OnKeyPress(Keys.Left);
					}
					if (CheckDirectionPress(1))
					{
						galCore.CurrentScreen.OnKeyPress(Keys.Right);
					}
					if (CheckDirectionPress(2))
					{
						galCore.CurrentScreen.OnKeyPress(Keys.Up);
					}
					if (CheckDirectionPress(3))
					{
						galCore.CurrentScreen.OnKeyPress(Keys.Down);
					}
					if (CheckPress(11))
					{
						galCore.CurrentScreen.OnKeyPress(Keys.F5);
					}
					if (CheckPress(12))
					{
						galCore.CurrentScreen.OnKeyPress(Keys.F6);
					}
				}
				if (CheckPress(15))
				{
					galCore.Snap();
				}
			}
			Point point = galCore.GameGraphics.GameWindow.PointToClient(Cursor.Position);
			if ((double)Math.Abs(mousePosition.X - (float)point.X) > 0.5 || (double)Math.Abs(mousePosition.Y - (float)point.Y) > 0.5)
			{
				mousePosition = new PointF(point.X, point.Y);
			}
			mousePosition = new PointF(mousePosition.X + elapsedTime * CheckPosition().X * 500f * galCore.GameGraphics.TargetBounds.Width, mousePosition.Y + elapsedTime * CheckPosition().Y * 500f * galCore.GameGraphics.TargetBounds.Height);
			Cursor.Position = galCore.GameGraphics.GameWindow.PointToScreen(new Point((int)((mousePosition.X > 0f) ? (mousePosition.X + 0.5f) : (mousePosition.X - 0.5f)), (int)((mousePosition.Y > 0f) ? (mousePosition.Y + 0.5f) : (mousePosition.Y - 0.5f))));
		}

		public void OnMouseDown(object sender, MouseEventArgs e)
		{
			lastMouseLocation = e.Location;
			try
			{
				if (galCore.GameVideo != null && galCore.GameVideo.IsPlaying)
				{
					if (e.Button == MouseButtons.Right && galCore.GameVideo.IsPlaying)
					{
						galCore.GameVideo.Stop();
					}
					return;
				}
				switch (e.Button)
				{
				case MouseButtons.Left:
					if (galCore.CurrentScreen == null)
					{
						if (galCore.WaitForClick)
						{
							galCore.Move(e.Location);
							galCore.Click();
						}
						else if (galCore.CurrentPlayMode == PlayMode.Normal)
						{
							galCore.QuickOnce();
						}
					}
					else if (galCore.CurrentScreen.Initialized)
					{
						galCore.CurrentScreen.OnMouseDown(e.Location);
					}
					break;
				case MouseButtons.Right:
					if (galCore.CurrentScreen == null)
					{
						if (galCore.CurrentPlayMode == PlayMode.Normal)
						{
							galCore.TogglePause();
						}
						else if (galCore.CurrentPlayMode == PlayMode.Auto)
						{
							galCore.ToggleAuto();
						}
						else if (galCore.CurrentPlayMode == PlayMode.Quick)
						{
							galCore.ToggleQuick();
						}
					}
					else if (galCore.CurrentScreen.Initialized)
					{
						galCore.CurrentScreen.OnKeyPress(Keys.Space);
					}
					break;
				}
			}
			catch (Exception ex)
			{
				if (MessageBox.Show(ex.Message + "\n" + ((ex.InnerException == null) ? "" : (ex.InnerException.Message + "\n")) + ex.Source + "\n" + ex.StackTrace + "\n", "Exception", MessageBoxButtons.OKCancel) == DialogResult.OK)
				{
					galCore.CanExitNow = true;
					Application.Exit();
				}
			}
		}

		public void OnMouseUp(object sender, MouseEventArgs e)
		{
			lastMouseLocation = e.Location;
			try
			{
				if ((galCore.GameVideo == null || !galCore.GameVideo.IsPlaying) && e.Button == MouseButtons.Left && galCore.CurrentScreen != null && galCore.CurrentScreen.Initialized)
				{
					galCore.CurrentScreen.OnMouseUp(e.Location);
				}
			}
			catch (Exception ex)
			{
				if (MessageBox.Show(ex.Message + "\n" + ((ex.InnerException == null) ? "" : (ex.InnerException.Message + "\n")) + ex.Source + "\n" + ex.StackTrace + "\n", "Exception", MessageBoxButtons.OKCancel) == DialogResult.OK)
				{
					galCore.CanExitNow = true;
					Application.Exit();
				}
			}
		}

		public void OnMouseMove(object sender, MouseEventArgs e)
		{
			if (Math.Abs(e.Location.X - lastMouseLocation.X) > 1 || Math.Abs(e.Location.Y - lastMouseLocation.Y) > 1)
			{
				lastMouseLocation = e.Location;
				OnMouseMove(e.Location);
			}
		}

		protected void OnMouseMove(Point location)
		{
			try
			{
				if (galCore.GameVideo != null && galCore.GameVideo.IsPlaying)
				{
					return;
				}
				if (galCore.CurrentScreen == null)
				{
					if (galCore.WaitForClick)
					{
						galCore.Move(location);
					}
				}
				else if (galCore.CurrentScreen.Initialized)
				{
					galCore.CurrentScreen.OnMouseMove(location);
				}
			}
			catch (Exception ex)
			{
				if (MessageBox.Show(ex.Message + "\n" + ((ex.InnerException == null) ? "" : (ex.InnerException.Message + "\n")) + ex.Source + "\n" + ex.StackTrace + "\n", "Exception", MessageBoxButtons.OKCancel) == DialogResult.OK)
				{
					galCore.CanExitNow = true;
					Application.Exit();
				}
			}
		}

		public void RefreshMouse()
		{
			OnMouseMove(lastMouseLocation);
		}

		public void OnMouseWheel(object sender, MouseEventArgs e)
		{
			try
			{
				if (galCore.GameVideo != null && galCore.GameVideo.IsPlaying)
				{
					return;
				}
				if (galCore.CurrentScreen == null)
				{
					if (galCore.CurrentPlayState == PlayState.ToClick && !galCore.Pause)
					{
						galCore.ShowMessageScreen();
					}
				}
				else if (galCore.CurrentScreen.Initialized)
				{
					galCore.CurrentScreen.OnMouseWheel(e.Delta);
				}
			}
			catch (Exception ex)
			{
				if (MessageBox.Show(ex.Message + "\n" + ((ex.InnerException == null) ? "" : (ex.InnerException.Message + "\n")) + ex.Source + "\n" + ex.StackTrace + "\n", "Exception", MessageBoxButtons.OKCancel) == DialogResult.OK)
				{
					galCore.CanExitNow = true;
					Application.Exit();
				}
			}
		}
	}
}
