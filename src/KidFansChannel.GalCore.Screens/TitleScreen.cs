using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace KidFansChannel.GalCore.Screens
{
	public abstract class TitleScreen : GameScreen
	{
		protected class MenuItem : Element
		{
			public class MenuEventArg : EventArgs
			{
				public bool Enabled = true;
			}

			public int Index;

			public event EventHandler<MenuEventArg> MenuEvent;

			public event EventHandler Initial;

			protected internal override bool OnMouseDown(Point location)
			{
				if (MenuEvent != null)
				{
					MenuEventArg menuEventArg = new MenuEventArg();
					MenuEvent(this, menuEventArg);
					if (menuEventArg.Enabled)
					{
						return base.OnMouseDown(location);
					}
					return false;
				}
				return base.OnMouseDown(location);
			}

			protected internal override void OnMouseEnter()
			{
				if (MenuEvent != null)
				{
					MenuEventArg menuEventArg = new MenuEventArg();
					MenuEvent(this, menuEventArg);
					if (menuEventArg.Enabled)
					{
						base.OnMouseEnter();
					}
				}
				else
				{
					base.OnMouseEnter();
				}
			}

			protected internal override void OnMouseLeave()
			{
				if (MenuEvent != null)
				{
					MenuEventArg menuEventArg = new MenuEventArg();
					MenuEvent(this, menuEventArg);
					if (menuEventArg.Enabled)
					{
						base.OnMouseLeave();
					}
				}
				else
				{
					base.OnMouseLeave();
				}
			}

			public void Initialize()
			{
				if (Initial != null)
				{
					Initial(this, EventArgs.Empty);
				}
			}
		}

		private string[] menuToShow;

		private bool exiting;

		protected MenuItem RootItem;

		protected MenuItem CurrentItem;

		protected int CurrentLevel;

		protected Stack<MenuItem> ItemStack = new Stack<MenuItem>();

		protected Element ItemElement;

		protected MenuItem ItemToEnter;

		protected MenuItem ItemToBack;

		private bool _hide;

		public override bool Hide
		{
			get
			{
				return _hide;
			}
		}

		public string[] GetMenuStack()
		{
			MenuItem[] items = ItemStack.ToArray();
			string[] values = new string[items.Length];
			for (int i = 0; i < items.Length; i++)
			{
				values[i] = (string)items[i].Value;
			}
			return values;
		}

		public void SetMenuStack(string[] value)
		{
			menuToShow = value;
		}

		public override bool InitScreen()
		{
			if (!base.InitScreen())
			{
				return false;
			}
			ItemElement = new Element();
			FocusedElement = ItemElement;
			ItemElement.Focused = true;
			CurrentItem = RootItem;
			CurrentLevel = 0;
			RootItem = new MenuItem();
			return true;
		}

		protected bool InitMenu()
		{
			MenuItem menuItem = RootItem;
			int menuIndex = 0;
			ItemStack.Clear();
			ItemStack.Push(RootItem);
			if (menuToShow != null)
			{
				for (int scanIndex = menuToShow.Length - 2; scanIndex >= 0; scanIndex--)
				{
					for (int i = 0; i < menuItem.Count; i++)
					{
						if (menuItem[i] is MenuItem && (string)menuItem[i].Value == menuToShow[scanIndex])
						{
							menuItem = (MenuItem)menuItem[i];
							ItemStack.Push(menuItem);
							scanIndex++;
						}
					}
				}
			}
			UpdateItem(menuItem, menuIndex);
			return menuToShow != null;
		}

		protected void FinalMenu()
		{
			DisposeMenuItem(RootItem);
			ItemElement[0].Dispose();
			ItemElement.Dispose();
		}

		private void DisposeMenuItem(MenuItem item)
		{
			for (int i = 0; i < item.Count; i++)
			{
				if (item[i] is MenuItem)
				{
					DisposeMenuItem((MenuItem)item[i]);
				}
			}
			item.Dispose();
		}

		protected void StartNewGame(string story)
		{
			GameManager.GameScript.StartStory(story, true);
			FinalScreen();
		}

		protected void StartNewGame(string story, string script, string chapter)
		{
			GameManager.GameScript.StartStory(story, false);
			GameManager.GameScript.CurrentGameScript.JumpScene(script, chapter, 0);
			FinalScreen();
		}

		protected void ExitGame()
		{
			if (!exiting)
			{
				GameManager.GalCore.CanExitNow = true;
				Application.Exit();
				exiting = true;
			}
		}

		protected virtual void UpdateItem(MenuItem item, int level)
		{
			CurrentItem = item;
			CurrentLevel = level;
			for (int menuIndex = ItemElement.Count - 1; menuIndex >= 0; menuIndex--)
			{
				ItemElement.Remove(menuIndex);
			}
			int i = 0;
			int itemIndex = 0;
			for (; i < CurrentItem.Count; i++)
			{
				if (!(CurrentItem[i] is MenuItem))
				{
					continue;
				}
				CurrentItem[i].Position = GetItemPosition(itemIndex, CurrentLevel);
				((MenuItem)CurrentItem[i]).Index = i;
				CurrentItem[i].MouseClick -= Item_MouseClick;
				CurrentItem[i].MouseClick += Item_MouseClick;
				ItemElement.Add(CurrentItem[i], (string)CurrentItem[i].Value);
				for (int j = 0; j < CurrentItem[i].Count; j++)
				{
					if (CurrentItem[i][j] is MenuItem)
					{
						CurrentItem[i].SetVisible(j, false);
					}
				}
				itemIndex++;
				((MenuItem)CurrentItem[i]).Initialize();
			}
			for (int k = 0; k < ItemElement.Count - 1; k++)
			{
				ItemElement[k].Down = ItemElement[k + 1];
				ItemElement[k + 1].Up = ItemElement[k];
			}
			ItemElement.ChildUp = ItemElement[ItemElement.Count - 1];
			ItemElement.ChildDown = ItemElement[0];
			ItemElement.ClearSelected();
		}

		private void Item_MouseClick(object sender, EventArgs e)
		{
			MenuItem menuItem = (MenuItem)sender;
			if (menuItem.Disposed)
			{
				return;
			}
			bool found = false;
			for (int i = 0; i < menuItem.Count; i++)
			{
				if (menuItem[i] is MenuItem)
				{
					found = true;
				}
			}
			if (found)
			{
				if (ItemToEnter == null)
				{
					ItemToEnter = (MenuItem)CurrentItem[menuItem.Index];
				}
			}
			else
			{
				ItemToEnter = null;
			}
		}

		public override void OnKeyPress(Keys keyData)
		{
			if (keyData == Keys.Space && CurrentLevel > 0 && ItemToBack == null)
			{
				ItemStack.Pop();
				ItemToBack = ItemStack.Peek();
			}
			if (FocusedElement != null)
			{
				if (FocusedElement.SelectedChild == null)
				{
					FocusedElement.OnKeyPress(keyData);
				}
				else
				{
					FocusedElement.SelectedChild.OnKeyPress(keyData);
				}
			}
		}

		protected void ChangeItem()
		{
			FocusedElement.ClearSelected();
			if (ItemToEnter != null)
			{
				UpdateItem(ItemToEnter, CurrentLevel + 1);
				ItemStack.Push(ItemToEnter);
				ItemToEnter = null;
			}
			else if (ItemToBack != null)
			{
				UpdateItem(ItemToBack, CurrentLevel - 1);
				ItemToBack = null;
			}
		}

		protected abstract Point GetItemPosition(int index, int level);

		public void HideTitle()
		{
			_hide = true;
		}
	}
}
