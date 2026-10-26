using System.Windows.Forms;
using KidFansChannel.CloseTo.Screens;
using KidFansChannel.GalCore;
using KidFansChannel.GalCore.Commands;
using KidFansChannel.GalCore.Screens;

namespace KidFansChannel.CloseTo.Commands
{
	internal class CloseToEnding : Ending
	{
		public CloseToEnding()
		{
			Single = true;
			Lasting = true;
		}

		public override void InitCommand()
		{
			base.InitCommand();
			bool flag = true;
			switch (EndingIndex)
			{
			case 0:
				ScriptCommand.GalCore.GameState.UpdateAlbum(126);
				ScriptCommand.GalCore.GameState.UpdateAlbum(130);
				ScriptCommand.GalCore.GameState.UpdateAlbum(132);
				ScriptCommand.GalCore.GameState.UpdateAlbum(103);
				ScriptCommand.GalCore.GameState.UpdateAlbum(104);
				ScriptCommand.GalCore.GameState.UpdateAlbum(105);
				ScriptCommand.GalCore.GameState.UpdateAlbum(106);
				ScriptCommand.GalCore.GameState.UpdateAlbum(107);
				ScriptCommand.GalCore.GameState.UpdateAlbum(108);
				ScriptCommand.GalCore.GameState.UpdateAlbum(109);
				ScriptCommand.GalCore.GameState.UpdateAlbum(110);
				ScriptCommand.GalCore.GameState.UpdateMovie(13);
				if (!ScriptCommand.GalCore.GameState.CheckEnding(0))
				{
					KidFansChannel.GalCore.GalCore galCore2 = ScriptCommand.GalCore;
					string text2 = ScriptCommand.GalCore.GameLanguage.ResourceManager.GetString("OpenMaiStr");
					MessageBoxScreen.MessageBoxCallback callBack2 = Finish;
					object[] callBackArgs2 = new object[1];
					galCore2.ShowMessageBox(new CloseToMessageBoxScreen(text2, MessageBoxButtons.OK, true, callBack2, callBackArgs2));
					flag = false;
				}
				ScriptCommand.GalCore.GameState.UpdateMusic(31);
				if (ScriptCommand.GalCore.GameState.CheckEnding(1))
				{
					ScriptCommand.GalCore.GameState.UpdateMusic(32);
				}
				break;
			case 1:
				ScriptCommand.GalCore.GameState.UpdateAlbum(127);
				ScriptCommand.GalCore.GameState.UpdateAlbum(133);
				ScriptCommand.GalCore.GameState.UpdateAlbum(111);
				ScriptCommand.GalCore.GameState.UpdateAlbum(112);
				ScriptCommand.GalCore.GameState.UpdateAlbum(113);
				ScriptCommand.GalCore.GameState.UpdateAlbum(114);
				ScriptCommand.GalCore.GameState.UpdateAlbum(115);
				ScriptCommand.GalCore.GameState.UpdateAlbum(116);
				ScriptCommand.GalCore.GameState.UpdateAlbum(117);
				ScriptCommand.GalCore.GameState.UpdateMovie(13);
				if (CheckToAllClear())
				{
					KidFansChannel.GalCore.GalCore galCore5 = ScriptCommand.GalCore;
					string text5 = ScriptCommand.GalCore.GameLanguage.ResourceManager.GetString("OpenGhostStr");
					MessageBoxScreen.MessageBoxCallback callBack5 = Finish;
					object[] callBackArgs5 = new object[1];
					galCore5.ShowMessageBox(new CloseToMessageBoxScreen(text5, MessageBoxButtons.OK, true, callBack5, callBackArgs5));
					flag = false;
				}
				if (ScriptCommand.GalCore.GameState.CheckEnding(0))
				{
					ScriptCommand.GalCore.GameState.UpdateMusic(32);
				}
				break;
			case 2:
				if (CheckToAllClear())
				{
					KidFansChannel.GalCore.GalCore galCore3 = ScriptCommand.GalCore;
					string text3 = ScriptCommand.GalCore.GameLanguage.ResourceManager.GetString("OpenGhostStr");
					MessageBoxScreen.MessageBoxCallback callBack3 = Finish;
					object[] callBackArgs3 = new object[1];
					galCore3.ShowMessageBox(new CloseToMessageBoxScreen(text3, MessageBoxButtons.OK, true, callBack3, callBackArgs3));
					flag = false;
				}
				break;
			case 3:
			case 4:
				ScriptCommand.GalCore.GameState.UpdateAlbum(128);
				ScriptCommand.GalCore.GameState.UpdateAlbum(134);
				ScriptCommand.GalCore.GameState.UpdateAlbum(118);
				ScriptCommand.GalCore.GameState.UpdateAlbum(119);
				ScriptCommand.GalCore.GameState.UpdateAlbum(120);
				ScriptCommand.GalCore.GameState.UpdateAlbum(121);
				ScriptCommand.GalCore.GameState.UpdateMovie(13);
				if (CheckToAllClear())
				{
					KidFansChannel.GalCore.GalCore galCore4 = ScriptCommand.GalCore;
					string text4 = ScriptCommand.GalCore.GameLanguage.ResourceManager.GetString("OpenGhostStr");
					MessageBoxScreen.MessageBoxCallback callBack4 = Finish;
					object[] callBackArgs4 = new object[1];
					galCore4.ShowMessageBox(new CloseToMessageBoxScreen(text4, MessageBoxButtons.OK, true, callBack4, callBackArgs4));
					flag = false;
				}
				break;
			case 5:
				ScriptCommand.GalCore.GameState.UpdateAlbum(129);
				ScriptCommand.GalCore.GameState.UpdateAlbum(135);
				ScriptCommand.GalCore.GameState.UpdateAlbum(122);
				ScriptCommand.GalCore.GameState.UpdateAlbum(123);
				ScriptCommand.GalCore.GameState.UpdateAlbum(124);
				ScriptCommand.GalCore.GameState.UpdateAlbum(125);
				ScriptCommand.GalCore.GameState.UpdateMovie(13);
				if (CheckToAllClear())
				{
					KidFansChannel.GalCore.GalCore galCore = ScriptCommand.GalCore;
					string text = ScriptCommand.GalCore.GameLanguage.ResourceManager.GetString("OpenGhostStr");
					MessageBoxScreen.MessageBoxCallback callBack = Finish;
					object[] callBackArgs = new object[1];
					galCore.ShowMessageBox(new CloseToMessageBoxScreen(text, MessageBoxButtons.OK, true, callBack, callBackArgs));
					flag = false;
				}
				ScriptCommand.GalCore.GameState.UpdateMusic(29);
				break;
			case 6:
				ScriptCommand.GalCore.GameState.UpdateAlbum(131);
				ScriptCommand.GalCore.GameState.UpdateMovie(13);
				ScriptCommand.GalCore.GameState.UpdateMusic(19);
				ScriptCommand.GalCore.GameState.UpdateMusic(30);
				break;
			}
			bool flag2 = true;
			for (int i = 0; i < 7; i++)
			{
				if (EndingIndex != i && !ScriptCommand.GalCore.GameState.CheckEnding(i))
				{
					flag2 = false;
					break;
				}
			}
			if (flag2)
			{
				for (int j = 22; j < 28; j++)
				{
					ScriptCommand.GalCore.GameState.UpdateMusic(j);
				}
			}
			ScriptCommand.GalCore.GameState.UpdateMusic(28);
			UpdateEnding();
			if (flag)
			{
				FinishCommand();
			}
		}

		protected bool CheckToAllClear()
		{
			bool[] array = new bool[6];
			for (int i = 0; i < 6; i++)
			{
				array[i] = ScriptCommand.GalCore.GameState.CheckEnding(i);
			}
			if (array[0] && (array[1] || array[2]) && (array[3] || array[4]) && array[5])
			{
				return false;
			}
			array[EndingIndex] = true;
			if (array[0] && (array[1] || array[2]) && (array[3] || array[4]) && array[5])
			{
				return true;
			}
			return false;
		}

		private void Finish(object[] indexAndIcon)
		{
			FinishCommand();
		}
	}
}
