using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Charlotte.Htt.Response
{
	public class HttRes404 : HttResponse
	{
		public string GetHTTPVersion()
		{
			return "HTTP/1.1";
		}

		public int GetStatusCode()
		{
			return 404;
		}

		public string GetReasonPhrase()
		{
			return "Not Found";
		}

		public void WriteHeaderFields(Dictionary<string, string> dest)
		{
			// noop
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
