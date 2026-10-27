using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace KidFansChannel.CloseTo
{
	internal static class Program
	{
		[STAThread]
		private static void Main(string[] args)
		{
			try
			{
				if (Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName).Length > 1)
				{
					Application.Exit();
					return;
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
			try
			{
				Application.EnableVisualStyles();
				CloseTo closeTo = new CloseTo();
				closeTo.Run(args);
			}
			catch (Exception ex2)
			{
				MessageBox.Show(ex2.Message + "\n" + ((ex2.InnerException == null) ? "" : (ex2.InnerException.Message + "\n")) + ex2.Source + "\n" + ex2.StackTrace + "\n", "Exception");
				Application.Exit();
			}
		}
	}
}
