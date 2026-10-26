namespace KidFansChannel.GameCore
{
	internal struct GameTimer
	{
		public TimerCallback Callback;

		public float TimeoutInSecs;

		public float Countdown;

		public bool IsEnabled;
	}
}
