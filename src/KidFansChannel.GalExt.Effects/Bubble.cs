using System;
using System.Drawing;
using System.IO;
using System.Xml;
using KidFansChannel.GalCore;
using KidFansChannel.GalCore.Commands;
using KidFansChannel.GalCore.ExtBases;
using KidFansChannel.Game2DCore;

namespace KidFansChannel.GalExt.Effects
{
	public class Bubble : Effect
	{
		private Picture PicBubble;

		private Random Ran = new Random((int)DateTime.Now.Ticks);

		protected int BubbleNum = 18;

		protected int XSpeed = 25;

		protected int YA = 20;

		protected float YP = 6f;

		protected int YMin = 200;

		protected int YMax = 400;

		protected float SizeMax = 1f;

		protected float SizeMin = 0.5f;

		protected float[] Xs;

		protected float[] Ys;

		protected float[] YBases;

		protected float[] Times;

		protected float[] Sizes;

		public override bool DataReady
		{
			get
			{
				return PicBubble.IsCreated;
			}
		}

		public override float Layer
		{
			get
			{
				return 16f;
			}
		}

		public Bubble()
		{
			Single = false;
			Lasting = false;
			SupportSpeed = false;
			Name = "bubble";
		}

		public override void ConvertXml(XmlNode node, NodeInfo info)
		{
		}

		public override void InitData()
		{
			PicBubble = ScriptCommand.Data.LoadPicture("system", "bubble.png");
		}

		public override void FinalData()
		{
			if (PicBubble != null)
			{
				PicBubble.Dispose();
				PicBubble = null;
			}
		}

		public override void InitCommand(float speed)
		{
			Xs = new float[BubbleNum];
			Ys = new float[BubbleNum];
			YBases = new float[BubbleNum];
			Times = new float[BubbleNum];
			Sizes = new float[BubbleNum];
			for (int i = 0; i < BubbleNum; i++)
			{
				Xs[i] = (float)(Ran.NextDouble() * 800.0);
				YBases[i] = (float)((double)YMin + Ran.NextDouble() * (double)(YMax - YMin)) - 20f;
				Times[i] = (float)(Ran.NextDouble() * (double)YP);
				Ys[i] = (float)((double)YBases[i] + (double)YA * Math.Sin((double)(Times[i] / YP * 2f) * Math.PI));
				Sizes[i] = (float)((double)SizeMin + Ran.NextDouble() * (double)(SizeMax - SizeMin));
			}
		}

		public override void FinalCommand(float speed)
		{
			Finished = true;
		}

		public override void Update(double appTime, float elaspedTime)
		{
			for (int i = 0; i < BubbleNum; i++)
			{
				Xs[i] -= (float)XSpeed * elaspedTime;
				if (Xs[i] < -40f)
				{
					Xs[i] = 800f;
					YBases[i] = (float)((double)YMin + Ran.NextDouble() * (double)(YMax - YMin)) - 20f;
					Times[i] = (float)(Ran.NextDouble() * (double)YP);
					Ys[i] = (float)((double)YBases[i] + (double)YA * Math.Sin((double)(Times[i] / YP * 2f) * Math.PI));
					Sizes[i] = (float)((double)SizeMin + Ran.NextDouble() * (double)(SizeMax - SizeMin));
				}
				else
				{
					Times[i] += elaspedTime;
					while (Times[i] >= YP)
					{
						Times[i] -= YP;
					}
					Ys[i] = (float)((double)YBases[i] + (double)YA * Math.Sin((double)(Times[i] / YP * 2f) * Math.PI));
				}
			}
		}

		public override void Draw()
		{
			for (int i = 0; i < BubbleNum; i++)
			{
				PicBubble.X = (int)Xs[i];
				PicBubble.Y = (int)Ys[i];
				PicBubble.Scale = new SizeF(Sizes[i], Sizes[i]);
				ScriptCommand.Canvas.DrawPicture(PicBubble);
			}
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
