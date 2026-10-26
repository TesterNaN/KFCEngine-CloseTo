using System.Collections.Generic;
using System.Drawing;
using KidFansChannel.DataCore;
using KidFansChannel.GalCore.Records;

namespace KidFansChannel.GalCore.Screens
{
	public abstract class MessageScreen : GameScreen
	{
		protected class MessageDisplay
		{
			public int Index;

			public int From;

			public int To;

			public int LinePos;

			public int LineNum;

			public string Text;

			public string Speaker;

			public string Voice;

			public Color Color = Color.Empty;

			public ContentAlignment Align = ContentAlignment.TopLeft;

			public static List<Color> SpeakerColor = new List<Color>();

			public static Color SelectTrueColor = Color.Empty;

			public static Color SelectFalseColor = Color.Empty;

			public MessageDisplay(Record record)
			{
				if (record is TextRecord)
				{
					int speakerNameLength = ((TextRecord)record).SpeakerNameLength;
					if (speakerNameLength == 0)
					{
						Speaker = null;
					}
					else
					{
						Speaker = record.Text.Substring(0, speakerNameLength);
					}
					Text = record.Text.Substring(speakerNameLength);
					if (record.HasVoice)
					{
						Voice = ((TextRecord)record).Voice;
					}
					if (((TextRecord)record).Speakers.Count > 0)
					{
						int num = ((TextRecord)record).Speakers[0];
						if (SpeakerColor.Count > num)
						{
							Color = SpeakerColor[num];
						}
					}
				}
				else if (record is SelectRecord)
				{
					if (((SelectRecord)record).Selected)
					{
						Color = SelectTrueColor;
					}
					else
					{
						Color = SelectFalseColor;
					}
					Text = record.Text;
					Align = ContentAlignment.TopCenter;
				}
				else
				{
					Text = record.Text;
				}
				LineNum = record.Line;
				From = 0;
				To = LineNum - 1;
			}
		}

		protected LinkedList<MessageDisplay> Messages = new LinkedList<MessageDisplay>();

		public static int PageLineNum = 10;

		public static int TextWithSpeakerWidth = 600;

		public static int TextWithoutSpeakerWidth = 700;

		protected bool IsVoicing
		{
			get
			{
				return GameManager.Sound.IsPlayingVoice;
			}
		}

		public float ScrollBarStart
		{
			get
			{
				if (Messages.Count == 0)
				{
					return 0f;
				}
				return ((float)GameManager.GameState.CurrentGameState.RecordLines[Messages.First.Value.Index] + (float)Messages.First.Value.From) / (float)GameManager.GameState.CurrentGameState.RecordLines[GameManager.GameState.CurrentGameState.Records.Count];
			}
		}

		public float ScrollBarLength
		{
			get
			{
				if (Messages.Count == 0)
				{
					return 1f;
				}
				int num = GameManager.GameState.CurrentGameState.RecordLines[GameManager.GameState.CurrentGameState.Records.Count];
				if (num > PageLineNum)
				{
					return (float)PageLineNum / (float)num;
				}
				return 1f;
			}
		}

		public int TotalRecordLineNum
		{
			get
			{
				return GameManager.GameState.CurrentGameState.RecordLines[GameManager.GameState.CurrentGameState.Records.Count];
			}
		}

		public override bool InitScreen()
		{
			if (!base.InitScreen())
			{
				return false;
			}
			int num = PageLineNum;
			int num2 = GameManager.GameState.CurrentGameState.Records.Count - 1;
			while (num > 0 && num2 >= 0)
			{
				MessageDisplay messageDisplay = new MessageDisplay(GameManager.GameState.CurrentGameState.Records[num2]);
				messageDisplay.Index = num2;
				messageDisplay.LinePos = num - messageDisplay.LineNum;
				if (messageDisplay.LinePos < 0)
				{
					messageDisplay.From = -messageDisplay.LinePos;
					messageDisplay.LinePos = 0;
					num = 0;
					Messages.AddFirst(messageDisplay);
					break;
				}
				num -= messageDisplay.LineNum;
				Messages.AddFirst(messageDisplay);
				num2--;
			}
			if (num2 < 0 && Messages.Count > 0)
			{
				int linePos = Messages.First.Value.LinePos;
				Move(-linePos);
			}
			return true;
		}

		private void Move(int offset)
		{
			foreach (MessageDisplay message in Messages)
			{
				message.LinePos += offset;
			}
		}

		protected void ScrollUp(int line)
		{
			if (Messages.Count != 0)
			{
				int num = Messages.Last.Value.LinePos + Messages.Last.Value.To - Messages.Last.Value.From + 1;
				int num2 = AddFirst(line);
				Move(num2);
				RemoveLast(num2 - (PageLineNum - num));
			}
		}

		private int AddFirst(int line)
		{
			int num = Messages.First.Value.From;
			int num2 = 0;
			if (num > 0)
			{
				if (num > line)
				{
					Messages.First.Value.From = num - line;
					Messages.First.Value.LinePos -= line;
					return num2 = line;
				}
				num2 += num;
				line -= num2;
				Messages.First.Value.From = 0;
				Messages.First.Value.LinePos -= num2;
			}
			int num3 = Messages.First.Value.Index - 1;
			while (line > 0 && num3 >= 0)
			{
				MessageDisplay messageDisplay = new MessageDisplay(GameManager.GameState.CurrentGameState.Records[num3]);
				messageDisplay.Index = num3;
				if (messageDisplay.LineNum <= line)
				{
					messageDisplay.LinePos = -num2 - messageDisplay.LineNum;
					line -= messageDisplay.LineNum;
					num2 += messageDisplay.LineNum;
					Messages.AddFirst(messageDisplay);
					num3--;
					continue;
				}
				messageDisplay.From = messageDisplay.LineNum - line;
				messageDisplay.LinePos = -num2 - line;
				num2 += line;
				line = 0;
				Messages.AddFirst(messageDisplay);
				break;
			}
			return num2;
		}

		private int RemoveLast(int line)
		{
			int num = 0;
			while (line > 0)
			{
				if (Messages.Last.Value.To >= line)
				{
					Messages.Last.Value.To -= line;
					num += line;
					line = 0;
					break;
				}
				int num2 = Messages.Last.Value.To - Messages.Last.Value.From + 1;
				num += num2;
				line -= num2;
				Messages.RemoveLast();
			}
			return num;
		}

		protected void ScrollDown(int line)
		{
			if (Messages.Count != 0)
			{
				int num = Messages.Last.Value.LinePos + Messages.Last.Value.To - Messages.Last.Value.From + 1;
				int num2 = AddLast(line);
				Move(-num2);
				RemoveFirst(num2 - (PageLineNum - num));
			}
		}

		private int AddLast(int line)
		{
			int num = Messages.Last.Value.LineNum - Messages.Last.Value.To - 1;
			int num2 = 0;
			if (num > 0)
			{
				if (num > line)
				{
					Messages.Last.Value.To = Messages.Last.Value.LineNum - (num - line) - 1;
					return num2 = line;
				}
				num2 += num;
				line -= num2;
				Messages.Last.Value.To = Messages.Last.Value.LineNum - 1;
			}
			int num3 = Messages.Last.Value.Index + 1;
			while (line > 0 && num3 < GameManager.GameState.CurrentGameState.Records.Count)
			{
				MessageDisplay messageDisplay = new MessageDisplay(GameManager.GameState.CurrentGameState.Records[num3]);
				messageDisplay.Index = num3;
				if (messageDisplay.LineNum <= line)
				{
					messageDisplay.LinePos = PageLineNum + num2;
					line -= messageDisplay.LineNum;
					num2 += messageDisplay.LineNum;
					Messages.AddLast(messageDisplay);
					num3++;
					continue;
				}
				messageDisplay.To = line - 1;
				messageDisplay.LinePos = PageLineNum + num2;
				num2 += line;
				line = 0;
				Messages.AddLast(messageDisplay);
				break;
			}
			return num2;
		}

		private int RemoveFirst(int line)
		{
			int num = 0;
			while (line > 0)
			{
				if (Messages.First.Value.LineNum - Messages.First.Value.From - 1 >= line)
				{
					Messages.First.Value.From += line;
					Messages.First.Value.LinePos += line;
					num += line;
					line = 0;
					break;
				}
				int num2 = Messages.First.Value.To - Messages.First.Value.From + 1;
				num += num2;
				line -= num2;
				Messages.RemoveFirst();
			}
			return num;
		}

		protected void PageUp()
		{
			ScrollUp(PageLineNum);
		}

		protected void PageDown()
		{
			ScrollDown(PageLineNum);
		}

		protected void MoveTo(int lineStart)
		{
			int num = -1;
			int num2 = 0;
			int num3 = 0;
			int num4 = GameManager.GameState.CurrentGameState.RecordLines.Count - 1;
			while (num4 - num3 > 1)
			{
				int num5 = (num3 + num4) / 2;
				if (GameManager.GameState.CurrentGameState.RecordLines[num5] > lineStart)
				{
					num4 = num5;
					continue;
				}
				if (GameManager.GameState.CurrentGameState.RecordLines[num5] < lineStart)
				{
					num3 = num5;
					continue;
				}
				num = num5;
				break;
			}
			if (num == -1)
			{
				num = num3;
				num2 = lineStart - GameManager.GameState.CurrentGameState.RecordLines[num];
			}
			int num6 = 0;
			Messages.Clear();
			if (num2 > 0)
			{
				MessageDisplay messageDisplay = new MessageDisplay(GameManager.GameState.CurrentGameState.Records[num]);
				messageDisplay.Index = num;
				messageDisplay.LinePos = 0;
				messageDisplay.From = num2;
				Messages.AddLast(messageDisplay);
				num6 += messageDisplay.LineNum - num2;
				num++;
			}
			while (num6 < PageLineNum && num < GameManager.GameState.CurrentGameState.Records.Count)
			{
				MessageDisplay messageDisplay2 = new MessageDisplay(GameManager.GameState.CurrentGameState.Records[num]);
				messageDisplay2.Index = num;
				messageDisplay2.LinePos = num6;
				num6 += messageDisplay2.LineNum;
				Messages.AddLast(messageDisplay2);
				num++;
			}
			if (num6 > PageLineNum)
			{
				Messages.Last.Value.To -= num6 - PageLineNum;
			}
		}

		protected bool FindVoiceUp()
		{
			int num = -1;
			for (LinkedListNode<MessageDisplay> linkedListNode = Messages.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
			{
				if (linkedListNode.Value.Voice != null)
				{
					num = linkedListNode.Value.Index;
					break;
				}
			}
			if (num == -1)
			{
				return false;
			}
			int num2 = num;
			for (int num3 = num - 1; num3 >= 0; num3--)
			{
				if (GameManager.GameState.CurrentGameState.Records[num3].HasVoice)
				{
					num2 = num3;
					break;
				}
			}
			if (num == num2)
			{
				return false;
			}
			int num4 = GameManager.GameState.CurrentGameState.RecordLines[num2] - (GameManager.GameState.CurrentGameState.RecordLines[Messages.First.Value.Index] + Messages.First.Value.From);
			ScrollUp(-num4);
			return true;
		}

		protected bool FindVoiceDown()
		{
			int num = -1;
			for (LinkedListNode<MessageDisplay> linkedListNode = Messages.Last; linkedListNode != null; linkedListNode = linkedListNode.Previous)
			{
				if (linkedListNode.Value.Voice != null)
				{
					num = linkedListNode.Value.Index;
					break;
				}
			}
			if (num == -1)
			{
				return false;
			}
			int num2 = num;
			for (int i = num + 1; i < GameManager.GameState.CurrentGameState.Records.Count; i++)
			{
				if (GameManager.GameState.CurrentGameState.Records[i].HasVoice)
				{
					num2 = i;
					break;
				}
			}
			if (num == num2)
			{
				return false;
			}
			int line = GameManager.GameState.CurrentGameState.RecordLines[num2] + GameManager.GameState.CurrentGameState.Records[num2].Line - (GameManager.GameState.CurrentGameState.RecordLines[Messages.Last.Value.Index] + Messages.Last.Value.To - Messages.Last.Value.From + 1);
			ScrollDown(line);
			return true;
		}

		protected void PlayVoice(string voice)
		{
			switch (GameManager.Data.CheckData("voice", voice + ".ogg", KidFansChannel.DataCore.DataCore.DataType.Package))
			{
			default:
				return;
			case KidFansChannel.DataCore.DataCore.DataType.File:
				GameManager.Sound.OpenVoice(GameManager.Data.GetFile(), true);
				break;
			case KidFansChannel.DataCore.DataCore.DataType.Package:
				GameManager.Sound.OpenVoice(GameManager.Data.GetStream(), true);
				break;
			}
			GameManager.Sound.PlayVoice();
		}

		protected void StopVoice()
		{
			GameManager.Sound.StopVoice();
		}
	}
}
