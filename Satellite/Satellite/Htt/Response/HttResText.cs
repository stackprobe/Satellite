using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Charlotte.Htt.Response
{
	public class HttResText : HttResponse
	{
		private string _text;
		private Encoding _encoding;

		public HttResText()
			: this("Happy tea time!")
		{ }

		public HttResText(String text)
			: this(text, Encoding.UTF8)
		{ }

		public HttResText(String text, Encoding encoding)
		{
			_text = text;
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
			dest.Add("Content-Type", "text/plain; charset=" + _encoding.WebName);
		}

		public string GetBodyPartFile()
		{
			return null;
		}

		public byte[] GetBodyPart()
		{
			return _encoding.GetBytes(_text);
		}
	}
}
