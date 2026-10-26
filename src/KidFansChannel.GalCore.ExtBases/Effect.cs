using System.IO;
using System.Xml;
using KidFansChannel.GalCore.Commands;

namespace KidFansChannel.GalCore.ExtBases
{
	public abstract class Effect : ISaveable
	{
		private string _name;

		private EffectStart _parent;

		private bool _single;

		private bool _lasting;

		private bool _supportSpeed;

		public bool Finished;

		public EffectStart Parent
		{
			get
			{
				return _parent;
			}
			protected set
			{
				_parent = value;
			}
		}

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

		public bool Single
		{
			get
			{
				return _single;
			}
			protected set
			{
				_single = value;
			}
		}

		public bool Lasting
		{
			get
			{
				return _lasting;
			}
			protected set
			{
				_lasting = value;
			}
		}

		public bool SupportSpeed
		{
			get
			{
				return _supportSpeed;
			}
			protected set
			{
				_supportSpeed = value;
			}
		}

		public abstract bool DataReady { get; }

		public abstract float Layer { get; }

		public abstract void ConvertXml(XmlNode node, NodeInfo info);

		public abstract void InitData();

		public abstract void FinalData();

		public void Init(EffectStart parent)
		{
			_parent = parent;
		}

		public abstract void InitCommand(float speed);

		public abstract void FinalCommand(float speed);

		public abstract void Update(double appTime, float elaspedTime);

		public abstract void Draw();

		public Effect()
		{
		}

		public abstract void Save(BinaryWriter bw);

		public abstract void Load(BinaryReader br);

		public abstract void ConfirmUpdate();
	}
}
