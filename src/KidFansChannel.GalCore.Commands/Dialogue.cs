using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Xml;
using KidFansChannel.DataCore;
using KidFansChannel.GalCore.Records;
using KidFansChannel.GalCore.Screens;

namespace KidFansChannel.GalCore.Commands
{
	public abstract class Dialogue : ScriptCommand, IStaticDataUser, IDrawable, IUpdatable, IResetable, ISaveable, IClickable, IRecordable
	{
		public enum DialogueState
		{
			Ready,
			ShowingBack,
			ShowingSpeakerBack,
			ShowingString,
			Shown,
			HidingString,
			HidingSpeakerBack,
			HidingBack,
			Hiden
		}

		public class LinkClickArgs : EventArgs
		{
			public int Value;

			public LinkClickArgs(int value)
			{
				Value = value;
			}
		}

		public static DialogueState State;

		private static int _stringNumber;

		private string _voiceStr;

		private List<int> _speakers = new List<int>();

		private int _index;

		private bool _showSpeaker;

		private bool _hideSpeaker;

		private short[] _voiceVol;

		private float _time;

		private static String2D _string;

		private static String2D _speakerString;

		private static String2D _currentString;

		protected bool ClearDialogue;

		protected bool HasVoice;

		protected bool PreparedVoice;

		protected bool StoppedVoice;

		protected float StringLength;

		protected float StringProgress;

		public static float VoiceHighlightFactor;

		protected float VoiceTime;

		protected float VoiceSpeed;

		protected float VoiceProgress;

		private int loadNum;

		private int saveStringNumber;

		private bool canRecord;

		public static int StringNumber
		{
			get
			{
				return _stringNumber;
			}
			set
			{
				_stringNumber = value;
			}
		}

		protected string VoiceStr
		{
			get
			{
				return _voiceStr;
			}
			set
			{
				_voiceStr = value;
			}
		}

		public List<int> Speakers
		{
			get
			{
				return _speakers;
			}
		}

		public int Index
		{
			get
			{
				return _index;
			}
			protected set
			{
				_index = value;
			}
		}

		protected bool ShowSpeaker
		{
			get
			{
				return _showSpeaker;
			}
			set
			{
				_showSpeaker = value;
			}
		}

		protected bool HideSpeaker
		{
			get
			{
				return _hideSpeaker;
			}
			set
			{
				_hideSpeaker = value;
			}
		}

		public short[] VoiceVol
		{
			get
			{
				return _voiceVol;
			}
			protected set
			{
				_voiceVol = value;
			}
		}

		public float Time
		{
			get
			{
				return _time;
			}
			set
			{
				_time = value;
			}
		}

		public static String2D String
		{
			get
			{
				return _string;
			}
			protected set
			{
				_string = value;
				_string.CheckLinkRead += String_CheckLinkRead;
				_string.ClickLink += String_ClickLink;
			}
		}

		public static String2D SpeakerString
		{
			get
			{
				return _speakerString;
			}
			protected set
			{
				_speakerString = value;
			}
		}

		public static String2D CurrentString
		{
			get
			{
				return _currentString;
			}
			set
			{
				_currentString = value;
			}
		}

		public int LayerMajor
		{
			get
			{
				return 26;
			}
		}

		public float LayerMinor
		{
			get
			{
				return 0f;
			}
		}

		public bool CanHide
		{
			get
			{
				return true;
			}
		}

		public bool ToClickState
		{
			get
			{
				return false;
			}
		}

		public static event EventHandler<LinkClickArgs> ClickLink;

		public Dialogue()
		{
			Single = true;
			Lasting = true;
			Command = "dialogue";
			DataName = "voice";
		}

		static Dialogue()
		{
			_stringNumber = 0;
			VoiceHighlightFactor = 0.25f;
			State = DialogueState.Ready;
		}

		private static void String_CheckLinkRead(object sender, String2D.LinkArgs e)
		{
			ScriptCommand.GameState.UpdateEncyOpen(e.Value);
			e.Read = ScriptCommand.GameState.CheckEncyView(e.Value);
		}

		private static void String_ClickLink(object sender, String2D.LinkArgs e)
		{
			ScriptCommand.GalCore.ShowEncyContentScreen(e.Value);
			if (ClickLink != null)
			{
				ClickLink(null, new LinkClickArgs(e.Value));
			}
		}

		public override void ConvertXml(XmlNode node, NodeInfo info)
		{
			base.ConvertXml(node, info);
			if (!node.HasChildNodes)
			{
				throw new ScriptStructureException(node, info, "Dialogue should have child node as content.");
			}
			if (node.InnerXml.Contains("\n") || node.InnerXml.Contains("\r"))
			{
				throw new ScriptStructureException(node, info, "Dialogue string should be in one line.");
			}
			_string.ClearClick();
			StringLength = _string.TotalWidth;
			_string.AddXmlString(node.ChildNodes, info);
			_currentString.ClearString();
			_currentString.AddXmlString(node.ChildNodes, info);
			try
			{
				_voiceStr = ScriptHelper.GetArgumentString(node, info, "voice");
			}
			catch (ScriptArgumentException)
			{
			}
			string argumentString = ScriptHelper.GetArgumentString(node, info, "speaker", string.Empty);
			if (argumentString != string.Empty)
			{
				string[] nodes = argumentString.Split(';');
				string[] nodeInfos = nodes;
				foreach (string s in nodeInfos)
				{
					try
					{
						_speakers.Add(int.Parse(s));
					}
					catch (Exception innerException)
					{
						throw new ScriptArgumentException(node, info, "speaker", innerException);
					}
				}
			}
			_time = ScriptHelper.GetArgumentFloat(node, info, "time", float.NaN);
			_index = ScriptHelper.GetArgumentInteger(node, info, "index", 0);
			if (_string.Speaker != null)
			{
				_showSpeaker = true;
				_speakerString.ClearString();
				_speakerString.AddString(_string.Speaker, ContentAlignment.TopCenter);
			}
			else
			{
				_showSpeaker = false;
			}
			_hideSpeaker = true;
			XmlNode xmlNode = node;
			while (xmlNode.NextSibling != null)
			{
				xmlNode = xmlNode.NextSibling;
				if (xmlNode.Name == "dialogueclear")
				{
					_hideSpeaker = false;
					break;
				}
				if (xmlNode.Name == "dialogue")
				{
					if (xmlNode.SelectSingleNode("s") != null)
					{
						_hideSpeaker = false;
					}
					else
					{
						_hideSpeaker = true;
					}
					break;
				}
			}
			string text = _string.String;
			while (text.Length > 0 && text[0] == '\n')
			{
				text = text.Substring(1);
			}
			while (text.Length > 0 && text[text.Length - 1] == '\n')
			{
				text = text.Substring(0, text.Length - 1);
			}
			int length;
			do
			{
				length = text.Length;
				text = text.Replace("\n\n", "\n");
			}
			while (text.Length < length);
			ScriptCommand.GameState.UpdateString(((_string.Speaker == null) ? string.Empty : _string.Speaker) + text, _index);
			if (loadNum == 0)
			{
				canRecord = true;
			}
		}

		public override void InitCommand()
		{
			base.InitCommand();
			if (loadNum > 0)
			{
				XmlNode xmlNode = ScriptCommand.GameScript.CommandNode;
				XmlNode[] nodes = new XmlNode[loadNum];
				NodeInfo[] nodeInfos = new NodeInfo[loadNum];
				for (int i = 0; i < loadNum; i++)
				{
					do
					{
						xmlNode = xmlNode.PreviousSibling;
					}
					while (xmlNode != null && xmlNode.Name != Command);
					if (xmlNode == null)
					{
						throw new RuntimeException(ScriptCommand.GameScript.CommandNode, new NodeInfo(), "No enough dialogue to load.");
					}
					nodes[loadNum - i - 1] = xmlNode;
					nodeInfos[loadNum - i - 1] = ScriptCommand.GameState.CurrentGameState.Info;
				}
				for (int j = 0; j < loadNum; j++)
				{
					ConvertXml(nodes[j], nodeInfos[j]);
				}
				StringLength = 0f;
				_stringNumber = loadNum;
			}
			else
			{
				_stringNumber++;
			}
			if (State == DialogueState.Shown)
			{
				if (_showSpeaker && !CheckSpeakerShown())
				{
					ShowSpeakerBack();
				}
				else
				{
					if (_showSpeaker)
					{
						ChangeSpeakerBack();
					}
					PrepareString();
					State = DialogueState.ShowingString;
				}
			}
			foreach (ScriptCommand command in ScriptCommand.CommandList)
			{
				if (command is Dialogue && command != this)
				{
					command.FinishCommand();
				}
			}
			if (_string.Comment != null)
			{
				ScriptCommand.GalCore.ShowComment(_string.Comment);
			}
		}

		public void FinishString(bool finishBack)
		{
			State = DialogueState.HidingString;
			if (HasVoice)
			{
				ScriptCommand.Sound.StopVoice();
				StoppedVoice = true;
				ScriptCommand.Sound.ChangeVolumeTempBgm(1f);
				foreach (ScriptCommand command in ScriptCommand.CommandList)
				{
					if (command is CharaDisplay && _speakers.Contains(((CharaDisplay)command).Id))
					{
						((CharaDisplay)command).MouthStop();
					}
				}
			}
			Lasting = true;
			if (_string.Comment != null)
			{
				ScriptCommand.GalCore.HideComment();
			}
			if (finishBack)
			{
				ClearDialogue = true;
			}
			_stringNumber = 0;
		}

		~Dialogue()
		{
			_speakers.Clear();
		}

		public abstract void InitStaticData();

		public abstract void FinalStaticData();

		public void Draw()
		{
			switch (State)
			{
			case DialogueState.ShowingBack:
				DrawBackGround();
				break;
			case DialogueState.ShowingSpeakerBack:
				DrawBackGround();
				DrawSpeakerBackGround();
				break;
			case DialogueState.ShowingString:
			case DialogueState.Shown:
			case DialogueState.HidingString:
				DrawBackGround();
				DrawString();
				if (_showSpeaker)
				{
					DrawSpeakerBackGround();
					DrawSpeakerString();
				}
				break;
			case DialogueState.HidingSpeakerBack:
				DrawBackGround();
				DrawSpeakerBackGround();
				break;
			case DialogueState.HidingBack:
				DrawBackGround();
				break;
			case DialogueState.Ready:
			case DialogueState.Hiden:
				break;
			}
		}

		protected abstract void DrawBackGround();

		protected abstract void DrawSpeakerBackGround();

		protected virtual void DrawString()
		{
			_string.Canvas = ScriptCommand.GalCore.CurrentCanvas;
			if (HasVoice && ScriptCommand.Options.TextSynchro)
			{
				if (StringProgress == 1f)
				{
					_string.DrawString();
				}
				else
				{
					_string.DrawPartString(StringProgress);
				}
			}
			else if (StringLength == (float)_string.TotalWidth)
			{
				_string.DrawString();
			}
			else
			{
				_string.DrawSubString(StringLength);
			}
		}

		private void DrawSpeakerString()
		{
			_speakerString.Canvas = ScriptCommand.GalCore.CurrentCanvas;
			_speakerString.DrawString();
		}

		public void Update(double appTime, float elaspedTime)
		{
			switch (State)
			{
			case DialogueState.Ready:
				ShowBack();
				break;
			case DialogueState.ShowingBack:
				UpdateBack(elaspedTime);
				if (CheckShown())
				{
					if (_showSpeaker)
					{
						ShowSpeakerBack();
					}
					else
					{
						PrepareString();
					}
				}
				break;
			case DialogueState.ShowingSpeakerBack:
				UpdateBack(elaspedTime);
				UpdateSpeakerBack(elaspedTime);
				if (CheckSpeakerShown())
				{
					PrepareString();
					PrepareSpeakerString();
				}
				break;
			case DialogueState.ShowingString:
				UpdateBack(elaspedTime);
				UpdateString(elaspedTime);
				if (_showSpeaker)
				{
					UpdateSpeakerBack(elaspedTime);
					UpdateSpeakerString(elaspedTime);
				}
				if (CheckStringShown())
				{
					State = DialogueState.Shown;
					Lasting = false;
				}
				break;
			case DialogueState.Shown:
				CheckKey();
				UpdateBack(elaspedTime);
				UpdateString(elaspedTime);
				if (_showSpeaker)
				{
					UpdateSpeakerBack(elaspedTime);
					UpdateSpeakerString(elaspedTime);
				}
				break;
			case DialogueState.HidingString:
				UpdateBack(elaspedTime);
				UpdateString(elaspedTime);
				if (_showSpeaker)
				{
					UpdateSpeakerBack(elaspedTime);
					UpdateSpeakerString(elaspedTime);
				}
				if (CheckStringHiden())
				{
					ClearString();
					if (_showSpeaker)
					{
						ClearSpeakerString();
					}
					if (_showSpeaker && _hideSpeaker)
					{
						HideSpeakerBack();
						break;
					}
					if (ClearDialogue)
					{
						HideBack();
						break;
					}
					State = DialogueState.Shown;
					Lasting = false;
				}
				break;
			case DialogueState.HidingSpeakerBack:
				UpdateBack(elaspedTime);
				UpdateSpeakerBack(elaspedTime);
				if (CheckSpeakerHiden())
				{
					if (ClearDialogue)
					{
						HideBack();
						break;
					}
					State = DialogueState.Shown;
					Lasting = false;
				}
				break;
			case DialogueState.HidingBack:
				UpdateBack(elaspedTime);
				if (CheckHiden())
				{
					State = DialogueState.Hiden;
				}
				break;
			case DialogueState.Hiden:
				FinishCommand();
				Lasting = false;
				State = DialogueState.Ready;
				break;
			}
		}

		protected void CheckKey()
		{
			if (_string.LinkList.Count == 0)
			{
				return;
			}
			if (ScriptCommand.GalCore.GameInput.CheckDirectionPress(0))
			{
				if (_string.CurrentPointedLinkValue == -1)
				{
					_string.CurrentPointedLinkValue = _string.LinkList[_string.LinkList.Count - 1].Value;
					return;
				}
				List<int> list = new List<int>();
				for (int i = 0; i < _string.LinkList.Count; i++)
				{
					if (!list.Contains(_string.LinkList[i].Value))
					{
						list.Add(_string.LinkList[i].Value);
					}
				}
				int loadIndex = list.IndexOf(_string.CurrentPointedLinkValue);
				if (loadIndex != -1)
				{
					loadIndex--;
					if (loadIndex == -1)
					{
						_string.CurrentPointedLinkValue = -1;
					}
					else
					{
						_string.CurrentPointedLinkValue = list[loadIndex];
					}
				}
			}
			else
			{
				if (!ScriptCommand.GalCore.GameInput.CheckDirectionPress(1))
				{
					return;
				}
				if (_string.CurrentPointedLinkValue == -1)
				{
					_string.CurrentPointedLinkValue = _string.LinkList[0].Value;
					return;
				}
				List<int> list2 = new List<int>();
				for (int j = 0; j < _string.LinkList.Count; j++)
				{
					if (!list2.Contains(_string.LinkList[j].Value))
					{
						list2.Add(_string.LinkList[j].Value);
					}
				}
				int num2 = list2.IndexOf(_string.CurrentPointedLinkValue);
				if (num2 != -1)
				{
					num2++;
					if (num2 == list2.Count)
					{
						_string.CurrentPointedLinkValue = -1;
					}
					else
					{
						_string.CurrentPointedLinkValue = list2[num2];
					}
				}
			}
		}

		protected virtual void ShowBack()
		{
			State = DialogueState.ShowingBack;
		}

		protected virtual void HideBack()
		{
			State = DialogueState.HidingBack;
		}

		protected virtual void ShowSpeakerBack()
		{
			State = DialogueState.ShowingSpeakerBack;
		}

		protected virtual void ChangeSpeakerBack()
		{
		}

		protected virtual void HideSpeakerBack()
		{
			State = DialogueState.HidingSpeakerBack;
		}

		protected abstract void UpdateBack(float elaspedTime);

		protected abstract void UpdateSpeakerBack(float elaspedTime);

		protected virtual bool CheckShown()
		{
			return true;
		}

		protected virtual bool CheckHiden()
		{
			return true;
		}

		protected virtual bool CheckSpeakerShown()
		{
			return true;
		}

		protected virtual bool CheckSpeakerHiden()
		{
			return true;
		}

		protected virtual void PrepareString()
		{
			State = DialogueState.ShowingString;
			if (ScriptCommand.PlayModeQuicking && ScriptCommand.PlayMode != PlayMode.QuickOnce)
			{
				StringLength = _string.TotalWidth;
				StringProgress = 1f;
				HasVoice = false;
				PreparedVoice = false;
				StoppedVoice = false;
				return;
			}
			if (StringLength == 0f)
			{
				StringLength = -String2D.AlphaWidth;
			}
			if (_voiceStr != null)
			{
				switch (ScriptCommand.Data.CheckData(DataName, _voiceStr + ".ogg", KidFansChannel.DataCore.DataCore.DataType.Package))
				{
				case KidFansChannel.DataCore.DataCore.DataType.Package:
					ScriptCommand.Sound.OpenVoice(ScriptCommand.Data.GetStream());
					HasVoice = true;
					PreparedVoice = false;
					StoppedVoice = false;
					VoiceTime = 0f;
					VoiceProgress = 0f;
					break;
				case KidFansChannel.DataCore.DataCore.DataType.File:
					ScriptCommand.Sound.OpenVoice(ScriptCommand.Data.GetFile());
					HasVoice = true;
					PreparedVoice = false;
					StoppedVoice = false;
					VoiceTime = 0f;
					VoiceProgress = 0f;
					break;
				default:
					HasVoice = false;
					PreparedVoice = false;
					StoppedVoice = false;
					return;
				}
				Stream stream = null;
				switch (ScriptCommand.Data.CheckData("voice", _voiceStr + ".vol"))
				{
				case KidFansChannel.DataCore.DataCore.DataType.Package:
					stream = ScriptCommand.Data.GetStream();
					break;
				case KidFansChannel.DataCore.DataCore.DataType.File:
					stream = new FileStream(ScriptCommand.Data.GetFile(), FileMode.Open, FileAccess.Read);
					break;
				}
				if (stream == null)
				{
					return;
				}
				BinaryReader binaryReader = new BinaryReader(stream);
				_voiceVol = new short[binaryReader.BaseStream.Length >> 1];
				for (int i = 0; i < _voiceVol.Length; i++)
				{
					_voiceVol[i] = binaryReader.ReadInt16();
				}
				binaryReader.Close();
				stream.Close();
				{
					foreach (ScriptCommand currentCommand in ScriptCommand.GalCore.CurrentCommandList)
					{
						if (currentCommand is CharaDisplay && _speakers.Contains(((CharaDisplay)currentCommand).Id))
						{
							((CharaDisplay)currentCommand).MouthPrepare(_voiceVol);
						}
					}
					return;
				}
			}
			HasVoice = false;
			PreparedVoice = false;
			StoppedVoice = false;
		}

		protected virtual void PrepareSpeakerString()
		{
		}

		protected virtual void UpdateString(float elaspedTime)
		{
			if (HasVoice && !StoppedVoice && PreparedVoice && VoiceTime * VoiceSpeed >= 1f)
			{
				StoppedVoice = true;
				ScriptCommand.Sound.ChangeVolumeTempBgm(1f);
			}
			switch (State)
			{
			case DialogueState.ShowingString:
				if (HasVoice)
				{
					if (!PreparedVoice)
					{
						if (!ScriptCommand.Sound.PreparedVoice)
						{
							break;
						}
						ScriptCommand.Sound.PlayVoice();
						PreparedVoice = true;
						VoiceSpeed = 1f / ScriptCommand.Sound.LengthVoice;
						if (ScriptCommand.Options.VoiceHighlight)
						{
							ScriptCommand.Sound.ChangeVolumeTempBgm(VoiceHighlightFactor);
						}
						foreach (ScriptCommand currentCommand in ScriptCommand.GalCore.CurrentCommandList)
						{
							if (currentCommand is CharaDisplay && _speakers.Contains(((CharaDisplay)currentCommand).Id))
							{
								((CharaDisplay)currentCommand).MouthPlay();
							}
						}
					}
					else
					{
						VoiceTime += elaspedTime;
						VoiceProgress = VoiceTime * VoiceSpeed;
						if (VoiceProgress > 1f)
						{
							VoiceProgress = 1f;
							VoiceTime = 1f / VoiceSpeed;
						}
					}
				}
				if (HasVoice && ScriptCommand.Options.TextSynchro)
				{
					if (ScriptCommand.PlayModeQuicking)
					{
						StringProgress = 1f;
						if (ScriptCommand.PlayMode != PlayMode.QuickOnce)
						{
							ScriptCommand.Sound.StopVoice();
						}
					}
					else
					{
						StringProgress = VoiceProgress;
					}
				}
				else if (ScriptCommand.PlayModeQuicking)
				{
					StringLength = _string.TotalWidth;
				}
				else
				{
					if (float.IsNaN(_time))
					{
						StringLength += elaspedTime * GetSpeed();
					}
					else
					{
						StringLength += (float)_string.TotalWidth * elaspedTime / _time;
					}
					if (StringLength >= (float)_string.TotalWidth)
					{
						StringLength = _string.TotalWidth;
					}
				}
				break;
			case DialogueState.Shown:
				if (HasVoice && PreparedVoice)
				{
					VoiceTime += elaspedTime;
				}
				break;
			case DialogueState.HidingString:
				break;
			}
		}

		protected virtual void UpdateSpeakerString(float elaspedTime)
		{
			switch (State)
			{
			}
		}

		private void ClearString()
		{
			String.ClearString();
			HasVoice = false;
			PreparedVoice = false;
			StoppedVoice = false;
			StringLength = -String2D.AlphaWidth;
			StringProgress = 0f;
		}

		private void ClearSpeakerString()
		{
			SpeakerString.ClearString();
		}

		protected virtual bool CheckStringShown()
		{
			if (HasVoice && ScriptCommand.Options.TextSynchro)
			{
				return StringProgress == 1f;
			}
			return StringLength == (float)_string.TotalWidth;
		}

		protected virtual bool CheckStringHiden()
		{
			return true;
		}

		protected abstract float GetSpeed();

		public void Reset()
		{
			State = DialogueState.Ready;
			_stringNumber = 0;
			_string.ClearString();
			_speakerString.ClearString();
			ScriptCommand.Sound.StopVoice();
		}

		public void Save(BinaryWriter bw)
		{
			bw.Write(Command);
			bw.Write(_stringNumber);
		}

		public void Load(BinaryReader br)
		{
			loadNum = br.ReadInt32();
		}

		public void ConfirmUpdate()
		{
			saveStringNumber = _stringNumber;
		}

		public bool OnItemClick()
		{
			return _string.OnItemClick();
		}

		public void OnItemMove(Point location)
		{
			_string.OnItemMove(location);
		}

		public Record[] GetRecord()
		{
			if (canRecord)
			{
				canRecord = false;
				if (_showSpeaker)
				{
					TextRecord textRecord = new TextRecord();
					textRecord.Text = _currentString.String;
					while (textRecord.Text.Length > 0 && textRecord.Text[0] == '\n')
					{
						textRecord.Text = textRecord.Text.Substring(1);
					}
					while (textRecord.Text.Length > 0 && textRecord.Text[textRecord.Text.Length - 1] == '\n')
					{
						textRecord.Text = textRecord.Text.Substring(0, textRecord.Text.Length - 1);
					}
					int length;
					do
					{
						length = textRecord.Text.Length;
						textRecord.Text = textRecord.Text.Replace("\n\n", "\n");
					}
					while (textRecord.Text.Length < length);
					if (_currentString.Comment != null)
					{
						textRecord.Text = textRecord.Text + "（" + _currentString.Comment + "）";
					}
					textRecord.Line = String2D.CalLine(textRecord.Text, _currentString.Font, MessageScreen.TextWithSpeakerWidth);
					textRecord.Text = _speakerString.String + textRecord.Text;
					textRecord.Speakers.AddRange(_speakers);
					textRecord.SpeakerNameLength = _speakerString.String.Length;
					if (_voiceStr != null)
					{
						textRecord.Voice = _voiceStr;
					}
					return new Record[1] { textRecord };
				}
				TextRecord textRecord2 = new TextRecord();
				textRecord2.Text = _currentString.String;
				while (textRecord2.Text.Length > 0 && textRecord2.Text[0] == '\n')
				{
					textRecord2.Text = textRecord2.Text.Substring(1);
				}
				while (textRecord2.Text.Length > 0 && textRecord2.Text[textRecord2.Text.Length - 1] == '\n')
				{
					textRecord2.Text = textRecord2.Text.Substring(0, textRecord2.Text.Length - 1);
				}
				int length2;
				do
				{
					length2 = textRecord2.Text.Length;
					textRecord2.Text = textRecord2.Text.Replace("\n\n", "\n");
				}
				while (textRecord2.Text.Length < length2);
				if (_currentString.Comment != null)
				{
					textRecord2.Text = textRecord2.Text + "（" + _currentString.Comment + "）";
				}
				textRecord2.Line = String2D.CalLine(textRecord2.Text, _currentString.Font, MessageScreen.TextWithoutSpeakerWidth);
				if (_voiceStr != null)
				{
					textRecord2.Voice = _voiceStr;
				}
				return new Record[1] { textRecord2 };
			}
			return null;
		}
	}
}
