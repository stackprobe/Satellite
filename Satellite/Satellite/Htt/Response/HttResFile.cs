using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Charlotte.Satellite.Tools;

namespace Charlotte.Htt.Response
{
	public class HttResFile : HttResponse
	{
		private string _file;
		private Encoding _encoding;

		public HttResFile(string file)
			: this(file, Encoding.UTF8)
		{ }

		public HttResFile(string file, Encoding encoding)
		{
			_file = file;
			_encoding = encoding;
		}

		public string GetHTTPVersion()
		{
			return "HTTP/1.1";
		}

		public int GetStatusCode()
		{
			return 200;
		}

		public string GetReasonPhrase()
		{
			return "OK";
		}

		public void WriteHeaderFields(Dictionary<string, string> dest)
		{
			dest.Add("Content-Type", GetContentType());
		}

		private string GetContentType()
		{
			string ret = ExtToContentType.GetContentType(Path.GetExtension(Path.GetFullPath(_file)));

			if (_encoding != null)
			{
				ret += "; charset=" + _encoding.WebName;
			}
			return ret;
		}

		public string GetBodyPartFile()
		{
			return _file;
		}

		public byte[] GetBodyPart()
		{
			return null;
		}
	}
}
