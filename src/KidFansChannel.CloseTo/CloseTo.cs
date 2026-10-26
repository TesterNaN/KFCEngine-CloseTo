using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Resources;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using KidFansChannel.GalCore;
using KidFansChannel.GalCore.Records;
using KidFansChannel.GalCore.Screens;
using KidFansChannel.InputCore;

namespace KidFansChannel.CloseTo
{
	internal class CloseTo
	{
		private KidFansChannel.GalCore.GalCore galCore;

		private string saveFile;

		public static string AssemblyPath;

		private bool novideo;

		protected static int Drive_CDRom;

		static CloseTo()
		{
			Drive_CDRom = 5;
			try
			{
				AssemblyPath = Application.ExecutablePath;
			}
			catch (Exception)
			{
			}
		}

		public void Run(string[] args)
		{
			galCore = new CloseToCore();
			GameManager.GalCore = galCore;
			if (args.Length == 1 && File.Exists(args[0]))
			{
				saveFile = args[0];
			}
			if (args.Length == 1 && args[0] == "/novideo")
			{
				novideo = true;
			}
			InitializeCloseTo();
		}

		private void InitializeCloseTo()
		{
			Environment.CurrentDirectory = Path.GetDirectoryName(AssemblyPath);
			GameOptions gameOptions = new GameOptions();
			gameOptions.SettingFolder = "KID\\CloseTo";
			if (!File.Exists(gameOptions.GetOptionsFile()) && File.Exists("CloseToSetting.exe"))
			{
				Process process = Process.Start("CloseToSetting.exe");
				process.WaitForExit();
				while (!process.HasExited)
				{
				}
			}
			gameOptions.Load();
			galCore.SetGameOptions(gameOptions);
			galCore.SetGameGraphics(new GameGraphics());
			galCore.ShowGameWindow();
			GameData gameData = new GameData();
			gameData.SetCurrentDirectory(Environment.CurrentDirectory);
			if (!File.Exists("bg.dat") || !File.Exists("chara.dat") || !File.Exists("bgm.dat") || !File.Exists("se.dat") || !File.Exists("voice.dat"))
			{
				bool initialized = false;
				string[] logicalDrives = Directory.GetLogicalDrives();
				StringBuilder stringBuilder = new StringBuilder(256);
				StringBuilder lpFileSystemNameBuffer = new StringBuilder(256);
				for (int i = 0; i < logicalDrives.Length; i++)
				{
					int lpVolumeSerialNumber;
					int lpMaximumComponentLength;
					int lpFileSystemFlags;
					string path;
					if (GetDriveType(logicalDrives[i]) == Drive_CDRom && GetVolumeInformation(logicalDrives[i], stringBuilder, 256, out lpVolumeSerialNumber, out lpMaximumComponentLength, out lpFileSystemFlags, lpFileSystemNameBuffer, 256) != 0 && stringBuilder.ToString().StartsWith("CloseTo") && Directory.Exists(path = Path.Combine(logicalDrives[i], "CloseTo")) && File.Exists(Path.Combine(path, "bg.dat")) && File.Exists(Path.Combine(path, "chara.dat")) && File.Exists(Path.Combine(path, "bgm.dat")) && File.Exists(Path.Combine(path, "se.dat")) && File.Exists(Path.Combine(path, "voice.dat")))
					{
						gameData.AddDirectory(Path.Combine(logicalDrives[i], "CloseTo"));
						initialized = true;
						break;
					}
				}
			}
			galCore.SetGameData(gameData);
			GameLanguage gameLanguage = new GameLanguage();
			ResourceManager resourceManager = new ResourceManager(typeof(Resource));
			gameLanguage.SetResourceManager(resourceManager);
			galCore.SetGameLanguage(gameLanguage);
			gameData.AddData("system");
			GameFont gameFont = new GameFont();
			galCore.SetGameFont(gameFont);
			GameInput gameInput = new GameInput();
			galCore.SetGameInput(gameInput);
			if (!File.Exists(Path.Combine(Environment.SystemDirectory, "XInput9_1_0.dll")) && gameOptions.XInput)
			{
				gameOptions.XInput = false;
			}
			if (gameOptions.XInput)
			{
				gameInput.UseXInput = true;
				gameInput.SetGamePad(null);
			}
			else
			{
				List<GamePadInput.GamePadInstance> gamePadList = gameInput.GetGamePadList();
				gameInput.UseXInput = false;
				if (gamePadList.Count > gameOptions.InputDevice)
				{
					gameInput.SetGamePad(gamePadList[gameOptions.InputDevice]);
				}
				else
				{
					gameInput.SetGamePad(null);
				}
			}
			gameOptions.Language = gameLanguage.ChangeLanguage(gameOptions.Language);
			gameOptions.Font = gameFont.ChangeFont(gameOptions.Font);
			galCore.SetGameSound(new GameSound(galCore.GameGraphics.GameWindow, gameOptions.SoundDevice));
			if (!novideo)
			{
				galCore.SetGameVideo(new GameVideo());
			}
			Label.Init();
			GameState gameState = new CloseToState();
			gameState.SaveFolder = "KID\\CloseTo";
			gameState.SaveExt = "CloseToSave-kfc";
			gameState.InfoExt = "CloseToInfo-kfc";
			gameState.Guid = new Guid(((GuidAttribute)Attribute.GetCustomAttribute(typeof(CloseTo).Assembly, typeof(GuidAttribute))).Value);
			gameState.GameName = "Omoi no Kakera -Close to-";
			gameState.GameSaveName = gameLanguage.ResourceManager.GetString("SaveFileStr");
			gameState.ConfigString = gameLanguage.ResourceManager.GetString("ConfigStr");
			gameState.ConfigLevel = gameLanguage.ResourceManager.GetString("LevelStr");
			gameState.ConfigComment = gameLanguage.ResourceManager.GetString("CommentStr");
			Record.AddRecordType(typeof(TextRecord));
			Record.AddRecordType(typeof(SceneRecord));
			Record.AddRecordType(typeof(ChapterRecord));
			Record.AddRecordType(typeof(SelectRecord));
			Record.AddRecordType(typeof(DateTimeRecord));
			Record.AddRecordType(typeof(NullRecord));
			galCore.SetGameState(gameState);
			CloseToScript closeToScript = new CloseToScript();
			gameLanguage.ChangeLanguage(gameOptions.Language);
			Stream stream = new MemoryStream((byte[])resourceManager.GetObject("script_lang_dat"));
			gameData.AddReplacePair(string.Format("script.{0}.dat", gameLanguage.CurrentLanguage), stream);
			galCore.SetGameScript(closeToScript);
			if (saveFile != null)
			{
				closeToScript.LoadStory(saveFile);
			}
			if (closeToScript.CurrentGameScript == null)
			{
				if (saveFile != null)
				{
					gameState.LoadState(saveFile);
				}
				galCore.ShowLogoScreen();
			}
			MessageScreen.TextWithoutSpeakerWidth = 600;
			MessageScreen.TextWithSpeakerWidth = 480;
			MessageScreen.PageLineNum = 11;
			galCore.Initialize();
			galCore.MainLoop();
			Label.Final();
			Record.ClearRecordType();
		}

		[DllImport("kernel32.dll", EntryPoint = "GetVolumeInformationA")]
		protected static extern int GetVolumeInformation(string lpRootPathName, StringBuilder lpVolumeNameBuffer, int nVolumeNameSize, out int lpVolumeSerialNumber, out int lpMaximumComponentLength, out int lpFileSystemFlags, StringBuilder lpFileSystemNameBuffer, int nFileSystemNameSize);

		[DllImport("kernel32.dll", EntryPoint = "GetDriveTypeA")]
		protected static extern int GetDriveType(string nDrive);
	}
}
