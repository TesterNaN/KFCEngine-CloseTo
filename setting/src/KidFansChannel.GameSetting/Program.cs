using System;
using System.Windows.Forms;

namespace KidFansChannel.GameSetting
{
	internal static class Program
	{
		[STAThread]
		private static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new Setting());
		}
	}
}
