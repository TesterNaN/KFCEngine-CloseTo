using System;

namespace KidFansChannel.GameCore
{
	public delegate IntPtr WndProcCallback(IntPtr hWnd, NativeMethods.WindowMessage msg, IntPtr wParam, IntPtr lParam, ref bool NoFurtherProcessing);
}
