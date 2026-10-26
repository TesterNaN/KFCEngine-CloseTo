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
	public abstract class Sakura : Effect
	{
		protected class OneSakura
		{
			public PointF Position;

			public PointF PositionStart;

			public PointF SpeedStart;

			public PointF GoalSpeed;

			public float ChangeTime;

			public float CurrentTime;

			public float OwnTime;

			public float Rotate;

			public float RotateSpeed;

			public int Type;
		}

		private int _style;

		protected int NumMax;

		protected int NumMin;

		protected PointF SpeedMax;

		protected float ChangeMin;

		protected float ChangeMax;

		protected PointF Acc;

		protected float SideRatio;

		protected float AnimiSpeed;

		protected int TypeNum;

		protected int AnimaSteps;

		protected float NextMax;

		protected float NextMin;

		protected float RotateMax;

		protected float RotateMin;

		protected Picture[,] PicSakuras;

		protected int HalfSize;

		protected List<OneSakura> Sakuras = new List<OneSakura>();

		protected float TimeToStartNext;

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
				return 13f;
			}
		}

		public Sakura()
		{
			Single = false;
			Lasting = false;
			SupportSpeed = false;
			Name = "sakura";
		}

		public override void ConvertXml(XmlNode node, NodeInfo info)
		{
			_style = ScriptHelper.GetArgumentInteger(node, info, "style");
			InitStyle();
		}

		protected virtual void InitStyle()
		{
		}

		protected OneSakura GenerateOneSakura(bool begin)
		{
			OneSakura oneSakura = new OneSakura();
			switch (_style % 4)
			{
			case 0:
				oneSakura.Position = (oneSakura.PositionStart = new PointF((float)(begin ? ((double)(-(HalfSize / 2))) : (Ran.NextDouble() * (double)BgDisplay.ScreenSize.Width)), (float)(Ran.NextDouble() * (double)((float)BgDisplay.ScreenSize.Height * SideRatio - (float)BgDisplay.ScreenSize.Height * (SideRatio - 1f) / 2f))));
				oneSakura.SpeedStart = new PointF(SpeedMax.X, (float)(Ran.NextDouble() * (double)SpeedMax.Y * 2.0 - (double)SpeedMax.Y));
				oneSakura.GoalSpeed = new PointF(SpeedMax.X, (float)(Ran.NextDouble() * (double)SpeedMax.Y * 2.0 - (double)SpeedMax.Y));
				break;
			case 1:
				oneSakura.Position = (oneSakura.PositionStart = new PointF((float)(begin ? ((double)(BgDisplay.ScreenSize.Width + HalfSize / 2)) : (Ran.NextDouble() * (double)BgDisplay.ScreenSize.Width)), (float)(Ran.NextDouble() * (double)((float)BgDisplay.ScreenSize.Height * SideRatio - (float)BgDisplay.ScreenSize.Height * (SideRatio - 1f) / 2f))));
				oneSakura.SpeedStart = new PointF(0f - SpeedMax.X, (float)(Ran.NextDouble() * (double)SpeedMax.Y * 2.0 - (double)SpeedMax.Y));
				oneSakura.GoalSpeed = new PointF(0f - SpeedMax.X, (float)(Ran.NextDouble() * (double)SpeedMax.Y * 2.0 - (double)SpeedMax.Y));
				break;
			case 2:
				oneSakura.Position = (oneSakura.PositionStart = new PointF((float)(Ran.NextDouble() * (double)((float)BgDisplay.ScreenSize.Width * SideRatio - (float)BgDisplay.ScreenSize.Width * (SideRatio - 1f) / 2f)), (float)(begin ? ((double)(-(HalfSize / 2))) : (Ran.NextDouble() * (double)BgDisplay.ScreenSize.Height))));
				oneSakura.SpeedStart = new PointF((float)(Ran.NextDouble() * (double)SpeedMax.X * 2.0 - (double)SpeedMax.X), SpeedMax.Y);
				oneSakura.GoalSpeed = new PointF((float)(Ran.NextDouble() * (double)SpeedMax.X * 2.0 - (double)SpeedMax.X), SpeedMax.Y);
				break;
			case 3:
				oneSakura.Position = (oneSakura.PositionStart = new PointF((float)(Ran.NextDouble() * (double)((float)BgDisplay.ScreenSize.Width * SideRatio - (float)BgDisplay.ScreenSize.Width * (SideRatio - 1f) / 2f)), (float)(begin ? ((double)(BgDisplay.ScreenSize.Height + HalfSize / 2)) : (Ran.NextDouble() * (double)BgDisplay.ScreenSize.Height))));
				oneSakura.SpeedStart = new PointF((float)(Ran.NextDouble() * (double)SpeedMax.X * 2.0 - (double)SpeedMax.X), 0f - SpeedMax.Y);
				oneSakura.GoalSpeed = new PointF((float)(Ran.NextDouble() * (double)SpeedMax.X * 2.0 - (double)SpeedMax.X), 0f - SpeedMax.Y);
				break;
			}
			oneSakura.PositionStart = oneSakura.Position;
			oneSakura.Type = Ran.Next(TypeNum);
			oneSakura.OwnTime = (float)(Ran.NextDouble() * (double)AnimiSpeed * (double)AnimaSteps);
			oneSakura.CurrentTime = 0f;
			oneSakura.ChangeTime = (float)(Ran.NextDouble() * (double)(ChangeMax - ChangeMin) + (double)ChangeMin);
			oneSakura.Rotate = (float)(Ran.NextDouble() * Math.PI * 2.0);
			oneSakura.RotateSpeed = (float)(Ran.NextDouble() * (double)(RotateMax - RotateMin) + (double)RotateMin);
			return oneSakura;
		}

		public override void InitCommand(float speed)
		{
			int spawnCount = Ran.Next(NumMin, NumMax);
			for (int i = 0; i < spawnCount; i++)
			{
				Sakuras.Add(GenerateOneSakura(false));
			}
			TimeToStartNext = (float)(Ran.NextDouble() * (double)(NextMax - NextMin) + (double)NextMin);
		}

		public override void FinalCommand(float speed)
		{
			Finished = true;
		}

		public override void Update(double appTime, float elaspedTime)
		{
			TimeToStartNext -= elaspedTime;
			if (TimeToStartNext < 0f)
			{
				Sakuras.Add(GenerateOneSakura(true));
				TimeToStartNext = (float)(Ran.NextDouble() * (double)(NextMax - NextMin) + (double)NextMin);
			}
			foreach (OneSakura sakura in Sakuras)
			{
				UpdateOneSakura(sakura, elaspedTime);
			}
			int spawnCount = 0;
			while (spawnCount < Sakuras.Count)
			{
				bool offscreen = false;
				switch (_style)
				{
				case 0:
					offscreen = Sakuras[spawnCount].Position.X > (float)(BgDisplay.ScreenSize.Width + HalfSize / 2);
					break;
				case 1:
					offscreen = Sakuras[spawnCount].Position.X < (float)(-HalfSize / 2);
					break;
				case 2:
					offscreen = Sakuras[spawnCount].Position.Y > (float)(BgDisplay.ScreenSize.Height + HalfSize / 2);
					break;
				case 3:
					offscreen = Sakuras[spawnCount].Position.Y < (float)(-HalfSize / 2);
					break;
				}
				if (!offscreen)
				{
					spawnCount++;
				}
				else
				{
					Sakuras.RemoveAt(spawnCount);
				}
			}
		}

		protected void UpdateOneSakura(OneSakura oneSakura, float elaspedTime)
		{
			oneSakura.CurrentTime += elaspedTime;
			oneSakura.OwnTime += elaspedTime;
			while (oneSakura.OwnTime > AnimiSpeed * (float)AnimaSteps)
			{
				oneSakura.OwnTime -= AnimiSpeed * (float)AnimaSteps;
			}
			PointF speedStart = default(PointF);
			PointF position = default(PointF);
			if (Math.Abs(oneSakura.GoalSpeed.X - oneSakura.SpeedStart.X) > oneSakura.CurrentTime * Acc.X)
			{
				speedStart.X = oneSakura.SpeedStart.X + oneSakura.CurrentTime * (float)Math.Sign(oneSakura.GoalSpeed.X - oneSakura.SpeedStart.X) * Acc.X;
				position.X = oneSakura.PositionStart.X + oneSakura.CurrentTime * oneSakura.SpeedStart.X + oneSakura.CurrentTime * oneSakura.CurrentTime * (float)Math.Sign(oneSakura.GoalSpeed.X - oneSakura.SpeedStart.X) * Acc.X / 2f;
			}
			else
			{
				float spawnCount = Math.Abs(oneSakura.GoalSpeed.X - oneSakura.SpeedStart.X) / Acc.X;
				speedStart.X = oneSakura.GoalSpeed.X;
				position.X = oneSakura.PositionStart.X + spawnCount * oneSakura.SpeedStart.X + spawnCount * spawnCount * (float)Math.Sign(oneSakura.GoalSpeed.X - oneSakura.SpeedStart.X) * Acc.X / 2f + (oneSakura.CurrentTime - spawnCount) * oneSakura.GoalSpeed.X;
			}
			if (Math.Abs(oneSakura.GoalSpeed.Y - oneSakura.SpeedStart.Y) > oneSakura.CurrentTime * Acc.Y)
			{
				speedStart.Y = oneSakura.SpeedStart.Y + oneSakura.CurrentTime * (float)Math.Sign(oneSakura.GoalSpeed.Y - oneSakura.SpeedStart.Y) * Acc.Y;
				position.Y = oneSakura.PositionStart.Y + oneSakura.CurrentTime * oneSakura.SpeedStart.Y + oneSakura.CurrentTime * oneSakura.CurrentTime * (float)Math.Sign(oneSakura.GoalSpeed.Y - oneSakura.SpeedStart.Y) * Acc.Y / 2f;
			}
			else
			{
				float speedDelta = Math.Abs(oneSakura.GoalSpeed.Y - oneSakura.SpeedStart.Y) / Acc.Y;
				speedStart.Y = oneSakura.GoalSpeed.Y;
				position.Y = oneSakura.PositionStart.Y + speedDelta * oneSakura.SpeedStart.Y + speedDelta * speedDelta * (float)Math.Sign(oneSakura.GoalSpeed.Y - oneSakura.SpeedStart.Y) * Acc.Y / 2f + (oneSakura.CurrentTime - speedDelta) * oneSakura.GoalSpeed.Y;
			}
			oneSakura.Position = position;
			if (oneSakura.CurrentTime > oneSakura.ChangeTime)
			{
				oneSakura.PositionStart = oneSakura.Position;
				oneSakura.CurrentTime = 0f;
				oneSakura.SpeedStart = speedStart;
				switch (_style)
				{
				case 0:
					oneSakura.GoalSpeed = new PointF(SpeedMax.X, (float)(Ran.NextDouble() * (double)SpeedMax.Y * 2.0 - (double)SpeedMax.Y));
					break;
				case 1:
					oneSakura.GoalSpeed = new PointF(0f - SpeedMax.X, (float)(Ran.NextDouble() * (double)SpeedMax.Y * 2.0 - (double)SpeedMax.Y));
					break;
				case 2:
					oneSakura.GoalSpeed = new PointF((float)(Ran.NextDouble() * (double)SpeedMax.X * 2.0 - (double)SpeedMax.X), SpeedMax.Y);
					break;
				case 3:
					oneSakura.GoalSpeed = new PointF((float)(Ran.NextDouble() * (double)SpeedMax.X * 2.0 - (double)SpeedMax.X), 0f - SpeedMax.Y);
					break;
				}
				oneSakura.ChangeTime = (float)(Ran.NextDouble() * (double)(ChangeMax - ChangeMin) + (double)ChangeMin);
			}
			oneSakura.Rotate += oneSakura.RotateSpeed * elaspedTime;
			while ((double)oneSakura.Rotate > Math.PI * 2.0)
			{
				oneSakura.Rotate -= (float)Math.PI * 2f;
			}
		}

		public override void Draw()
		{
			if (ScriptCommand.Options.Graphics == GameOptions.Level.Low)
			{
				return;
			}
			foreach (OneSakura sakura in Sakuras)
			{
				DrawOneSakura(sakura);
			}
		}

		protected void DrawOneSakura(OneSakura oneSakura)
		{
			int spawnCount = (int)(oneSakura.OwnTime / AnimiSpeed);
			PicSakuras[oneSakura.Type, spawnCount].X = (int)oneSakura.Position.X;
			PicSakuras[oneSakura.Type, spawnCount].Y = (int)oneSakura.Position.Y;
			PicSakuras[oneSakura.Type, spawnCount].Rotate = oneSakura.Rotate;
			ScriptCommand.Canvas.DrawPicture(PicSakuras[oneSakura.Type, spawnCount]);
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
