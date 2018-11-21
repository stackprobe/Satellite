using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;

namespace Charlotte.Satellite.Tools
{
	public class FileTools
	{
		public static void CreateFile(string file)
		{
			File.WriteAllBytes(file, new byte[0]);
		}

		public static void CreateDir(string dir)
		{
			CreateDir_Ex(dir);
		}

		public static bool ExistFile(string file)
		{
			return File.Exists(file);
		}

		public static bool ExistDir(string dir)
		{
			return Directory.Exists(dir);
		}

		public static void DeleteFile(string file)
		{
			DeleteFile_Ex(file);
		}

		public static void DelFile(string file)
		{
			try
			{
				DeleteFile(file);
			}
			catch
			{ }
		}

		public static void DeleteDir(string dir, bool recursive = false)
		{
			if (Directory.Exists(dir) == false)
				return;

			if (recursive == false && 1 <= Directory.GetFileSystemEntries(dir).Length)
				return; // この動作を意図している呼び出し元もあるよ！

			DeleteDir_Ex(dir);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="dir"></param>
		/// <returns>dir 直下のディレクトリとファイルのローカル名の一覧</returns>
		public static List<string> List(string dir)
		{
			List<string> list = new List<string>();

			foreach (string[] paths in new string[][] { Directory.GetDirectories(dir), Directory.GetFiles(dir) })
				foreach (string path in paths)
					list.Add(Path.GetFileName(path));

			return list;
		}

		public static void WriteAllBytes(string file, QueueData<SubBlock> fileData)
		{
			using (FileStream fs = new FileStream(file, FileMode.Create, FileAccess.Write))
			{
				for (; ; )
				{
					SubBlock block = fileData.Poll(null);

					if (block == null)
						break;

					fs.Write(block.Block, block.StartPos, block.Length);
				}
			}
		}

		private const int EX_TRY_MAX = 100;

		public static void CreateDir_Ex(string dir)
		{
			if (Directory.Exists(dir) == false)
			{
				for (int c = 0; c < EX_TRY_MAX; c++)
				{
					try
					{
						Directory.CreateDirectory(dir);

						if (Directory.Exists(dir))
							return;
					}
					catch
					{ }

					Thread.Sleep(c);
				}

				{
					Directory.CreateDirectory(dir);

					if (Directory.Exists(dir))
						return;

					throw new Exception("ディレクトリを作成出来ません。" + dir);
				}
			}
		}

		private static void DeleteFile_Ex(string file)
		{
			if (File.Exists(file))
			{
				for (int c = 0; c < EX_TRY_MAX; c++)
				{
					try
					{
						File.Delete(file);

						if (File.Exists(file) == false)
							return;
					}
					catch
					{ }

					Thread.Sleep(c);
				}

				{
					File.Delete(file);

					if (File.Exists(file) == false)
						return;

					throw new Exception("ファイルを削除出来ません。" + file);
				}
			}
		}

		private static void DeleteDir_Ex(string dir)
		{
			if (Directory.Exists(dir))
			{
				for (int c = 0; c < EX_TRY_MAX; c++)
				{
					try
					{
						Directory.Delete(dir, true);

						if (Directory.Exists(dir) == false)
							return;
					}
					catch
					{ }

					Thread.Sleep(c);
				}

				{
					Directory.Delete(dir, true);

					if (Directory.Exists(dir) == false)
						return;

					throw new Exception("ディレクトリを削除出来ません。" + dir);
				}
			}
		}
	}
}
