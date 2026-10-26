using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using KidFansChannel.GalCore.Commands;
using KidFansChannel.GalCore.Defines;

namespace KidFansChannel.GalCore
{
	public class GameScript
	{
		private GalCore _galCore;

		private GameInstanceScript _currentGameScript;

		private GameState state;

		public int QuickIndexStart;

		public int QuickIndexEnd;

		public int QuickIcon;

		public int TitleIcon;

		public int SelectIcon;

		private int lastSaveIndex = -1;

		private Dictionary<string, Type> _types = new Dictionary<string, Type>();

		public GalCore GalCore
		{
			get
			{
				return _galCore;
			}
		}

		public GameInstanceScript CurrentGameScript
		{
			get
			{
				return _currentGameScript;
			}
		}

		internal Dictionary<string, Type> Types
		{
			get
			{
				return _types;
			}
		}

		public GameScript()
		{
			PrepareTypes();
		}

		protected virtual void ClearCommands()
		{
			EffectStart.ClearEffect();
			BgDisplay.ClearStyle();
			BgTransform.ClearStyle();
			CharaTransform.ClearStyle();
		}

		~GameScript()
		{
			ClearCommands();
			_types.Clear();
		}

		public void StartStory(string storyName)
		{
			StartStory(storyName, true);
		}

		public void StartStory(string storyName, bool jump)
		{
			_galCore.GameSound.StopBgm();
			_currentGameScript = CreateInstanceScript(storyName, jump);
			_galCore.CurrentPlayState = PlayState.Running;
			_galCore.CurrentPlayMode = PlayMode.Normal;
			_galCore.GameState.StartTimer();
			_galCore.GameState.CurrentGameState.ResetBaseTime();
		}

		protected virtual GameInstanceScript CreateInstanceScript(string storyName, bool jump)
		{
			return new GameInstanceScript(storyName, jump, this);
		}

		public void StopStory()
		{
			foreach (Type value in _types.Values)
			{
				ScriptCommand scriptCommand = (ScriptCommand)value.Assembly.CreateInstance(value.FullName);
				if (scriptCommand is IResetable)
				{
					((IResetable)scriptCommand).Reset();
				}
			}
			_galCore.GameState.ClearLastString();
			_galCore.GameSound.StopBgm();
			_galCore.StopShock(true);
			_galCore.HideComment();
			_currentGameScript = null;
			_galCore.GameState.StopTimer();
		}

		public void SetCore(GalCore gc)
		{
			_galCore = gc;
			foreach (KeyValuePair<string, Type> type2 in _types)
			{
				Type value = type2.Value;
				string dataName = ((ScriptCommand)value.Assembly.CreateInstance(value.FullName)).DataName;
				if (dataName != null)
				{
					gc.GameData.AddData(dataName);
				}
			}
			gc.GameData.AddData("script");
			ScriptCommand.SetCore(gc);
			foreach (KeyValuePair<string, Type> type3 in _types)
			{
				Type value2 = type3.Value;
				Type[] interfaces = value2.GetInterfaces();
				bool implemented = false;
				Type[] interfacesToScan = interfaces;
				foreach (Type type in interfacesToScan)
				{
					if (type == typeof(IStaticDataUser))
					{
						implemented = true;
						break;
					}
				}
				if (implemented)
				{
					((IStaticDataUser)value2.Assembly.CreateInstance(value2.FullName)).InitStaticData();
				}
			}
			state = _galCore.GameState;
			ProcessIndex();
		}

		protected void ProcessIndex()
		{
			XmlDocument xmlDocument = new XmlDocument();
			if (!_galCore.GameData.LoadXmlDocument("script", "index.xml", xmlDocument))
			{
				throw new FileNotFoundException(null, Path.Combine("script", "index.xml"));
			}
			NodeInfo nodeInfo = new NodeInfo();
			nodeInfo.File = "index";
			XmlNode documentElement = xmlDocument.DocumentElement;
			if (documentElement.Name != "info")
			{
				throw new ScriptStructureException(documentElement, nodeInfo, "Index's document element should be info.");
			}
			int firstChild = 0;
			List<string> storyNames = new List<string>();
			for (int i = 0; i < documentElement.ChildNodes.Count; i++)
			{
				if (documentElement.ChildNodes[i].Name == "definestory")
				{
					storyNames.Add(ScriptHelper.GetArgumentString(documentElement.ChildNodes[i], nodeInfo, "title"));
				}
				else if (documentElement.ChildNodes[i].NodeType != XmlNodeType.Comment)
				{
					break;
				}
				firstChild++;
			}
			if (storyNames.Count == 0)
			{
				throw new ScriptCommandException(documentElement, nodeInfo, "DefineStorys should come first.");
			}
			XmlDocument xmlDocument2 = new XmlDocument();
			foreach (string item in storyNames)
			{
				if (!_galCore.GameData.LoadXmlDocument("script", item + "\\index.xml", xmlDocument2))
				{
					throw new FileNotFoundException(null, Path.Combine("script", item + "\\index.xml"));
				}
				XmlNode documentElement2 = xmlDocument2.DocumentElement;
				if (documentElement2.Name != "info")
				{
					throw new ScriptStructureException(documentElement2, nodeInfo, "Index's document element must be info.");
				}
				foreach (XmlNode childNode in documentElement2.ChildNodes)
				{
					if (childNode.Name == "definechapter")
					{
						string argumentString = ScriptHelper.GetArgumentString(childNode, nodeInfo, "title");
						int argumentInteger = ScriptHelper.GetArgumentInteger(childNode, nodeInfo, "stringnum", 0);
						state.InitAddString(item, argumentString, argumentInteger);
					}
					else if (childNode.Name == "defineselect")
					{
						string argumentString2 = ScriptHelper.GetArgumentString(childNode, nodeInfo, "title");
						string[] cases = ScriptHelper.GetArgumentString(childNode, nodeInfo, "case").Split(';');
						state.InitAddSelect(item, argumentString2, cases);
					}
				}
			}
			for (int j = firstChild; j < documentElement.ChildNodes.Count; j++)
			{
				if (documentElement.ChildNodes[j].Name == "definevar")
				{
					VarDefine varDefine = new VarDefine();
					varDefine.Title = ScriptHelper.GetArgumentString(documentElement.ChildNodes[j], nodeInfo, "title");
					varDefine.Value = ScriptHelper.GetArgumentInteger(documentElement.ChildNodes[j], nodeInfo, "value", 0);
					state.GlobalVarDefines.Add(varDefine.Title, varDefine);
				}
				else if (documentElement.ChildNodes[j].Name == "defineending")
				{
					int argumentInteger2 = ScriptHelper.GetArgumentInteger(documentElement.ChildNodes[j], nodeInfo, "firstChild", 0);
					state.InitEnding(argumentInteger2);
				}
				else if (documentElement.ChildNodes[j].Name == "definealbum")
				{
					int argumentInteger3 = ScriptHelper.GetArgumentInteger(documentElement.ChildNodes[j], nodeInfo, "firstChild", 0);
					state.InitAlbum(argumentInteger3);
				}
				else if (documentElement.ChildNodes[j].Name == "definemusic")
				{
					int argumentInteger4 = ScriptHelper.GetArgumentInteger(documentElement.ChildNodes[j], nodeInfo, "firstChild", 0);
					state.InitMusic(argumentInteger4);
				}
				else if (documentElement.ChildNodes[j].Name == "definemovie")
				{
					int argumentInteger5 = ScriptHelper.GetArgumentInteger(documentElement.ChildNodes[j], nodeInfo, "firstChild", 0);
					state.InitMovie(argumentInteger5);
				}
				else if (documentElement.ChildNodes[j].Name == "defineency")
				{
					int argumentInteger6 = ScriptHelper.GetArgumentInteger(documentElement.ChildNodes[j], nodeInfo, "firstChild", 0);
					state.InitEncy(argumentInteger6);
				}
				else if (documentElement.ChildNodes[j].NodeType != XmlNodeType.Comment)
				{
					throw new ScriptStructureException(documentElement.ChildNodes[j], nodeInfo, "Info's child must be defines.");
				}
			}
		}

		public virtual bool SaveStory(int index, string comment, int icon)
		{
			if (_currentGameScript == null)
			{
				throw new InvalidOperationException("No Game Running");
			}
			if (_galCore.GameState.CurrentGameState.CanSave)
			{
				_currentGameScript.SaveStory(index, comment, icon);
				lastSaveIndex = index;
				return true;
			}
			return false;
		}

		public virtual void LoadStory(int index)
		{
			string saveStoryName = _galCore.GameState.GetSaveStoryName(index);
			if (saveStoryName != null)
			{
				if (_currentGameScript == null)
				{
					StartStory(saveStoryName, false);
					_currentGameScript.LoadStory(index);
				}
				else
				{
					StopStory();
					StartStory(saveStoryName, false);
					_currentGameScript.LoadStory(index);
				}
				lastSaveIndex = index;
			}
		}

		public void LoadStory(string fileName)
		{
			string saveStoryName = _galCore.GameState.GetSaveStoryName(fileName);
			if (saveStoryName != null)
			{
				StartStory(saveStoryName, false);
				_currentGameScript.LoadStory(fileName);
			}
		}

		public virtual bool QuickSave(QuickSaveType type)
		{
			int quickSaveIndex = _galCore.GameOptions.QuickSaveIndex;
			quickSaveIndex = ((quickSaveIndex != -1 && quickSaveIndex != QuickIndexStart) ? (quickSaveIndex - 1) : (QuickIndexEnd - 1));
			int iconId = 0;
			switch (type)
			{
			case QuickSaveType.Quick:
				iconId = QuickIcon;
				break;
			case QuickSaveType.Title:
				iconId = TitleIcon;
				break;
			case QuickSaveType.Select:
				iconId = SelectIcon;
				break;
			default:
				iconId = QuickIcon;
				break;
			}
			if (SaveStory(quickSaveIndex, null, iconId))
			{
				_galCore.GameOptions.QuickSaveIndex = quickSaveIndex;
				return true;
			}
			return false;
		}

		public virtual bool SaveLastGame()
		{
			if (SaveStory(QuickIndexEnd, null, QuickIcon))
			{
				return true;
			}
			if (lastSaveIndex == -1)
			{
				return false;
			}
			if (lastSaveIndex == QuickIndexEnd)
			{
				return true;
			}
			try
			{
				File.Copy(_galCore.GameState.GetSaveFileName(lastSaveIndex), _galCore.GameState.GetSaveFileName(QuickIndexEnd), true);
				File.Copy(_galCore.GameState.GetInfoFileName(lastSaveIndex), _galCore.GameState.GetInfoFileName(QuickIndexEnd), true);
				File.Copy(_galCore.GameState.GetTextFileName(lastSaveIndex), _galCore.GameState.GetTextFileName(QuickIndexEnd), true);
				return true;
			}
			catch
			{
			}
			return false;
		}

		public virtual void QuickLoad()
		{
			int quickSlot = _galCore.GameOptions.QuickSaveIndex;
			if (quickSlot == -1)
			{
				quickSlot = QuickIndexStart;
			}
			LoadStory(quickSlot);
		}

		protected virtual void PrepareTypes()
		{
			AssignTypeClass(typeof(Wait));
			AssignTypeClass(typeof(DialogueClear));
			AssignTypeClass(typeof(ParagraphClear));
			AssignTypeClass(typeof(Scene));
			AssignTypeClass(typeof(BgDisplay));
			AssignTypeClass(typeof(BgClear));
			AssignTypeClass(typeof(BgTransform));
			AssignTypeClass(typeof(BgChange));
			AssignTypeClass(typeof(CharaDisplay));
			AssignTypeClass(typeof(CharaClear));
			AssignTypeClass(typeof(CharaTransform));
			AssignTypeClass(typeof(BgmPlay));
			AssignTypeClass(typeof(BgmVolume));
			AssignTypeClass(typeof(BgmStop));
			AssignTypeClass(typeof(SePlay));
			AssignTypeClass(typeof(SeVolume));
			AssignTypeClass(typeof(SeStop));
			AssignTypeClass(typeof(Par));
			AssignTypeClass(typeof(EffectStart));
			AssignTypeClass(typeof(EffectStop));
			AssignTypeClass(typeof(Jump));
			AssignTypeClass(typeof(Var));
			AssignTypeClass(typeof(Movie));
			AssignTypeClass(typeof(Timer));
			AssignTypeClass(typeof(DisSave));
		}

		protected void AssignTypeClass(Type scriptCommandType)
		{
			if (!scriptCommandType.IsSubclassOf(typeof(ScriptCommand)))
			{
				throw new ArgumentException();
			}
			string command = ((ScriptCommand)scriptCommandType.Assembly.CreateInstance(scriptCommandType.FullName)).Command;
			if (_types.ContainsKey(command))
			{
				_types.Remove(command);
			}
			_types.Add(command, scriptCommandType);
		}

		public virtual string GetStoryName(string story)
		{
			return story;
		}

		public string GetChapterName(string story, string chapter)
		{
			XmlDocument xmlDocument = new XmlDocument();
			if (!_galCore.GameData.LoadXmlDocument("script", story + "\\index.xml", xmlDocument))
			{
				throw new FileNotFoundException(null, Path.Combine("script", story + "\\index.xml"));
			}
			XmlNode documentElement = xmlDocument.DocumentElement;
			NodeInfo info = new NodeInfo();
			if (documentElement.Name != "info")
			{
				throw new ScriptStructureException(documentElement, info, "Index's document element must be info.");
			}
			ScriptHelper.GetArgumentString(documentElement, info, "script");
			string[] scripts = null;
			foreach (XmlNode childNode in documentElement.ChildNodes)
			{
				if (childNode.Name == "definechapter" && ScriptHelper.GetArgumentString(childNode, info, "title") == chapter)
				{
					scripts = ScriptHelper.GetArgumentString(childNode, info, "script").Split(';');
					break;
				}
			}
			if (scripts == null)
			{
				return chapter;
			}
			xmlDocument = new XmlDocument();
			if (!LoadScript(story + "\\" + scripts[0], xmlDocument))
			{
				throw new FileNotFoundException(null, Path.Combine("script", story + "\\" + scripts[0] + ".xml"));
			}
			documentElement = xmlDocument.SelectSingleNode("script/chapter[@title='" + chapter + "']/scene/showchapter");
			if (documentElement != null && documentElement.HasChildNodes && documentElement.ChildNodes.Count == 1 && documentElement.ChildNodes[0].NodeType == XmlNodeType.Text)
			{
				return documentElement.ChildNodes[0].Value;
			}
			return chapter;
		}

		public bool LoadScript(string scriptFile, XmlDocument doc)
		{
			if (!_galCore.GameData.LoadXmlDocument("script", scriptFile + ".xml", doc))
			{
				return false;
			}
			XmlDocument xmlDocument = new XmlDocument();
			if (_galCore.GameData.LoadXmlDocument("script", scriptFile + ".string.xml", xmlDocument))
			{
				AddScriptString(doc, xmlDocument);
			}
			for (int i = 0; i < 100; i++)
			{
				if (!_galCore.GameData.LoadXmlDocument("script", scriptFile + ".string" + i.ToString("00") + ".xml", xmlDocument))
				{
					break;
				}
				AddScriptString(doc, xmlDocument);
			}
			return true;
		}

		private void AddScriptString(XmlDocument doc, XmlDocument stringDoc)
		{
			XmlNode documentElement = stringDoc.DocumentElement;
			XmlNodeList xmlNodeList = documentElement.SelectNodes("chapter");
			for (int i = 0; i < xmlNodeList.Count; i++)
			{
				string value = xmlNodeList[i].Attributes["title"].Value;
				XmlNode xmlNode = doc.SelectSingleNode("script/chapter[@title='" + value + "']");
				if (xmlNode == null)
				{
					continue;
				}
				XmlNodeList childNodes = xmlNodeList[i].ChildNodes;
				for (int j = 0; j < childNodes.Count; j++)
				{
					if (childNodes[j].Name == "text")
					{
						string value2 = childNodes[j].Attributes["index"].Value;
						XmlNodeList xmlNodeList2 = xmlNode.SelectNodes("scene/dialogue[@index='" + value2 + "']");
						for (int k = 0; k < xmlNodeList2.Count; k++)
						{
							xmlNodeList2[k].InnerXml = childNodes[j].InnerXml;
						}
						xmlNodeList2 = xmlNode.SelectNodes("scene/paragraph[@index='" + value2 + "']");
						for (int l = 0; l < xmlNodeList2.Count; l++)
						{
							xmlNodeList2[l].InnerXml = childNodes[j].InnerXml;
						}
					}
					else if (childNodes[j].Name == "showchapter")
					{
						XmlNodeList xmlNodeList2 = xmlNode.SelectNodes("scene/showchapter");
						for (int m = 0; m < xmlNodeList2.Count; m++)
						{
							xmlNodeList2[m].InnerXml = childNodes[j].InnerXml;
						}
					}
					else
					{
						if (!(childNodes[j].Name == "select"))
						{
							continue;
						}
						string value3 = childNodes[j].Attributes["title"].Value;
						XmlNodeList xmlNodeList2 = xmlNode.SelectNodes("scene/select[@title='" + value3 + "']");
						for (int n = 0; n < xmlNodeList2.Count; n++)
						{
							XmlNodeList xmlNodeList3 = childNodes[j].SelectNodes("item");
							for (int quickSlot = 0; quickSlot < xmlNodeList3.Count; quickSlot++)
							{
								string value4 = xmlNodeList3[quickSlot].Attributes["case"].Value;
								XmlNode xmlNode2 = xmlNodeList2[n].SelectSingleNode("item[@case='" + value4 + "']");
								xmlNode2.InnerXml = xmlNodeList3[quickSlot].InnerXml;
							}
						}
					}
				}
			}
		}
	}
}
