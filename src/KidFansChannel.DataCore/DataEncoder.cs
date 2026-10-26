using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;

namespace KidFansChannel.DataCore
{
	public class DataEncoder
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

		public int Version
		{
			get
			{
				return _version;
			}
			set
			{
				_version = value;
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

		public DataEncoder(string packageName)
		{
			_packageName = packageName;
			_currentDirectory = Path.GetDirectoryName(".");
		}

		~DataEncoder()
		{
			fileCryptos.Clear();
			fileLengths.Clear();
			fileNames.Clear();
			fileOffsets.Clear();
		}

		public void AddFile(string fileName, bool crypto)
		{
			if (fileName.Length >= 48)
			{
				throw new ArgumentException("fileName");
			}
			if (!File.Exists(Path.Combine(_currentDirectory, fileName)))
			{
				throw new ArgumentException("fileName");
			}
			fileNames.Add(fileName);
			fileCryptos.Add(crypto);
			_fileNumber++;
		}

		public void Encode()
		{
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
			using (FileStream fileStream = new FileStream(_packageName, FileMode.Create))
			{
				long offset = 32 + 64 * _fileNumber;
				fileStream.SetLength(offset);
				fileStream.Seek(offset, SeekOrigin.Begin);
				for (int k = 0; k < _fileNumber; k++)
				{
					byte[] fileBytes = File.ReadAllBytes(Path.Combine(_currentDirectory, fileNames[k]));
					fileOffsets.Add(offset);
					fileLengths.Add(fileBytes.Length);
					int encryptedLength = (fileBytes.Length + 24) & -16;
					using (DataStream dataStream = new DataStream(fileStream, offset, encryptedLength))
					{
						if (fileCryptos[k])
						{
							using (CryptoStream cryptoStream = new CryptoStream(dataStream, tripleDESCryptoServiceProvider.CreateEncryptor(keyBytes, ivBytes), CryptoStreamMode.Write))
							{
								cryptoStream.Write(fileBytes, 0, fileBytes.Length);
								cryptoStream.Flush();
								cryptoStream.FlushFinalBlock();
							}
						}
						else
						{
							dataStream.Write(fileBytes, 0, fileBytes.Length);
							dataStream.Flush();
						}
						int entryIndex = (int)(encryptedLength - dataStream.Position);
						for (int l = 0; l < entryIndex; l++)
						{
							dataStream.WriteByte(0);
						}
						dataStream.Flush();
					}
					offset += encryptedLength;
				}
				fileStream.Seek(0L, SeekOrigin.Begin);
				fileStream.WriteByte(75);
				fileStream.WriteByte(70);
				fileStream.WriteByte(67);
				fileStream.WriteByte(68);
				for (int m = 0; m < 4; m++)
				{
					fileStream.WriteByte((byte)((_version >> 8 * m) & 0xFF));
				}
				for (int n = 0; n < 4; n++)
				{
					fileStream.WriteByte((byte)((_fileNumber >> 8 * n) & 0xFF));
				}
				for (int reservedField = 0; reservedField < 4; reservedField++)
				{
					fileStream.WriteByte(0);
				}
				using (DataStream stream = new DataStream(fileStream, 16L, 16 + 64 * _fileNumber))
				{
					using (CryptoStream output = new CryptoStream(stream, tripleDESCryptoServiceProvider.CreateEncryptor(keyBytes, ivBytes), CryptoStreamMode.Write))
					{
						using (BinaryWriter binaryWriter = new BinaryWriter(output))
						{
							for (int entryIndex = 0; entryIndex < _fileNumber; entryIndex++)
							{
								binaryWriter.Write(fileOffsets[entryIndex]);
								if (fileCryptos[entryIndex])
								{
									binaryWriter.Write(-fileLengths[entryIndex]);
								}
								else
								{
									binaryWriter.Write(fileLengths[entryIndex]);
								}
								for (int nameByte = 0; nameByte < 48; nameByte++)
								{
									if (nameByte < fileNames[entryIndex].Length)
									{
										binaryWriter.Write((byte)fileNames[entryIndex][nameByte]);
									}
									else
									{
										binaryWriter.Write((byte)0);
									}
								}
							}
							binaryWriter.Flush();
						}
					}
				}
			}
		}
	}
}
