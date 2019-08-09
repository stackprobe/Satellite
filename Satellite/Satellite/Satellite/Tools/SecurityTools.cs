using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace Charlotte.Satellite.Tools
{
	public static class SecurityTools
	{
		public static string GetSHA512_128String(string str)
		{
			return GetSHA512String(str).Substring(0, 32);
		}

		public static string GetSHA512String(string str)
		{
			return GetSHA512String(StringTools.ENCODING_SJIS.GetBytes(str));
		}

		public static string GetSHA512String(byte[] data)
		{
			using (SHA512CryptoServiceProvider sha512 = new SHA512CryptoServiceProvider())
			{
				return StringTools.ToHex(sha512.ComputeHash(data));
			}
		}
	}
}
