using System;
using Microsoft.DirectX.Direct3D;

namespace KidFansChannel.Game2DCore
{
	public class MixPicture : Picture
	{
		private Texture textureA;

		private Texture textureB;

		private float _mixAlpha;

		private float invMixAlpha;

		private static bool _shader = false;

		private static Effect _effect;

		public float MixAlpha
		{
			get
			{
				return _mixAlpha;
			}
			set
			{
				if (value < 0f || value > 1f)
				{
					throw new ArgumentOutOfRangeException();
				}
				_mixAlpha = value;
				invMixAlpha = 1f - value;
			}
		}

		public static Effect Effect
		{
			get
			{
				return _effect;
			}
		}

		internal MixPicture(Game2DCore core, Picture pictureA, Picture pictureB)
		{
			if (core == null)
			{
				throw new ArgumentNullException("Core must not be null.");
			}
			if (!pictureA.GetType().Equals(typeof(Picture)) || !pictureB.GetType().Equals(typeof(Picture)))
			{
				throw new NotSupportedException("Only support basic pictures.");
			}
			if (pictureA.Size != pictureB.Size)
			{
				throw new ArgumentException("Unmatched size for two pictures");
			}
			Core = core;
			core.PictureList.Add(this);
			Size = pictureA.Size;
			Texture = new Texture(core.Device, Size.Width, Size.Height, 1, Usage.None, Format.A8R8G8B8, Pool.Managed);
			Textured = true;
			_mixAlpha = 0f;
			invMixAlpha = 1f;
			textureA = pictureA.Texture;
			textureB = pictureB.Texture;
			IsCreated = true;
		}

		public static void SetShader(Device device)
		{
			_shader = true;
			if (_effect == null)
			{
				_effect = Effect.FromString(device, "\r\nfloat g_Alpha;\r\nTexture g_TextureA;\r\nTexture g_TextureB;\r\nfloat4 g_ArgColor;\r\nsampler TextureSamplerA = sampler_state\r\n{\r\n    texture = <g_TextureA>;\r\n    MinFilter = ANISOTROPIC;\r\n    MagFilter = ANISOTROPIC;\r\n    AddressU = Clamp;\r\n    AddressV = Clamp;\r\n};\r\n\r\nsampler TextureSamplerB = sampler_state\r\n{\r\n    texture = <g_TextureB>;\r\n    MinFilter = ANISOTROPIC;\r\n    MagFilter = ANISOTROPIC;\r\n    AddressU = Clamp;\r\n    AddressV = Clamp;\r\n};\r\n\r\nvoid MixPS(\r\n    in float2 texCoord : TEXCOORD0,\r\n    out float4 color : COLOR0\r\n    )\r\n{\r\n    float4 colorA = tex2D(TextureSamplerA, texCoord);\r\n    float4 colorB = tex2D(TextureSamplerB, texCoord);\r\n    if (colorA.a == 0 && colorB.a == 0)\r\n        color = float4(0, 0, 0, 0);\r\n    else if (colorA.a == 0)\r\n        color = float4(colorB.rgb, colorB.a * g_Alpha);\r\n    else if (colorB.a == 0)\r\n        color = float4(colorA.rgb, colorA.a * g_Alpha);\r\n    else\r\n        color = lerp(colorA, colorB, g_Alpha);\r\n    if (g_ArgColor.a > 0)\r\n        color.rgb = color.rgb * (1 - g_ArgColor.a) + g_ArgColor.rgb * g_ArgColor.a;\r\n};\r\n\r\ntechnique MixPicture\r\n{\r\n    pass P0\r\n    {\r\n        VertexShader = null;\r\n        PixelShader = compile ps_2_0 MixPS();\r\n    }\r\n}\r\n", null, null, ShaderFlags.None, null);
			}
		}

		public static void ClearShader()
		{
			_shader = false;
			if (_effect != null)
			{
				_effect.Dispose();
				_effect = null;
			}
		}

		public unsafe override void Draw(Device device, VertexBuffer vb, bool target)
		{
			if (_shader)
			{
				_effect.SetValue("g_Alpha", _mixAlpha);
				_effect.SetValue("g_TextureA", textureA);
				_effect.SetValue("g_TextureB", textureB);
				_effect.SetValue("g_ArgColor", new ColorValue(ArgumentColor.R, ArgumentColor.G, ArgumentColor.B, ArgumentColor.A));
				_effect.Technique = "MixPicture";
				Textured = false;
				int passCount = _effect.Begin(FX.None);
				for (int i = 0; i < passCount; i++)
				{
					_effect.BeginPass(i);
					base.Draw(device, vb, target);
					_effect.EndPass();
				}
				_effect.End();
				Textured = true;
				return;
			}
			uint* ptr = (uint*)Texture.LockRectangle(0, LockFlags.None).InternalData.ToPointer();
			uint* ptr2 = (uint*)textureA.LockRectangle(0, LockFlags.None).InternalData.ToPointer();
			uint* ptr3 = (uint*)textureB.LockRectangle(0, LockFlags.None).InternalData.ToPointer();
			for (int j = 0; j < Height; j++)
			{
				for (int k = 0; k < Width; k++)
				{
					uint srcAlpha = *ptr2 >> 24;
					uint dstAlpha = *ptr3 >> 24;
					if (srcAlpha == 0 && dstAlpha == 0)
					{
						*ptr = 0u;
					}
					else if (srcAlpha == 0)
					{
						uint num4 = (uint)((float)dstAlpha * _mixAlpha + 0.5f);
						*ptr = (*ptr3 & 0xFFFFFF) | (num4 << 24);
					}
					else if (dstAlpha == 0)
					{
						uint num5 = (uint)((float)srcAlpha * invMixAlpha + 0.5f);
						*ptr = (*ptr2 & 0xFFFFFF) | (num5 << 24);
					}
					else if (*ptr2 == *ptr3)
					{
						*ptr = *ptr2;
					}
					else
					{
						uint num6 = (uint)((float)srcAlpha * invMixAlpha + (float)dstAlpha * _mixAlpha + 0.5f);
						uint num7 = (uint)((float)(*ptr2 & 0xFF) * invMixAlpha + (float)(*ptr3 & 0xFF) * _mixAlpha + 0.5f);
						uint num8 = (uint)((float)((*ptr2 >> 8) & 0xFF) * invMixAlpha + (float)((*ptr3 >> 8) & 0xFF) * _mixAlpha + 0.5f);
						uint num9 = (uint)((float)((*ptr2 >> 16) & 0xFF) * invMixAlpha + (float)((*ptr3 >> 16) & 0xFF) * _mixAlpha + 0.5f);
						*ptr = num7 | (num8 << 8) | (num9 << 16) | (num6 << 24);
					}
					ptr++;
					ptr2++;
					ptr3++;
				}
			}
			Texture.UnlockRectangle(0);
			textureA.UnlockRectangle(0);
			textureB.UnlockRectangle(0);
			base.Draw(device, vb, target);
		}
	}
}
