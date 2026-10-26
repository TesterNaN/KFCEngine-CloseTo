using System.Collections.Generic;
using System.Xml;

namespace KidFansChannel.GalCore.Commands
{
	public class Wait : ScriptCommand, IUpdatable
	{
		private float _waitTime;

		private bool _voice;

		private List<ScriptCommand> _parList;

		protected float WaitTime
		{
			get
			{
				return _waitTime;
			}
		}

		public bool Voice
		{
			get
			{
				return _voice;
			}
		}

		protected internal List<ScriptCommand> ParList
		{
			get
			{
				return _parList;
			}
			set
			{
				_parList = value;
			}
		}

		public Wait()
		{
			Command = "wait";
			Single = true;
			Lasting = true;
		}

		public override void ConvertXml(XmlNode node, NodeInfo info)
		{
			base.ConvertXml(node, info);
			_waitTime = ScriptHelper.GetArgumentFloat(node, info, "value");
			_voice = ScriptHelper.GetArgumentBool(node, info, "voice", false);
		}

		public override void InitCommand()
		{
			base.InitCommand();
			if (_waitTime != 0f || _voice)
			{
				return;
			}
			Lasting = false;
			FinishCommand();
			if (_parList == null)
			{
				return;
			}
			foreach (ScriptCommand par in _parList)
			{
				if (par is IInstanceDataUser)
				{
					((IInstanceDataUser)par).InitInstanceData();
				}
				ScriptCommand.GalCore.ToPrepareCommandList.Add(par);
			}
			_parList.Clear();
			_parList = null;
		}

		public void Update(double appTime, float elaspedTime)
		{
			switch (ScriptCommand.PlayMode)
			{
			case PlayMode.Normal:
			case PlayMode.Auto:
				if (!_voice || !ScriptCommand.Sound.IsPlayingVoice)
				{
					_waitTime -= elaspedTime;
				}
				break;
			case PlayMode.QuickOnce:
			case PlayMode.Quick:
			case PlayMode.QuickRead:
			case PlayMode.QuickAll:
				_waitTime -= elaspedTime * (float)ScriptCommand.QuickFactor;
				break;
			}
			if (!(_waitTime <= 0f) || (_voice && ScriptCommand.Sound.IsPlayingVoice))
			{
				return;
			}
			Lasting = false;
			FinishCommand();
			if (_parList == null)
			{
				return;
			}
			foreach (ScriptCommand par in _parList)
			{
				if (par is IInstanceDataUser)
				{
					((IInstanceDataUser)par).InitInstanceData();
				}
				ScriptCommand.GalCore.ToPrepareCommandList.Add(par);
			}
			_parList.Clear();
			_parList = null;
		}
	}
}
