using System.Collections.Generic;
using Microsoft.DirectX.Direct3D;

namespace KidFansChannel.GameCore
{
	internal class DisplayModeSorter : IComparer<DisplayMode>
	{
		public int Compare(DisplayMode x, DisplayMode y)
		{
			if (x.Width > y.Width)
			{
				return 1;
			}
			if (x.Width < y.Width)
			{
				return -1;
			}
			if (x.Height > y.Height)
			{
				return 1;
			}
			if (x.Height < y.Height)
			{
				return -1;
			}
			if (x.Format > y.Format)
			{
				return 1;
			}
			if (x.Format < y.Format)
			{
				return -1;
			}
			if (x.RefreshRate > y.RefreshRate)
			{
				return 1;
			}
			if (x.RefreshRate < y.RefreshRate)
			{
				return -1;
			}
			return 0;
		}
	}
}
