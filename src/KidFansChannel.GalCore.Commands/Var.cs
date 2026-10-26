using System.Xml;
using KidFansChannel.GalCore.Defines;

namespace KidFansChannel.GalCore.Commands
{
	public class Var : ScriptCommand
	{
		private string _title;

		private int _value;

		private bool? _sign;

		private VarDefine _define;

		public string Title
		{
			get
			{
				return _title;
			}
			protected set
			{
				_title = value;
			}
		}

		public int Value
		{
			get
			{
				return _value;
			}
			protected set
			{
				_value = value;
			}
		}

		public bool? Sign
		{
			get
			{
				return _sign;
			}
			protected set
			{
				_sign = value;
			}
		}

		protected VarDefine Define
		{
			get
			{
				return _define;
			}
			set
			{
				_define = value;
			}
		}

		public Var()
		{
			Single = false;
			Lasting = false;
			Command = "var";
		}

		public override void ConvertXml(XmlNode node, NodeInfo info)
		{
			base.ConvertXml(node, info);
			_title = ScriptHelper.GetArgumentString(node, info, "title");
			if (ScriptCommand.GameState.GlobalVarDefines.ContainsKey(_title))
			{
				_define = ScriptCommand.GameState.GlobalVarDefines[_title];
			}
			else
			{
				if (!ScriptCommand.GameState.CurrentGameState.VarDefines.ContainsKey(_title))
				{
					throw new RuntimeException(node, info, "Var should be defined in index first.");
				}
				_define = ScriptCommand.GameState.CurrentGameState.VarDefines[_title];
			}
			_value = ScriptHelper.GetArgumentInteger(node, info, "value");
			string value = node.Attributes["value"].Value;
			if (value.StartsWith("+"))
			{
				_sign = true;
			}
			else if (value.StartsWith("-"))
			{
				_sign = false;
				_value = -_value;
			}
			else
			{
				_sign = null;
			}
		}

		public override void InitCommand()
		{
			base.InitCommand();
			if (_sign.HasValue)
			{
				if (_sign.Value)
				{
					_define.Value += _value;
				}
				else
				{
					_define.Value -= _value;
				}
			}
			else
			{
				_define.Value = _value;
			}
			FinishCommand();
		}
	}
}
