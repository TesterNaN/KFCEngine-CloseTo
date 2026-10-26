using System;

namespace KidFansChannel.GameCore
{
	public class RenderEventArgs : EventArgs
	{
		private float elapsedTime;

		private double appTime;

		public float ElapsedTime
		{
			get
			{
				return elapsedTime;
			}
			set
			{
				elapsedTime = value;
			}
		}

		public double AppTime
		{
			get
			{
				return appTime;
			}
			set
			{
				appTime = value;
			}
		}

		public RenderEventArgs(double appTime, float elapsedTime)
		{
			this.appTime = appTime;
			this.elapsedTime = elapsedTime;
		}
	}
}
