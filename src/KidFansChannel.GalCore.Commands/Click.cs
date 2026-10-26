using System;
using System.Drawing;
using System.Xml;

namespace KidFansChannel.GalCore.Commands
{
	public abstract class Click : ScriptCommand, IClickable, IDrawable, IUpdatable, IStaticDataUser
	{
		private int _type;

		private string _label;

		private float _time;

		private Timer timer;

		private Point _clickPosition;

		private float lastTime;

		private bool _toClickState;

		protected int Type
		{
			get
			{
				return _type;
			}
			set
			{
				_type = value;
			}
		}

		public string Label
		{
			get
			{
				return _label;
			}
			protected set
			{
				_label = value;
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

		protected virtual bool QuickSaveChapter
		{
			get
			{
				return true;
			}
		}

		protected Point ClickPosition
		{
			get
			{
				return _clickPosition;
			}
		}

		public bool ToClickState
		{
			get
			{
				return _toClickState;
			}
		}

		public int LayerMajor
		{
			get
			{
				return 27;
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

		public Click()
		{
			Single = true;
			Lasting = true;
			Command = "click";
		}

		public override void ConvertXml(XmlNode node, NodeInfo info)
		{
			base.ConvertXml(node, info);
			try
			{
				_type = ScriptHelper.GetArgumentInteger(node, info, "type", 0);
				if (_type < 0)
				{
					throw new ScriptArgumentException(node, info, "type", "Type must not less than zero or be null");
				}
			}
			catch (ScriptArgumentException ex)
			{
				if (!(ex.InnerException is FormatException) || !(ScriptHelper.GetArgumentString(node, info, "type") == "null"))
				{
					throw;
				}
				_type = -1;
			}
			_label = ScriptHelper.GetArgumentString(node, info, "label", string.Empty);
			_time = ScriptHelper.GetArgumentFloat(node, info, "time", float.NaN);
			if (_type == -1)
			{
				Single = false;
				Lasting = false;
			}
			else
			{
				Single = true;
				Lasting = true;
			}
		}

		public override void InitCommand()
		{
			base.InitCommand();
			ScriptCommand.GalCore.GameState.CurrentGameState.Update(ScriptCommand.GalCore.CurrentCommandList);
			if (ScriptCommand.GalCore.GameScript.CurrentGameScript.NewChapter && ScriptCommand.GalCore.GameState.CurrentGameState.CanSave)
			{
				if ((ScriptCommand.GalCore.GameOptions.QuickSave & 2) == 2 && QuickSaveChapter)
				{
					ScriptCommand.GalCore.GameScript.QuickSave(QuickSaveType.Title);
				}
				ScriptCommand.GalCore.GameScript.CurrentGameScript.NewChapter = false;
			}
			if (_type == -1)
			{
				FinishCommand();
				return;
			}
			if (ScriptCommand.GalCore.ToNormal)
			{
				ScriptCommand.GalCore.CurrentPlayMode = PlayMode.Normal;
				ScriptCommand.GalCore.ToNormal = false;
			}
			if (float.IsNaN(_time))
			{
				switch (ScriptCommand.PlayMode)
				{
				case PlayMode.Normal:
					ScriptCommand.GalCore.CurrentPlayState = PlayState.ToClick;
					break;
				case PlayMode.Auto:
					lastTime = GetAutoTime();
					break;
				case PlayMode.QuickOnce:
					ScriptCommand.GalCore.CurrentPlayMode = PlayMode.Normal;
					ScriptCommand.GalCore.CurrentPlayState = PlayState.ToClick;
					break;
				case PlayMode.Quick:
				case PlayMode.QuickRead:
				case PlayMode.QuickAll:
					OnItemClick();
					break;
				}
			}
			else
			{
				foreach (ScriptCommand command in ScriptCommand.CommandList)
				{
					if (command is Timer && command != this)
					{
						timer = (Timer)command;
						break;
					}
				}
				if (timer == null)
				{
					throw new RuntimeException(Node, Info, "No timer is running.");
				}
			}
			for (int commandIndex = ScriptCommand.CommandList.Count - 1; commandIndex >= 0; commandIndex--)
			{
				if (ScriptCommand.GalCore.CurrentCommandList[commandIndex] is Dialogue)
				{
					_clickPosition = Dialogue.String.Click;
					break;
				}
				if (ScriptCommand.GalCore.CurrentCommandList[commandIndex] is Paragraph)
				{
					_clickPosition = Paragraph.ShownString.Click;
					break;
				}
			}
			_toClickState = true;
		}

		public override void FinalCommand()
		{
			base.FinalCommand();
			_toClickState = false;
		}

		protected abstract float GetAutoTime();

		public bool OnItemClick()
		{
			if ((_type & 1) == 0)
			{
				foreach (ScriptCommand currentCommand in ScriptCommand.GalCore.CurrentCommandList)
				{
					if (currentCommand is Dialogue)
					{
						((Dialogue)currentCommand).FinishString(false);
					}
				}
			}
			FinishCommand();
			bool shouldAdvance = true;
			foreach (ScriptCommand currentCommand2 in ScriptCommand.GalCore.CurrentCommandList)
			{
				if (currentCommand2 is IClickable && currentCommand2 != this && ((IClickable)currentCommand2).ToClickState)
				{
					shouldAdvance = false;
					break;
				}
			}
			if (shouldAdvance)
			{
				ScriptCommand.GalCore.CurrentPlayState = PlayState.Running;
			}
			return true;
		}

		public void OnItemMove(Point location)
		{
		}

		public virtual void Draw()
		{
		}

		public abstract void InitStaticData();

		public abstract void FinalStaticData();

		public virtual void Update(double appTime, float elaspedTime)
		{
			if (float.IsNaN(_time))
			{
				switch (ScriptCommand.PlayMode)
				{
				case PlayMode.Normal:
					ScriptCommand.GalCore.CurrentPlayState = PlayState.ToClick;
					break;
				case PlayMode.Auto:
					if (!ScriptCommand.Sound.IsPlayingVoice)
					{
						lastTime -= elaspedTime;
						if (lastTime <= 0f)
						{
							OnItemClick();
						}
					}
					break;
				case PlayMode.QuickOnce:
				case PlayMode.Quick:
				case PlayMode.QuickRead:
				case PlayMode.QuickAll:
					OnItemClick();
					break;
				}
			}
			else if (timer.Time >= _time)
			{
				OnItemClick();
			}
			for (int commandIndex = ScriptCommand.CommandList.Count - 1; commandIndex >= 0; commandIndex--)
			{
				if (ScriptCommand.GalCore.CurrentCommandList[commandIndex] is Dialogue)
				{
					_clickPosition = Dialogue.String.Click;
					break;
				}
				if (ScriptCommand.GalCore.CurrentCommandList[commandIndex] is Paragraph)
				{
					_clickPosition = Paragraph.ShownString.Click;
					break;
				}
			}
		}
	}
}
