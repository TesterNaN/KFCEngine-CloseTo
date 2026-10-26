using System;
using System.Collections.Generic;
using System.IO;

namespace KidFansChannel.DataCore
{
	public class DataCore : IDisposable
	{
		public enum DataType
		{
			Null,
			Package,
			File,
			Any
		}

		private List<string> directoryNames = new List<string>();

		private List<string> dataNames = new List<string>();

		private List<List<bool>> dataIsPackages = new List<List<bool>>();

		private List<List<string>> dataDirectories = new List<List<string>>();

		private List<List<DataDecoder>> dataDecoders = new List<List<DataDecoder>>();

		private List<List<DataDecoder>> dataDecoderWithLanguages = new List<List<DataDecoder>>();

		private bool currentDirectorySetted;

		private bool directoryFinished;

		private string _languageExt = "";

		private string _extension = "dat";

		private List<string> replaceNames = new List<string>();

		private List<Stream> replaceStreams = new List<Stream>();

		private DataType nowData;

		private string nowFile;

		private Stream nowStream;

		public string Extension
		{
			get
			{
				return _extension;
			}
			set
			{
				_extension = value;
			}
		}

		public string LanguageExt
		{
			get
			{
				return _languageExt;
			}
			set
			{
				_languageExt = value;
			}
		}

		~DataCore()
		{
			Dispose();
		}

		public void SetCurrentDirectory(string currentDirectory)
		{
			if (currentDirectorySetted || directoryFinished)
			{
				throw new InvalidOperationException();
			}
			currentDirectorySetted = true;
			dataNames.Clear();
			directoryNames.Add(currentDirectory);
		}

		public void AddReplacePair(string name, Stream stream)
		{
			replaceNames.Add(name);
			replaceStreams.Add(stream);
		}

		public void AddDirectory(string directory)
		{
			if (!currentDirectorySetted || directoryFinished)
			{
				throw new InvalidOperationException();
			}
			if (!directoryNames.Contains(Path.GetFullPath(directory)))
			{
				directoryNames.Add(Path.GetFullPath(directory));
			}
		}

		public void AddData(string dataName)
		{
			if (!currentDirectorySetted)
			{
				throw new InvalidProgramException();
			}
			directoryFinished = true;
			if (dataNames.Contains(dataName))
			{
				return;
			}
			dataNames.Add(dataName);
			int count = dataIsPackages.Count;
			dataIsPackages.Add(new List<bool>());
			dataDecoders.Add(new List<DataDecoder>());
			dataDecoderWithLanguages.Add(new List<DataDecoder>());
			dataDirectories.Add(new List<string>());
			for (int i = 0; i < directoryNames.Count; i++)
			{
				string item;
				if (Directory.Exists(item = Path.Combine(directoryNames[i], dataName)))
				{
					dataIsPackages[count].Add(false);
					dataDirectories[count].Add(item);
				}
				List<DataDecoder> list = new List<DataDecoder>();
				List<DataDecoder> list2 = new List<DataDecoder>();
				string text;
				string text2;
				for (int j = 0; j < 100; j++)
				{
					if (!File.Exists(text = Path.Combine(directoryNames[i], string.Format("{0}{1:D2}.{2}", dataName, j, _extension))) && !replaceNames.Contains(Path.GetFileName(text)))
					{
						break;
					}
					try
					{
						DataDecoder item2 = ((!replaceNames.Contains(Path.GetFileName(text))) ? new DataDecoder(text) : new DataDecoder(text, replaceStreams[replaceNames.IndexOf(Path.GetFileName(text))]));
						dataIsPackages[count].Add(true);
						list.Add(item2);
						if (File.Exists(text2 = Path.Combine(directoryNames[i], string.Format("{0}{1:D2}.{2}.{3}", dataName, j, _languageExt, _extension))) || replaceNames.Contains(Path.GetFileName(text2)))
						{
							try
							{
								DataDecoder item3 = ((!replaceNames.Contains(Path.GetFileName(text2))) ? new DataDecoder(text2) : new DataDecoder(text2, replaceStreams[replaceNames.IndexOf(Path.GetFileName(text2))]));
								list2.Add(item3);
							}
							catch (InvalidDataException)
							{
								list2.Add(null);
							}
						}
						list2.Add(null);
					}
					catch (InvalidDataException)
					{
					}
				}
				for (int num = list.Count - 1; num >= 0; num--)
				{
					dataDecoders[count].Add(list[num]);
					dataDecoderWithLanguages[count].Add(list2[num]);
				}
				list.Clear();
				list2.Clear();
				if (!File.Exists(text = Path.Combine(directoryNames[i], string.Format("{0}.{1}", dataName, _extension))) && !replaceNames.Contains(Path.GetFileName(text)))
				{
					continue;
				}
				try
				{
					DataDecoder item4 = ((!replaceNames.Contains(Path.GetFileName(text))) ? new DataDecoder(text) : new DataDecoder(text, replaceStreams[replaceNames.IndexOf(Path.GetFileName(text))]));
					dataIsPackages[count].Add(true);
					dataDecoders[count].Add(item4);
				}
				catch (InvalidDataException)
				{
				}
				if (File.Exists(text2 = Path.Combine(directoryNames[i], string.Format("{0}.{1}.{2}", dataName, _languageExt, _extension))) || replaceNames.Contains(Path.GetFileName(text2)))
				{
					try
					{
						DataDecoder item5 = ((!replaceNames.Contains(Path.GetFileName(text2))) ? new DataDecoder(text2) : new DataDecoder(text2, replaceStreams[replaceNames.IndexOf(Path.GetFileName(text2))]));
						dataDecoderWithLanguages[count].Add(item5);
					}
					catch (InvalidDataException)
					{
						dataDecoderWithLanguages[count].Add(null);
					}
				}
				dataDecoderWithLanguages[count].Add(null);
			}
		}

		public DataType CheckData(string dataName, string pathName)
		{
			return CheckData(dataName, pathName, DataType.Any);
		}

		public DataType CheckData(string dataName, string pathName, DataType defaultType)
		{
			int num = dataNames.IndexOf(dataName);
			if (num == -1)
			{
				nowData = DataType.Null;
				nowFile = null;
				if (nowStream != null)
				{
					nowStream.Close();
				}
				nowStream = null;
				return DataType.Null;
			}
			bool flag = false;
			switch (defaultType)
			{
			case DataType.Package:
			{
				for (int j = 0; j < dataDecoders[num].Count; j++)
				{
					try
					{
						if (CheckPackage(dataDecoders[num][j], dataDecoderWithLanguages[num][j], pathName))
						{
							flag = true;
							break;
						}
					}
					catch (Exception)
					{
						if (!File.Exists(dataDecoders[num][j].PackageName) || !File.Exists(dataDecoderWithLanguages[num][j].PackageName))
						{
							RemovePackage(num, j);
						}
					}
				}
				if (!flag)
				{
					for (int k = 0; k < dataDirectories[num].Count; k++)
					{
						if (CheckFile(dataDirectories[num][k], pathName))
						{
							flag = true;
							break;
						}
					}
				}
				if (flag)
				{
					break;
				}
				goto default;
			}
			case DataType.File:
			{
				for (int l = 0; l < dataDirectories[num].Count; l++)
				{
					if (CheckFile(dataDirectories[num][l], pathName))
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					for (int m = 0; m < dataDecoders[num].Count; m++)
					{
						try
						{
							if (CheckPackage(dataDecoders[num][m], dataDecoderWithLanguages[num][m], pathName))
							{
								flag = true;
								break;
							}
						}
						catch (Exception)
						{
							if (!File.Exists(dataDecoders[num][m].PackageName) || !File.Exists(dataDecoderWithLanguages[num][m].PackageName))
							{
								RemovePackage(num, m);
							}
						}
					}
				}
				if (flag)
				{
					break;
				}
				goto default;
			}
			case DataType.Any:
			{
				int num2 = 0;
				int num3 = 0;
				for (int i = 0; i < dataIsPackages[num].Count; i++)
				{
					if (dataIsPackages[num][i])
					{
						try
						{
							if (CheckPackage(dataDecoders[num][num2], dataDecoderWithLanguages[num][num2], pathName))
							{
								flag = true;
								break;
							}
							num2++;
						}
						catch (Exception)
						{
							if (!File.Exists(dataDecoders[num][num2].PackageName) || !File.Exists(dataDecoderWithLanguages[num][num2].PackageName))
							{
								RemovePackage(num, num2);
							}
							i--;
						}
					}
					else
					{
						if (CheckFile(dataDirectories[num][num3], pathName))
						{
							flag = true;
							break;
						}
						num3++;
					}
				}
				if (flag)
				{
					break;
				}
				goto default;
			}
			default:
				nowData = DataType.Null;
				nowFile = null;
				if (nowStream != null)
				{
					nowStream.Close();
				}
				nowStream = null;
				break;
			}
			return nowData;
		}

		private void RemovePackage(int index, int decoderIndex)
		{
			dataDecoders[index].RemoveAt(decoderIndex);
			dataDecoderWithLanguages[index].RemoveAt(decoderIndex);
			int num = 0;
			for (int i = 0; i < dataIsPackages[index].Count; i++)
			{
				if (dataIsPackages[index][i])
				{
					if (num == decoderIndex)
					{
						dataIsPackages[index].RemoveAt(i);
						break;
					}
					num++;
				}
			}
		}

		private bool CheckFile(string directory, string pathName)
		{
			string fileName = Path.GetFileName(pathName);
			string arg = pathName.Substring(0, pathName.Length - fileName.Length);
			string text;
			if (File.Exists(text = Path.Combine(directory, string.Format("{0}{1}\\{2}", arg, _languageExt, fileName))))
			{
				nowData = DataType.File;
				nowFile = text;
				if (nowStream != null)
				{
					nowStream.Close();
				}
				nowStream = null;
				return true;
			}
			if (File.Exists(text = Path.Combine(directory, pathName)))
			{
				nowData = DataType.File;
				nowFile = text;
				if (nowStream != null)
				{
					nowStream.Close();
				}
				nowStream = null;
				return true;
			}
			return false;
		}

		private bool CheckPackage(DataDecoder decoder, DataDecoder decoderWithLanguage, string pathName)
		{
			Stream stream;
			if (decoderWithLanguage != null && (stream = decoderWithLanguage.DecodeStream(pathName)) != null)
			{
				nowData = DataType.Package;
				nowFile = null;
				if (nowStream != null)
				{
					nowStream.Close();
				}
				nowStream = stream;
				return true;
			}
			if ((stream = decoder.DecodeStream(pathName)) != null)
			{
				nowData = DataType.Package;
				nowFile = null;
				if (nowStream != null)
				{
					nowStream.Close();
				}
				nowStream = stream;
				return true;
			}
			return false;
		}

		public string GetFile()
		{
			return nowFile;
		}

		public Stream GetStream()
		{
			Stream result = nowStream;
			nowStream = null;
			return result;
		}

		public void Dispose()
		{
			for (int i = 0; i < dataDecoders.Count; i++)
			{
				for (int j = 0; j < dataDecoders[i].Count; j++)
				{
					dataDecoders[i][j].Dispose();
				}
				dataIsPackages[i].Clear();
				dataDecoders[i].Clear();
				dataDecoderWithLanguages[i].Clear();
				dataDirectories[i].Clear();
			}
			directoryNames.Clear();
			dataNames.Clear();
			dataIsPackages.Clear();
			dataDecoders.Clear();
			dataDecoderWithLanguages.Clear();
			dataDirectories.Clear();
			replaceNames.Clear();
			replaceStreams.Clear();
		}
	}
}
