using System;
using Microsoft.DirectX.Direct3D;

namespace KidFansChannel.Game2DCore
{
	public class HlslPicture : Picture
	{
		public delegate void SetValueFunc();

		private Texture textureSrc;

		private bool ready;

		private string _hlsl;

		private SetValueFunc _setValue;

		private static bool _shader = false;

		private static Device _device;

		private Effect _effect;

		public string Hlsl
		{
			get
			{
				return _hlsl;
			}
			set
			{
				_hlsl = value;
			}
		}

		public SetValueFunc SetValue
		{
			get
			{
				return _setValue;
			}
			set
			{
				_setValue = value;
			}
		}

		public Effect Effect
		{
			get
			{
				return _effect;
			}
		}

		internal HlslPicture(Game2DCore core, Picture pictureSource)
		{
			if (core == null)
			{
				throw new ArgumentNullException("Core must not be null.");
			}
			Core = core;
			core.PictureList.Add(this);
			Size = pictureSource.Size;
			Texture = new Texture(core.Device, Size.Width, Size.Height, 1, Usage.None, Format.A8R8G8B8, Pool.Managed);
			Textured = true;
			textureSrc = pictureSource.Texture;
			IsCreated = true;
		}

		public static void SetShader(Device device)
		{
			_shader = true;
			_device = device;
		}

		public static void ClearShader()
		{
			_shader = false;
		}

		public override void Draw(Device device, VertexBuffer vb, bool target)
		{
			if (_shader && _hlsl != null)
			{
				if (_effect == null)
				{
					_effect = Effect.FromString(device, string.Format(_hlsl, "\r\nTexture g_Texture;\r\nsampler TextureSampler = sampler_state\r\n{\r\n    texture = <g_Texture>;\r\n    MinFilter = ANISOTROPIC;\r\n    MagFilter = ANISOTROPIC;\r\n    AddressU = Clamp;\r\n    AddressV = Clamp;\r\n};\r\n\r\nvoid HlslPS(\r\n    in float2 texCoord : TEXCOORD0,\r\n    out float4 color : COLOR0\r\n    )\r\n{", "};\r\n\r\ntechnique HlslPicture\r\n{\r\n    pass P0\r\n    {\r\n        VertexShader = null;\r\n        PixelShader = compile ps_2_0 HlslPS();\r\n    }\r\n}\r\n"), null, null, ShaderFlags.None, null);
				}
				if (_setValue != null)
				{
					_setValue();
				}
				_effect.SetValue("g_Texture", textureSrc);
				_effect.Technique = "HlslPicture";
				bool textured = Textured;
				Textured = false;
				int passCount = _effect.Begin(FX.None);
				for (int i = 0; i < passCount; i++)
				{
					_effect.BeginPass(i);
					base.Draw(device, vb, target);
					_effect.EndPass();
				}
				_effect.End();
				Textured = textured;
				return;
			}
			if (_effect != null)
			{
				_effect.Dispose();
				_effect = null;
			}
			if (!ready)
			{
				using (Surface srcSurface = textureSrc.GetSurfaceLevel(0))
				{
					using (Surface destSurface = Texture.GetSurfaceLevel(0))
					{
						SurfaceLoader.FromSurface(destSurface, srcSurface, Filter.None, 0);
					}
				}
				ready = true;
			}
			base.Draw(device, vb, target);
		}

		public override void Dispose()
		{
			base.Dispose();
			try
			{
				if (_effect != null)
				{
					_effect.Dispose();
					_effect = null;
				}
			}
			catch
			{
			}
		}
	}
}
