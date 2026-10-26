using System.Xml;

namespace KidFansChannel.GalCore.Screens
{
	public abstract class EncyListScreen : GameScreen
	{
		protected string[] Titles;

		protected string[] Catalogues;

		protected bool[] Opens;

		protected bool[] Views;

		public override void InitData()
		{
			base.InitData();
			int encyNumber = GameManager.GameState.GetEncyNumber();
			Titles = new string[encyNumber];
			Catalogues = new string[encyNumber];
			Opens = new bool[encyNumber];
			Views = new bool[encyNumber];
			XmlDocument xmlDocument = new XmlDocument();
			if (!GameManager.Data.LoadXmlDocument("system", "ency_info.xml", xmlDocument))
			{
				return;
			}
			for (int i = 0; i < encyNumber; i++)
			{
				if (GameManager.GameState.CheckEncyOpen(i))
				{
					Opens[i] = true;
					Views[i] = GameManager.GameState.CheckEncyView(i);
				}
				XmlNode xmlNode = xmlDocument.DocumentElement.SelectSingleNode("item[@id=\"" + i + "\"]");
				if (xmlNode != null)
				{
					XmlNode xmlNode2;
					if ((xmlNode2 = xmlNode.SelectSingleNode("title")) != null && xmlNode2.HasChildNodes && xmlNode2.ChildNodes.Count == 1 && xmlNode2.FirstChild.NodeType == XmlNodeType.Text)
					{
						Titles[i] = xmlNode2.FirstChild.Value;
					}
					if ((xmlNode2 = xmlNode.SelectSingleNode("catalogue")) != null && xmlNode2.HasChildNodes && xmlNode2.ChildNodes.Count == 1 && xmlNode2.FirstChild.NodeType == XmlNodeType.Text)
					{
						Catalogues[i] = xmlNode2.FirstChild.Value;
					}
				}
				if (Titles[i] == null)
				{
					Titles[i] = string.Empty;
				}
				if (Catalogues[i] == null)
				{
					Catalogues[i] = string.Empty;
				}
			}
		}
	}
}
