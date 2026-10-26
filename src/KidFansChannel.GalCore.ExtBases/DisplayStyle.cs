using KidFansChannel.GalCore.Commands;

namespace KidFansChannel.GalCore.ExtBases
{
	public abstract class DisplayStyle
	{
		private string _name;

		private Scene _parent;

		public string Name
		{
			get
			{
				return _name;
			}
			protected set
			{
				_name = value;
			}
		}

		protected Scene Parent
		{
			get
			{
				return _parent;
			}
		}

		public abstract bool Finished { get; }

		public abstract bool Ready { get; }

		public virtual void Init(Scene parent)
		{
			_parent = parent;
		}

		public abstract void InitStyle(float speed);

		public abstract void UpdateStyle(float elaspedTime);

		public abstract void Draw();

		public abstract void InitData();

		public abstract void FinalData();
	}
}
