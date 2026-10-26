using System;
using System.Xml;
using KidFansChannel.GalCore.Defines;

namespace KidFansChannel.GalCore.Commands
{
	public class Jump : ScriptCommand
	{
		private string _script;

		private string _chapter;

		private int _scene;

		private int _click;

		private string _label;

		private string[] _conditions;

		private string[] _values;

		private bool hasScript = true;

		private bool hasChapter = true;

		private Random random = new Random();

		public string Script
		{
			get
			{
				return _script;
			}
			protected set
			{
				_script = value;
			}
		}

		public string Chapter
		{
			get
			{
				return _chapter;
			}
			protected set
			{
				_chapter = value;
			}
		}

		public int Scene
		{
			get
			{
				return _scene;
			}
			protected set
			{
				_scene = value;
			}
		}

		public int Click
		{
			get
			{
				return _click;
			}
			protected set
			{
				_click = value;
			}
		}

		public string Label
		{
			get
			{
				return _label;
			}
			protected set
			{
				_label = value;
			}
		}

		public string[] Conditions
		{
			get
			{
				return _conditions;
			}
			protected set
			{
				_conditions = value;
			}
		}

		public string[] Values
		{
			get
			{
				return _values;
			}
			protected set
			{
				_values = value;
			}
		}

		public Jump()
		{
			Single = true;
			Lasting = false;
			Command = "jump";
		}

		public override void ConvertXml(XmlNode node, NodeInfo info)
		{
			base.ConvertXml(node, info);
			_click = ScriptHelper.GetArgumentInteger(node, info, "click", -1);
			if (_click < -1)
			{
				throw new ScriptArgumentException(node, info, "click", "Click should be not less than 0.");
			}
			if (_click == -1)
			{
				_label = ScriptHelper.GetArgumentString(node, info, "label", string.Empty);
				if (_label == string.Empty)
				{
					_label = null;
				}
				_script = ScriptHelper.GetArgumentString(node, info, "script", string.Empty);
				if (_script == string.Empty)
				{
					hasScript = false;
					_script = info.File;
					_chapter = ScriptHelper.GetArgumentString(node, info, "chapter", string.Empty);
				}
				else
				{
					_chapter = ScriptHelper.GetArgumentString(node, info, "chapter");
				}
				if (_chapter == string.Empty)
				{
					hasChapter = false;
					_chapter = info.Chapter;
					_scene = ScriptHelper.GetArgumentInteger(node, info, "scene", info.Scene);
				}
				else
				{
					_scene = ScriptHelper.GetArgumentInteger(node, info, "scene", 0);
				}
			}
			string argumentString = ScriptHelper.GetArgumentString(node, info, "condition", string.Empty);
			if (argumentString == string.Empty)
			{
				_conditions = new string[0];
			}
			else
			{
				_conditions = argumentString.Split(';');
			}
			string argumentString2 = ScriptHelper.GetArgumentString(node, info, "value", string.Empty);
			if (argumentString2 == string.Empty)
			{
				_values = new string[0];
			}
			else
			{
				_values = argumentString2.Split(';');
			}
			if (_conditions.Length != _values.Length)
			{
				throw new ScriptArgumentException(node, info, "value", "The number of values and conditions should be matched.");
			}
		}

		public override void InitCommand()
		{
			base.InitCommand();
			bool shouldJump = true;
			for (int i = 0; i < _conditions.Length; i++)
			{
				if (_conditions[i] == "RANDOM")
				{
					float probability;
					try
					{
						probability = float.Parse(_values[i].Substring(1));
					}
					catch (Exception innerException)
					{
						throw new RuntimeException(Node, Info, innerException);
					}
					switch (_values[i][0])
					{
					case '>':
						if (!(random.NextDouble() <= (double)probability))
						{
							continue;
						}
						shouldJump = false;
						break;
					case '<':
						if (!(random.NextDouble() >= (double)probability))
						{
							continue;
						}
						shouldJump = false;
						break;
					default:
						throw new RuntimeException(Node, Info, "Value is not valid.");
					}
					break;
				}
				if (ScriptCommand.GameState.CurrentGameState.SelectDefines.ContainsKey(_conditions[i]))
				{
					SelectDefine selectDefine = ScriptCommand.GameState.CurrentGameState.SelectDefines[_conditions[i]];
					if (_values[i] == "NULL")
					{
						if (selectDefine.Value >= 0)
						{
							shouldJump = false;
							break;
						}
						continue;
					}
					if (_values[i] == "VALUE")
					{
						if (selectDefine.Value == -1)
						{
							shouldJump = false;
							break;
						}
						continue;
					}
					if (selectDefine.Case.Contains(_values[i]))
					{
						if (selectDefine.Value == -1 || selectDefine.Case[selectDefine.Value] != _values[i])
						{
							shouldJump = false;
							break;
						}
						continue;
					}
					throw new RuntimeException(Node, Info, "Value is not valid.");
				}
				VarDefine varDefine;
				if (ScriptCommand.GameState.GlobalVarDefines.ContainsKey(_conditions[i]))
				{
					varDefine = ScriptCommand.GameState.GlobalVarDefines[_conditions[i]];
				}
				else
				{
					if (!ScriptCommand.GameState.CurrentGameState.VarDefines.ContainsKey(_conditions[i]))
					{
						throw new RuntimeException(Node, Info, "Invalid condition.");
					}
					varDefine = ScriptCommand.GameState.CurrentGameState.VarDefines[_conditions[i]];
				}
				int result;
				if (!int.TryParse(_values[i].Substring(1), out result))
				{
					if (ScriptCommand.GameState.GlobalVarDefines.ContainsKey(_values[i].Substring(1)))
					{
						result = ScriptCommand.GameState.GlobalVarDefines[_values[i].Substring(1)].Value;
					}
					else
					{
						if (!ScriptCommand.GameState.CurrentGameState.VarDefines.ContainsKey(_values[i].Substring(1)))
						{
							throw new RuntimeException(Node, Info, "Invalid value.");
						}
						result = ScriptCommand.GameState.CurrentGameState.VarDefines[_values[i].Substring(1)].Value;
					}
				}
				switch (_values[i][0])
				{
				case '=':
					if (varDefine.Value == result)
					{
						continue;
					}
					shouldJump = false;
					break;
				case '>':
					if (varDefine.Value > result)
					{
						continue;
					}
					shouldJump = false;
					break;
				case '<':
					if (varDefine.Value < result)
					{
						continue;
					}
					shouldJump = false;
					break;
				default:
					throw new RuntimeException(Node, Info, "Value is not valid.");
				}
				break;
			}
			if (shouldJump)
			{
				if (_click != -1)
				{
					ScriptCommand.GameScript.Jump(_click);
				}
				else if (_label != null)
				{
					if (hasScript || hasChapter)
					{
						ScriptCommand.GameScript.JumpScene(_script, _chapter, _label);
					}
					else
					{
						ScriptCommand.GameScript.Jump(_label);
					}
				}
				else
				{
					ScriptCommand.GameScript.JumpScene(_script, _chapter, _scene);
				}
			}
			else
			{
				ScriptCommand.GameScript.JumpFake();
			}
			FinishCommand();
		}
	}
}
