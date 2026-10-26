using System;
using System.Windows.Forms;

namespace KidFansChannel.CloseToSetting
{
	internal static class Program
	{
		[STAThread]
		private static void Main()
		{
			try
			{
				Application.EnableVisualStyles();
				Application.SetCompatibleTextRenderingDefault(false);
				Application.Run(new CloseToSetting());
			}
			catch (Exception ex)
			{
				try
				{
					System.IO.File.WriteAllText(
						System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "error.log"),
						ex.ToString());
				}
				catch (Exception)
				{
				}
				MessageBox.Show(ex.Message, "Exception");
			}
		}
	}
}
