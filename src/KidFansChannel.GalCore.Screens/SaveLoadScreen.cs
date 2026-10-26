using System;
using KidFansChannel.Game2DCore;

namespace KidFansChannel.GalCore.Screens
{
	public abstract class SaveLoadScreen : GameScreen
	{
		protected int[] Icons;

		protected Picture picSnap;

		protected string Comment;

		protected string Level;

		protected DateTime SaveTime;

		public bool Finish;

		private int lastIndex = -1;

		public abstract bool IsSave { get; }

		public virtual bool CanDrop
		{
			get
			{
				return true;
			}
		}

		public void Save(int index, string comment, int icon)
		{
			GameManager.GameScript.SaveStory(index, comment, icon);
			FinalScreen();
		}

		public void Load(int index)
		{
			GameManager.GameScript.LoadStory(index);
			FinalScreen();
		}

		public override bool InitScreen()
		{
			if (!base.InitScreen())
			{
				return false;
			}
			Icons = new int[GameManager.GameScript.QuickIndexEnd];
			for (int i = 0; i < GameManager.GameScript.QuickIndexEnd; i++)
			{
				GameManager.GameState.GetSaveInfo(i, out Icons[i]);
			}
			return true;
		}

		public void GetInfo(int index)
		{
			GetInfo(index, false);
		}

		public void GetInfo(int index, bool always)
		{
			if (index != lastIndex || always)
			{
				if (picSnap != null)
				{
					picSnap.Dispose();
					picSnap = null;
				}
				GameManager.GameState.GetSaveInfo(index, out Icons[index], out Comment, out Level, out SaveTime, out picSnap);
				lastIndex = index;
			}
		}
	}
}
