using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using KidFansChannel.DataCore;
using KidFansChannel.GalCore.Commands;
using KidFansChannel.GalCore.Defines;
using KidFansChannel.Game2DCore;

namespace KidFansChannel.GalCore
{
	public class GameState : IDisposable
	{
		public class Message
		{
			public string Text;

			public int Voice;

			public Message(string Text, int Voice)
			{
			}
		}

		public class DrawStateItem
		{
			public string Chapter;

			public string BgName;

			public string BgmName;

			public float ReadPercentage;

			public string GameDateTime;

			public int CurrentTime;

			public int TotalTime;

			public void Clear()
			{
				Chapter = null;
				BgName = null;
				BgmName = null;
				ReadPercentage = 0f;
				GameDateTime = "??/?? ??:??";
				CurrentTime = 0;
				TotalTime = 0;
			}
		}

		protected struct InfoHeader
		{
			public uint Size;

			public uint StringReadOffset;

			public int StringReadLength;

			public uint SelectChosenOffset;

			public int SelectChosenLength;

			public uint EndingOffset;

			public int EndingLength;

			public uint AlbumOffset;

			public int AlbumLength;

			public uint MusicOffset;

			public int MusicLength;

			public uint MovieOffset;

			public int MovieLength;

			public uint EncyOpenOffset;

			public int EncyOpenLength;

			public uint EncyViewOffset;

			public int EncyViewLength;

			public int Time;

			public uint GlobalVarOffset;

			public int GlobalVarLength;
		}

		public class RichSaveGames
		{
			protected const int MaxLength = 1024;

			protected const int MagicNumber = 1213024082;

			protected const int HeaderVersion = 1;

			public const int HeaderSize = 8232;

			public long ThumbnailOffset;

			public int ThumbnailSize;

			public Guid GameId;

			public string GameName;

			public string SaveName;

			public string LevelName;

			public string Comments;

			public void Write(BinaryWriter bw)
			{
				bw.BaseStream.Position = 0L;
				bw.Write(1213024082);
				bw.Write(1);
				bw.Write(8232);
				bw.Write(ThumbnailOffset);
				bw.Write(ThumbnailSize);
				bw.Write(GameId.ToByteArray());
				if (GameName == null)
				{
					GameName = string.Empty;
				}
				bw.Write(GameName.ToCharArray(0, Math.Min(1024, GameName.Length)));
				if (GameName.Length < 1024)
				{
					for (int i = 0; i < 1024 - GameName.Length; i++)
					{
						bw.Write('\0');
					}
				}
				if (SaveName == null)
				{
					SaveName = string.Empty;
				}
				bw.Write(SaveName.ToCharArray(0, Math.Min(1024, SaveName.Length)));
				if (SaveName.Length < 1024)
				{
					for (int j = 0; j < 1024 - SaveName.Length; j++)
					{
						bw.Write('\0');
					}
				}
				if (LevelName == null)
				{
					LevelName = string.Empty;
				}
				bw.Write(LevelName.ToCharArray(0, Math.Min(1024, LevelName.Length)));
				if (LevelName.Length < 1024)
				{
					for (int k = 0; k < 1024 - LevelName.Length; k++)
					{
						bw.Write('\0');
					}
				}
				if (Comments == null)
				{
					Comments = string.Empty;
				}
				bw.Write(Comments.ToCharArray(0, Math.Min(1024, Comments.Length)));
				if (Comments.Length < 1024)
				{
					for (int l = 0; l < 1024 - Comments.Length; l++)
					{
						bw.Write('\0');
					}
				}
			}

			public void Read(BinaryReader br)
			{
				br.BaseStream.Position = 0L;
				if (br.ReadInt32() != 1213024082)
				{
					throw new InvalidDataException();
				}
				if (br.ReadInt32() != 1)
				{
					throw new InvalidDataException();
				}
				if (br.ReadInt32() != 8232)
				{
					throw new InvalidDataException();
				}
				ThumbnailOffset = br.ReadInt64();
				ThumbnailSize = br.ReadInt32();
				if (new Guid(br.ReadBytes(16)) != GameId)
				{
					throw new InvalidDataException();
				}
				StringBuilder stringBuilder = new StringBuilder();
				char[] charBuffer = br.ReadChars(1024);
				for (int i = 0; i < 1024 && charBuffer[i] != '\0'; i++)
				{
					stringBuilder.Append(charBuffer[i]);
				}
				if (stringBuilder.ToString() != GameName)
				{
					throw new InvalidDataException();
				}
				stringBuilder = new StringBuilder();
				charBuffer = br.ReadChars(1024);
				for (int j = 0; j < 1024 && charBuffer[j] != '\0'; j++)
				{
					stringBuilder.Append(charBuffer[j]);
				}
				SaveName = stringBuilder.ToString();
				stringBuilder = new StringBuilder();
				charBuffer = br.ReadChars(1024);
				for (int k = 0; k < 1024 && charBuffer[k] != '\0'; k++)
				{
					stringBuilder.Append(charBuffer[k]);
				}
				LevelName = stringBuilder.ToString();
				stringBuilder = new StringBuilder();
				charBuffer = br.ReadChars(1024);
				for (int l = 0; l < 1024 && charBuffer[l] != '\0'; l++)
				{
					stringBuilder.Append(charBuffer[l]);
				}
				Comments = stringBuilder.ToString();
			}
		}

		protected String2D StateString;

		private GalCore _galCore;

		private GameInstanceState _currentGameState;

		private Dictionary<string, VarDefine> _globalVarDefines = new Dictionary<string, VarDefine>();

		protected Dictionary<string, bool[]> StringReads = new Dictionary<string, bool[]>();

		protected Dictionary<string, int> StringReadNum = new Dictionary<string, int>();

		protected Dictionary<string, bool> SelectChosens = new Dictionary<string, bool>();

		protected bool[] EncyOpens;

		protected bool[] EncyViews;

		protected bool[] Albums;

		protected bool[] Musics;

		protected bool[] Movies;

		protected bool[] Endings;

		protected float Time;

		private double baseTime;

		private double stopTime;

		private bool timerStopped = true;

		private double lastAppTime;

		protected List<GameState> Histories = new List<GameState>();

		protected List<Message> Messages = new List<Message>();

		private Queue<string> stringTexts = new Queue<string>();

		private Queue<string> stringChapters = new Queue<string>();

		private Queue<int> stringIndexs = new Queue<int>();

		private int _lastStringIndex;

		private string _lastString;

		public DrawStateItem Item = new DrawStateItem();

		private string _saveFolder;

		public string SaveFullPath;

		public string GameSaveName;

		private string _saveExt;

		private string _infoExt;

		private bool _isCreatingState;

		private Guid _guid;

		private string _gameName;

		public string ConfigString;

		public string ConfigLevel;

		public string ConfigComment;

		public RichSaveGames FileHeader = new RichSaveGames();

		public GalCore GalCore
		{
			get
			{
				return _galCore;
			}
		}

		public GameInstanceState CurrentGameState
		{
			get
			{
				return _currentGameState;
			}
		}

		public Dictionary<string, VarDefine> GlobalVarDefines
		{
			get
			{
				return _globalVarDefines;
			}
		}

		public bool IsTimerStopped
		{
			get
			{
				return timerStopped;
			}
		}

		public int LastStringIndex
		{
			get
			{
				return _lastStringIndex;
			}
		}

		public string LastString
		{
			get
			{
				return _lastString;
			}
		}

		public string SaveFolder
		{
			get
			{
				return _saveFolder;
			}
			set
			{
				_saveFolder = value;
				if (Environment.OSVersion.Version.Major >= 6)
				{
					StringBuilder stringBuilder = new StringBuilder(260);
					NativeMethods.SHGetFolderPath(IntPtr.Zero, 40, IntPtr.Zero, 0, stringBuilder);
					SaveFullPath = Path.Combine(stringBuilder.ToString() + "\\Saved Games", _saveFolder);
				}
				else
				{
					SaveFullPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "\\My Games", _saveFolder);
				}
			}
		}

		public string SaveExt
		{
			get
			{
				return _saveExt;
			}
			set
			{
				_saveExt = value;
			}
		}

		public string InfoExt
		{
			get
			{
				return _infoExt;
			}
			set
			{
				_infoExt = value;
			}
		}

		public bool IsCreatingState
		{
			get
			{
				return _isCreatingState;
			}
		}

		public Guid Guid
		{
			get
			{
				return _guid;
			}
			set
			{
				_guid = value;
				FileHeader.GameId = _guid;
			}
		}

		public string GameName
		{
			get
			{
				return _gameName;
			}
			set
			{
				_gameName = value;
				FileHeader.GameName = _gameName;
			}
		}

		public void SetCore(GalCore galCore)
		{
			_galCore = galCore;
			_galCore.GameGraphics.FullscreenToggled += GameGraphics_FullscreenToggled;
			InitDrawString();
		}

		private void GameGraphics_FullscreenToggled(object sender, EventArgs e)
		{
			InitDrawString();
		}

		public void StartTimer()
		{
			if (timerStopped)
			{
				baseTime += lastAppTime - stopTime;
				stopTime = 0.0;
			}
			timerStopped = false;
		}

		public void StopTimer()
		{
			if (!timerStopped)
			{
				stopTime = lastAppTime;
				timerStopped = true;
			}
		}

		public void UpdateTime(double appTime)
		{
			lastAppTime = appTime;
			double now = ((!timerStopped) ? appTime : stopTime);
			Time = (float)(now - baseTime);
		}

		public void InitNewState(string storyName)
		{
			if (_currentGameState != null)
			{
				_currentGameState.Dispose();
			}
			_currentGameState = new GameInstanceState(this, storyName);
		}

		public void UpdateString(string text, int index)
		{
			if (!string.IsNullOrEmpty(_currentGameState.StoryName) && !string.IsNullOrEmpty(_currentGameState.Chapter))
			{
				if (!StringReads[_currentGameState.StoryName + "_" + _currentGameState.Chapter][index])
				{
					stringChapters.Enqueue(_currentGameState.StoryName + "_" + _currentGameState.Chapter);
					stringIndexs.Enqueue(index);
				}
				else
				{
					stringChapters.Enqueue(null);
					stringIndexs.Enqueue(index);
				}
				stringTexts.Enqueue(text);
			}
		}

		public void ConfirmString()
		{
			while (stringTexts.Count > 0)
			{
				_lastString = stringTexts.Dequeue();
				_lastStringIndex = stringIndexs.Dequeue();
				string text = stringChapters.Dequeue();
				if (text != null)
				{
					StringReads[text][_lastStringIndex] = true;
					StringReadNum[text]++;
				}
			}
		}

		public void ClearLastString()
		{
			stringTexts.Clear();
			stringChapters.Clear();
			stringIndexs.Clear();
			_lastString = string.Empty;
			_lastStringIndex = -1;
		}

		public void UpdateSelect(string title, string caseSelected)
		{
			if (!string.IsNullOrEmpty(_currentGameState.StoryName))
			{
				SelectChosens[_currentGameState.StoryName + "_" + title + "_" + caseSelected] = true;
			}
		}

		public bool CheckString(int index)
		{
			if (!string.IsNullOrEmpty(_currentGameState.StoryName) && !string.IsNullOrEmpty(_currentGameState.Chapter))
			{
				if (StringReads[_currentGameState.StoryName + "_" + _currentGameState.Chapter].Length <= index)
				{
					bool found = false;
					foreach (string key in StringReads.Keys)
					{
						if (found)
						{
							return StringReads[key][0];
						}
						if (key == _currentGameState.StoryName + "_" + _currentGameState.Chapter)
						{
							found = true;
						}
					}
					return true;
				}
				return StringReads[_currentGameState.StoryName + "_" + _currentGameState.Chapter][index];
			}
			return false;
		}

		public bool CheckSelect(string title, string caseSelected)
		{
			if (!string.IsNullOrEmpty(_currentGameState.StoryName))
			{
				return SelectChosens[_currentGameState.StoryName + "_" + title + "_" + caseSelected];
			}
			return false;
		}

		public void InitAddString(string story, string chapter, int number)
		{
			StringReads.Add(story + "_" + chapter, new bool[number]);
			StringReadNum.Add(story + "_" + chapter, 0);
		}

		public void InitAddSelect(string story, string title, string[] cases)
		{
			foreach (string text in cases)
			{
				SelectChosens.Add(story + "_" + title + "_" + text, false);
			}
		}

		public void UpdateEncyOpen(int index)
		{
			EncyOpens[index] = true;
		}

		public void UpdateEncyView(int index)
		{
			EncyViews[index] = true;
		}

		public void InitEncy(int number)
		{
			EncyOpens = new bool[number];
			EncyViews = new bool[number];
		}

		public bool CheckEncyView(int index)
		{
			return EncyViews[index];
		}

		public bool CheckEncyOpen(int index)
		{
			return EncyOpens[index];
		}

		public int GetEncyNumber()
		{
			return EncyOpens.Length;
		}

		public virtual bool CheckChapterOpen(string story, string chapter)
		{
			return StringReadNum[story + "_" + chapter] > 0;
		}

		public virtual int GetChapterNumber()
		{
			return StringReads.Count;
		}

		public void UpdateAlbum(int index)
		{
			Albums[index] = true;
		}

		public int CountEndings()
		{
			int count = 0;
			for (int i = 0; i < Endings.Length; i++)
			{
				if (Endings[i])
				{
					count++;
				}
			}
			return count;
		}

		public virtual int CountChapters()
		{
			int count = 0;
			foreach (KeyValuePair<string, int> item in StringReadNum)
			{
				if (item.Value > 0)
				{
					count++;
				}
			}
			return count;
		}

		public int CountStringsTotal()
		{
			int count = 0;
			foreach (KeyValuePair<string, bool[]> stringRead in StringReads)
			{
				count += stringRead.Value.Length;
			}
			return count;
		}

		public int CountStringsRead()
		{
			int count = 0;
			foreach (KeyValuePair<string, int> item in StringReadNum)
			{
				count += item.Value;
			}
			return count;
		}

		public int CountSelectsTotal()
		{
			return SelectChosens.Count;
		}

		public int CountSelectsRead()
		{
			int count = 0;
			foreach (KeyValuePair<string, bool> selectChosen in SelectChosens)
			{
				if (selectChosen.Value)
				{
					count++;
				}
			}
			return count;
		}

		public bool CheckAlbum(int index)
		{
			return Albums[index];
		}

		public int CountAlbums()
		{
			int count = 0;
			for (int i = 0; i < Albums.Length; i++)
			{
				if (Albums[i])
				{
					count++;
				}
			}
			return count;
		}

		public int CountEncys()
		{
			int count = 0;
			for (int i = 0; i < EncyOpens.Length; i++)
			{
				if (EncyOpens[i])
				{
					count++;
				}
			}
			return count;
		}

		public void InitAlbum(int number)
		{
			Albums = new bool[number];
		}

		public void UpdateEnding(int index)
		{
			Endings[index] = true;
		}

		public void InitEnding(int number)
		{
			Endings = new bool[number];
		}

		public bool CheckEnding(int index)
		{
			return Endings[index];
		}

		public int GetEndingNumber()
		{
			return Endings.Length;
		}

		public void InitMusic(int number)
		{
			Musics = new bool[number];
		}

		public void UpdateMusic(int index)
		{
			Musics[index] = true;
		}

		public bool CheckMusic(int index)
		{
			return Musics[index];
		}

		public int CountMusics()
		{
			int count = 0;
			for (int i = 0; i < Musics.Length; i++)
			{
				if (Musics[i])
				{
					count++;
				}
			}
			return count;
		}

		public int GetMusicNumber()
		{
			return Musics.Length;
		}

		public void InitMovie(int number)
		{
			Movies = new bool[number];
		}

		public void UpdateMovie(int index)
		{
			Movies[index] = true;
		}

		public bool CheckMovie(int index)
		{
			return Movies[index];
		}

		public int CountMovies()
		{
			int count = 0;
			for (int i = 0; i < Movies.Length; i++)
			{
				if (Movies[i])
				{
					count++;
				}
			}
			return count;
		}

		public int GetMovieNumber()
		{
			return Movies.Length;
		}

		public float GetTime()
		{
			return Time;
		}

		protected virtual void InitDrawString()
		{
			if (!(GalCore.GameGraphics.Resolution.StateArea == Rectangle.Empty))
			{
				if (StateString == null)
				{
					StateString = new String2D();
					StateString.DefaultColor = Color.White;
				}
				RectangleF targetBounds = GalCore.GameGraphics.TargetBounds;
				Rectangle stateArea = GalCore.GameGraphics.Resolution.StateArea;
				StateString.Location = new Point((int)(((float)stateArea.X - targetBounds.X) / targetBounds.Width + 0.5f), (int)(((float)stateArea.Y - targetBounds.Y) / targetBounds.Height + 0.5f));
				StateString.LineSize = new Size((int)((float)stateArea.Width / targetBounds.Width + 0.5f), StateString.LineSize.Height);
				StateString.LineNum = (int)((float)stateArea.Height / targetBounds.Height + 0.5f) / StateString.LineSize.Height;
			}
		}

		public virtual bool DrawState()
		{
			StateString.Canvas = GalCore.CurrentCanvas;
			if (_galCore.GameGraphics.Resolution.StateArea == Rectangle.Empty)
			{
				return false;
			}
			UpdateDrawState();
			return true;
		}

		public virtual void UpdateDrawState()
		{
			Item.Clear();
			if (_currentGameState == null)
			{
				return;
			}
			Item.Chapter = _currentGameState.Chapter;
			if (Item.Chapter == string.Empty)
			{
				Item.Chapter = null;
			}
			else
			{
				Item.Chapter = GalCore.GameScript.CurrentGameScript.GetChapterName(Item.Chapter);
			}
			foreach (ScriptCommand currentCommand in _galCore.CurrentCommandList)
			{
				if (currentCommand is BgDisplay && ((BgDisplay)currentCommand).Layer == 0)
				{
					Item.BgName = ((BgDisplay)currentCommand).BgName;
				}
				if (currentCommand is BgmPlay)
				{
					Item.BgmName = ((BgmPlay)currentCommand).BgmName;
				}
			}
			if (!string.IsNullOrEmpty(_currentGameState.StoryName) && !string.IsNullOrEmpty(_currentGameState.Chapter))
			{
				Item.ReadPercentage = GetStringPercentage(_currentGameState.StoryName, _currentGameState.Chapter);
			}
			Item.GameDateTime = _currentGameState.GameDateTime;
			Item.CurrentTime = (int)_currentGameState.GetTime();
			Item.TotalTime = (int)Time;
		}

		public virtual float GetStringPercentage(string story, string chapter)
		{
			return (float)StringReadNum[story + "_" + chapter] / (float)StringReads[story + "_" + chapter].Length;
		}

		public string GetSaveFileName(int index)
		{
			return Path.Combine(SaveFullPath, "save" + index.ToString("00") + "." + _saveExt);
		}

		public string GetInfoFileName(int index)
		{
			return Path.Combine(SaveFullPath, "save" + index.ToString("00") + "." + _infoExt);
		}

		public string GetTextFileName(int index)
		{
			return Path.Combine(SaveFullPath, "save" + index.ToString("00") + ".txt");
		}

		public virtual void GetSaveInfo(int index, out int icon, out string comment, out string level, out DateTime saveTime, out Picture snap)
		{
			if (!File.Exists(GetSaveFileName(index)))
			{
				icon = -1;
				comment = null;
				level = null;
				saveTime = default(DateTime);
				snap = null;
				return;
			}
			try
			{
				using (FileStream fileStream = new FileStream(GetSaveFileName(index), FileMode.Open))
				{
					using (BinaryReader binaryReader = new BinaryReader(fileStream, Encoding.Unicode))
					{
						FileHeader.Read(binaryReader);
						GameInstanceState.InfoHeader infoHeader = default(GameInstanceState.InfoHeader);
						infoHeader.Size = binaryReader.ReadUInt32();
						infoHeader.StoryNameOffset = binaryReader.ReadUInt32();
						infoHeader.StoryNameLength = binaryReader.ReadInt32();
						infoHeader.SelectOffset = binaryReader.ReadUInt32();
						infoHeader.SelectLength = binaryReader.ReadInt32();
						infoHeader.VarOffset = binaryReader.ReadUInt32();
						infoHeader.VarLength = binaryReader.ReadInt32();
						infoHeader.PlayTime = binaryReader.ReadInt32();
						infoHeader.SaveTime = binaryReader.ReadInt64();
						infoHeader.Icon = binaryReader.ReadInt32();
						infoHeader.CommandOffset = binaryReader.ReadUInt32();
						infoHeader.CommandLength = binaryReader.ReadInt32();
						saveTime = new DateTime(infoHeader.SaveTime);
						icon = infoHeader.Icon;
						comment = FileHeader.Comments;
						level = FileHeader.LevelName;
						DataStream dataStream;
						snap = GalCore.GameGraphics.CreatePictureInstant(dataStream = new DataStream(fileStream, 8232 + FileHeader.ThumbnailOffset, FileHeader.ThumbnailSize));
						dataStream.Close();
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show("An Error occured while loading save.\n" + ex.Message);
				icon = -1;
				comment = null;
				level = null;
				saveTime = default(DateTime);
				snap = null;
			}
		}

		public virtual void GetSaveInfo(int index, out int icon)
		{
			if (!File.Exists(GetSaveFileName(index)))
			{
				icon = -1;
				return;
			}
			try
			{
				using (FileStream input = new FileStream(GetSaveFileName(index), FileMode.Open))
				{
					using (BinaryReader binaryReader = new BinaryReader(input, Encoding.Unicode))
					{
						FileHeader.Read(binaryReader);
						GameInstanceState.InfoHeader infoHeader = default(GameInstanceState.InfoHeader);
						infoHeader.Size = binaryReader.ReadUInt32();
						infoHeader.StoryNameOffset = binaryReader.ReadUInt32();
						infoHeader.StoryNameLength = binaryReader.ReadInt32();
						infoHeader.SelectOffset = binaryReader.ReadUInt32();
						infoHeader.SelectLength = binaryReader.ReadInt32();
						infoHeader.VarOffset = binaryReader.ReadUInt32();
						infoHeader.VarLength = binaryReader.ReadInt32();
						infoHeader.PlayTime = binaryReader.ReadInt32();
						infoHeader.SaveTime = binaryReader.ReadInt64();
						infoHeader.Icon = binaryReader.ReadInt32();
						infoHeader.CommandOffset = binaryReader.ReadUInt32();
						infoHeader.CommandLength = binaryReader.ReadInt32();
						icon = infoHeader.Icon;
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show("An Error occured while loading save.\n" + ex.Message);
				icon = -1;
			}
		}

		public void LoadState()
		{
			LoadState(Path.Combine(SaveFullPath, "save_sys." + _saveExt));
		}

		public void LoadState(string fileName)
		{
			if (!File.Exists(fileName))
			{
				_isCreatingState = true;
				SaveState();
				_isCreatingState = false;
				return;
			}
			try
			{
				using (FileStream input = new FileStream(fileName, FileMode.Open, FileAccess.Read))
				{
					using (BinaryReader binaryReader = new BinaryReader(input, Encoding.Unicode))
					{
						FileHeader.Read(binaryReader);
						InfoHeader infoHeader = default(InfoHeader);
						infoHeader.Size = binaryReader.ReadUInt32();
						if (infoHeader.Size == Marshal.SizeOf(infoHeader))
						{
							if (infoHeader.Size > 1)
							{
								infoHeader.StringReadOffset = binaryReader.ReadUInt32();
							}
							if (infoHeader.Size > 2)
							{
								infoHeader.StringReadLength = binaryReader.ReadInt32();
							}
							if (infoHeader.Size > 3)
							{
								infoHeader.SelectChosenOffset = binaryReader.ReadUInt32();
							}
							if (infoHeader.Size > 4)
							{
								infoHeader.SelectChosenLength = binaryReader.ReadInt32();
							}
							if (infoHeader.Size > 5)
							{
								infoHeader.EndingOffset = binaryReader.ReadUInt32();
							}
							if (infoHeader.Size > 6)
							{
								infoHeader.EndingLength = binaryReader.ReadInt32();
							}
							if (infoHeader.Size > 7)
							{
								infoHeader.AlbumOffset = binaryReader.ReadUInt32();
							}
							if (infoHeader.Size > 8)
							{
								infoHeader.AlbumLength = binaryReader.ReadInt32();
							}
							if (infoHeader.Size > 9)
							{
								infoHeader.MusicOffset = binaryReader.ReadUInt32();
							}
							if (infoHeader.Size > 10)
							{
								infoHeader.MusicLength = binaryReader.ReadInt32();
							}
							if (infoHeader.Size > 11)
							{
								infoHeader.MovieOffset = binaryReader.ReadUInt32();
							}
							if (infoHeader.Size > 12)
							{
								infoHeader.MovieLength = binaryReader.ReadInt32();
							}
							if (infoHeader.Size > 13)
							{
								infoHeader.EncyOpenOffset = binaryReader.ReadUInt32();
							}
							if (infoHeader.Size > 14)
							{
								infoHeader.EncyOpenLength = binaryReader.ReadInt32();
							}
							if (infoHeader.Size > 15)
							{
								infoHeader.EncyViewOffset = binaryReader.ReadUInt32();
							}
							if (infoHeader.Size > 16)
							{
								infoHeader.EncyViewLength = binaryReader.ReadInt32();
							}
							if (infoHeader.Size > 17)
							{
								infoHeader.Time = binaryReader.ReadInt32();
							}
							if (infoHeader.Size > 18)
							{
								infoHeader.GlobalVarOffset = binaryReader.ReadUInt32();
							}
							if (infoHeader.Size > 19)
							{
								infoHeader.GlobalVarLength = binaryReader.ReadInt32();
							}
							baseTime = -infoHeader.Time;
							binaryReader.BaseStream.Position = 8232 + infoHeader.StringReadOffset;
							int entryCount = binaryReader.ReadInt32();
							uint[] offsets = new uint[entryCount];
							int[] lengths = new int[entryCount];
							for (int i = 0; i < entryCount; i++)
							{
								offsets[i] = binaryReader.ReadUInt32();
								lengths[i] = binaryReader.ReadInt32();
							}
							int index = 0;
							foreach (bool[] value in StringReads.Values)
							{
								binaryReader.BaseStream.Position = 8232 + offsets[index];
								LoadArrays(binaryReader, value, lengths[index]);
								index++;
							}
							binaryReader.BaseStream.Position = 8232 + infoHeader.SelectChosenOffset;
							int selectCount = binaryReader.ReadInt32();
							uint[] selectOffsets = new uint[selectCount];
							int[] selectLengths = new int[selectCount];
							for (int j = 0; j < selectCount; j++)
							{
								selectOffsets[j] = binaryReader.ReadUInt32();
								selectLengths[j] = binaryReader.ReadInt32();
							}
							int selectIndex = -1;
							string lastKey = null;
							int bitIndex = 7;
							byte b = 0;
							int itemIndex = 0;
							List<string> chosen = new List<string>();
							foreach (KeyValuePair<string, bool> selectChosen in SelectChosens)
							{
								string key = selectChosen.Key;
								key = key.Substring(0, key.IndexOf('_'));
								if (lastKey != key)
								{
									lastKey = key;
									selectIndex++;
									binaryReader.BaseStream.Position = 8232 + selectOffsets[selectIndex];
									itemIndex = 0;
									if (itemIndex >= selectLengths[selectIndex])
									{
										break;
									}
									b = binaryReader.ReadByte();
									itemIndex++;
									bitIndex = 7;
								}
								if (((b >> bitIndex) & 1) == 1)
								{
									chosen.Add(selectChosen.Key);
								}
								bitIndex--;
								if (bitIndex < 0)
								{
									if (itemIndex >= selectLengths[selectIndex])
									{
										break;
									}
									b = binaryReader.ReadByte();
									itemIndex++;
									bitIndex = 7;
								}
							}
							foreach (string item in chosen)
							{
								SelectChosens[item] = true;
							}
							binaryReader.BaseStream.Position = 8232 + infoHeader.EndingOffset;
							LoadArrays(binaryReader, Endings, infoHeader.EndingLength);
							binaryReader.BaseStream.Position = 8232 + infoHeader.AlbumOffset;
							LoadArrays(binaryReader, Albums, infoHeader.AlbumLength);
							binaryReader.BaseStream.Position = 8232 + infoHeader.MusicOffset;
							LoadArrays(binaryReader, Musics, infoHeader.MusicLength);
							binaryReader.BaseStream.Position = 8232 + infoHeader.MovieOffset;
							LoadArrays(binaryReader, Movies, infoHeader.MovieLength);
							binaryReader.BaseStream.Position = 8232 + infoHeader.EncyOpenOffset;
							LoadArrays(binaryReader, EncyOpens, infoHeader.EncyOpenLength);
							binaryReader.BaseStream.Position = 8232 + infoHeader.EncyViewOffset;
							LoadArrays(binaryReader, EncyViews, infoHeader.EncyViewLength);
							binaryReader.BaseStream.Position = 8232 + infoHeader.GlobalVarOffset;
							int storyIndex = 0;
							foreach (VarDefine value2 in _globalVarDefines.Values)
							{
								if (storyIndex < infoHeader.GlobalVarLength)
								{
									value2.Value = binaryReader.ReadByte();
									if (value2.Value >= 128)
									{
										value2.Value -= 256;
									}
									storyIndex++;
									continue;
								}
								break;
							}
						}
					}
				}
				foreach (KeyValuePair<string, bool[]> stringRead in StringReads)
				{
					int readCount = 0;
					for (int k = 0; k < stringRead.Value.Length; k++)
					{
						if (stringRead.Value[k])
						{
							readCount++;
						}
					}
					StringReadNum[stringRead.Key] = readCount;
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show("An Error occured while loading state.\n" + ex.Message);
			}
		}

		private void LoadArrays(BinaryReader br, bool[] datas, int size)
		{
			byte b = 0;
			for (int i = 0; i < size; i++)
			{
				b = br.ReadByte();
				for (int j = 0; j < 8; j++)
				{
					if (i * 8 + j >= datas.Length)
					{
						return;
					}
					datas[i * 8 + j] = ((b >> 7 - j) & 1) == 1;
				}
			}
		}

		public virtual void SaveState()
		{
			SaveState(Path.Combine(SaveFullPath, "save_sys." + _saveExt));
		}

		public void SaveState(string fileName)
		{
			try
			{
				if (!Directory.Exists(Path.GetDirectoryName(fileName)))
				{
					Directory.CreateDirectory(Path.GetDirectoryName(fileName));
				}
				using (FileStream fileStream = new FileStream(fileName, FileMode.Create, FileAccess.Write))
				{
					using (BinaryWriter binaryWriter = new BinaryWriter(fileStream, Encoding.Unicode))
					{
						FileHeader.SaveName = ConfigString;
						FileHeader.LevelName = string.Format(ConfigLevel, CountEndings(), Endings.Length, (double)CountStringsRead() / (double)CountStringsTotal());
						FileHeader.Comments = ConfigComment;
						Stream stream;
						switch (_galCore.GameData.CheckData("system", "save_sys.png"))
						{
						case KidFansChannel.DataCore.DataCore.DataType.Package:
							stream = _galCore.GameData.GetStream();
							break;
						case KidFansChannel.DataCore.DataCore.DataType.File:
							stream = new FileStream(_galCore.GameData.GetFile(), FileMode.Open);
							break;
						default:
							throw new FileNotFoundException(null, Path.Combine("system", "save_sys.png"));
						}
						byte[] thumbnail = new byte[stream.Length];
						stream.Read(thumbnail, 0, thumbnail.Length);
						stream.Close();
						InfoHeader infoHeader = default(InfoHeader);
						FileHeader.ThumbnailOffset = Marshal.SizeOf(infoHeader);
						FileHeader.ThumbnailSize = thumbnail.Length;
						FileHeader.Write(binaryWriter);
						uint offset = 0u;
						offset = (infoHeader.StringReadOffset = (infoHeader.Size = (uint)Marshal.SizeOf(infoHeader)) + (uint)((thumbnail.Length + 3) / 4 * 4));
						List<int> stringCounts = new List<int>();
						foreach (bool[] value in StringReads.Values)
						{
							stringCounts.Add((value.Length + 7) / 8);
						}
						int totalStrings = 0;
						foreach (int item in stringCounts)
						{
							totalStrings += item;
						}
						infoHeader.StringReadLength = 4 + 8 * stringCounts.Count + totalStrings;
						offset = (infoHeader.SelectChosenOffset = offset + (uint)((infoHeader.StringReadLength + 3) / 4 * 4));
						List<int> selectCounts = new List<int>();
						string lastKey = null;
						foreach (KeyValuePair<string, bool> selectChosen in SelectChosens)
						{
							string key = selectChosen.Key;
							key = key.Substring(0, key.IndexOf('_'));
							if (lastKey != key)
							{
								lastKey = key;
								selectCounts.Add(1);
							}
							else
							{
								selectCounts[selectCounts.Count - 1]++;
							}
						}
						for (int i = 0; i < selectCounts.Count; i++)
						{
							selectCounts[i] = (selectCounts[i] + 7) / 8;
						}
						int totalSelects = 0;
						foreach (int item2 in selectCounts)
						{
							totalSelects += item2;
						}
						infoHeader.SelectChosenLength = 4 + 8 * selectCounts.Count + totalSelects;
						offset = (infoHeader.EndingOffset = offset + (uint)((infoHeader.SelectChosenLength + 3) / 4 * 4));
						infoHeader.EndingLength = (Endings.Length + 7) / 8;
						offset = (infoHeader.AlbumOffset = offset + (uint)((infoHeader.EndingLength + 3) / 4 * 4));
						infoHeader.AlbumLength = (Albums.Length + 7) / 8;
						offset = (infoHeader.MusicOffset = offset + (uint)((infoHeader.AlbumLength + 3) / 4 * 4));
						infoHeader.MusicLength = (Musics.Length + 7) / 8;
						offset = (infoHeader.MovieOffset = offset + (uint)((infoHeader.MusicLength + 3) / 4 * 4));
						infoHeader.MovieLength = (Movies.Length + 7) / 8;
						offset = (infoHeader.EncyOpenOffset = offset + (uint)((infoHeader.MovieLength + 3) / 4 * 4));
						infoHeader.EncyOpenLength = (EncyOpens.Length + 7) / 8;
						offset = (infoHeader.EncyViewOffset = offset + (uint)((infoHeader.EncyOpenLength + 3) / 4 * 4));
						infoHeader.EncyViewLength = (EncyViews.Length + 7) / 8;
						infoHeader.Time = (int)Time;
						offset = (infoHeader.GlobalVarOffset = offset + (uint)((infoHeader.EncyViewLength + 3) / 4 * 4));
						infoHeader.GlobalVarLength = _globalVarDefines.Count;
						binaryWriter.Write(infoHeader.Size);
						binaryWriter.Write(infoHeader.StringReadOffset);
						binaryWriter.Write(infoHeader.StringReadLength);
						binaryWriter.Write(infoHeader.SelectChosenOffset);
						binaryWriter.Write(infoHeader.SelectChosenLength);
						binaryWriter.Write(infoHeader.EndingOffset);
						binaryWriter.Write(infoHeader.EndingLength);
						binaryWriter.Write(infoHeader.AlbumOffset);
						binaryWriter.Write(infoHeader.AlbumLength);
						binaryWriter.Write(infoHeader.MusicOffset);
						binaryWriter.Write(infoHeader.MusicLength);
						binaryWriter.Write(infoHeader.MovieOffset);
						binaryWriter.Write(infoHeader.MovieLength);
						binaryWriter.Write(infoHeader.EncyOpenOffset);
						binaryWriter.Write(infoHeader.EncyOpenLength);
						binaryWriter.Write(infoHeader.EncyViewOffset);
						binaryWriter.Write(infoHeader.EncyViewLength);
						binaryWriter.Write(infoHeader.Time);
						binaryWriter.Write(infoHeader.GlobalVarOffset);
						binaryWriter.Write(infoHeader.GlobalVarLength);
						fileStream.Write(thumbnail, 0, thumbnail.Length);
						for (int j = thumbnail.Length; j < (thumbnail.Length + 3) / 4 * 4; j++)
						{
							binaryWriter.Write((byte)0);
						}
						offset = infoHeader.StringReadOffset;
						binaryWriter.Write(stringCounts.Count);
						offset += (uint)(4 + 8 * stringCounts.Count);
						for (int k = 0; k < stringCounts.Count; k++)
						{
							binaryWriter.Write(offset);
							binaryWriter.Write(stringCounts[k]);
							offset += (uint)stringCounts[k];
						}
						byte b = 0;
						int bitIndex = 7;
						foreach (bool[] value2 in StringReads.Values)
						{
							for (int l = 0; l < value2.Length; l++)
							{
								if (value2[l])
								{
									b |= (byte)(1 << bitIndex);
								}
								bitIndex--;
								if (bitIndex < 0)
								{
									binaryWriter.Write(b);
									b = 0;
									bitIndex = 7;
								}
							}
							if (bitIndex < 7)
							{
								binaryWriter.Write(b);
								b = 0;
								bitIndex = 7;
							}
						}
						for (int m = totalStrings; m < (totalStrings + 3) / 4 * 4; m++)
						{
							binaryWriter.Write((byte)0);
						}
						offset = infoHeader.SelectChosenOffset;
						binaryWriter.BaseStream.Position = 8232 + offset;
						binaryWriter.Write(selectCounts.Count);
						offset += (uint)(4 + 8 * selectCounts.Count);
						for (int n = 0; n < selectCounts.Count; n++)
						{
							binaryWriter.Write(offset);
							binaryWriter.Write(selectCounts[n]);
							offset += (uint)selectCounts[n];
						}
						lastKey = null;
						b = 0;
						bitIndex = 7;
						foreach (KeyValuePair<string, bool> selectChosen2 in SelectChosens)
						{
							string key2 = selectChosen2.Key;
							key2 = key2.Substring(0, key2.IndexOf('_'));
							if (lastKey != key2)
							{
								lastKey = key2;
								if (bitIndex < 7)
								{
									binaryWriter.Write(b);
									b = 0;
									bitIndex = 7;
								}
							}
							if (selectChosen2.Value)
							{
								b |= (byte)(1 << bitIndex);
							}
							bitIndex--;
							if (bitIndex < 0)
							{
								binaryWriter.Write(b);
								b = 0;
								bitIndex = 7;
							}
						}
						if (bitIndex < 7)
						{
							binaryWriter.Write(b);
							b = 0;
							bitIndex = 7;
						}
						for (int padding = totalSelects; padding < (totalSelects + 3) / 4 * 4; padding++)
						{
							binaryWriter.Write((byte)0);
						}
						SaveArrays(binaryWriter, Endings);
						SaveArrays(binaryWriter, Albums);
						SaveArrays(binaryWriter, Musics);
						SaveArrays(binaryWriter, Movies);
						SaveArrays(binaryWriter, EncyOpens);
						SaveArrays(binaryWriter, EncyViews);
						foreach (VarDefine value3 in _globalVarDefines.Values)
						{
							binaryWriter.Write((byte)value3.Value);
						}
						for (int varPad = infoHeader.GlobalVarLength; varPad < (infoHeader.GlobalVarLength + 3) / 4 * 4; varPad++)
						{
							binaryWriter.Write((byte)0);
						}
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show("An Error occured while saving state.\n" + ex.Message);
			}
		}

		public string GetSaveStoryName(int index)
		{
			string saveFileName = GetSaveFileName(index);
			return GetSaveStoryName(saveFileName);
		}

		public string GetSaveStoryName(string fileName)
		{
			if (!File.Exists(fileName))
			{
				return null;
			}
			try
			{
				string result;
				using (FileStream input = new FileStream(fileName, FileMode.Open))
				{
					using (BinaryReader binaryReader = new BinaryReader(input, Encoding.Unicode))
					{
						FileHeader.Read(binaryReader);
						GameInstanceState.InfoHeader infoHeader = default(GameInstanceState.InfoHeader);
						infoHeader.Size = binaryReader.ReadUInt32();
						if (infoHeader.Size != Marshal.SizeOf(infoHeader))
						{
							result = null;
						}
						else
						{
							infoHeader.StoryNameOffset = binaryReader.ReadUInt32();
							infoHeader.StoryNameLength = binaryReader.ReadInt32();
							infoHeader.SelectOffset = binaryReader.ReadUInt32();
							infoHeader.SelectLength = binaryReader.ReadInt32();
							infoHeader.VarOffset = binaryReader.ReadUInt32();
							infoHeader.VarLength = binaryReader.ReadInt32();
							infoHeader.PlayTime = binaryReader.ReadInt32();
							infoHeader.SaveTime = binaryReader.ReadInt32();
							infoHeader.Icon = binaryReader.ReadInt32();
							infoHeader.CommandOffset = binaryReader.ReadUInt32();
							infoHeader.CommandLength = binaryReader.ReadInt32();
							binaryReader.BaseStream.Position = 8232 + infoHeader.StoryNameOffset;
							result = binaryReader.ReadString();
						}
					}
				}
				return result;
			}
			catch (Exception ex)
			{
				MessageBox.Show("An Error occured while loading save.\n" + ex.Message);
				return null;
			}
		}

		private void SaveArrays(BinaryWriter bw, bool[] datas)
		{
			byte b = 0;
			int bitIndex = 7;
			for (int i = 0; i < datas.Length; i++)
			{
				if (datas[i])
				{
					b |= (byte)(1 << bitIndex);
				}
				bitIndex--;
				if (bitIndex < 0)
				{
					bw.Write(b);
					b = 0;
					bitIndex = 7;
				}
			}
			if (bitIndex < 7)
			{
				bw.Write(b);
				b = 0;
				bitIndex = 7;
			}
			for (int j = (datas.Length + 7) / 8; j < ((datas.Length + 7) / 8 + 3) / 4 * 4; j++)
			{
				bw.Write((byte)0);
			}
		}

		public void Dispose()
		{
			_globalVarDefines.Clear();
			Histories.Clear();
			Messages.Clear();
		}
	}
}
