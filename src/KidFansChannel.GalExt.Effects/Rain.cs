using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Xml;
using KidFansChannel.GalCore;
using KidFansChannel.GalCore.Commands;
using KidFansChannel.GalCore.ExtBases;
using KidFansChannel.Game2DCore;

namespace KidFansChannel.GalExt.Effects
{
	public abstract class Rain : Effect
	{
		protected class OneRain
		{
			public PointF Position;

			public float Angle;

			public float Length;
		}

		private int _style;

		protected int Num;

		protected float AngleMax;

		protected float AngleMin;

		protected int TypeNum;

		protected float LengthMax;

		protected float LengthMin;

		protected float Speed;

		protected float SideRate = 0.1f;

		protected Picture PicRain;

		protected bool Back;

		protected List<OneRain> Rains = new List<OneRain>();

		protected Random Ran = new Random();

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
				return 12f;
			}
		}

		public Rain()
		{
			Single = false;
			Lasting = false;
			SupportSpeed = false;
			Name = "rain";
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

		protected virtual void InitStyle()
		{
		}

		protected OneRain GenerateOneRain(bool begin)
		{
			OneRain oneRain = new OneRain();
			oneRain.Length = (float)(Ran.NextDouble() * (double)(LengthMax - LengthMin) + (double)LengthMin);
			oneRain.Angle = (float)(Ran.NextDouble() * (double)(AngleMax - AngleMin) + (double)AngleMin);
			if (begin)
			{
				double num = Ran.NextDouble();
				if (num > (double)SideRate)
				{
					oneRain.Position = new PointF((float)(num - (double)SideRate) / (1f - SideRate) * (float)(BgDisplay.ScreenSize.Width + BgDisplay.ScreenSize.Height), 0f);
				}
				else
				{
					oneRain.Position = new PointF((!(oneRain.Angle > 0f)) ? BgDisplay.ScreenSize.Width : 0, (float)(num / (double)SideRate * (double)BgDisplay.ScreenSize.Height));
				}
			}
			else
			{
				oneRain.Position = new PointF((float)(Ran.NextDouble() * (double)BgDisplay.ScreenSize.Width), (float)(Ran.NextDouble() * (double)BgDisplay.ScreenSize.Height));
			}
			return oneRain;
		}

		public override void InitCommand(float speed)
		{
			if (Back)
			{
				Parent.CreateShadow();
				Parent.SetShadowLayer(Layer);
				Parent.SetShadowDraw(Draw);
			}
			for (int i = 0; i < Num; i++)
			{
				Rains.Add(GenerateOneRain(false));
			}
		}

		public override void FinalCommand(float speed)
		{
			Finished = true;
		}

		public override void Update(double appTime, float elaspedTime)
		{
			if (Rains.Count < Num)
			{
				for (int i = 0; i < Num - Rains.Count; i++)
				{
					Rains.Add(GenerateOneRain(true));
				}
			}
			foreach (OneRain rain in Rains)
			{
				UpdateOneRain(rain, elaspedTime);
			}
			int num = 0;
			while (num < Rains.Count)
			{
				if (!((double)Rains[num].Position.Y > (double)BgDisplay.ScreenSize.Height + (double)Rains[num].Length * Math.Cos(Rains[num].Angle)) && !((double)Rains[num].Position.X < (double)(0f - Rains[num].Length) * Math.Abs(Math.Sin(Rains[num].Angle))) && !((double)Rains[num].Position.X > (double)BgDisplay.ScreenSize.Width + (double)Rains[num].Length * Math.Abs(Math.Sin(Rains[num].Angle))))
				{
					num++;
				}
				else
				{
					Rains.RemoveAt(num);
				}
			}
		}

		protected void UpdateOneRain(OneRain oneRain, float elaspedTime)
		{
			oneRain.Position.X += oneRain.Length * (float)Math.Sin(oneRain.Angle) * Speed * elaspedTime;
			oneRain.Position.Y += oneRain.Length * (float)Math.Cos(oneRain.Angle) * Speed * elaspedTime;
		}

		public override void Draw()
		{
			if (!Back)
			{
				Draw(ScriptCommand.Canvas);
			}
		}

		public void Draw(ICanvas canvas)
		{
			foreach (OneRain rain in Rains)
			{
				DrawOneRain(rain, canvas);
			}
		}

		protected void DrawOneRain(OneRain oneRain, ICanvas canvas)
		{
			if (ScriptCommand.Options.Graphics != GameOptions.Level.Low)
			{
				PicRain.X = (int)((double)oneRain.Position.X - (double)oneRain.Length * Math.Sin(oneRain.Angle) + 0.5);
				PicRain.Y = (int)((double)oneRain.Position.Y - (double)oneRain.Length * Math.Cos(oneRain.Angle) + 0.5);
				PicRain.Rotate = oneRain.Angle;
				PicRain.ScaleY = oneRain.Length / (float)PicRain.Height;
				canvas.DrawPicture(PicRain);
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
	}
}
