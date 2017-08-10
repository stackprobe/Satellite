using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Charlotte.Htt
{
	public interface HttResponse
	{
		string GetHTTPVersion();
		int GetStatusCode();
		string GetReasonPhrase();
		void WriteHeaderFields(Dictionary<string, string> dest);
		string GetBodyPartFile();
		byte[] GetBodyPart();
	}
}
