using System.Xml;
using KidFansChannel.GalCore.Records;

namespace KidFansChannel.GalCore.Commands
{
	public abstract class ShowTime : ScriptCommand, IStaticDataUser, IDrawable, IUpdatable, IRecordable
	{
		private string _time;

		private bool _hide;

		public static string AboveTime = "==========";

		public static string BelowTime = "==========";

		private bool _canRecord;

		public string Time
		{
			get
			{
				return _time;
			}
			set
			{
				_time = value;
			}
		}

		public bool Hide
		{
			get
			{
				return _hide;
			}
			set
			{
				_hide = value;
			}
		}

		public int LayerMajor
		{
			get
			{
				return 30;
			}
		}

		public float LayerMinor
		{
			get
			{
				return 0f;
			}
		}

		public bool CanHide
		{
			get
			{
				return true;
			}
		}

		public bool CanRecord
		{
			get
			{
				return _canRecord;
			}
			set
			{
				_canRecord = value;
			}
		}

		public ShowTime()
		{
			Single = false;
			Lasting = false;
			Command = "showtime";
		}

		public override void ConvertXml(XmlNode node, NodeInfo info)
		{
			base.ConvertXml(node, info);
			_time = ScriptHelper.GetArgumentString(node, info, "value");
			_hide = ScriptHelper.GetArgumentBool(node, info, "hide", false);
			if (!_hide)
			{
				_canRecord = true;
			}
		}

		public override void InitCommand()
		{
			base.InitCommand();
			foreach (ScriptCommand command in ScriptCommand.CommandList)
			{
				if (command is ShowTime && command != this)
				{
					command.FinishCommand();
					break;
				}
			}
			if (ScriptCommand.GameState.CurrentGameState.GameDateTime.Length < 6)
			{
				ScriptCommand.GameState.CurrentGameState.GameDateTime = "??/?? " + _time;
			}
			else
			{
				ScriptCommand.GameState.CurrentGameState.GameDateTime = ScriptCommand.GameState.CurrentGameState.GameDateTime.Substring(0, 6) + _time;
			}
			if (_hide)
			{
				FinishCommand();
			}
		}

		public override void FinalCommand()
		{
			base.FinalCommand();
		}

		public abstract void Draw();

		public abstract void Update(double appTime, float elaspedTime);

		public abstract void InitStaticData();

		public abstract void FinalStaticData();

		public Record[] GetRecord()
		{
			if (_canRecord)
			{
				_canRecord = false;
				DateTimeRecord dateTimeRecord = new DateTimeRecord();
				dateTimeRecord.Value = ScriptCommand.GameState.CurrentGameState.GameDateTime;
				dateTimeRecord.Text = ConvertTime(_time);
				NullRecord nullRecord = new NullRecord();
				nullRecord.Text = AboveTime;
				NullRecord nullRecord2 = new NullRecord();
				nullRecord2.Text = BelowTime;
				return new Record[3] { nullRecord, dateTimeRecord, nullRecord2 };
			}
			return null;
		}

		protected virtual string ConvertTime(string time)
		{
			return time;
		}
	}
}
