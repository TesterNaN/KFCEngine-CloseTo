using System;
using System.Collections.Generic;
using System.Drawing;
using Microsoft.DirectX.Direct3D;

namespace KidFansChannel.Game2DCore
{
	public class BindingPicture : Picture
	{
		private List<Picture> pictures;

		private List<Point> locations;

		private List<bool> visibles;

		private Size _size = Size.Empty;

		public override Size Size
		{
			get
			{
				return _size;
			}
			set
			{
				_size = value;
			}
		}

		public override int Width
		{
			get
			{
				return _size.Width;
			}
		}

		public override int Height
		{
			get
			{
				return _size.Height;
			}
		}

		public override Point Center
		{
			get
			{
				return base.Center;
			}
			set
			{
				base.Center = value;
				for (int i = 0; i < pictures.Count; i++)
				{
					pictures[i].CenterX = value.X - locations[i].X;
					pictures[i].CenterY = value.Y - locations[i].Y;
				}
			}
		}

		public override int CenterX
		{
			get
			{
				return base.CenterX;
			}
			set
			{
				base.CenterX = value;
				for (int i = 0; i < pictures.Count; i++)
				{
					pictures[i].CenterX = value - locations[i].X;
				}
			}
		}

		public override int CenterY
		{
			get
			{
				return base.CenterY;
			}
			set
			{
				base.CenterY = value;
				for (int i = 0; i < pictures.Count; i++)
				{
					pictures[i].CenterY = value - locations[i].Y;
				}
			}
		}

		public override Point Position
		{
			get
			{
				return base.Position;
			}
			set
			{
				base.Position = value;
				for (int i = 0; i < pictures.Count; i++)
				{
					pictures[i].Position = value;
				}
			}
		}

		public override int X
		{
			get
			{
				return base.X;
			}
			set
			{
				base.X = value;
				for (int i = 0; i < pictures.Count; i++)
				{
					pictures[i].X = value;
				}
			}
		}

		public override int Y
		{
			get
			{
				return base.Y;
			}
			set
			{
				base.Y = value;
				for (int i = 0; i < pictures.Count; i++)
				{
					pictures[i].Y = value;
				}
			}
		}

		public override SizeF Scale
		{
			get
			{
				return base.Scale;
			}
			set
			{
				base.Scale = value;
				for (int i = 0; i < pictures.Count; i++)
				{
					pictures[i].ScaleX = value.Width;
					pictures[i].ScaleY = value.Height;
				}
			}
		}

		public override float ScaleX
		{
			get
			{
				return base.ScaleX;
			}
			set
			{
				base.ScaleX = value;
				for (int i = 0; i < pictures.Count; i++)
				{
					pictures[i].ScaleX = value;
				}
			}
		}

		public override float ScaleY
		{
			get
			{
				return base.ScaleY;
			}
			set
			{
				base.ScaleY = value;
				for (int i = 0; i < pictures.Count; i++)
				{
					pictures[i].ScaleY = value;
				}
			}
		}

		public override float Rotate
		{
			get
			{
				return base.Rotate;
			}
			set
			{
				base.Rotate = value;
				for (int i = 0; i < pictures.Count; i++)
				{
					pictures[i].Rotate = value;
				}
			}
		}

		public override int Alpha
		{
			get
			{
				return base.Alpha;
			}
			set
			{
				base.Alpha = value;
				for (int i = 0; i < pictures.Count; i++)
				{
					pictures[i].Alpha = value;
				}
			}
		}

		public override Color ArgumentColor
		{
			get
			{
				return base.ArgumentColor;
			}
			set
			{
				base.ArgumentColor = value;
				for (int i = 0; i < pictures.Count; i++)
				{
					pictures[i].ArgumentColor = value;
				}
			}
		}

		public override bool IsCreated
		{
			get
			{
				for (int i = 0; i < pictures.Count; i++)
				{
					if (!pictures[i].IsCreated)
					{
						return false;
					}
				}
				return true;
			}
		}

		public override void BeginTransform(float time)
		{
			base.BeginTransform(time);
			foreach (Picture picture in pictures)
			{
				picture.BeginTransform(time);
			}
		}

		public override void EndTransform()
		{
			base.EndTransform();
			foreach (Picture picture in pictures)
			{
				picture.EndTransform();
			}
		}

		public override void ResetTime()
		{
			base.ResetTime();
			foreach (Picture picture in pictures)
			{
				picture.ResetTime();
			}
		}

		public override void PositionTo(Point position)
		{
			base.PositionTo(position);
			foreach (Picture picture in pictures)
			{
				picture.PositionTo(position);
			}
		}

		public override void PositionTo(Point position, float time)
		{
			base.PositionTo(position, time);
			foreach (Picture picture in pictures)
			{
				picture.PositionTo(position, time);
			}
		}

		public override void PositionTo(Point position1, Point position2, float time)
		{
			base.PositionTo(position1, position2, time);
			foreach (Picture picture in pictures)
			{
				picture.PositionTo(position1, position2, time);
			}
		}

		public override void PositionTo(TransformFunction functionX, TransformFunction functionY)
		{
			base.PositionTo(functionX, functionY);
			foreach (Picture picture in pictures)
			{
				picture.PositionTo(functionX, functionY);
			}
		}

		public override void ScaleTo(SizeF scale)
		{
			base.ScaleTo(scale);
			foreach (Picture picture in pictures)
			{
				picture.ScaleTo(scale);
			}
		}

		public override void ScaleTo(SizeF scale, float time)
		{
			base.ScaleTo(scale, time);
			foreach (Picture picture in pictures)
			{
				picture.ScaleTo(scale, time);
			}
		}

		public override void ScaleTo(SizeF scale1, SizeF scale2, float time)
		{
			base.ScaleTo(scale1, scale2, time);
			foreach (Picture picture in pictures)
			{
				picture.ScaleTo(scale1, scale2, time);
			}
		}

		public override void ScaleTo(TransformFunction functionX, TransformFunction functionY)
		{
			base.ScaleTo(functionX, functionY);
			foreach (Picture picture in pictures)
			{
				picture.ScaleTo(functionX, functionY);
			}
		}

		public override void RotateTo(float rotate)
		{
			base.RotateTo(rotate);
			foreach (Picture picture in pictures)
			{
				picture.RotateTo(rotate);
			}
		}

		public override void RotateTo(float rotate, float time)
		{
			base.RotateTo(rotate, time);
			foreach (Picture picture in pictures)
			{
				picture.RotateTo(rotate, time);
			}
		}

		public override void RotateTo(float rotate1, float rotate2, float time)
		{
			base.RotateTo(rotate1, rotate2, time);
			foreach (Picture picture in pictures)
			{
				picture.RotateTo(rotate1, rotate2, time);
			}
		}

		public override void RotateTo(TransformFunction function)
		{
			base.RotateTo(function);
			foreach (Picture picture in pictures)
			{
				picture.RotateTo(function);
			}
		}

		public override void AlphaTo(int alpha)
		{
			base.AlphaTo(alpha);
			foreach (Picture picture in pictures)
			{
				picture.AlphaTo(alpha);
			}
		}

		public override void AlphaTo(int alpha, float time)
		{
			base.AlphaTo(alpha, time);
			foreach (Picture picture in pictures)
			{
				picture.AlphaTo(alpha, time);
			}
		}

		public override void AlphaTo(int alpha1, int alpha2, float time)
		{
			base.AlphaTo(alpha1, alpha2, time);
			foreach (Picture picture in pictures)
			{
				picture.AlphaTo(alpha1, alpha2, time);
			}
		}

		public override void AlphaTo(TransformFunction function)
		{
			base.AlphaTo(function);
			foreach (Picture picture in pictures)
			{
				picture.AlphaTo(function);
			}
		}

		public override void Update(float elaspedTime)
		{
			base.Update(elaspedTime);
			for (int i = 0; i < pictures.Count; i++)
			{
				pictures[i].Update(elaspedTime);
			}
		}

		public override void Draw(Device device, VertexBuffer vb, bool target)
		{
			for (int i = 0; i < pictures.Count; i++)
			{
				if (visibles[i])
				{
					pictures[i].Draw(device, vb, target);
				}
			}
		}

		internal BindingPicture(Game2DCore core)
		{
			if (core == null)
			{
				throw new ArgumentNullException("Core must not be null.");
			}
			Core = core;
			core.PictureList.Add(this);
			Textured = true;
			pictures = new List<Picture>();
			locations = new List<Point>();
			visibles = new List<bool>();
			IsCreated = true;
		}

		public void AddPicture(Picture picture, Point location)
		{
			pictures.Add(picture);
			locations.Add(location);
			visibles.Add(true);
		}

		public void SetVisible(int index, bool value)
		{
			visibles[index] = value;
		}

		public override void Dispose()
		{
			base.Dispose();
			for (int i = 0; i < pictures.Count; i++)
			{
				pictures[i].Dispose();
			}
		}

		public override object Clone()
		{
			return base.Clone();
		}
	}
}
