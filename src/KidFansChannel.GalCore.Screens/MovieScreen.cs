using System.IO;
using System.Xml;
using KidFansChannel.DataCore;

namespace KidFansChannel.GalCore.Screens
{
	public abstract class MovieScreen : GameScreen
	{
		protected string[] Files;

		protected bool[] Opens;

		protected string[] Names;

		public override void InitData()
		{
			base.InitData();
			XmlDocument xmlDocument = new XmlDocument();
			if (!GameManager.Data.LoadXmlDocument("system", "movie_info.xml", xmlDocument))
			{
				return;
			}
			int argumentInteger = ScriptHelper.GetArgumentInteger(xmlDocument.DocumentElement, new NodeInfo(), "num", 0);
			Files = new string[argumentInteger];
			Opens = new bool[argumentInteger];
			Names = new string[argumentInteger];
			for (int i = 0; i < argumentInteger; i++)
			{
				Opens[i] = GameManager.GameState.CheckMovie(i);
				XmlNode xmlNode = xmlDocument.DocumentElement.SelectSingleNode("item[@id=\"" + i + "\"]");
				if (xmlNode != null)
				{
					Files[i] = ScriptHelper.GetArgumentString(xmlNode, new NodeInfo(), "file");
				}
				if (Files[i] == null)
				{
					Files[i] = string.Empty;
				}
				Names[i] = string.Empty;
				XmlNode xmlNode2 = xmlNode.SelectSingleNode("name");
				if (xmlNode2 != null && xmlNode2.FirstChild != null && xmlNode2.FirstChild.NodeType == XmlNodeType.Text)
				{
					Names[i] = xmlNode2.FirstChild.Value;
				}
			}
		}

		protected void PlayMovie(string movie)
		{
			KidFansChannel.DataCore.DataCore.DataType dataType = GameManager.Data.CheckData("movie", movie + ".wmv", KidFansChannel.DataCore.DataCore.DataType.File);
			if (dataType == KidFansChannel.DataCore.DataCore.DataType.File)
			{
				string text = GameManager.Data.GetFile();
				if (!GameManager.GalCore.GameOptions.SubtitleShow && text.EndsWith(GameManager.Data.LanguageExt + "\\" + movie + ".wmv") && File.Exists(text.Replace(GameManager.Data.LanguageExt + "\\" + movie + ".wmv", movie + ".wmv")))
				{
					text = text.Replace(GameManager.Data.LanguageExt + "\\" + movie + ".wmv", movie + ".wmv");
				}
				if (GameManager.GalCore.GameVideo != null)
				{
					GameManager.GalCore.GameVideo.Play(text);
				}
			}
		}
	}
}
