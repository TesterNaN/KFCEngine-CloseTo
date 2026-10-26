using System.Xml;
using KidFansChannel.GalCore.Records;
using KidFansChannel.Game2DCore;

namespace KidFansChannel.GalCore.Commands
{
	public class ShowDate : ScriptCommand, IInstanceDataUser, IDrawable, IUpdatable, IRecordable
	{
		private string _date;

		private Picture _datePic;

		private string _dateStr;

		private bool _hide;

		public float ShowHideTime = 1f;

		public float LastingTime = 5f;

		private float time;

		public static string AboveDate = "==========";

		public static string BelowDate = "==========";

		private bool _canRecord;

		public string Date
		{
			get
			{
				return _date;
			}
			set
			{
				_date = value;
			}
		}

		public Picture DatePic
		{
			get
			{
				return _datePic;
			}
			set
			{
				_datePic = value;
			}
		}

		public string DateStr
		{
			get
			{
				return _dateStr;
			}
			set
			{
				_dateStr = value;
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

		public bool DataReady
		{
			get
			{
				if (_dateStr != null)
				{
					return _datePic.IsCreated;
				}
				return true;
			}
		}

		public int LayerMajor
		{
			get
			{
				return 19;
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
				return false;
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

		public ShowDate()
		{
			Single = true;
			Lasting = true;
			Command = "showdate";
		}

		public override void ConvertXml(XmlNode node, NodeInfo info)
		{
			base.ConvertXml(node, info);
			_date = ScriptHelper.GetArgumentString(node, info, "value");
			_dateStr = ScriptHelper.GetArgumentString(node, info, "file", string.Empty);
			_hide = ScriptHelper.GetArgumentBool(node, info, "hide", false);
			if (!_hide && _dateStr == string.Empty)
			{
				throw new ScriptArgumentException(node, info, "file", ArgumentType.String, "File cannot be empty.");
			}
			if (_hide)
			{
				_dateStr = null;
			}
			if (!_hide)
			{
				_canRecord = true;
				return;
			}
			Single = false;
			Lasting = false;
		}

		public override void InitCommand()
		{
			base.InitCommand();
			foreach (ScriptCommand command in ScriptCommand.CommandList)
			{
				if (command is ShowDate && command != this)
				{
					command.FinishCommand();
					break;
				}
			}
			ScriptCommand.GameState.CurrentGameState.GameDateTime = _date + " ??:??";
			if (!_hide)
			{
				_datePic.Alpha = 0;
				_datePic.BeginTransform(ShowHideTime);
				_datePic.AlphaTo(255);
				_datePic.EndTransform();
				time = LastingTime;
			}
			else
			{
				FinishCommand();
			}
		}

		public override void FinalCommand()
		{
			base.FinalCommand();
			FinalInstanceData();
		}

		public void InitInstanceData()
		{
			if (_dateStr != null)
			{
				_datePic = ScriptCommand.Data.LoadPicture("system", "cal\\" + _dateStr + ".png");
			}
		}

		public void FinalInstanceData()
		{
			if (_datePic != null)
			{
				_datePic.Dispose();
				_datePic = null;
			}
		}

		public void Draw()
		{
			if (DataReady)
			{
				ScriptCommand.Canvas.DrawPicture(_datePic);
			}
		}

		public void Update(double appTime, float elaspedTime)
		{
			if (ScriptCommand.PlayModeQuicking)
			{
				_datePic.Update(elaspedTime * (float)ScriptCommand.QuickFactor);
			}
			else
			{
				_datePic.Update(elaspedTime);
			}
			if (_datePic.Time != 0f)
			{
				return;
			}
			if (_datePic.Alpha == 255)
			{
				if (ScriptCommand.PlayModeQuicking)
				{
					time -= elaspedTime * (float)ScriptCommand.QuickFactor;
				}
				else
				{
					time -= elaspedTime;
				}
				if (time < 0f)
				{
					time = 0f;
				}
				if (time == 0f)
				{
					_datePic.BeginTransform(ShowHideTime);
					_datePic.AlphaTo(0);
					_datePic.EndTransform();
				}
			}
			else
			{
				FinishCommand();
			}
		}

		public Record[] GetRecord()
		{
			if (_canRecord)
			{
				_canRecord = false;
				DateTimeRecord dateTimeRecord = new DateTimeRecord();
				dateTimeRecord.Value = ScriptCommand.GameState.CurrentGameState.GameDateTime;
				dateTimeRecord.Text = ConvertDate(_date);
				NullRecord nullRecord = new NullRecord();
				nullRecord.Text = AboveDate;
				NullRecord nullRecord2 = new NullRecord();
				nullRecord2.Text = BelowDate;
				return new Record[3] { nullRecord, dateTimeRecord, nullRecord2 };
			}
			return null;
		}

		protected virtual string ConvertDate(string date)
		{
			return date;
		}
	}
}
