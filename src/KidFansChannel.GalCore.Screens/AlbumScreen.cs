using System.Xml;

namespace KidFansChannel.GalCore.Screens
{
	public abstract class AlbumScreen : GameScreen
	{
		protected bool[][] GroupOpens;

		protected bool[][][] Opens;

		protected string[][][] Files;

		protected string[][] Names;

		public override void InitData()
		{
			base.InitData();
			XmlDocument xmlDocument = new XmlDocument();
			if (!GameManager.Data.LoadXmlDocument("system", "album_info.xml", xmlDocument))
			{
				return;
			}
			int argumentInteger = ScriptHelper.GetArgumentInteger(xmlDocument.DocumentElement, new NodeInfo(), "pagenum", 0);
			Files = new string[argumentInteger][][];
			Opens = new bool[argumentInteger][][];
			GroupOpens = new bool[argumentInteger][];
			Names = new string[argumentInteger][];
			for (int i = 0; i < argumentInteger; i++)
			{
				XmlNode xmlNode = xmlDocument.DocumentElement.SelectSingleNode("page[@index=\"" + i + "\"]");
				if (xmlNode == null)
				{
					continue;
				}
				int argumentInteger2 = ScriptHelper.GetArgumentInteger(xmlNode, new NodeInfo(), "groupnum", 0);
				Files[i] = new string[argumentInteger2][];
				Opens[i] = new bool[argumentInteger2][];
				GroupOpens[i] = new bool[argumentInteger2];
				Names[i] = new string[argumentInteger2];
				for (int j = 0; j < argumentInteger2; j++)
				{
					XmlNode xmlNode2 = xmlNode.SelectSingleNode("group[@index=\"" + j + "\"]");
					if (xmlNode2 == null)
					{
						continue;
					}
					XmlNodeList xmlNodeList = xmlNode2.SelectNodes("item");
					int count = xmlNodeList.Count;
					Files[i][j] = new string[count];
					Opens[i][j] = new bool[count];
					GroupOpens[i][j] = false;
					Names[i][j] = string.Empty;
					XmlNode xmlNode3 = xmlNode2.SelectSingleNode("name");
					if (xmlNode3 != null && xmlNode3.FirstChild != null && xmlNode3.FirstChild.NodeType == XmlNodeType.Text)
					{
						Names[i][j] = xmlNode3.FirstChild.Value;
					}
					for (int k = 0; k < count; k++)
					{
						XmlNode node = xmlNodeList[k];
						int argumentInteger3 = ScriptHelper.GetArgumentInteger(node, new NodeInfo(), "id");
						Files[i][j][k] = ScriptHelper.GetArgumentString(node, new NodeInfo(), "file");
						Opens[i][j][k] = GameManager.GameState.CheckAlbum(argumentInteger3);
						if (Opens[i][j][k])
						{
							GroupOpens[i][j] = true;
						}
					}
				}
			}
		}
	}
}
