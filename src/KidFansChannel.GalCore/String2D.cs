using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using KidFansChannel.GalCore.Commands;
using KidFansChannel.Game2DCore;

namespace KidFansChannel.GalCore
{
	public class String2D : IClickable
	{
		public struct StringChar
		{
			public char Char;

			public char Special;

			public int SpecialArg;

			public StringChar(char c, char s, int arg)
			{
				Char = c;
				Special = s;
				SpecialArg = arg;
			}

			public Size GetSize(Font2D font)
			{
				if (Char == '\0')
				{
					return Size.Empty;
				}
				if (Char == ' ')
				{
					return font.Measure("\b");
				}
				return font.Measure(Char.ToString());
			}

			public StringChar(char c)
				: this(c, '\0', 0)
			{
			}

			public StringChar(char s, int arg)
				: this('\0', s, arg)
			{
			}

			public bool IsSpecial()
			{
				return Special != '\0';
			}

			public bool IsChar()
			{
				return Char != '\0';
			}
		}

		public struct Link
		{
			public int Value;

			public Rectangle Rect;

			public Link(int value, Rectangle rect)
			{
				Value = value;
				Rect = rect;
			}
		}

		public class LinkArgs : EventArgs
		{
			public int Value;

			public bool Read;

			public LinkArgs(int value)
			{
				Value = value;
			}
		}

		public delegate void DrawCharDelegete(Point location, char c, float alpha, Color color, ICanvas canvas, Font2D font);

		private ICanvas _canvas;

		private string _string = string.Empty;

		private Point _location;

		private Size _lineSize;

		private int _lineNum;

		private Font2D _font;

		private Color _defaultColor = Color.White;

		private static int _alphaWidth = 100;

		private List<StringChar> _stringChars = new List<StringChar>();

		private int _width;

		private int _totalWidth;

		private int _currentWidth;

		private int _currentLine;

		private int startPos;

		private string _speaker;

		private Point _click;

		private List<Link> _linkList = new List<Link>();

		private Color _linkColor = Color.Aquamarine;

		private Color _linkColorRead = Color.Aqua;

		private string _comment;

		public DrawCharDelegete DrawCharCallback;

		private int currentLinkValue = -1;

		private int _currentPointedLinkValue = -1;

		public ICanvas Canvas
		{
			get
			{
				return _canvas;
			}
			set
			{
				_canvas = value;
			}
		}

		public string String
		{
			get
			{
				return _string;
			}
		}

		public Point Location
		{
			get
			{
				return _location;
			}
			set
			{
				_location = value;
			}
		}

		public Size LineSize
		{
			get
			{
				return _lineSize;
			}
			set
			{
				_lineSize = value;
			}
		}

		public int LineNum
		{
			get
			{
				return _lineNum;
			}
			set
			{
				_lineNum = value;
			}
		}

		public Font2D Font
		{
			get
			{
				return _font;
			}
			set
			{
				_font = value;
			}
		}

		public Color DefaultColor
		{
			get
			{
				return _defaultColor;
			}
			set
			{
				_defaultColor = value;
			}
		}

		public static int AlphaWidth
		{
			get
			{
				return _alphaWidth;
			}
			set
			{
				_alphaWidth = value;
			}
		}

		public List<StringChar> StringChars
		{
			get
			{
				return _stringChars;
			}
		}

		public int Width
		{
			get
			{
				return _width;
			}
		}

		public int TotalWidth
		{
			get
			{
				return _totalWidth;
			}
		}

		public int CurrentWidth
		{
			get
			{
				return _currentWidth;
			}
			set
			{
				_currentWidth = value;
			}
		}

		public int CurrentLine
		{
			get
			{
				return _currentLine;
			}
			set
			{
				_currentLine = value;
			}
		}

		public int TotalLine
		{
			get
			{
				return _currentLine + 1;
			}
		}

		public string Speaker
		{
			get
			{
				return _speaker;
			}
		}

		public Point Click
		{
			get
			{
				return _click;
			}
		}

		public List<Link> LinkList
		{
			get
			{
				return _linkList;
			}
		}

		public Color LinkColor
		{
			get
			{
				return _linkColor;
			}
			set
			{
				_linkColor = value;
			}
		}

		public Color LinkColorRead
		{
			get
			{
				return _linkColorRead;
			}
			set
			{
				_linkColorRead = value;
			}
		}

		public string Comment
		{
			get
			{
				return _comment;
			}
		}

		public int CurrentPointedLinkValue
		{
			get
			{
				return _currentPointedLinkValue;
			}
			set
			{
				_currentPointedLinkValue = value;
			}
		}

		public bool ToClickState
		{
			get
			{
				return false;
			}
		}

		public event EventHandler<LinkArgs> CheckLinkRead;

		public event EventHandler<LinkArgs> ClickLink;

		public event EventHandler<LinkArgs> MoveLink;

		public Point DrawString()
		{
			return DrawString(1f);
		}

		public virtual void ClearString()
		{
			_speaker = null;
			_comment = null;
			_width = 0;
			_totalWidth = 0;
			_currentWidth = 0;
			_currentLine = 0;
			startPos = 0;
			_stringChars.Clear();
			_string = string.Empty;
		}

		public virtual void AddString(string value)
		{
			AddString(value, ContentAlignment.TopLeft);
		}

		public virtual void AddString(string value, ContentAlignment align)
		{
			ConvertStringFragment(value, align);
			FinishString(align);
			_string += value;
		}

		public void AddXmlString(XmlNodeList xml, NodeInfo info)
		{
			AddXmlString(xml, info, ContentAlignment.TopLeft);
		}

		public virtual void AddXmlString(XmlNodeList xml, NodeInfo info, ContentAlignment align)
		{
			if (align != ContentAlignment.TopLeft && align != ContentAlignment.TopCenter && align != ContentAlignment.TopRight)
			{
				throw new ArgumentOutOfRangeException();
			}
			StringBuilder stringBuilder = new StringBuilder();
			foreach (XmlNode item in xml)
			{
				switch (item.NodeType)
				{
				case XmlNodeType.Element:
					stringBuilder.Append(ConvertXml(item, info, align));
					break;
				case XmlNodeType.Text:
					stringBuilder.Append(item.Value);
					ConvertStringFragment(item.Value, align);
					break;
				default:
					throw new ScriptStructureException(item, info, "String's child notes should be texts, elements or comments");
				case XmlNodeType.Comment:
					break;
				}
			}
			FinishString(align);
			_string += stringBuilder.ToString();
		}

		public void ClearClick()
		{
			for (int i = 0; i < _stringChars.Count; i++)
			{
				if (_stringChars[i].Special == '\r')
				{
					_stringChars.RemoveAt(i);
					i--;
				}
			}
		}

		protected void ConvertStringFragment(string value, ContentAlignment align)
		{
			foreach (char c in value)
			{
				bool flag = false;
				StringChar item;
				if (c == '\n')
				{
					flag = true;
					item = new StringChar('\n', 0);
				}
				else
				{
					item = new StringChar(c);
					if (_currentWidth + item.GetSize(_font).Width > _lineSize.Width)
					{
						flag = true;
					}
				}
				if (flag)
				{
					if (!item.IsSpecial())
					{
						_stringChars.Add(new StringChar('\n', 0));
					}
					else
					{
						_stringChars.Add(item);
					}
					FinishString(align);
					NewLine();
				}
				if (!flag || !item.IsSpecial())
				{
					_stringChars.Add(item);
				}
				_currentWidth += item.GetSize(_font).Width;
			}
		}

		protected void FinishString(ContentAlignment align)
		{
			if (_currentWidth == 0)
			{
				return;
			}
			if (_stringChars[startPos].Special != 'a')
			{
				switch (align)
				{
				case ContentAlignment.TopCenter:
					_stringChars.Insert(startPos, new StringChar('a', (_lineSize.Width - _currentWidth) / 2));
					break;
				case ContentAlignment.TopRight:
					_stringChars.Insert(startPos, new StringChar('a', _lineSize.Width - _currentWidth));
					break;
				}
			}
			if (_width < _currentWidth)
			{
				_width = _currentWidth;
			}
			_totalWidth += _currentWidth;
		}

		protected void NewLine()
		{
			startPos = _stringChars.Count;
			_currentWidth = 0;
			_currentLine++;
		}

		protected virtual string ConvertXml(XmlNode node, NodeInfo info, ContentAlignment align)
		{
			if (node.Name == "s")
			{
				return ConvertXmlS(node, info, align);
			}
			if (node.Name == "br")
			{
				return ConvertXmlBr(node, info, align);
			}
			if (node.Name == "l")
			{
				return ConvertXmlL(node, info, align);
			}
			if (node.Name == "t")
			{
				return ConvertXmlT(node, info, align);
			}
			if (node.Name == "c")
			{
				return ConvertXmlC(node, info, align);
			}
			if (node.Name == "e")
			{
				return ConvertXmlE(node, info, align);
			}
			if (node.Name == "color")
			{
				return ConvertXmlColor(node, info, align);
			}
			if (node.Name == "comment")
			{
				return ConvertXmlComment(node, info, align);
			}
			if (node.Name == "x")
			{
				return ConvertXmlX(node, info, align);
			}
			return string.Empty;
		}

		protected virtual string ConvertXmlS(XmlNode node, NodeInfo info, ContentAlignment align)
		{
			if (!node.HasChildNodes || node.ChildNodes.Count != 1 || node.ChildNodes[0].NodeType != XmlNodeType.Text)
			{
				throw new ScriptStructureException(node, info, "S should have one text child node");
			}
			if (node != node.ParentNode.FirstChild)
			{
				throw new ScriptStructureException(node, info, "S should be the first child node");
			}
			_speaker = node.FirstChild.Value;
			return string.Empty;
		}

		protected virtual string ConvertXmlBr(XmlNode node, NodeInfo info, ContentAlignment align)
		{
			FinishString(align);
			int argumentInteger = ScriptHelper.GetArgumentInteger(node, info, "value", 1);
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < argumentInteger; i++)
			{
				stringBuilder.Append("\n");
				_stringChars.Add(new StringChar('\n', 0));
				NewLine();
			}
			return stringBuilder.ToString();
		}

		protected virtual string ConvertXmlL(XmlNode node, NodeInfo info, ContentAlignment align)
		{
			int argumentInteger = ScriptHelper.GetArgumentInteger(node, info, "value", 1);
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < argumentInteger; i++)
			{
				stringBuilder.Append(" ");
				_stringChars.Add(new StringChar('\b', 0));
				_currentWidth += new StringChar(' ').GetSize(_font).Width;
			}
			return stringBuilder.ToString();
		}

		protected virtual string ConvertXmlT(XmlNode node, NodeInfo info, ContentAlignment align)
		{
			int argumentInteger = ScriptHelper.GetArgumentInteger(node, info, "value", 1);
			for (int i = 0; i < argumentInteger; i++)
			{
				_stringChars.Add(new StringChar('\t', '\t', 0));
			}
			return string.Empty;
		}

		protected virtual string ConvertXmlC(XmlNode node, NodeInfo info, ContentAlignment align)
		{
			StringChars.Add(new StringChar('\r', 0));
			return string.Empty;
		}

		protected virtual string ConvertXmlE(XmlNode node, NodeInfo info, ContentAlignment align)
		{
			if (!node.HasChildNodes || node.ChildNodes.Count != 1 || node.FirstChild.NodeType != XmlNodeType.Text)
			{
				throw new ScriptStructureException(node, info, "E should have only one text child node.");
			}
			int argumentInteger = ScriptHelper.GetArgumentInteger(node, info, "value");
			_stringChars.Add(new StringChar('e', argumentInteger));
			ConvertStringFragment(node.FirstChild.Value, align);
			_stringChars.Add(new StringChar('f', argumentInteger));
			return node.FirstChild.Value;
		}

		protected virtual string ConvertXmlColor(XmlNode node, NodeInfo info, ContentAlignment align)
		{
			if (!node.HasChildNodes || node.ChildNodes.Count != 1 || node.FirstChild.NodeType != XmlNodeType.Text)
			{
				throw new ScriptStructureException(node, info, "Color should have only one text child node.");
			}
			string argumentString = ScriptHelper.GetArgumentString(node, info, "value");
			Color color;
			try
			{
				color = Color.FromName(argumentString);
			}
			catch (Exception innerException)
			{
				throw new ScriptArgumentException(node, info, "value", "Value must be a color.", innerException);
			}
			_stringChars.Add(new StringChar('c', color.ToArgb()));
			ConvertStringFragment(node.FirstChild.Value, align);
			_stringChars.Add(new StringChar('d', color.ToArgb()));
			return node.FirstChild.Value;
		}

		protected virtual string ConvertXmlComment(XmlNode node, NodeInfo info, ContentAlignment align)
		{
			if (!node.HasChildNodes || node.ChildNodes.Count != 1 || node.FirstChild.NodeType != XmlNodeType.Text)
			{
				throw new ScriptStructureException(node, info, "Comment should have one text child node");
			}
			if (node != node.ParentNode.LastChild)
			{
				throw new ScriptStructureException(node, info, "Comment should be the last child node");
			}
			_comment = node.FirstChild.Value;
			return string.Empty;
		}

		private string ConvertXmlX(XmlNode node, NodeInfo info, ContentAlignment align)
		{
			int argumentInteger = ScriptHelper.GetArgumentInteger(node, info, "value", 1);
			_stringChars.Add(new StringChar('a', argumentInteger - _currentWidth));
			_currentWidth = argumentInteger;
			return string.Empty;
		}

		public Point DrawString(float alpha)
		{
			if (_stringChars.Count == 0)
			{
				return _click = _location;
			}
			Point location = _location;
			Point click = _location;
			Color color = _defaultColor;
			currentLinkValue = -1;
			_linkList.Clear();
			foreach (StringChar stringChar in _stringChars)
			{
				if (stringChar.IsSpecial())
				{
					if (!DrawSpecial(stringChar.Special, stringChar.SpecialArg, ref location, alpha, ref color, ref click))
					{
						break;
					}
					continue;
				}
				if (currentLinkValue == -1)
				{
					DrawChar(location, stringChar.Char, alpha, color);
				}
				else
				{
					Color color2;
					if (CheckLinkRead != null)
					{
						LinkArgs linkArgs = new LinkArgs(currentLinkValue);
						CheckLinkRead(this, linkArgs);
						color2 = ((!linkArgs.Read) ? _linkColor : _linkColorRead);
					}
					else
					{
						color2 = _linkColor;
					}
					DrawChar(location, stringChar.Char, alpha, color2);
					if (_currentPointedLinkValue == currentLinkValue)
					{
						Size size = stringChar.GetSize(_font);
						_canvas.DrawLine(location.X, location.Y + size.Height + 1, location.X + size.Width, location.Y + size.Height + 1, color2, 3);
					}
					if (alpha == 1f)
					{
						Rectangle rect = default(Rectangle);
						rect.Width = stringChar.GetSize(_font).Width;
						rect.Height = _lineSize.Height;
						rect.X = location.X;
						rect.Y = location.Y;
						_linkList.Add(new Link(currentLinkValue, rect));
					}
				}
				location.X += stringChar.GetSize(_font).Width;
			}
			if (click == _location)
			{
				click = location;
			}
			return _click = click;
		}

		public Point DrawPartString(float percent)
		{
			if (_stringChars.Count == 0 || percent <= 0f)
			{
				return _click = _location;
			}
			return DrawSubString((float)(-_alphaWidth) + percent * (float)(_totalWidth + _alphaWidth));
		}

		public Point DrawSubString(float length)
		{
			if (_stringChars.Count == 0)
			{
				return _click = _location;
			}
			Point location = _location;
			Point click = _location;
			Color color = _defaultColor;
			currentLinkValue = -1;
			_linkList.Clear();
			int num = 0;
			foreach (StringChar stringChar in _stringChars)
			{
				float num2;
				if ((float)num <= length)
				{
					num2 = 1f;
				}
				else
				{
					if ((float)num >= length + (float)_alphaWidth)
					{
						break;
					}
					num2 = (length + (float)_alphaWidth - (float)num) / (float)_alphaWidth;
				}
				if (stringChar.IsSpecial())
				{
					if (!DrawSpecial(stringChar.Special, stringChar.SpecialArg, ref location, num2, ref color, ref click))
					{
						break;
					}
					continue;
				}
				if (currentLinkValue == -1)
				{
					DrawChar(location, stringChar.Char, num2, color);
				}
				else
				{
					if (CheckLinkRead != null)
					{
						LinkArgs linkArgs = new LinkArgs(currentLinkValue);
						CheckLinkRead(this, linkArgs);
						if (linkArgs.Read)
						{
							DrawChar(location, stringChar.Char, num2, _linkColorRead);
						}
						else
						{
							DrawChar(location, stringChar.Char, num2, _linkColor);
						}
					}
					else
					{
						DrawChar(location, stringChar.Char, num2, _linkColor);
					}
					if (num2 == 1f)
					{
						Rectangle rect = default(Rectangle);
						rect.Width = stringChar.GetSize(_font).Width;
						rect.Height = _lineSize.Height;
						rect.X = location.X;
						rect.Y = location.Y;
						_linkList.Add(new Link(currentLinkValue, rect));
					}
				}
				int width = stringChar.GetSize(_font).Width;
				location.X += width;
				num += width;
			}
			if (click == _location)
			{
				click = location;
			}
			return _click = click;
		}

		protected virtual void DrawChar(Point location, char c, float alpha, Color color)
		{
			if (alpha != 0f)
			{
				if (DrawCharCallback != null)
				{
					DrawCharCallback(location, c, alpha, color, _canvas, _font);
				}
				else if (alpha == 1f)
				{
					_canvas.DrawString(_font, c.ToString(), location.X, location.Y, color);
				}
				else
				{
					_canvas.DrawString(_font, c.ToString(), location.X, location.Y, Color.FromArgb((int)(alpha * (float)(int)color.A + 0.5f), color));
				}
			}
		}

		protected virtual bool DrawSpecial(char c, int arg, ref Point location, double alpha, ref Color color, ref Point click)
		{
			switch (c)
			{
			case '\n':
				return DrawN(c, arg, ref location, alpha, ref color, ref click);
			case '\b':
				return DrawB(c, arg, ref location, alpha, ref color, ref click);
			case 'a':
				return DrawA(c, arg, ref location, alpha, ref color, ref click);
			case '\t':
				return DrawT(c, arg, ref location, alpha, ref color, ref click);
			case '\r':
				return DrawR(c, arg, ref location, alpha, ref color, ref click);
			case 'c':
				return DrawCC(c, arg, ref location, alpha, ref color, ref click);
			case 'd':
				return DrawCD(c, arg, ref location, alpha, ref color, ref click);
			case 'e':
				return DrawCE(c, arg, ref location, alpha, ref color, ref click);
			case 'f':
				return DrawCF(c, arg, ref location, alpha, ref color, ref click);
			default:
				return true;
			}
		}

		protected virtual bool DrawN(char c, int arg, ref Point location, double alpha, ref Color color, ref Point click)
		{
			location.X = _location.X;
			location.Y += _lineSize.Height;
			if (_lineNum > 0 && location.Y == _location.Y + _lineSize.Height * _lineNum)
			{
				return false;
			}
			return true;
		}

		protected virtual bool DrawB(char c, int arg, ref Point location, double alpha, ref Color color, ref Point click)
		{
			location.X += new StringChar(' ').GetSize(_font).Width;
			return true;
		}

		protected virtual bool DrawA(char c, int arg, ref Point location, double alpha, ref Color color, ref Point click)
		{
			location.X += arg;
			return true;
		}

		protected virtual bool DrawT(char c, int arg, ref Point location, double alpha, ref Color color, ref Point click)
		{
			return true;
		}

		protected virtual bool DrawR(char c, int arg, ref Point location, double alpha, ref Color color, ref Point click)
		{
			click = location;
			return true;
		}

		protected virtual bool DrawCC(char c, int arg, ref Point location, double alpha, ref Color color, ref Point click)
		{
			color = Color.FromArgb(arg);
			return true;
		}

		protected virtual bool DrawCD(char c, int arg, ref Point location, double alpha, ref Color color, ref Point click)
		{
			color = _defaultColor;
			return true;
		}

		protected virtual bool DrawCE(char c, int arg, ref Point location, double alpha, ref Color color, ref Point click)
		{
			currentLinkValue = arg;
			return true;
		}

		protected virtual bool DrawCF(char c, int arg, ref Point location, double alpha, ref Color color, ref Point click)
		{
			currentLinkValue = -1;
			return true;
		}

		public bool OnItemClick()
		{
			if (_currentPointedLinkValue >= 0)
			{
				if (ClickLink != null)
				{
					ClickLink(this, new LinkArgs(_currentPointedLinkValue));
				}
				if (Application.OpenForms[0].Cursor == Cursors.Default || Application.OpenForms[0].Cursor == Cursors.Hand)
				{
					Application.OpenForms[0].Cursor = Cursors.Default;
				}
				return true;
			}
			return false;
		}

		public void OnItemMove(Point location)
		{
			foreach (Link link in _linkList)
			{
				int x = location.X;
				Rectangle rect = link.Rect;
				if (x < rect.Left)
				{
					continue;
				}
				int x2 = location.X;
				Rectangle rect2 = link.Rect;
				if (x2 > rect2.Right)
				{
					continue;
				}
				int y = location.Y;
				Rectangle rect3 = link.Rect;
				if (y < rect3.Top)
				{
					continue;
				}
				int y2 = location.Y;
				Rectangle rect4 = link.Rect;
				if (y2 <= rect4.Bottom)
				{
					_currentPointedLinkValue = link.Value;
					if (MoveLink != null)
					{
						MoveLink(this, new LinkArgs(_currentPointedLinkValue));
					}
					if (Application.OpenForms[0].Cursor == Cursors.Default || Application.OpenForms[0].Cursor == Cursors.Hand)
					{
						Application.OpenForms[0].Cursor = Cursors.Hand;
					}
					return;
				}
			}
			_currentPointedLinkValue = -1;
			if (MoveLink != null)
			{
				MoveLink(this, new LinkArgs(_currentPointedLinkValue));
			}
			if (Application.OpenForms[0].Cursor == Cursors.Default || Application.OpenForms[0].Cursor == Cursors.Hand)
			{
				Application.OpenForms[0].Cursor = Cursors.Default;
			}
		}

		public void CutLine(int from, int to)
		{
			int num = 0;
			while (num < from && _stringChars.Count > 0)
			{
				bool flag = _stringChars[0].Special == '\n';
				_stringChars.RemoveAt(0);
				if (flag)
				{
					num++;
				}
			}
			int num2 = 0;
			while (num <= to && num2 < _stringChars.Count)
			{
				if (_stringChars[num2].Special == '\n')
				{
					num++;
				}
				num2++;
			}
			_stringChars.RemoveRange(num2, _stringChars.Count - num2);
			startPos = _stringChars.Count;
		}

		public static int CalLine(string value, Font2D font, int width)
		{
			String2D string2D = new String2D();
			string2D.Font = font;
			string2D.LineSize = new Size(width, 1);
			string2D.AddString(value);
			return string2D.TotalLine;
		}
	}
}
