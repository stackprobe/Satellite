using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Charlotte.Htt.Response
{
	public class HttResHtml : HttResponse
	{
		private string _htmlText;
		private Encoding _encoding;

		public HttResHtml()
			: this("<html><body><h1>Happy tea time!</h1></body></html>")
		{ }

		public HttResHtml(String htmlText)
			: this(htmlText, Encoding.UTF8)
		{ }

		public HttResHtml(String htmlText, Encoding encoding)
		{
			_htmlText = htmlText;
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
			dest.Add("Content-Type", "text/html; charset=" + _encoding.WebName);
		}

		public string GetBodyPartFile()
		{
			return null;
		}

		public byte[] GetBodyPart()
		{
			return _encoding.GetBytes(_htmlText);
		}
	}
}
