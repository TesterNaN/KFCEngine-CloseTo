using System.Windows.Forms;
using KidFansChannel.CloseTo.Commands;
using KidFansChannel.CloseTo.Effects;
using KidFansChannel.CloseTo.Screens;
using KidFansChannel.GalCore;
using KidFansChannel.GalCore.Commands;
using KidFansChannel.GalExt.DisplayStyles;
using KidFansChannel.GalExt.Effects;
using KidFansChannel.GalExt.TransformStyles;

namespace KidFansChannel.CloseTo
{
	internal class CloseToScript : GameScript
	{
		public CloseToScript()
		{
			QuickIcon = 4;
			TitleIcon = 5;
			SelectIcon = 6;
			QuickIndexStart = 60;
			QuickIndexEnd = 90;
		}

		protected override void PrepareTypes()
		{
			base.PrepareTypes();
			AssignTypeClass(typeof(CloseToClick));
			AssignTypeClass(typeof(CloseToDialogue));
			AssignTypeClass(typeof(CloseToParagraph));
			AssignTypeClass(typeof(CloseToSelect));
			AssignTypeClass(typeof(CloseToShowChapter));
			AssignTypeClass(typeof(CloseToEnding));
			AssignTypeClass(typeof(CloseToShowDate));
			AssignTypeClass(typeof(CloseToShowTime));
			AssignTypeClass(typeof(CloseToBgDisplay));
			CharaTransform.AddStyle(typeof(BottomWaveOneChara));
			CharaTransform.AddStyle(typeof(BottomWaveChara));
			CharaTransform.AddStyle(typeof(SmallSwingChara));
			CharaTransform.AddStyle(typeof(WaveFall1Chara));
			CharaTransform.AddStyle(typeof(WaveFall2Chara));
			CharaTransform.AddStyle(typeof(Shock1Chara));
			CharaTransform.AddStyle(typeof(Shock2Chara));
			CharaTransform.AddStyle(typeof(SwingChara));
			BgTransform.AddStyle(typeof(DropBg));
			BgTransform.AddStyle(typeof(Shock1Bg));
			BgTransform.AddStyle(typeof(Shock2Bg));
			BgTransform.AddStyle(typeof(ShockLoopBg));
			BgTransform.AddStyle(typeof(ShockLoopZoomInBg));
			BgDisplay.AddStyle(typeof(TransLeft));
			BgDisplay.AddStyle(typeof(TransRight));
			BgDisplay.AddStyle(typeof(TransUp));
			BgDisplay.AddStyle(typeof(TransDown));
			BgDisplay.AddStyle(typeof(BlindLeft));
			BgDisplay.AddStyle(typeof(CircleOut));
			BgDisplay.AddStyle(typeof(CircleOut));
			BgDisplay.AddStyle(typeof(CircleIn));
			EffectStart.AddEffect(typeof(Light));
			EffectStart.AddEffect(typeof(Dark));
			EffectStart.AddEffect(typeof(CloseToSun));
			EffectStart.AddEffect(typeof(Shock));
			EffectStart.AddEffect(typeof(CloseToFog));
			EffectStart.AddEffect(typeof(CloseToSakura));
			EffectStart.AddEffect(typeof(CloseToRain));
			EffectStart.AddEffect(typeof(CloseToFrame));
			EffectStart.AddEffect(typeof(ColorEft));
		}

		public override string GetStoryName(string story)
		{
			if (story == "CloseTo")
			{
				return GalCore.GameLanguage.ResourceManager.GetString("CloseToStr");
			}
			return story;
		}

		public override bool QuickSave(QuickSaveType type)
		{
			if (base.QuickSave(type))
			{
				((CloseToCore)GalCore).StartQuickSaveIcon();
				return true;
			}
			return false;
		}

		public override void QuickLoad()
		{
			GalCore.ShowMessageBox(new CloseToMessageBoxScreen(GalCore.GameLanguage.ResourceManager.GetString("LoadStr"), MessageBoxButtons.YesNo, CloseToQuickLoad, null));
		}

		private void CloseToQuickLoad(object[] empty)
		{
			base.QuickLoad();
			((CloseToCore)GalCore).ResetScreenElement();
		}

		public override bool SaveStory(int index, string comment, int icon)
		{
			if (base.SaveStory(index, comment, icon))
			{
				if (index < QuickIndexStart)
				{
					((CloseToState)GalCore.GameState).SaveStateNoPrompt();
				}
				return true;
			}
			return false;
		}

		protected override GameInstanceScript CreateInstanceScript(string storyName, bool jump)
		{
			return new CloseToInstanceScript(storyName, jump, this);
		}
	}
}
