using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace EasyBBS
{
	public static class Utils
	{
		public static string CutTrail(string str, int maxlen)
		{
			if (maxlen < str.Length)
				str = str.Substring(0, maxlen);

			return str;
		}

		public static Dictionary<string, string> ParseQuery(string query)
		{
			Dictionary<string, string> ret = new Dictionary<string, string>();

			foreach (string part in query.Split('&'))
			{
				string[] tokens = part.Split('=');

				if (tokens.Length == 2)
				{
					string key = tokens[0];
					string value = tokens[1];

					key = DecodeUrl(key);
					value = DecodeUrl(value);

					ret.Add(key, value);
				}
			}
			return ret;
		}

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

		public static string EncodeMultiLineText(string text)
		{
			text = text.Replace("\r\n", "\n");
			text = text.Replace("\r", "");

			string[] lines = text.Split('\n');

			for (int index = 0; index < lines.Length; index++)
				lines[index] = Encode(lines[index]);

			return string.Join("<br/>", lines);
		}

		public static string Encode(string str)
		{
			StringBuilder buff = new StringBuilder();

			foreach (char chr in str)
			{
				if (char.IsDigit(chr) || char.IsUpper(chr) || char.IsLower(chr) || 0x00ff < chr)
				{
					buff.Append(chr);
				}
				else
				{
					buff.Append("&#x00");
					buff.Append(((int)chr).ToString("X2"));
					buff.Append(";");
				}
			}
			return buff.ToString();
		}
	}
}
