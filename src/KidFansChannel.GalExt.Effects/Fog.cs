using System.IO;
using System.Xml;
using KidFansChannel.GalCore;
using KidFansChannel.GalCore.Commands;
using KidFansChannel.GalCore.ExtBases;
using KidFansChannel.Game2DCore;

namespace KidFansChannel.GalExt.Effects
{
	public abstract class Fog : Effect
	{
		private int _style;

		protected bool Direction;

		protected int TypeNum;

		protected Picture[] PicFogs = new Picture[2];

		private float posX;

		protected float Speed;

		public int Style
		{
			get
			{
				return _style;
			}
			set
			{
				_style = value;
			}
		}

		public override float Layer
		{
			get
			{
				return 14f;
			}
		}

		public Fog()
		{
			Single = false;
			Lasting = false;
			SupportSpeed = false;
			Name = "fog";
		}

		public override void ConvertXml(XmlNode node, NodeInfo info)
		{
			_style = ScriptHelper.GetArgumentInteger(node, info, "style");
			if (_style >= TypeNum)
			{
				throw new ScriptArgumentException(node, info, "style", "Style's number is not defined.");
			}
			InitStyle();
		}

		protected abstract void InitStyle();

		public override void InitCommand(float speed)
		{
			posX = 0f;
		}

		public override void FinalCommand(float speed)
		{
			Finished = true;
		}

		public override void Update(double appTime, float elaspedTime)
		{
			posX += elaspedTime * Speed;
			while (posX >= 1600f)
			{
				posX -= 1600f;
			}
			if (Direction)
			{
				if (posX < 800f)
				{
					PicFogs[0].X = (int)posX;
				}
				else
				{
					PicFogs[0].X = (int)posX - 1600;
				}
				PicFogs[1].X = (int)posX - 800;
			}
			else
			{
				PicFogs[0].X = 800 - (int)posX;
				if (posX < 800f)
				{
					PicFogs[1].X = -(int)posX;
				}
				else
				{
					PicFogs[1].X = 1600 - (int)posX;
				}
			}
		}

		public override void Save(BinaryWriter bw)
		{
			bw.Write(Name);
			bw.Write(_style);
		}

		public override void Load(BinaryReader br)
		{
			_style = br.ReadInt32();
			InitStyle();
		}

		public override void ConfirmUpdate()
		{
		}

		public override void Draw()
		{
			if (ScriptCommand.Options.Graphics != GameOptions.Level.Low)
			{
				for (int i = 0; i < 2; i++)
				{
					ScriptCommand.Canvas.DrawPicture(PicFogs[i]);
				}
			}
		}
	}
}
