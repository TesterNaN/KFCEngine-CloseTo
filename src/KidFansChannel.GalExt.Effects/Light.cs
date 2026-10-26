using System.Drawing;
using System.IO;
using System.Xml;
using KidFansChannel.GalCore;
using KidFansChannel.GalCore.Commands;
using KidFansChannel.GalCore.ExtBases;
using KidFansChannel.Game2DCore;

namespace KidFansChannel.GalExt.Effects
{
	public class Light : Effect
	{
		private float _intensity;

		protected Picture PicWhite;

		public float Intensity
		{
			get
			{
				return _intensity;
			}
			protected set
			{
				_intensity = value;
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
				return 21f;
			}
		}

		public Light()
		{
			Single = true;
			Lasting = true;
			Name = "light";
			SupportSpeed = true;
		}

		public override void ConvertXml(XmlNode node, NodeInfo info)
		{
			_intensity = ScriptHelper.GetArgumentFloat(node, info, "intensity", 0.5f);
			Single = ScriptHelper.GetArgumentFloat(node, info, "speed", 0.5f) > 0f;
		}

		public override void InitData()
		{
		}

		public override void FinalData()
		{
		}

		public override void InitCommand(float speed)
		{
			PicWhite = ScriptCommand.Graphics.CreatePicture(Color.White);
			PicWhite.Size = BgDisplay.ScreenSize;
			if (speed > 0f)
			{
				PicWhite.Alpha = 0;
				PicWhite.BeginTransform(speed);
				PicWhite.AlphaTo((int)(255f * _intensity));
				PicWhite.EndTransform();
			}
			else
			{
				PicWhite.Alpha = (int)(255f * _intensity);
			}
		}

		public override void FinalCommand(float speed)
		{
			if (speed > 0f)
			{
				PicWhite.BeginTransform(speed);
				PicWhite.AlphaTo(0);
				PicWhite.EndTransform();
			}
			else
			{
				PicWhite.Alpha = 0;
			}
		}

		public override void Update(double appTime, float elaspedTime)
		{
			if (ScriptCommand.PlayModeQuicking)
			{
				PicWhite.Update(elaspedTime * (float)ScriptCommand.QuickFactor);
			}
			else
			{
				PicWhite.Update(elaspedTime);
			}
			if (PicWhite.Time == 0f)
			{
				Lasting = false;
				if (PicWhite.Alpha == 0)
				{
					PicWhite.Dispose();
					Finished = true;
				}
			}
		}

		public override void Draw()
		{
			ScriptCommand.Graphics.DrawPicture(PicWhite);
		}

		public override void Save(BinaryWriter bw)
		{
			bw.Write(Name);
			bw.Write(_intensity);
		}

		public override void Load(BinaryReader br)
		{
			_intensity = br.ReadSingle();
		}

		public override void ConfirmUpdate()
		{
		}
	}
}
