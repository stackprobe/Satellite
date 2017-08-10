using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Satellite.Tools;
using System.IO;

namespace Charlotte.Htt.Response
{
	public class HttResFileImage : HttResponse
	{
		private byte[] _fileData;
		private string _virPath;
		private Encoding _encoding;

		public HttResFileImage(byte[] fileData, string virPath)
			: this(fileData, virPath, null)
		{ }

		public HttResFileImage(byte[] fileData, string virPath, Encoding encoding)
		{
			_fileData = fileData;
			_virPath = virPath;
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
			string ret = ExtToContentType.GetContentType(Path.GetExtension(Path.GetFullPath(_virPath)));

			if (_encoding != null)
			{
				ret += "; charset=" + _encoding.WebName;
			}
			return ret;
		}

		public string GetBodyPartFile()
		{
			return null;
		}

		public byte[] GetBodyPart()
		{
			return _fileData;
		}
	}
}
