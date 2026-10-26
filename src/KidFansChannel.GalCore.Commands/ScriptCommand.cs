using System.Collections.Generic;
using System.Xml;
using KidFansChannel.Game2DCore;

namespace KidFansChannel.GalCore.Commands
{
	public abstract class ScriptCommand
	{
		private bool _single;

		private bool _lasting;

		private static GalCore _galCore;

		private bool _finished;

		private string _dataName;

		private string _command;

		private XmlNode _node;

		private NodeInfo _info;

		public static int QuickFactor = 4;

		public bool Single
		{
			get
			{
				return _single;
			}
			set
			{
				_single = value;
			}
		}

		public bool Lasting
		{
			get
			{
				return _lasting;
			}
			set
			{
				_lasting = value;
			}
		}

		public static GalCore GalCore
		{
			get
			{
				return _galCore;
			}
			protected set
			{
				_galCore = value;
			}
		}

		public bool Finished
		{
			get
			{
				return _finished;
			}
		}

		public string DataName
		{
			get
			{
				return _dataName;
			}
			protected set
			{
				_dataName = value;
			}
		}

		public string Command
		{
			get
			{
				return _command;
			}
			protected set
			{
				_command = value;
			}
		}

		public XmlNode Node
		{
			get
			{
				return _node;
			}
			set
			{
				_node = value;
			}
		}

		public NodeInfo Info
		{
			get
			{
				return _info;
			}
			set
			{
				_info = value;
			}
		}

		public static GameOptions Options
		{
			get
			{
				return _galCore.GameOptions;
			}
		}

		public static PlayMode PlayMode
		{
			get
			{
				return _galCore.CurrentPlayMode;
			}
		}

		public static bool PlayModeQuicking
		{
			get
			{
				if (PlayMode == PlayMode.Quick && Options.QuickReadOnly && !GameScript.StringRead)
				{
					_galCore.ToNormal = true;
					return false;
				}
				if ((PlayMode != PlayMode.Quick || Options.QuickReadOnly) && PlayMode != PlayMode.QuickAll && PlayMode != PlayMode.QuickOnce)
				{
					if (PlayMode == PlayMode.QuickRead || (PlayMode == PlayMode.Quick && Options.QuickReadOnly))
					{
						return GameScript.StringRead;
					}
					return false;
				}
				return true;
			}
		}

		public static GameData Data
		{
			get
			{
				return _galCore.GameData;
			}
		}

		public static GameSound Sound
		{
			get
			{
				return _galCore.GameSound;
			}
		}

		public static GameGraphics Graphics
		{
			get
			{
				return _galCore.GameGraphics;
			}
		}

		public static List<ScriptCommand> CommandList
		{
			get
			{
				return _galCore.CurrentCommandList;
			}
		}

		public static ICanvas Canvas
		{
			get
			{
				return _galCore.CurrentCanvas;
			}
		}

		public static GameState GameState
		{
			get
			{
				return _galCore.GameState;
			}
		}

		public static GameInstanceScript GameScript
		{
			get
			{
				return _galCore.GameScript.CurrentGameScript;
			}
		}

		public virtual void FinishCommand()
		{
			_finished = true;
		}

		public static void SetCore(GalCore gc)
		{
			_galCore = gc;
		}

		public virtual void InitCommand()
		{
			_galCore.CurrentCommandList.Add(this);
		}

		public virtual void FinalCommand()
		{
			_galCore.CurrentCommandList.Remove(this);
		}

		public virtual void ConvertXml(XmlNode node, NodeInfo info)
		{
			if (node.Name != Command)
			{
				throw new ScriptCommandException(node, info);
			}
			_node = node;
			_info = info;
		}
	}
}
