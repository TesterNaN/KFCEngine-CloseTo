using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using KidFansChannel.GalCore.ExtBases;
using KidFansChannel.Game2DCore;

namespace KidFansChannel.GalCore.Commands
{
	public class EffectStart : ScriptCommand, IDrawable, IUpdatable, IInstanceDataUser, ISaveable
	{
		private string _typeStr;

		private Effect _effect;

		private float _speed;

		private bool _local;

		private EffectShadow shadow;

		private float fakeLayer = (float)new Random().NextDouble();

		public static Dictionary<string, Type> Types = new Dictionary<string, Type>();

		private bool saveLocal;

		private Effect saveEffect;

		public string TypeStr
		{
			get
			{
				return _typeStr;
			}
			protected set
			{
				_typeStr = value;
			}
		}

		public Effect Effect
		{
			get
			{
				return _effect;
			}
			protected set
			{
				_effect = value;
			}
		}

		public float Speed
		{
			get
			{
				return _speed;
			}
			protected set
			{
				_speed = value;
			}
		}

		public bool Local
		{
			get
			{
				return _local;
			}
			protected set
			{
				_local = value;
			}
		}

		public List<Picture> ShadowPictures
		{
			get
			{
				if (shadow != null)
				{
					return shadow.Pictures;
				}
				return null;
			}
		}

		public int LayerMajor
		{
			get
			{
				if (!_local)
				{
					return 24;
				}
				return 16;
			}
		}

		public float LayerMinor
		{
			get
			{
				if (_effect != null)
				{
					return _effect.Layer;
				}
				return fakeLayer;
			}
		}

		public bool CanHide
		{
			get
			{
				return false;
			}
		}

		public bool DataReady
		{
			get
			{
				if (_effect != null)
				{
					return _effect.DataReady;
				}
				return true;
			}
		}

		public EffectStart()
		{
			Single = false;
			Lasting = false;
			Command = "effectstart";
		}

		public override void ConvertXml(XmlNode node, NodeInfo info)
		{
			base.ConvertXml(node, info);
			_typeStr = ScriptHelper.GetArgumentString(node, info, "type");
			_local = ScriptHelper.GetArgumentBool(node, info, "local", true);
			if (!Types.ContainsKey(_typeStr))
			{
				try
				{
					_speed = ScriptHelper.GetArgumentFloat(node, info, "speed");
					if (_speed > 0f)
					{
						Single = true;
					}
					return;
				}
				catch
				{
					return;
				}
			}
			Type type = Types[_typeStr];
			_effect = (Effect)type.Assembly.CreateInstance(type.FullName);
			_effect.ConvertXml(node, info);
			if (_effect.SupportSpeed)
			{
				_speed = ScriptHelper.GetArgumentFloat(node, info, "speed", 0.5f);
			}
			Single = _effect.Single;
			Lasting = _effect.Lasting;
		}

		public override void InitCommand()
		{
			base.InitCommand();
			if (_effect != null)
			{
				_effect.Init(this);
				_effect.InitCommand(_speed);
			}
		}

		public override void FinalCommand()
		{
			base.FinalCommand();
			FinalInstanceData();
		}

		public void Stop(float speed)
		{
			if (_effect != null)
			{
				_effect.FinalCommand(speed);
				Lasting = _effect.Lasting;
				if (_effect.Finished)
				{
					if (shadow != null)
					{
						shadow.Local = false;
						shadow.FinishCommand();
						shadow = null;
					}
					FinishCommand();
				}
			}
			else
			{
				if (shadow != null)
				{
					shadow.Local = false;
					shadow.FinishCommand();
					shadow = null;
				}
				FinishCommand();
			}
		}

		public void CreateShadow()
		{
			if (shadow != null)
			{
				throw new InvalidOperationException();
			}
			shadow = new EffectShadow();
			ScriptCommand.CommandList.Add(shadow);
			if (_local)
			{
				shadow.Local = true;
			}
		}

		public void SetShadowLayer(float value)
		{
			shadow.LayerMinor = value;
		}

		public void SetShadowDraw(EffectShadow.DrawDelegate drawCallback)
		{
			shadow.DrawCallBack = drawCallback;
		}

		public void Draw()
		{
			if (_effect != null)
			{
				_effect.Draw();
			}
		}

		public void Update(double appTime, float elaspedTime)
		{
			if (_effect == null)
			{
				return;
			}
			_effect.Update(appTime, elaspedTime);
			Lasting = _effect.Lasting;
			if (_effect.Finished)
			{
				if (shadow != null)
				{
					shadow.Local = false;
					shadow.FinishCommand();
					shadow = null;
				}
				FinishCommand();
			}
		}

		public void InitInstanceData()
		{
			if (_effect != null)
			{
				_effect.InitData();
			}
		}

		public void FinalInstanceData()
		{
			if (_effect != null)
			{
				_effect.FinalData();
			}
		}

		public static void AddEffect(Type effect)
		{
			if (!effect.IsSubclassOf(typeof(Effect)))
			{
				throw new ArgumentException();
			}
			string name = ((Effect)effect.Assembly.CreateInstance(effect.FullName)).Name;
			if (Types.ContainsKey(name))
			{
				Types.Remove(name);
			}
			Types.Add(name, effect);
		}

		public static void ClearEffect()
		{
			Types.Clear();
		}

		public void Save(BinaryWriter bw)
		{
			bw.Write(Command);
			bw.Write(saveLocal);
			if (saveEffect != null)
			{
				saveEffect.Save(bw);
			}
			else
			{
				bw.Write(_typeStr);
			}
		}

		public void Load(BinaryReader br)
		{
			_local = br.ReadBoolean();
			_typeStr = br.ReadString();
			if (Types.ContainsKey(_typeStr))
			{
				Type type = Types[_typeStr];
				_effect = (Effect)type.Assembly.CreateInstance(type.FullName);
				_effect.Load(br);
				_speed = 0f;
			}
		}

		public void ConfirmUpdate()
		{
			saveLocal = _local;
			saveEffect = _effect;
			if (_effect != null)
			{
				_effect.ConfirmUpdate();
			}
		}
	}
}
