using System;
using System.Drawing;
using System.Windows.Forms;

namespace KidFansChannel.GalCore.Screens
{
	public abstract class MessageBoxScreen : GameScreen
	{
		public delegate void MessageBoxCallback(object[] args);

		private static string _buttonOK = "OK";

		private static string _buttonYes = "Yes";

		private static string _buttonNo = "No";

		private string _text;

		private MessageBoxButtons _buttons;

		private MessageBoxCallback _callBack;

		private object[] _callBackArgs;

		private String2D strOK;

		private String2D strCancel;

		private String2D strText;

		protected Element eleScreen;

		protected Element[] eleButtons;

		protected Element eleText;

		private bool _pauseBack;

		public static string ButtonOK
		{
			get
			{
				return _buttonOK;
			}
			set
			{
				_buttonOK = value;
			}
		}

		public static string ButtonYes
		{
			get
			{
				return _buttonYes;
			}
			set
			{
				_buttonYes = value;
			}
		}

		public static string ButtonNo
		{
			get
			{
				return _buttonNo;
			}
			set
			{
				_buttonNo = value;
			}
		}

		public string Text
		{
			get
			{
				return _text;
			}
		}

		public MessageBoxButtons Buttons
		{
			get
			{
				return _buttons;
			}
		}

		public MessageBoxCallback CallBack
		{
			get
			{
				return _callBack;
			}
		}

		public object[] CallBackArgs
		{
			get
			{
				return _callBackArgs;
			}
		}

		public bool PauseBack
		{
			get
			{
				return _pauseBack;
			}
			set
			{
				_pauseBack = value;
			}
		}

		public MessageBoxScreen(string text, MessageBoxButtons buttons, bool pauseBack, MessageBoxCallback callBack, object[] callBackArgs)
		{
			_text = text;
			if (buttons != MessageBoxButtons.OK && buttons != MessageBoxButtons.YesNo)
			{
				throw new ArgumentException();
			}
			_buttons = buttons;
			_pauseBack = pauseBack;
			_callBack = callBack;
			_callBackArgs = callBackArgs;
		}

		public MessageBoxScreen(string text, MessageBoxButtons buttons, MessageBoxCallback callBack, object[] callBackArgs)
			: this(text, buttons, false, callBack, callBackArgs)
		{
		}

		protected virtual void OnOK()
		{
			FinalScreen();
			_callBack(_callBackArgs);
		}

		protected virtual void OnCancel()
		{
			FinalScreen();
		}

		public override void InitData()
		{
			base.InitData();
			if (_buttons == MessageBoxButtons.OK)
			{
				strOK = new String2D();
				SetButtonFontStyle(strOK);
				strOK.AddString(_buttonOK, ContentAlignment.TopCenter);
			}
			else
			{
				strOK = new String2D();
				SetButtonFontStyle(strOK);
				strOK.AddString(_buttonYes, ContentAlignment.TopCenter);
				strCancel = new String2D();
				SetButtonFontStyle(strCancel);
				strCancel.AddString(_buttonNo, ContentAlignment.TopCenter);
			}
			strText = new String2D();
			SetTextFontStyle(strText);
			strText.AddString(_text, ContentAlignment.TopCenter);
		}

		protected abstract void SetButtonFontStyle(String2D button);

		protected abstract void SetTextFontStyle(String2D text);

		public override bool InitScreen()
		{
			if (!base.InitScreen())
			{
				return false;
			}
			eleScreen = new Element();
			eleScreen.Focused = true;
			FocusedElement = eleScreen;
			RootElement = eleScreen;
			eleText = new Element(strText);
			eleScreen.Add(eleText, "text");
			eleButtons = new Element[(_buttons == MessageBoxButtons.OK) ? 1 : 2];
			eleButtons[0] = new Element();
			Element item = new Element(strOK);
			eleButtons[0].Add(item, "text");
			eleButtons[0].MouseClick += OK_MouseClick;
			eleScreen.Add(eleButtons[0], "ok");
			if (_buttons == MessageBoxButtons.OK)
			{
				Element element = eleScreen;
				Element element2 = eleScreen;
				Element element3 = eleScreen;
				Element element4 = (eleScreen.ChildDown = eleButtons[0]);
				Element element6 = (element3.ChildUp = element4);
				Element childLeft = (element2.ChildRight = element6);
				element.ChildLeft = childLeft;
			}
			else
			{
				eleButtons[1] = new Element();
				Element item2 = new Element(strCancel);
				eleButtons[1].Add(item2, "text");
				eleButtons[1].MouseClick += Cancel_MouseClick;
				eleScreen.Add(eleButtons[1], "cancel");
				for (int i = 0; i < 2; i++)
				{
					Element obj = eleButtons[i];
					Element obj2 = eleButtons[i];
					Element obj3 = eleButtons[i];
					Element element9 = (eleButtons[i].Down = eleButtons[1 - i]);
					Element element11 = (obj3.Up = element9);
					Element left = (obj2.Right = element11);
					obj.Left = left;
				}
				Element element14 = eleScreen;
				Element childLeft2 = (eleScreen.ChildUp = eleButtons[0]);
				element14.ChildLeft = childLeft2;
				Element element16 = eleScreen;
				Element childRight = (eleScreen.ChildDown = eleButtons[1]);
				element16.ChildRight = childRight;
			}
			return true;
		}

		private void OK_MouseClick(object sender, EventArgs e)
		{
			OnOK();
		}

		private void Cancel_MouseClick(object sender, EventArgs e)
		{
			OnCancel();
		}

		public override bool FinalScreen()
		{
			if (!base.FinalScreen())
			{
				return false;
			}
			for (int i = 0; i < eleScreen["ok"].Count; i++)
			{
				eleScreen["ok"][i].Dispose();
			}
			if (_buttons == MessageBoxButtons.YesNo)
			{
				for (int j = 0; j < eleScreen["cancel"].Count; j++)
				{
					eleScreen["cancel"][j].Dispose();
				}
			}
			for (int k = 0; k < eleScreen.Count; k++)
			{
				eleScreen[k].Dispose();
			}
			eleScreen.Dispose();
			eleScreen = null;
			FinalData();
			Finished = true;
			FocusedElement = null;
			RootElement = null;
			return true;
		}

		public override void OnKeyPress(Keys keyData)
		{
			if (keyData != Keys.Space)
			{
				base.OnKeyPress(keyData);
			}
		}
	}
}
