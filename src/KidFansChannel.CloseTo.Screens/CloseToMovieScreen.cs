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
			bool[] array = new bool[Opens.Length + 1];
			string[] array2 = new string[Files.Length + 1];
			string[] array3 = new string[Names.Length + 1];
			for (int i = 0; i < Opens.Length; i++)
			{
				array[i] = Opens[i];
				array2[i] = Files[i];
				array3[i] = Names[i];
			}
			array[Opens.Length] = true;
			array2[Files.Length] = "staff";
			array3[Names.Length] = "Staff";
			Opens = array;
			Files = array2;
			Names = array3;
		}
	}
}
