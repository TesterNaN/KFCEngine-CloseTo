using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using KidFansChannel.GalCore.Commands;
using KidFansChannel.GalCore.Records;
using KidFansChannel.GalCore.Screens;
using KidFansChannel.Game2DCore;
using KidFansChannel.GameCore;

namespace KidFansChannel.GalCore
{
	public class GalCore
	{
		private struct Layer : IComparable
		{
			public int Major;

			public float Minor;

			public Layer(int major, float minor)
			{
				Major = major;
				Minor = minor;
			}

			public int CompareTo(object obj)
			{
				int compareResult = Major.CompareTo(((Layer)obj).Major);
				if (compareResult != 0)
				{
					return compareResult;
				}
				return Minor.CompareTo(((Layer)obj).Minor);
			}
		}

		private GameGraphics _gameGraphics;

		private GameSound _gameSound;

		private GameData _gameData;

		private GameLanguage _gameLanguage;

		private GameInput _gameInput;

		private GameOptions _gameOptions;

		private GameState _gameState;

		private GameVideo _gameVideo;

		private GameScript _gameScript;

		private GameFont _gameFont;

		private string _name;

		private string _icon;

		private bool _canExitNow;

		internal CanvasPicture ScenePicture;

		internal CanvasPicture TempPicture;

		private SaveLoadScreen _dropScreen;

		private PlayMode _currentPlayMode;

		private bool _toNormal;

		private PlayState _currentPlayState;

		private List<ScriptCommand> _currentCommandList = new List<ScriptCommand>();

		private List<ScriptCommand> _preparingCommandList = new List<ScriptCommand>();

		private List<ScriptCommand> _toPrepareCommandList = new List<ScriptCommand>();

		public ICanvas CurrentCanvas;

		private SortedList<Layer, IDrawable> normal = new SortedList<Layer, IDrawable>();

		private SortedList<Layer, IDrawable> canvas = new SortedList<Layer, IDrawable>();

		private int sceneIndex;

		private bool lastPause;

		private bool _hide;

		private bool _pause;

		private bool _hideString;

		private int currentShockNum;

		private Stack<GameScreen> screenStacks = new Stack<GameScreen>();

		private bool preparingScreen;

		private string titleStory;

		private string titleScript;

		private string titleChapter;

		private int titleScene;

		private string[] menuStack;

		private MessageBoxScreen currentMessageBox;

		private bool preparingMessageBox;

		private bool isComment;

		private string _comment;

		private bool isNextStopVideo;

		public GameGraphics GameGraphics
		{
			get
			{
				return _gameGraphics;
			}
		}

		public GameSound GameSound
		{
			get
			{
				return _gameSound;
			}
		}

		public GameData GameData
		{
			get
			{
				return _gameData;
			}
		}

		public GameLanguage GameLanguage
		{
			get
			{
				return _gameLanguage;
			}
		}

		public GameInput GameInput
		{
			get
			{
				return _gameInput;
			}
		}

		public GameOptions GameOptions
		{
			get
			{
				return _gameOptions;
			}
		}

		public GameState GameState
		{
			get
			{
				return _gameState;
			}
		}

		public GameVideo GameVideo
		{
			get
			{
				return _gameVideo;
			}
		}

		public GameScript GameScript
		{
			get
			{
				return _gameScript;
			}
		}

		public GameFont GameFont
		{
			get
			{
				return _gameFont;
			}
		}

		public string Name
		{
			get
			{
				return _name;
			}
			set
			{
				_name = value;
				_gameGraphics.GameWindow.Text = _gameLanguage.ResourceManager.GetString(_name);
			}
		}

		public string Icon
		{
			get
			{
				return _icon;
			}
			set
			{
				_icon = value;
				_gameGraphics.GameWindow.Icon = (Icon)_gameLanguage.ResourceManager.GetObject(_icon);
			}
		}

		public bool CanExitNow
		{
			get
			{
				return _canExitNow;
			}
			set
			{
				_canExitNow = value;
			}
		}

		protected SaveLoadScreen DropScreen
		{
			get
			{
				return _dropScreen;
			}
		}

		public PlayMode CurrentPlayMode
		{
			get
			{
				return _currentPlayMode;
			}
			internal set
			{
				if (value != PlayMode.Normal && value != PlayMode.Auto)
				{
					foreach (ScriptCommand currentCommand in _currentCommandList)
					{
						if (currentCommand is KidFansChannel.GalCore.Commands.Timer)
						{
							return;
						}
					}
				}
				_currentPlayMode = value;
			}
		}

		public bool ToNormal
		{
			get
			{
				return _toNormal;
			}
			internal set
			{
				_toNormal = value;
			}
		}

		public PlayState CurrentPlayState
		{
			get
			{
				return _currentPlayState;
			}
			internal set
			{
				_currentPlayState = value;
			}
		}

		public List<ScriptCommand> CurrentCommandList
		{
			get
			{
				return _currentCommandList;
			}
		}

		public List<ScriptCommand> PreparingCommandList
		{
			get
			{
				return _preparingCommandList;
			}
		}

		public List<ScriptCommand> ToPrepareCommandList
		{
			get
			{
				return _toPrepareCommandList;
			}
		}

		public bool WaitForClick
		{
			get
			{
				return _currentPlayState == PlayState.ToClick;
			}
		}

		public bool QuickRead
		{
			set
			{
				_hide = false;
				if (value)
				{
					foreach (ScriptCommand currentCommand in _currentCommandList)
					{
						if (currentCommand is KidFansChannel.GalCore.Commands.Timer)
						{
							return;
						}
					}
					_currentPlayMode = PlayMode.QuickRead;
					_toNormal = false;
				}
				else if (_currentPlayMode == PlayMode.QuickRead)
				{
					_toNormal = true;
				}
			}
		}

		public bool QuickAll
		{
			set
			{
				_hide = false;
				if (value)
				{
					foreach (ScriptCommand currentCommand in _currentCommandList)
					{
						if (currentCommand is KidFansChannel.GalCore.Commands.Timer)
						{
							return;
						}
					}
					_currentPlayMode = PlayMode.QuickAll;
					_toNormal = false;
				}
				else if (_currentPlayMode == PlayMode.QuickAll)
				{
					_toNormal = true;
				}
			}
		}

		public bool Hide
		{
			get
			{
				return _hide;
			}
		}

		public bool Pause
		{
			get
			{
				return _pause;
			}
			protected set
			{
				if (value)
				{
					GameState.StopTimer();
				}
				else if (GameScript.CurrentGameScript != null)
				{
					GameState.StartTimer();
				}
				_pause = value;
			}
		}

		public bool HideString
		{
			get
			{
				return _hideString;
			}
		}

		public GameScreen CurrentScreen
		{
			get
			{
				if (currentMessageBox != null)
				{
					return currentMessageBox;
				}
				if (screenStacks.Count > 0 && !screenStacks.Peek().Finished)
				{
					return screenStacks.Peek();
				}
				return null;
			}
		}

		public string Comment
		{
			get
			{
				return _comment;
			}
		}

		public GalCore(string name, string icon)
		{
			_name = name;
			_icon = icon;
		}

		public virtual void SetGameGraphics(GameGraphics gameGraphics)
		{
			if (_gameGraphics != null)
			{
				throw new InvalidOperationException();
			}
			_gameGraphics = gameGraphics;
			gameGraphics.SetCore(this);
		}

		public void SetGameSound(GameSound gameSound)
		{
			if (_gameSound != null || _gameGraphics == null)
			{
				throw new InvalidOperationException();
			}
			_gameSound = gameSound;
			gameSound.SetCore(this);
		}

		public void SetGameData(GameData gameData)
		{
			if (_gameData != null)
			{
				throw new InvalidOperationException();
			}
			_gameData = gameData;
			gameData.SetCore(this);
		}

		public void SetGameLanguage(GameLanguage gameLanguage)
		{
			if (_gameLanguage != null || _gameGraphics == null)
			{
				throw new InvalidOperationException();
			}
			_gameLanguage = gameLanguage;
			gameLanguage.SetCore(this);
			gameLanguage.LanguageChanged += LanguagedChanged;
			gameLanguage.LoadLanguages();
		}

		public void SetGameInput(GameInput gameInput)
		{
			if (_gameInput != null || _gameGraphics == null)
			{
				throw new InvalidOperationException();
			}
			_gameInput = gameInput;
			gameInput.SetCore(this);
		}

		public void SetGameOptions(GameOptions gameOptions)
		{
			if (_gameOptions != null)
			{
				throw new InvalidOperationException();
			}
			_gameOptions = gameOptions;
		}

		public void SetGameState(GameState gameState)
		{
			if (_gameState != null)
			{
				throw new InvalidOperationException();
			}
			_gameState = gameState;
			gameState.SetCore(this);
		}

		public void SetGameVideo(GameVideo gameVideo)
		{
			if (_gameVideo != null)
			{
				throw new InvalidOperationException();
			}
			_gameVideo = gameVideo;
			gameVideo.SetCore(this);
		}

		public void SetGameScript(GameScript gameScript)
		{
			if (_gameScript != null || _gameState == null)
			{
				throw new InvalidOperationException();
			}
			_gameScript = gameScript;
			gameScript.SetCore(this);
		}

		public void SetGameFont(GameFont gameFont)
		{
			if (_gameFont != null || _gameGraphics == null || _gameLanguage == null)
			{
				throw new InvalidOperationException();
			}
			_gameFont = gameFont;
			gameFont.SetCore(this);
			gameFont.LoadFont();
		}

		public virtual void Initialize()
		{
		}

		private void LanguagedChanged(object sender, LanguageEventArgs e)
		{
			ResetLanguage();
		}

		private void ResetLanguage()
		{
			_gameGraphics.GameWindow.Text = _gameLanguage.ResourceManager.GetString(_name);
			_gameGraphics.GameWindow.Icon = (Icon)_gameLanguage.ResourceManager.GetObject(_icon);
		}

		public void ShowGameWindow()
		{
			_gameGraphics.WindowResolution = _gameOptions.WindowResolution;
			_gameGraphics.FullscreenResolution = _gameOptions.FullscreenResolution;
			_gameGraphics.CreateWindow(_gameGraphics.WindowResolution.Size.Width, _gameGraphics.WindowResolution.Size.Height);
			_gameGraphics.CreateDevice((uint)_gameOptions.GraphicsDevice, !_gameOptions.FullScreen, _gameGraphics.FullscreenResolution.Size.Width, _gameGraphics.FullscreenResolution.Size.Height, _gameOptions.VSync);
			_gameGraphics.GraphicsInit();
			ScenePicture = _gameGraphics.CreateCanvasPicture(GameGraphics.BaseSize);
			TempPicture = _gameGraphics.CreateCanvasPicture(GameGraphics.BaseSize);
			PreparePause();
			_gameGraphics.FrameMove += OnFrameMove;
			GameGraphics gameGraphics = _gameGraphics;
			gameGraphics.FramePrepare = (RenderEventHandler)Delegate.Combine(gameGraphics.FramePrepare, new RenderEventHandler(OnFramePrepare));
			GameGraphics gameGraphics2 = _gameGraphics;
			gameGraphics2.FrameRender = (RenderEventHandler)Delegate.Combine(gameGraphics2.FrameRender, new RenderEventHandler(OnFrameRender));
			_gameGraphics.FrameCapture += OnFrameCapture;
			GameGraphics gameGraphics3 = _gameGraphics;
			gameGraphics3.PeripheryRender = (RenderEventHandler)Delegate.Combine(gameGraphics3.PeripheryRender, new RenderEventHandler(OnPeripheryRender));
			_gameGraphics.GameWindow.FormClosing += GameWindow_FormClosing;
			_gameGraphics.SetWndProcCallback(WndProc);
			_gameGraphics.GameWindow.AllowDrop = true;
			_gameGraphics.GameWindow.DragEnter += GameWindow_DragEnter;
			_gameGraphics.GameWindow.DragDrop += GameWindow_DragDrop;
		}

		private void GameWindow_DragEnter(object sender, DragEventArgs e)
		{
			if ((CurrentScreen != null || _currentPlayState != PlayState.ToClick || _pause) && (!(CurrentScreen is SaveLoadScreen) || ((SaveLoadScreen)CurrentScreen).IsSave || !((SaveLoadScreen)CurrentScreen).CanDrop))
			{
				return;
			}
			string[] droppedFiles = (string[])e.Data.GetData(DataFormats.FileDrop);
			if (droppedFiles != null)
			{
				if (droppedFiles.Length == 1 && droppedFiles[0].EndsWith("." + _gameState.SaveExt))
				{
					e.Effect = DragDropEffects.Move;
					_dropScreen = (SaveLoadScreen)CurrentScreen;
				}
			}
			else if (e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				e.Effect = DragDropEffects.Copy;
				_dropScreen = (SaveLoadScreen)CurrentScreen;
			}
			else
			{
				e.Effect = DragDropEffects.None;
			}
		}

		private void GameWindow_DragDrop(object sender, DragEventArgs e)
		{
			string[] droppedFiles = (string[])e.Data.GetData(DataFormats.FileDrop);
			string text = null;
			if (droppedFiles.Length == 0)
			{
				_dropScreen = null;
				return;
			}
			for (int i = 0; i < droppedFiles.Length; i++)
			{
				if (droppedFiles[i].EndsWith("." + _gameState.SaveExt))
				{
					text = droppedFiles[i];
				}
			}
			if (text == null)
			{
				_dropScreen = null;
			}
			else
			{
				LoadDropSave(text);
			}
		}

		protected virtual void LoadDropSave(string fileName)
		{
			_gameScript.LoadStory(fileName);
			if (_dropScreen != null)
			{
				_dropScreen.FinalScreen();
				_dropScreen = null;
			}
		}

		protected virtual void GameWindow_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (_gameScript.CurrentGameScript != null)
			{
				_gameScript.SaveLastGame();
			}
			_gameState.SaveState();
			_gameOptions.Save();
			_gameSound.Dispose();
		}

		public void MainLoop()
		{
			_gameState.LoadState();
			_gameGraphics.MainLoop();
		}

		protected virtual void OnFrameMove(object sender, RenderEventArgs e)
		{
			try
			{
				GameInput.ResponseInput(e.ElapsedTime);
				GameSound.Update(e.AppTime, e.ElapsedTime);
				GameGraphics.Update(e.AppTime, e.ElapsedTime);
				GameState.UpdateTime(e.AppTime);
				if (GameVideo != null && GameVideo.IsPlaying)
				{
					GameVideo.Update();
					return;
				}
				if (_gameScript.CurrentGameScript != null && !_pause && (currentMessageBox == null || !currentMessageBox.PauseBack))
				{
					List<ScriptCommand> executedCommands = new List<ScriptCommand>();
					bool shouldPrepare = true;
					foreach (ScriptCommand preparingCommand in _preparingCommandList)
					{
						if (preparingCommand is IInstanceDataUser && !((IInstanceDataUser)preparingCommand).DataReady)
						{
							shouldPrepare = false;
							break;
						}
					}
					if (shouldPrepare)
					{
						foreach (ScriptCommand preparingCommand2 in _preparingCommandList)
						{
							preparingCommand2.InitCommand();
						}
					}
					foreach (ScriptCommand currentCommand in _currentCommandList)
					{
						if (currentCommand.Finished)
						{
							executedCommands.Add(currentCommand);
						}
					}
					foreach (ScriptCommand item in executedCommands)
					{
						item.FinalCommand();
					}
					executedCommands.Clear();
					foreach (ScriptCommand currentCommand2 in _currentCommandList)
					{
						if (currentCommand2.Finished)
						{
							executedCommands.Add(currentCommand2);
						}
					}
					foreach (ScriptCommand item2 in executedCommands)
					{
						item2.FinalCommand();
					}
					executedCommands.Clear();
					foreach (ScriptCommand currentCommand3 in _currentCommandList)
					{
						if (!_preparingCommandList.Contains(currentCommand3) && currentCommand3 is IUpdatable)
						{
							((IUpdatable)currentCommand3).Update(e.AppTime, e.ElapsedTime);
							if (currentCommand3.Finished)
							{
								executedCommands.Add(currentCommand3);
							}
						}
					}
					foreach (ScriptCommand currentCommand4 in _currentCommandList)
					{
						if (currentCommand4 is IRecordable)
						{
							Record[] record = ((IRecordable)currentCommand4).GetRecord();
							if (record != null)
							{
								_gameState.CurrentGameState.AddRecord(record);
							}
						}
					}
					foreach (ScriptCommand item3 in executedCommands)
					{
						item3.FinalCommand();
					}
					executedCommands.Clear();
					foreach (ScriptCommand currentCommand5 in _currentCommandList)
					{
						if (currentCommand5.Finished)
						{
							executedCommands.Add(currentCommand5);
						}
					}
					foreach (ScriptCommand item4 in executedCommands)
					{
						item4.FinalCommand();
					}
					executedCommands.Clear();
					if (shouldPrepare)
					{
						_preparingCommandList.Clear();
					}
					_preparingCommandList.AddRange(_toPrepareCommandList);
					_toPrepareCommandList.Clear();
					switch (_currentPlayState)
					{
					case PlayState.Running:
					{
						bool flag2 = false;
						if (_preparingCommandList.Count == 0)
						{
							foreach (ScriptCommand currentCommand6 in _currentCommandList)
							{
								if (currentCommand6.Lasting)
								{
									flag2 = true;
								}
							}
						}
						else
						{
							flag2 = true;
						}
						if (flag2)
						{
							break;
						}
						List<ScriptCommand> list2 = _gameScript.CurrentGameScript.ProcessNextCommands();
						if (list2 == null)
						{
							ReturnTitleScreen(false);
							return;
						}
						foreach (ScriptCommand item5 in list2)
						{
							if (item5 is IInstanceDataUser)
							{
								((IInstanceDataUser)item5).InitInstanceData();
							}
							_preparingCommandList.Add(item5);
						}
						break;
					}
					}
					UpdateStateIcon(e.ElapsedTime);
					if (isComment)
					{
						UpdateComment(e.ElapsedTime);
					}
				}
				if (currentMessageBox == null || !currentMessageBox.PauseBack)
				{
					bool flag3 = false;
					if (lastPause && !_pause)
					{
						lastPause = false;
						GameInput.RefreshMouse();
					}
					if (_pause)
					{
						lastPause = true;
						if (UpdatePause(e.AppTime, e.ElapsedTime) && screenStacks.Count > 0)
						{
							flag3 = true;
						}
					}
					else if (_gameScript.CurrentGameScript == null)
					{
						if (screenStacks.Count > 0)
						{
							flag3 = true;
						}
					}
					else if (screenStacks.Count > 0)
					{
						flag3 = true;
					}
					if (flag3)
					{
						GameScreen gameScreen = screenStacks.Peek();
						if (gameScreen.Finished)
						{
							GameScreen gameScreen2 = screenStacks.Peek();
							if (gameScreen2 is TitleScreen)
							{
								menuStack = null;
							}
							screenStacks.Pop();
							while (screenStacks.Count > 0)
							{
								GameScreen gameScreen3 = screenStacks.Peek();
								if (!gameScreen3.Hide)
								{
									if (gameScreen3 is TitleScreen)
									{
										((TitleScreen)gameScreen3).SetMenuStack(menuStack);
									}
									ShowScreen(gameScreen3, false);
									break;
								}
								gameScreen2 = screenStacks.Peek();
								if (gameScreen2 is TitleScreen)
								{
									menuStack = null;
								}
								screenStacks.Pop();
							}
							if (screenStacks.Count == 0 && _gameScript.CurrentGameScript != null && _pause)
							{
								ChangePauseState(false);
							}
						}
						else if (gameScreen.DataReady)
						{
							if (preparingScreen)
							{
								preparingScreen = false;
								gameScreen.InitScreen();
							}
							else
							{
								gameScreen.Update(e.ElapsedTime);
							}
						}
					}
				}
				if (currentMessageBox == null)
				{
					return;
				}
				if (currentMessageBox.Finished)
				{
					currentMessageBox = null;
				}
				else if (currentMessageBox.DataReady)
				{
					if (preparingMessageBox)
					{
						preparingMessageBox = false;
						currentMessageBox.InitScreen();
					}
					else
					{
						currentMessageBox.Update(e.ElapsedTime);
					}
				}
			}
			catch (Exception ex)
			{
				if (MessageBox.Show(ex.Message + "\n" + ((ex.InnerException == null) ? "" : (ex.InnerException.Message + "\n")) + ex.Source + "\n" + ex.StackTrace + "\n", "Exception", MessageBoxButtons.OKCancel) == DialogResult.OK)
				{
					_canExitNow = true;
					Application.Exit();
				}
			}
		}

		protected void OnFramePrepare(object sender, RenderEventArgs e)
		{
			try
			{
				if (GameVideo != null && GameVideo.IsPlaying)
				{
					return;
				}
				if (_gameScript.CurrentGameScript != null && (currentMessageBox == null || !currentMessageBox.PauseBack))
				{
					sceneIndex = -1;
					for (int i = 0; i < _currentCommandList.Count; i++)
					{
						if (_currentCommandList[i] is Scene && (!((Scene)_currentCommandList[i]).Continue || ((Scene)_currentCommandList[i]).Load))
						{
							sceneIndex = i;
							break;
						}
					}
					if (sceneIndex == -1)
					{
						foreach (ScriptCommand currentCommand in _currentCommandList)
						{
							if (currentCommand is IDrawable)
							{
								IDrawable drawable = currentCommand as IDrawable;
								try
								{
									normal.Add(new Layer(drawable.LayerMajor, drawable.LayerMinor), drawable);
								}
								catch (Exception)
								{
									throw;
								}
							}
						}
					}
					else
					{
						for (int j = 0; j < _currentCommandList.Count; j++)
						{
							if (!(_currentCommandList[j] is IDrawable))
							{
								continue;
							}
							IDrawable drawable2 = _currentCommandList[j] as IDrawable;
							try
							{
								if (j > sceneIndex && drawable2.LayerMajor < ((IDrawable)_currentCommandList[sceneIndex]).LayerMajor)
								{
									canvas.Add(new Layer(drawable2.LayerMajor, drawable2.LayerMinor), drawable2);
								}
								else
								{
									normal.Add(new Layer(drawable2.LayerMajor, drawable2.LayerMinor), drawable2);
								}
							}
							catch (Exception)
							{
								throw;
							}
						}
					}
					if (sceneIndex != -1 && GameOptions.Graphics != GameOptions.Level.Low)
					{
						CurrentCanvas = ScenePicture;
						ScenePicture.DrawBegin();
						foreach (KeyValuePair<Layer, IDrawable> canva in canvas)
						{
							canva.Value.Draw();
						}
						ScenePicture.DrawEnd();
					}
				}
				if (currentMessageBox == null || currentMessageBox.PauseBack || screenStacks.Count <= 0)
				{
					return;
				}
				CurrentCanvas = TempPicture;
				TempPicture.DrawBegin();
				foreach (KeyValuePair<Layer, IDrawable> item in normal)
				{
					if (!item.Value.CanHide)
					{
						_hideString = true;
						item.Value.Draw();
						_hideString = false;
					}
				}
				normal.Clear();
				canvas.Clear();
				if (_pause)
				{
					DrawPause();
				}
				GameScreen gameScreen = screenStacks.Peek();
				if (gameScreen.DataReady)
				{
					gameScreen.Draw();
				}
				TempPicture.DrawEnd();
			}
			catch (Exception ex3)
			{
				if (MessageBox.Show(ex3.Message + "\n" + ((ex3.InnerException == null) ? "" : (ex3.InnerException.Message + "\n")) + ex3.Source + "\n" + ex3.StackTrace + "\n", "Exception", MessageBoxButtons.OKCancel) == DialogResult.OK)
				{
					_canExitNow = true;
					Application.Exit();
				}
			}
		}

		protected virtual void OnFrameRender(object sender, RenderEventArgs e)
		{
			try
			{
				if (GameVideo != null && GameVideo.IsPlaying)
				{
					CurrentCanvas = GameGraphics;
					GameVideo.Draw();
					return;
				}
				CurrentCanvas = GameGraphics;
				if (_gameScript.CurrentGameScript != null && (currentMessageBox == null || !currentMessageBox.PauseBack) && (currentMessageBox == null || screenStacks.Count <= 0))
				{
					foreach (KeyValuePair<Layer, IDrawable> item in normal)
					{
						if (_pause || _hide || currentMessageBox != null)
						{
							if (!item.Value.CanHide)
							{
								_hideString = true;
								item.Value.Draw();
								_hideString = false;
							}
						}
						else
						{
							item.Value.Draw();
						}
					}
					normal.Clear();
					canvas.Clear();
					if (!_pause)
					{
						if (currentMessageBox == null)
						{
							DrawStateIcon();
						}
					}
					else
					{
						DrawPause();
					}
				}
				if (screenStacks.Count > 0)
				{
					if (currentMessageBox == null)
					{
						GameScreen gameScreen = screenStacks.Peek();
						if (gameScreen.DataReady)
						{
							gameScreen.Draw();
						}
					}
					else if (!currentMessageBox.PauseBack)
					{
						GameGraphics.DrawPicture(TempPicture);
					}
				}
				if (currentMessageBox != null && currentMessageBox.DataReady)
				{
					currentMessageBox.Draw();
				}
			}
			catch (Exception ex)
			{
				if (MessageBox.Show(ex.Message + "\n" + ((ex.InnerException == null) ? "" : (ex.InnerException.Message + "\n")) + ex.Source + "\n" + ex.StackTrace + "\n", "Exception", MessageBoxButtons.OKCancel) == DialogResult.OK)
				{
					_canExitNow = true;
					Application.Exit();
				}
			}
		}

		protected void OnFrameCapture(object sender, RenderEventArgs e)
		{
			try
			{
				SortedList<Layer, IDrawable> sortedList = new SortedList<Layer, IDrawable>();
				if (_gameScript.CurrentGameScript == null)
				{
					return;
				}
				sceneIndex = -1;
				for (int i = 0; i < _currentCommandList.Count; i++)
				{
					if (_currentCommandList[i] is Scene && (!((Scene)_currentCommandList[i]).Continue || ((Scene)_currentCommandList[i]).Load))
					{
						sceneIndex = i;
						break;
					}
				}
				if (sceneIndex == -1)
				{
					foreach (ScriptCommand currentCommand in _currentCommandList)
					{
						if (currentCommand is IDrawable && !((IDrawable)currentCommand).CanHide)
						{
							IDrawable drawable = currentCommand as IDrawable;
							try
							{
								sortedList.Add(new Layer(drawable.LayerMajor, drawable.LayerMinor), drawable);
							}
							catch (Exception)
							{
								throw;
							}
						}
					}
				}
				else
				{
					for (int j = 0; j < _currentCommandList.Count; j++)
					{
						if (_currentCommandList[j] is IDrawable && !((IDrawable)_currentCommandList[j]).CanHide)
						{
							IDrawable drawable2 = _currentCommandList[j] as IDrawable;
							if (j <= sceneIndex || drawable2.LayerMajor >= ((IDrawable)_currentCommandList[sceneIndex]).LayerMajor)
							{
								sortedList.Add(new Layer(drawable2.LayerMajor, drawable2.LayerMinor), drawable2);
							}
						}
					}
				}
				foreach (KeyValuePair<Layer, IDrawable> item in sortedList)
				{
					_hideString = true;
					item.Value.Draw();
					_hideString = false;
				}
				sortedList.Clear();
			}
			catch (Exception ex2)
			{
				if (MessageBox.Show(ex2.Message + "\n" + ((ex2.InnerException == null) ? "" : (ex2.InnerException.Message + "\n")) + ex2.Source + "\n" + ex2.StackTrace + "\n", "Exception", MessageBoxButtons.OKCancel) == DialogResult.OK)
				{
					_canExitNow = true;
					Application.Exit();
				}
			}
		}

		protected virtual void OnPeripheryRender(object sender, RenderEventArgs e)
		{
			CurrentCanvas = GameGraphics;
			if ((GameVideo == null || (!GameVideo.IsPlaying && _gameScript.CurrentGameScript != null)) && !_pause && !_hide && currentMessageBox == null)
			{
				if (_gameGraphics.Resolution.StateArea != Rectangle.Empty)
				{
					GameState.DrawState();
				}
				if (isComment && _gameGraphics.Resolution.CommentArea != Rectangle.Empty)
				{
					DrawComment();
				}
			}
		}

		public void Click()
		{
			_hide = false;
			foreach (ScriptCommand currentCommand in _currentCommandList)
			{
				if (currentCommand is IClickable && ((IClickable)currentCommand).OnItemClick())
				{
					break;
				}
			}
		}

		public void Move(Point location)
		{
			foreach (ScriptCommand currentCommand in _currentCommandList)
			{
				if (currentCommand is IClickable)
				{
					((IClickable)currentCommand).OnItemMove(location);
				}
			}
		}

		public void QuickOnce()
		{
			foreach (ScriptCommand currentCommand in _currentCommandList)
			{
				if (currentCommand is KidFansChannel.GalCore.Commands.Timer)
				{
					return;
				}
			}
			_hide = false;
			_currentPlayMode = PlayMode.QuickOnce;
		}

		public void ToggleAuto()
		{
			_hide = false;
			bool flag = _currentPlayMode == PlayMode.Auto;
			_currentPlayMode = ((!flag) ? PlayMode.Auto : PlayMode.Normal);
		}

		public void ToggleQuick()
		{
			_hide = false;
			if (_currentPlayMode == PlayMode.Quick)
			{
				_toNormal = true;
				return;
			}
			foreach (ScriptCommand currentCommand in _currentCommandList)
			{
				if (currentCommand is KidFansChannel.GalCore.Commands.Timer)
				{
					return;
				}
			}
			_currentPlayMode = PlayMode.Quick;
			_toNormal = false;
		}

		public void ToggleHide()
		{
			_hide = !_hide;
		}

		public void TogglePause()
		{
			if (_pause)
			{
				ChangePauseState(false);
			}
			else if (_currentPlayState == PlayState.ToClick)
			{
				ChangePauseState(true);
				ShowPauseScreen();
			}
		}

		protected virtual void ChangePauseState(bool value)
		{
			_hide = false;
			Pause = value;
			GameSound.StopVoice();
			GameSound.ChangeVolumeTempBgm(1f);
		}

		protected virtual bool UpdatePause(double appTime, float elaspedTime)
		{
			return true;
		}

		protected virtual void DrawPause()
		{
		}

		protected virtual void PreparePause()
		{
		}

		public void StartShock()
		{
			switch (_gameOptions.Shock)
			{
			case GameOptions.ShockMethod.Gamepad:
				_gameInput.StartShock();
				break;
			case GameOptions.ShockMethod.Window:
				_gameGraphics.StartShock();
				break;
			case GameOptions.ShockMethod.GamepadAndWindow:
				_gameInput.StartShock();
				_gameGraphics.StartShock();
				break;
			}
			currentShockNum++;
		}

		public void StopShock()
		{
			StopShock(false);
		}

		public void StopShock(bool all)
		{
			if (currentShockNum == 0)
			{
				return;
			}
			if (all)
			{
				currentShockNum = 0;
			}
			else
			{
				currentShockNum--;
			}
			if (currentShockNum == 0)
			{
				switch (_gameOptions.Shock)
				{
				case GameOptions.ShockMethod.Gamepad:
					_gameInput.StopShock();
					break;
				case GameOptions.ShockMethod.Window:
					_gameGraphics.StopShock();
					break;
				case GameOptions.ShockMethod.GamepadAndWindow:
					_gameInput.StopShock();
					_gameGraphics.StopShock();
					break;
				case GameOptions.ShockMethod.None:
					break;
				}
			}
		}

		public void ShowScreen(GameScreen screen)
		{
			ShowScreen(screen, true);
		}

		public void ShowScreen(GameScreen screen, bool newScreen)
		{
			if (_gameScript.CurrentGameScript != null && !_pause)
			{
				ChangePauseState(true);
			}
			if (newScreen)
			{
				if (screenStacks.Count > 0)
				{
					GameScreen gameScreen = screenStacks.Peek();
					if (gameScreen is TitleScreen)
					{
						menuStack = ((TitleScreen)gameScreen).GetMenuStack();
					}
					gameScreen.FinalScreen();
				}
				screenStacks.Push(screen);
			}
			else
			{
				screen.Finished = false;
			}
			screen.InitData();
			preparingScreen = true;
		}

		public void ExitScreen()
		{
			screenStacks.Peek().FinalScreen();
		}

		public virtual void ShowSaveLoadScreen(bool isSave)
		{
			if (isSave)
			{
				_gameScript.SaveStory(0, null, 0);
			}
			else
			{
				_gameScript.LoadStory(0);
			}
		}

		public virtual void ShowMessageScreen()
		{
		}

		public virtual void ShowPauseScreen()
		{
		}

		public virtual void ShowOptionsScreen()
		{
		}

		public virtual void ShowTitleScreen()
		{
		}

		public virtual void ShowEncyListScreen()
		{
		}

		public virtual void ShowEncyContentScreen(int index)
		{
		}

		public virtual void ShowAlbumScreen()
		{
		}

		public virtual void ShowMusicScreen()
		{
		}

		public virtual void ShowMovieScreen()
		{
		}

		public virtual void ShowClearListScreen()
		{
		}

		public virtual void ShowShortcutScreen()
		{
		}

		public virtual void ShowLogoScreen()
		{
		}

		public void HideTitleScreen()
		{
			foreach (GameScreen screenStack in screenStacks)
			{
				if (screenStack is TitleScreen)
				{
					((TitleScreen)screenStack).HideTitle();
				}
			}
		}

		public void ReturnTitleScreen()
		{
			ReturnTitleScreen(true);
		}

		public virtual void ReturnTitleScreen(bool save)
		{
			if (_gameScript.CurrentGameScript != null)
			{
				if (save)
				{
					_gameScript.SaveLastGame();
				}
				List<ScriptCommand> list = new List<ScriptCommand>();
				list.AddRange(_currentCommandList);
				foreach (ScriptCommand item in list)
				{
					item.FinishCommand();
					item.FinalCommand();
				}
				list.Clear();
				_gameScript.StopStory();
				GC.Collect();
			}
			screenStacks.Clear();
			if (titleStory == null)
			{
				ShowTitleScreen();
				return;
			}
			_gameScript.StartStory(titleStory, false);
			_gameScript.CurrentGameScript.JumpScene(titleScript, titleChapter, titleScene);
			titleStory = (titleScript = (titleChapter = null));
			titleScene = -1;
		}

		public void SetReturnTitleStory(string story, string script, string chapter, int scene)
		{
			titleStory = story;
			titleScript = script;
			titleChapter = chapter;
			titleScene = scene;
		}

		public virtual void ShowMessageBox(MessageBoxScreen messageBox)
		{
			currentMessageBox = messageBox;
			currentMessageBox.InitData();
			preparingMessageBox = true;
		}

		public void Snap()
		{
			if (!Directory.Exists(Path.Combine(_gameState.SaveFullPath, "Snaps")))
			{
				try
				{
					Directory.CreateDirectory(Path.Combine(_gameState.SaveFullPath, "Snaps"));
				}
				catch (Exception ex)
				{
					MessageBox.Show("An Error occured while creating snap directory.\n" + ex.Message);
				}
			}
			for (int i = 0; i < 1000; i++)
			{
				if (!File.Exists(Path.Combine(_gameState.SaveFullPath, "Snaps\\Snap" + i.ToString("000") + ".png")))
				{
					try
					{
						GameGraphics.Snap(Path.Combine(_gameState.SaveFullPath, "Snaps\\Snap" + i.ToString("000") + ".png"));
						break;
					}
					catch (Exception ex2)
					{
						MessageBox.Show("An Error occured while snaping.\n" + ex2.Message);
						break;
					}
				}
			}
		}

		protected virtual void UpdateStateIcon(float elapsedTime)
		{
		}

		protected virtual void DrawStateIcon()
		{
		}

		public virtual void ShowComment(string comment)
		{
			isComment = true;
			_comment = comment;
		}

		public virtual void HideComment()
		{
			isComment = false;
		}

		protected virtual void UpdateComment(float elapsedTime)
		{
		}

		protected virtual void DrawComment()
		{
		}

		public void NextStopVideo()
		{
			isNextStopVideo = true;
		}

		protected virtual IntPtr WndProc(IntPtr hWnd, KidFansChannel.GameCore.NativeMethods.WindowMessage msg, IntPtr wParam, IntPtr lParam, ref bool NoFurtherProcessing)
		{
			if (msg == (KidFansChannel.GameCore.NativeMethods.WindowMessage)16507u)
			{
				if (GameVideo.IsPlaying)
				{
					GameVideo.HandleGraphicEvent();
				}
				return IntPtr.Zero;
			}
			bool flag = isNextStopVideo;
			isNextStopVideo = false;
			if (flag && GameVideo.IsPlaying)
			{
				GameVideo.Stop();
			}
			return IntPtr.Zero;
		}
	}
}
