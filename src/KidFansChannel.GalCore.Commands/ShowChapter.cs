using System.Drawing;
using System.Xml;
using KidFansChannel.GalCore.Records;

namespace KidFansChannel.GalCore.Commands
{
	public abstract class ShowChapter : ScriptCommand, IStaticDataUser, IDrawable, IUpdatable, IRecordable
	{
		private String2D _chapter;

		private bool _hide;

		public float ShowHideTime = 1f;

		public float LastingTime = 5f;

		private float alpha;

		private float time;

		public static string AboveChapter = "----------------------------------------";

		public static string BelowChapter = "----------------------------------------";

		private bool _canRecord;

		public String2D Chapter
		{
			get
			{
				return _chapter;
			}
			set
			{
				_chapter = value;
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
				return 29;
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

		public ShowChapter()
		{
			Single = false;
			Lasting = false;
			Command = "showchapter";
		}

		public override void ConvertXml(XmlNode node, NodeInfo info)
		{
			base.ConvertXml(node, info);
			if (!node.HasChildNodes || node.ChildNodes.Count != 1 || node.FirstChild.NodeType != XmlNodeType.Text)
			{
				throw new ScriptStructureException(node, info, "Showchapter should have 1 text child node.");
			}
			if (node.FirstChild.Value.Contains("\n") || node.FirstChild.Value.Contains("\r"))
			{
				throw new ScriptStructureException(node, info, "Showchapter string should be in one line.");
			}
			_chapter = new String2D();
			SetFontStyle(_chapter);
			_chapter.AddString(node.FirstChild.Value, ContentAlignment.TopLeft);
			_hide = ScriptHelper.GetArgumentBool(node, info, "hide", false);
			_canRecord = true;
		}

		protected abstract void SetFontStyle(String2D chapter);

		public override void InitCommand()
		{
			base.InitCommand();
			foreach (ScriptCommand command in ScriptCommand.CommandList)
			{
				if (command is ShowChapter && command != this)
				{
					command.FinishCommand();
					break;
				}
			}
			if (ScriptCommand.Options.TitleShow && !_hide)
			{
				alpha = 0f;
				time = 0f;
			}
			else
			{
				FinishCommand();
			}
		}

		public override void FinalCommand()
		{
			_chapter = null;
			base.FinalCommand();
		}

		public void Draw()
		{
			_chapter.Canvas = ScriptCommand.GalCore.CurrentCanvas;
			_chapter.DrawString(alpha);
		}

		public void Update(double appTime, float elaspedTime)
		{
			if (!ScriptCommand.Options.TitleShow)
			{
				alpha = 0f;
				FinishCommand();
				return;
			}
			time += elaspedTime;
			if (time < ShowHideTime)
			{
				alpha = time / ShowHideTime;
				return;
			}
			if (time < ShowHideTime + LastingTime)
			{
				alpha = 1f;
				return;
			}
			if (time < 2f * ShowHideTime + LastingTime)
			{
				alpha = (2f * ShowHideTime + LastingTime - time) / ShowHideTime;
				return;
			}
			alpha = 0f;
			FinishCommand();
		}

		public void InitStaticData()
		{
		}

		public void FinalStaticData()
		{
		}

		public Record[] GetRecord()
		{
			if (_canRecord)
			{
				_canRecord = false;
				ChapterRecord chapterRecord = new ChapterRecord();
				chapterRecord.Title = ScriptCommand.GameState.CurrentGameState.Chapter;
				chapterRecord.Text = _chapter.String;
				NullRecord nullRecord = new NullRecord();
				nullRecord.Text = AboveChapter;
				NullRecord nullRecord2 = new NullRecord();
				nullRecord2.Text = BelowChapter;
				return new Record[3] { nullRecord, chapterRecord, nullRecord2 };
			}
			return null;
		}
	}
}
