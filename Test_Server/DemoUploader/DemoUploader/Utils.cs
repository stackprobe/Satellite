using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Charlotte.Satellite.Tools;

namespace DemoUploader
{
	public static class Utils
	{
		public static string DecodeUrl(string str)
		{
			using (MemoryStream mem = new MemoryStream())
			{
				byte[] bStr = Encoding.ASCII.GetBytes(str);

				for (int index = 0; index < bStr.Length; index++)
				{
					byte bChr = bStr[index];

					if (bChr == 0x25) // ? '%'
					{
						bChr = (byte)Convert.ToInt32(Encoding.ASCII.GetString(new byte[] { bStr[index + 1], bStr[index + 2] }), 16);
						index += 2;
					}
					else if (bChr == 0x2b) // ? '+'
					{
						bChr = 0x20; // ' '
					}
					mem.WriteByte(bChr);
				}
				return Encoding.UTF8.GetString(mem.ToArray());
			}
		}

		private static readonly byte[] UNENCODE_CHRS = Encoding.ASCII.GetBytes(
			"0123456789" +
			"ABCDEFGHIJKLMNOPQRSTUVWXYZ" +
			"abcdefghijklmnopqrstuvwxyz" +
			".:/?&="
			);

		private static readonly byte[] B_HEXADECIMAL = Encoding.ASCII.GetBytes(StringTools.hexadecimal);

		public static string EncodeUrl(string str)
		{
			using (MemoryStream buff = new MemoryStream())
			{
				foreach (byte chr in Encoding.UTF8.GetBytes(str))
				{
					if (Contains(UNENCODE_CHRS, chr))
					{
						buff.WriteByte(chr);
					}
					else
					{
						buff.WriteByte((byte)0x25);
						buff.WriteByte(B_HEXADECIMAL[chr >> 4]);
						buff.WriteByte(B_HEXADECIMAL[chr & 0x0f]);
					}
				}
				return Encoding.ASCII.GetString(buff.ToArray());
			}
		}

		private static bool Contains(byte[] text, byte target)
		{
			return IndexOf(text, target) != -1;
		}

		private static int IndexOf(byte[] text, byte target)
		{
			return IndexOf(text, new byte[] { target });
		}

		public static int IndexOf(byte[] text, byte[] target, int start = 0)
		{
			for (int index = start; index + target.Length <= text.Length; index++)
				if (IsSame(text, index, target, 0, target.Length))
					return index;

			return -1;
		}

		private static bool IsSame(byte[] a, int aOffset, byte[] b, int bOffset, int length)
		{
			for (int index = 0; index < length; index++)
				if (a[aOffset + index] != b[bOffset + index])
					return false;

			return true;
		}

		public static byte[] GetRange(byte[] src, int offset, int length)
		{
			byte[] dest = new byte[length];
			Array.Copy(src, offset, dest, 0, length);
			return dest;
		}

		public static void WriteLog(object message)
		{
			Console.WriteLine("[" + DateTime.Now + "] " + message);
		}
	}
}
