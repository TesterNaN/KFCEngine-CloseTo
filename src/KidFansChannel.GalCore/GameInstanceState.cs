using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using KidFansChannel.GalCore.Commands;
using KidFansChannel.GalCore.Defines;
using KidFansChannel.GalCore.Records;

namespace KidFansChannel.GalCore
{
	public class GameInstanceState : ICloneable, IDisposable
	{
		internal struct InfoHeader
		{
			public uint Size;

			public uint StoryNameOffset;

			public int StoryNameLength;

			public uint SelectOffset;

			public int SelectLength;

			public uint VarOffset;

			public int VarLength;

			public int PlayTime;

			public long SaveTime;

			public int Icon;

			public uint CommandOffset;

			public int CommandLength;
		}

		private string _storyName;

		private NodeInfo _info = new NodeInfo();

		private Dictionary<string, ChapterDefine> _chapterDefines = new Dictionary<string, ChapterDefine>();

		private Dictionary<string, SelectDefine> _selectDefines = new Dictionary<string, SelectDefine>();

		private Dictionary<string, VarDefine> _varDefines = new Dictionary<string, VarDefine>();

		private List<ISaveable> _currentStateCommands = new List<ISaveable>();

		private List<Record> _records = new List<Record>();

		private List<int> _recordLines = new List<int>();

		private string _gameDateTime = "??/?? ??:??";

		protected GameState Parent;

		public readonly Rectangle SnapRegion = new Rectangle(8, 38, 240, 180);

		private NodeInfo stateInfo = new NodeInfo();

		private bool _canSave;

		private Queue<Record> recordBuffers = new Queue<Record>();

		private double _baseTime;

		private DateTime _saveTime;

		public string StoryName
		{
			get
			{
				return _storyName;
			}
		}

		public NodeInfo Info
		{
			get
			{
				return _info;
			}
		}

		public string File
		{
			get
			{
				return _info.File;
			}
			set
			{
				_info.File = value;
			}
		}

		public string Chapter
		{
			get
			{
				return _info.Chapter;
			}
			set
			{
				_info.Chapter = value;
			}
		}

		public int Scene
		{
			get
			{
				return _info.Scene;
			}
			set
			{
				_info.Scene = value;
			}
		}

		public int Click
		{
			get
			{
				return _info.Click;
			}
			set
			{
				_info.Click = value;
			}
		}

		public Dictionary<string, ChapterDefine> ChaptereDefines
		{
			get
			{
				return _chapterDefines;
			}
		}

		public Dictionary<string, SelectDefine> SelectDefines
		{
			get
			{
				return _selectDefines;
			}
		}

		public Dictionary<string, VarDefine> VarDefines
		{
			get
			{
				return _varDefines;
			}
		}

		public List<ISaveable> CurrentStateCommands
		{
			get
			{
				return _currentStateCommands;
			}
		}

		public List<Record> Records
		{
			get
			{
				return _records;
			}
		}

		public List<int> RecordLines
		{
			get
			{
				return _recordLines;
			}
		}

		public string GameDateTime
		{
			get
			{
				return _gameDateTime;
			}
			set
			{
				_gameDateTime = value;
			}
		}

		public bool CanSave
		{
			get
			{
				foreach (ScriptCommand currentCommand in Parent.GalCore.CurrentCommandList)
				{
					if (currentCommand is DisSave)
					{
						return false;
					}
				}
				return _canSave;
			}
		}

		public double BaseTime
		{
			get
			{
				return _baseTime;
			}
			set
			{
				_baseTime = value;
			}
		}

		public DateTime SaveTime
		{
			get
			{
				return _saveTime;
			}
		}

		public GameInstanceState(GameState parent, string storyName)
		{
			Parent = parent;
			_storyName = storyName;
			_recordLines.Add(0);
		}

		~GameInstanceState()
		{
			_records.Clear();
			_recordLines.Clear();
			recordBuffers.Clear();
		}

		public void Update(List<ScriptCommand> currentCommandList)
		{
			_currentStateCommands.Clear();
			foreach (ScriptCommand currentCommand in currentCommandList)
			{
				if (currentCommand is ISaveable)
				{
					((ISaveable)currentCommand).ConfirmUpdate();
					_currentStateCommands.Add((ISaveable)currentCommand);
				}
			}
			Parent.ConfirmString();
			stateInfo.File = _info.File;
			stateInfo.Chapter = _info.Chapter;
			stateInfo.Scene = _info.Scene;
			stateInfo.Click = _info.Click;
			ConfirmRecord();
			_canSave = true;
		}

		public void SaveState(int index, string comment, int icon)
		{
			if (!Directory.Exists(Parent.SaveFullPath))
			{
				try
				{
					Directory.CreateDirectory(Parent.SaveFullPath);
				}
				catch (Exception ex)
				{
					MessageBox.Show("An Error occured while creating save directory.\n" + ex.Message);
				}
			}
			try
			{
				using (FileStream fileStream = new FileStream(Parent.GetSaveFileName(index), FileMode.Create))
				{
					using (BinaryWriter binaryWriter = new BinaryWriter(fileStream, Encoding.Unicode))
					{
						Parent.FileHeader.SaveName = Parent.GameSaveName;
						Parent.FileHeader.LevelName = string.Format("{0} / {1}", Parent.GalCore.GameScript.GetStoryName(_storyName), Parent.GalCore.GameScript.GetChapterName(_storyName, stateInfo.Chapter));
						if (comment == null)
						{
							Parent.FileHeader.Comments = Parent.LastString;
						}
						else
						{
							Parent.FileHeader.Comments = comment;
						}
						Bitmap bitmap = new Bitmap(Parent.GalCore.GameGraphics.Capture(SnapRegion.Size));
						Bitmap bitmap2 = new Bitmap(256, 256, PixelFormat.Format32bppArgb);
						using (Graphics graphics = Graphics.FromImage(bitmap2))
						{
							graphics.DrawImageUnscaled(bitmap, SnapRegion.Location);
							graphics.Flush();
						}
						MemoryStream memoryStream = new MemoryStream();
						bitmap2.Save(memoryStream, ImageFormat.Png);
						bitmap2.Dispose();
						bitmap.Dispose();
						byte[] thumbnail = memoryStream.ToArray();
						memoryStream.Close();
						InfoHeader infoHeader = default(InfoHeader);
						Parent.FileHeader.ThumbnailOffset = Marshal.SizeOf(infoHeader);
						Parent.FileHeader.ThumbnailSize = thumbnail.Length;
						Parent.FileHeader.Write(binaryWriter);
						uint offset = 0u;
						offset = (infoHeader.StoryNameOffset = (infoHeader.Size = (uint)Marshal.SizeOf(infoHeader)) + (uint)((thumbnail.Length + 3) / 4 * 4));
						infoHeader.StoryNameLength = _storyName.Length * 2 + 1;
						offset = (infoHeader.SelectOffset = offset + (uint)((infoHeader.StoryNameLength + 3) / 4 * 4));
						infoHeader.SelectLength = _selectDefines.Count;
						offset = (infoHeader.VarOffset = offset + (uint)((infoHeader.SelectLength + 3) / 4 * 4));
						infoHeader.VarLength = _varDefines.Count;
						infoHeader.PlayTime = (int)GetTime();
						_saveTime = DateTime.Now;
						infoHeader.SaveTime = _saveTime.Ticks;
						infoHeader.Icon = icon;
						offset = (infoHeader.CommandOffset = offset + (uint)((infoHeader.VarLength + 3) / 4 * 4));
						infoHeader.CommandLength = 0;
						binaryWriter.Write(infoHeader.Size);
						binaryWriter.Write(infoHeader.StoryNameOffset);
						binaryWriter.Write(infoHeader.StoryNameLength);
						binaryWriter.Write(infoHeader.SelectOffset);
						binaryWriter.Write(infoHeader.SelectLength);
						binaryWriter.Write(infoHeader.VarOffset);
						binaryWriter.Write(infoHeader.VarLength);
						binaryWriter.Write(infoHeader.PlayTime);
						binaryWriter.Write(infoHeader.SaveTime);
						binaryWriter.Write(infoHeader.Icon);
						binaryWriter.Write(infoHeader.CommandOffset);
						binaryWriter.Write(infoHeader.CommandLength);
						while (fileStream.Position < 8232 + Parent.FileHeader.ThumbnailOffset)
						{
							fileStream.WriteByte(0);
						}
						fileStream.Write(thumbnail, 0, thumbnail.Length);
						for (int i = thumbnail.Length; i < (thumbnail.Length + 3) / 4 * 4; i++)
						{
							binaryWriter.Write((byte)0);
						}
						binaryWriter.Write(_storyName);
						for (int j = infoHeader.StoryNameLength; j < (infoHeader.StoryNameLength + 3) / 4 * 4; j++)
						{
							binaryWriter.Write((byte)0);
						}
						foreach (SelectDefine value in _selectDefines.Values)
						{
							binaryWriter.Write((byte)value.Value);
						}
						for (int k = infoHeader.SelectLength; k < (infoHeader.SelectLength + 3) / 4 * 4; k++)
						{
							binaryWriter.Write((byte)0);
						}
						foreach (VarDefine value2 in _varDefines.Values)
						{
							binaryWriter.Write((byte)value2.Value);
						}
						for (int l = infoHeader.VarLength; l < (infoHeader.VarLength + 3) / 4 * 4; l++)
						{
							binaryWriter.Write((byte)0);
						}
						binaryWriter.Write(_currentStateCommands.Count);
						for (int m = 0; m < _currentStateCommands.Count; m++)
						{
							binaryWriter.Write(0u);
							binaryWriter.Write(0);
						}
						binaryWriter.Write(stateInfo.File);
						binaryWriter.Write(stateInfo.Chapter);
						binaryWriter.Write(stateInfo.Scene);
						binaryWriter.Write(stateInfo.Click);
						binaryWriter.Write(_gameDateTime);
						List<uint> blockOffsets = new List<uint>();
						List<int> blockLengths = new List<int>();
						foreach (ISaveable currentStateCommand in _currentStateCommands)
						{
							blockOffsets.Add((uint)(binaryWriter.BaseStream.Position - 8232));
							currentStateCommand.Save(binaryWriter);
							int blockLength = (int)(binaryWriter.BaseStream.Position - 8232 - blockOffsets[blockOffsets.Count - 1]);
							if (blockLength == 0)
							{
								blockOffsets.RemoveAt(blockOffsets.Count - 1);
							}
							else
							{
								blockLengths.Add(blockLength);
							}
						}
						binaryWriter.BaseStream.Position = infoHeader.CommandOffset + 8232;
						binaryWriter.Write(blockOffsets.Count);
						int shift = 8 * (_currentStateCommands.Count - blockOffsets.Count);
						for (int n = 0; n < blockOffsets.Count; n++)
						{
							binaryWriter.Write((uint)(blockOffsets[n] - shift));
							binaryWriter.Write(blockLengths[n]);
						}
						if (blockOffsets.Count != _currentStateCommands.Count)
						{
							binaryWriter.Write(stateInfo.File);
							binaryWriter.Write(stateInfo.Chapter);
							binaryWriter.Write(stateInfo.Scene);
							binaryWriter.Write(stateInfo.Click);
							binaryWriter.Write(_gameDateTime);
							foreach (ISaveable currentStateCommand2 in _currentStateCommands)
							{
								currentStateCommand2.Save(binaryWriter);
							}
							binaryWriter.BaseStream.SetLength(binaryWriter.BaseStream.Position);
						}
					}
				}
				using (FileStream output = new FileStream(Parent.GetInfoFileName(index), FileMode.Create))
				{
					using (BinaryWriter binaryWriter2 = new BinaryWriter(output, Encoding.Unicode))
					{
						using (FileStream stream = new FileStream(Parent.GetTextFileName(index), FileMode.Create))
						{
							using (StreamWriter streamWriter = new StreamWriter(stream))
							{
								foreach (Record record in _records)
								{
									byte[] info;
									string recordText;
									record.Save(out info, out recordText);
									binaryWriter2.Write(info);
									streamWriter.WriteLine(recordText);
								}
							}
						}
					}
				}
			}
			catch (Exception ex2)
			{
				MessageBox.Show("An Error occured while saving save.\n" + ex2.Message);
			}
		}

		public void LoadState(int index)
		{
			string saveFileName = Parent.GetSaveFileName(index);
			LoadState(saveFileName);
		}

		public void LoadState(string fileName)
		{
			if (!System.IO.File.Exists(fileName))
			{
				return;
			}
			try
			{
				using (FileStream input = new FileStream(fileName, FileMode.Open, FileAccess.Read))
				{
					using (BinaryReader binaryReader = new BinaryReader(input, Encoding.Unicode))
					{
						Parent.FileHeader.Read(binaryReader);
						InfoHeader infoHeader = default(InfoHeader);
						infoHeader.Size = binaryReader.ReadUInt32();
						if (infoHeader.Size == Marshal.SizeOf(infoHeader))
						{
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
							_baseTime -= infoHeader.PlayTime;
							_saveTime = new DateTime(infoHeader.SaveTime);
							binaryReader.BaseStream.Position = 8232 + infoHeader.StoryNameOffset;
							if (_storyName != binaryReader.ReadString())
							{
								throw new InvalidDataException("Different story");
							}
							binaryReader.BaseStream.Position = 8232 + infoHeader.SelectOffset;
							int selectIndex = 0;
							foreach (SelectDefine value in _selectDefines.Values)
							{
								if (selectIndex < infoHeader.SelectLength)
								{
									value.Value = binaryReader.ReadByte();
									if (value.Value >= 128)
									{
										value.Value -= 256;
									}
									selectIndex++;
									continue;
								}
								break;
							}
							binaryReader.BaseStream.Position = 8232 + infoHeader.VarOffset;
							int varIndex = 0;
							foreach (VarDefine value2 in _varDefines.Values)
							{
								if (varIndex < infoHeader.VarLength)
								{
									value2.Value = binaryReader.ReadByte();
									if (value2.Value >= 128)
									{
										value2.Value -= 256;
									}
									varIndex++;
									continue;
								}
								break;
							}
							_currentStateCommands.Clear();
							binaryReader.BaseStream.Position = 8232 + infoHeader.CommandOffset;
							int num3 = binaryReader.ReadInt32();
							List<uint> list = new List<uint>();
							List<int> list2 = new List<int>();
							for (int i = 0; i < num3; i++)
							{
								list.Add(binaryReader.ReadUInt32());
								list2.Add(binaryReader.ReadInt32());
							}
							NodeInfo info = _info;
							string file = (stateInfo.File = binaryReader.ReadString());
							info.File = file;
							NodeInfo info2 = _info;
							string chapter = (stateInfo.Chapter = binaryReader.ReadString());
							info2.Chapter = chapter;
							NodeInfo info3 = _info;
							int scene = (stateInfo.Scene = binaryReader.ReadInt32());
							info3.Scene = scene;
							NodeInfo info4 = _info;
							int click = (stateInfo.Click = binaryReader.ReadInt32());
							info4.Click = click;
							_gameDateTime = binaryReader.ReadString();
							for (int j = 0; j < num3; j++)
							{
								binaryReader.BaseStream.Position = 8232 + list[j];
								string key = binaryReader.ReadString();
								Type type = Parent.GalCore.GameScript.Types[key];
								ISaveable saveable = (ISaveable)type.Assembly.CreateInstance(type.FullName);
								saveable.Load(binaryReader);
								_currentStateCommands.Add(saveable);
							}
						}
					}
				}
				string path = fileName.Substring(0, fileName.Length - Path.GetExtension(fileName).Length) + "." + Parent.InfoExt;
				string path2 = fileName.Substring(0, fileName.Length - Path.GetExtension(fileName).Length) + ".txt";
				_records.Clear();
				_recordLines.Clear();
				recordBuffers.Clear();
				_recordLines.Add(0);
				if (!System.IO.File.Exists(path) || !System.IO.File.Exists(path2))
				{
					return;
				}
				using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
				{
					using (BinaryReader binaryReader2 = new BinaryReader(fileStream, Encoding.Unicode))
					{
						using (FileStream stream = new FileStream(path2, FileMode.Open, FileAccess.Read))
						{
							using (StreamReader streamReader = new StreamReader(stream))
							{
								byte[] infos = binaryReader2.ReadBytes((int)fileStream.Length);
								int index = 0;
								while (!streamReader.EndOfStream)
								{
									string text3 = streamReader.ReadLine();
									Record record = Record.Load(infos, ref index, text3);
									_records.Add(record);
									_recordLines.Add(record.Line + _recordLines[_recordLines.Count - 1]);
								}
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show("An Error occured while loading save.\n" + ex.Message);
			}
		}

		public void AddRecord(Record[] records)
		{
			foreach (Record item in records)
			{
				recordBuffers.Enqueue(item);
			}
		}

		public void ConfirmRecord()
		{
			while (recordBuffers.Count > 0)
			{
				Record record = recordBuffers.Dequeue();
				_records.Add(record);
				_recordLines.Add(record.Line + _recordLines[_recordLines.Count - 1]);
			}
		}

		public void ResetBaseTime()
		{
			_baseTime = Parent.GetTime();
		}

		public float GetTime()
		{
			return (float)((double)Parent.GetTime() - _baseTime);
		}

		public object Clone()
		{
			GameInstanceState gameInstanceState = new GameInstanceState(Parent, _storyName);
			gameInstanceState._info = _info;
			gameInstanceState.stateInfo = stateInfo;
			foreach (KeyValuePair<string, ChapterDefine> chapterDefine in _chapterDefines)
			{
				gameInstanceState._chapterDefines.Add(chapterDefine.Key, chapterDefine.Value);
			}
			foreach (KeyValuePair<string, SelectDefine> selectDefine in _selectDefines)
			{
				gameInstanceState._selectDefines.Add(selectDefine.Key, selectDefine.Value);
			}
			foreach (KeyValuePair<string, VarDefine> varDefine in _varDefines)
			{
				gameInstanceState._varDefines.Add(varDefine.Key, varDefine.Value);
			}
			gameInstanceState._currentStateCommands.AddRange(_currentStateCommands);
			return gameInstanceState;
		}

		public void Dispose()
		{
			_chapterDefines.Clear();
			_selectDefines.Clear();
			_varDefines.Clear();
			_currentStateCommands.Clear();
		}
	}
}
