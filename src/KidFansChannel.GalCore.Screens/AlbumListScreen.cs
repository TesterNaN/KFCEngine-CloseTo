using System.Xml;

namespace KidFansChannel.GalCore.Screens
{
	public abstract class AlbumListScreen : GameScreen
	{
		protected bool[] Opens;

		public override void InitData()
		{
			base.InitData();
			XmlDocument xmlDocument = new XmlDocument();
			if (!GameManager.Data.LoadXmlDocument("system", "album_info.xml", xmlDocument))
			{
				return;
			}
			int argumentInteger = ScriptHelper.GetArgumentInteger(xmlDocument.DocumentElement, new NodeInfo(), "pagenum", 0);
			Opens = new bool[argumentInteger];
			for (int i = 0; i < argumentInteger; i++)
			{
				XmlNode xmlNode = xmlDocument.DocumentElement.SelectSingleNode("page[@index=\"" + i + "\"]");
				if (xmlNode == null)
				{
					continue;
				}
				int argumentInteger2 = ScriptHelper.GetArgumentInteger(xmlNode, new NodeInfo(), "groupnum", 0);
				for (int j = 0; j < argumentInteger2; j++)
				{
					XmlNode xmlNode2 = xmlNode.SelectSingleNode("group[@index=\"" + j + "\"]");
					if (xmlNode2 == null)
					{
						continue;
					}
					int count = xmlNode2.ChildNodes.Count;
					for (int k = 0; k < count; k++)
					{
						XmlNode node = xmlNode2.ChildNodes[k];
						int argumentInteger3 = ScriptHelper.GetArgumentInteger(node, new NodeInfo(), "id");
						if (GameManager.GameState.CheckAlbum(argumentInteger3))
						{
							Opens[i] = true;
							break;
						}
					}
					if (Opens[i])
					{
						break;
					}
				}
			}
		}
	}
}
