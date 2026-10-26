namespace KidFansChannel.GalCore.ExtBases
{
	public abstract class TransformStyle<Command>
	{
		private string _name;

		private bool _lasting;

		private Command _parent;

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

		public bool Lasting
		{
			get
			{
				return _lasting;
			}
			set
			{
				_lasting = value;
			}
		}

		protected Command Parent
		{
			get
			{
				return _parent;
			}
		}

		public abstract bool Finished { get; }

		public TransformStyle()
		{
		}

		public virtual void Init(Command parent)
		{
			_parent = parent;
		}

		public abstract void InitStyle();

		public abstract void UpdateStyle(float elaspedTime);
	}
}
