using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using KidFansChannel.GalCore.Commands;
using KidFansChannel.GalCore.Defines;

namespace KidFansChannel.GalCore
{
	public class GameInstanceScript
	{
		private GameScript parent;

		private string _storyName;

		private XmlDocument _currentXmlDocument;

		private XmlNode _currentChapter;

		private ChapterDefine _currentChapterDefine;

		private XmlNode _currentScene;

		private XmlNode _nextCommand;

		private GameInstanceState _state;

		private bool _newChapter;

		private List<ScriptCommand> preCommandList = new List<ScriptCommand>();

		private bool NewScene;

		private bool justLoad;

		private bool _stringRead;

		protected XmlNode NextStringNode;

		private bool nextProcessStringRead;

		protected XmlDocument CurrentXmlDocument
		{
			get
			{
				return _currentXmlDocument;
			}
		}

		protected XmlNode CurrentChapter
		{
			get
			{
				return _currentChapter;
			}
		}

		protected ChapterDefine CurrentChapterDefine
		{
			get
			{
				return _currentChapterDefine;
			}
		}

		protected XmlNode CurrentScene
		{
			get
			{
				return _currentScene;
			}
		}

		protected XmlNode NextCommand
		{
			get
			{
				return _nextCommand;
			}
		}

		protected GameInstanceState State
		{
			get
			{
				return _state;
			}
			set
			{
				_state = value;
			}
		}

		public string StoryName
		{
			get
			{
				return _storyName;
			}
			protected set
			{
				_storyName = value;
			}
		}

		public XmlNode CommandNode
		{
			get
			{
				return _nextCommand;
			}
		}

		public bool NewChapter
		{
			get
			{
				return _newChapter;
			}
			set
			{
				_newChapter = value;
			}
		}

		public bool StringRead
		{
			get
			{
				return _stringRead;
			}
		}

		protected internal GameInstanceScript(string storyName, bool jump, GameScript gameScript)
		{
			parent = gameScript;
			_storyName = storyName;
			parent.GalCore.GameState.InitNewState(_storyName);
			_state = parent.GalCore.GameState.CurrentGameState;
			string text = ProcessIndex();
			if (jump)
			{
				if (!parent.LoadScript(_storyName + "\\" + text, _currentXmlDocument))
				{
					throw new FileNotFoundException(null, Path.Combine("script", _storyName + "\\" + text + ".xml"));
				}
				_state.File = text;
				_state.Chapter = "";
				_state.Scene = 0;
				_state.Click = 0;
				XmlNode documentElement = _currentXmlDocument.DocumentElement;
				if (documentElement.Name != "script")
				{
					throw new ScriptStructureException(documentElement, _state.Info, "Script's document element must be script.");
				}
				XmlNode xmlNode = documentElement.FirstChild;
				while (xmlNode != null && xmlNode.NodeType == XmlNodeType.Comment)
				{
					xmlNode = xmlNode.NextSibling;
				}
				if (xmlNode == null || xmlNode.Name != "chapter")
				{
					throw new ScriptStructureException(xmlNode, _state.Info, "Script should have chapter child node only.");
				}
				string argumentString = ScriptHelper.GetArgumentString(xmlNode, _state.Info, "title");
				JumpScene(_state.File, argumentString, 0);
			}
		}

		protected string ProcessIndex()
		{
			_currentXmlDocument = new XmlDocument();
			if (!parent.GalCore.GameData.LoadXmlDocument("script", _storyName + "\\index.xml", _currentXmlDocument))
			{
				throw new FileNotFoundException(null, Path.Combine("script", _storyName + "\\index.xml"));
			}
			_state.File = "index";
			_state.Chapter = "";
			_state.Scene = 0;
			_state.Click = 0;
			XmlNode documentElement = _currentXmlDocument.DocumentElement;
			if (documentElement.Name != "info")
			{
				throw new ScriptStructureException(documentElement, _state.Info, "Index's document element must be info.");
			}
			string argumentString = ScriptHelper.GetArgumentString(documentElement, _state.Info, "script");
			foreach (XmlNode childNode in documentElement.ChildNodes)
			{
				if (childNode.Name == "definechapter")
				{
					ChapterDefine chapterDefine = new ChapterDefine();
					chapterDefine.Title = ScriptHelper.GetArgumentString(childNode, _state.Info, "title");
					chapterDefine.StringNum = ScriptHelper.GetArgumentInteger(childNode, _state.Info, "stringnum", 0);
					chapterDefine.File = ScriptHelper.GetArgumentString(childNode, _state.Info, "script").Split(';');
					_state.ChaptereDefines.Add(chapterDefine.Title, chapterDefine);
				}
				else if (childNode.Name == "definevar")
				{
					VarDefine varDefine = new VarDefine();
					varDefine.Title = ScriptHelper.GetArgumentString(childNode, _state.Info, "title");
					varDefine.Value = ScriptHelper.GetArgumentInteger(childNode, _state.Info, "value", 0);
					_state.VarDefines.Add(varDefine.Title, varDefine);
				}
				else if (childNode.Name == "defineselect")
				{
					SelectDefine selectDefine = new SelectDefine();
					selectDefine.Title = ScriptHelper.GetArgumentString(childNode, _state.Info, "title");
					selectDefine.Case.AddRange(ScriptHelper.GetArgumentString(childNode, _state.Info, "case").Split(';'));
					string argumentString2 = ScriptHelper.GetArgumentString(childNode, _state.Info, "value", string.Empty);
					if (argumentString2 != string.Empty)
					{
						selectDefine.Value = selectDefine.Case.IndexOf(argumentString2);
						if (selectDefine.Value == -1)
						{
							throw new ScriptArgumentException(childNode, _state.Info, "value", "Value is not exist in cases.");
						}
					}
					else
					{
						selectDefine.Value = -1;
					}
					_state.SelectDefines.Add(selectDefine.Title, selectDefine);
				}
				else if (childNode.NodeType != XmlNodeType.Comment)
				{
					throw new ScriptStructureException(childNode, _state.Info, "Info's child must be defines.");
				}
			}
			return argumentString;
		}

		public void Jump(string script, string chapter, int scene, int click)
		{
			NewScene = false;
			JumpScene(script, chapter, scene, false, false);
			if (click != _state.Click)
			{
				if (click > 0)
				{
					_nextCommand = _currentScene.SelectSingleNode("click[" + click + "]").NextSibling;
				}
				else
				{
					_nextCommand = _currentScene.FirstChild;
				}
				while (_nextCommand != null && _nextCommand.NodeType == XmlNodeType.Comment)
				{
					_nextCommand = _nextCommand.NextSibling;
				}
			}
			_state.Click = click;
		}

		public void JumpScene(string chapter, int scene)
		{
			JumpScene(_state.File, chapter, scene);
		}

		public void JumpScene(string script, string chapter, string label)
		{
			XmlDocument xmlDocument = new XmlDocument();
			if (script != _state.File)
			{
				if (!parent.GalCore.GameData.LoadXmlDocument("script", _storyName + "\\" + script + ".xml", xmlDocument))
				{
					throw new FileNotFoundException(null, Path.Combine("script", _storyName + "\\" + script + ".xml"));
				}
			}
			else
			{
				xmlDocument = _currentXmlDocument;
			}
			XmlNode xmlNode = xmlDocument.DocumentElement.SelectSingleNode("chapter[@title=\"" + chapter + "\"]");
			XmlNode xmlNode2 = xmlNode.SelectSingleNode("scene[@label=\"" + label + "\"]");
			int i;
			for (i = 0; xmlNode2 != xmlNode.SelectSingleNode("scene[" + (i + 1) + "]"); i++)
			{
			}
			JumpScene(script, chapter, i);
		}

		public void JumpScene(string script, string chapter, int scene)
		{
			JumpScene(script, chapter, scene, true, false);
		}

		public void JumpScene(string script, string chapter, int scene, bool only, bool load)
		{
			NewScene = false;
			bool flag = load;
			if (flag || script != _state.File)
			{
				if (!parent.LoadScript(_storyName + "\\" + script, _currentXmlDocument))
				{
					throw new FileNotFoundException(null, Path.Combine("script", _storyName + "\\" + script + ".xml"));
				}
				flag = true;
			}
			if (flag || chapter != _state.Chapter)
			{
				_currentChapter = _currentXmlDocument.DocumentElement.SelectSingleNode("chapter[@title=\"" + chapter + "\"]");
				if (InitChapter() && !load)
				{
					_newChapter = true;
				}
				flag = true;
			}
			if (flag || scene != _state.Scene)
			{
				_currentScene = _currentChapter.SelectSingleNode("scene[" + (scene + 1) + "]");
				flag = true;
			}
			_state.File = script;
			_state.Chapter = chapter;
			_state.Scene = scene;
			if (only)
			{
				Type type = parent.Types["scene"];
				ScriptCommand scriptCommand = (ScriptCommand)type.Assembly.CreateInstance(type.FullName);
				if (load)
				{
					((Scene)scriptCommand).Load = true;
				}
				scriptCommand.ConvertXml(_currentScene, _state.Info);
				bool flag2 = false;
				foreach (ScriptCommand preCommand in preCommandList)
				{
					if (preCommand is Scene && (!((Scene)preCommand).Continue || ((Scene)preCommand).Load))
					{
						flag2 = true;
						break;
					}
				}
				if (!flag2)
				{
					preCommandList.Add(scriptCommand);
				}
				_nextCommand = _currentScene.FirstChild;
				while (_nextCommand != null && _nextCommand.NodeType == XmlNodeType.Comment)
				{
					_nextCommand = _nextCommand.NextSibling;
				}
				_state.Click = 0;
			}
			nextProcessStringRead = true;
		}

		public void Jump(int click, bool next)
		{
			NewScene = false;
			if (_state.Click == click && next)
			{
				return;
			}
			if (click > 0)
			{
				_nextCommand = _currentScene.SelectSingleNode("click[" + click + "]");
				if (next)
				{
					_nextCommand = _nextCommand.NextSibling;
					while (_nextCommand != null && _nextCommand.NodeType == XmlNodeType.Comment)
					{
						_nextCommand = _nextCommand.NextSibling;
					}
					_state.Click = click;
				}
				else
				{
					_state.Click = click - 1;
				}
				nextProcessStringRead = true;
				return;
			}
			throw new InvalidOperationException("Click should be greater than 0.");
		}

		public void Jump(int click)
		{
			Jump(click, true);
		}

		public void Jump(string label)
		{
			NewScene = false;
			if (_state.Click != 0 && !(label != ScriptHelper.GetArgumentString(_currentScene.SelectSingleNode("click[" + _state.Click + "]"), _state.Info, "label", "")))
			{
				return;
			}
			XmlNode xmlNode = _currentScene.SelectSingleNode("click[@label=\"" + label + "\"]");
			if (xmlNode == null)
			{
				JumpScene(_state.File, _state.Chapter, label);
				return;
			}
			XmlNodeList xmlNodeList = _currentScene.SelectNodes("click");
			for (int i = 0; i < xmlNodeList.Count; i++)
			{
				if (xmlNode == xmlNodeList[i])
				{
					_state.Click = i + 1;
					break;
				}
			}
			_nextCommand = xmlNode.NextSibling;
			while (_nextCommand != null && _nextCommand.NodeType == XmlNodeType.Comment)
			{
				_nextCommand = _nextCommand.NextSibling;
			}
			nextProcessStringRead = true;
		}

		public void JumpFake()
		{
			nextProcessStringRead = true;
		}

		protected virtual bool InitChapter()
		{
			try
			{
				if (_currentChapterDefine == _state.ChaptereDefines[_currentChapter.Attributes["title"].Value])
				{
					GC.Collect();
					return false;
				}
				_currentChapterDefine = _state.ChaptereDefines[_currentChapter.Attributes["title"].Value];
				parent.GalCore.GameState.ClearLastString();
			}
			catch (KeyNotFoundException innerException)
			{
				throw new RuntimeException(_currentChapter, _state.Info, "Chapter's title cannot be found in defines.", innerException);
			}
			GC.Collect();
			return true;
		}

		private bool GotoNextCommand(bool passScene)
		{
			XmlNode nextCommand = _nextCommand;
			if (_nextCommand == _currentScene)
			{
				_nextCommand = _currentScene.FirstChild;
				while (_nextCommand != null && _nextCommand.NodeType == XmlNodeType.Comment)
				{
					_nextCommand = _nextCommand.NextSibling;
				}
			}
			else
			{
				do
				{
					_nextCommand = _nextCommand.NextSibling;
				}
				while (_nextCommand != null && _nextCommand.NodeType == XmlNodeType.Comment);
			}
			if (_nextCommand != null)
			{
				return true;
			}
			if (!passScene)
			{
				_nextCommand = nextCommand;
				return false;
			}
			do
			{
				_currentScene = _currentScene.NextSibling;
			}
			while (_currentScene != null && _currentScene.NodeType == XmlNodeType.Comment);
			if (_currentScene != null)
			{
				if (_currentScene.Name != "scene")
				{
					throw new ScriptStructureException(_currentScene, _state.Info, "Chapter should have scene child node only.");
				}
				_state.Scene++;
				_state.Click = 0;
				_nextCommand = _currentScene;
				return true;
			}
			do
			{
				_currentChapter = _currentChapter.NextSibling;
			}
			while (_currentChapter != null && _currentChapter.NodeType == XmlNodeType.Comment);
			if (_currentChapter != null)
			{
				_state.Chapter = ScriptHelper.GetArgumentString(_currentChapter, _state.Info, "title");
				_state.Scene = 0;
				_state.Click = 0;
				if (InitChapter())
				{
					_newChapter = true;
				}
				_currentScene = _currentChapter.FirstChild;
				while (_currentScene == null || _currentScene.NodeType == XmlNodeType.Comment)
				{
					_currentScene = _currentScene.NextSibling;
				}
				if (_currentScene.Name != "scene")
				{
					throw new ScriptStructureException(_currentScene, _state.Info, "Chapter should have scene child node only.");
				}
				_nextCommand = _currentScene;
				return true;
			}
			_currentScene = null;
			_nextCommand = null;
			_state.Chapter = "";
			_state.Scene = 0;
			_state.Click = 0;
			return false;
		}

		public List<ScriptCommand> ProcessNextCommands()
		{
			List<ScriptCommand> list = new List<ScriptCommand>();
			list.AddRange(preCommandList);
			preCommandList.Clear();
			if (justLoad)
			{
				FindNextStringNode();
				nextProcessStringRead = _nextCommand == NextStringNode;
				justLoad = false;
				return list;
			}
			if (NewScene)
			{
				GotoNextCommand(true);
				NewScene = false;
			}
			if (_nextCommand == null)
			{
				return null;
			}
			bool flag = false;
			int parValue = 0;
			do
			{
				if (nextProcessStringRead)
				{
					FindNextStringNode();
				}
				nextProcessStringRead = _nextCommand == NextStringNode;
				ScriptCommand scriptCommand = ProcessCommand(_nextCommand, _state.Info);
				if (scriptCommand is Click)
				{
					_state.Click++;
				}
				else if (scriptCommand is Par)
				{
					parValue = ((Par)scriptCommand).Value;
					GotoNextCommand(true);
					break;
				}
				if (scriptCommand != null)
				{
					list.Add(scriptCommand);
					if (scriptCommand.Single)
					{
						flag = true;
					}
				}
				if (!GotoNextCommand(false))
				{
					flag = true;
					NewScene = true;
				}
			}
			while (!flag);
			list.AddRange(ProcessPar(parValue));
			return list;
		}

		protected List<ScriptCommand> ProcessPar(int parValue)
		{
			List<ScriptCommand> list = new List<ScriptCommand>();
			while (parValue > 0)
			{
				bool flag = false;
				if (nextProcessStringRead)
				{
					FindNextStringNode();
				}
				nextProcessStringRead = _nextCommand == NextStringNode;
				ScriptCommand scriptCommand = ProcessCommand(_nextCommand, _state.Info);
				parValue--;
				if (scriptCommand is Click)
				{
					throw new ScriptCommandException(_nextCommand, _state.Info, "Click should not be within par command.");
				}
				if (scriptCommand is Wait)
				{
					if (parValue == 0)
					{
						for (int i = 0; i < list.Count; i++)
						{
							if (!(list[i] is Wait))
							{
								list[i].Lasting = false;
							}
						}
					}
					else
					{
						Wait wait = (Wait)scriptCommand;
						list.Add(scriptCommand);
						GotoNextCommand(true);
						if (nextProcessStringRead)
						{
							FindNextStringNode();
						}
						nextProcessStringRead = _nextCommand == NextStringNode;
						scriptCommand = ProcessCommand(_nextCommand, _state.Info);
						parValue--;
						if (!(scriptCommand is Par))
						{
							throw new ScriptCommandException(_nextCommand, _state.Info, "Wait should be followed with par or at the end of command series within par command.");
						}
						if (((Par)scriptCommand).Value > parValue)
						{
							throw new ScriptCommandException(_nextCommand, _state.Info, "Child par value plus number of earlier commands should not be greater than parent par value.");
						}
						GotoNextCommand(true);
						wait.ParList = ProcessPar(((Par)scriptCommand).Value);
						parValue -= ((Par)scriptCommand).Value;
						flag = true;
					}
				}
				else if (scriptCommand is Par)
				{
					throw new ScriptCommandException(_nextCommand, _state.Info, "Par should follow wait within par command.");
				}
				if (scriptCommand != null && !(scriptCommand is Par))
				{
					list.Add(scriptCommand);
				}
				if (!flag)
				{
					GotoNextCommand(false);
				}
				if (_nextCommand == null && parValue > 0)
				{
					throw new ScriptCommandException(_currentXmlDocument.DocumentElement, _state.Info, "No enough commands in par series.");
				}
			}
			return list;
		}

		protected virtual ScriptCommand ProcessCommand(XmlNode nextCommand, NodeInfo info)
		{
			if (parent.Types.ContainsKey(nextCommand.Name))
			{
				Type type = parent.Types[nextCommand.Name];
				ScriptCommand scriptCommand = (ScriptCommand)type.Assembly.CreateInstance(type.FullName);
				scriptCommand.ConvertXml(nextCommand, info);
				return scriptCommand;
			}
			return null;
		}

		public virtual string GetChapterName(string chapter)
		{
			if (!_state.ChaptereDefines.ContainsKey(chapter))
			{
				return null;
			}
			string text = _state.ChaptereDefines[chapter].File[0];
			XmlDocument xmlDocument = new XmlDocument();
			if (!parent.LoadScript(_storyName + "\\" + text, xmlDocument))
			{
				throw new FileNotFoundException(null, Path.Combine("script", _storyName + "\\" + text + ".xml"));
			}
			XmlNode xmlNode = xmlDocument.SelectSingleNode("script/chapter[@title='" + chapter + "']/scene/showchapter");
			if (xmlNode != null && xmlNode.HasChildNodes && xmlNode.ChildNodes.Count == 1 && xmlNode.ChildNodes[0].NodeType == XmlNodeType.Text)
			{
				return xmlNode.ChildNodes[0].Value;
			}
			return null;
		}

		public void LoadStory(int index)
		{
			_state.LoadState(index);
			LoadStory();
		}

		public void LoadStory(string fileName)
		{
			_state.LoadState(fileName);
			LoadStory();
		}

		private void LoadStory()
		{
			if (_state.StoryName != _storyName)
			{
				_storyName = _state.StoryName;
				parent.GalCore.GameState.InitNewState(_storyName);
				_state = parent.GalCore.GameState.CurrentGameState;
				ProcessIndex();
			}
			NodeInfo nodeInfo = new NodeInfo();
			nodeInfo.File = _state.Info.File;
			nodeInfo.Chapter = _state.Info.Chapter;
			nodeInfo.Scene = _state.Info.Scene;
			nodeInfo.Click = _state.Info.Click;
			JumpScene(nodeInfo.File, nodeInfo.Chapter, nodeInfo.Scene, true, true);
			Jump(nodeInfo.Click, false);
			foreach (ISaveable currentStateCommand in _state.CurrentStateCommands)
			{
				preCommandList.Add((ScriptCommand)currentStateCommand);
			}
			if (parent.Types.ContainsKey("showchapter"))
			{
				Type type = parent.Types["showchapter"];
				ScriptCommand scriptCommand = (ScriptCommand)type.Assembly.CreateInstance(type.FullName);
				scriptCommand.ConvertXml(_currentChapter.SelectSingleNode("scene/showchapter"), new NodeInfo());
				((ShowChapter)scriptCommand).CanRecord = false;
				preCommandList.Add(scriptCommand);
			}
			justLoad = true;
			foreach (ScriptCommand currentCommand in parent.GalCore.CurrentCommandList)
			{
				currentCommand.FinishCommand();
				if (currentCommand is IResetable)
				{
					((IResetable)currentCommand).Reset();
				}
			}
			parent.GalCore.CurrentPlayState = PlayState.Running;
		}

		public void SaveStory(int index, string comment, int icon)
		{
			_state.SaveState(index, comment, icon);
		}

		protected void FindNextStringNode()
		{
			XmlNode xmlNode = NextStringNode;
			if (xmlNode == null)
			{
				xmlNode = _nextCommand;
				if (xmlNode.Name == "paragraph" || xmlNode.Name == "dialogue")
				{
					NextStringNode = xmlNode;
					_stringRead = parent.GalCore.GameState.CheckString(ScriptHelper.GetArgumentInteger(NextStringNode, new NodeInfo(), "index"));
					return;
				}
				if (xmlNode.Name == "jump")
				{
					NextStringNode = null;
					return;
				}
			}
			XmlNode xmlNode2 = ((xmlNode.Name == "scene") ? xmlNode : xmlNode.ParentNode);
			while (true)
			{
				if (xmlNode == xmlNode2)
				{
					xmlNode = xmlNode2.FirstChild;
					while (xmlNode != null && xmlNode.NodeType == XmlNodeType.Comment)
					{
						xmlNode = xmlNode.NextSibling;
					}
				}
				else
				{
					do
					{
						xmlNode = xmlNode.NextSibling;
					}
					while (xmlNode != null && xmlNode.NodeType == XmlNodeType.Comment);
				}
				if (xmlNode == null)
				{
					do
					{
						xmlNode2 = xmlNode2.NextSibling;
					}
					while (xmlNode2 != null && xmlNode2.NodeType == XmlNodeType.Comment);
					if (xmlNode2 == null)
					{
						NextStringNode = null;
						return;
					}
					if (xmlNode2.Name != "scene")
					{
						throw new ScriptStructureException(xmlNode2, new NodeInfo(), "Chapter should have scene child node only.");
					}
					xmlNode = xmlNode2;
				}
				if (xmlNode != xmlNode2)
				{
					if (xmlNode.Name == "paragraph" || xmlNode.Name == "dialogue")
					{
						NextStringNode = xmlNode;
						_stringRead = parent.GalCore.GameState.CheckString(ScriptHelper.GetArgumentInteger(NextStringNode, new NodeInfo(), "index"));
						return;
					}
					if (xmlNode.Name == "jump")
					{
						break;
					}
				}
			}
			NextStringNode = null;
			_stringRead = true;
		}
	}
}
