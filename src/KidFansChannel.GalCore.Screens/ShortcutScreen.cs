using System;
using System.Xml;
using KidFansChannel.GalCore.Defines;

namespace KidFansChannel.GalCore.Screens
{
	public abstract class ShortcutScreen : GameScreen
	{
		protected string[] Names;

		protected string[] Storys;

		protected string[] Scripts;

		protected string[] Chapters;

		protected int[] Scenes;

		protected string[][] Conditions;

		protected string[][] Values;

		protected string[] Infos;

		protected bool[] Opens;

		public override void InitData()
		{
			base.InitData();
			XmlDocument xmlDocument = new XmlDocument();
			if (!GameManager.Data.LoadXmlDocument("system", "shortcut_info.xml", xmlDocument))
			{
				return;
			}
			int argumentInteger = ScriptHelper.GetArgumentInteger(xmlDocument.DocumentElement, new NodeInfo(), "num", 0);
			Names = new string[argumentInteger];
			Storys = new string[argumentInteger];
			Scripts = new string[argumentInteger];
			Chapters = new string[argumentInteger];
			Scenes = new int[argumentInteger];
			Conditions = new string[argumentInteger][];
			Values = new string[argumentInteger][];
			Scripts = new string[argumentInteger];
			Infos = new string[argumentInteger];
			Opens = new bool[argumentInteger];
			for (int i = 0; i < argumentInteger; i++)
			{
				XmlNode xmlNode = xmlDocument.DocumentElement.SelectSingleNode("item[@id=\"" + i + "\"]");
				if (xmlNode != null)
				{
					Storys[i] = ScriptHelper.GetArgumentString(xmlNode, new NodeInfo(), "story");
					Scripts[i] = ScriptHelper.GetArgumentString(xmlNode, new NodeInfo(), "script");
					Chapters[i] = ScriptHelper.GetArgumentString(xmlNode, new NodeInfo(), "chapter");
					Scenes[i] = ScriptHelper.GetArgumentInteger(xmlNode, new NodeInfo(), "scene", 0);
					string argumentString = ScriptHelper.GetArgumentString(xmlNode, new NodeInfo(), "condition", string.Empty);
					if (argumentString == string.Empty)
					{
						Conditions[i] = new string[0];
					}
					else
					{
						Conditions[i] = argumentString.Split(';');
					}
					string argumentString2 = ScriptHelper.GetArgumentString(xmlNode, new NodeInfo(), "value", string.Empty);
					if (argumentString2 == string.Empty)
					{
						Values[i] = new string[0];
					}
					else
					{
						Values[i] = argumentString2.Split(';');
					}
					if (Conditions[i].Length != Values[i].Length)
					{
						throw new Exception();
					}
					XmlNode xmlNode2;
					if ((xmlNode2 = xmlNode.SelectSingleNode("name")) != null && xmlNode2.HasChildNodes && xmlNode2.ChildNodes.Count == 1 && xmlNode2.FirstChild.NodeType == XmlNodeType.Text)
					{
						Names[i] = xmlNode2.FirstChild.Value;
					}
					if ((xmlNode2 = xmlNode.SelectSingleNode("info")) != null && xmlNode2.HasChildNodes && xmlNode2.ChildNodes.Count == 1 && xmlNode2.FirstChild.NodeType == XmlNodeType.Text)
					{
						Infos[i] = xmlNode2.FirstChild.Value;
					}
				}
				if (Storys[i] == null)
				{
					Storys[i] = string.Empty;
				}
				if (Names[i] == null)
				{
					Names[i] = string.Empty;
				}
				if (Infos[i] == null)
				{
					Infos[i] = string.Empty;
				}
				if (Scripts[i] == null)
				{
					Scripts[i] = string.Empty;
				}
				if (Chapters[i] == null)
				{
					Chapters[i] = string.Empty;
				}
				Opens[i] = true;
				Opens[i] = GameManager.GameState.CheckChapterOpen(Storys[i], Chapters[i]);
			}
		}

		protected virtual void StartShortcut(int index)
		{
			GameManager.GameScript.StartStory(Storys[index], false);
			GameManager.GameScript.CurrentGameScript.JumpScene(Scripts[index], Chapters[index], Scenes[index]);
			for (int i = 0; i < Conditions[index].Length; i++)
			{
				if (GameManager.GameState.CurrentGameState.SelectDefines.ContainsKey(Conditions[index][i]))
				{
					SelectDefine selectDefine = GameManager.GameState.CurrentGameState.SelectDefines[Conditions[index][i]];
					selectDefine.Value = selectDefine.Case.IndexOf(Values[index][i]);
					continue;
				}
				if (GameManager.GameState.CurrentGameState.VarDefines.ContainsKey(Conditions[index][i]))
				{
					VarDefine varDefine = GameManager.GameState.CurrentGameState.VarDefines[Conditions[index][i]];
					int result;
					if (!int.TryParse(Values[index][i], out result))
					{
						throw new Exception();
					}
					varDefine.Value = result;
					continue;
				}
				throw new Exception();
			}
			FinalScreen();
			GameManager.GalCore.HideTitleScreen();
		}
	}
}
