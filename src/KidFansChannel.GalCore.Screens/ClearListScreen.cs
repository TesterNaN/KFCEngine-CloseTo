using System.Xml;

namespace KidFansChannel.GalCore.Screens
{
	public abstract class ClearListScreen : GameScreen
	{
		protected bool[] SceneOpens;

		protected string[] SceneTitles;

		protected float[] Percentages;

		protected bool[] EndingOpens;

		protected string[] EndingTitles;

		public GameState GameState
		{
			get
			{
				return GameManager.GameState;
			}
		}

		public override void InitData()
		{
			base.InitData();
			XmlDocument xmlDocument = new XmlDocument();
			if (!GameManager.Data.LoadXmlDocument("system", "scene_info.xml", xmlDocument))
			{
				return;
			}
			int argumentInteger = ScriptHelper.GetArgumentInteger(xmlDocument.DocumentElement, new NodeInfo(), "num", 0);
			SceneOpens = new bool[argumentInteger];
			SceneTitles = new string[argumentInteger];
			Percentages = new float[argumentInteger];
			for (int i = 0; i < argumentInteger; i++)
			{
				string text = null;
				string text2 = null;
				XmlNode xmlNode = xmlDocument.DocumentElement.SelectSingleNode("item[@id=\"" + i + "\"]");
				if (xmlNode != null)
				{
					text = ScriptHelper.GetArgumentString(xmlNode, new NodeInfo(), "story");
					text2 = ScriptHelper.GetArgumentString(xmlNode, new NodeInfo(), "chapter");
					XmlNode xmlNode2;
					if ((xmlNode2 = xmlNode.SelectSingleNode("name")) != null && xmlNode2.HasChildNodes && xmlNode2.ChildNodes.Count == 1 && xmlNode2.FirstChild.NodeType == XmlNodeType.Text)
					{
						SceneTitles[i] = xmlNode2.FirstChild.Value;
					}
				}
				if (SceneTitles[i] == null)
				{
					SceneTitles[i] = string.Empty;
				}
				if (text != null && text2 != null)
				{
					SceneOpens[i] = GameManager.GameState.CheckChapterOpen(text, text2);
					Percentages[i] = GameManager.GameState.GetStringPercentage(text, text2);
				}
			}
			xmlDocument = new XmlDocument();
			if (!GameManager.Data.LoadXmlDocument("system", "ending_info.xml", xmlDocument))
			{
				return;
			}
			argumentInteger = ScriptHelper.GetArgumentInteger(xmlDocument.DocumentElement, new NodeInfo(), "num", 0);
			EndingOpens = new bool[argumentInteger];
			EndingTitles = new string[argumentInteger];
			for (int j = 0; j < argumentInteger; j++)
			{
				int num = -1;
				XmlNode xmlNode3 = xmlDocument.DocumentElement.SelectSingleNode("item[@id=\"" + j + "\"]");
				if (xmlNode3 != null)
				{
					num = ScriptHelper.GetArgumentInteger(xmlNode3, new NodeInfo(), "value");
					XmlNode xmlNode4;
					if ((xmlNode4 = xmlNode3.SelectSingleNode("name")) != null && xmlNode4.HasChildNodes && xmlNode4.ChildNodes.Count == 1 && xmlNode4.FirstChild.NodeType == XmlNodeType.Text)
					{
						EndingTitles[j] = xmlNode4.FirstChild.Value;
					}
				}
				if (EndingTitles[j] == null)
				{
					EndingTitles[j] = string.Empty;
				}
				if (num >= 0)
				{
					EndingOpens[j] = GameManager.GameState.CheckEnding(num);
				}
			}
		}
	}
}
