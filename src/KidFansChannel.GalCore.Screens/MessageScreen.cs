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
						int speakerIndex = ((TextRecord)record).Speakers[0];
						if (SpeakerColor.Count > speakerIndex)
						{
							Color = SpeakerColor[speakerIndex];
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
				int totalLines = GameManager.GameState.CurrentGameState.RecordLines[GameManager.GameState.CurrentGameState.Records.Count];
				if (totalLines > PageLineNum)
				{
					return (float)PageLineNum / (float)totalLines;
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
			int remainingLines = PageLineNum;
			int recordIndex = GameManager.GameState.CurrentGameState.Records.Count - 1;
			while (remainingLines > 0 && recordIndex >= 0)
			{
				MessageDisplay messageDisplay = new MessageDisplay(GameManager.GameState.CurrentGameState.Records[recordIndex]);
				messageDisplay.Index = recordIndex;
				messageDisplay.LinePos = remainingLines - messageDisplay.LineNum;
				if (messageDisplay.LinePos < 0)
				{
					messageDisplay.From = -messageDisplay.LinePos;
					messageDisplay.LinePos = 0;
					remainingLines = 0;
					Messages.AddFirst(messageDisplay);
					break;
				}
				remainingLines -= messageDisplay.LineNum;
				Messages.AddFirst(messageDisplay);
				recordIndex--;
			}
			if (recordIndex < 0 && Messages.Count > 0)
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
				int shortfall = Messages.Last.Value.LinePos + Messages.Last.Value.To - Messages.Last.Value.From + 1;
				int filled = AddFirst(line);
				Move(filled);
				RemoveLast(filled - (PageLineNum - shortfall));
			}
		}

		private int AddFirst(int line)
		{
			int shortfall = Messages.First.Value.From;
			int filled = 0;
			if (shortfall > 0)
			{
				if (shortfall > line)
				{
					Messages.First.Value.From = shortfall - line;
					Messages.First.Value.LinePos -= line;
					return filled = line;
				}
				filled += shortfall;
				line -= filled;
				Messages.First.Value.From = 0;
				Messages.First.Value.LinePos -= filled;
			}
			int historyIndex = Messages.First.Value.Index - 1;
			while (line > 0 && historyIndex >= 0)
			{
				MessageDisplay messageDisplay = new MessageDisplay(GameManager.GameState.CurrentGameState.Records[historyIndex]);
				messageDisplay.Index = historyIndex;
				if (messageDisplay.LineNum <= line)
				{
					messageDisplay.LinePos = -filled - messageDisplay.LineNum;
					line -= messageDisplay.LineNum;
					filled += messageDisplay.LineNum;
					Messages.AddFirst(messageDisplay);
					historyIndex--;
					continue;
				}
				messageDisplay.From = messageDisplay.LineNum - line;
				messageDisplay.LinePos = -filled - line;
				filled += line;
				line = 0;
				Messages.AddFirst(messageDisplay);
				break;
			}
			return filled;
		}

		private int RemoveLast(int line)
		{
			int total = 0;
			while (line > 0)
			{
				if (Messages.Last.Value.To >= line)
				{
					Messages.Last.Value.To -= line;
					total += line;
					line = 0;
					break;
				}
				int tailLines = Messages.Last.Value.To - Messages.Last.Value.From + 1;
				total += tailLines;
				line -= tailLines;
				Messages.RemoveLast();
			}
			return total;
		}

		protected void ScrollDown(int line)
		{
			if (Messages.Count != 0)
			{
				int shortfall = Messages.Last.Value.LinePos + Messages.Last.Value.To - Messages.Last.Value.From + 1;
				int filled = AddLast(line);
				Move(-filled);
				RemoveFirst(filled - (PageLineNum - shortfall));
			}
		}

		private int AddLast(int line)
		{
			int shortfall = Messages.Last.Value.LineNum - Messages.Last.Value.To - 1;
			int filled = 0;
			if (shortfall > 0)
			{
				if (shortfall > line)
				{
					Messages.Last.Value.To = Messages.Last.Value.LineNum - (shortfall - line) - 1;
					return filled = line;
				}
				filled += shortfall;
				line -= filled;
				Messages.Last.Value.To = Messages.Last.Value.LineNum - 1;
			}
			int historyIndex = Messages.Last.Value.Index + 1;
			while (line > 0 && historyIndex < GameManager.GameState.CurrentGameState.Records.Count)
			{
				MessageDisplay messageDisplay = new MessageDisplay(GameManager.GameState.CurrentGameState.Records[historyIndex]);
				messageDisplay.Index = historyIndex;
				if (messageDisplay.LineNum <= line)
				{
					messageDisplay.LinePos = PageLineNum + filled;
					line -= messageDisplay.LineNum;
					filled += messageDisplay.LineNum;
					Messages.AddLast(messageDisplay);
					historyIndex++;
					continue;
				}
				messageDisplay.To = line - 1;
				messageDisplay.LinePos = PageLineNum + filled;
				filled += line;
				line = 0;
				Messages.AddLast(messageDisplay);
				break;
			}
			return filled;
		}

		private int RemoveFirst(int line)
		{
			int total = 0;
			while (line > 0)
			{
				if (Messages.First.Value.LineNum - Messages.First.Value.From - 1 >= line)
				{
					Messages.First.Value.From += line;
					Messages.First.Value.LinePos += line;
					total += line;
					line = 0;
					break;
				}
				int tailLines = Messages.First.Value.To - Messages.First.Value.From + 1;
				total += tailLines;
				line -= tailLines;
				Messages.RemoveFirst();
			}
			return total;
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
			int recordIndex = -1;
			int offsetInRecord = 0;
			int low = 0;
			int high = GameManager.GameState.CurrentGameState.RecordLines.Count - 1;
			while (high - low > 1)
			{
				int mid = (low + high) / 2;
				if (GameManager.GameState.CurrentGameState.RecordLines[mid] > lineStart)
				{
					high = mid;
					continue;
				}
				if (GameManager.GameState.CurrentGameState.RecordLines[mid] < lineStart)
				{
					low = mid;
					continue;
				}
				recordIndex = mid;
				break;
			}
			if (recordIndex == -1)
			{
				recordIndex = low;
				offsetInRecord = lineStart - GameManager.GameState.CurrentGameState.RecordLines[recordIndex];
			}
			int usedLines = 0;
			Messages.Clear();
			if (offsetInRecord > 0)
			{
				MessageDisplay messageDisplay = new MessageDisplay(GameManager.GameState.CurrentGameState.Records[recordIndex]);
				messageDisplay.Index = recordIndex;
				messageDisplay.LinePos = 0;
				messageDisplay.From = offsetInRecord;
				Messages.AddLast(messageDisplay);
				usedLines += messageDisplay.LineNum - offsetInRecord;
				recordIndex++;
			}
			while (usedLines < PageLineNum && recordIndex < GameManager.GameState.CurrentGameState.Records.Count)
			{
				MessageDisplay messageDisplay2 = new MessageDisplay(GameManager.GameState.CurrentGameState.Records[recordIndex]);
				messageDisplay2.Index = recordIndex;
				messageDisplay2.LinePos = usedLines;
				usedLines += messageDisplay2.LineNum;
				Messages.AddLast(messageDisplay2);
				recordIndex++;
			}
			if (usedLines > PageLineNum)
			{
				Messages.Last.Value.To -= usedLines - PageLineNum;
			}
		}

		protected bool FindVoiceUp()
		{
			int recordIndex = -1;
			for (LinkedListNode<MessageDisplay> linkedListNode = Messages.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
			{
				if (linkedListNode.Value.Voice != null)
				{
					recordIndex = linkedListNode.Value.Index;
					break;
				}
			}
			if (recordIndex == -1)
			{
				return false;
			}
			int voiceIndex = recordIndex;
			for (int probeIndex = recordIndex - 1; probeIndex >= 0; probeIndex--)
			{
				if (GameManager.GameState.CurrentGameState.Records[probeIndex].HasVoice)
				{
					voiceIndex = probeIndex;
					break;
				}
			}
			if (recordIndex == voiceIndex)
			{
				return false;
			}
			int scrollAmount = GameManager.GameState.CurrentGameState.RecordLines[voiceIndex] - (GameManager.GameState.CurrentGameState.RecordLines[Messages.First.Value.Index] + Messages.First.Value.From);
			ScrollUp(-scrollAmount);
			return true;
		}

		protected bool FindVoiceDown()
		{
			int startIndex = -1;
			for (LinkedListNode<MessageDisplay> linkedListNode = Messages.Last; linkedListNode != null; linkedListNode = linkedListNode.Previous)
			{
				if (linkedListNode.Value.Voice != null)
				{
					startIndex = linkedListNode.Value.Index;
					break;
				}
			}
			if (startIndex == -1)
			{
				return false;
			}
			int nextIndex = startIndex;
			for (int i = startIndex + 1; i < GameManager.GameState.CurrentGameState.Records.Count; i++)
			{
				if (GameManager.GameState.CurrentGameState.Records[i].HasVoice)
				{
					nextIndex = i;
					break;
				}
			}
			if (startIndex == nextIndex)
			{
				return false;
			}
			int line = GameManager.GameState.CurrentGameState.RecordLines[nextIndex] + GameManager.GameState.CurrentGameState.Records[nextIndex].Line - (GameManager.GameState.CurrentGameState.RecordLines[Messages.Last.Value.Index] + Messages.Last.Value.To - Messages.Last.Value.From + 1);
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
