using System.Drawing;
using System.IO;
using System.Xml;
using KidFansChannel.GalCore;
using KidFansChannel.GalCore.Commands;
using KidFansChannel.GalCore.ExtBases;
using KidFansChannel.Game2DCore;

namespace KidFansChannel.GalExt.Effects
{
	public class Dark : Effect
	{
		private float _intensity;

		protected Picture PicBlack;

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
				return 20f;
			}
		}

		public Dark()
		{
			Single = true;
			Lasting = true;
			Name = "dark";
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
			PicBlack = ScriptCommand.Graphics.CreatePicture(Color.Black);
			PicBlack.Size = BgDisplay.ScreenSize;
			if (speed > 0f)
			{
				PicBlack.Alpha = 0;
				PicBlack.BeginTransform(speed);
				PicBlack.AlphaTo((int)(255f * _intensity));
				PicBlack.EndTransform();
			}
			else
			{
				PicBlack.Alpha = (int)(255f * _intensity);
			}
		}

		public override void FinalCommand(float speed)
		{
			if (speed > 0f)
			{
				PicBlack.BeginTransform(speed);
				PicBlack.AlphaTo(0);
				PicBlack.EndTransform();
			}
			else
			{
				PicBlack.Alpha = 0;
			}
		}

		public override void Update(double appTime, float elaspedTime)
		{
			if (ScriptCommand.PlayModeQuicking)
			{
				PicBlack.Update(elaspedTime * (float)ScriptCommand.QuickFactor);
			}
			else
			{
				PicBlack.Update(elaspedTime);
			}
			if (PicBlack.Time == 0f)
			{
				Lasting = false;
				if (PicBlack.Alpha == 0)
				{
					PicBlack.Dispose();
					Finished = true;
				}
			}
		}

		public override void Draw()
		{
			ScriptCommand.Graphics.DrawPicture(PicBlack);
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
