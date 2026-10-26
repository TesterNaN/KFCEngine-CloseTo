using System;
using System.Drawing;
using KidFansChannel.GalCore;
using KidFansChannel.GalCore.Commands;
using KidFansChannel.Game2DCore;

namespace KidFansChannel.CloseTo.Commands
{
	internal class CloseToDialogue : Dialogue
	{
		private static Picture[] dialogueBacks = new Picture[5];

		private static Picture[] dialogueSpeakerBacks = new Picture[5];

		private static int currentColorIndex;

		private static string currentColor;

		private static float currentAlpha;

		private static int currentAlphaInt;

		private bool alphaChanged;

		protected void GameFont_FontChanged(object sender, EventArgs e)
		{
			if (Dialogue.String != null && Dialogue.String.Font != null)
			{
				Dialogue.String.Font = ScriptCommand.GalCore.GameFont.GetFont(24);
			}
			if (Dialogue.SpeakerString != null && Dialogue.SpeakerString.Font != null)
			{
				Dialogue.SpeakerString.Font = ScriptCommand.GalCore.GameFont.GetFont(24);
			}
		}

		protected override void ShowBack()
		{
			base.ShowBack();
			dialogueBacks[0].Alpha = currentAlphaInt;
			dialogueBacks[0].BeginTransform(0.1f);
			dialogueBacks[0].ScaleTo(new SizeF(1f, 1f));
			dialogueBacks[0].EndTransform();
			for (int i = 1; i < 5; i++)
			{
				dialogueBacks[i].BeginTransform(0.1f);
				dialogueBacks[i].ScaleTo(new SizeF(1f, 1f));
				dialogueBacks[i].EndTransform();
			}
		}

		protected override void HideBack()
		{
			base.HideBack();
			Picture[] array = dialogueBacks;
			foreach (Picture picture in array)
			{
				picture.BeginTransform(0.1f);
				picture.ScaleTo(SizeF.Empty);
				picture.EndTransform();
			}
		}

		protected override void ShowSpeakerBack()
		{
			base.ShowSpeakerBack();
			float scaleX;
			if (Dialogue.SpeakerString.Width <= 150)
			{
				scaleX = 1f;
			}
			else
			{
				scaleX = ((Dialogue.SpeakerString.Width > 250) ? 2f : 1.5f);
			}
			dialogueSpeakerBacks[0].Alpha = currentAlphaInt;
			for (int i = 1; i < 5; i++)
			{
				dialogueSpeakerBacks[i].Alpha = 255;
			}
			Picture[] array = dialogueSpeakerBacks;
			foreach (Picture picture in array)
			{
				picture.ScaleX = scaleX;
			}
		}

		protected override void ChangeSpeakerBack()
		{
			float scaleX;
			if (Dialogue.SpeakerString.Width <= 150)
			{
				scaleX = 1f;
			}
			else
			{
				scaleX = ((Dialogue.SpeakerString.Width > 250) ? 2f : 1.5f);
			}
			Picture[] array = dialogueSpeakerBacks;
			foreach (Picture picture in array)
			{
				picture.ScaleX = scaleX;
			}
		}

		protected override void HideSpeakerBack()
		{
			base.HideSpeakerBack();
			Picture[] array = dialogueSpeakerBacks;
			foreach (Picture picture in array)
			{
				picture.Alpha = 0;
			}
		}

		protected override void UpdateBack(float elaspedTime)
		{
			if (ScriptCommand.Options.DialogueAlpha != currentAlpha)
			{
				currentAlpha = ScriptCommand.Options.DialogueAlpha;
				currentAlphaInt = (int)(currentAlpha * 255f);
				dialogueBacks[0].Alpha = currentAlphaInt;
				alphaChanged = true;
			}
			Picture[] array = dialogueBacks;
			foreach (Picture picture in array)
			{
				picture.Update(elaspedTime);
			}
		}

		protected override void UpdateSpeakerBack(float elaspedTime)
		{
			if (alphaChanged)
			{
				dialogueSpeakerBacks[0].Alpha = currentAlphaInt;
				alphaChanged = false;
			}
			Picture[] array = dialogueSpeakerBacks;
			foreach (Picture picture in array)
			{
				picture.Update(elaspedTime);
			}
		}

		protected override bool CheckShown()
		{
			if (dialogueBacks[currentColorIndex].Time == 0f)
			{
				return dialogueBacks[currentColorIndex].Scale == new SizeF(1f, 1f);
			}
			return false;
		}

		protected override bool CheckHiden()
		{
			if (dialogueBacks[currentColorIndex].Time == 0f)
			{
				return dialogueBacks[currentColorIndex].Scale == SizeF.Empty;
			}
			return false;
		}

		protected override bool CheckSpeakerShown()
		{
			if (dialogueSpeakerBacks[currentColorIndex].Time == 0f)
			{
				return dialogueSpeakerBacks[currentColorIndex].Alpha == 255;
			}
			return false;
		}

		protected override bool CheckSpeakerHiden()
		{
			if (dialogueSpeakerBacks[currentColorIndex].Time == 0f)
			{
				return dialogueSpeakerBacks[currentColorIndex].Alpha == 0;
			}
			return false;
		}

		protected override void DrawBackGround()
		{
			ScriptCommand.Graphics.DrawPicture(dialogueBacks[0]);
			if (ScriptCommand.Options.DialogueColor != currentColor)
			{
				GetColorIndex();
			}
			ScriptCommand.Graphics.DrawPicture(dialogueBacks[currentColorIndex]);
		}

		protected override void DrawSpeakerBackGround()
		{
			float num;
			if (Dialogue.SpeakerString.Width <= 150)
			{
				num = 1f;
			}
			else
			{
				num = ((Dialogue.SpeakerString.Width > 250) ? 2f : 1.5f);
			}
			ScriptCommand.Graphics.DrawPicture(dialogueSpeakerBacks[0]);
			ScriptCommand.Graphics.DrawPicture(dialogueSpeakerBacks[currentColorIndex]);
			Dialogue.SpeakerString.Location = new Point((int)(-190f + 100f * num), 396);
		}

		public override void InitStaticData()
		{
			ScriptCommand.GalCore.GameFont.FontChanged += GameFont_FontChanged;
			Dialogue.String = new String2D();
			Dialogue.String.ClickLink += String_ClickLink;
			Dialogue.String.MoveLink += String_MoveLink;
			Dialogue.SpeakerString = new String2D();
			Dialogue.String.Font = ScriptCommand.GalCore.GameFont.GetFont(24);
			Dialogue.String.Location = new Point(40, 464);
			Dialogue.String.LineSize = new Size(720, 36);
			Dialogue.String.LineNum = 3;
			Dialogue.SpeakerString.Font = ScriptCommand.GalCore.GameFont.GetFont(24);
			Dialogue.SpeakerString.Location = new Point(10, 396);
			Dialogue.SpeakerString.LineSize = new Size(400, 40);
			Dialogue.SpeakerString.LineNum = 1;
			Dialogue.CurrentString = new String2D();
			Dialogue.CurrentString.Font = ScriptCommand.GalCore.GameFont.GetFont(24);
			Dialogue.CurrentString.Location = new Point(40, 464);
			Dialogue.CurrentString.LineSize = new Size(720, 36);
			Dialogue.CurrentString.LineNum = 3;
			for (int i = 0; i < 5; i++)
			{
				dialogueBacks[i] = ScriptCommand.Data.LoadPictureInstant("system", "dialogue" + i + ".png");
				dialogueBacks[i].Center = new Point(390, 75);
				dialogueBacks[i].Position = new Point(400, 515);
				dialogueBacks[i].Scale = SizeF.Empty;
				dialogueSpeakerBacks[i] = ScriptCommand.Data.LoadPictureInstant("system", "dialogue_speaker" + i + ".png");
				dialogueSpeakerBacks[i].Center = new Point(0, 0);
				dialogueSpeakerBacks[i].Position = new Point(10, 380);
				dialogueSpeakerBacks[i].Alpha = 0;
			}
			currentColor = ScriptCommand.Options.DialogueColor;
			GetColorIndex();
			currentAlpha = ScriptCommand.Options.DialogueAlpha;
			currentAlphaInt = (int)(currentAlpha * 255f);
		}

		private void String_MoveLink(object sender, String2D.LinkArgs e)
		{
			if (e.Value == -1)
			{
				((CloseToCore)ScriptCommand.GalCore).CursorActive = false;
			}
			else
			{
				((CloseToCore)ScriptCommand.GalCore).CursorActive = true;
			}
		}

		private void String_ClickLink(object sender, String2D.LinkArgs e)
		{
			((CloseToCore)ScriptCommand.GalCore).CursorActive = false;
		}

		public override void FinalStaticData()
		{
			base.FinalCommand();
			for (int i = 0; i < 5; i++)
			{
				dialogueBacks[i].Dispose();
				dialogueBacks[i] = null;
				dialogueSpeakerBacks[i].Dispose();
				dialogueSpeakerBacks[i] = null;
			}
		}

		private void GetColorIndex()
		{
			currentColor = ScriptCommand.Options.DialogueColor;
			if (currentColor == "Blue")
			{
				currentColorIndex = 1;
				return;
			}
			if (currentColor == "Green")
			{
				currentColorIndex = 2;
				return;
			}
			if (currentColor == "Pink")
			{
				currentColorIndex = 3;
				return;
			}
			if (currentColor == "Gray")
			{
				currentColorIndex = 4;
				return;
			}
			throw new Exception("Unbelievable!");
		}

		protected override float GetSpeed()
		{
			switch (ScriptCommand.Options.TextSpeed)
			{
			case GameOptions.Level.High:
				return 1000f;
			case GameOptions.Level.Mid:
				return 500f;
			case GameOptions.Level.Low:
				return 250f;
			case GameOptions.Level.None:
				return 2500f;
			default:
				return 500f;
			}
		}
	}
}
