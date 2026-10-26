using System.IO;
using System.Xml;

namespace KidFansChannel.GalCore.Commands
{
	public class DisSave : ScriptCommand, ISaveable
	{
		private bool _start;

		private bool _save;

		private bool previousExist;

		public bool Start
		{
			get
			{
				return _start;
			}
			set
			{
				_start = value;
			}
		}

		public DisSave()
		{
			Single = false;
			Lasting = false;
			Command = "dissave";
		}

		public override void ConvertXml(XmlNode node, NodeInfo info)
		{
			base.ConvertXml(node, info);
			_start = ScriptHelper.GetArgumentBool(node, info, "start");
		}

		public override void InitCommand()
		{
			base.InitCommand();
			DisSave disSave = null;
			foreach (ScriptCommand command in ScriptCommand.CommandList)
			{
				if (command is DisSave && command != this)
				{
					disSave = (DisSave)command;
					break;
				}
			}
			if (_start)
			{
				if (disSave != null)
				{
					FinishCommand();
				}
				ScriptCommand.GalCore.GameState.CurrentGameState.Update(ScriptCommand.GalCore.CurrentCommandList);
				if (!previousExist)
				{
					if ((ScriptCommand.Options.QuickSave & 2) == 2)
					{
						ScriptCommand.CommandList.Remove(this);
						ScriptCommand.GalCore.GameScript.QuickSave(QuickSaveType.Title);
						ScriptCommand.CommandList.Add(this);
					}
					else
					{
						ScriptCommand.CommandList.Remove(this);
						ScriptCommand.GalCore.GameScript.SaveStory(ScriptCommand.GalCore.GameScript.QuickIndexEnd, null, ScriptCommand.GalCore.GameScript.QuickIcon);
						ScriptCommand.CommandList.Add(this);
					}
				}
			}
			else
			{
				if (disSave == null)
				{
					throw new RuntimeException(Node, Info, "No dissave is running.");
				}
				disSave.FinishCommand();
				FinishCommand();
			}
		}

		void ISaveable.Save(BinaryWriter bw)
		{
			bw.Write(Command);
		}

		public void Load(BinaryReader br)
		{
			previousExist = true;
			_start = true;
		}

		public void ConfirmUpdate()
		{
		}
	}
}
