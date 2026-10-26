using System.Xml;

namespace KidFansChannel.GalCore.Screens
{
	public abstract class EncyContentScreen : GameScreen
	{
		protected string Title;

		protected string Catalogue;

		protected bool View;

		protected XmlNodeList Content;

		private int _index;

		public int Index
		{
			get
			{
				return _index;
			}
			set
			{
				_index = value;
			}
		}

		public EncyContentScreen(int index)
		{
			_index = index;
		}

		public override void InitData()
		{
			base.InitData();
			XmlDocument xmlDocument = new XmlDocument();
			View = GameManager.GameState.CheckEncyView(_index);
			GameManager.GameState.UpdateEncyView(_index);
			if (!GameManager.Data.LoadXmlDocument("system", "ency_info.xml", xmlDocument))
			{
				Title = string.Empty;
				Catalogue = string.Empty;
				return;
			}
			XmlNode xmlNode = xmlDocument.DocumentElement.SelectSingleNode("item[@id=\"" + _index + "\"]");
			if (xmlNode != null)
			{
				XmlNode xmlNode2;
				if ((xmlNode2 = xmlNode.SelectSingleNode("title")) != null && xmlNode2.HasChildNodes && xmlNode2.ChildNodes.Count == 1 && xmlNode2.ChildNodes[0].NodeType == XmlNodeType.Text)
				{
					Title = xmlNode2.ChildNodes[0].Value;
				}
				if ((xmlNode2 = xmlNode.SelectSingleNode("catalogue")) != null && xmlNode2.HasChildNodes && xmlNode2.ChildNodes.Count == 1 && xmlNode2.ChildNodes[0].NodeType == XmlNodeType.Text)
				{
					Catalogue = xmlNode2.ChildNodes[0].Value;
				}
				if ((xmlNode2 = xmlNode.SelectSingleNode("content")) != null && xmlNode2.HasChildNodes)
				{
					Content = xmlNode2.ChildNodes;
				}
			}
			if (Title == null)
			{
				Title = string.Empty;
			}
			if (Catalogue == null)
			{
				Catalogue = string.Empty;
			}
		}
	}
}
