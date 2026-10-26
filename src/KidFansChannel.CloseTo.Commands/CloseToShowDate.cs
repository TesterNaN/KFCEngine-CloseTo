using KidFansChannel.GalCore.Commands;

namespace KidFansChannel.CloseTo.Commands
{
	public class CloseToShowDate : ShowDate
	{
		public CloseToShowDate()
		{
			ShowHideTime = 1f;
			LastingTime = 3f;
		}

		protected override string ConvertDate(string date)
		{
			return string.Format(ScriptCommand.GalCore.GameLanguage.ResourceManager.GetString("DateStr"), date.Substring(0, 2), date.Substring(3, 2));
		}
	}
}
