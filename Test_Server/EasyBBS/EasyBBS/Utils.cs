using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EasyBBS
{
	public class Utils
	{
		public static string CutTrail(string str, int maxlen)
		{
			if (maxlen < str.Length)
				str = str.Substring(0, maxlen);

			return str;
		}

		public static Dictionary<string, string> ParseQuery(string query)
		{
			throw null; // TODO
		}

		public static string Encode(string str)
		{
			throw null; // TODO
		}

		public static string EncodeMultiLineText(string text)
		{
			throw null; // TODO
		}
	}
}
