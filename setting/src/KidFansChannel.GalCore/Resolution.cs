using System.Drawing;

namespace KidFansChannel.GalCore
{
	public struct Resolution
	{
		public Size Size;

		public Size Proportion;

		public Rectangle Bounds;

		public float FontScale;

		public Rectangle StateArea;

		public Rectangle CommentArea;

		public static readonly Resolution Empty;

		public Resolution(Size size, Size proportion, Rectangle bounds, float fontScale, Rectangle stateArea, Rectangle commentArea)
		{
			Size = size;
			Proportion = proportion;
			Bounds = bounds;
			FontScale = fontScale;
			StateArea = stateArea;
			CommentArea = commentArea;
		}

		public static bool operator ==(Resolution left, Resolution right)
		{
			if (left.Size == right.Size && left.Proportion == right.Proportion && left.Bounds == right.Bounds && left.FontScale == right.FontScale && left.StateArea == right.StateArea)
			{
				return left.CommentArea == right.CommentArea;
			}
			return false;
		}

		public static bool operator !=(Resolution left, Resolution right)
		{
			if (!(left.Size != right.Size) && !(left.Proportion != right.Proportion) && !(left.Bounds != right.Bounds) && left.FontScale == right.FontScale && !(left.StateArea != right.StateArea))
			{
				return left.CommentArea != right.CommentArea;
			}
			return true;
		}

		public override bool Equals(object obj)
		{
			if (obj is Resolution)
			{
				Resolution resolution = (Resolution)obj;
				if (resolution.Size == Size && resolution.Proportion == Proportion && resolution.Bounds == Bounds && resolution.FontScale == FontScale && resolution.StateArea == StateArea)
				{
					return resolution.CommentArea == CommentArea;
				}
				return false;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return Size.Width ^ Size.Height ^ Proportion.Width ^ Proportion.Height ^ Bounds.X ^ Bounds.Y ^ Bounds.Width ^ Bounds.Height ^ (int)(FontScale * 1000f) ^ StateArea.X ^ StateArea.Y ^ StateArea.Width ^ StateArea.Height ^ CommentArea.X ^ CommentArea.Y ^ CommentArea.Width ^ CommentArea.Height;
		}
	}
}
