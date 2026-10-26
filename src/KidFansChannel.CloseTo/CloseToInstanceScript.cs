using KidFansChannel.GalCore;

namespace KidFansChannel.CloseTo
{
	public class CloseToInstanceScript : GameInstanceScript
	{
		public string ChapterChara;

		protected internal CloseToInstanceScript(string storyName, bool jump, GameScript gameScript)
			: base(storyName, jump, gameScript)
		{
		}

		public override string GetChapterName(string chapter)
		{
			if (chapter.StartsWith("Room"))
			{
				int index = -1;
				for (int i = 0; i < CloseToState.RoomIndex.Count; i++)
				{
					if (CloseToState.RoomIndex[i].Contains(chapter.Substring(4)))
					{
						index = i;
						break;
					}
				}
				return GetChapterName(CloseToState.RoomName[index]);
			}
			return base.GetChapterName(chapter);
		}

		protected override bool InitChapter()
		{
			bool result = base.InitChapter();
			if (CurrentChapter.Attributes["title"].Value.StartsWith("Koyuki"))
			{
				ChapterChara = "k";
			}
			else if (CurrentChapter.Attributes["title"].Value.StartsWith("Mai"))
			{
				ChapterChara = "m";
			}
			else if (CurrentChapter.Attributes["title"].Value.StartsWith("Shoko"))
			{
				ChapterChara = "s";
			}
			else if (CurrentChapter.Attributes["title"].Value.StartsWith("Yuna"))
			{
				ChapterChara = "y";
			}
			return result;
		}
	}
}
