using System.Xml;
using KidFansChannel.DataCore;

namespace KidFansChannel.GalCore.Screens
{
	public abstract class MusicScreen : GameScreen
	{
		protected string[] Names;

		protected string[] Files;

		protected string[] Times;

		protected string[] Infos;

		protected bool[] Opens;

		protected string[] Lyrics;

		protected float[] Starts;

		protected float[] Ends;

		protected bool IsMusicing
		{
			get
			{
				return GameManager.Sound.IsPlayingVoice;
			}
		}

		public override void InitData()
		{
			base.InitData();
			XmlDocument xmlDocument = new XmlDocument();
			if (!GameManager.Data.LoadXmlDocument("system", "music_info.xml", xmlDocument))
			{
				return;
			}
			int argumentInteger = ScriptHelper.GetArgumentInteger(xmlDocument.DocumentElement, new NodeInfo(), "num", 0);
			Names = new string[argumentInteger];
			Files = new string[argumentInteger];
			Times = new string[argumentInteger];
			Infos = new string[argumentInteger];
			Opens = new bool[argumentInteger];
			for (int i = 0; i < argumentInteger; i++)
			{
				Opens[i] = GameManager.GameState.CheckMusic(i);
				XmlNode xmlNode = xmlDocument.DocumentElement.SelectSingleNode("item[@id=\"" + i + "\"]");
				if (xmlNode != null)
				{
					Files[i] = ScriptHelper.GetArgumentString(xmlNode, new NodeInfo(), "file");
					XmlNode xmlNode2;
					if ((xmlNode2 = xmlNode.SelectSingleNode("name")) != null && xmlNode2.HasChildNodes && xmlNode2.ChildNodes.Count == 1 && xmlNode2.FirstChild.NodeType == XmlNodeType.Text)
					{
						Names[i] = xmlNode2.FirstChild.Value;
					}
					if ((xmlNode2 = xmlNode.SelectSingleNode("info")) != null && xmlNode2.HasChildNodes && xmlNode2.ChildNodes.Count == 1 && xmlNode2.FirstChild.NodeType == XmlNodeType.Text)
					{
						Infos[i] = xmlNode2.FirstChild.Value;
					}
					if ((xmlNode2 = xmlNode.SelectSingleNode("time")) != null && xmlNode2.HasChildNodes && xmlNode2.ChildNodes.Count == 1 && xmlNode2.FirstChild.NodeType == XmlNodeType.Text)
					{
						Times[i] = xmlNode2.FirstChild.Value;
					}
				}
				if (Files[i] == null)
				{
					Files[i] = string.Empty;
				}
				if (Names[i] == null)
				{
					Names[i] = string.Empty;
				}
				if (Infos[i] == null)
				{
					Infos[i] = string.Empty;
				}
				if (Times[i] == null)
				{
					Times[i] = string.Empty;
				}
			}
		}

		public override bool InitScreen()
		{
			if (!base.InitScreen())
			{
				return false;
			}
			StopMusic();
			return true;
		}

		public override bool FinalScreen()
		{
			if (!base.FinalScreen())
			{
				return false;
			}
			StopMusic();
			return true;
		}

		protected void PlayMusic(string music)
		{
			switch (GameManager.Data.CheckData("bgm", music + ".ogg", KidFansChannel.DataCore.DataCore.DataType.File))
			{
			default:
				return;
			case KidFansChannel.DataCore.DataCore.DataType.File:
				GameManager.Sound.OpenVoice(GameManager.Data.GetFile(), true);
				break;
			case KidFansChannel.DataCore.DataCore.DataType.Package:
				GameManager.Sound.OpenVoice(GameManager.Data.GetStream(), true);
				break;
			}
			GameManager.Sound.PlayVoice();
		}

		protected void StopMusic()
		{
			GameManager.Sound.StopVoice();
		}

		protected bool PrepareLyric(string name)
		{
			XmlDocument xmlDocument = new XmlDocument();
			if (GameManager.Data.LoadXmlDocument("system", "lyric\\" + name + ".xml", xmlDocument))
			{
				XmlNodeList xmlNodeList = xmlDocument.DocumentElement.SelectNodes("sentence");
				int count = xmlNodeList.Count;
				Lyrics = new string[count];
				Starts = new float[count];
				Ends = new float[count];
				for (int i = 0; i < count; i++)
				{
					XmlNode xmlNode = xmlNodeList[i];
					if (xmlNode.HasChildNodes && xmlNode.ChildNodes.Count == 1 && xmlNode.FirstChild.NodeType == XmlNodeType.Text)
					{
						Lyrics[i] = xmlNode.FirstChild.Value;
					}
					else
					{
						Lyrics[i] = string.Empty;
					}
					Starts[i] = ScriptHelper.GetArgumentFloat(xmlNode, new NodeInfo(), "start", (i == 0) ? 0f : Ends[i - 1]);
					Ends[i] = ScriptHelper.GetArgumentFloat(xmlNode, new NodeInfo(), "end", Starts[i]);
				}
				return true;
			}
			Lyrics = null;
			Starts = null;
			Ends = null;
			return false;
		}
	}
}
