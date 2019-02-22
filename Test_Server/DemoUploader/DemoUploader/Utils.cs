using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Charlotte.Satellite.Tools;

namespace DemoUploader
{
	public class Utils
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

		private static bool Contains(byte[] chrs, byte target)
		{
			for (int index = 0; index < chrs.Length; index++)
				if (chrs[index] == target)
					return true;

			return false;
		}
	}
}
