using System.Xml;
using KidFansChannel.GalCore.ExtBases;
using KidFansChannel.GalCore.Records;
using KidFansChannel.Game2DCore;

namespace KidFansChannel.GalCore.Commands
{
	public class Scene : ScriptCommand, IDrawable, IUpdatable, IRecordable
	{
		private float _speed;

		private DisplayStyle _style;

		private bool _continue;

		public CanvasPicture Picture;

		protected ScriptCommand Bg;

		private float styletime;

		private bool _load;

		private bool canRecord;

		public float Speed
		{
			get
			{
				return _speed;
			}
			set
			{
				_speed = value;
			}
		}

		public DisplayStyle Type
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

		public bool Continue
		{
			get
			{
				return _continue;
			}
			set
			{
				_continue = value;
			}
		}

		public bool Load
		{
			get
			{
				return _load;
			}
			set
			{
				_load = value;
			}
		}

		public int LayerMajor
		{
			get
			{
				return 20;
			}
		}

		public float LayerMinor
		{
			get
			{
				return 0f;
			}
		}

		public bool CanHide
		{
			get
			{
				return false;
			}
		}

		public Scene()
		{
			Single = false;
			Lasting = true;
			Command = "scene";
		}

		public override void ConvertXml(XmlNode node, NodeInfo info)
		{
			base.ConvertXml(node, info);
			_continue = ScriptHelper.GetArgumentBool(node, info, "continue", false);
			if (!_continue && !_load)
			{
				canRecord = true;
			}
		}

		public override void InitCommand()
		{
			base.InitCommand();
			if (_continue && !_load)
			{
				FinishCommand();
				return;
			}
			bool queued = false;
			for (int i = 0; i < ScriptCommand.GalCore.PreparingCommandList.Count; i++)
			{
				if (ScriptCommand.GalCore.PreparingCommandList[i] == this)
				{
					queued = true;
				}
				else if (queued && (ScriptCommand.GalCore.PreparingCommandList[i] is BgDisplay || ScriptCommand.GalCore.PreparingCommandList[i] is BgClear))
				{
					Bg = ScriptCommand.GalCore.PreparingCommandList[i];
				}
			}
			Picture = ScriptCommand.GalCore.ScenePicture;
			Picture.Alpha = 0;
		}

		public override void FinalCommand()
		{
			if (!_continue || _load)
			{
				foreach (ScriptCommand currentCommand in ScriptCommand.GalCore.CurrentCommandList)
				{
					if (currentCommand != this)
					{
						if (currentCommand is IDrawable && ((IDrawable)currentCommand).LayerMajor < LayerMajor)
						{
							currentCommand.FinishCommand();
						}
						continue;
					}
					break;
				}
			}
			base.FinalCommand();
		}

		public void Display(float speed, DisplayStyle style)
		{
			_speed = speed;
			_style = style;
			if (style == null)
			{
				Picture.BeginTransform(speed);
				Picture.AlphaTo(255);
				Picture.EndTransform();
			}
			else
			{
				styletime = speed;
				_style.InitStyle(speed);
			}
		}

		public void Draw()
		{
			if (_style == null)
			{
				if (ScriptCommand.GalCore.GameOptions.Graphics != GameOptions.Level.Low)
				{
					ScriptCommand.Graphics.DrawPicture(Picture);
				}
				else if (Bg is BgDisplay)
				{
					int alpha = ((BgDisplay)Bg).Bg.Alpha;
					((BgDisplay)Bg).Bg.Alpha = Picture.Alpha;
					ScriptCommand.Graphics.DrawPicture(((BgDisplay)Bg).Bg);
					((BgDisplay)Bg).Bg.Alpha = alpha;
				}
				else if (Bg is BgClear)
				{
					int alpha2 = ((BgClear)Bg).Bg.Alpha;
					((BgClear)Bg).Bg.Alpha = Picture.Alpha;
					ScriptCommand.Graphics.DrawPicture(((BgClear)Bg).Bg);
					((BgClear)Bg).Bg.Alpha = alpha2;
				}
			}
			else if (ScriptCommand.GalCore.GameOptions.Graphics == GameOptions.Level.High || ScriptCommand.GalCore.GameOptions.Graphics == GameOptions.Level.Mid)
			{
				_style.Draw();
			}
			else if (Bg is BgDisplay)
			{
				int alpha3 = ((BgDisplay)Bg).Bg.Alpha;
				((BgDisplay)Bg).Bg.Alpha = (int)((_speed - styletime) / _speed * 255f);
				ScriptCommand.Graphics.DrawPicture(((BgDisplay)Bg).Bg);
				((BgDisplay)Bg).Bg.Alpha = alpha3;
			}
			else if (Bg is BgClear)
			{
				int alpha4 = ((BgClear)Bg).Bg.Alpha;
				((BgClear)Bg).Bg.Alpha = (int)((_speed - styletime) / _speed * 255f);
				ScriptCommand.Graphics.DrawPicture(((BgClear)Bg).Bg);
				((BgClear)Bg).Bg.Alpha = alpha4;
			}
		}

		public void Update(double appTime, float elaspedTime)
		{
			if (_style == null)
			{
				if (ScriptCommand.PlayModeQuicking)
				{
					Picture.Update(elaspedTime * (float)ScriptCommand.QuickFactor);
				}
				else
				{
					Picture.Update(elaspedTime);
				}
				if (Picture.Time == 0f)
				{
					FinishCommand();
				}
				return;
			}
			if (ScriptCommand.PlayModeQuicking)
			{
				styletime -= elaspedTime * (float)ScriptCommand.QuickFactor;
				_style.UpdateStyle(elaspedTime * (float)ScriptCommand.QuickFactor);
			}
			else
			{
				styletime -= elaspedTime;
				_style.UpdateStyle(elaspedTime);
			}
			if (styletime < 0f)
			{
				styletime = 0f;
			}
			if (_style.Finished)
			{
				FinishCommand();
			}
		}

		public Record[] GetRecord()
		{
			if (canRecord)
			{
				canRecord = false;
				return new Record[1]
				{
					new SceneRecord()
				};
			}
			return null;
		}
	}
}
