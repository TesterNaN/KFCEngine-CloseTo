namespace KidFansChannel.GameCore
{
	internal class GameCoreTimer
	{
		private static bool isUsingQPF;

		private static bool isTimerStopped;

		private static long ticksPerSecond;

		private static long stopTime;

		private static long lastElapsedTime;

		private static long baseTime;

		public static bool IsStopped
		{
			get
			{
				return isTimerStopped;
			}
		}

		private GameCoreTimer()
		{
		}

		static GameCoreTimer()
		{
			isTimerStopped = true;
			ticksPerSecond = 0L;
			stopTime = 0L;
			lastElapsedTime = 0L;
			baseTime = 0L;
			isUsingQPF = NativeMethods.QueryPerformanceFrequency(ref ticksPerSecond);
		}

		public static void Reset()
		{
			if (isUsingQPF)
			{
				long PerformanceCount = 0L;
				if (stopTime != 0)
				{
					PerformanceCount = stopTime;
				}
				else
				{
					NativeMethods.QueryPerformanceCounter(ref PerformanceCount);
				}
				baseTime = PerformanceCount;
				lastElapsedTime = PerformanceCount;
				stopTime = 0L;
				isTimerStopped = false;
			}
		}

		public static void Start()
		{
			if (isUsingQPF)
			{
				long PerformanceCount = 0L;
				if (stopTime != 0)
				{
					PerformanceCount = stopTime;
				}
				else
				{
					NativeMethods.QueryPerformanceCounter(ref PerformanceCount);
				}
				if (isTimerStopped)
				{
					baseTime += PerformanceCount - stopTime;
				}
				stopTime = 0L;
				lastElapsedTime = PerformanceCount;
				isTimerStopped = false;
			}
		}

		public static void Stop()
		{
			if (isUsingQPF && !isTimerStopped)
			{
				long PerformanceCount = 0L;
				if (stopTime != 0)
				{
					PerformanceCount = stopTime;
				}
				else
				{
					NativeMethods.QueryPerformanceCounter(ref PerformanceCount);
				}
				stopTime = PerformanceCount;
				lastElapsedTime = PerformanceCount;
				isTimerStopped = true;
			}
		}

		public static double GetAbsoluteTime()
		{
			if (!isUsingQPF)
			{
				return -1.0;
			}
			long PerformanceCount = 0L;
			if (stopTime != 0)
			{
				PerformanceCount = stopTime;
			}
			else
			{
				NativeMethods.QueryPerformanceCounter(ref PerformanceCount);
			}
			return (double)PerformanceCount / (double)ticksPerSecond;
		}

		public static double GetTime()
		{
			if (!isUsingQPF)
			{
				return -1.0;
			}
			long PerformanceCount = 0L;
			if (stopTime != 0)
			{
				PerformanceCount = stopTime;
			}
			else
			{
				NativeMethods.QueryPerformanceCounter(ref PerformanceCount);
			}
			return (double)(PerformanceCount - baseTime) / (double)ticksPerSecond;
		}

		public static double GetElapsedTime()
		{
			if (!isUsingQPF)
			{
				return -1.0;
			}
			long PerformanceCount = 0L;
			if (stopTime != 0)
			{
				PerformanceCount = stopTime;
			}
			else
			{
				NativeMethods.QueryPerformanceCounter(ref PerformanceCount);
			}
			double result = (double)(PerformanceCount - lastElapsedTime) / (double)ticksPerSecond;
			lastElapsedTime = PerformanceCount;
			return result;
		}
	}
}
