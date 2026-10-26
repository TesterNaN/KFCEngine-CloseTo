using KidFansChannel.GalCore.Screens;

namespace KidFansChannel.CloseTo.Screens
{
	public class CloseToMovieScreen : MovieScreen
	{
		public bool[] GetOpens()
		{
			return Opens;
		}

		public void Play(int index)
		{
			PlayMovie(Files[index]);
		}

		public string GetName(int index)
		{
			return Names[index];
		}

		public override void InitData()
		{
			base.InitData();
			bool[] opensBuf = new bool[Opens.Length + 1];
			string[] filesBuf = new string[Files.Length + 1];
			string[] namesBuf = new string[Names.Length + 1];
			for (int i = 0; i < Opens.Length; i++)
			{
				opensBuf[i] = Opens[i];
				filesBuf[i] = Files[i];
				namesBuf[i] = Names[i];
			}
			opensBuf[Opens.Length] = true;
			filesBuf[Files.Length] = "staff";
			namesBuf[Names.Length] = "Staff";
			Opens = opensBuf;
			Files = filesBuf;
			Names = namesBuf;
		}
	}
}
