using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Resources;
using System.Threading;
using System.Xml;

namespace KidFansChannel.GalCore
{
	public class GameLanguage
	{
		private List<string> languages = new List<string>();

		private int currentIndex = -1;

		private string _currentLanguage;

		private GalCore galCore;

		private ResourceManager _resourceManager;

		public string CurrentLanguage
		{
			get
			{
				return _currentLanguage;
			}
		}

		public int LanguageNum
		{
			get
			{
				return languages.Count;
			}
		}

		public ResourceManager ResourceManager
		{
			get
			{
				return _resourceManager;
			}
		}

		public event EventHandler<LanguageEventArgs> LanguageChanged;

		public void SetCore(GalCore gc)
		{
			galCore = gc;
		}

		public string GetLanguageFullName(int index)
		{
			if (index >= languages.Count)
			{
				throw new IndexOutOfRangeException();
			}
			return new CultureInfo(languages[index]).NativeName;
		}

		public string GetLanguageName(int index)
		{
			if (index >= languages.Count)
			{
				throw new IndexOutOfRangeException();
			}
			return languages[index];
		}

		public void LoadLanguages()
		{
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.Load("language.xml");
			XmlNode documentElement = xmlDocument.DocumentElement;
			if (documentElement.Name != "language")
			{
				throw new InvalidDataException();
			}
			languages.Clear();
			foreach (XmlNode childNode in documentElement.ChildNodes)
			{
				if (childNode.Name != "item" || !childNode.HasChildNodes || childNode.FirstChild.Name != "name" || !childNode.FirstChild.HasChildNodes || childNode.FirstChild.ChildNodes.Count != 1 || childNode.FirstChild.FirstChild.NodeType != XmlNodeType.Text)
				{
					throw new InvalidDataException();
				}
				languages.Add(childNode.FirstChild.FirstChild.Value);
			}
			if (galCore.GameOptions == null || galCore.GameOptions.Language != ChangeLanguage(galCore.GameOptions.Language))
			{
				ChangeLanguage(0);
			}
		}

		public void SetResourceManager(ResourceManager rm)
		{
			_resourceManager = rm;
		}

		public void ChangeLanguage(int index)
		{
			if (index != currentIndex)
			{
				currentIndex = index;
				_currentLanguage = languages[index];
				CultureInfo currentUICulture = new CultureInfo(_currentLanguage);
				Thread.CurrentThread.CurrentUICulture = currentUICulture;
				if (galCore != null && galCore.GameOptions != null)
				{
					galCore.GameOptions.Language = _currentLanguage;
				}
				if (LanguageChanged != null)
				{
					LanguageChanged(this, new LanguageEventArgs(languages[index]));
				}
				galCore.GameData.LanguageExt = _currentLanguage;
			}
		}

		public string ChangeLanguage(string language)
		{
			for (int i = 0; i < languages.Count; i++)
			{
				if (languages[i] == language)
				{
					ChangeLanguage(i);
					return language;
				}
			}
			return _currentLanguage;
		}
	}
}
