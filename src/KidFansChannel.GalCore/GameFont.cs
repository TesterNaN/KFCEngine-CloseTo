using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Runtime.InteropServices;
using KidFansChannel.DataCore;
using KidFansChannel.Game2DCore;

namespace KidFansChannel.GalCore
{
	public class GameFont
	{
		private List<FontFamily> fonts = new List<FontFamily>();

		private int currentIndex = -1;

		private string _currentFont;

		private GalCore galCore;

		private PrivateFontCollection pfc;

		private InstalledFontCollection ifc;

		protected Dictionary<string, FontFamily> DefaultFontFamilys = new Dictionary<string, FontFamily>();

		private Dictionary<int, Font2D> fontSizes = new Dictionary<int, Font2D>();

		public string CurrentFont
		{
			get
			{
				return _currentFont;
			}
			set
			{
				_currentFont = value;
			}
		}

		public event EventHandler FontChanged;

		[DllImport("gdi32.dll")]
		private static extern IntPtr AddFontMemResourceEx(IntPtr pbFont, uint cbFont, IntPtr pdv, [In] ref uint pcFonts);

		~GameFont()
		{
			foreach (Font2D value in fontSizes.Values)
			{
				value.Dispose();
			}
			fontSizes.Clear();
		}

		public void SetCore(GalCore gc)
		{
			galCore = gc;
			galCore.GameLanguage.LanguageChanged += LanguageChanged;
			galCore.GameData.AddData("font");
		}

		private void LanguageChanged(object sender, LanguageEventArgs e)
		{
			fonts.Clear();
			fonts.Add(DefaultFontFamilys[e.Name]);
			if (ifc == null)
			{
				ifc = new InstalledFontCollection();
			}
			FontFamily[] families = ifc.Families;
			foreach (FontFamily fontFamily in families)
			{
				if (fontFamily.IsStyleAvailable(FontStyle.Regular))
				{
					fonts.Add(fontFamily);
				}
			}
			if (galCore != null && galCore.GameOptions != null)
			{
				galCore.GameOptions.Font = ChangeFont(galCore.GameOptions.Font, true);
			}
		}

		public string GetFontName(int index)
		{
			if (index >= fonts.Count)
			{
				throw new IndexOutOfRangeException();
			}
			return fonts[index].Name;
		}

		public int GetFontNumber()
		{
			return fonts.Count;
		}

		public unsafe void LoadFont()
		{
			if (pfc == null)
			{
				pfc = new PrivateFontCollection();
				KidFansChannel.DataCore.DataCore.DataType dataType = galCore.GameData.CheckData("font", "font.ttf", KidFansChannel.DataCore.DataCore.DataType.Package);
				if (dataType == KidFansChannel.DataCore.DataCore.DataType.Package)
				{
					Stream stream = galCore.GameData.GetStream();
					byte[] fontBytes = new byte[stream.Length];
					stream.Read(fontBytes, 0, (int)stream.Length);
					fixed (byte* ptr = &fontBytes[0])
					{
						uint pcFonts = 1u;
						pfc.AddMemoryFont((IntPtr)ptr, fontBytes.Length);
						AddFontMemResourceEx((IntPtr)ptr, (uint)fontBytes.Length, IntPtr.Zero, ref pcFonts);
					}
					DefaultFontFamilys.Add(galCore.GameLanguage.CurrentLanguage, pfc.Families[0]);
				}
				else
				{
					DefaultFontFamilys.Add(galCore.GameLanguage.CurrentLanguage, FontFamily.GenericSansSerif);
				}
				fonts.Clear();
				fonts.Add(DefaultFontFamilys[galCore.GameLanguage.CurrentLanguage]);
			}
			if (ifc == null)
			{
				ifc = new InstalledFontCollection();
			}
			currentIndex = 0;
			_currentFont = "@default";
			FontFamily[] families = ifc.Families;
			foreach (FontFamily fontFamily in families)
			{
				if (fontFamily.IsStyleAvailable(FontStyle.Regular))
				{
					fonts.Add(fontFamily);
				}
			}
			if (galCore != null && galCore.GameOptions != null)
			{
				galCore.GameOptions.Font = ChangeFont(galCore.GameOptions.Font);
			}
		}

		public void ChangeFont(int index)
		{
			ChangeFont(index, false);
		}

		public void ChangeFont(int index, bool always)
		{
			if (always || index != currentIndex)
			{
				currentIndex = index;
				if (index > 0)
				{
					_currentFont = fonts[index].Name;
				}
				else
				{
					_currentFont = "@default";
				}
				if (FontChanged != null)
				{
					FontChanged(this, EventArgs.Empty);
				}
			}
		}

		public string ChangeFont(string font)
		{
			return ChangeFont(font, false);
		}

		public string ChangeFont(string font, bool always)
		{
			if (font == "@default")
			{
				ChangeFont(0, always);
				return font;
			}
			for (int i = 0; i < fonts.Count; i++)
			{
				if (fonts[i].Name == font)
				{
					ChangeFont(i, always);
					return font;
				}
			}
			if (always)
			{
				ChangeFont(0);
				return font;
			}
			return _currentFont;
		}

		public Font2D GetFont(int size)
		{
			if (fontSizes.ContainsKey(size))
			{
				return fontSizes[size];
			}
			fontSizes.Add(size, galCore.GameGraphics.CreateFont(new Font(fonts[currentIndex], size)));
			return fontSizes[size];
		}
	}
}
