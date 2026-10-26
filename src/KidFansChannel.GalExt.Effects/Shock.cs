using System.IO;
using System.Xml;
using KidFansChannel.GalCore;
using KidFansChannel.GalCore.Commands;
using KidFansChannel.GalCore.ExtBases;

namespace KidFansChannel.GalExt.Effects
{
	public class Shock : Effect
	{
		private bool _loop;

		private bool shock;

		public bool Loop
		{
			get
			{
				return _loop;
			}
			set
			{
				_loop = value;
			}
		}

		public override bool DataReady
		{
			get
			{
				return true;
			}
		}

		public override float Layer
		{
			get
			{
				return 0f;
			}
		}

		public Shock()
		{
			Single = true;
			Lasting = false;
			Name = "shock";
			SupportSpeed = false;
		}

		public override void ConvertXml(XmlNode node, NodeInfo info)
		{
			_loop = ScriptHelper.GetArgumentBool(node, info, "loop", false);
			Lasting = !_loop;
		}

		public override void InitData()
		{
		}

		public override void FinalData()
		{
			if (shock)
			{
				ScriptCommand.GalCore.StopShock();
				shock = false;
			}
		}

		public override void InitCommand(float speed)
		{
			shock = true;
			ScriptCommand.GalCore.StartShock();
		}

		public override void FinalCommand(float speed)
		{
			if (shock)
			{
				ScriptCommand.GalCore.StopShock();
				shock = false;
			}
		}

		public override void Update(double appTime, float elaspedTime)
		{
		}

		public override void Draw()
		{
		}

		public override void Save(BinaryWriter bw)
		{
			bw.Write(Name);
		}

		public override void Load(BinaryReader br)
		{
		}

		public override void ConfirmUpdate()
		{
		}
	}
}
