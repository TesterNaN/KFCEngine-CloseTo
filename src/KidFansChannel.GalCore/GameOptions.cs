using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Xml;

namespace KidFansChannel.GalCore
{
	public class GameOptions
	{
		public enum Level
		{
			High,
			Mid,
			Low,
			None
		}

		public enum ShockMethod
		{
			None,
			Gamepad,
			Window,
			GamepadAndWindow
		}

		private int _graphicsDevice;

		private int _soundDevice;

		private int _inputDevice;

		private bool _xInput;

		private Resolution _windowResolution = new Resolution(new Size(800, 600), new Size(4, 3), new Rectangle(0, 0, 800, 600), 1f, Rectangle.Empty, new Rectangle(0, 0, 800, 40));

		private Resolution _fullscreenResolution = Resolution.Empty;

		private bool _fullScreen = true;

		private bool _vsync;

		private Level _graphics;

		private string _dialogueColor = "Blue";

		private float _dialogueAlpha = 0.75f;

		private float _systemVolume = 0.5f;

		private float _bgmVolume = 0.5f;

		private float _seVolume = 0.5f;

		private float _voiceVolume = 0.75f;

		private bool _voiceHighlight = true;

		private bool _textSynchro = true;

		private Level _textSpeed = Level.Mid;

		private Level _autoSpeed = Level.Mid;

		private bool _quickReadOnly = true;

		private bool _selectedShow = true;

		private bool _titleShow = true;

		private bool _subtitleShow = true;

		private bool _hintShow = true;

		private int _quickSave = 3;

		private int _encyOrder;

		private int _quickSaveIndex = -1;

		private ShockMethod _shock = ShockMethod.GamepadAndWindow;

		private GamePadMap _gamePadMap = new GamePadMap(true);

		private string _language = "en-US";

		private Dictionary<string, string> _font = new Dictionary<string, string>();

		private string _settingFolder;

		public int GraphicsDevice
		{
			get
			{
				return _graphicsDevice;
			}
			set
			{
				_graphicsDevice = value;
			}
		}

		public int SoundDevice
		{
			get
			{
				return _soundDevice;
			}
			set
			{
				_soundDevice = value;
			}
		}

		public int InputDevice
		{
			get
			{
				return _inputDevice;
			}
			set
			{
				_inputDevice = value;
			}
		}

		public bool XInput
		{
			get
			{
				return _xInput;
			}
			set
			{
				_xInput = value;
			}
		}

		public Resolution WindowResolution
		{
			get
			{
				return _windowResolution;
			}
			set
			{
				_windowResolution = value;
			}
		}

		public Resolution FullscreenResolution
		{
			get
			{
				return _fullscreenResolution;
			}
			set
			{
				_fullscreenResolution = value;
			}
		}

		public bool FullScreen
		{
			get
			{
				return _fullScreen;
			}
			set
			{
				_fullScreen = value;
			}
		}

		public bool VSync
		{
			get
			{
				return _vsync;
			}
			set
			{
				_vsync = value;
			}
		}

		public Level Graphics
		{
			get
			{
				return _graphics;
			}
			set
			{
				if (value == Level.None)
				{
					throw new ArgumentOutOfRangeException();
				}
				_graphics = value;
				if (OptionsUpdate != null)
				{
					OptionsUpdate(this, EventArgs.Empty);
				}
			}
		}

		public string DialogueColor
		{
			get
			{
				return _dialogueColor;
			}
			set
			{
				_dialogueColor = value;
				if (OptionsUpdate != null)
				{
					OptionsUpdate(this, EventArgs.Empty);
				}
			}
		}

		public float DialogueAlpha
		{
			get
			{
				return _dialogueAlpha;
			}
			set
			{
				if (value < 0f || value > 1f)
				{
					throw new ArgumentOutOfRangeException();
				}
				_dialogueAlpha = value;
				if (OptionsUpdate != null)
				{
					OptionsUpdate(this, EventArgs.Empty);
				}
			}
		}

		public float SystemVolume
		{
			get
			{
				return _systemVolume;
			}
			set
			{
				if (value < 0f || value > 1f)
				{
					throw new ArgumentOutOfRangeException();
				}
				_systemVolume = value;
				if (OptionsUpdate != null)
				{
					OptionsUpdate(this, EventArgs.Empty);
				}
			}
		}

		public float BgmVolume
		{
			get
			{
				return _bgmVolume;
			}
			set
			{
				if (value < 0f || value > 1f)
				{
					throw new ArgumentOutOfRangeException();
				}
				_bgmVolume = value;
				if (OptionsUpdate != null)
				{
					OptionsUpdate(this, EventArgs.Empty);
				}
			}
		}

		public float SeVolume
		{
			get
			{
				return _seVolume;
			}
			set
			{
				if (value < 0f || value > 1f)
				{
					throw new ArgumentOutOfRangeException();
				}
				_seVolume = value;
				if (OptionsUpdate != null)
				{
					OptionsUpdate(this, EventArgs.Empty);
				}
			}
		}

		public float VoiceVolume
		{
			get
			{
				return _voiceVolume;
			}
			set
			{
				if (value < 0f || value > 1f)
				{
					throw new ArgumentOutOfRangeException();
				}
				_voiceVolume = value;
				if (OptionsUpdate != null)
				{
					OptionsUpdate(this, EventArgs.Empty);
				}
			}
		}

		public bool VoiceHighlight
		{
			get
			{
				return _voiceHighlight;
			}
			set
			{
				_voiceHighlight = value;
				if (OptionsUpdate != null)
				{
					OptionsUpdate(this, EventArgs.Empty);
				}
			}
		}

		public bool TextSynchro
		{
			get
			{
				return _textSynchro;
			}
			set
			{
				_textSynchro = value;
				if (OptionsUpdate != null)
				{
					OptionsUpdate(this, EventArgs.Empty);
				}
			}
		}

		public Level TextSpeed
		{
			get
			{
				return _textSpeed;
			}
			set
			{
				_textSpeed = value;
				if (OptionsUpdate != null)
				{
					OptionsUpdate(this, EventArgs.Empty);
				}
			}
		}

		public Level AutoSpeed
		{
			get
			{
				return _autoSpeed;
			}
			set
			{
				_autoSpeed = value;
				if (OptionsUpdate != null)
				{
					OptionsUpdate(this, EventArgs.Empty);
				}
			}
		}

		public bool QuickReadOnly
		{
			get
			{
				return _quickReadOnly;
			}
			set
			{
				_quickReadOnly = value;
				if (OptionsUpdate != null)
				{
					OptionsUpdate(this, EventArgs.Empty);
				}
			}
		}

		public bool SelectedShow
		{
			get
			{
				return _selectedShow;
			}
			set
			{
				_selectedShow = value;
				if (OptionsUpdate != null)
				{
					OptionsUpdate(this, EventArgs.Empty);
				}
			}
		}

		public bool TitleShow
		{
			get
			{
				return _titleShow;
			}
			set
			{
				_titleShow = value;
				if (OptionsUpdate != null)
				{
					OptionsUpdate(this, EventArgs.Empty);
				}
			}
		}

		public bool SubtitleShow
		{
			get
			{
				return _subtitleShow;
			}
			set
			{
				_subtitleShow = value;
			}
		}

		public bool HintShow
		{
			get
			{
				return _hintShow;
			}
			set
			{
				_hintShow = value;
			}
		}

		public int QuickSave
		{
			get
			{
				return _quickSave;
			}
			set
			{
				_quickSave = value;
				if (OptionsUpdate != null)
				{
					OptionsUpdate(this, EventArgs.Empty);
				}
			}
		}

		public int EncyOrder
		{
			get
			{
				return _encyOrder;
			}
			set
			{
				_encyOrder = value;
			}
		}

		public int QuickSaveIndex
		{
			get
			{
				return _quickSaveIndex;
			}
			set
			{
				_quickSaveIndex = value;
			}
		}

		public ShockMethod Shock
		{
			get
			{
				return _shock;
			}
			set
			{
				_shock = value;
				if (OptionsUpdate != null)
				{
					OptionsUpdate(this, EventArgs.Empty);
				}
			}
		}

		public GamePadMap GamePadMap
		{
			get
			{
				return _gamePadMap;
			}
			set
			{
				_gamePadMap = value;
			}
		}

		public string Language
		{
			get
			{
				return _language;
			}
			set
			{
				_language = value;
				if (OptionsUpdate != null)
				{
					OptionsUpdate(this, EventArgs.Empty);
				}
			}
		}

		public string Font
		{
			get
			{
				if (_font.ContainsKey(_language))
				{
					return _font[_language];
				}
				_font.Add(_language, "@default");
				return "@default";
			}
			set
			{
				if (_font.ContainsKey(_language))
				{
					_font[_language] = value;
				}
				else
				{
					_font.Add(_language, value);
				}
				if (OptionsUpdate != null)
				{
					OptionsUpdate(this, EventArgs.Empty);
				}
			}
		}

		public string SettingFolder
		{
			get
			{
				return _settingFolder;
			}
			set
			{
				_settingFolder = value;
			}
		}

		public event EventHandler OptionsUpdate;

		~GameOptions()
		{
			_font.Clear();
		}

		public string GetOptionsFile()
		{
			if (!Directory.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), _settingFolder)))
			{
				try
				{
					Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), _settingFolder));
				}
				catch (Exception ex)
				{
					MessageBox.Show("An Error occured while creating options directory.\n" + ex.Message);
				}
			}
			return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), _settingFolder + "\\Options.xml");
		}

		protected void SaveSetting(XmlDocument doc, string name, object value)
		{
			if (doc == null)
			{
				throw new ArgumentNullException("doc");
			}
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentNullException("name");
			}
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (doc.DocumentElement.Name != "Options")
			{
				throw new InvalidDataException();
			}
			string[] pathSegments = name.Split('/');
			XmlNode xmlNode = doc.DocumentElement;
			for (int i = 0; i < pathSegments.Length; i++)
			{
				XmlNode xmlNode2 = xmlNode.SelectSingleNode(pathSegments[i]);
				xmlNode = ((xmlNode2 != null) ? xmlNode2 : xmlNode.AppendChild(doc.CreateElement(pathSegments[i])));
			}
			xmlNode.InnerText = value.ToString();
		}

		public virtual void Save()
		{
			try
			{
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.AppendChild(xmlDocument.CreateElement("Options"));
				SaveSetting(xmlDocument, "GraphicsDevice", GraphicsDevice);
				SaveSetting(xmlDocument, "SoundDevice", SoundDevice);
				SaveSetting(xmlDocument, "InputDevice", InputDevice);
				SaveSetting(xmlDocument, "XInput", XInput);
				SaveSetting(xmlDocument, "WindowResolution/Size/Width", WindowResolution.Size.Width);
				SaveSetting(xmlDocument, "WindowResolution/Size/Height", WindowResolution.Size.Height);
				SaveSetting(xmlDocument, "WindowResolution/Proportion/Width", WindowResolution.Proportion.Width);
				SaveSetting(xmlDocument, "WindowResolution/Proportion/Height", WindowResolution.Proportion.Height);
				SaveSetting(xmlDocument, "WindowResolution/Bounds/X", WindowResolution.Bounds.X);
				SaveSetting(xmlDocument, "WindowResolution/Bounds/Y", WindowResolution.Bounds.Y);
				SaveSetting(xmlDocument, "WindowResolution/Bounds/Width", WindowResolution.Bounds.Width);
				SaveSetting(xmlDocument, "WindowResolution/Bounds/Height", WindowResolution.Bounds.Height);
				SaveSetting(xmlDocument, "WindowResolution/FontScale", WindowResolution.FontScale);
				SaveSetting(xmlDocument, "WindowResolution/StateArea/X", WindowResolution.StateArea.X);
				SaveSetting(xmlDocument, "WindowResolution/StateArea/Y", WindowResolution.StateArea.Y);
				SaveSetting(xmlDocument, "WindowResolution/StateArea/Width", WindowResolution.StateArea.Width);
				SaveSetting(xmlDocument, "WindowResolution/StateArea/Height", WindowResolution.StateArea.Height);
				SaveSetting(xmlDocument, "WindowResolution/CommentArea/X", WindowResolution.CommentArea.X);
				SaveSetting(xmlDocument, "WindowResolution/CommentArea/Y", WindowResolution.CommentArea.Y);
				SaveSetting(xmlDocument, "WindowResolution/CommentArea/Width", WindowResolution.CommentArea.Width);
				SaveSetting(xmlDocument, "WindowResolution/CommentArea/Height", WindowResolution.CommentArea.Height);
				SaveSetting(xmlDocument, "FullscreenResolution/Size/Width", FullscreenResolution.Size.Width);
				SaveSetting(xmlDocument, "FullscreenResolution/Size/Height", FullscreenResolution.Size.Height);
				SaveSetting(xmlDocument, "FullscreenResolution/Proportion/Width", FullscreenResolution.Proportion.Width);
				SaveSetting(xmlDocument, "FullscreenResolution/Proportion/Height", FullscreenResolution.Proportion.Height);
				SaveSetting(xmlDocument, "FullscreenResolution/Bounds/X", FullscreenResolution.Bounds.X);
				SaveSetting(xmlDocument, "FullscreenResolution/Bounds/Y", FullscreenResolution.Bounds.Y);
				SaveSetting(xmlDocument, "FullscreenResolution/Bounds/Width", FullscreenResolution.Bounds.Width);
				SaveSetting(xmlDocument, "FullscreenResolution/Bounds/Height", FullscreenResolution.Bounds.Height);
				SaveSetting(xmlDocument, "FullscreenResolution/FontScale", FullscreenResolution.FontScale);
				SaveSetting(xmlDocument, "FullscreenResolution/StateArea/X", FullscreenResolution.StateArea.X);
				SaveSetting(xmlDocument, "FullscreenResolution/StateArea/Y", FullscreenResolution.StateArea.Y);
				SaveSetting(xmlDocument, "FullscreenResolution/StateArea/Width", FullscreenResolution.StateArea.Width);
				SaveSetting(xmlDocument, "FullscreenResolution/StateArea/Height", FullscreenResolution.StateArea.Height);
				SaveSetting(xmlDocument, "FullscreenResolution/CommentArea/X", FullscreenResolution.CommentArea.X);
				SaveSetting(xmlDocument, "FullscreenResolution/CommentArea/Y", FullscreenResolution.CommentArea.Y);
				SaveSetting(xmlDocument, "FullscreenResolution/CommentArea/Width", FullscreenResolution.CommentArea.Width);
				SaveSetting(xmlDocument, "FullscreenResolution/CommentArea/Height", FullscreenResolution.CommentArea.Height);
				SaveSetting(xmlDocument, "FullScreen", FullScreen);
				SaveSetting(xmlDocument, "VSync", VSync);
				SaveSetting(xmlDocument, "Graphics", Graphics);
				SaveSetting(xmlDocument, "DialogueColor", DialogueColor);
				SaveSetting(xmlDocument, "DialogueAlpha", DialogueAlpha);
				SaveSetting(xmlDocument, "SystemVolume", SystemVolume);
				SaveSetting(xmlDocument, "BgmVolume", BgmVolume);
				SaveSetting(xmlDocument, "SeVolume", SeVolume);
				SaveSetting(xmlDocument, "VoiceVolume", VoiceVolume);
				SaveSetting(xmlDocument, "VoiceHighlight", VoiceHighlight);
				SaveSetting(xmlDocument, "TextSynchro", TextSynchro);
				SaveSetting(xmlDocument, "TextSpeed", TextSpeed);
				SaveSetting(xmlDocument, "AutoSpeed", AutoSpeed);
				SaveSetting(xmlDocument, "QuickReadOnly", QuickReadOnly);
				SaveSetting(xmlDocument, "SelectedShow", SelectedShow);
				SaveSetting(xmlDocument, "TitleShow", TitleShow);
				SaveSetting(xmlDocument, "HintShow", HintShow);
				SaveSetting(xmlDocument, "SubtitleShow", SubtitleShow);
				SaveSetting(xmlDocument, "QuickSave", QuickSave);
				SaveSetting(xmlDocument, "EncyOrder", EncyOrder);
				SaveSetting(xmlDocument, "QuickSaveIndex", QuickSaveIndex);
				SaveSetting(xmlDocument, "Shock", Shock);
				SaveSetting(xmlDocument, "GamePadMap/OK", GamePadMap.OK);
				SaveSetting(xmlDocument, "GamePadMap/Cancel", GamePadMap.Cancel);
				SaveSetting(xmlDocument, "GamePadMap/SkipRead", GamePadMap.SkipRead);
				SaveSetting(xmlDocument, "GamePadMap/SkipAll", GamePadMap.SkipAll);
				SaveSetting(xmlDocument, "GamePadMap/Auto", GamePadMap.Auto);
				SaveSetting(xmlDocument, "GamePadMap/Quick", GamePadMap.Quick);
				SaveSetting(xmlDocument, "GamePadMap/Hide", GamePadMap.Hide);
				SaveSetting(xmlDocument, "GamePadMap/Message", GamePadMap.Message);
				SaveSetting(xmlDocument, "GamePadMap/PageUp", GamePadMap.PageUp);
				SaveSetting(xmlDocument, "GamePadMap/PageDown", GamePadMap.PageDown);
				SaveSetting(xmlDocument, "Language", Language);
				if (!_font.ContainsKey(Language))
				{
					_font.Add(Language, "@default");
				}
				foreach (KeyValuePair<string, string> item in _font)
				{
					SaveSetting(xmlDocument, "Font/" + item.Key, item.Value);
				}
				xmlDocument.Save(GetOptionsFile());
			}
			catch (Exception ex)
			{
				MessageBox.Show("An Error occured while saving options.\n" + ex.Message);
			}
		}

		protected object LoadSetting(XmlDocument doc, string name, object defaultValue)
		{
			if (doc == null)
			{
				throw new ArgumentNullException("doc");
			}
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentNullException("name");
			}
			if (defaultValue == null)
			{
				throw new ArgumentNullException("defaultValue");
			}
			XmlNode xmlNode = doc.SelectSingleNode("Options/" + name);
			if (xmlNode == null)
			{
				return defaultValue;
			}
			string innerText = xmlNode.InnerText;
			if (defaultValue is string)
			{
				return innerText;
			}
			if (defaultValue is int)
			{
				int result;
				if (int.TryParse(innerText, out result))
				{
					return result;
				}
				return defaultValue;
			}
			if (defaultValue is float)
			{
				float result2;
				if (float.TryParse(innerText, out result2))
				{
					return result2;
				}
				return defaultValue;
			}
			if (defaultValue is bool)
			{
				bool result3;
				if (bool.TryParse(innerText, out result3))
				{
					return result3;
				}
				return defaultValue;
			}
			if (defaultValue is Enum)
			{
				try
				{
					return Enum.Parse(defaultValue.GetType(), innerText);
				}
				catch (ArgumentException)
				{
					return defaultValue;
				}
			}
			return defaultValue;
		}

		public void Load()
		{
			try
			{
				if (!File.Exists(GetOptionsFile()))
				{
					return;
				}
				XmlDocument xmlDocument = new XmlDocument();
				try
				{
					xmlDocument.Load(GetOptionsFile());
				}
				catch
				{
					return;
				}
				try
				{
					GraphicsDevice = (int)LoadSetting(xmlDocument, "GraphicsDevice", GraphicsDevice);
				}
				catch
				{
				}
				try
				{
					SoundDevice = (int)LoadSetting(xmlDocument, "SoundDevice", SoundDevice);
				}
				catch
				{
				}
				try
				{
					InputDevice = (int)LoadSetting(xmlDocument, "InputDevice", InputDevice);
				}
				catch
				{
				}
				try
				{
					XInput = (bool)LoadSetting(xmlDocument, "XInput", XInput);
				}
				catch
				{
				}
				Resolution resolution = default(Resolution);
				try
				{
					resolution.Size.Width = (int)LoadSetting(xmlDocument, "WindowResolution/Size/Width", WindowResolution.Size.Width);
				}
				catch
				{
				}
				try
				{
					resolution.Size.Height = (int)LoadSetting(xmlDocument, "WindowResolution/Size/Height", WindowResolution.Size.Height);
				}
				catch
				{
				}
				try
				{
					resolution.Proportion.Width = (int)LoadSetting(xmlDocument, "WindowResolution/Proportion/Width", WindowResolution.Proportion.Width);
				}
				catch
				{
				}
				try
				{
					resolution.Proportion.Height = (int)LoadSetting(xmlDocument, "WindowResolution/Proportion/Height", WindowResolution.Proportion.Height);
				}
				catch
				{
				}
				try
				{
					resolution.Bounds.X = (int)LoadSetting(xmlDocument, "WindowResolution/Bounds/X", WindowResolution.Bounds.X);
				}
				catch
				{
				}
				try
				{
					resolution.Bounds.Y = (int)LoadSetting(xmlDocument, "WindowResolution/Bounds/Y", WindowResolution.Bounds.Y);
				}
				catch
				{
				}
				try
				{
					resolution.Bounds.Width = (int)LoadSetting(xmlDocument, "WindowResolution/Bounds/Width", WindowResolution.Bounds.Width);
				}
				catch
				{
				}
				try
				{
					resolution.Bounds.Height = (int)LoadSetting(xmlDocument, "WindowResolution/Bounds/Height", WindowResolution.Bounds.Height);
				}
				catch
				{
				}
				try
				{
					resolution.FontScale = (float)LoadSetting(xmlDocument, "WindowResolution/FontScale", WindowResolution.FontScale);
				}
				catch
				{
				}
				try
				{
					resolution.StateArea.X = (int)LoadSetting(xmlDocument, "WindowResolution/StateArea/X", WindowResolution.StateArea.X);
				}
				catch
				{
				}
				try
				{
					resolution.StateArea.Y = (int)LoadSetting(xmlDocument, "WindowResolution/StateArea/Y", WindowResolution.StateArea.Y);
				}
				catch
				{
				}
				try
				{
					resolution.StateArea.Width = (int)LoadSetting(xmlDocument, "WindowResolution/StateArea/Width", WindowResolution.StateArea.Width);
				}
				catch
				{
				}
				try
				{
					resolution.StateArea.Height = (int)LoadSetting(xmlDocument, "WindowResolution/StateArea/Height", WindowResolution.StateArea.Height);
				}
				catch
				{
				}
				try
				{
					resolution.CommentArea.X = (int)LoadSetting(xmlDocument, "WindowResolution/CommentArea/X", WindowResolution.CommentArea.X);
				}
				catch
				{
				}
				try
				{
					resolution.CommentArea.Y = (int)LoadSetting(xmlDocument, "WindowResolution/CommentArea/Y", WindowResolution.CommentArea.Y);
				}
				catch
				{
				}
				try
				{
					resolution.CommentArea.Width = (int)LoadSetting(xmlDocument, "WindowResolution/CommentArea/Width", WindowResolution.CommentArea.Width);
				}
				catch
				{
				}
				try
				{
					resolution.CommentArea.Height = (int)LoadSetting(xmlDocument, "WindowResolution/CommentArea/Height", WindowResolution.CommentArea.Height);
				}
				catch
				{
				}
				WindowResolution = resolution;
				try
				{
					resolution.Size.Width = (int)LoadSetting(xmlDocument, "FullscreenResolution/Size/Width", FullscreenResolution.Size.Width);
				}
				catch
				{
				}
				try
				{
					resolution.Size.Height = (int)LoadSetting(xmlDocument, "FullscreenResolution/Size/Height", FullscreenResolution.Size.Height);
				}
				catch
				{
				}
				try
				{
					resolution.Proportion.Width = (int)LoadSetting(xmlDocument, "FullscreenResolution/Proportion/Width", FullscreenResolution.Proportion.Width);
				}
				catch
				{
				}
				try
				{
					resolution.Proportion.Height = (int)LoadSetting(xmlDocument, "FullscreenResolution/Proportion/Height", FullscreenResolution.Proportion.Height);
				}
				catch
				{
				}
				try
				{
					resolution.Bounds.X = (int)LoadSetting(xmlDocument, "FullscreenResolution/Bounds/X", FullscreenResolution.Bounds.X);
				}
				catch
				{
				}
				try
				{
					resolution.Bounds.Y = (int)LoadSetting(xmlDocument, "FullscreenResolution/Bounds/Y", FullscreenResolution.Bounds.Y);
				}
				catch
				{
				}
				try
				{
					resolution.Bounds.Width = (int)LoadSetting(xmlDocument, "FullscreenResolution/Bounds/Width", FullscreenResolution.Bounds.Width);
				}
				catch
				{
				}
				try
				{
					resolution.Bounds.Height = (int)LoadSetting(xmlDocument, "FullscreenResolution/Bounds/Height", FullscreenResolution.Bounds.Height);
				}
				catch
				{
				}
				try
				{
					resolution.FontScale = (float)LoadSetting(xmlDocument, "FullscreenResolution/FontScale", FullscreenResolution.FontScale);
				}
				catch
				{
				}
				try
				{
					resolution.StateArea.X = (int)LoadSetting(xmlDocument, "FullscreenResolution/StateArea/X", FullscreenResolution.StateArea.X);
				}
				catch
				{
				}
				try
				{
					resolution.StateArea.Y = (int)LoadSetting(xmlDocument, "FullscreenResolution/StateArea/Y", FullscreenResolution.StateArea.Y);
				}
				catch
				{
				}
				try
				{
					resolution.StateArea.Width = (int)LoadSetting(xmlDocument, "FullscreenResolution/StateArea/Width", FullscreenResolution.StateArea.Width);
				}
				catch
				{
				}
				try
				{
					resolution.StateArea.Height = (int)LoadSetting(xmlDocument, "FullscreenResolution/StateArea/Height", FullscreenResolution.StateArea.Height);
				}
				catch
				{
				}
				try
				{
					resolution.CommentArea.X = (int)LoadSetting(xmlDocument, "FullscreenResolution/CommentArea/X", FullscreenResolution.CommentArea.X);
				}
				catch
				{
				}
				try
				{
					resolution.CommentArea.Y = (int)LoadSetting(xmlDocument, "FullscreenResolution/CommentArea/Y", FullscreenResolution.CommentArea.Y);
				}
				catch
				{
				}
				try
				{
					resolution.CommentArea.Width = (int)LoadSetting(xmlDocument, "FullscreenResolution/CommentArea/Width", FullscreenResolution.CommentArea.Width);
				}
				catch
				{
				}
				try
				{
					resolution.CommentArea.Height = (int)LoadSetting(xmlDocument, "FullscreenResolution/CommentArea/Height", FullscreenResolution.CommentArea.Height);
				}
				catch
				{
				}
				FullscreenResolution = resolution;
				try
				{
					FullScreen = (bool)LoadSetting(xmlDocument, "FullScreen", FullScreen);
				}
				catch
				{
				}
				try
				{
					VSync = (bool)LoadSetting(xmlDocument, "VSync", VSync);
				}
				catch
				{
				}
				try
				{
					Graphics = (Level)LoadSetting(xmlDocument, "Graphics", Graphics);
				}
				catch
				{
				}
				try
				{
					DialogueColor = (string)LoadSetting(xmlDocument, "DialogueColor", DialogueColor);
				}
				catch
				{
				}
				try
				{
					DialogueAlpha = (float)LoadSetting(xmlDocument, "DialogueAlpha", DialogueAlpha);
				}
				catch
				{
				}
				try
				{
					SystemVolume = (float)LoadSetting(xmlDocument, "SystemVolume", SystemVolume);
				}
				catch
				{
				}
				try
				{
					BgmVolume = (float)LoadSetting(xmlDocument, "BgmVolume", BgmVolume);
				}
				catch
				{
				}
				try
				{
					SeVolume = (float)LoadSetting(xmlDocument, "SeVolume", SeVolume);
				}
				catch
				{
				}
				try
				{
					VoiceVolume = (float)LoadSetting(xmlDocument, "VoiceVolume", VoiceVolume);
				}
				catch
				{
				}
				try
				{
					VoiceHighlight = (bool)LoadSetting(xmlDocument, "VoiceHighlight", VoiceHighlight);
				}
				catch
				{
				}
				try
				{
					TextSynchro = (bool)LoadSetting(xmlDocument, "TextSynchro", TextSynchro);
				}
				catch
				{
				}
				try
				{
					TextSpeed = (Level)LoadSetting(xmlDocument, "TextSpeed", TextSpeed);
				}
				catch
				{
				}
				try
				{
					AutoSpeed = (Level)LoadSetting(xmlDocument, "AutoSpeed", AutoSpeed);
				}
				catch
				{
				}
				try
				{
					QuickReadOnly = (bool)LoadSetting(xmlDocument, "QuickReadOnly", QuickReadOnly);
				}
				catch
				{
				}
				try
				{
					SelectedShow = (bool)LoadSetting(xmlDocument, "SelectedShow", SelectedShow);
				}
				catch
				{
				}
				try
				{
					TitleShow = (bool)LoadSetting(xmlDocument, "TitleShow", TitleShow);
				}
				catch
				{
				}
				try
				{
					HintShow = (bool)LoadSetting(xmlDocument, "HintShow", HintShow);
				}
				catch
				{
				}
				try
				{
					SubtitleShow = (bool)LoadSetting(xmlDocument, "SubtitleShow", SubtitleShow);
				}
				catch
				{
				}
				try
				{
					QuickSave = (int)LoadSetting(xmlDocument, "QuickSave", QuickSave);
				}
				catch
				{
				}
				try
				{
					EncyOrder = (int)LoadSetting(xmlDocument, "EncyOrder", EncyOrder);
				}
				catch
				{
				}
				try
				{
					QuickSaveIndex = (int)LoadSetting(xmlDocument, "QuickSaveIndex", QuickSaveIndex);
				}
				catch
				{
				}
				try
				{
					Shock = (ShockMethod)LoadSetting(xmlDocument, "Shock", Shock);
				}
				catch
				{
				}
				GamePadMap gamePadMap = new GamePadMap(false);
				try
				{
					gamePadMap.OK = (int)LoadSetting(xmlDocument, "GamePadMap/OK", GamePadMap.OK);
				}
				catch
				{
				}
				try
				{
					gamePadMap.Cancel = (int)LoadSetting(xmlDocument, "GamePadMap/Cancel", GamePadMap.Cancel);
				}
				catch
				{
				}
				try
				{
					gamePadMap.SkipRead = (int)LoadSetting(xmlDocument, "GamePadMap/SkipRead", GamePadMap.SkipRead);
				}
				catch
				{
				}
				try
				{
					gamePadMap.SkipAll = (int)LoadSetting(xmlDocument, "GamePadMap/SkipAll", GamePadMap.SkipAll);
				}
				catch
				{
				}
				try
				{
					gamePadMap.Auto = (int)LoadSetting(xmlDocument, "GamePadMap/Auto", GamePadMap.Auto);
				}
				catch
				{
				}
				try
				{
					gamePadMap.Quick = (int)LoadSetting(xmlDocument, "GamePadMap/Quick", GamePadMap.Quick);
				}
				catch
				{
				}
				try
				{
					gamePadMap.Hide = (int)LoadSetting(xmlDocument, "GamePadMap/Hide", GamePadMap.Hide);
				}
				catch
				{
				}
				try
				{
					gamePadMap.Message = (int)LoadSetting(xmlDocument, "GamePadMap/Message", GamePadMap.Message);
				}
				catch
				{
				}
				try
				{
					gamePadMap.PageUp = (int)LoadSetting(xmlDocument, "GamePadMap/PageUp", GamePadMap.PageUp);
				}
				catch
				{
				}
				try
				{
					gamePadMap.PageDown = (int)LoadSetting(xmlDocument, "GamePadMap/PageDown", GamePadMap.PageDown);
				}
				catch
				{
				}
				GamePadMap = gamePadMap;
				try
				{
					Language = (string)LoadSetting(xmlDocument, "Language", Language);
				}
				catch
				{
				}
				XmlNode xmlNode = xmlDocument.SelectSingleNode("Options/Font");
				if (xmlNode != null)
				{
					foreach (XmlNode childNode in xmlNode.ChildNodes)
					{
						if (childNode.NodeType != XmlNodeType.Element)
						{
							continue;
						}
						try
						{
							string value = (string)LoadSetting(xmlDocument, "Font/" + childNode.Name, "@default");
							if (_font.ContainsKey(childNode.Name))
							{
								_font[childNode.Name] = value;
							}
							else
							{
								_font.Add(childNode.Name, value);
							}
						}
						catch
						{
						}
					}
				}
				if (!_font.ContainsKey(Language))
				{
					_font.Add(Language, "@default");
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show("An Error occured while loading options.\n" + ex.Message);
			}
		}

		protected Level ParseLevel(string value)
		{
			switch (value)
			{
			case "Low":
				return Level.Low;
			case "Mid":
				return Level.Mid;
			case "High":
				return Level.High;
			default:
				return Level.None;
			}
		}

		public void SetValue(string title, string value)
		{
			switch (title)
			{
			case "Graphics":
				Graphics = ParseLevel(value);
				break;
			case "DialogueColor":
				DialogueColor = value;
				break;
			case "DialogueAlpha":
				DialogueAlpha = float.Parse(value);
				break;
			case "SystemVolume":
				SystemVolume = float.Parse(value);
				break;
			case "BgmVolume":
				BgmVolume = float.Parse(value);
				break;
			case "SeVolume":
				SeVolume = float.Parse(value);
				break;
			case "VoiceVolume":
				VoiceVolume = float.Parse(value);
				break;
			case "VoiceHighlight":
				VoiceHighlight = bool.Parse(value);
				break;
			case "TextSynchro":
				TextSynchro = bool.Parse(value);
				break;
			case "TextSpeed":
				TextSpeed = ParseLevel(value);
				break;
			case "AutoSpeed":
				AutoSpeed = ParseLevel(value);
				break;
			case "QuickReadOnly":
				QuickReadOnly = bool.Parse(value);
				break;
			case "SelectedShow":
				SelectedShow = bool.Parse(value);
				break;
			case "TitleShow":
				TitleShow = bool.Parse(value);
				break;
			case "SubtitleShow":
				SubtitleShow = bool.Parse(value);
				break;
			case "HintShow":
				HintShow = bool.Parse(value);
				break;
			case "QuickSave":
				QuickSave = int.Parse(value);
				break;
			case "EncyOrder":
				EncyOrder = int.Parse(value);
				break;
			}
		}

		protected string ToStringLevel(Level value)
		{
			switch (value)
			{
			case Level.High:
				return "High";
			case Level.Mid:
				return "Mid";
			case Level.Low:
				return "Low";
			case Level.None:
				return "None";
			default:
				return "None";
			}
		}

		public string GetValue(string title)
		{
			switch (title)
			{
			case "Graphics":
				return Graphics.ToString();
			case "DialogueColor":
				return DialogueColor.ToString();
			case "DialogueAlpha":
				return DialogueAlpha.ToString();
			case "SystemVolume":
				return SystemVolume.ToString();
			case "BgmVolume":
				return BgmVolume.ToString();
			case "SeVolume":
				return SeVolume.ToString();
			case "VoiceVolume":
				return VoiceVolume.ToString();
			case "VoiceHighlight":
				return VoiceHighlight.ToString();
			case "TextSynchro":
				return TextSynchro.ToString();
			case "TextSpeed":
				return TextSpeed.ToString();
			case "AutoSpeed":
				return AutoSpeed.ToString();
			case "QuickReadOnly":
				return QuickReadOnly.ToString();
			case "SelectedShow":
				return SelectedShow.ToString();
			case "TitleShow":
				return TitleShow.ToString();
			case "SubtitleShow":
				return SubtitleShow.ToString();
			case "HintShow":
				return HintShow.ToString();
			case "QuickSave":
				return QuickSave.ToString();
			case "EncyOrder":
				return EncyOrder.ToString();
			default:
				return string.Empty;
			}
		}
	}
}
