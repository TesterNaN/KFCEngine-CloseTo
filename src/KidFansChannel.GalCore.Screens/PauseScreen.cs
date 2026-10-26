namespace KidFansChannel.GalCore.Screens
{
	public abstract class PauseScreen : GameScreen
	{
		public GameState.DrawStateItem Item
		{
			get
			{
				return GameManager.GameState.Item;
			}
		}

		public override bool Hide
		{
			get
			{
				return true;
			}
		}

		public override bool InitScreen()
		{
			if (!base.InitScreen())
			{
				return false;
			}
			GameManager.GameState.UpdateDrawState();
			return true;
		}

		protected void ExitGame()
		{
			FinalScreen();
			GameManager.GalCore.ReturnTitleScreen();
		}
	}
}
