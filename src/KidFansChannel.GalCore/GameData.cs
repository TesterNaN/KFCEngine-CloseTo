using System.IO;
using System.Xml;
using KidFansChannel.DataCore;
using KidFansChannel.Game2DCore;

namespace KidFansChannel.GalCore
{
	public class GameData : KidFansChannel.DataCore.DataCore
	{
		private GalCore galCore;

		public void SetCore(GalCore gc)
		{
			galCore = gc;
		}

		public Picture LoadPicture(string dataName, string pathName)
		{
			switch (CheckData(dataName, pathName, DataType.Package))
			{
			case DataType.Package:
				return galCore.GameGraphics.CreatePicture(GetStream());
			case DataType.File:
				return galCore.GameGraphics.CreatePicture(GetFile());
			default:
				throw new FileNotFoundException(null, Path.Combine(dataName, pathName));
			}
		}

		public Picture LoadPictureInstant(string dataName, string pathName)
		{
			switch (CheckData(dataName, pathName, DataType.Package))
			{
			case DataType.Package:
				return galCore.GameGraphics.CreatePictureInstant(GetStream());
			case DataType.File:
				return galCore.GameGraphics.CreatePictureInstant(GetFile());
			default:
				throw new FileNotFoundException(null, Path.Combine(dataName, pathName));
			}
		}

		public bool LoadXmlDocument(string dataName, string pathName, XmlDocument doc)
		{
			switch (CheckData(dataName, pathName, DataType.Package))
			{
			case DataType.Package:
				doc.Load(GetStream());
				return true;
			case DataType.File:
				doc.Load(GetFile());
				return true;
			default:
				return false;
			}
		}

		public bool LoadAndPlaySystemSound(string pathName)
		{
			switch (GameManager.Data.CheckData("system", pathName, DataType.Package))
			{
			case DataType.Package:
				GameManager.Sound.OpenSystem(GetStream(), true);
				break;
			case DataType.File:
				GameManager.Sound.OpenSystem(GetFile(), true);
				break;
			default:
				return false;
			}
			GameManager.Sound.PlaySystem();
			return true;
		}
	}
}
