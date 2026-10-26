using System;
using System.Collections.Generic;
using System.Drawing;

namespace KidFansChannel.Game2DCore
{
	public class AnimePicture : Picture
	{
		private List<Picture> _framePictures = new List<Picture>();

		private List<float> _frameTimes = new List<float>();

		private int currentFrame;

		private float currentTime;

		public List<Picture> FramePictures
		{
			get
			{
				return _framePictures;
			}
		}

		public List<float> FrameTimes
		{
			get
			{
				return _frameTimes;
			}
		}

		public AnimePicture(Game2DCore core, Size size)
		{
			if (core == null)
			{
				throw new ArgumentNullException("Core must not be null.");
			}
			Core = core;
			core.PictureList.Add(this);
			Size = size;
			Textured = true;
			IsCreated = true;
		}

		~AnimePicture()
		{
			_framePictures.Clear();
			_frameTimes.Clear();
		}

		public void AddFrame(Picture picture, float time)
		{
			if (picture.Size != Size || !picture.Textured)
			{
				throw new ArgumentException();
			}
			_framePictures.Add(picture);
			_frameTimes.Add(time);
			if (Texture == null)
			{
				Texture = _framePictures[currentFrame].Texture;
				BoundRect = _framePictures[currentFrame].BoundRect;
			}
		}

		public override void Update(float elaspedTime)
		{
			if (IsSetupCircle)
			{
				base.Update(elaspedTime);
				IsSetupCircle = false;
				return;
			}
			currentTime += elaspedTime;
			while (currentTime >= _frameTimes[currentFrame])
			{
				currentTime -= _frameTimes[currentFrame];
				currentFrame++;
				if (currentFrame == _framePictures.Count)
				{
					currentFrame = 0;
				}
			}
			Texture = _framePictures[currentFrame].Texture;
			BoundRect = _framePictures[currentFrame].BoundRect;
			base.Update(elaspedTime);
		}

		public override void Dispose()
		{
			base.Dispose();
			Texture = null;
		}

		public void ResetFrame()
		{
			currentFrame = 0;
			currentTime = 0f;
		}
	}
}
