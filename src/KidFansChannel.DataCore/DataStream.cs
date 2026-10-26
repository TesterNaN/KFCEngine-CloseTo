using System;
using System.IO;

namespace KidFansChannel.DataCore
{
	public class DataStream : Stream
	{
		private Stream _baseStream;

		private long _offset;

		private long _length;

		public Stream BaseStream
		{
			get
			{
				return _baseStream;
			}
		}

		public long Offset
		{
			get
			{
				return _offset;
			}
		}

		public override bool CanRead
		{
			get
			{
				return _baseStream.CanRead;
			}
		}

		public override bool CanSeek
		{
			get
			{
				return _baseStream.CanSeek;
			}
		}

		public override bool CanWrite
		{
			get
			{
				return _baseStream.CanWrite;
			}
		}

		public override long Length
		{
			get
			{
				return _length;
			}
		}

		public override long Position
		{
			get
			{
				return _baseStream.Position - _offset;
			}
			set
			{
				_baseStream.Position = value + _offset;
			}
		}

		public DataStream(Stream baseStream, long offset, long length)
		{
			_baseStream = baseStream;
			_offset = offset;
			_length = length;
		}

		public override void Flush()
		{
			_baseStream.Flush();
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			if (Position + count > _length)
			{
				count = (int)(_length - Position);
			}
			return _baseStream.Read(buffer, offset, count);
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			switch (origin)
			{
			case SeekOrigin.Begin:
				return _baseStream.Seek(offset + _offset, SeekOrigin.Begin) - _offset;
			case SeekOrigin.Current:
				return _baseStream.Seek(offset, SeekOrigin.Current) - _offset;
			case SeekOrigin.End:
				return _baseStream.Seek(offset - _baseStream.Length + _offset + _length, SeekOrigin.End) - _offset;
			default:
				return -1L;
			}
		}

		public override void SetLength(long value)
		{
			throw new NotSupportedException();
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			_baseStream.Write(buffer, offset, count);
		}
	}
}
