using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using KidFansChannel.Game2DCore;

namespace KidFansChannel.GalCore.Screens
{
	public class Element : ICloneable, IEnumerable<Element>, IEnumerable, IDisposable
	{
		public struct Enumerator : IEnumerator<Element>, IDisposable, IEnumerator
		{
			public Element _current;

			public Element children;

			public int index;

			public Element Current
			{
				get
				{
					return _current;
				}
			}

			object IEnumerator.Current
			{
				get
				{
					if (index == 0 || index == children.Count + 1)
					{
						throw new Exception();
					}
					return Current;
				}
			}

			internal Enumerator(Element element)
			{
				children = element;
				index = 0;
				_current = null;
			}

			public void Dispose()
			{
			}

			public bool MoveNext()
			{
				if (index < children.Count)
				{
					_current = children[index];
					index++;
					return true;
				}
				index = children.Count + 1;
				_current = null;
				return false;
			}

			public void Reset()
			{
				index = 0;
				_current = null;
			}
		}

		private List<Element> childs;

		private List<string> names;

		private List<bool> visibles;

		private Picture _elementPicture;

		private String2D _elementString;

		private Element _parent;

		private Point _position;

		private bool _focused;

		private bool _selected;

		private Element _left;

		private Element _right;

		private Element _up;

		private Element _down;

		private Element _childLeft;

		private Element _childRight;

		private Element _childUp;

		private Element _childDown;

		private Rectangle _region;

		private object _value;

		private int selectedChildIndex = -1;

		private bool _canDrag;

		private bool _draging;

		private bool indrag;

		private bool _disposed;

		public Picture ElementPicture
		{
			get
			{
				return _elementPicture;
			}
			set
			{
				_elementPicture = value;
			}
		}

		public String2D ElementString
		{
			get
			{
				return _elementString;
			}
			set
			{
				_elementString = value;
			}
		}

		public Element Parent
		{
			get
			{
				return _parent;
			}
		}

		public Point Position
		{
			get
			{
				return _position;
			}
			set
			{
				_position = value;
			}
		}

		public bool Focused
		{
			get
			{
				return _focused;
			}
			set
			{
				if (_focused && !value && selectedChildIndex != -1)
				{
					childs[selectedChildIndex].Selected = false;
					selectedChildIndex = -1;
				}
				_focused = value;
			}
		}

		public bool Selected
		{
			get
			{
				return _selected;
			}
			set
			{
				if (!value)
				{
					if (_parent != null && _parent._focused && _parent.selectedChildIndex != -1 && _parent.childs[_parent.selectedChildIndex] == this)
					{
						_parent.selectedChildIndex = -1;
					}
					if (MouseLeave != null)
					{
						MouseLeave(this, EventArgs.Empty);
					}
				}
				else
				{
					if (_parent == null || !_parent._focused)
					{
						return;
					}
					if (_parent.selectedChildIndex != -1)
					{
						Element element = _parent.childs[_parent.selectedChildIndex];
						element._selected = false;
						if (element.MouseLeave != null)
						{
							element.MouseLeave(element, EventArgs.Empty);
						}
					}
					_parent.selectedChildIndex = _parent.childs.IndexOf(this);
					if (MouseEnter != null)
					{
						MouseEnter(this, EventArgs.Empty);
					}
				}
				_selected = value;
			}
		}

		public Element Left
		{
			get
			{
				return _left;
			}
			set
			{
				_left = value;
			}
		}

		public Element Right
		{
			get
			{
				return _right;
			}
			set
			{
				_right = value;
			}
		}

		public Element Up
		{
			get
			{
				return _up;
			}
			set
			{
				_up = value;
			}
		}

		public Element Down
		{
			get
			{
				return _down;
			}
			set
			{
				_down = value;
			}
		}

		public Element ChildLeft
		{
			get
			{
				return _childLeft;
			}
			set
			{
				if (!childs.Contains(value))
				{
					throw new ArgumentException("Must be child");
				}
				_childLeft = value;
			}
		}

		public Element ChildRight
		{
			get
			{
				return _childRight;
			}
			set
			{
				if (!childs.Contains(value))
				{
					throw new ArgumentException("Must be child");
				}
				_childRight = value;
			}
		}

		public Element ChildUp
		{
			get
			{
				return _childUp;
			}
			set
			{
				if (!childs.Contains(value))
				{
					throw new ArgumentException("Must be child");
				}
				_childUp = value;
			}
		}

		public Element ChildDown
		{
			get
			{
				return _childDown;
			}
			set
			{
				if (!childs.Contains(value))
				{
					throw new ArgumentException("Must be child");
				}
				_childDown = value;
			}
		}

		public Rectangle Region
		{
			get
			{
				return _region;
			}
			set
			{
				_region = value;
			}
		}

		public object Value
		{
			get
			{
				return _value;
			}
			set
			{
				_value = value;
			}
		}

		public Element SelectedChild
		{
			get
			{
				if (selectedChildIndex == -1)
				{
					return null;
				}
				return childs[selectedChildIndex];
			}
		}

		public Element this[int index]
		{
			get
			{
				return childs[index];
			}
		}

		public Element this[string name]
		{
			get
			{
				int index = names.IndexOf(name);
				if (index == -1)
				{
					throw new ArgumentException();
				}
				return childs[index];
			}
		}

		public int Count
		{
			get
			{
				return childs.Count;
			}
		}

		public bool CanDrag
		{
			get
			{
				return _canDrag;
			}
			set
			{
				_canDrag = value;
			}
		}

		public bool Draging
		{
			get
			{
				return _draging;
			}
		}

		public bool Disposed
		{
			get
			{
				return _disposed;
			}
		}

		public event MouseEventHandler DragStart;

		public event MouseEventHandler DragEnd;

		public event MouseEventHandler DragMove;

		public event EventHandler MouseClick;

		public event EventHandler MouseEnter;

		public event EventHandler MouseLeave;

		public event EventHandler<KeyEventArgs> KeyPress;

		public void SetVisible(int index, bool value)
		{
			if (index >= childs.Count)
			{
				throw new ArgumentOutOfRangeException();
			}
			visibles[index] = value;
		}

		public void SetVisible(string name, bool value)
		{
			int index = names.IndexOf(name);
			if (index == -1)
			{
				throw new ArgumentException();
			}
			SetVisible(index, value);
		}

		public bool GetVisible(int index)
		{
			if (index >= childs.Count)
			{
				throw new ArgumentOutOfRangeException();
			}
			return visibles[index];
		}

		public bool GetVisible(string name)
		{
			int index = names.IndexOf(name);
			if (index == -1)
			{
				throw new ArgumentException();
			}
			return GetVisible(index);
		}

		public void ClearSelected()
		{
			if (selectedChildIndex >= 0)
			{
				childs[selectedChildIndex].Selected = false;
			}
			selectedChildIndex = -1;
		}

		public Element()
		{
			_elementPicture = null;
			_elementString = null;
			childs = new List<Element>();
			names = new List<string>();
			visibles = new List<bool>();
			_parent = null;
			_position = Point.Empty;
		}

		~Element()
		{
			Dispose();
		}

		protected void ReConstruct()
		{
			if (childs == null || names == null || visibles == null)
			{
				childs = new List<Element>();
				names = new List<string>();
				visibles = new List<bool>();
			}
		}

		public Element(Picture elementPicture)
			: this()
		{
			_elementPicture = elementPicture;
		}

		public Element(String2D elementString)
			: this()
		{
			_elementString = elementString;
		}

		public void Add(Element item)
		{
			Add(item, string.Empty, true);
		}

		public void Add(Element item, string name)
		{
			Add(item, name, true);
		}

		public void Add(Element item, string name, bool visible)
		{
			if (names.IndexOf(name) != -1)
			{
				throw new ArgumentException();
			}
			childs.Add(item);
			item._parent = this;
			names.Add(name);
			visibles.Add(visible);
		}

		public void Remove(Element item)
		{
			int index = childs.IndexOf(item);
			if (index == -1)
			{
				throw new ArgumentException();
			}
			Remove(index);
		}

		public void Remove(string name)
		{
			int index = names.IndexOf(name);
			if (index == -1)
			{
				throw new ArgumentException();
			}
			Remove(index);
		}

		public void Remove(int index)
		{
			childs[index]._parent = null;
			childs.RemoveAt(index);
			names.RemoveAt(index);
			visibles.RemoveAt(index);
		}

		public int IndexOf(Element item)
		{
			return childs.IndexOf(item);
		}

		public int IndexOf(string name)
		{
			return names.IndexOf(name);
		}

		public string NameOf(Element item)
		{
			int index = childs.IndexOf(item);
			if (index == -1)
			{
				return null;
			}
			return names[index];
		}

		public string NameOf(int index)
		{
			if (index == -1 || index >= childs.Count)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			return names[index];
		}

		public void Draw()
		{
			Draw(Point.Empty);
		}

		public void Draw(Point position)
		{
			position.X += _position.X;
			position.Y += _position.Y;
			if (_elementPicture != null)
			{
				_elementPicture.X += position.X;
				_elementPicture.Y += position.Y;
				GameManager.GalCore.CurrentCanvas.DrawPicture(_elementPicture);
				_elementPicture.X -= position.X;
				_elementPicture.Y -= position.Y;
			}
			if (_elementString != null)
			{
				Point location = _elementString.Location;
				location.X += position.X;
				location.Y += position.Y;
				_elementString.Canvas = GameManager.GalCore.CurrentCanvas;
				_elementString.Location = location;
				_elementString.DrawString();
				location.X -= position.X;
				location.Y -= position.Y;
				_elementString.Location = location;
			}
			for (int i = 0; i < childs.Count; i++)
			{
				if (GetVisible(i))
				{
					childs[i].Draw(position);
				}
			}
		}

		public void Update(float elaspedTime)
		{
			if (_elementPicture != null)
			{
				_elementPicture.Update(elaspedTime);
			}
			for (int i = 0; i < childs.Count; i++)
			{
				if (GetVisible(i))
				{
					childs[i].Update(elaspedTime);
				}
			}
		}

		public void Drag()
		{
			if (_canDrag && indrag)
			{
				_draging = true;
			}
		}

		protected internal virtual bool OnMouseDown(Point location)
		{
			if (_selected)
			{
				if (MouseClick != null)
				{
					MouseClick(this, EventArgs.Empty);
				}
				if (_canDrag && DragStart != null && location != Point.Empty)
				{
					indrag = true;
					if (DragStart != null)
					{
						DragStart(this, new MouseEventArgs(MouseButtons.Left, 0, location.X - _position.X, location.Y - _position.Y, 0));
					}
					indrag = false;
				}
			}
			return _selected;
		}

		protected internal virtual bool OnMouseUp(Point location)
		{
			if (_draging)
			{
				if (DragEnd != null)
				{
					DragEnd(this, new MouseEventArgs(MouseButtons.Left, 0, location.X - _position.X, location.Y - _position.Y, 0));
				}
				_draging = false;
				return true;
			}
			return false;
		}

		protected internal virtual bool OnMouseMove(Point location)
		{
			if (_draging)
			{
				if (DragMove != null)
				{
					DragMove(this, new MouseEventArgs(MouseButtons.Left, 0, location.X - _position.X, location.Y - _position.Y, 0));
				}
				return true;
			}
			if (_parent == null || !_parent._focused)
			{
				for (int i = 0; i < childs.Count; i++)
				{
					if (visibles[i] && childs[i].OnMouseMove(location))
					{
						return true;
					}
				}
				if (_focused && selectedChildIndex != -1)
				{
					childs[selectedChildIndex].Selected = false;
				}
				return false;
			}
			if (_region.Width == 0 || _region.Height == 0)
			{
				return false;
			}
			Point position = _position;
			for (Element parent = _parent; parent != null; parent = parent._parent)
			{
				position.X += parent._position.X;
				position.Y += parent._position.Y;
			}
			if (location.X >= position.X + _region.Left && location.X < position.X + _region.Right && location.Y >= position.Y + _region.Top && location.Y < position.Y + _region.Bottom)
			{
				if (!_selected)
				{
					OnMouseEnter();
				}
				return true;
			}
			return false;
		}

		protected internal virtual void OnMouseEnter()
		{
			Selected = true;
		}

		protected internal virtual void OnMouseLeave()
		{
			Selected = false;
		}

		internal void OnKeyPress(Keys keyData)
		{
			if (KeyPress != null)
			{
				KeyPress(this, new KeyEventArgs(keyData));
			}
			if (_focused && selectedChildIndex == -1)
			{
				switch (keyData)
				{
				case Keys.Left:
					if (_childLeft != null)
					{
						_childLeft.OnMouseEnter();
					}
					break;
				case Keys.Right:
					if (_childRight != null)
					{
						_childRight.OnMouseEnter();
					}
					break;
				case Keys.Up:
					if (_childUp != null)
					{
						_childUp.OnMouseEnter();
					}
					break;
				case Keys.Down:
					if (_childDown != null)
					{
						_childDown.OnMouseEnter();
					}
					break;
				}
			}
			else
			{
				if (!_selected)
				{
					return;
				}
				switch (keyData)
				{
				case Keys.Return:
					OnMouseDown(Point.Empty);
					break;
				case Keys.Left:
					if (_left != null)
					{
						_left.OnMouseEnter();
					}
					break;
				case Keys.Right:
					if (_right != null)
					{
						_right.OnMouseEnter();
					}
					break;
				case Keys.Up:
					if (_up != null)
					{
						_up.OnMouseEnter();
					}
					break;
				case Keys.Down:
					if (_down != null)
					{
						_down.OnMouseEnter();
					}
					break;
				}
			}
		}

		public object Clone()
		{
			Element element = new Element();
			element._elementPicture = _elementPicture;
			element._elementString = _elementString;
			element._position = _position;
			element._region = _region;
			element._value = _value;
			element.visibles.AddRange(visibles);
			element.childs.AddRange(childs);
			element.names.AddRange(names);
			return element;
		}

		public IEnumerator<Element> GetEnumerator()
		{
			return new Enumerator(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return new Enumerator(this);
		}

		public void Dispose()
		{
			_elementPicture = null;
			_elementString = null;
			if (childs != null)
			{
				childs.Clear();
				childs = null;
			}
			if (names != null)
			{
				names.Clear();
				names = null;
			}
			if (visibles != null)
			{
				visibles.Clear();
				visibles = null;
			}
			_parent = null;
			selectedChildIndex = -1;
			_disposed = true;
		}
	}
}
