using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using KidFansChannel.GalCore;
using KidFansChannel.GalCore.Screens;
using KidFansChannel.Game2DCore;

namespace KidFansChannel.CloseTo.Screens
{
	public class CloseToMessageScreen : MessageScreen
	{
		private Picture picVoiceBase;

		private Picture[] picVoices = new Picture[4];

		private Picture picVoiceAnime;

		private Picture[] picMains = new Picture[2];

		private Picture picBack;

		private String2D[] strTextWithouts = new String2D[MessageScreen.PageLineNum];

		private String2D[] strTextWiths = new String2D[MessageScreen.PageLineNum];

		private String2D[] strSpeakers = new String2D[MessageScreen.PageLineNum];

		private Element eleScreen;

		private CloseToScrollBarElement scrollBar = new CloseToScrollBarElement();

		private int lastVoiceIndex = -1;

		private float centerBack;

		public override bool DataReady
		{
			get
			{
				if (base.DataReady)
				{
					return true;
				}
				if (picMains[0] != null && picMains[0].IsCreated && picMains[1] != null && picMains[1].IsCreated && picVoiceBase != null && picVoiceBase.IsCreated && scrollBar.DataReady && picBack != null && picBack.IsCreated)
				{
					picMains[0].Alpha = (int)(GameManager.GalCore.GameOptions.DialogueAlpha * 255f + 0.5f);
					for (int i = 0; i < 4; i++)
					{
						picVoices[i] = GameManager.Graphics.CreateSubPicture(picVoiceBase, new Rectangle((i & 1) * 40, (i >> 1) * 40, 40, 40));
					}
					picVoiceAnime = GameManager.Graphics.CreateAnimePicture(new Size(40, 40));
					for (int j = 1; j < 4; j++)
					{
						((AnimePicture)picVoiceAnime).AddFrame(picVoices[j], 1f / 3f);
					}
					DataReady = true;
					return true;
				}
				return false;
			}
		}

		static CloseToMessageScreen()
		{
			MessageDisplay.SpeakerColor.Add(Color.White);
			MessageDisplay.SpeakerColor.Add(Color.Chocolate);
			MessageDisplay.SpeakerColor.Add(Color.ForestGreen);
			MessageDisplay.SpeakerColor.Add(Color.RoyalBlue);
			MessageDisplay.SpeakerColor.Add(Color.Gold);
			MessageDisplay.SpeakerColor.Add(Color.Olive);
			MessageDisplay.SpeakerColor.Add(Color.Silver);
			MessageDisplay.SelectTrueColor = Color.Aquamarine;
			MessageDisplay.SelectFalseColor = Color.LightSteelBlue;
		}

		public override void InitData()
		{
			base.InitData();
			picVoiceBase = GameManager.Data.LoadPicture("system", "voice.png");
			picMains[0] = GameManager.Data.LoadPicture("system", "message0.png");
			int num;
			switch (GameManager.GalCore.GameOptions.DialogueColor)
			{
			case "Blue":
				num = 1;
				break;
			case "Green":
				num = 2;
				break;
			case "Pink":
				num = 3;
				break;
			case "Gray":
				num = 4;
				break;
			default:
				throw new Exception("Unbelievable!");
			}
			picMains[1] = GameManager.Data.LoadPicture("system", "message" + num + ".png");
			picBack = GameManager.Graphics.CreatePicture(Color.FromArgb(128, Color.Black));
			Font2D font = GameManager.GalCore.GameFont.GetFont(24);
			for (int i = 0; i < MessageScreen.PageLineNum; i++)
			{
				strTextWithouts[i] = new String2D();
				strTextWithouts[i].Font = font;
				strTextWithouts[i].Location = Point.Empty;
				strTextWithouts[i].LineSize = new Size(600, 40);
				strTextWithouts[i].LineNum = 1;
			}
			for (int j = 0; j < MessageScreen.PageLineNum; j++)
			{
				strTextWiths[j] = new String2D();
				strTextWiths[j].Font = font;
				strTextWiths[j].Location = Point.Empty;
				strTextWiths[j].LineSize = new Size(480, 40);
				strTextWiths[j].LineNum = 1;
			}
			for (int k = 0; k < MessageScreen.PageLineNum; k++)
			{
				strSpeakers[k] = new String2D();
				strSpeakers[k].Font = font;
				strSpeakers[k].Location = Point.Empty;
				strSpeakers[k].LineSize = new Size(120, 40);
				strSpeakers[k].LineNum = 1;
			}
			scrollBar.InitData();
		}

		public override void FinalData()
		{
			base.FinalData();
			picBack.Dispose();
			picVoiceAnime.Dispose();
			for (int i = 0; i < 4; i++)
			{
				picVoices[i].Dispose();
			}
			picVoiceBase.Dispose();
			for (int j = 0; j < 2; j++)
			{
				picMains[j].Dispose();
			}
			scrollBar.FinalData();
		}

		public override bool InitScreen()
		{
			if (!base.InitScreen())
			{
				return false;
			}
			Element item = new Element(picVoices[0]);
			Element item2 = new Element(picVoices[3]);
			Element item3 = new Element(picVoiceAnime);
			Element element = new Element();
			element.Add(item, "normal", true);
			element.Add(item2, "select", false);
			element.Add(item3, "play", false);
			element.Region = new Rectangle(0, 0, 40, 40);
			Element element2 = new Element(picMains[0]);
			eleScreen = new Element(picBack);
			eleScreen.Add(element2, "main");
			element2.Add(new Element(picMains[1]), "main0");
			eleScreen = new Element(picBack);
			eleScreen.Add(element2);
			for (int i = 0; i < MessageScreen.PageLineNum; i++)
			{
				Element element3 = (Element)element.Clone();
				element3.Position = new Point(60, 100 + i * 40);
				element3.Value = i;
				eleScreen.Add(element3, "voice" + i.ToString("00"));
			}
			element.Dispose();
			Element[] array = new Element[MessageScreen.PageLineNum];
			Element[] array2 = new Element[MessageScreen.PageLineNum];
			Element[] array3 = new Element[MessageScreen.PageLineNum];
			Element element4 = new Element();
			for (int j = 0; j < MessageScreen.PageLineNum; j++)
			{
				array[j] = new Element(strTextWithouts[j]);
				array2[j] = new Element(strTextWiths[j]);
				array3[j] = new Element(strSpeakers[j]);
				element4.Add(array[j], "textwithout" + j.ToString("00"));
				element4.Add(array2[j], "textwith" + j.ToString("00"));
				element4.Add(array3[j], "speaker" + j.ToString("00"));
			}
			element4.Region = new Rectangle(0, 0, 600, 440);
			element4.Position = new Point(100, 100);
			eleScreen.Add(element4, "text");
			eleScreen.Focused = true;
			element4.Selected = true;
			FocusedElement = eleScreen;
			RootElement = eleScreen;
			eleScreen.Add(new Element(), "voiceup");
			eleScreen.Add(new Element(), "voicedown");
			scrollBar.Init();
			scrollBar.Position = new Point(755, 75);
			scrollBar.Size = 490;
			eleScreen.Add(scrollBar, "scrollbar");
			scrollBar.ValueChanged += ScrollBar_ValueChanged;
			for (int k = 0; k < MessageScreen.PageLineNum; k++)
			{
				eleScreen["voice" + k.ToString("00")].Right = eleScreen["text"];
				eleScreen["voice" + k.ToString("00")].MouseClick += VoiceUnit_MouseClick;
				eleScreen["voice" + k.ToString("00")].MouseEnter += VoiceUnit_MouseEnter;
				eleScreen["voice" + k.ToString("00")].MouseLeave += VoiceUnit_MouseLeave;
			}
			eleScreen["text"].KeyPress += Text_KeyPress;
			eleScreen["voiceup"].MouseEnter += VoiceUp_MouseEnter;
			eleScreen["voicedown"].MouseEnter += VoiceDown_MouseEnter;
			UpdateMessage();
			return true;
		}

		private void ScrollBar_ValueChanged(object sender, EventArgs e)
		{
			int num = (int)(((ScrollBarElement)sender).Start * (float)TotalRecordLineNum + 0.5f);
			if (num < 0)
			{
				num = 0;
			}
			if (num > TotalRecordLineNum - MessageScreen.PageLineNum)
			{
				num = TotalRecordLineNum - MessageScreen.PageLineNum;
				if (num < 0)
				{
					num = 0;
				}
			}
			MoveTo(num);
			UpdateMessage();
		}

		private void VoiceUp_MouseEnter(object sender, EventArgs e)
		{
			if (FindVoiceUp())
			{
				FindVoiceUp();
				UpdateMessage();
				eleScreen["voice00"].Selected = true;
				return;
			}
			for (int i = 0; i < MessageScreen.PageLineNum; i++)
			{
				if (eleScreen.GetVisible("voice" + i.ToString("00")))
				{
					eleScreen["voice" + i.ToString("00")].Selected = true;
					break;
				}
			}
		}

		private void VoiceDown_MouseEnter(object sender, EventArgs e)
		{
			if (FindVoiceDown())
			{
				UpdateMessage();
				eleScreen["voice" + (Messages.Count - 1).ToString("00")].Selected = true;
				return;
			}
			for (int num = MessageScreen.PageLineNum - 1; num >= 0; num--)
			{
				if (eleScreen.GetVisible("voice" + num.ToString("00")))
				{
					eleScreen["voice" + num.ToString("00")].Selected = true;
					break;
				}
			}
		}

		private void VoiceUnit_MouseClick(object sender, EventArgs e)
		{
			Element element = (Element)sender;
			LinkedListNode<MessageDisplay> linkedListNode;
			if (lastVoiceIndex != -1)
			{
				linkedListNode = Messages.First;
				for (int i = 0; i < MessageScreen.PageLineNum; i++)
				{
					if (linkedListNode.Value.Index == lastVoiceIndex)
					{
						eleScreen["voice" + i.ToString("00")].SetVisible("normal", true);
						eleScreen["voice" + i.ToString("00")].SetVisible("select", false);
						eleScreen["voice" + i.ToString("00")].SetVisible("play", false);
						break;
					}
					linkedListNode = linkedListNode.Next;
					if (linkedListNode == null)
					{
						break;
					}
				}
			}
			linkedListNode = Messages.First;
			for (int j = 0; j < (int)element.Value; j++)
			{
				linkedListNode = linkedListNode.Next;
			}
			PlayVoice(linkedListNode.Value.Voice);
			element.SetVisible("play", true);
			element.SetVisible("normal", false);
			element.SetVisible("select", false);
			((AnimePicture)element["play"].ElementPicture).ResetFrame();
			int num = (int)element.Value;
			linkedListNode = Messages.First;
			for (int k = 0; k < num; k++)
			{
				linkedListNode = linkedListNode.Next;
			}
			lastVoiceIndex = linkedListNode.Value.Index;
		}

		private void VoiceUnit_MouseEnter(object sender, EventArgs e)
		{
			Element element = (Element)sender;
			if (!element.GetVisible("play"))
			{
				element.SetVisible("select", true);
				element.SetVisible("normal", false);
				element.SetVisible("play", false);
			}
		}

		private void VoiceUnit_MouseLeave(object sender, EventArgs e)
		{
			Element element = (Element)sender;
			if (!element.GetVisible("play"))
			{
				element.SetVisible("normal", true);
				element.SetVisible("select", false);
				element.SetVisible("play", false);
			}
		}

		private void Text_KeyPress(object sender, KeyEventArgs e)
		{
			switch (e.KeyData)
			{
			case Keys.Up:
				ScrollUp(1);
				UpdateMessage();
				break;
			case Keys.Down:
				ScrollDown(1);
				UpdateMessage();
				break;
			case Keys.Prior:
				PageUp();
				UpdateMessage();
				break;
			case Keys.Next:
				PageDown();
				UpdateMessage();
				break;
			}
		}

		public override void OnMouseWheel(int delta)
		{
			base.OnMouseWheel(delta);
			if (delta > 0)
			{
				ScrollUp(delta / 120);
				UpdateMessage();
			}
			else if (delta < 0)
			{
				ScrollDown(-delta / 120);
				UpdateMessage();
			}
		}

		private void UpdateMessage()
		{
			int num = 0;
			int num2 = -1;
			foreach (MessageDisplay message in Messages)
			{
				if (message.Voice != null && message.From == 0)
				{
					eleScreen.SetVisible("voice" + num.ToString("00"), true);
					if (message.Index == lastVoiceIndex)
					{
						eleScreen["voice" + num.ToString("00")].SetVisible("normal", false);
						eleScreen["voice" + num.ToString("00")].SetVisible("select", false);
						eleScreen["voice" + num.ToString("00")].SetVisible("play", true);
					}
					else
					{
						if (eleScreen["voice" + num.ToString("00")].Selected)
						{
							eleScreen["voice" + num.ToString("00")].SetVisible("select", true);
							eleScreen["voice" + num.ToString("00")].SetVisible("normal", false);
						}
						else
						{
							eleScreen["voice" + num.ToString("00")].SetVisible("normal", true);
							eleScreen["voice" + num.ToString("00")].SetVisible("select", false);
						}
						eleScreen["voice" + num.ToString("00")].SetVisible("play", false);
					}
					if (num2 >= 0)
					{
						eleScreen["voice" + num.ToString("00")].Up = eleScreen["voice" + num2.ToString("00")];
						eleScreen["voice" + num2.ToString("00")].Down = eleScreen["voice" + num.ToString("00")];
					}
					else
					{
						eleScreen["voice" + num.ToString("00")].Up = eleScreen["voiceup"];
						eleScreen["text"].Left = eleScreen["voice" + num.ToString("00")];
					}
					num2 = num;
					eleScreen["voice" + num.ToString("00")].Position = new Point(60, 100 + message.LinePos * 40);
				}
				else
				{
					eleScreen.SetVisible("voice" + num.ToString("00"), false);
				}
				if (message.Speaker == null)
				{
					eleScreen["text"].SetVisible("textwithout" + num.ToString("00"), true);
					eleScreen["text"].SetVisible("textwith" + num.ToString("00"), false);
					eleScreen["text"].SetVisible("speaker" + num.ToString("00"), false);
					eleScreen["text"]["textwithout" + num.ToString("00")].Position = new Point(0, message.LinePos * 40);
					strTextWithouts[num].ClearString();
					strTextWithouts[num].AddString(message.Text, message.Align);
					strTextWithouts[num].LineNum = message.To - message.From + 1;
					strTextWithouts[num].CutLine(message.From, message.To);
					strTextWithouts[num].DefaultColor = ((message.Color == Color.Empty) ? Color.White : message.Color);
				}
				else
				{
					eleScreen["text"].SetVisible("textwith" + num.ToString("00"), true);
					eleScreen["text"].SetVisible("speaker" + num.ToString("00"), true);
					eleScreen["text"].SetVisible("textwithout" + num.ToString("00"), false);
					eleScreen["text"]["textwith" + num.ToString("00")].Position = new Point(120, message.LinePos * 40);
					eleScreen["text"]["speaker" + num.ToString("00")].Position = new Point(0, message.LinePos * 40);
					strTextWiths[num].ClearString();
					strTextWiths[num].AddString(message.Text);
					strTextWiths[num].LineNum = message.To - message.From + 1;
					strTextWiths[num].CutLine(message.From, message.To);
					strTextWiths[num].DefaultColor = ((message.Color == Color.Empty) ? Color.White : message.Color);
					strSpeakers[num].ClearString();
					strSpeakers[num].AddString(message.Speaker);
					strSpeakers[num].LineNum = message.To - message.From + 1;
					strSpeakers[num].CutLine(message.From, message.To);
					strSpeakers[num].DefaultColor = ((message.Color == Color.Empty) ? Color.White : message.Color);
				}
				num++;
			}
			for (int i = num; i < MessageScreen.PageLineNum; i++)
			{
				eleScreen.SetVisible("voice" + i.ToString("00"), false);
				eleScreen["text"].SetVisible("textwithout" + i.ToString("00"), false);
				eleScreen["text"].SetVisible("textwith" + i.ToString("00"), false);
				eleScreen["text"].SetVisible("speaker" + i.ToString("00"), false);
			}
			if (num2 == -1)
			{
				eleScreen["text"].Left = null;
			}
			else
			{
				eleScreen["voice" + num2.ToString("00")].Down = eleScreen["voicedown"];
			}
			eleScreen["text"].Selected = true;
			scrollBar.Start = ScrollBarStart;
			scrollBar.Length = ScrollBarLength;
		}

		public override bool FinalScreen()
		{
			if (!base.FinalScreen())
			{
				return false;
			}
			StopVoice();
			Element element = eleScreen["voice00"];
			for (int i = 0; i < element.Count; i++)
			{
				element[i].Dispose();
			}
			for (int j = 0; j < eleScreen["text"].Count; j++)
			{
				if (eleScreen["text"][j] != scrollBar)
				{
					eleScreen["text"][j].Dispose();
				}
			}
			scrollBar.Final();
			for (int k = 0; k < eleScreen.Count; k++)
			{
				eleScreen[k].Dispose();
			}
			eleScreen.Dispose();
			eleScreen = null;
			FinalData();
			Finished = true;
			FocusedElement = null;
			RootElement = null;
			return true;
		}

		public override void Update(float elaspedTime)
		{
			centerBack += elaspedTime * 80f;
			while (centerBack >= 80f)
			{
				centerBack -= 80f;
			}
			picBack.Center = new Point((int)(centerBack + 0.5f), (int)(centerBack + 0.5f));
			if (lastVoiceIndex != -1 && !IsVoicing)
			{
				LinkedListNode<MessageDisplay> linkedListNode = Messages.First;
				for (int i = 0; i < MessageScreen.PageLineNum; i++)
				{
					if (linkedListNode.Value.Index == lastVoiceIndex)
					{
						if (eleScreen["voice" + i.ToString("00")].Selected)
						{
							eleScreen["voice" + i.ToString("00")].SetVisible("select", true);
							eleScreen["voice" + i.ToString("00")].SetVisible("normal", false);
						}
						else
						{
							eleScreen["voice" + i.ToString("00")].SetVisible("normal", true);
							eleScreen["voice" + i.ToString("00")].SetVisible("select", false);
						}
						eleScreen["voice" + i.ToString("00")].SetVisible("play", false);
						break;
					}
					linkedListNode = linkedListNode.Next;
					if (linkedListNode == null)
					{
						break;
					}
				}
				lastVoiceIndex = -1;
			}
			base.Update(elaspedTime);
		}
	}
}
