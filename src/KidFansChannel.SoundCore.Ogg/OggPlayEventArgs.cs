using System;

namespace KidFansChannel.SoundCore.Ogg
{
	public class OggPlayEventArgs : EventArgs
	{
		private bool _success;

		private string _reasonForFailure;

		private bool _stopRequest;

		public bool Success
		{
			get
			{
				return _success;
			}
			set
			{
				_success = value;
			}
		}

		public string ReasonForFailure
		{
			get
			{
				return _reasonForFailure;
			}
			set
			{
				_reasonForFailure = value;
			}
		}

		public bool StopRequest
		{
			get
			{
				return _stopRequest;
			}
			set
			{
				_stopRequest = value;
			}
		}

		public OggPlayEventArgs(bool success, string reasonForFailure, bool stopRequest)
		{
			_success = success;
			_reasonForFailure = reasonForFailure;
			_stopRequest = stopRequest;
		}

		public OggPlayEventArgs()
		{
			ReasonForFailure = "Unknown";
			Success = true;
			StopRequest = false;
		}

		public OggPlayEventArgs(bool stopRequest)
		{
			_stopRequest = stopRequest;
			Success = true;
			ReasonForFailure = (stopRequest ? "This was a stop request." : "Unknown");
		}

		public OggPlayEventArgs(OggPlayEventArgs e)
			: this(e._success, e._reasonForFailure, e._stopRequest)
		{
		}
	}
}
