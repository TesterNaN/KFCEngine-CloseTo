using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace KidFansChannel.DataCore
{
	public class DataDecoder : IDisposable
	{
		private string _packageName;

		private int _version;

		private int _fileNumber;

		private List<string> fileNames = new List<string>();

		private List<long> fileOffsets = new List<long>();

		private List<long> fileLengths = new List<long>();

		private List<bool> fileCryptos = new List<bool>();

		private string _currentDirectory;

		private static readonly string key = "www.KIDS FansChannel.net";

		private static readonly string iv = "ctMullen";

		private Stream fs;

		private ICryptoTransform ct;

		public int Version
		{
			get
			{
				return _version;
			}
		}

		public string PackageName
		{
			get
			{
				return _packageName;
			}
		}

		public int FileNumber
		{
			get
			{
				return _fileNumber;
			}
		}

		public string CurrentDirectory
		{
			get
			{
				return _currentDirectory;
			}
			set
			{
				_currentDirectory = value;
			}
		}

		public DataDecoder(string packageName)
		{
			_packageName = packageName;
			_currentDirectory = Path.GetDirectoryName(".");
			byte[] keyBytes = new byte[24];
			byte[] ivBytes = new byte[8];
			for (int i = 0; i < 24; i++)
			{
				keyBytes[i] = (byte)key[i];
			}
			for (int j = 0; j < 8; j++)
			{
				ivBytes[j] = (byte)iv[j];
			}
			TripleDESCryptoServiceProvider tripleDESCryptoServiceProvider = new TripleDESCryptoServiceProvider();
			ct = tripleDESCryptoServiceProvider.CreateDecryptor(keyBytes, ivBytes);
			fs = new FileStream(_packageName, FileMode.Open, FileAccess.Read);
			DecodeHeader();
		}

		public DataDecoder(string packageName, Stream packageStream)
		{
			_packageName = packageName;
			_currentDirectory = Path.GetDirectoryName(".");
			byte[] keyBytes = new byte[24];
			byte[] ivBytes = new byte[8];
			for (int i = 0; i < 24; i++)
			{
				keyBytes[i] = (byte)key[i];
			}
			for (int j = 0; j < 8; j++)
			{
				ivBytes[j] = (byte)iv[j];
			}
			TripleDESCryptoServiceProvider tripleDESCryptoServiceProvider = new TripleDESCryptoServiceProvider();
			ct = tripleDESCryptoServiceProvider.CreateDecryptor(keyBytes, ivBytes);
			fs = packageStream;
			DecodeHeader();
		}

		~DataDecoder()
		{
			Dispose();
		}

		protected void DecodeHeader()
		{
			fs.Seek(0L, SeekOrigin.Begin);
			if (fs.ReadByte() == 75 && fs.ReadByte() == 70 && fs.ReadByte() == 67 && fs.ReadByte() == 68)
			{
				_version = 0;
				for (int i = 0; i < 4; i++)
				{
					_version += fs.ReadByte() << 8 * i;
				}
				_fileNumber = 0;
				for (int j = 0; j < 4; j++)
				{
					_fileNumber += fs.ReadByte() << 8 * j;
				}
				fs.Seek(16L, SeekOrigin.Begin);
				if (_fileNumber <= 0)
				{
					return;
				}
				using (DataStream stream = new DataStream(fs, 16L, 16 + 64 * _fileNumber))
				{
					using (CryptoStream input = new CryptoStream(stream, ct, CryptoStreamMode.Read))
					{
						using (BinaryReader binaryReader = new BinaryReader(input))
						{
							for (int k = 0; k < _fileNumber; k++)
							{
								fileOffsets.Add(binaryReader.ReadInt64());
								long offset = binaryReader.ReadInt64();
								if (offset >= 0)
								{
									fileLengths.Add(offset);
									fileCryptos.Add(false);
								}
								else
								{
									fileLengths.Add(-offset);
									fileCryptos.Add(true);
								}
								StringBuilder stringBuilder = new StringBuilder();
								bool entryFound = false;
								for (int l = 0; l < 48; l++)
								{
									byte b = binaryReader.ReadByte();
									if (!entryFound)
									{
										if (b == 0)
										{
											entryFound = true;
										}
										else
										{
											stringBuilder.Append((char)b);
										}
									}
								}
								fileNames.Add(stringBuilder.ToString());
							}
							return;
						}
					}
				}
			}
			throw new InvalidDataException();
		}

		public string[] GetFileList()
		{
			return fileNames.ToArray();
		}

		public Stream DecodeStream(string fileName)
		{
			int offset = fileNames.IndexOf(fileName);
			if (offset < 0)
			{
				return null;
			}
			if (fileCryptos[offset])
			{
				using (DataStream dataStream = new DataStream(fs, fileOffsets[offset], (fileLengths[offset] + 8) & 0xFFFFFFF8u))
				{
					dataStream.Seek(0L, SeekOrigin.Begin);
					using (CryptoStream cryptoStream = new CryptoStream(dataStream, ct, CryptoStreamMode.Read))
					{
						byte[] keyBytes = new byte[fileLengths[offset]];
						cryptoStream.Read(keyBytes, 0, keyBytes.Length);
						MemoryStream memoryStream = new MemoryStream(keyBytes, 0, (int)fileLengths[offset]);
						memoryStream.Seek(0L, SeekOrigin.Begin);
						return memoryStream;
					}
				}
			}
			DataStream dataStream2 = new DataStream(fs, fileOffsets[offset], fileLengths[offset]);
			dataStream2.Seek(0L, SeekOrigin.Begin);
			return dataStream2;
		}

		public void DecodeFile(string fileName)
		{
			int offset = fileNames.IndexOf(fileName);
			if (offset < 0)
			{
				return;
			}
			byte[] keyBytes = new byte[fileLengths[offset]];
			if (fileCryptos[offset])
			{
				using (DataStream dataStream = new DataStream(fs, fileOffsets[offset], (fileLengths[offset] + 8) & 0xFFFFFFF8u))
				{
					dataStream.Seek(0L, SeekOrigin.Begin);
					using (CryptoStream cryptoStream = new CryptoStream(dataStream, ct, CryptoStreamMode.Read))
					{
						cryptoStream.Read(keyBytes, 0, keyBytes.Length);
					}
				}
			}
			else
			{
				using (DataStream dataStream2 = new DataStream(fs, fileOffsets[offset], fileLengths[offset]))
				{
					dataStream2.Seek(0L, SeekOrigin.Begin);
					dataStream2.Read(keyBytes, 0, keyBytes.Length);
				}
			}
			string path = Path.Combine(_currentDirectory, fileName);
			if (!Directory.Exists(Path.GetDirectoryName(path)))
			{
				Directory.CreateDirectory(Path.GetDirectoryName(path));
			}
			File.WriteAllBytes(path, keyBytes);
		}

		public void Decode()
		{
			foreach (string fileName in fileNames)
			{
				DecodeFile(fileName);
			}
		}

		public void Dispose()
		{
			fs.Close();
			fileCryptos.Clear();
			fileLengths.Clear();
			fileNames.Clear();
			fileOffsets.Clear();
		}
	}
}
