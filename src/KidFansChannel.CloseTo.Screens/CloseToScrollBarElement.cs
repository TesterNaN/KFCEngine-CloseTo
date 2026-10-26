using System.Drawing;
using KidFansChannel.GalCore;
using KidFansChannel.GalCore.Screens;
using KidFansChannel.Game2DCore;

namespace KidFansChannel.CloseTo.Screens
{
	public class CloseToScrollBarElement : ScrollBarElement
	{
		private Picture picScrollBar;

		private Picture[] picScrollBarParts = new Picture[3];

		private bool _dataReady;

		public override bool DataReady
		{
			get
			{
				if (_dataReady)
				{
					return true;
				}
				if (picScrollBar != null && picScrollBar.IsCreated)
				{
					picScrollBarParts[0] = GameManager.Graphics.CreateSubPicture(picScrollBar, new Rectangle(0, 0, 15, 15));
					picScrollBarParts[1] = GameManager.Graphics.CreateSubPicture(picScrollBar, new Rectangle(0, 15, 15, 15));
					picScrollBarParts[2] = GameManager.Graphics.CreateSubPicture(picScrollBar, new Rectangle(0, 30, 15, 15));
					_dataReady = true;
					return true;
				}
				return false;
			}
		}

		public override int Size
		{
			get
			{
				return base.Size;
			}
			set
			{
				base.Size = value;
				Region = new Rectangle(0, 0, 15, Size);
			}
		}

		public override void InitData()
		{
			picScrollBar = GameManager.Data.LoadPicture("system", "scroll_bar.png");
		}

		public override void FinalData()
		{
			for (int i = 0; i < 3; i++)
			{
				picScrollBarParts[i].Dispose();
			}
			picScrollBar.Dispose();
			_dataReady = false;
		}

		public override void Init()
		{
			ReConstruct();
			for (int i = 0; i < 3; i++)
			{
				Add(new Element(picScrollBarParts[i]), "scrollbar" + i);
			}
		}

		public override void Final()
		{
			base.Final();
			for (int i = 0; i < 3; i++)
			{
				base["scrollbar" + i].Dispose();
			}
			Dispose();
		}

		protected override void UpdateBar()
		{
			int num = (int)(Start * (float)Size + 0.5f);
			int num2 = ((Length != 0f) ? ((int)(Length * (float)Size + 0.5f)) : Size);
			if (num2 > 30)
			{
				base["scrollbar0"].Position = new Point(0, num);
				SetVisible("scrollbar1", true);
				base["scrollbar1"].Position = new Point(0, num + 15);
				base["scrollbar1"].ElementPicture.ScaleY = (float)(num2 - 30) / 15f;
				base["scrollbar2"].Position = new Point(0, num + num2 - 15);
				return;
			}
			int num3 = (int)((Start + Length / 2f) * (float)Size + 0.5f);
			SetVisible("scrollbar1", false);
			if (num3 < 15)
			{
				base["scrollbar0"].Position = new Point(0, 0);
				base["scrollbar2"].Position = new Point(0, 15);
			}
			else if (num3 > Size - 15)
			{
				base["scrollbar0"].Position = new Point(0, Size - 30);
				base["scrollbar2"].Position = new Point(0, Size - 15);
			}
			else
			{
				base["scrollbar0"].Position = new Point(0, num3 - 15);
				base["scrollbar2"].Position = new Point(0, num3);
			}
		}

		public override ScrollBarArea GetArea(Point location)
		{
			int num = (int)(Start * (float)Size + 0.5f);
			int num2 = ((Length != 0f) ? ((int)(Length * (float)Size + 0.5f)) : Size);
			if (num2 < 30)
			{
				int num3 = (int)((Start + Length / 2f) * (float)Size + 0.5f);
				num = ((num3 >= 15) ? ((num3 <= Size - 15) ? (num3 - 15) : (Size - 30)) : 0);
				num2 = 30;
			}
			if (location.Y < num)
			{
				return ScrollBarArea.Up;
			}
			if (location.Y < num + num2)
			{
				return ScrollBarArea.Block;
			}
			return ScrollBarArea.Down;
		}

		public override float DragValue(Point distance)
		{
			return (float)distance.Y / (float)Size;
		}
	}
}
