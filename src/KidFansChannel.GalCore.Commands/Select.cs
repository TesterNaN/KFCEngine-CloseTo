using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Xml;
using KidFansChannel.GalCore.Defines;
using KidFansChannel.GalCore.Records;

namespace KidFansChannel.GalCore.Commands
{
	public abstract class Select : ScriptCommand, IStaticDataUser, IDrawable, IUpdatable, IClickable, ISaveable, IRecordable, IResetable
	{
		public enum SelectState
		{
			Ready,
			Showing,
			Shown,
			Hiding,
			Hiden
		}

		private string _title;

		private List<string> _cases = new List<string>();

		private List<String2D> _itemStrings = new List<String2D>();

		private SelectDefine _define;

		public static int SelectCenterY = 240;

		private bool previousExist;

		public static SelectState State;

		private bool selectedShow;

		private int _currentSelected = -1;

		private bool _toClickState;

		private bool canRecord;

		public string Title
		{
			get
			{
				return _title;
			}
			protected set
			{
				_title = value;
			}
		}

		public List<string> Cases
		{
			get
			{
				return _cases;
			}
			protected set
			{
				_cases = value;
			}
		}

		public List<String2D> ItemStrings
		{
			get
			{
				return _itemStrings;
			}
			protected set
			{
				_itemStrings = value;
			}
		}

		protected SelectDefine Define
		{
			get
			{
				return _define;
			}
			set
			{
				_define = value;
			}
		}

		public int LayerMajor
		{
			get
			{
				return 28;
			}
		}

		public float LayerMinor
		{
			get
			{
				return 0f;
			}
		}

		public virtual bool CanHide
		{
			get
			{
				return true;
			}
		}

		public int CurrentSelected
		{
			get
			{
				return _currentSelected;
			}
			set
			{
				_currentSelected = value;
			}
		}

		public virtual bool ToClickState
		{
			get
			{
				return _toClickState;
			}
		}

		public Select()
		{
			Single = true;
			Lasting = true;
			Command = "select";
		}

		public override void ConvertXml(XmlNode node, NodeInfo info)
		{
			base.ConvertXml(node, info);
			_title = ScriptHelper.GetArgumentString(node, info, "title");
			if (!ScriptCommand.GameState.CurrentGameState.SelectDefines.ContainsKey(_title))
			{
				throw new RuntimeException(node, info, "Select should be defined in index first.");
			}
			_define = ScriptCommand.GameState.CurrentGameState.SelectDefines[_title];
			_cases.Clear();
			_itemStrings.Clear();
			foreach (XmlNode childNode in node.ChildNodes)
			{
				if (childNode.Name == "item")
				{
					string argumentString = ScriptHelper.GetArgumentString(childNode, info, "case");
					_cases.Add(argumentString);
					if (childNode.ChildNodes == null || childNode.ChildNodes.Count != 1 || childNode.FirstChild.NodeType != XmlNodeType.Text)
					{
						throw new ScriptStructureException(childNode, info, "Item should have only one text child.");
					}
					if (childNode.FirstChild.Value.Contains("\n") || childNode.FirstChild.Value.Contains("\r"))
					{
						throw new ScriptStructureException(childNode, info, "Select item string should be in one line.");
					}
					String2D string2D = new String2D();
					bool chosen = ScriptCommand.GameState.CheckSelect(Title, argumentString);
					selectedShow = ScriptCommand.Options.SelectedShow;
					SetFontStyle(string2D, chosen);
					string2D.AddString(childNode.FirstChild.Value, ContentAlignment.TopLeft);
					_itemStrings.Add(string2D);
				}
				else if (childNode.NodeType != XmlNodeType.Comment)
				{
					throw new ScriptStructureException(childNode, info, "Select's child nodes should be item.");
				}
			}
			for (int i = 0; i < _cases.Count; i++)
			{
				if (_define.Case.IndexOf(_cases[i]) == -1)
				{
					throw new RuntimeException(node, info, "Select item should be within its define.");
				}
			}
		}

		public override void InitCommand()
		{
			base.InitCommand();
			if (_itemStrings.Count == 0)
			{
				return;
			}
			for (int i = 0; i < _itemStrings.Count; i++)
			{
				int x = BgDisplay.ScreenSize.Width / 2 - _itemStrings[i].Width / 2;
				int y = SelectCenterY - _itemStrings.Count * _itemStrings[i].LineSize.Height / 2 + i * _itemStrings[i].LineSize.Height;
				_itemStrings[i].Location = new Point(x, y);
			}
			SetBackStyle();
			foreach (ScriptCommand command in ScriptCommand.CommandList)
			{
				if (command is Select && command != this)
				{
					command.FinishCommand();
					previousExist = true;
				}
			}
		}

		public override void FinalCommand()
		{
			_cases.Clear();
			_itemStrings.Clear();
			base.FinalCommand();
		}

		protected abstract void SetBackStyle();

		protected abstract void SetFontStyle(String2D itemString, bool chosen);

		public abstract void InitStaticData();

		public abstract void FinalStaticData();

		public virtual void Draw()
		{
			if (_itemStrings.Count == 0)
			{
				return;
			}
			switch (State)
			{
			case SelectState.Showing:
				DrawBackGround();
				break;
			case SelectState.Shown:
				DrawBackGround();
				if (_currentSelected >= 0)
				{
					DrawItem();
				}
				DrawString();
				break;
			case SelectState.Hiding:
				DrawBackGround();
				break;
			case SelectState.Ready:
			case SelectState.Hiden:
				break;
			}
		}

		public virtual void Update(double appTime, float elaspedTime)
		{
			if (_itemStrings.Count == 0)
			{
				return;
			}
			if (ScriptCommand.GalCore.ToNormal || ScriptCommand.GalCore.CurrentPlayMode == PlayMode.QuickOnce)
			{
				ScriptCommand.GalCore.CurrentPlayMode = PlayMode.Normal;
				ScriptCommand.GalCore.ToNormal = false;
			}
			switch (State)
			{
			case SelectState.Ready:
				Show();
				break;
			case SelectState.Showing:
				UpdateBack(elaspedTime);
				if (CheckShown())
				{
					State = SelectState.Shown;
					ScriptCommand.GalCore.CurrentPlayState = PlayState.ToClick;
					_toClickState = true;
					ScriptCommand.GalCore.GameState.CurrentGameState.Update(ScriptCommand.GalCore.CurrentCommandList);
					if (!previousExist && (ScriptCommand.Options.QuickSave & 1) == 1)
					{
						ScriptCommand.GalCore.GameScript.QuickSave(QuickSaveType.Select);
					}
				}
				break;
			case SelectState.Shown:
				ScriptCommand.GalCore.CurrentPlayState = PlayState.ToClick;
				_toClickState = true;
				CheckKey();
				if (selectedShow != ScriptCommand.Options.SelectedShow)
				{
					for (int i = 0; i < _itemStrings.Count; i++)
					{
						SetFontStyle(_itemStrings[i], ScriptCommand.GameState.CheckSelect(Title, _cases[i]));
					}
					selectedShow = ScriptCommand.Options.SelectedShow;
				}
				UpdateBack(elaspedTime);
				UpdateItem(elaspedTime);
				break;
			case SelectState.Hiding:
				UpdateBack(elaspedTime);
				if (CheckHiden())
				{
					State = SelectState.Hiden;
				}
				break;
			case SelectState.Hiden:
				Lasting = false;
				FinishCommand();
				State = SelectState.Ready;
				break;
			}
		}

		protected virtual void CheckKey()
		{
			if (_itemStrings.Count == 0)
			{
				return;
			}
			if (ScriptCommand.GalCore.GameInput.CheckDirectionPress(2))
			{
				if (_currentSelected == -1)
				{
					_currentSelected = _itemStrings.Count - 1;
					return;
				}
				_currentSelected--;
				if (_currentSelected == -1)
				{
					_currentSelected = _itemStrings.Count - 1;
				}
			}
			else
			{
				if (!ScriptCommand.GalCore.GameInput.CheckDirectionPress(3))
				{
					return;
				}
				if (_currentSelected == -1)
				{
					_currentSelected = 0;
					return;
				}
				_currentSelected++;
				if (_currentSelected == _itemStrings.Count)
				{
					_currentSelected = 0;
				}
			}
		}

		protected virtual void Show()
		{
			State = SelectState.Showing;
		}

		protected virtual void Hide()
		{
			State = SelectState.Hiding;
			_toClickState = false;
		}

		protected abstract void UpdateBack(float elaspedTime);

		protected abstract void UpdateItem(float elaspedTime);

		protected abstract bool CheckShown();

		protected abstract bool CheckHiden();

		protected abstract void DrawBackGround();

		protected abstract void DrawItem();

		protected virtual void DrawString()
		{
			foreach (String2D itemString in _itemStrings)
			{
				itemString.Canvas = ScriptCommand.GalCore.CurrentCanvas;
				itemString.DrawString();
			}
		}

		public virtual bool OnItemClick()
		{
			if (_itemStrings.Count == 0)
			{
				return false;
			}
			if (_currentSelected < 0)
			{
				return false;
			}
			_define.Value = _define.Case.IndexOf(_cases[_currentSelected]);
			ScriptCommand.GameState.UpdateSelect(_title, _cases[_currentSelected]);
			Hide();
			foreach (ScriptCommand currentCommand in ScriptCommand.GalCore.CurrentCommandList)
			{
				if (currentCommand is Dialogue)
				{
					((Dialogue)currentCommand).FinishString(false);
				}
			}
			ScriptCommand.GalCore.CurrentPlayState = PlayState.Running;
			canRecord = true;
			return true;
		}

		public virtual void OnItemMove(Point location)
		{
			if (_itemStrings.Count == 0)
			{
				return;
			}
			_currentSelected = -1;
			for (int i = 0; i < _itemStrings.Count; i++)
			{
				if (location.X >= _itemStrings[i].Location.X && location.X < _itemStrings[i].Location.X + _itemStrings[i].Width && location.Y >= _itemStrings[i].Location.Y && location.Y < _itemStrings[i].Location.Y + _itemStrings[i].LineSize.Height)
				{
					_currentSelected = i;
					break;
				}
			}
		}

		public void Save(BinaryWriter bw)
		{
			bw.Write(Command);
		}

		public void Load(BinaryReader br)
		{
			Lasting = false;
		}

		public void ConfirmUpdate()
		{
		}

		public Record[] GetRecord()
		{
			if (canRecord)
			{
				canRecord = false;
				SelectRecord[] records = new SelectRecord[_cases.Count];
				for (int i = 0; i < _cases.Count; i++)
				{
					records[i] = new SelectRecord();
					records[i].Text = _itemStrings[i].String;
					if (_cases[i] == _define.Case[_define.Value])
					{
						records[i].Selected = true;
					}
					else
					{
						records[i].Selected = false;
					}
					records[i].Title = _define.Title;
					records[i].Case = _cases[i];
				}
				return records;
			}
			return null;
		}

		public void Reset()
		{
			State = SelectState.Ready;
		}
	}
}
