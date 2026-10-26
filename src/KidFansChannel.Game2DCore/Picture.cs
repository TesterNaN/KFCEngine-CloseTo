using System;
using System.Drawing;
using System.IO;
using System.Threading;
using Microsoft.DirectX.Direct3D;

namespace KidFansChannel.Game2DCore
{
	public class Picture : IDisposable, ICloneable
	{
		protected enum TransformType
		{
			None,
			Global,
			Local,
			Loop,
			Function
		}

		public delegate float TransformFunction(float time);

		private Size _size = Size.Empty;

		private Point _center = Point.Empty;

		private Point _position = Point.Empty;

		private SizeF _scale = new SizeF(1f, 1f);

		private float _rotate;

		private int _alpha = 255;

		private Color _argumentColor = Color.Transparent;

		private float _time;

		private PointF[] verts = new PointF[4];

		private float fAlpha = 255f;

		private int _alphaTo = 255;

		private PointF[] vertsTo = new PointF[4];

		private Texture _texture;

		private Color _color;

		private bool _textured;

		private Game2DCore _core;

		private bool _isCreated;

		private PointF[] _bounds = new PointF[4]
		{
			new PointF(0f, 0f),
			new PointF(1f, 0f),
			new PointF(0f, 1f),
			new PointF(1f, 1f)
		};

		private float _z = 0.5f;

		private bool _isTransformSetup;

		private TransformType positionType;

		private float positionTime;

		private PointF positionTemp;

		private Point positionTo;

		private Point positionLoop;

		private float positionSpeedX;

		private float positionSpeedY;

		private float positionLoopTime;

		private TransformFunction positionFunctionX;

		private TransformFunction positionFunctionY;

		private TransformType scaleType;

		private float scaleTime;

		private SizeF scaleTo;

		private SizeF scaleLoop;

		private float scaleSpeedX;

		private float scaleSpeedY;

		private float scaleLoopTime;

		private TransformFunction scaleFunctionX;

		private TransformFunction scaleFunctionY;

		private TransformType rotateType;

		private float rotateTime;

		private float rotateTo;

		private float rotateLoop;

		private float rotateSpeed;

		private float rotateLoopTime;

		private TransformFunction rotateFunction;

		private TransformType alphaType;

		private float alphaTime;

		private int alphaTo;

		private int alphaLoop;

		private float alphaSpeed;

		private float alphaLoopTime;

		private TransformFunction alphaFunction;

		private float alphaTemp;

		private bool _isSetupCircle;

		private bool isCloned;

		public virtual Size Size
		{
			get
			{
				return _size;
			}
			set
			{
				if (_textured)
				{
					throw new AccessViolationException("Cannot set textures' size");
				}
				_size = value;
				RefreshVerts();
			}
		}

		public virtual int Width
		{
			get
			{
				return _size.Width;
			}
			set
			{
				if (_textured)
				{
					throw new AccessViolationException("Cannot set textures' size");
				}
				_size.Width = value;
				RefreshVerts();
			}
		}

		public virtual int Height
		{
			get
			{
				return _size.Height;
			}
			set
			{
				if (_textured)
				{
					throw new AccessViolationException("Cannot set textures' size");
				}
				_size.Height = value;
				RefreshVerts();
			}
		}

		public virtual Point Center
		{
			get
			{
				return _center;
			}
			set
			{
				_center = value;
				RefreshVerts();
			}
		}

		public virtual int CenterX
		{
			get
			{
				return _center.X;
			}
			set
			{
				_center.X = value;
				RefreshVerts();
			}
		}

		public virtual int CenterY
		{
			get
			{
				return _center.Y;
			}
			set
			{
				_center.Y = value;
				RefreshVerts();
			}
		}

		public virtual Point Position
		{
			get
			{
				return _position;
			}
			set
			{
				_position = value;
				RefreshVerts();
			}
		}

		public virtual int X
		{
			get
			{
				return _position.X;
			}
			set
			{
				_position.X = value;
				RefreshVerts();
			}
		}

		public virtual int Y
		{
			get
			{
				return _position.Y;
			}
			set
			{
				_position.Y = value;
				RefreshVerts();
			}
		}

		public virtual SizeF Scale
		{
			get
			{
				return _scale;
			}
			set
			{
				_scale = value;
				RefreshVerts();
			}
		}

		public virtual float ScaleX
		{
			get
			{
				return _scale.Width;
			}
			set
			{
				_scale.Width = value;
				RefreshVerts();
			}
		}

		public virtual float ScaleY
		{
			get
			{
				return _scale.Height;
			}
			set
			{
				_scale.Height = value;
				RefreshVerts();
			}
		}

		public virtual float Rotate
		{
			get
			{
				return _rotate;
			}
			set
			{
				_rotate = value;
				RefreshVerts();
			}
		}

		public virtual int Alpha
		{
			get
			{
				return _alpha;
			}
			set
			{
				_alpha = value;
				fAlpha = value;
			}
		}

		public virtual Color ArgumentColor
		{
			get
			{
				return _argumentColor;
			}
			set
			{
				_argumentColor = value;
			}
		}

		public virtual float Time
		{
			get
			{
				return _time;
			}
		}

		internal PointF Vertex1
		{
			get
			{
				return verts[0];
			}
		}

		internal PointF Vertex2
		{
			get
			{
				return verts[1];
			}
		}

		internal PointF Vertex3
		{
			get
			{
				return verts[2];
			}
		}

		internal PointF Vertex4
		{
			get
			{
				return verts[3];
			}
		}

		public Texture Texture
		{
			get
			{
				return _texture;
			}
			protected set
			{
				_texture = value;
			}
		}

		public bool Textured
		{
			get
			{
				return _textured;
			}
			protected set
			{
				_textured = value;
			}
		}

		public Game2DCore Core
		{
			get
			{
				return _core;
			}
			protected set
			{
				_core = value;
			}
		}

		public virtual bool IsCreated
		{
			get
			{
				return _isCreated;
			}
			protected set
			{
				_isCreated = value;
			}
		}

		public RectangleF BoundRect
		{
			get
			{
				if (_bounds[0].X == _bounds[2].X && _bounds[1].X == _bounds[3].X && _bounds[0].Y == _bounds[1].Y && _bounds[2].Y == _bounds[3].Y)
				{
					return new RectangleF(_bounds[0].X, _bounds[0].Y, _bounds[3].X - _bounds[0].X, _bounds[3].Y - _bounds[0].Y);
				}
				return RectangleF.Empty;
			}
			protected set
			{
				PointF[] bounds = _bounds;
				float x = (_bounds[2].X = value.Left);
				bounds[0].X = x;
				PointF[] bounds2 = _bounds;
				float x2 = (_bounds[3].X = value.Right);
				bounds2[1].X = x2;
				PointF[] bounds3 = _bounds;
				float y = (_bounds[1].Y = value.Top);
				bounds3[0].Y = y;
				PointF[] bounds4 = _bounds;
				float y2 = (_bounds[3].Y = value.Bottom);
				bounds4[2].Y = y2;
			}
		}

		public PointF[] Bounds
		{
			get
			{
				return _bounds;
			}
			protected set
			{
				_bounds = value;
			}
		}

		public float Z
		{
			get
			{
				return _z;
			}
			set
			{
				_z = value;
			}
		}

		protected bool IsTransformSetup
		{
			get
			{
				return _isTransformSetup;
			}
		}

		protected bool IsSetupCircle
		{
			get
			{
				return _isSetupCircle;
			}
			set
			{
				if (!value)
				{
					_isSetupCircle = false;
				}
			}
		}

		public virtual void BeginTransform(float time)
		{
			_time = time;
			_isTransformSetup = true;
			_isSetupCircle = true;
			positionType = TransformType.None;
			rotateType = TransformType.None;
			scaleType = TransformType.None;
			alphaType = TransformType.None;
		}

		public virtual void EndTransform()
		{
			_isTransformSetup = false;
		}

		public virtual void ResetTime()
		{
			if (_isTransformSetup)
			{
				throw new Exception("Cannot reset time inside transform setup.");
			}
			_time = float.NaN;
		}

		public virtual void PositionTo(Point position)
		{
			if (!_isTransformSetup)
			{
				throw new Exception("Cannot set position transform outside transform setup.");
			}
			positionType = TransformType.Global;
			positionTo = position;
			positionTemp.X = _position.X;
			positionTemp.Y = _position.Y;
		}

		public virtual void PositionTo(Point position, float time)
		{
			if (!_isTransformSetup)
			{
				throw new Exception("Cannot set position transform outside transform setup.");
			}
			positionType = TransformType.Local;
			positionTime = time;
			positionTo = position;
			positionTemp.X = _position.X;
			positionTemp.Y = _position.Y;
		}

		public virtual void PositionTo(Point position1, Point position2, float time)
		{
			if (!_isTransformSetup)
			{
				throw new Exception("Cannot set position transform outside transform setup.");
			}
			positionType = TransformType.Loop;
			positionLoopTime = time;
			positionTo = position1;
			positionLoop = position2;
			positionSpeedX = (float)(positionLoop.X - positionTo.X) / (time / 2f);
			positionSpeedY = (float)(positionLoop.Y - positionTo.Y) / (time / 2f);
			positionTime = (float)(_position.X - positionTo.X) / positionSpeedX;
			positionTemp.X = _position.X;
			positionTemp.Y = _position.Y;
		}

		public virtual void PositionTo(TransformFunction functionX, TransformFunction functionY)
		{
			if (!_isTransformSetup)
			{
				throw new Exception("Cannot set position transform outside transform setup.");
			}
			positionType = TransformType.Function;
			positionFunctionX = functionX;
			positionFunctionY = functionY;
			positionTime = 0f;
			positionTemp.X = _position.X;
			positionTemp.Y = _position.Y;
		}

		public virtual void ScaleTo(SizeF scale)
		{
			if (!_isTransformSetup)
			{
				throw new Exception("Cannot set scale transform outside transform setup.");
			}
			scaleType = TransformType.Global;
			scaleTo = scale;
		}

		public virtual void ScaleTo(SizeF scale, float time)
		{
			if (!_isTransformSetup)
			{
				throw new Exception("Cannot set scale transform outside transform setup.");
			}
			scaleType = TransformType.Local;
			scaleTime = time;
			scaleTo = scale;
		}

		public virtual void ScaleTo(SizeF scale1, SizeF scale2, float time)
		{
			if (!_isTransformSetup)
			{
				throw new Exception("Cannot set scale transform outside transform setup.");
			}
			scaleType = TransformType.Loop;
			scaleLoopTime = time;
			scaleTo = scale1;
			scaleLoop = scale2;
			scaleSpeedX = (scaleLoop.Width - scaleTo.Width) / (time / 2f);
			scaleSpeedY = (scaleLoop.Height - scaleTo.Height) / (time / 2f);
			scaleTime = (_scale.Width - scaleTo.Width) / scaleSpeedX;
		}

		public virtual void ScaleTo(TransformFunction functionX, TransformFunction functionY)
		{
			if (!_isTransformSetup)
			{
				throw new Exception("Cannot set scale transform outside transform setup.");
			}
			scaleType = TransformType.Function;
			scaleFunctionX = functionX;
			scaleFunctionY = functionY;
			scaleTime = 0f;
		}

		public virtual void RotateTo(float rotate)
		{
			if (!_isTransformSetup)
			{
				throw new Exception("Cannot set rotate transform outside transform setup.");
			}
			rotateType = TransformType.Global;
			rotateTo = rotate;
		}

		public virtual void RotateTo(float rotate, float time)
		{
			if (!_isTransformSetup)
			{
				throw new Exception("Cannot set rotate transform outside transform setup.");
			}
			rotateType = TransformType.Local;
			rotateTime = time;
			rotateTo = rotate;
		}

		public virtual void RotateTo(float rotate1, float rotate2, float time)
		{
			if (!_isTransformSetup)
			{
				throw new Exception("Cannot set rotate transform outside transform setup.");
			}
			rotateType = TransformType.Loop;
			rotateLoopTime = time;
			rotateTo = rotate1;
			rotateLoop = rotate2;
			rotateSpeed = (rotateLoop - rotateTo) / (time / 2f);
			rotateTime = (_rotate - rotateTo) / rotateSpeed;
		}

		public virtual void RotateTo(TransformFunction function)
		{
			if (!_isTransformSetup)
			{
				throw new Exception("Cannot set rotate transform outside transform setup.");
			}
			rotateType = TransformType.Function;
			rotateFunction = function;
			rotateTime = 0f;
		}

		public virtual void AlphaTo(int alpha)
		{
			if (!_isTransformSetup)
			{
				throw new Exception("Cannot set alpha transform outside transform setup.");
			}
			alphaType = TransformType.Global;
			alphaTo = alpha;
			alphaTemp = _alpha;
		}

		public virtual void AlphaTo(int alpha, float time)
		{
			if (!_isTransformSetup)
			{
				throw new Exception("Cannot set alpha transform outside transform setup.");
			}
			alphaType = TransformType.Local;
			alphaTime = time;
			alphaTo = alpha;
			alphaTemp = _alpha;
		}

		public virtual void AlphaTo(int alpha1, int alpha2, float time)
		{
			if (!_isTransformSetup)
			{
				throw new Exception("Cannot set alpha transform outside transform setup.");
			}
			alphaType = TransformType.Loop;
			alphaLoopTime = time;
			alphaTo = alpha1;
			alphaLoop = alpha2;
			alphaSpeed = (float)(alphaLoop - alphaTo) / (time / 2f);
			alphaTime = (float)(_alpha - alphaTo) / alphaSpeed;
			alphaTemp = _alpha;
		}

		public virtual void AlphaTo(TransformFunction function)
		{
			if (!_isTransformSetup)
			{
				throw new Exception("Cannot set alpha transform outside transform setup.");
			}
			alphaType = TransformType.Function;
			alphaFunction = function;
			alphaTime = 0f;
			alphaTemp = _alpha;
		}

		public virtual void Update(float elaspedTime)
		{
			if (_isTransformSetup)
			{
				throw new Exception("Cannot update inside transform setup.");
			}
			if (_isSetupCircle)
			{
				Update();
				_isSetupCircle = false;
				return;
			}
			bool flag = false;
			if (UpdatePosition(elaspedTime))
			{
				flag = true;
			}
			if (UpdateScale(elaspedTime))
			{
				flag = true;
			}
			if (UpdateRotate(elaspedTime))
			{
				flag = true;
			}
			UpdateAlpha(elaspedTime);
			if (_time > elaspedTime)
			{
				_time -= elaspedTime;
			}
			else if (_time > 0f)
			{
				_time = 0f;
			}
			if (flag)
			{
				verts = CalculateVerts(_position, _scale, _rotate);
			}
			Update();
		}

		private bool UpdatePosition(float elaspedTime)
		{
			bool flag = false;
			switch (positionType)
			{
			case TransformType.Global:
				if (_time > elaspedTime)
				{
					flag = true;
					float num = elaspedTime / _time;
					positionTemp.X += ((float)positionTo.X - positionTemp.X) * num;
					positionTemp.Y += ((float)positionTo.Y - positionTemp.Y) * num;
				}
				else if (_time > 0f)
				{
					flag = true;
					positionTemp.X = positionTo.X;
					positionTemp.Y = positionTo.Y;
				}
				break;
			case TransformType.Local:
				if (positionTime > elaspedTime)
				{
					flag = true;
					float num = elaspedTime / positionTime;
					positionTemp.X += ((float)positionTo.X - positionTemp.X) * num;
					positionTemp.Y += ((float)positionTo.Y - positionTemp.Y) * num;
					positionTime -= elaspedTime;
				}
				else if (positionTime > 0f)
				{
					flag = true;
					positionTemp.X = positionTo.X;
					positionTemp.Y = positionTo.Y;
					positionTime = 0f;
				}
				break;
			case TransformType.Loop:
				flag = true;
				positionTime -= elaspedTime;
				while (positionTime < (0f - positionLoopTime) / 2f)
				{
					positionTime += positionLoopTime;
				}
				positionTemp.X = (float)positionTo.X + Math.Abs(positionTime) * positionSpeedX;
				positionTemp.Y = (float)positionTo.Y + Math.Abs(positionTime) * positionSpeedY;
				break;
			case TransformType.Function:
				flag = true;
				positionTime += elaspedTime;
				positionTemp.X = positionFunctionX(positionTime);
				positionTemp.Y = positionFunctionY(positionTime);
				break;
			}
			if (flag)
			{
				_position.X = (int)((double)positionTemp.X + 0.5);
				_position.Y = (int)((double)positionTemp.Y + 0.5);
			}
			return flag;
		}

		private bool UpdateScale(float elaspedTime)
		{
			bool result = false;
			switch (scaleType)
			{
			case TransformType.Global:
				if (_time > elaspedTime)
				{
					result = true;
					float num = elaspedTime / _time;
					_scale.Width += (scaleTo.Width - _scale.Width) * num;
					_scale.Height += (scaleTo.Height - _scale.Height) * num;
				}
				else if (_time > 0f)
				{
					result = true;
					_scale.Width = scaleTo.Width;
					_scale.Height = scaleTo.Height;
				}
				break;
			case TransformType.Local:
				if (scaleTime > elaspedTime)
				{
					result = true;
					float num = elaspedTime / scaleTime;
					_scale.Width += (scaleTo.Width - _scale.Width) * num;
					_scale.Height += (scaleTo.Height - _scale.Height) * num;
					scaleTime -= elaspedTime;
				}
				else if (scaleTime > 0f)
				{
					result = true;
					_scale.Width = scaleTo.Width;
					_scale.Height = scaleTo.Height;
					scaleTime = 0f;
				}
				break;
			case TransformType.Loop:
				result = true;
				scaleTime -= elaspedTime;
				while (scaleTime < (0f - scaleLoopTime) / 2f)
				{
					scaleTime += scaleLoopTime;
				}
				_scale.Width = scaleTo.Width + Math.Abs(scaleTime) * scaleSpeedX;
				_scale.Height = scaleTo.Height + Math.Abs(scaleTime) * scaleSpeedY;
				break;
			case TransformType.Function:
				result = true;
				scaleTime += elaspedTime;
				_scale.Width = scaleFunctionX(scaleTime);
				_scale.Height = scaleFunctionX(scaleTime);
				break;
			}
			return result;
		}

		private bool UpdateRotate(float elaspedTime)
		{
			bool result = false;
			switch (rotateType)
			{
			case TransformType.Global:
				if (_time > elaspedTime)
				{
					result = true;
					float num = elaspedTime / _time;
					_rotate += (rotateTo - _rotate) * num;
				}
				else if (_time > 0f)
				{
					result = true;
					_rotate = rotateTo;
				}
				break;
			case TransformType.Local:
				if (rotateTime > elaspedTime)
				{
					result = true;
					float num = elaspedTime / rotateTime;
					_rotate += (rotateTo - _rotate) * num;
					rotateTime -= elaspedTime;
				}
				else if (rotateTime > 0f)
				{
					result = true;
					_rotate = rotateTo;
					rotateTime = 0f;
				}
				break;
			case TransformType.Loop:
				result = true;
				rotateTime -= elaspedTime;
				while (rotateTime < (0f - rotateLoopTime) / 2f)
				{
					rotateTime += rotateLoopTime;
				}
				_rotate = rotateTo + Math.Abs(rotateTime) * rotateSpeed;
				break;
			case TransformType.Function:
				result = true;
				rotateTime += elaspedTime;
				_rotate = rotateFunction(rotateTime);
				break;
			}
			return result;
		}

		private bool UpdateAlpha(float elaspedTime)
		{
			bool flag = false;
			switch (alphaType)
			{
			case TransformType.Global:
				if (_time > elaspedTime)
				{
					flag = true;
					float num = elaspedTime / _time;
					alphaTemp += ((float)alphaTo - alphaTemp) * num;
				}
				else if (_time > 0f)
				{
					flag = true;
					alphaTemp = alphaTo;
				}
				break;
			case TransformType.Local:
				if (alphaTime > elaspedTime)
				{
					flag = true;
					float num = elaspedTime / alphaTime;
					alphaTemp += ((float)alphaTo - alphaTemp) * num;
					alphaTime -= elaspedTime;
				}
				else if (alphaTime > 0f)
				{
					flag = true;
					alphaTemp = alphaTo;
					alphaTime = 0f;
				}
				break;
			case TransformType.Loop:
				flag = true;
				alphaTime -= elaspedTime;
				while (alphaTime < (0f - alphaLoopTime) / 2f)
				{
					alphaTime += alphaLoopTime;
				}
				alphaTemp = (float)alphaTo + Math.Abs(alphaTime) * alphaSpeed;
				break;
			case TransformType.Function:
				flag = true;
				alphaTime += elaspedTime;
				alphaTemp = alphaFunction(alphaTime);
				break;
			}
			if (flag)
			{
				_alpha = (int)((double)alphaTemp + 0.5);
			}
			return flag;
		}

		public void Update()
		{
		}

		public void Draw(Device device, VertexBuffer vb)
		{
			Draw(device, vb, true);
		}

		public virtual void Draw(Device device, VertexBuffer vb, bool target)
		{
			if (_isTransformSetup)
			{
				throw new Exception("Cannot draw inside transform setup.");
			}
			if (!_isCreated || _size.Width == 0 || _size.Height == 0 || _scale.Width == 0f || _scale.Height == 0f || _alpha == 0)
			{
				return;
			}
			if (device == null)
			{
				throw new ArgumentNullException("Device is null while drawing.");
			}
			if (_textured)
			{
				if (_texture == null)
				{
					throw new NullReferenceException("Texture has been disposed.");
				}
				if (vb == null)
				{
					throw new ArgumentNullException("Vertex buffer is null.");
				}
				CustomVertex.TransformedColoredTextured[] array = (CustomVertex.TransformedColoredTextured[])vb.Lock(0, LockFlags.None);
				if (array.Length < 4)
				{
					throw new ArgumentException("Vertex buffer's number of vertices is less than 4.");
				}
				for (int i = 0; i < 4; i++)
				{
					if (target)
					{
						array[i].X = verts[i].X * _core.TargetBounds.Width + _core.TargetBounds.X;
						array[i].Y = verts[i].Y * _core.TargetBounds.Height + _core.TargetBounds.Y;
					}
					else
					{
						array[i].X = verts[i].X;
						array[i].Y = verts[i].Y;
					}
					array[i].Color = _argumentColor.ToArgb();
					array[i].Z = _z;
					array[i].Rhw = 1f;
					array[i].Tu = _bounds[i].X;
					array[i].Tv = _bounds[i].Y;
				}
				vb.Unlock();
				array = null;
				device.SetTexture(0, _texture);
				if (_argumentColor == Color.Transparent)
				{
					device.TextureState[0].ColorOperation = TextureOperation.SelectArg1;
					device.TextureState[0].ColorArgument1 = TextureArgument.TextureColor;
				}
				else
				{
					device.TextureState[0].ColorOperation = TextureOperation.BlendCurrentAlpha;
					device.TextureState[0].ColorArgument1 = TextureArgument.Current;
					device.TextureState[0].ColorArgument2 = TextureArgument.TextureColor;
				}
				if (_alpha == 255)
				{
					device.TextureState[0].AlphaOperation = TextureOperation.SelectArg1;
					device.TextureState[0].AlphaArgument1 = TextureArgument.TextureColor;
				}
				else
				{
					device.TextureState[0].AlphaOperation = TextureOperation.Modulate;
					device.TextureState[0].AlphaArgument1 = TextureArgument.TextureColor;
					device.TextureState[0].AlphaArgument2 = TextureArgument.TFactor;
				}
				if (_alpha < 255 || _argumentColor != Color.Transparent)
				{
					device.RenderState.TextureFactor = Color.FromArgb(_alpha, Color.Black).ToArgb();
				}
				device.SamplerState[0].MagFilter = TextureFilter.Anisotropic;
				device.SamplerState[0].MinFilter = TextureFilter.Anisotropic;
				device.SamplerState[0].AddressU = TextureAddress.Clamp;
				device.SamplerState[0].AddressV = TextureAddress.Clamp;
				device.SetStreamSource(0, vb, 0);
				device.VertexFormat = VertexFormats.Texture1 | VertexFormats.Diffuse | VertexFormats.Transformed;
				device.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);
				device.SetTexture(0, null);
			}
			else
			{
				if (_color == Color.Transparent)
				{
					return;
				}
				if (vb == null)
				{
					throw new ArgumentNullException("Vertex buffer is null.");
				}
				CustomVertex.TransformedColoredTextured[] array2 = (CustomVertex.TransformedColoredTextured[])vb.Lock(0, LockFlags.None);
				if (array2.Length < 4)
				{
					throw new ArgumentException("Vertex buffer's number of vertices is less than 4.");
				}
				for (int j = 0; j < 4; j++)
				{
					if (target)
					{
						array2[j].X = verts[j].X * _core.TargetBounds.Width + _core.TargetBounds.X;
						array2[j].Y = verts[j].Y * _core.TargetBounds.Height + _core.TargetBounds.Y;
					}
					else
					{
						array2[j].X = verts[j].X;
						array2[j].Y = verts[j].Y;
					}
					array2[j].Z = _z;
					array2[j].Rhw = 1f;
					array2[j].Color = Color.FromArgb(_alpha * _color.A / 255, _color).ToArgb();
					array2[j].Tu = j & 1;
					array2[j].Tv = j >> 1;
				}
				vb.Unlock();
				array2 = null;
				device.SetStreamSource(0, vb, 0);
				device.VertexFormat = VertexFormats.Texture1 | VertexFormats.Diffuse | VertexFormats.Transformed;
				device.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);
			}
		}

		protected PointF[] CalculateVerts(Point position, SizeF scale, float rotate)
		{
			PointF[] array = new PointF[4];
			array[0].X = -_center.X;
			array[0].Y = -_center.Y;
			array[1].X = _size.Width - _center.X;
			array[1].Y = -_center.Y;
			array[2].X = -_center.X;
			array[2].Y = _size.Height - _center.Y;
			array[3].X = _size.Width - _center.X;
			array[3].Y = _size.Height - _center.Y;
			for (int i = 0; i < 4; i++)
			{
				array[i].X *= scale.Width;
				array[i].Y *= scale.Height;
				float x = (float)((double)array[i].X * Math.Cos(rotate) + (double)array[i].Y * Math.Sin(rotate));
				float y = (float)((double)(0f - array[i].X) * Math.Sin(rotate) + (double)array[i].Y * Math.Cos(rotate));
				array[i].X = x;
				array[i].Y = y;
				array[i].X += position.X;
				array[i].Y += position.Y;
				array[i].X -= 0.5f;
				array[i].Y -= 0.5f;
			}
			return array;
		}

		protected void RefreshVerts()
		{
			verts = CalculateVerts(_position, _scale, _rotate);
		}

		protected void CreateTextureThreadProc(object source)
		{
			if (source is string)
			{
				string srcFile = source as string;
				try
				{
					ImageInformation imageInformation = TextureLoader.ImageInformationFromFile(srcFile);
					_size.Width = imageInformation.Width;
					_size.Height = imageInformation.Height;
					_texture = TextureLoader.FromFile(_core.Device, srcFile, _size.Width, _size.Height, 1, Usage.None, Format.A8R8G8B8, Pool.Managed, Filter.None, Filter.None, 0);
				}
				catch
				{
				}
				RefreshVerts();
				_isCreated = true;
			}
			else if (source is Stream)
			{
				try
				{
					Stream stream = source as Stream;
					stream.Seek(0L, SeekOrigin.Begin);
					ImageInformation imageInformation2 = TextureLoader.ImageInformationFromStream(stream);
					_size.Width = imageInformation2.Width;
					_size.Height = imageInformation2.Height;
					stream.Seek(0L, SeekOrigin.Begin);
					_texture = TextureLoader.FromStream(_core.Device, stream, _size.Width, _size.Height, 1, Usage.None, Format.A8R8G8B8, Pool.Managed, Filter.None, Filter.None, 0);
					stream.Close();
					stream.Dispose();
				}
				catch
				{
				}
				RefreshVerts();
				_isCreated = true;
			}
		}

		internal Picture(Game2DCore core, string fileName)
		{
			if (core == null)
			{
				throw new ArgumentNullException("Core must not be null.");
			}
			if (core.Device == null)
			{
				throw new ArgumentException("Core's device must not be null.");
			}
			_core = core;
			core.PictureList.Add(this);
			_textured = true;
			new Thread(CreateTextureThreadProc).Start(fileName);
		}

		internal Picture(Game2DCore core, Stream stream)
		{
			if (core == null)
			{
				throw new ArgumentNullException("Core must not be null.");
			}
			if (core.Device == null)
			{
				throw new ArgumentException("Core's device must not be null.");
			}
			_core = core;
			core.PictureList.Add(this);
			_textured = true;
			new Thread(CreateTextureThreadProc).Start(stream);
		}

		internal Picture(Game2DCore core, Color color)
		{
			if (core == null)
			{
				throw new ArgumentNullException("Core must not be null.");
			}
			_core = core;
			core.PictureList.Add(this);
			_textured = false;
			_color = color;
			RefreshVerts();
			_isCreated = true;
		}

		protected Picture()
		{
		}

		internal static Picture CreateInstant(Game2DCore core, string fileName)
		{
			if (core == null)
			{
				throw new ArgumentNullException("Core must not be null.");
			}
			if (core.Device == null)
			{
				throw new ArgumentException("Core's device must not be null.");
			}
			Picture picture = new Picture();
			picture._core = core;
			core.PictureList.Add(picture);
			picture._textured = true;
			picture.CreateTextureThreadProc(fileName);
			return picture;
		}

		internal static Picture CreateInstant(Game2DCore core, Stream stream)
		{
			if (core == null)
			{
				throw new ArgumentNullException("Core must not be null.");
			}
			if (core.Device == null)
			{
				throw new ArgumentException("Core's device must not be null.");
			}
			Picture picture = new Picture();
			picture._core = core;
			core.PictureList.Add(picture);
			picture._textured = true;
			picture.CreateTextureThreadProc(stream);
			return picture;
		}

		public virtual void Dispose()
		{
			if (!isCloned)
			{
				if (_texture != null)
				{
					_texture.Dispose();
					_texture = null;
				}
			}
			else
			{
				_texture = null;
			}
			_core.PictureList.Remove(this);
		}

		~Picture()
		{
			Dispose();
		}

		public virtual object Clone()
		{
			Picture picture = new Picture();
			picture._core = _core;
			picture._isCreated = _isCreated;
			picture._textured = _textured;
			picture._texture = _texture;
			picture._color = _color;
			picture._size = _size;
			picture._center = _center;
			picture._position = _position;
			picture._scale = _scale;
			picture._rotate = _rotate;
			picture._alpha = _alpha;
			picture._argumentColor = _argumentColor;
			picture._bounds = (PointF[])_bounds.Clone();
			picture._z = _z;
			picture.isCloned = true;
			return picture;
		}
	}
}
