using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using KidFansChannel.GalCore;
using KidFansChannel.GalCore.Commands;
using KidFansChannel.GalCore.ExtBases;
using KidFansChannel.Game2DCore;

namespace KidFansChannel.GalExt.Effects
{
	public abstract class Sun : Effect
	{
		protected class OneSun
		{
			public int X;

			public int Y;

			public int Alpha;

			public int CurrentAlpha;

			public float Time;

			public float CurrentTime;

			public int Type;
		}

		private int _style;

		protected int XMax;

		protected int XMin;

		protected int YMax;

		protected int YMin;

		protected int AlphaMax;

		protected int AlphaMin;

		protected float TimeMax;

		protected float TimeMin;

		protected float NextMax;

		protected float NextMin;

		protected float ShowHideTime;

		protected int TypeNum;

		protected Picture[] PicSuns;

		protected List<OneSun> Suns = new List<OneSun>();

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
				return 11f;
			}
		}

		public Sun()
		{
			Single = false;
			Lasting = false;
			SupportSpeed = false;
			Name = "sun";
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

		protected OneSun GenerateOneSun()
		{
			OneSun oneSun = new OneSun();
			oneSun.X = Ran.Next(XMax - XMin) + XMin;
			oneSun.Y = Ran.Next(YMax - YMin) + YMin;
			oneSun.Alpha = Ran.Next(AlphaMax - AlphaMin) + AlphaMin;
			oneSun.CurrentAlpha = 0;
			oneSun.Time = (float)(Ran.NextDouble() * (double)(TimeMax - TimeMin) + (double)TimeMin);
			oneSun.CurrentTime = 0f;
			oneSun.Type = Ran.Next(TypeNum);
			TimeToStartNext = (float)(Ran.NextDouble() * (double)(NextMax - NextMin) + (double)NextMin);
			return oneSun;
		}

		public override void InitCommand(float speed)
		{
			Suns.Add(GenerateOneSun());
			Suns[0].CurrentTime = (float)(Ran.NextDouble() * (double)(Suns[0].Time + 2f * ShowHideTime));
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
				Suns.Add(GenerateOneSun());
			}
			foreach (OneSun sun in Suns)
			{
				UpdateOneSun(sun, elaspedTime);
			}
			int index = 0;
			while (index < Suns.Count)
			{
				if (Suns[index].CurrentTime < Suns[index].Time + 2f * ShowHideTime)
				{
					index++;
				}
				else
				{
					Suns.RemoveAt(index);
				}
			}
		}

		protected void UpdateOneSun(OneSun oneSun, float elaspedTime)
		{
			oneSun.CurrentTime += elaspedTime;
			if (oneSun.CurrentTime < ShowHideTime)
			{
				oneSun.CurrentAlpha = (int)(oneSun.CurrentTime / ShowHideTime * (float)oneSun.Alpha);
			}
			else if (oneSun.CurrentTime < oneSun.Time + ShowHideTime)
			{
				oneSun.CurrentAlpha = oneSun.Alpha;
			}
			else if (oneSun.CurrentTime < oneSun.Time + 2f * ShowHideTime)
			{
				oneSun.CurrentAlpha = (int)((oneSun.Time + 2f * ShowHideTime - oneSun.CurrentTime) / ShowHideTime * (float)oneSun.Alpha);
			}
			else
			{
				oneSun.CurrentAlpha = 0;
			}
		}

		public override void Draw()
		{
			if (ScriptCommand.Options.Graphics == GameOptions.Level.Low)
			{
				return;
			}
			foreach (OneSun sun in Suns)
			{
				DrawOneSun(sun);
			}
		}

		protected void DrawOneSun(OneSun oneSun)
		{
			PicSuns[oneSun.Type].X = oneSun.X;
			PicSuns[oneSun.Type].Y = oneSun.Y;
			PicSuns[oneSun.Type].Alpha = oneSun.CurrentAlpha;
			ScriptCommand.Canvas.DrawPicture(PicSuns[oneSun.Type]);
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
