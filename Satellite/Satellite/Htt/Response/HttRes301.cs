using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Charlotte.Htt.Response
{
	public class HttRes301 : HttResponse
	{
		private string _location;

		public HttRes301(string location)
		{
			_location = location;
		}

		public string GetHTTPVersion()
		{
			return "HTTP/1.1";
		}

		public int GetStatusCode()
		{
			return 301;
		}

		public string GetReasonPhrase()
		{
			return "Moved Permanently";
		}

		public void WriteHeaderFields(Dictionary<string, string> dest)
		{
			dest.Add("Location", _location);
		}

		public string GetBodyPartFile()
		{
			return null;
		}

		public byte[] GetBodyPart()
		{
			return null;
		}
	}
}
