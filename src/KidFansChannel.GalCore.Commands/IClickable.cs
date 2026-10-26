using System.Drawing;

namespace KidFansChannel.GalCore.Commands
{
	public interface IClickable
	{
		bool ToClickState { get; }

		bool OnItemClick();

		void OnItemMove(Point location);
	}
}
