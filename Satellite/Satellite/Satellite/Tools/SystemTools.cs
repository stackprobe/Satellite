using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Charlotte.Satellite.Tools
{
	public static class SystemTools
	{
		public static string GetTmp()
		{
			return GetEnv("TMP", @"C:\temp");
		}

		public static string GetEnv(string name, string defval)
		{
			string value = Environment.GetEnvironmentVariable(name);

			if (string.IsNullOrEmpty(value))
				return defval;

			return value;
		}

		public static void Error(object obj)
		{
			//Console.WriteLine(obj); // todo
		}

		public static bool IsProcessAlive(int pid)
		{
			try
			{
				return Process.GetProcessById(pid) != null;
			}
			catch
			{ }

			return false;
		}

#if false // old -- MoveFileEx() does not works in Windows10 ???
		// ---- MoveFileEx ----

		[Flags]
		public enum MoveFileFlags
		{
			None = 0,
			ReplaceExisting = 1,
			CopyAllowed = 2,
			DelayUntilReboot = 4,
			WriteThrough = 8,
			CreateHardlink = 16,
			FailIfNotTrackable = 32,
		}

		[DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
		public static extern bool MoveFileEx(
			string lpExistingFileName,
			string lpNewFileName,
			MoveFileFlags dwFlags
			);

		// ----
#endif
	}
}
