using System.Collections.Generic;
using KidFansChannel.Game2DCore;

namespace KidFansChannel.GalCore.Commands
{
	public class EffectShadow : ScriptCommand, IDrawable, IUpdatable
	{
		public delegate void DrawDelegate(ICanvas canvas);

		private bool _local;

		private List<Picture> _pictures = new List<Picture>();

		private float _layerMinor;

		public DrawDelegate DrawCallBack;

		public bool Local
		{
			get
			{
				return _local;
			}
			set
			{
				_local = value;
			}
		}

		public List<Picture> Pictures
		{
			get
			{
				return _pictures;
			}
		}

		public int LayerMajor
		{
			get
			{
				return 4;
			}
		}

		public float LayerMinor
		{
			get
			{
				return _layerMinor;
			}
			set
			{
				_layerMinor = value;
			}
		}

		public bool CanHide
		{
			get
			{
				return false;
			}
		}

		internal EffectShadow()
		{
			Single = false;
			Lasting = false;
			Command = "";
		}

		public void Draw()
		{
			if (DrawCallBack != null)
			{
				DrawCallBack(ScriptCommand.Canvas);
				return;
			}
			foreach (Picture picture in _pictures)
			{
				ScriptCommand.Canvas.DrawPicture(picture);
			}
		}

		public void Update(double appTime, float elaspedTime)
		{
		}
	}
}
