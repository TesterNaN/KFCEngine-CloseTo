using System;
using System.Runtime.InteropServices;
using System.Text;

namespace KidFansChannel.GalCore
{
	internal class NativeMethods
	{
		internal const int PATH_MAX = 260;

		internal const int CSIDL_PROFILE = 40;

		[DllImport("shfolder.dll", CharSet = CharSet.Unicode)]
		internal static extern int SHGetFolderPath(IntPtr hwndOwner, int nFolder, IntPtr hToken, int dwFlags, StringBuilder lpszPath);

		[DllImport("user32.dll", CharSet = CharSet.Unicode)]
		internal static extern IntPtr PostMessage(IntPtr hwnd, int wMsg, IntPtr wParam, IntPtr lParam);
	}
}
